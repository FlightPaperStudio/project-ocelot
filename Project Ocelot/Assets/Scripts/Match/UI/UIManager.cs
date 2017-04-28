using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour 
{
	// UI elements
	public PlayerHUD [ ] huds;
	public GameObject midTurnControls;
	public GameObject endOfMatchControls;

	// UI information
	public TurnTimer timer;
	public GameObject conflictPrompt;
	public SplashPrompt splash;
	public PopUpMenu popUp;
	public LoadingScreen load;
	public Dictionary<Player, PlayerHUD> hudDic = new Dictionary<Player, PlayerHUD> ( );
	public bool isPaused = false;
	public Menu [ ] menus;

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
	/// Initializes the match UI.
	/// </summary>
	public void Initialize ( Player [ ] players )
	{
		// Set up player HUDs
		for ( int i = 0; i < players.Length; i++ )
		{
			huds [ i ].Initialize ( players [ i ] );
			hudDic.Add ( players [ i ], huds [ i ] );
		}

		// Hide mid-turn controls
		ToggleMidTurnControls ( false );

		// Hide prompts
		conflictPrompt.SetActive ( false );
	}

	/// <summary>
	/// Toggles the mid-turn controls on and off.
	/// </summary>
	public void ToggleMidTurnControls ( bool isVisible )
	{
		// Toggle controls
		midTurnControls.SetActive ( isVisible );
	}

	/// <summary>
	/// Prompts that a player has won the match.
	/// </summary>
	public void WinPrompt ( Player p )
	{
		// Play win animation
		StartCoroutine ( WinCoroutine ( p ) );
	}

	/// <summary>
	/// Prompts that a player has won the match.
	/// </summary>
	private IEnumerator WinCoroutine ( Player p )
	{
		// Wait for shake animation
		yield return splash.Shake ( p.name + " Wins!", Util.TeamColor ( p.team ), false ).WaitForCompletion ( );

		// Display end of match controls
		endOfMatchControls.SetActive ( true );
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
}
