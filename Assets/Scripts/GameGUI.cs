using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	public List<PlayerController> player = new List<PlayerController>();

	// Use this for initialization
	void Start () {
		foreach(GameObject p in GameObject.FindGameObjectsWithTag("Player"))
		{
			player.Add(p.GetComponent<PlayerController>());
		}
	}
	
	void OnGUI () {
		int border = 20;
		int width = (Screen.width - border*2) / 4;
		int height = 24;
		int y = Screen.height - border - height*3;
		for (int i = 0; i < player.Count; i++)
		{
			GUI.Label(new Rect(border + width * i, y, width, height), "Player " + i);
			GUI.Label(new Rect(border + width * i, y + height, width, height), (player[i].damage*100).ToString("0") + "%");
			for (int l = 0; l < player[i].lives; l++)
			{
				GUI.Label(new Rect(border + width * i + (width/16) * l, y + height*2, width, height), "○");
			}
		}
	}
}
