using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebateMenu : Menu
{
	#region Private Classes

	[System.Serializable]
	private class DebateCards
	{
		[HideInInspector]
		public UnitDefaultData Unit;

		[HideInInspector]
		public Player.TeamColor Team;

		public UnitCard Card;
		public GameObject StanceContainer;
		public Image StanceBorder;
		public TextMeshProUGUI StanceText;
		public Button SelectButton;

		private bool isEnabled;
		private bool isAvailable;

		/// <summary>
		/// Whether or not the debate participant UI elements are visible.
		/// </summary>
		public bool IsEnabled
		{
			get
			{
				// Return value
				return isEnabled;
			}
			set
			{
				// Store value
				isEnabled = value;

				// Display or hide elements
				Card.IsEnabled = isEnabled;
				StanceContainer.SetActive ( isEnabled );
				SelectButton.gameObject.SetActive ( isEnabled && isAvailable );
			}
		}

		/// <summary>
		/// Whether or not the debate participant UI elements are selectable.
		/// </summary>
		public bool IsAvailable
		{
			get
			{
				// Return value
				return isAvailable;
			}
			set
			{
				// Store value
				isAvailable = value;

				// Set color and selectability
				Card.IsAvailable = isAvailable;
				StanceBorder.color = isAvailable ? Util.TeamColor ( Team ) : (Color32)Color.grey;
				SelectButton.gameObject.SetActive ( isEnabled && isAvailable );
			}
		}
	}

	#endregion // Private Clases

	#region UI Elements

	[SerializeField]
	private TextMeshProUGUI debateText;

	[SerializeField]
	private DebateCards [ ] cards;

	[SerializeField]
	private GameObject confirmControls;

	[SerializeField]
	private GameObject instructionPrompt;

	#endregion // UI Elements

	#region Menu Data

	[SerializeField]
	private TeamSetup setupManager;

	[SerializeField]
	private TeamSelectionMenu teamSelectionMenu;

	private Player.TeamColor selectedTeam;

	#endregion // Menu Data

	#region Menu Override Functions

	public override void OpenMenu ( bool closeParent = true, params object [ ] values )
	{
		// Open the menu
		base.OpenMenu ( closeParent, values );

		// Display debate topic
		debateText.text = "Match Debate\n<size=60%>" + MatchSettings.MatchDebate.DebateTopic;

		// Set slot meter to preview leader unit
		setupManager.SlotMeter.PreviewSlots ( 1 );

		// Display participants
		DisplayParticipants ( );
	}

	#endregion // Menu Override Functions

	#region Public Functions

	/// <summary>
	/// Selects a leader for a match.
	/// </summary>
	/// <param name="index"> The index of the participant to convert to team color. </param>
	public void SelectLeader ( int index )
	{
		// Store selected leader
		selectedTeam = (Player.TeamColor)index;

		// Display selected leader
		DisplaySelectedParticipant ( selectedTeam );

		// Display confirmation prompt
		instructionPrompt.SetActive ( false );
		confirmControls.SetActive ( true );
	}

	/// <summary>
	/// Confirm the selected leader for a match
	/// </summary>
	public void ConfirmLeader ( )
	{
		// Set leader for player
		setupManager.CurrentPlayer.Team = selectedTeam;
		setupManager.CurrentPlayer.Units.Add ( UnitDataStorage.GetLeaderDefault ( selectedTeam ) );

		// Check for mirror match
		if ( MatchSettings.type == MatchType.Mirror || MatchSettings.type == MatchType.CustomMirror )
		{
			// Move on to the next player
			setupManager.SetNextPlayer ( );
		}
		else
		{
			// Add unit to meter
			setupManager.SlotMeter.SetMeter ( 1 );

			// Continue to unit selection
			teamSelectionMenu.OpenMenu ( );
		}
	}

	/// <summary>
	/// Cancel a selection for a leader for a match.
	/// </summary>
	public void CancelSelection ( )
	{
		// Display participants
		DisplayParticipants ( );

		// Display instruction prompt
		instructionPrompt.SetActive ( true );
		confirmControls.SetActive ( false );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Displays each debate participant and which leaders are available for selection.
	/// </summary>
	private void DisplayParticipants ( )
	{
		// Display each debate participant
		for ( int i = 0; i < cards.Length; i++ )
		{
			// Check for debate stance
			if ( MatchSettings.MatchDebate.GetLeaderResponse ( (Player.TeamColor)i ).HasStance )
			{
				// Set leader unit and team color
				cards [ i ].Unit = UnitDataStorage.GetLeaderDefault ( (Player.TeamColor)i );
				cards [ i ].Team = (Player.TeamColor)i;
				cards [ i ].Card.SetCard ( UnitDataStorage.GetLeaderDefault ( (Player.TeamColor)i ), (Player.TeamColor)i );

				// Display participant info
				cards [ i ].IsEnabled = true;

				// Check if participant is available
				cards [ i ].IsAvailable = !MatchSettings.Players.Exists ( x => x.Team == (Player.TeamColor)i );

				// Display stance
				cards [ i ].StanceText.text = MatchSettings.MatchDebate.GetLeaderResponse ( (Player.TeamColor)i ).Answer;
			}
			else
			{
				// Remove participant
				cards [ i ].IsEnabled = false;
			}
		}
	}

	/// <summary>
	/// Displays each debate participant and which leader is currently selected.
	/// </summary>
	/// <param name="team"> The team color of the selected leader. </param>
	private void DisplaySelectedParticipant ( Player.TeamColor team )
	{
		// Display debate participants
		for ( int i = 0; i < cards.Length; i++ )
		{
			// Check for debate stance
			if ( MatchSettings.MatchDebate.GetLeaderResponse ( (Player.TeamColor)i ).HasStance )
			{
				// Display participant info
				cards [ i ].IsEnabled = true;

				// Check if participant is available
				cards [ i ].IsAvailable = (Player.TeamColor)i == team;

				// Display stance
				cards [ i ].SelectButton.gameObject.SetActive ( false );
			}
			else
			{
				// Remove participant
				cards [ i ].IsEnabled = false;
			}
		}
	}

	#endregion // Private Functions
}
