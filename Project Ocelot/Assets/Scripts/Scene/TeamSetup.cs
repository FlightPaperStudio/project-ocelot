using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSetup : MonoBehaviour 
{
	#region UI Elements

	[SerializeField]
	private TextMeshProUGUI matchPrompt;

	[SerializeField]
	private TextMeshProUGUI playerName;

	public TeamSlotMeter SlotMeter;

	#endregion // UI Elements

	#region Match Setup Data

	[SerializeField]
	private DebateMenu debateMenu;

	[SerializeField]
	private TeamFormationMenu formationMenu;

	[SerializeField]
	private Menu [ ] menus;

	public SplashPrompt Splash;
	public PopUpMenu PopUp;
	public LoadingScreen Load;

	private bool isPaused = false;
	private int playerIndex = 0;

	/// <summary>
	/// Whether or not the game is currently paused.
	/// </summary>
	public bool IsPaused
	{
		get
		{
			// Return value
			return isPaused;
		}
		set
		{
			// Store value
			isPaused = value;
		}
	}

	/// <summary>
	/// The current player up for team selection.
	/// </summary>
	public PlayerSettings CurrentPlayer
	{
		get
		{
			return playerIndex < MatchSettings.Players.Count ? MatchSettings.Players [ playerIndex ] : null;
		}
	}

	#endregion // Match Setup Data

	#region MonoBehaviour Functions

	/// <summary>
	/// Start the team setup menu.
	/// </summary>
	private void Start ( )
	{
		// Display match info
		matchPrompt.text = MatchSettings.MatchDebate.EventName;
		switch ( MatchSettings.Type )
		{
		case MatchType.Classic:
		case MatchType.CustomClassic:
			matchPrompt.text += "\n<size=60%><color=#FFD24BFF>Classic Match";
			break;
		case MatchType.Mirror:
		case MatchType.CustomMirror:
			matchPrompt.text += "\n<size=60%><color=#FFD24BFF>Mirror Match";
			break;
		case MatchType.Rumble:
		case MatchType.CustomRumble:
			matchPrompt.text += "\n<size=60%><color=#FFD24BFF>Rumble Match";
			break;
		case MatchType.Ladder:
		case MatchType.CustomLadder:
			matchPrompt.text += "\n<size=60%><color=#FFD24BFF>Ladder Match";
			break;
		}

		// Display player name
		playerName.text = CurrentPlayer.PlayerName;

		// Begin team selection
		debateMenu.OpenMenu ( );
	}

	/// <summary>
	/// Listens for the pause button being pressed.
	/// </summary>
	private void Update ( )
	{
		// Check for the escape button being pressed
		if ( Input.GetKeyDown ( KeyCode.Escape ) && !PopUp.IsOpen )
		{
			// Check if the game is paused
			if ( IsPaused )
			{
				// Find the current open menu and close it
				foreach ( Menu m in menus )
				{
					if ( m.IsOpen )
					{
						// Check if the current menu is the base pause menu
						if ( m is PauseMenu )
							IsPaused = false;

						// Close the menu
						m.CloseMenu ( );
						break;
					}
				}
			}
			else
			{
				// Mark that the game is paused
				IsPaused = true;

				// Open the pause menu
				menus [ 0 ].OpenMenu ( );
			}
		}
	}

	#endregion // MonoBehaviour Functions

	#region Public Functions

	/// <summary>
	/// Sets the next player for team selection.
	/// Returns false if all players have selected their teams.
	/// </summary>
	public void SetNextPlayer ( )
	{
		// Increment index
		playerIndex++;

		// Check if players remain
		if ( playerIndex < MatchSettings.playerSettings.Count )
		{
			// Display name of current player
			playerName.text = CurrentPlayer.PlayerName;

			// Reset slot meter
			SlotMeter.ResetMeter ( );

			// Begin the setup process for player
			formationMenu.CloseMenu ( false );
			debateMenu.OpenMenu ( );
		}
		else
		{
			// Set the units and formation for a mirror match
			if ( MatchSettings.Type == MatchType.Mirror || MatchSettings.Type == MatchType.CustomMirror )
				MatchSettings.SetMirrorUnits ( );

			// Begin match
			BeginMatch ( );
		}
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Begins the match.
	/// </summary>
	public void BeginMatch ( )
	{
		// Track scene
		Scenes scene = Scenes.CLASSIC;

		// Check match type
		switch ( MatchSettings.Type )
		{
		case MatchType.Classic:
		case MatchType.CustomClassic:
		case MatchType.Mirror:
		case MatchType.CustomMirror:
			scene = Scenes.CLASSIC;
			break;
		case MatchType.Rumble:
		case MatchType.CustomRumble:
			scene = Scenes.RUMBLE;
			break;
		}

		// Load match
		Load.LoadScene ( scene );
	}

	#endregion // Private Functions
}
