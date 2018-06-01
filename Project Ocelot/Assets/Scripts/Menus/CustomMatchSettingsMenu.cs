using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomMatchSettingsMenu : Menu 
{
	#region UI Elements

	[SerializeField]
	private SlideMenu settingsOverviewMenu;

	#endregion // UI Elements

	#region Custom Match Settings Data

	[HideInInspector]
	public MatchType MatchTypeSetting;

	[HideInInspector]
	public bool EnableTurnTimerSetting;

	[HideInInspector]
	public float TimePerTurnSetting;

	[HideInInspector]
	public int HeroesPerTeamSetting;

	[HideInInspector]
	public bool HeroLimitSetting;

	[HideInInspector]
	public List<UnitSettingData> HeroSettings = new List<UnitSettingData> ( );

	public PopUpMenu popUp;
	public LoadingScreen load;

	#endregion // Custom Match Settings Data

	#region Menu Override Functions

	/// <summary>
	/// Opens the menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true )
	{
		// Open the menu
		base.OpenMenu ( closeParent );

		// Initialize the custom match setttings
		InitializeCustomMatchSettings ( );

		// Open the settings overview menu by default
		settingsOverviewMenu.OpenMenu ( );
	}

	#endregion // Menu Override Functions

	#region Public Functions

	/// <summary>
	/// Resets all custom match settings to their defaults.
	/// </summary>
	public void ResetCustomMatchSettings ( )
	{
		// Reset the match settings
		ResetMatchSettings ( );

		// Reset the team settings
		ResetTeamSettings ( );

		// Reset each hero setting
		for ( int i = 0; i < HeroSettings.Count; i++ )
			ResetHeroSetting ( i );
	}

	/// <summary>
	/// Resets only the match settings of the custom match settings to their defaults.
	/// </summary>
	public void ResetMatchSettings ( )
	{
		// Set the match type
		MatchTypeSetting = MatchType.CustomClassic;

		// Set the turn timer
		EnableTurnTimerSetting = true;
		TimePerTurnSetting = 90f;
	}

	/// <summary>
	/// Resets only the team settings of the custom match settings to their defaults.
	/// </summary>
	public void ResetTeamSettings ( )
	{
		// Set the heroes per team
		HeroesPerTeamSetting = 3;

		// Set the hero limit
		HeroLimitSetting = true;
	}

	/// <summary>
	/// Resets the hero settings of a specified hero to their defaults.
	/// </summary>
	/// <param name="index"> The index of the hero in the hero list. </param>
	public void ResetHeroSetting ( int index )
	{
		// Get default hero setting
		UnitSettingData defaultSetting = UnitDatabase.GetUnit ( HeroSettings [ index ].ID );

		// Set the hero as enabled
		HeroSettings [ index ].IsEnabled = defaultSetting.IsEnabled;

		// Set ability 1 to its defaults
		if ( defaultSetting.Ability1 != null )
		{
			// Set ability 1 as enabled
			HeroSettings [ index ].Ability1.IsEnabled = defaultSetting.Ability1.IsEnabled;

			// Set ability 1 duration
			HeroSettings [ index ].Ability1.Duration = defaultSetting.Ability1.Duration;

			// Set ability 1 cooldown
			HeroSettings [ index ].Ability1.Cooldown = defaultSetting.Ability1.Cooldown;
		}

		// Set ability 2 to its defaults
		if ( defaultSetting.Ability2 != null )
		{
			// Set ability 2 as enabled
			HeroSettings [ index ].Ability2.IsEnabled = defaultSetting.Ability2.IsEnabled;

			// Set ability 2 duration
			HeroSettings [ index ].Ability2.Duration = defaultSetting.Ability2.Duration;

			// Set ability 2 cooldown
			HeroSettings [ index ].Ability2.Cooldown = defaultSetting.Ability2.Cooldown;
		}

		// Set ability 3 to its defaults
		if ( defaultSetting.Ability3 != null )
		{
			// Set ability 3 as enabled
			HeroSettings [ index ].Ability3.IsEnabled = defaultSetting.Ability3.IsEnabled;

			// Set ability 3 duration
			HeroSettings [ index ].Ability3.Duration = defaultSetting.Ability3.Duration;

			// Set ability 3 cooldown
			HeroSettings [ index ].Ability3.Cooldown = defaultSetting.Ability3.Cooldown;
		}
	}

	/// <summary>
	/// Begins the match if the custom match settings are valid.
	/// </summary>
	public void BeginMatch ( )
	{
		// Check if the match settings are valid
		if ( ValidateCustomMatchSettings ( ) )
		{
			// Start load
			load.BeginLoad ( );

			// Set match settings
			MatchSettings.SetMatchSettings ( MatchTypeSetting, EnableTurnTimerSetting, TimePerTurnSetting, HeroesPerTeamSetting, HeroLimitSetting, HeroSettings );

			// Begin the match
			load.LoadScene ( Scenes.MATCH_SETUP );
		}
		else
		{
			// Display warning pop up
			popUp.SetAcknowledgementPopUp ( "There are not enough heroes enabled per team!", null );
			popUp.OpenMenu ( );
		}
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Initializes the match settings to their defaults for customization.
	/// </summary>
	private void InitializeCustomMatchSettings ( )
	{
		// Reset the match settings
		ResetMatchSettings ( );

		// Reset the team settings
		ResetTeamSettings ( );

		// Initialize new instance data for each hero
		HeroSettings.Clear ( );
		HeroSettings = UnitDatabase.GetHeroes ( );
	}

	/// <summary>
	/// Checks if the custom match settings are valid enough for a match.
	/// </summary>
	/// <returns> Whether or not the settings are valid. </returns>
	private bool ValidateCustomMatchSettings ( )
	{
		// Check if there are no heroes to choose from
		if ( HeroesPerTeamSetting > 0 && HeroSettings.FindAll ( x => x.IsEnabled == true ).Count == 0 )
			return false;

		// Check if there are not enough heroes to choose from
		if ( HeroLimitSetting && HeroSettings.FindAll ( x => x.IsEnabled == true ).Count < HeroesPerTeamSetting )
			return false;

		// Return that the settings are valid
		return true;
	}

	#endregion // Private Functions
}
