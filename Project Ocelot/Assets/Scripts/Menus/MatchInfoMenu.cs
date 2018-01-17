using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchInfoMenu : Menu
{
	#region UI Elements

	public TextMeshProUGUI matchTypeText;
	public PlayerHUD [ ] playerHUDs;

	#endregion // UI Elements

	#region Public Functions

	/// <summary>
	/// Initializes the Player HUDs for the players.
	/// </summary>
	/// <param name="players"> The list of players in the match. </param>
	public void Initialize ( Player [ ] players )
	{
		// Display match type
		switch ( MatchSettings.type )
		{
		case MatchType.Classic:
		case MatchType.CustomClassic:
			matchTypeText.text = "Classic Match";
			break;
		case MatchType.Mirror:
		case MatchType.CustomMirror:
			matchTypeText.text = "Mirror Match";
			break;
		case MatchType.Rumble:
		case MatchType.CustomRumble:
			matchTypeText.text = "Rumble Match";
			break;
		}

		// Initialize each player HUD
		for ( int i = 0; i < playerHUDs.Length; i++ )
			playerHUDs [ i ].Initialize ( players [ i ] );
	}

	/// <summary>
	/// Gets the Player HUD for a particular player.
	/// Returns null if a match is not found.
	/// </summary>
	/// <param name="p"> The player whose Player HUD is being retrieved. </param>
	/// <returns> The Player HUD containing the provided player. </returns>
	public PlayerHUD GetPlayerHUD ( Player p )
	{
		// Return the matching HUD
		for ( int i = 0; i < playerHUDs.Length; i++ )
			if ( playerHUDs [ i ].Player == p )
				return playerHUDs [ i ];

		// Return that the HUD was not found
		return null;
	}

	/// <summary>
	/// Gets the Player HUD for a particular unit.
	/// Returns null if a match is not found.
	/// </summary>
	/// <param name="u"> The unit who is contained in the Player HUD being retrieved. </param>
	/// <returns> The Player HUD containing the provided unit. </returns>
	public PlayerHUD GetPlayerHUD ( Unit u )
	{
		// Return the matching HUD
		for ( int i = 0; i < playerHUDs.Length; i++ )
			if ( playerHUDs [ i ].CheckForUnit ( u.instanceID ) )
				return playerHUDs [ i ];

		// Return that the HUD was not found
		return null;
	}

	#endregion // Public Functions
}
