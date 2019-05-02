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
		PURPLE = 5,
		NO_TEAM = 6
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
	public int TurnOrder;

	#endregion // Player Data

	#region Team Data

	public TeamColor Team;
	public Direction TeamDirection;
	public EntranceArea Entrance;
	public ObjectiveArea Objective;
	public List<UnitSettingData> Units = new List<UnitSettingData> ( );
	public List<Unit> UnitInstances = new List<Unit> ( );
	public List<int> StarterInstanceIDs = new List<int> ( );
	public Unit.KOdelegate StandardKOdelegate;
	public List<TileObject> TileObjects = new List<TileObject> ( );

	/// <summary>
	/// Whether or not the player is eliminated from the match.
	/// </summary>
	public bool IsEliminated
	{
		get
		{
			// Check for any remaining units
			return UnitInstances.Count == 0;
		}
	}

	/// <summary>
	/// Whether or not the player is controlled by bot.
	/// </summary>
	public bool IsBot
	{
		get
		{
			// Check for local or online bot
			return Control == PlayerControl.LOCAL_BOT || Control == PlayerControl.ONLINE_BOT;
		}
	}

	#endregion // Team Data
}
