using UnityEngine;
using System.Collections;

public class GrabLedge : MonoBehaviour
{
	void OnTriggerEnter (Collider col)
	{
		print((int)col.transform.rotation.y);
		if (col.tag == "Player" && col.GetComponent<Rigidbody>().velocity.y < 0 && (int)col.transform.eulerAngles.y == (int)transform.eulerAngles.y)
		{
			col.GetComponent<PlayerController>().GrabLedge(true);
			CapsuleCollider capCol = col.GetComponent<CapsuleCollider>();
			if ((int)transform.eulerAngles.y == 90)
			{
				capCol.transform.position = new Vector3(transform.position.x - 0.2f - capCol.radius, transform.position.y - 0.2f - capCol.height, 0);
			}
			else
			{
				capCol.transform.position = new Vector3(transform.position.x + 0.2f + capCol.radius, transform.position.y - 0.2f - capCol.height, 0);
			}
		}
	}
}
