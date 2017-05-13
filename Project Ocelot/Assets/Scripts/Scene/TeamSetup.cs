﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSetup : MonoBehaviour 
{
	// UI elements
	public TextMeshProUGUI matchInfo;

	// Player settings
	private int playerIndex = 0;
	public PlayerSettings currentPlayer;

	// Menu information
	public bool isPaused = false;
	public Menu teamSelection;
	public SplashPrompt splash;
	public PopUpMenu popUp;
	public LoadingScreen load;
	public Menu [ ] menus;

	/// <summary>
	/// Start the team setup menu.
	/// </summary>
	private void Start ( )
	{
		// Set player
		currentPlayer = MatchSettings.playerSettings [ playerIndex ];

		// Display match info
		switch ( MatchSettings.type )
		{
		case MatchType.Classic:
		case MatchType.CustomClassic:
			matchInfo.text = "Classic Match";
			break;
		case MatchType.Rumble:
		case MatchType.CustomRumble:
			matchInfo.text = "Rumble Match";
			break;
		}
		matchInfo.text += "\n<size=60%>Location Name";

		// Begin team selection
		teamSelection.OpenMenu ( false, currentPlayer );
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
	public bool SetNextPlayer ( )
	{
		// Increment index
		playerIndex++;

		// Check if players remain
		if ( playerIndex < MatchSettings.playerSettings.Count )
		{
			// Set next player
			currentPlayer = MatchSettings.playerSettings [ playerIndex ];
			return true;
		}
		else
		{
			// Return that all players have selected their team
			return false;
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
