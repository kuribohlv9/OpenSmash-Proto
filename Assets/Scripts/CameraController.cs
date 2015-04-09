using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	private float shake = 0;
	public float shakeFrequency = 50;
	public float shakeIntensity = 0.35f;
	public float shakeDuration = 0.25f;

	public Transform[] players;
	public int alivePlayers;
	public float distance = 3;
	public float distanceMulti = 1;
	public float m_MinDistance = 5;
	public float m_MaxDistance = 20;
	public float minY = 0.5f;
	public float maxY = 10;

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
			

			if (shake > 0)
			{
				transform.position = new Vector3(transform.position.x + Mathf.Sin(Time.realtimeSinceStartup*shakeFrequency) * shakeIntensity, transform.position.y, transform.position.z);
				shake = Mathf.Clamp(shake - Time.deltaTime, 0, 2);
			}
			else
			{
				Vector3 middle = Vector3.zero;
				Vector3 max = Vector3.one * -1000;
				Vector3 min = Vector3.one * 1000;
				for (int i = 0; i < players.Length; i++)
				{
					if (!players[i].GetComponent<PlayerController>().isAlive) continue;
					middle += (players[i].position + Vector3.up + (players[i].GetComponent<Rigidbody>().velocity / 4)) / alivePlayers;
					max = Vector3.Max(GetPlayerPos(i), max);
					min = Vector3.Min(GetPlayerPos(i), min);
				}			
				distance = Mathf.Clamp(Vector3.Distance(max, min) * distanceMulti, m_MinDistance, m_MaxDistance);
				targetPosition = new Vector3(middle.x, Mathf.Clamp(middle.y, minY, maxY), -distance);
				transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*5);
			}
	}

	Vector3 GetPlayerPos (int index)
	{
		if (index >= players.Length) return Vector3.zero;
		else return players[index].position;
	}

	public void Shake () {
		shake = shakeDuration;
	}
}