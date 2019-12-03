using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ProjectOcelot.Menues
{
	public class MatchInfoMenu : Menu
	{
		#region UI Elements

		[SerializeField]
		private TextMeshProUGUI matchPrompt;

		[SerializeField]
		private Match.HUD.PlayerHUD [ ] playerHUDs;

		#endregion // UI Elements

		#region Public Functions

		/// <summary>
		/// Initializes the Player HUDs for the players.
		/// </summary>
		/// <param name="players"> The list of players in the match. </param>
		public void Initialize ( Match.Player [ ] players )
		{
			// Display match info
			matchPrompt.text = Match.MatchSettings.MatchDebate.EventName;
			switch ( Match.MatchSettings.Type )
			{
			case Match.MatchType.Classic:
			case Match.MatchType.CustomClassic:
				matchPrompt.text += "\n<size=60%><color=#FFD24BFF>Classic Match";
				break;
			case Match.MatchType.Inferno:
			case Match.MatchType.CustomInferno:
				matchPrompt.text += "\n<size=60%><color=#FFD24BFF>Mirror Match";
				break;
			case Match.MatchType.Rumble:
			case Match.MatchType.CustomRumble:
				matchPrompt.text += "\n<size=60%><color=#FFD24BFF>Rumble Match";
				break;
			case Match.MatchType.Control:
			case Match.MatchType.CustomControl:
				matchPrompt.text += "\n<size=60%><color=#FFD24BFF>Ladder Match";
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
		public Match.HUD.PlayerHUD GetPlayerHUD ( Match.Player p )
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
		public Match.HUD.PlayerHUD GetPlayerHUD ( Units.Unit u )
		{
			// Return the matching HUD
			for ( int i = 0; i < playerHUDs.Length; i++ )
				if ( playerHUDs [ i ].CheckForUnit ( u.InstanceID ) )
					return playerHUDs [ i ];

			// Return that the HUD was not found
			return null;
		}

		#endregion // Public Functions
	}
}