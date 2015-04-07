using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour {
	public List<Controller> player = new List<Controller>();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 4; i++)
		{
			if (GameObject.Find("P" + i))
			{
				player.Add(GameObject.Find("P" + i).GetComponent<Controller>());
			}
		}
	}
	
	void OnGUI () {
		int border = 20;
		int width = (Screen.width - border*2) / 4;
		int height = 24;
		int y = Screen.height - border - height*2;
		for (int i = 0; i < player.Count; i++)
		{
			GUI.Label(new Rect(border + width * i, y, width, height), "Player " + i);
			GUI.Label(new Rect(border + width * i, y + height, width, height), (player[i].damage*100).ToString("0") + "%");
		}
	}
}
