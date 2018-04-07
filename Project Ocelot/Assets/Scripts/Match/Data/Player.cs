using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{
	#region Enums

	public enum Direction
	{
		LEFT_TO_RIGHT,
		RIGHT_TO_LEFT,
		TOP_LEFT_TO_BOTTOM_RIGHT,
		TOP_RIGHT_TO_BOTTOM_LEFT,
		BOTTOM_LEFT_TO_TOP_RIGHT,
		BOTTOM_RIGHT_TO_TOP_LEFT
	}

	public enum TeamColor
	{
		BLUE = 0,
		GREEN = 1,
		YELLOW = 2,
		ORANGE = 3,
		PINK = 4,
		PURPLE = 5
	}

	public enum PlayerControl
	{
		LOCAL_PLAYER,
		LOCAL_BOT,
		ONLINE_PLAYER,
		ONLINE_BOT
	}

	#endregion // Enums

	#region Player Data

	public string PlayerName;
	public PlayerControl Control;
	public int PlayerOrder;

	#endregion // Player Data

	#region Team Data

	public TeamColor Team;
	public Direction TeamDirection;
	public int [ ] specialIDs;
	public StartArea startArea;
	public List<Unit> units = new List<Unit> ( );
	public Unit.KOdelegate standardKOdelegate;
	public List<TileObject> tileObjects = new List<TileObject> ( );

	#endregion // Team Data
}
