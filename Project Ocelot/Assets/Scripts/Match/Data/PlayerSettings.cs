using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings 
{
	public string PlayerName;
	public int TurnOrder;
	public Player.TeamColor Team;
	public Player.Direction TeamDirection;
	public Player.PlayerControl Control;
	public List<UnitSettingData> Units = new List<UnitSettingData> ( );
	public Dictionary<UnitSettingData, int> UnitFormation = new Dictionary<UnitSettingData, int> ( );
	public int [ ] Formation;
}
