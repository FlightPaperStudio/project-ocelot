using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings 
{
	public string name;

	public Player.TeamColor TeamColor
	{
		get;
		private set;
	}

	public Player.Direction Direction
	{
		get;
		private set;
	}

	public Player.PlayerControl Control
	{
		get;
		private set;
	}

	public List<int> heroIDs = new List<int> ( );

	public int [ ] formation;

	public PlayerSettings ( Player.TeamColor _color, Player.Direction _direction, Player.PlayerControl _control )
	{
		TeamColor = _color;
		Direction = _direction;
		Control = _control;
	}
}
