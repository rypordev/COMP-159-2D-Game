using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    [SerializeField] private GameObject ropeSegmentPrefab;
    [SerializeField] private DistanceJoint2D player;
    [SerializeField] private Transform gunBarrel;
    [SerializeField] private float segmentsLength;

    [SerializeField] private Rigidbody2D anchor;

    [SerializeField] private LineRenderer graphic;
    private List<RopeSegment> segments;

    /*private Vector2 gizmoTargetPos;
    private Vector2 gizmoGunBarrel;
    private Vector2 gizmoAnchorToGun;
    */
    // Start is called before the first frame update
    void Start()
    {
        segments = new List<RopeSegment>();
    }

    public void Extend(bool connectPlayer)
    {
        Rigidbody2D previousLink = anchor;
        if (segments.Count > 0)
            previousLink = segments[segments.Count - 1].GetComponent<Rigidbody2D>();
        
        Vector3 pos = player.transform.position;
        RopeSegment segment = Instantiate(ropeSegmentPrefab, pos, Quaternion.identity).GetComponent<RopeSegment>();
        segment.SetSegmentLength(segmentsLength);
        segment.Link(previousLink);
        segments.Add(segment);

        if (connectPlayer)
            player.connectedBody = segment.GetComponent<Rigidbody2D>();
    }
    
    public void Retract()
    {
        if (segments.Count < 1)
            return;

        int index = segments.Count - 1;
        DistanceJoint2D toDelete = segments[index].GetJoint();
        player.connectedBody = toDelete.connectedBody;
        segments.RemoveAt(index);
        Destroy(toDelete.gameObject);
    }

    public void CreateRopeTo(Vector2 pos)
    {
        //gizmoGunBarrel = gunBarrel.position;
        //gizmoTargetPos = pos;
        //Debug.Log("CREATING ROPE: Anchor - "+pos+ ",  Barrel - "+(Vector2)gunBarrel.position);
        //Set Anchor
        anchor.gameObject.SetActive(true);
        anchor.position = pos;
        
        //Create rope segments - 1, because player is last segment
        float ropeLength = ((Vector2) gunBarrel.position - pos).magnitude;
        int numSegments = Mathf.CeilToInt(ropeLength / segmentsLength) - 1;
        numSegments = Mathf.Clamp(numSegments, 0, int.MaxValue);
        
        //Debug.Log("RopeLength: "+ropeLength+", numSegs: "+numSegments+", ");

        Vector2 gunToAnchor = anchor.position - (Vector2)gunBarrel.position;
        //gizmoAnchorToGun = gunToAnchor;
        Rigidbody2D previousLink = anchor;
        for (int i = 0; i < numSegments; i++)
        {
            gunToAnchor -= gunToAnchor.normalized * segmentsLength;
            RopeSegment segment = Instantiate(ropeSegmentPrefab, gunToAnchor + (Vector2)gunBarrel.position, Quaternion.identity).GetComponent<RopeSegment>();
            segment.SetSegmentLength(segmentsLength);
            segment.Link(previousLink);
            previousLink = segment.GetComponent<Rigidbody2D>();
            segments.Add(segment);
        }
        
        graphic.enabled = true;
    }

    public void AttachPlayer()
    {
        Debug.Log("Segment Count: " + segments.Count);
        player.connectedBody = segments.Count > 0 ? segments[segments.Count-1].GetComponent<Rigidbody2D>() : anchor;
    }

    private void Update()
    {
        UpdateGraphic();
    }

    private void UpdateGraphic()
    {
        if (!graphic.enabled)
            return;

        graphic.positionCount = segments.Count + 2;
        graphic.SetPosition(0, anchor.position);
        for (int i = 0; i < segments.Count; i++)
        {
            graphic.SetPosition(i+1, segments[i].transform.position);
        }
        graphic.SetPosition(segments.Count + 1, player.transform.position);
    }

    public void DeleteRope()
    {
        player.enabled = false;
        anchor.gameObject.SetActive(false);
        foreach (RopeSegment segment in segments)
        {
                Destroy(segment.transform.root.gameObject);
        }
        segments = new List<RopeSegment>();
        
        graphic.enabled = false;
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gizmoGunBarrel, 0.1f);
        Gizmos.DrawSphere(gizmoTargetPos, 0.1f);
        Gizmos.DrawLine(gizmoGunBarrel, gizmoGunBarrel+gizmoAnchorToGun);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(gizmoGunBarrel, gizmoAnchorToGun + gizmoGunBarrel);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(gizmoGunBarrel, gizmoTargetPos - gizmoGunBarrel + gizmoGunBarrel);
    }*/
}
