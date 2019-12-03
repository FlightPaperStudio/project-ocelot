using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectOcelot.Menues
{
	public class VideoSettingsMenu : Menu
	{
		#region UI Elements

		[SerializeField]
		private UI.CarouselButton display;

		[SerializeField]
		private UI.CarouselButton resolution;

		[SerializeField]
		private UI.CarouselButton quality;

		[SerializeField]
		private UI.CarouselButton vsync;

		[SerializeField]
		private Button apply;

		[SerializeField]
		private Button reset;

		#endregion // UI Elements

		#region Menu Data

		private bool displayValue;
		private int resolutionValue;
		private int qualityValue;
		private bool vsyncValue;

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

			// Set the current display setting
			SetDisplay ( Settings.Display );

			// Set the current resolution setting
			resolution.InitializeOptions ( GetResolutions ( ) );
			SetResolution ( Settings.ResolutionWidth, Settings.ResolutionHeight );

			// Set the current quality setting
			SetQuality ( Settings.Quality );

			// Set the current vsync setting
			SetVsync ( Settings.Vsync == 1 );

			// Set the apply button to be inactive
			apply.interactable = false;

			// Set if reset button is active
			reset.interactable = SettingDefaultCheck ( );
		}

		#endregion // Menu Override Functions

		#region Public Functions

		/// <summary>
		/// Toggles the current display setting.
		/// Use this as a button event wrapper.
		/// </summary>
		public void SetDisplay ( )
		{
			// Toggle the current display setting
			displayValue = display.IsOptionTrue;

			// Set whether or not the apply button should be interactable
			apply.interactable = SettingUpdateCheck ( );
		}

		/// <summary>
		/// Increments or decrements the current resolution setting.
		/// Use this as a button event wrapper.
		/// </summary>
		public void SetResolution ( )
		{
			// Set current resolution
			resolutionValue = resolution.OptionIndex;

			// Set whether or not the apply button should be interactable
			apply.interactable = SettingUpdateCheck ( );
		}

		/// <summary>
		/// Sets the current quality setting.
		/// Use this as a button event wrapper.
		/// </summary>
		private void SetQuality ( )
		{
			// Store the current quality setting
			qualityValue = quality.OptionIndex;

			// Set whether or not the apply button should be interactable
			apply.interactable = SettingUpdateCheck ( );
		}

		/// <summary>
		/// Toggles the current vsync setting.
		/// Use this as a button event wrapper.
		/// </summary>
		public void SetVsync ( )
		{
			// Toggle the current vsync setting
			vsyncValue = vsync.IsOptionTrue;

			// Set whether or not the apply button should be interactable
			apply.interactable = SettingUpdateCheck ( );
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
			// Set the current display setting
			SetDisplay ( Settings.DISPLAY_DEFAULT );

			// Set the current resolution setting
			SetResolution ( Screen.resolutions [ Screen.resolutions.Length - 1 ].width, Screen.resolutions [ Screen.resolutions.Length - 1 ].height );

			// Set the current quality setting
			SetQuality ( Settings.QUALITY_DEFAULT );

			// Set the current vsync setting
			SetVsync ( Settings.VSYNC_DEFAULT == 1 );

			// Apply any changes
			ApplySettings ( );
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Gets the list of resolutions available.
		/// </summary>
		/// <returns> The list of resolutions available to the device. </returns>
		private string [ ] GetResolutions ( )
		{
			// Create list of resolutions
			string [ ] resolutions = new string [ Screen.resolutions.Length ];

			// Populate resolutions
			for ( int i = 0; i < resolutions.Length; i++ )
				resolutions [ i ] = Screen.resolutions [ i ].width + " x " + Screen.resolutions [ i ].height;

			// Return list
			return resolutions;
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
			display.SetOption ( value, false );
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
			resolution.SetOption ( resolutionValue, false );
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
			quality.SetOption ( index, false );
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
			vsync.SetOption ( value, false );
		}

		/// <summary>
		/// Checks if any of the current settings differ from the actual settings.
		/// Use this to determine if the Apply button should be active.
		/// </summary>
		/// <returns> Whether or not any setting differs from the actual settings. </returns>
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
		/// Checks if any of the saved settings differ from the default settings.
		/// Use this to determine if the Reset button should be active.
		/// </summary>
		/// <returns> Whether or not any setting differs from the defaults. </returns>
		private bool SettingDefaultCheck ( )
		{
			// Check if the display setting is the default
			if ( Settings.Display != Settings.DISPLAY_DEFAULT )
				return true;

			// Check if the resolution setting has been changed
			if ( Settings.ResolutionWidth != Screen.resolutions [ Screen.resolutions.Length - 1 ].width || Settings.ResolutionHeight != Screen.resolutions [ Screen.resolutions.Length - 1 ].height )
				return true;

			// Check if the quality setting is the default
			if ( Settings.Quality != Settings.QUALITY_DEFAULT )
				return true;

			// Check if the vsync setting is the default
			if ( Settings.Vsync != Settings.VSYNC_DEFAULT )
				return true;

			// Return that all settings are at default
			return false;
		}

		#endregion // Private Functions
	}
}
