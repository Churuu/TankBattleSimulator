using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    public NavMeshAgent tank;

    public Camera cam;

    public bool hasMovePos;
    bool m_IsRotating = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputBehavior();
    }

    void UpdateInputBehavior()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryToMoveTank();
        }
    }

    void TryToMoveTank()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitpoint = hit.point;
            Vector3 direction = hitpoint - tank.transform.position;
            direction.y = 0;


            //Waste of my time since it didnt solve my problem at first but cant bother removing it now
            float degreesToRotate = GetAngle(direction, tank.transform.forward);

            //Why has nobody taught me this before!?
            Vector3 cross = Vector3.Cross(tank.transform.forward, direction);

            if (!m_IsRotating)
            {
                tank.isStopped = true;
                m_IsRotating = true;
                LeanTween.rotateY(tank.gameObject, tank.transform.eulerAngles.y + (cross.y > 0 ? degreesToRotate : -degreesToRotate), 1f).setOnComplete(() => MoveTank(hitpoint));
            }
        }
    }

    //Move the tank
    void MoveTank(Vector3 position)
    {
        m_IsRotating = false;
        tank.isStopped = false;
        tank.SetDestination(position);
    }

    //Calulate the dot product for two vectors
    float DotProduct(Vector3 dot1, Vector3 dot2)
    {
        float value1 = dot1.x * dot2.x;
        float value2 = dot1.y * dot2.y;
        float value3 = dot1.z * dot2.z;
        float dotproduct = value1 + value2 + value3;

        return dotproduct;
    }

    //Calculate the angle between two vectors
    float GetAngle(Vector3 vec1, Vector3 vec2)
    {
        float dot = DotProduct(vec1, vec2);

        float angle = dot / (vec1.magnitude * vec2.magnitude);
        return Mathf.Rad2Deg * Mathf.Acos(angle);
    }
}
