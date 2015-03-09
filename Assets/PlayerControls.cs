using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

   
	// Use this for initialization
	void Start () {
        
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetButtonDown("P1_A")){
            Debug.Log("A");
    }
        if(Input.GetButtonDown("P1_B")){
            Debug.Log("B");
    }
        if(Input.GetButtonDown("P1_X")){
            Debug.Log("X");
    }
        if (Input.GetButtonDown("P1_Y"))
        {
            Debug.Log("Y");
    }
	}
}
