using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings 
{
	public string name;

	public Player.TeamColor teamColor
	{
		get;
		private set;
	}

	public Player.Direction direction
	{
		get;
		private set;
	}

	public Player.PlayerControl control
	{
		get;
		private set;
	}

	public List<int> specialIDs = new List<int> ( );

	public int [ ] formation;

	public PlayerSettings ( Player.TeamColor _color, Player.Direction _direction, Player.PlayerControl _control )
	{
		teamColor = _color;
		direction = _direction;
		control = _control;
	}
}
