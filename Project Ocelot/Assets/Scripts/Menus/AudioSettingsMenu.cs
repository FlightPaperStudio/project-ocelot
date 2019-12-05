using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectOcelot.Menues
{
	public class AudioSettingsMenu : Menu
	{
		#region Private Classes

		[System.Serializable]
		private class SettingSlider
		{
			public Slider Slider;
			public TextMeshProUGUI Text;
		}

		#endregion // Private Classes

		#region UI Elements

		[SerializeField]
		private UI.CarouselButton muteSetting;

		[SerializeField]
		private SettingSlider musicSetting;

		[SerializeField]
		private SettingSlider sfxSetting;

		[SerializeField]
		private SettingSlider uiSetting;

		[SerializeField]
		private Button reset;

		#endregion // UI Elements

		#region Menu Data

		private bool mute;
		private float music;
		private float sfx;
		private float ui;

		/// <summary>
		/// Whether or not the audio settings have changed since opening the Audio Setting Menu.
		/// </summary>
		private bool HasSettingsChanged
		{
			get
			{
				// Check if mute setting has changed
				if ( mute != Settings.MuteVolume )
					return true;

				// Check if music setting has changed
				if ( music != Settings.MusicVolume )
					return true;

				// Check if sfx setting has changed
				if ( sfx != Settings.SoundVolume )
					return true;

				// Check if ui setting has changed
				if ( ui != Settings.UIVolume )
					return true;

				// Return that no settings have changed
				return false;
			}
		}

		/// <summary>
		/// Whether or not the audio settings are set to the default settings.
		/// </summary>
		private bool IsDefaultSettings
		{
			get
			{
				// Check if mute setting is default
				if ( Settings.MuteVolume != Settings.MUTE_VOLUME_DEFAULT )
					return false;

				// Check if music setting is default
				if ( Settings.MusicVolume != Settings.MUSIC_VOLUME_DEFAULT )
					return false;

				// Check if sfx setting is default
				if ( Settings.SoundVolume != Settings.SOUND_VOLUME_DEFAULT )
					return false;

				// Check if ui setting is default
				if ( Settings.UIVolume != Settings.UI_VOLUME_DEFAULT )
					return false;

				// Return that all settings are at default
				return true;
			}
		}

		#endregion // Menu Data

		#region Menu Override Functions

		/// <summary>
		/// Opens the menu.
		/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
		/// </summary>
		public override void OpenMenu ( bool closeParent = true )
		{
			// Open the menu
			base.OpenMenu ( closeParent );

			// Set mute volume carousel
			SetMuteVolume ( Settings.MuteVolume, false );

			// Set music volume slider
			SetMusicVolume ( Settings.MusicVolume );

			// Set sfx volume slider
			SetSFXVolume ( Settings.SoundVolume );

			// Set ui volume slider
			SetUIVolume ( Settings.UIVolume );

			// Set the starting settings
			mute = Settings.MuteVolume;
			music = Settings.MusicVolume;
			sfx = Settings.SoundVolume;
			ui = Settings.UIVolume;
		}

		/// <summary>
		/// Closes the menu.
		/// Use this for going up a layer (e.g. from a sub menu to a parent menu).
		/// </summary>
		public override void CloseMenu ( bool openParent = true )
		{
			// Close menu
			base.CloseMenu ( openParent );

			// Save audio settings
			if ( HasSettingsChanged )
				Settings.SaveAudioSettings ( );
		}

		#endregion // Menu Override Functions

		#region Public Functions

		/// <summary>
		/// Sets the Mute Volume Setting.
		/// Use this function as a carousel button click event wrapper.
		/// </summary>
		public void UpdateMuteVolume ( )
		{
			// Store value
			Settings.MuteVolume = muteSetting.IsOptionTrue;

			// Update volume
			MusicManager.Instance.AdjustVolume ( );
			SFXManager.Instance.UpdateSound ( );

			// Set if reset is active
			reset.interactable = !IsDefaultSettings;
		}

		/// <summary>
		/// Sets the music volume.
		/// </summary>
		/// <param name="value"> The value of the music setting. </param>
		public void SetMusicVolume ( float value )
		{
			// Store value
			Settings.MusicVolume = value;

			// Set the slider
			musicSetting.Slider.value = value;

			// Display value
			musicSetting.Text.text = ( (int)( value * 100 ) ).ToString ( );

			// Update volume
			MusicManager.Instance.AdjustVolume ( );

			// Set if reset is active
			reset.interactable = !IsDefaultSettings;
		}

		/// <summary>
		/// Sets the SFX volume.
		/// </summary>
		/// <param name="value"> The value of the SFX setting. </param>
		public void SetSFXVolume ( float value )
		{
			// Store value
			Settings.SoundVolume = value;

			// Set the slider
			sfxSetting.Slider.value = value;

			// Display value
			sfxSetting.Text.text = ( (int)( value * 100 ) ).ToString ( );

			// Update volume
			SFXManager.Instance.UpdateSound ( );

			// Set if reset is active
			reset.interactable = !IsDefaultSettings;
		}

		/// <summary>
		/// Sets the UI volume setting.
		/// </summary>
		/// <param name="value"> The value of the UI setting. </param>
		public void SetUIVolume ( float value )
		{
			// Store value
			Settings.UIVolume = value;

			// Set the slider
			uiSetting.Slider.value = value;

			// Display value
			uiSetting.Text.text = ( (int)( value * 100 ) ).ToString ( );

			// Set if reset is active
			reset.interactable = !IsDefaultSettings;
		}

		/// <summary>
		/// Resets the audio settings to their default values.
		/// </summary>
		public void ResetAudio ( )
		{
			// Set default values
			Settings.RestoreDefaultAudioSettings ( );

			// Set mute volume carousel
			SetMuteVolume ( Settings.MuteVolume, true );

			// Set music volume slider
			SetMusicVolume ( Settings.MusicVolume );

			// Set sfx volume slider
			SetSFXVolume ( Settings.SoundVolume );

			// Set ui volume slider
			SetUIVolume ( Settings.UIVolume );

			// Set reset as inactive
			reset.interactable = false;
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Sets the mute volume setting.
		/// </summary>
		/// <param name="value"> The value of the mute setting. </param>
		/// <param name="playAnimation"> Whether or not animations should play for the carousel button. </param>
		private void SetMuteVolume ( bool value, bool playAnimation )
		{
			// Store value
			Settings.MuteVolume = value;

			// Set carousel
			muteSetting.SetOption ( value, playAnimation );

			// Set if reset is active
			reset.interactable = !IsDefaultSettings;
		}

		#endregion // Private Functions
	}
}