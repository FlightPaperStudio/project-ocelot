using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : Menu 
{
	// Menu information
	public UIManager UI;
	public TeamSetup setup;
	private PopUpMenu popUp;
	private LoadingScreen load;
	public PopUpMenu.PopUpDelegate popUpDelegate;

	/// <summary>
	/// Gets the UI information dependent on the scene.
	/// </summary>
	private void Start ( )
	{
		if ( UI != null )
		{
			popUp = UI.popUp;
			load = UI.load;
		}
		else if ( setup != null )
		{
			popUp = setup.PopUp;
			load = setup.Load;
		}
	}

	/// <summary>
	/// Closes the pause menu.
	/// This is button event wrapper specifically for marking that the match is no longer paused.
	/// </summary>
	public void Resume ( )
	{
		// Mark that the match is no longer paused
		if ( UI != null )
			UI.isPaused = false;
		else if ( setup != null )
			setup.IsPaused = false;

		// Close the pause menu
		base.CloseMenu ( );
	}

	/// <summary>
	/// Quits the match and returns to the main menu.
	/// </summary>
	public void MainMenu ( )
	{
		// Set delegate
		popUpDelegate = null;
		popUpDelegate += ReturnToMainMenu;

		// Prompt user
		popUp.SetConfirmationPopUp ( "Are you sure you want to leave the match?\n<size=75%>(All progress in your current match will be lost!)", popUpDelegate, null );
		popUp.OpenMenu ( false );
	}

	/// <summary>
	/// Returns to the Main Menu.
	/// Use this as a delegate for the Pop Up Menu.
	/// </summary>
	private void ReturnToMainMenu ( )
	{
		// Load the main menu
		load.LoadScene ( Scenes.MENUS );
	}

	/// <summary>
	/// Exits the game.
	/// </summary>
	public void Quit ( )
	{
		// Set delegate
		popUpDelegate = null;
		popUpDelegate += QuitToDesktop;

		// Prompt user
		popUp.SetConfirmationPopUp ( "Are you sure you want to exit to the desktop?\n<size=75%>(All progress in your current match will be lost!)", popUpDelegate, null );
		popUp.OpenMenu ( false );
	}

	/// <summary>
	/// Quits to desktop.
	/// Use this as a delegate for the Pop Up Menu.
	/// </summary>
	private void QuitToDesktop ( )
	{
		// Quit the game
		Application.Quit ( );
	}
}
