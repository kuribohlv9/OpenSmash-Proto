using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveDatabase : MonoBehaviour {
	public Move move;
	public List<Move> moves = new List<Move>();

	void Awake () {
		//load from xml
		moves.Add(new Move());
		move = moves[moves.Count-1];
		move.name = "Punch Left";
		move.hitTime = 0.1f;
		move.duration = 0.6f;
		move.damage = 7.5f;
		move.reach = 0.8f;
		move.forwardForce = 150;

		moves.Add(new Move());
		move = moves[moves.Count-1];
		move.name = "Punch Right";
		move.hitTime = 0.1f;
		move.duration = 0.5f;
		move.damage = 10f;
		move.forwardForce = 180;

		moves.Add(new Move());
		move = moves[moves.Count-1];
		move.name = "Kick";
		move.hitTime = 0.2f;
		move.duration = 0.7f;
		move.damage = 15f;
		move.reach = 1;
		move.forwardForce = 250;
		move.upForce = 200;
		move.restrictAirControl = true;
	}

	public Move Get (string name) {
		for (int i = 0; i < moves.Count; i++)
		{
			if (moves[i].name == name)
			{
				return moves[i];
			}
		}
		return new Move();		
	}
}
