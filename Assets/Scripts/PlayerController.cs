using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
/*
 * IDEASSSSSSSSSSSSSSS
 * FIRING ON THE ROPE???
 * Make rapelling faster the longer the rope is
 */

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

    private bool isGrappled;
    private bool canRetract;
    private bool reelingOut;

    private Vector2 gunDir;
    private Vector3 previousPlayerPos;
    [Header("Player")]
    [SerializeField] private DistanceJoint2D joint;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isGrappled = false;
        canRetract = true;
        reelingOut = false;
    }

    private void FixedUpdate()
    {
        if (joint.connectedBody is null)
            return;
        Vector2 connectedPos = joint.connectedBody.position;
        Vector2 dir = (Vector2) transform.position - connectedPos;
        Debug.Log(dir.magnitude);
        if (dir.magnitude > joint.distance)
        {
            /*
            * remove dir from rb. velocity
            */

            transform.position = connectedPos + dir.normalized * joint.distance;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateGunRot();
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            muzzleFlash.Play();
            ApplyRecoil();
            if(!isGrappled)
                FireGun();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Debug.Log("Releasing Mouse");
            StopCoroutine("ExtendRope");
            if (reelingOut)
            {
                ropeController.InitialExtend();
                ropeController.AttachPlayer();
            }

            reelingOut = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StopCoroutine("ExtendRope");
            ropeController.DeleteRope();
            isGrappled = false;
        }

        if (Input.GetKey(KeyCode.W) && isGrappled)
        {
            if (canRetract)
            {
                canRetract = false;
                Retract();
                StartCoroutine(RetractCooldown());
            }
        }
        if (Input.GetKey(KeyCode.S) && isGrappled)
        {
            if (canRetract)
            {
                canRetract = false;
                ropeController.Extend(true);
                StartCoroutine(RetractCooldown());
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
        ropeController.DeleteRope();
        RaycastHit2D hit =  Physics2D.Raycast(gunPivot.position, gunDir, 200, canHookTo);
        if (hit.collider != null)
        {
            Debug.Log("Hit! : "+hit.transform.gameObject);
            ropeController.CreateRopeTo(hit.point, hit.transform);
            isGrappled = true;
            reelingOut = true;

            StartCoroutine("ExtendRope");
        }
    }

    private void ApplyRecoil()
    {
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
        gunDir = mainCamera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
    }
    #endregion 
}
