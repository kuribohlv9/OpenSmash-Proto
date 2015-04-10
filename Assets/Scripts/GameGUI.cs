using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {
	public GUISkin skin;
	public PlayerController[] player;

	private Texture2D[] portrait = new Texture2D[4];
	private Texture2D[] portraitBack = new Texture2D[4];
	private Texture2D[] nameBack = new Texture2D[4];

	public int borderX = 100;

	public int _x = 0;
	public int _y = 0;
	public int _width = 170;
	public int _height = 128;

	// Use this for initialization
	void Start () {
		GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
		player = new PlayerController[playerObjects.Length];
		for(int i = 0; i < playerObjects.Length; i++)
		{
			player[i] = playerObjects[i].GetComponent<PlayerController>();
			portrait[i] = Resources.Load<Texture2D>("Textures/" + playerObjects[i].name + " Portrait");
			portraitBack[i] = Resources.Load<Texture2D>("Textures/portrait back " + (i+1));
			print("load "+ "Textures/portrait back " + (i+1));
			nameBack[i] = Resources.Load<Texture2D>("Textures/name back " + (i+1));
		}
	}
	
	void OnGUI () {
		GUI.skin = skin;
		int width = 128;
		int distance = (Screen.width - borderX*2) / 4;
		int height = 30;
		int y = Screen.height - height*3;
		int x = borderX + width/2;
		for (int i = 0; i < player.Length; i++)
		{
			GUI.Label(new Rect(x - 16 + distance * i, y - 40, _width, _height), portraitBack[i]);
			GUI.Label(new Rect(12 - 16 + x + distance * i, 19 + y - 40, _width, _height), portrait[i]);
			GUI.Label(new Rect(x - 16 + distance * i, y - 40, _width, _height), nameBack[i]);
			GUI.Label(new Rect(x + distance * i + 56, y, width, height), (player[i].damage*100).ToString("0") + "%", "Damage Shadow");
			Color32 color = new Color32((byte)(255 - player[i].damage*50), (byte)(255 - player[i].damage*100), (byte)(255 - player[i].damage*100), 1);
			string hex = "#";
			hex += color.r.ToString("x");
			hex += color.g.ToString("x");
			hex += color.b.ToString("x");
			GUI.Label(new Rect(x + distance * i + 56, y, width, height), "<color="+hex+">" + (player[i].damage*100).ToString("0") + "%</color>", "Damage");
			GUI.Label(new Rect(x + distance * i, y + height, width, height), player[i].gameObject.name, "Player Name Shadow");
			GUI.Label(new Rect(x + distance * i, y + height, width, height), player[i].gameObject.name, "Player Name");
			for (int l = 0; l < player[i].lives; l++)
			{
				GUI.Label(new Rect(borderX + width + distance * i + (width/6) * l, y + 55, width, height), "○", "Life");
			}
		}
	}
}
