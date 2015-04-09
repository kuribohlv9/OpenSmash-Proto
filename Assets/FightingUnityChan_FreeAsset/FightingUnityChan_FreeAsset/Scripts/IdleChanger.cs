﻿using UnityEngine;
using System.Collections;

//
// アニメーション簡易プレビュースクリプト
// 2014/12/07 quad arrow
//

// Require these components when using this script
[RequireComponent(typeof(Animator))]

public class IdleChanger : MonoBehaviour
{
	
	private Animator anim;						// Animatorへの参照
	private AnimatorStateInfo currentState;		// 現在のステート状態を保存する参照
	private AnimatorStateInfo previousState;	// ひとつ前のステート状態を保存する参照


	// Use this for initialization
	void Start ()
	{
		// 各参照の初期化
		anim = GetComponent<Animator> ();
		currentState = anim.GetCurrentAnimatorStateInfo (0);
		previousState = currentState;

	}



	void OnGUI()
	{	
		GUI.Box(new Rect(Screen.width - 200 , 45 ,120 , 290), "");
		if(GUI.Button(new Rect(Screen.width - 190 , 60 ,100, 20), "Jab"))
			anim.SetBool ("Jab", true);
		if(GUI.Button(new Rect(Screen.width - 190 , 90 ,100, 20), "Jab Right"))
			anim.SetBool ("Jab Right", true);
		if(GUI.Button(new Rect(Screen.width - 190 , 120 ,100, 20), "Hikick"))
			anim.SetBool ("Hikick", true);
		if(GUI.Button(new Rect(Screen.width - 190 , 150 ,100, 20), "Spinkick"))
			anim.SetBool ("Spinkick", true);
		if(GUI.Button(new Rect(Screen.width - 190 , 180 ,100, 20), "Rising_P"))
			anim.SetBool ("Rising_P", true);
		if(GUI.Button(new Rect(Screen.width - 190 , 210 ,100, 20), "Headspring"))
			anim.SetBool ("Headspring", true);
		if(GUI.Button(new Rect(Screen.width - 190 , 240 ,100, 20), "Land"))
			anim.SetBool ("Land", true);
		if(GUI.Button(new Rect(Screen.width - 190 , 270 ,100, 20), "Run"))
			anim.SetBool ("Run", true);
		if(GUI.Button(new Rect(Screen.width - 190 , 300 ,100, 20), "Run_end"))
			anim.SetBool ("Run", false);

;
	}
}