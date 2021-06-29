using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LineHandler : MonoBehaviour
{

    LineRenderer lineRenderer;

    [SerializeField]
    List<GameObject> nodePositions = new List<GameObject>();

    [SerializeField]
    float m_MaxCastDistance, m_Radius, m_LineMovementSpeed;



    //Used for gizmos drawing
    Vector3 g_Tank;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            GameObject midPointGameObject = new GameObject();
            midPointGameObject.transform.position = lineRenderer.GetPosition(i);
            nodePositions.Insert(i, midPointGameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveLine();
        UpdateLine();
    }


    //Update the current line with a new one that has more vertexes;
    void UpdateLine()
    {
        for (int i = 0; i < nodePositions.Count - 1; i += 2)
        {

            //Gets position i and i+1
            var pos1 = nodePositions[i];
            var pos2 = nodePositions[i + 1];

            //Measures the distances between pos1 and pos2
            float distance = Vector3.Distance(pos1.transform.position, pos2.transform.position);

            //If the distance is greater than 1 unit then
            if (distance > 1f)
            {
                //Calculate the position in-between both positions
                var midPoint = (pos1.transform.position + pos2.transform.position) / 2;
                //Create a new node in-between node i and i+1, i+1 is moved forward 1 position when the new node is inserted at i+1, so it becomes i+2
                GameObject midPointGameObject = new GameObject();
                midPointGameObject.transform.position = midPoint;
                nodePositions.Insert(i + 1, midPointGameObject);
            }
            //If distance is less than 0.5 units remove it to save memory
            else if (distance < 0.5f)
            {
                var obj = nodePositions[i + 1];
                nodePositions.RemoveAt(i + 1);
                Destroy(obj);
            }
        }

        // Update the linerenderer size to match nodePositions
        lineRenderer.positionCount = nodePositions.Count;
        for (int j = 0; j < lineRenderer.positionCount; j++)
        {
            //Re-render the entire line with the new coordinates and nodes.
            lineRenderer.SetPosition(j, nodePositions[j].transform.position);
        }
    }



    void MoveLine()
    {
        for (int i = 0; i < nodePositions.Count; i++)
        {
            //Cast a sphere around the node to check for collisions
            Collider[] hitTargets = SphereCast(nodePositions[i].transform.position);
            foreach (var hit in hitTargets)
            {
                //If hit nothing move on to the next vertex
                if (hit == null)
                    continue;

                Debug.Log(hit.name);

                //If hit check if collision is tank
                if (hit.CompareTag("Tank"))
                {
                    //Move node in oposit direction of tank
                    g_Tank = hit.transform.position;
                    var tankDirection = nodePositions[i].transform.position - hit.transform.position;
                    tankDirection.y = 0;
                    nodePositions[i].transform.Translate(tankDirection * Time.deltaTime * m_LineMovementSpeed);
                }
            }
        }
    }

    //Cast a sphere at the given position to check for collision
    Collider[] SphereCast(Vector3 position)
    {
        Collider[] hit;
        hit = Physics.OverlapSphere(position, m_Radius);
        return hit;
    }


    private void OnDrawGizmos()
    {
        foreach (var node in nodePositions)
        {
            Gizmos.DrawWireSphere(node.transform.position, m_Radius);
            var dir = node.transform.position - g_Tank;
            Gizmos.DrawRay(node.transform.position, dir);
        }

    }
}
