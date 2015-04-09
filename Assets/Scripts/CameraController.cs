using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	public Transform[] players;
	public int alivePlayers;
	private Vector3 targetPosition = Vector3.zero;


	// Use this for initialization
	void Start () {
		List<Transform> playerTransforms = new List<Transform>();
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		{
			playerTransforms.Add(player.transform);
		}
		players = playerTransforms.ToArray();
		alivePlayers = players.Length;
	}
	
	// Update is called once per frame
	void Update () {
		float distance = 5;

		Vector3 middle = Vector3.zero;
		Vector3 max = Vector3.one * -1000;
		Vector3 min = Vector3.one * 1000;
		for (int i = 0; i < players.Length; i++)
		{
			if (!players[i].GetComponent<PlayerController>().isAlive) continue;
			middle += (players[i].position + Vector3.up) / alivePlayers;
			max = Vector3.Max(GetPlayerPos(i), max);
			min = Vector3.Min(GetPlayerPos(i), min);
		}
		
		distance = Mathf.Clamp(Vector3.Distance(max, min), 2, 20);

		targetPosition = new Vector3(middle.x, middle.y, -distance);

		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*5);
		print(middle);
	}

	Vector3 GetPlayerPos (int index)
	{
		if (index >= players.Length) return Vector3.zero;
		else return players[index].position;
	}
}
