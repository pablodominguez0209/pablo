using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {

    GameObject Point;
    NavMeshAgent Nav;

    Vector3 target;


    [SerializeField] Animator animator;

	// Use this for initialization
	void Start () {
        Nav = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast (ray,out hitInfo)) {

                //Debug.Log(hitInfo.point);

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = hitInfo.point;
                Destroy(cube, 0.5f);

                Nav.destination = hitInfo.point;
                
             
            }
            animator.SetBool("Walking", true);
            target = hitInfo.point;
        }

        float distance = Vector3.Distance(transform.position, target);

        if (distance < 1.51) {
            animator.SetBool("Walking", false);
        }
    }

   
}
