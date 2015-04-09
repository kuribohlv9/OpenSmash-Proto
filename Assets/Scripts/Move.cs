using UnityEngine;
using System.Collections;

[System.Serializable]
public class Move {
	public string name;
	public float hitTime; 	//after what time the target is hit?
	public float duration; 	//how long does the whole action take
	public float damage;
	public float reach = 0.5f;
	public int upForce = 0;
	public int forwardForce = 0;
	public float restrictAirControl = 0;

	public Move () {
	}
}