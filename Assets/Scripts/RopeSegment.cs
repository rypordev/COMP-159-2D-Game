using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RopeSegment : MonoBehaviour
{
    [SerializeField] private DistanceJoint2D joint;
    [SerializeField] private Rigidbody2D rb;

    public bool retract;
    // Start is called before the first frame update

    public DistanceJoint2D GetJoint()
    {
        return joint;
    }

    public void SetSegmentLength(float length)
    {
        joint.distance = length;
    }

    public void RestrictJointDistance()
    {
        if (!retract)
            return;
        Vector2 connectedPos = joint.connectedBody.position;
        Vector2 dir = (Vector2) transform.position - connectedPos;
        Debug.Log(dir.magnitude);
        if (dir.magnitude > joint.distance)
        {
            /*
             * remove dir from rb. velocity
             */
            transform.position = connectedPos + dir.normalized * Mathf.Clamp(joint.distance*4, 0,dir.magnitude);
        }
    }

    public void Link(Rigidbody2D rb)
    {
        joint.connectedBody = rb;
        joint.enabled = true;
    }

    public void UnLink()
    {
        joint.enabled = false;
    }
}
