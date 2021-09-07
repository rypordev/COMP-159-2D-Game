using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    [SerializeField] private DistanceJoint2D joint;
    // Start is called before the first frame update

    public DistanceJoint2D GetJoint()
    {
        return joint;
    }
    
    public void SetSegmentLength(float length)
    {
        joint.distance = length;
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
