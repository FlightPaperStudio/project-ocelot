 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectOcelot.Match.Setup
{
	public class DebateMenu : Menues.Menu
	{
		#region UI Elements

		[SerializeField]
		private TextMeshProUGUI debateText;

		[SerializeField]
		private UI.UnitPortrait [ ] leaders;

		[SerializeField]
		private Image opinionBorder;

		[SerializeField]
		private TextMeshProUGUI opinionText;

		[SerializeField]
		private TextMeshProUGUI teamNameText;

		#endregion // UI Elements

		#region Menu Data

		[SerializeField]
		private TeamSetup setupManager;

		[SerializeField]
		private TeamSelectionMenu teamSelectionMenu;

		private Player.TeamColor selectedTeam = Player.TeamColor.NO_TEAM;

		private const float PORTRAIT_OFFSET = 5f;

		#endregion // Menu Data

		#region Menu Override Functions

		public override void OpenMenu ( bool closeParent = true )
		{
			// Open the menu
			base.OpenMenu ( closeParent );

			// Display menu instructions
			setupManager.DisplayInstructions ( "Select your team" );

			// Display debate topic
			debateText.text = "<size=80%><color=#FFFFD2FF>Match Debate</color></size><i>\n" + MatchSettings.MatchDebate.DebateTopic;

			// Reset lineup
			setupManager.ResetLineup ( );

			// Track available leaders for random selection
			List<int> availableLeaders = new List<int> ( );

			// Display each debate participant
			for ( int i = 0; i < leaders.Length; i++ )
			{
				// Set leader unit and team color
				leaders [ i ].SetPortrait ( MatchSettings.GetLeader ( (Player.TeamColor)i ), (Player.TeamColor)i );

				// Check for debate stance
				if ( MatchSettings.MatchDebate.GetLeaderResponse ( ( Player.TeamColor )i ).HasStance )
				{
					// Display participant info
					leaders [ i ].IsEnabled = true;

					// Check if participant is available
					leaders [ i ].IsAvailable = !MatchSettings.Players.Exists ( x => x.Team == (Player.TeamColor)i );

					// Add available leaders
					if ( leaders [ i ].IsAvailable )
						availableLeaders.Add ( i );
				}
				else
				{
					// Remove participant
					leaders [ i ].IsEnabled = false;
				}
			}

			// Select random leader
			SelectLeader ( availableLeaders [ Random.Range ( 0, availableLeaders.Count ) ] );

			// Display prompt
			setupManager.Splash.Slide ( "<size=75%>" + setupManager.CurrentPlayer.PlayerName + "</size>\n<color=white>Debate Stance", Color.white, true );
		}

		#endregion // Menu Override Functions

		#region Public Functions

		/// <summary>
		/// Highlights a leader on mouse enter of the leader's portrait.
		/// </summary>
		/// <param name="index"> The index of the leader. </param>
		public void MouseEnter ( int index )
		{
			// Check for available leader
			if ( leaders [ index ].IsEnabled && leaders [ index ].IsAvailable )
				DisplayUnit ( (Player.TeamColor)index );
		}

		/// <summary>
		/// Unhighlights a leader on mouse exit of the leader's portrait.
		/// </summary>
		/// <param name="index"> The index of the leader. </param>
		public void MouseExit ( int index )
		{
			// Check for available leader
			if ( leaders [ index ].IsEnabled && leaders [ index ].IsAvailable && selectedTeam != (Player.TeamColor)index )
			{
				// Reset portrait
				leaders [ index ].ResetSize ( );

				// Display selected leader
				DisplayUnit ( selectedTeam );
			}
		}

		/// <summary>
		/// Selects a leader for a match.
		/// </summary>
		/// <param name="index"> The index of the participant to convert to team color. </param>
		public void SelectLeader ( int index )
		{
			// Check for available leader
			if ( leaders [ index ].IsEnabled && leaders [ index ].IsAvailable )
			{
				// Reset previously selected unit
				if ( selectedTeam != Player.TeamColor.NO_TEAM )
				{
					leaders [ (int)selectedTeam ].ResetSize ( );
					leaders [ (int)selectedTeam ].IsBorderHighlighted = false;
				}

				// Store selected leader
				selectedTeam = (Player.TeamColor)index;

				// Display unit
				DisplayUnit ( selectedTeam );

				// Highlight portrait
				leaders [ index ].IsBorderHighlighted = true;
			}
		}

		/// <summary>
		/// Confirm the selected leader for a match.
		/// </summary>
		public void ConfirmLeader ( )
		{
			// Set leader for player
			setupManager.CurrentPlayer.Team = selectedTeam;
			setupManager.CurrentPlayer.Units.Add ( MatchSettings.GetLeader ( selectedTeam ) );
			//setupManager.CurrentPlayer.UnitFormation.Add ( setupManager.CurrentPlayer.Units [ 0 ], 0 );

			// Continue to unit selection
			teamSelectionMenu.OpenMenu ( );
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Previews a leader in the HUD.
		/// </summary>
		/// <param name="team"> The team of the leader. </param>
		private void DisplayUnit ( Player.TeamColor team )
		{
			// Increase size of portrait
			leaders [ (int)team ].ChangeSize ( PORTRAIT_OFFSET );

			// Display unit
			setupManager.DisplayUnit ( MatchSettings.GetLeader ( team ), team );

			// Display opinion
			opinionBorder.color = Tools.Util.TeamColor ( team );
			opinionText.text = MatchSettings.MatchDebate.GetLeaderResponse ( team ).Answer;

			// Display team
			teamNameText.text = Tools.Util.TeamName ( team );

			// Display unit in lineup
			setupManager.SetCardInLineup ( 0, MatchSettings.GetLeader ( team ), team );
		}

		#endregion // Private Functions
	}
}