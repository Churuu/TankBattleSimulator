using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualTopCamera;
    public CinemachineFreeLook virtualTankCamera;

    [HideInInspector]
    public bool isControllingTank;

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
            SelectTank();
        }

        if(isControllingTank)
        {
            LookRotation();
            DriveInput();
        }
    }

    void LookRotation()
    {

    }

    void DriveInput()
    {

    }

    void SelectTank()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Tank"))
                {
                    hit.collider.GetComponent<StatePatternTank>().enabled = false;
                    hit.collider.GetComponent<NavMeshAgent>().enabled = false;
                    GetComponent<Camera_Controller>().isControllingTank = true;

                    isControllingTank = true;

                    GameObject selectedTank = hit.collider.gameObject;

                    virtualTankCamera.gameObject.SetActive(true);
                    virtualTopCamera.gameObject.SetActive(false);

                    virtualTankCamera.Follow = selectedTank.transform;
                    virtualTankCamera.LookAt = selectedTank.GetComponent<StatePatternTank>().turret.transform;
                }
            }
        }
    }


    #region Gizmos

    Vector3 fooPos;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(fooPos, 0.3f);
    }

    #endregion
}
