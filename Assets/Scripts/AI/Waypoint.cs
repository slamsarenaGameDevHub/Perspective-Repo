using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Transform> nodes;
    [SerializeField] Color lineColor=Color.yellow,nodeColor = Color.red;

    [SerializeField] bool _showOnSelected = false;
    [SerializeField] LayerMask GroundLayerMask;
    Vector3 groundPos;
    private void OnDrawGizmosSelected()
    {
        if (!_showOnSelected) return;
        Transform[] waypoints = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();


        for(int i = 0;i < waypoints.Length; i++)
        {
            if (waypoints[i].transform!=transform)
            {
                nodes.Add(waypoints[i]);
            }
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            RaycastHit hit;
            foreach (Transform node in nodes)
            {
                if(node!=transform)
                {
                    node.gameObject.layer = 2;
                    BoxCollider col=node.GetComponent<BoxCollider>();
                    if (col != null) return;
                    BoxCollider collider=node.gameObject.AddComponent<BoxCollider>();
                    collider.isTrigger = true;
                }
                if(Physics.Raycast(node.transform.position,-node.transform.up,out hit,1000,GroundLayerMask))
                {
                    groundPos=new Vector3(node.position.x,hit.point.y,node.position.z);
                    node.position = groundPos;
                }
            }
            Vector3 CurrentPosition = nodes[i].position;
            Vector3 previousPosition = Vector3.zero;
            if(i>0)
            {
                previousPosition = nodes[i-1].position;
            }
            else
            {
                previousPosition = nodes[nodes.Count-1].position;
            }
            Gizmos.color = lineColor;
            Gizmos.DrawLine(previousPosition, CurrentPosition);
            Gizmos.color=nodeColor;
            Gizmos.DrawWireSphere(CurrentPosition, 3);
        }
    }
    private void OnDrawGizmos()
    {
        if (_showOnSelected) return;
        Transform[] waypoints = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i].transform != transform)
            {
                nodes.Add(waypoints[i]);
            }
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            RaycastHit hit;
            foreach (Transform node in nodes)
            {
                if (Physics.Raycast(node.transform.position, -node.transform.up, out hit, 1000, GroundLayerMask))
                {
                    groundPos = new Vector3(node.position.x, hit.point.y, node.position.z);
                    node.position = groundPos;
                }
            }
            Vector3 CurrentPosition = nodes[i].position;
            Vector3 previousPosition = Vector3.zero;
            if (i > 0)
            {
                previousPosition = nodes[i - 1].position;
            }
            else
            {
                previousPosition = nodes[nodes.Count - 1].position;
            }
            Gizmos.color = lineColor;
            Gizmos.DrawLine(previousPosition, CurrentPosition);
            Gizmos.color = nodeColor;
            Gizmos.DrawWireSphere(CurrentPosition, 3);
        }
    }
}
