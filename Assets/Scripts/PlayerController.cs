using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [Header("Gun")]
    [SerializeField] private RopeController ropeController;
    [SerializeField] private MuzzleFlash muzzleFlash;
    [SerializeField] private RectTransform gunPivot;
    [SerializeField] private LayerMask canHookTo;
    [SerializeField] private float playerRecoil;

    private Rigidbody2D rb;

    private bool canFire;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGunRot();
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && canFire)
        {
            Debug.Log("Left Mouse Pressed");
            FireGun();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Retract();
        }
    }

    #region Retract

    public void Retract()
    {
        ropeController.Retract();
    }

    #endregion
    
    #region FireGun
    private void FireGun()
    {
        muzzleFlash.Play();
        
        ropeController.DeleteRope();
        Vector2 gunDir = camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        RaycastHit2D hit =  Physics2D.Raycast(gunPivot.position, gunDir, 200, canHookTo);
        if (hit.collider != null)
        {
            Debug.Log("Hit! : "+hit.transform.gameObject);
            ropeController.CreateRopeTo(hit.point);
        }
        
        rb.AddForce(gunDir * -playerRecoil, ForceMode2D.Impulse);
    }

    private void UpdateGunRot()
    {
        Vector2 gunPivotPosition = gunPivot.position;
        Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);

        bool mouseIsRight = (mousePos.x > gunPivotPosition.x);
        gunPivot.localScale = new Vector3(mouseIsRight? 1:-1, 1, 1);

        float angle = Vector2.SignedAngle(new Vector2(1, 0), (mousePos - gunPivotPosition));

        if (!mouseIsRight)
            angle += 180;
        
        gunPivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    #endregion 
}
