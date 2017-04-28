using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : Menu 
{
	// Menu information
	public UIManager UI;
	public PopUpMenu.PopUpDelegate popUpDelegate;

	/// <summary>
	/// Closes the pause menu.
	/// This is button event wrapper specifically for marking that the match is no longer paused.
	/// </summary>
	public void Resume ( )
	{
		// Mark that the match is no longer paused
		UI.isPaused = false;

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
		UI.popUp.OpenMenu ( false, false, "Are you sure you want to leave the match?\n<size=75%>(All progress in your current match will be lost!)", popUpDelegate, null );
	}

	/// <summary>
	/// Returns to the Main Menu.
	/// Use this as a delegate for the Pop Up Menu.
	/// </summary>
	private void ReturnToMainMenu ( )
	{
		// Load the main menu
		UI.load.LoadScene ( Scenes.Menus );
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
		UI.popUp.OpenMenu ( false, false, "Are you sure you want to exit to the desktop?\n<size=75%>(All progress in your current match will be lost!)", popUpDelegate, null );
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
