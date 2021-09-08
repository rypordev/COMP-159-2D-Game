using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [Header("Gun")]
    [SerializeField] private RopeController ropeController;
    [SerializeField] private MuzzleFlash muzzleFlash;
    [SerializeField] private RectTransform gunPivot;
    [SerializeField] private LayerMask canHookTo;
    [SerializeField] private float playerRecoil;
    [SerializeField] private float retractCooldown;

    private Rigidbody2D rb;

    private bool canFire;
    private bool canRetract;

    private Vector3 previousPlayerPos;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canFire = true;
        canRetract = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGunRot();
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Left Mouse Pressed");
            if (canFire)
            {
                FireGun();
            }
            else
            {
                ropeController.DeleteRope();
                canFire = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Debug.Log("Releasing Mouse");
            StopCoroutine("ExtendRope");
            ropeController.AttachPlayer();
        }

        //Debug.Log("Scroll: " + Input.mouseScrollDelta);
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (canRetract)
            {
                canRetract = false;
                Retract();
                StartCoroutine(RetractCooldown());
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                ropeController.Extend(true);
            }
        }
    }

    #region Retract

    public void Retract()
    {
        ropeController.Retract();
    }

    private IEnumerator RetractCooldown()
    {
        yield return new WaitForSeconds(retractCooldown);
        canRetract = true;
    }

    #endregion
    
    #region FireGun
    private void FireGun()
    {
        muzzleFlash.Play();
        
        ropeController.DeleteRope();
        Vector2 gunDir = mainCamera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        RaycastHit2D hit =  Physics2D.Raycast(gunPivot.position, gunDir, 200, canHookTo);
        if (hit.collider != null)
        {
            Debug.Log("Hit! : "+hit.transform.gameObject);
            ropeController.CreateRopeTo(hit.point);
            canFire = false;

            StartCoroutine("ExtendRope");
        }
        
        rb.AddForce(gunDir * -playerRecoil, ForceMode2D.Impulse);
    }

    private IEnumerator ExtendRope()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            ropeController.InitialExtend();
        }
    }
    
    private void UpdateGunRot()
    {
        Vector2 gunPivotPosition = gunPivot.position;
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        bool mouseIsRight = (mousePos.x > gunPivotPosition.x);
        gunPivot.localScale = new Vector3(mouseIsRight? 1:-1, 1, 1);

        float angle = Vector2.SignedAngle(new Vector2(1, 0), (mousePos - gunPivotPosition));

        if (!mouseIsRight)
            angle += 180;
        
        gunPivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    #endregion 
}
