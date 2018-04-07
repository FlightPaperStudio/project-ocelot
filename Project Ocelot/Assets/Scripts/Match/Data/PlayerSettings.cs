using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings 
{
	public string PlayerName;
	public Player.TeamColor Team;
	public Player.Direction TeamDirection;
	public Player.PlayerControl Control;
	public List<UnitDefaultData> Units = new List<UnitDefaultData> ( );
	public int [ ] Formation;


	public List<int> heroIDs = new List<int> ( );

	public PlayerSettings ( Player.TeamColor _color, Player.Direction _direction, Player.PlayerControl _control )
	{
		Team = _color;
		TeamDirection = _direction;
		Control = _control;
	}
}
