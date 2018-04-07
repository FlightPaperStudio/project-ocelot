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

	public Menu teamSelection;
	public SplashPrompt splash;
	public PopUpMenu popUp;
	public LoadingScreen load;
	public Menu [ ] menus;

	[HideInInspector]
	public bool isPaused = false;
	private int playerIndex = 0;

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

	// Menu information
	

	#endregion // Match Setup Data

	/// <summary>
	/// Start the team setup menu.
	/// </summary>
	private void Start ( )
	{
		// Display match info
		matchPrompt.text = MatchSettings.MatchDebate.EventName;
		switch ( MatchSettings.type )
		{
		case MatchType.Classic:
		case MatchType.CustomClassic:
			matchPrompt.text += "\n<size=60%>Classic Match";
			break;
		case MatchType.Rumble:
		case MatchType.CustomRumble:
			matchPrompt.text += "\n<size=60%>Rumble Match";
			break;
		}

		// Begin team selection
		debateMenu.OpenMenu ( );
	}

	/// <summary>
	/// Listens for the pause button being pressed.
	/// </summary>
	private void Update ( )
	{
		// Check for the escape button being pressed
		if ( Input.GetKeyDown ( KeyCode.Escape ) && !popUp.menuContainer.activeSelf )
		{
			// Check if the game is paused
			if ( isPaused )
			{
				// Find the current open menu and close it
				foreach ( Menu m in menus )
				{
					if ( m.menuContainer.activeSelf )
					{
						// Check if the current menu is the base pause menu
						if ( m is PauseMenu )
							isPaused = false;

						// Close the menu
						m.CloseMenu ( );
						break;
					}
				}
			}
			else
			{
				// Mark that the game is paused
				isPaused = true;

				// Open the pause menu
				menus [ 0 ].OpenMenu ( );
			}
		}
	}

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
			debateMenu.OpenMenu ( );
		}
		else
		{
			// Begin match
			BeginMatch ( );
		}
	}

	/// <summary>
	/// Begins the match.
	/// </summary>
	public void BeginMatch ( )
	{
		// Track scene
		Scenes scene = Scenes.Classic;

		// Check match type
		switch ( MatchSettings.type )
		{
		case MatchType.Classic:
		case MatchType.CustomClassic:
		case MatchType.Mirror:
		case MatchType.CustomMirror:
			scene = Scenes.Classic;
			break;
		case MatchType.Rumble:
		case MatchType.CustomRumble:
			scene = Scenes.Rumble;
			break;
		}

		// Load match
		load.LoadScene ( scene );
	}
}
