using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ProjectOcelot.Match
{
	public class TurnHUD : MonoBehaviour
	{
		#region UI Elements

		[SerializeField]
		private TextMeshProUGUI roundText;

		[SerializeField]
		private TextMeshProUGUI turnText;

		#endregion // UI Elements

		#region Public Functions

		/// <summary>
		/// Displays the current round and turn in the HUD.
		/// </summary>
		/// <param name="round"> The current round number. </param>
		/// <param name="turn"> The player of the current turn. </param>
		public void DisplayTurn ( int round, Player turn )
		{
			// Display current round
			roundText.text = "Round " + round;

			// Display current turn
			turnText.text = turn.PlayerName + "'s Turn";
			turnText.color = Tools.Util.TeamColor ( turn.Team );
		}

		#endregion // Public Functions
	}
}