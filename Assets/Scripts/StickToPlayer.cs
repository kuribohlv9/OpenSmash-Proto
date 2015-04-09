using UnityEngine;
using System.Collections;

public class StickToPlayer : MonoBehaviour {
	public Transform player;
	private CapsuleCollider capsCol;
	// Update is called once per frame
	void Update () {
		if (player != null)
		{
			if (capsCol == null) capsCol = player.GetComponent<CapsuleCollider>();
			transform.position = player.position + Vector3.up * capsCol.height;
		}
	}
}
