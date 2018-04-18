using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettingsMenu : Menu 
{
	// UI elements
	public Slider masterSlider;
	public TextMeshProUGUI masterValue;
	public Slider musicSlider;
	public TextMeshProUGUI musicValue;
	public Slider soundSlider;
	public TextMeshProUGUI soundValue;

	// Menu information
	private bool hasSettingsChanged;

	/// <summary>
	/// Opens the menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true )
	{
		// Open the menu
		base.OpenMenu ( closeParent );

		// Set master volume slider
		SetMasterVolume ( Settings.MasterVolume );

		// Set music volume slider
		SetMusicVolume ( Settings.MusicVolume );

		// Set sound volume slider
		SetSoundVolume ( Settings.SoundVolume );

		// Set the start setting
		hasSettingsChanged = false;
	}

	/// <summary>
	/// Closes the menu.
	/// Use this for going up a layer (e.g. from a sub menu to a parent menu).
	/// </summary>
	public override void CloseMenu ( bool openParent = true )
	{
		// Close menu
		base.CloseMenu (openParent);

		// Save audio settings
		if ( hasSettingsChanged )
			Settings.SaveAudioSettings ( );
	}

	/// <summary>
	/// Sets the master volume setting.
	/// </summary>
	public void SetMasterVolume ( float value )
	{
		// Store value
		Settings.MasterVolume = value;

		// Set the slider
		masterSlider.value = value;

		// Display value
		masterValue.text = ( (int)( value * 100 ) ).ToString ( );
	}

	/// <summary>
	/// Sets the music volume.
	/// </summary>
	public void SetMusicVolume ( float value )
	{
		// Store value
		Settings.MusicVolume = value;

		// Set the slider
		musicSlider.value = value;

		// Display value
		musicValue.text = ( (int)( value * 100 ) ).ToString ( );
	}

	/// <summary>
	/// Sets the sound volume.
	/// </summary>
	public void SetSoundVolume ( float value )
	{
		// Store value
		Settings.SoundVolume = value;

		// Set the slider
		soundSlider.value = value;

		// Display value
		soundValue.text = ( (int)( value * 100 ) ).ToString ( );
	}

	/// <summary>
	/// Resets the audio settings to their default values.
	/// </summary>
	public void ResetAudio ( )
	{
		// Set default values
		Settings.RestoreDefaultAudioSettings ( );

		// Set master volume slider
		SetMasterVolume ( Settings.MasterVolume );

		// Set music volume slider
		SetMusicVolume ( Settings.MusicVolume );

		// Set sound volume slider
		SetSoundVolume ( Settings.SoundVolume );
	}
}
