using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour 
{
	#region UI Elements
	
	public GameObject midTurnControls;
	public GameObject endOfMatchControls;
	public GameObject conflictPrompt;
	public GameObject skipControls;

	#endregion // UI Elements

	#region UI Data

	public MatchInfoMenu matchInfoMenu;
	public UnitHUD unitHUD;
	public TurnTimer timer;
	public SplashPrompt splash;
	public PopUpMenu popUp;
	public LoadingScreen load;
	public bool isPaused = false;
	public Menu [ ] pauseMenus;

	#endregion // UI Data

	#region MonoBehaviour Functions

	/// <summary>
	/// Listens for the pause button being pressed.
	/// </summary>
	private void Update ( )
	{
		// Check for the escape button being pressed
		if ( Input.GetKeyDown ( KeyCode.Escape ) && !popUp.IsOpen && !matchInfoMenu.IsOpen )
		{
			// Check if the game is paused
			if ( isPaused )
			{
				// Find the current open menu and close it
				foreach ( Menu m in pauseMenus )
				{
					if ( m.IsOpen )
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
				pauseMenus [ 0 ].OpenMenu ( );
			}
		}
		// Check for the tab button being pressed
		else if ( Input.GetKeyDown ( KeyCode.Tab ) && !isPaused && !popUp.IsOpen )
		{
			// Toggle open/close the match info menu
			if ( matchInfoMenu.IsOpen )
				matchInfoMenu.CloseMenu ( );
			else
				matchInfoMenu.OpenMenu ( );
		}
	}

	#endregion // MonoBehaviour Functions

	#region Public Functions

	/// <summary>
	/// Initializes the match UI.
	/// </summary>
	/// <param name="players"> A list of the players in the match. </param>
	public void Initialize ( Player [ ] players )
	{
		// Set up player HUDs
		matchInfoMenu.Initialize ( players );

		// Hide unit HUD
		unitHUD.HideHUD ( );

		// Hide mid-turn controls
		ToggleMidTurnControls ( false, false );

		// Hide prompts
		conflictPrompt.SetActive ( false );
	}

	/// <summary>
	/// Toggles the mid-turn controls on and off.
	/// </summary>
	/// <param name="moveControls"> Whether or not the move controls should be displayed. </param>
	/// <param name="skipUnitControls"> Whether or not the skip controls should be displayed. </param>
	public void ToggleMidTurnControls ( bool moveControls, bool skipUnitControls )
	{
		// Toggle movement controls
		midTurnControls.SetActive ( moveControls );

		// Toggle skip controls
		skipControls.SetActive ( skipUnitControls );
	}

	/// <summary>
	/// Prompts that a player has won the match.
	/// </summary>
	/// <param name="p"> The player that won the match. </param>
	public void WinPrompt ( Player p )
	{
		// Play win animation
		StartCoroutine ( WinCoroutine ( p ) );
	}

	/// <summary>
	/// Restarts the match with the exact same teams and formations.
	/// </summary>
	public void Rematch ( )
	{
		// Restart match
		switch ( MatchSettings.type )
		{
		case MatchType.Classic:
		case MatchType.Mirror:
		case MatchType.CustomClassic:
		case MatchType.CustomMirror:
			load.LoadScene ( Scenes.Classic );
			break;
		case MatchType.Rumble:
		case MatchType.CustomRumble:
			load.LoadScene ( Scenes.Rumble );
			break;
		}
	}

	/// <summary>
	/// Start a new match with new teams and formations.
	/// </summary>
	public void ChangeTeams ( )
	{
		// Begin load
		load.BeginLoad ( );

		// Reset match settings
		MatchSettings.SetMatchSettings ( MatchSettings.type );

		// Check match type
		if ( MatchSettings.type == MatchType.Mirror || MatchSettings.type == MatchType.CustomMirror )
		{
			// Load the match
			load.LoadScene ( Scenes.Classic );
		}
		else
		{
			// Load match setup
			load.LoadScene ( Scenes.MatchSetup );
		}
	}

	/// <summary>
	/// Leaves the game and sends the player to the main menu.
	/// </summary>
	public void MainMenu ( )
	{
		// Load main menu
		load.LoadScene ( Scenes.Menus );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Prompts that a player has won the match.
	/// </summary>
	/// <param name="p"> The player that won the match. </param>
	private IEnumerator WinCoroutine ( Player p )
	{
		// Wait for shake animation
		yield return splash.Shake ( p.PlayerName + " Wins!", Util.TeamColor ( p.Team ), false ).WaitForCompletion ( );

		// Display end of match controls
		endOfMatchControls.SetActive ( true );
	}

	#endregion // Private Functions
}
