using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoSettingsMenu : Menu 
{
	// UI elements
	public TextMeshProUGUI display;
	public TextMeshProUGUI resolution;
	public TextMeshProUGUI quality;
	public TextMeshProUGUI vsync;
	public Button apply;

	// Menu information
	private bool displayValue;
	private int resolutionValue;
	private int qualityValue;
	private bool vsyncValue;

	/// <summary>
	/// Opens the menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true )
	{
		// Open the menu
		base.OpenMenu ( closeParent );

		// Set the current display setting
		SetDisplay ( Settings.Display );

		// Set the current resolution setting
		SetResolution ( Settings.ResolutionWidth, Settings.ResolutionHeight );

		// Set the current quality setting
		SetQuality ( Settings.Quality );

		// Set the current vsync setting
		SetVsync ( Settings.Vsync == 1 );

		// Set the apply button to be inactive
		apply.interactable = false;
	}

	/// <summary>
	/// Toggles the current display setting.
	/// Use this as a button event wrapper.
	/// </summary>
	public void SetDisplay ( )
	{
		// Toggle the current display setting
		SetDisplay ( !displayValue );

		// Set whether or not the apply button should be interactable
		apply.interactable = SettingUpdateCheck ( );
	}

	/// <summary>
	/// Sets the current display setting.
	/// Does not apply to the actual display setting.
	/// </summary>
	private void SetDisplay ( bool value )
	{
		// Toggle the current display setting
		displayValue = value;

		// Display the current display setting
		if ( displayValue )
			display.text = "Full Screen";
		else
			display.text = "Windowed";
	}

	/// <summary>
	/// Increments or decrements the current resolution setting.
	/// Use this as a button event wrapper.
	/// </summary>
	public void SetResolution ( bool increment )
	{
		// Track index
		int index = resolutionValue;

		// Check if the resolution is getting incremented or decremented
		if ( increment )
		{
			// Increment the index
			index++;

			// Check for wrap around
			if ( index >= Screen.resolutions.Length )
				index = 0;
		}
		else
		{
			// Decrement the index
			index--;

			// Check for wrap around
			if ( index < 0 )
				index = Screen.resolutions.Length - 1;
		}

		// Set current resolution
		SetResolution ( index );

		// Set whether or not the apply button should be interactable
		apply.interactable = SettingUpdateCheck ( );
	}

	/// <summary>
	/// Sets the current resolution setting.
	/// Does not apply to the actual resolution setting.
	/// </summary>
	private void SetResolution ( int index )
	{
		// Store the current resolution setting index
		resolutionValue = index;

		// Display the current resolution setting
		resolution.text = Screen.resolutions [ resolutionValue ].width + " x " + Screen.resolutions [ resolutionValue ].height;
	}

	/// <summary>
	/// Sets the current resolution setting.
	/// Use this to set the index from the stored width and height.
	/// </summary>
	private void SetResolution ( int width, int height )
	{
		// Find the index with match resolution
		for ( int i = 0; i < Screen.resolutions.Length; i++ )
		{
			// Check if the resolution matches
			if ( Screen.resolutions [ i ].width == width && Screen.resolutions [ i ].height == height )
			{
				// Set index
				resolutionValue = i;
				break;
			}
		}

		// Display the current resolution setting
		resolution.text = width + " x " + height;
	}

	/// <summary>
	/// Increments or decrements the current quality setting.
	/// Use this as a button event wrapper.
	/// </summary>
	public void SetQuality ( bool increment )
	{
		// Track index
		int index = qualityValue;

		// Check if the quality is getting incremented or decremented
		if ( increment )
		{
			// Increment the index
			index++;

			// Check for wrap around
			if ( index >= QualitySettings.names.Length )
				index = 0;
		}
		else
		{
			// Decrement the index
			index--;

			// Check for wrap around
			if ( index < 0 )
				index = QualitySettings.names.Length - 1;
		}

		// Set the current quality setting
		SetQuality ( index );

		// Set whether or not the apply button should be interactable
		apply.interactable = SettingUpdateCheck ( );
	}

	/// <summary>
	/// Sets the current quality setting.
	/// Does not apply to the actual quality setting.
	/// </summary>
	private void SetQuality ( int index )
	{
		// Store the current quality setting
		qualityValue = index;

		// Display the current quality setting
		quality.text = QualitySettings.names [ qualityValue ];
	}

	/// <summary>
	/// Toggles the current vsync setting.
	/// Use this as a button event wrapper.
	/// </summary>
	public void SetVsync ( )
	{
		// Toggle the current vsync setting
		SetVsync ( !vsyncValue );

		// Set whether or not the apply button should be interactable
		apply.interactable = SettingUpdateCheck ( );
	}

	/// <summary>
	/// Sets the current vsync setting.
	/// Does not apply to the actual vsync setting.
	/// </summary>
	private void SetVsync ( bool value )
	{
		// Store the current vsync setting
		vsyncValue = value;

		// Display the current vsync setting
		if ( vsyncValue )
			vsync.text = "On";
		else
			vsync.text = "Off";
	}

	/// <summary>
	/// Checks if any of the current settings differ from the actual settings.
	/// Use this to determine if the Apply button should be active.
	/// </summary>
	private bool SettingUpdateCheck ( )
	{
		// Check if the display setting has been changed
		if ( displayValue != Settings.Display )
			return true;

		// Check if the resolution setting has been changed
		if ( Screen.resolutions [ resolutionValue ].width != Settings.ResolutionWidth || Screen.resolutions [ resolutionValue ].height != Settings.ResolutionHeight )
			return true;

		// Check if the quality setting has been changed
		if ( qualityValue != Settings.Quality )
			return true;

		// Check if the vsync setting has been changed
		if ( ( vsyncValue && Settings.Vsync == 0 ) || ( !vsyncValue && Settings.Vsync == 1 ) )
			return true;

		// Return that none of the settings have changed
		return false;
	}

	/// <summary>
	/// Applies the current video settings.
	/// </summary>
	public void ApplySettings ( )
	{
		// Apply any display setting change or resolution setting change
		if ( displayValue != Settings.Display || Screen.resolutions [ resolutionValue ].width != Settings.ResolutionWidth || Screen.resolutions [ resolutionValue ].height != Settings.ResolutionHeight )
		{
			// Store display setting change
			Settings.Display = displayValue;

			// Store resolution setting change
			Settings.ResolutionWidth = Screen.resolutions [ resolutionValue ].width;
			Settings.ResolutionHeight = Screen.resolutions [ resolutionValue ].height;

			// Set the display and resolution of the game
			Screen.SetResolution ( Settings.ResolutionWidth, Settings.ResolutionHeight, Settings.Display );
		}

		// Apply any quality setting or vsync setting change
		if ( qualityValue != Settings.Quality || ( vsyncValue && Settings.Vsync == 0 ) || ( !vsync && Settings.Vsync == 1 ) )
		{
			// Store quality setting change
			Settings.Quality = qualityValue;

			// Store vsync setting change
			if ( vsyncValue )
				Settings.Vsync = 1;
			else
				Settings.Vsync = 0;

			// Set the graphical quality of the game
			QualitySettings.SetQualityLevel ( Settings.Quality );

			// Set if vsync is enabled for the game
			QualitySettings.vSyncCount = Settings.Vsync;
		}

		// Save the current settings
		Settings.SaveVideoSettings ( );

		// Make the applay button inactive
		apply.interactable = false;
	}

	/// <summary>
	/// Resets the video settings to their default values.
	/// </summary>
	public void ResetVideo ( )
	{
		// Reset the actual video settings
		Settings.RestoreDefaultVideoSettings ( );

		// Set the current display setting
		SetDisplay ( Settings.Display );

		// Set the current resolution setting
		SetResolution ( Settings.ResolutionWidth, Settings.ResolutionHeight );

		// Set the current quality setting
		SetQuality ( Settings.Quality );

		// Set the current vsync setting
		SetVsync ( Settings.Vsync == 1 );

		// Apply any changes
		ApplySettings ( );
	}
}
