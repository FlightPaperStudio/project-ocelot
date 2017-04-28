using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchTypeMenu : Menu
{
	// UI elements
	public RectTransform [ ] buttons;
	public GameObject [ ] outlines;

	// Menu information
	public LoadingScreen load;

	/// <summary>
	/// Opens the menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true, params object [ ] values )
	{
		// Open the menu
		base.OpenMenu ( closeParent, values );

		// Reset each button
		for ( int i = 0; i < buttons.Length; i++ )
			MouseExit ( i );
	}

	/// <summary>
	/// Highlights the game mode button.
	/// </summary>
	public void MouseEnter ( int index )
	{
		// Increase the size of the button
		buttons [ index ].offsetMax = new Vector2 ( 5f, 5f );
		buttons [ index ].offsetMin = new Vector2 ( -5f, -5f );

		// Display outline
		outlines [ index ].SetActive ( true );
	}

	/// <summary>
	/// Unhighlights the game mode button.
	/// </summary>
	public void MouseExit ( int index )
	{
		// Decrease the size of the button
		buttons [ index ].offsetMax = Vector2.zero;
		buttons [ index ].offsetMin = Vector2.zero;

		// Hide outline
		outlines [ index ].SetActive ( false );
	}

	/// <summary>
	/// Selects and loads a match for the given game mode.
	/// </summary>
	public void MouseClick ( int index )
	{
		// Check match type
		switch ( index )
		{
		case 0:
			LoadClassicMatch ( );
			break;
		case 1:
			LoadMirrorMatch ( );
			break;
		case 2:
			LoadRumbleMatch ( );
			break;
		}
	}

	private void LoadClassicMatch ( )
	{
		// Begin loading 
		load.BeginLoad ( );

		// Set the match settings
		MatchSettings.SetMatchSettings ( MatchType.Classic );

		// Load classic match
		load.LoadScene ( Scenes.MatchSetup );
	}

	private void LoadMirrorMatch ( )
	{
		// Begin loading 
		load.BeginLoad ( );

		// Set the match settings
		MatchSettings.SetMatchSettings ( MatchType.Mirror );

		// Load classic match
		load.LoadScene ( Scenes.Classic );
	}

	/// <summary>
	/// Loads a rumble match.
	/// </summary>
	private void LoadRumbleMatch ( )
	{
		// Begin loading 
		load.BeginLoad ( );

		// Set the match settings
		MatchSettings.SetMatchSettings ( MatchType.Rumble );

		// Load rumble match
		load.LoadScene ( Scenes.MatchSetup );
	}
}
