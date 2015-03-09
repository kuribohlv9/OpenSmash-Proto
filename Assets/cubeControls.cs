using UnityEngine;
using System.Collections;

public class cubeControls : MonoBehaviour {

    Rigidbody rb;

	// Use this for initialization
	void Start () {
	 rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        // Move Cube Left If J Is Down.
	    if(Input.GetKeyDown(KeyCode.J))
        {
            rb.velocity = new Vector3(-3,0,0);
        }

          // Move Cube Right If L Is Down.
	    if(Input.GetKeyDown(KeyCode.L))
        {
            rb.velocity = new Vector3(3,0,0);
        }

          // Move Cube Up If I Is Down.
	    if(Input.GetKeyDown(KeyCode.I))
        {
            rb.velocity = new Vector3(0,3,0);;
        }

          // Move Cube Down If K Is Down.
	    if(Input.GetKeyDown(KeyCode.K))
        {
            rb.velocity = new Vector3(0,-3,0);
        }
	}
}
