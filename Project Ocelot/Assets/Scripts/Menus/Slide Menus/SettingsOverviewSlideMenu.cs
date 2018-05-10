using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsOverviewSlideMenu : SlideMenu
{
	#region UI Elements

	[SerializeField]
	private TextMeshProUGUI matchTypeText;

	[SerializeField]
	private TextMeshProUGUI turnTimerText;

	[SerializeField]
	private TextMeshProUGUI heroesPerTeamText;

	[SerializeField]
	private TextMeshProUGUI heroLimitText;

	[SerializeField]
	private GameObject heroSettingsPanel;

	[SerializeField]
	private TextMeshProUGUI [ ] heroSettingTexts;

	#endregion // UI Elements

	#region Menu Data

	[SerializeField]
	private CustomMatchSettingsMenu customSettingsManager;

	#endregion // Menu Data

	#region SlideMenu Override Functions

	protected override void OpenMenu ( bool playAnimation )
	{
		// Opent the menu
		base.OpenMenu ( playAnimation );

		// Display the current settings
		DisplaySettingsOverview ( );
	}

	#endregion // SlideMenu Override Functions

	#region Public Functions

	/// <summary>
	/// Resets all of the current custom match settings to their defaults.
	/// Use this function as a button click event wrapper.
	/// </summary>
	public void ResetCustomMatchSettings ( )
	{
		// Reset the custom match settings to their defaults
		customSettingsManager.ResetCustomMatchSettings ( );

		// Display the new settings
		DisplaySettingsOverview ( );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Displays all of the current custom match settings.
	/// </summary>
	private void DisplaySettingsOverview ( )
	{
		// Display match type setting
		switch ( customSettingsManager.MatchTypeSetting )
		{
		case MatchType.CustomClassic:
			matchTypeText.text = "Classic Match";
			break;
		case MatchType.CustomMirror:
			matchTypeText.text = "Mirror Match";
			break;
		case MatchType.CustomRumble:
			matchTypeText.text = "Rumble Match";
			break;
		case MatchType.CustomLadder:
			matchTypeText.text = "Ladder Match";
			break;
		}

		// Display turn timer setting
		if ( !customSettingsManager.EnableTurnTimerSetting )
		{
			// Display that there is no turn timer
			turnTimerText.text = "No Turn Timer";
		}
		else
		{
			// Check for if the time per turn is less than a minute
			if ( customSettingsManager.TimePerTurnSetting < 60f )
				turnTimerText.text = customSettingsManager.TimePerTurnSetting + " sec Turn Timer";
			else
				turnTimerText.text = string.Format ( "{0:0}:{1:00}", (int)( customSettingsManager.TimePerTurnSetting / 60 ), (int)( customSettingsManager.TimePerTurnSetting % 60 ) ) + " min Turn Timer";
		}

		// Check if any heroes are available
		if ( customSettingsManager.HeroesPerTeamSetting == 0 )
		{
			// Display that no heroes are available for this match
			heroesPerTeamText.text = "No Heroes";
			heroLimitText.gameObject.SetActive ( false );
			heroSettingsPanel.SetActive ( false );
		}
		else
		{
			// Display that heroes are available for this match
			heroesPerTeamText.text = customSettingsManager.HeroesPerTeamSetting == 1 ? "1 Hero Per Team" : customSettingsManager.HeroesPerTeamSetting + " Heroes Per Team";
			heroLimitText.gameObject.SetActive ( true );
			heroSettingsPanel.SetActive ( true );

			// Display the hero limit setting
			heroLimitText.text = customSettingsManager.HeroLimitSetting ? "No Duplicate Heroes" : "No Limits";

			// Display each hero setting
			for ( int i = 0; i < heroSettingTexts.Length; i++ )
				DisplayHeroSetting ( heroSettingTexts [ i ], customSettingsManager.HeroSettings [ i ], UnitDatabase.GetUnit ( customSettingsManager.HeroSettings [ i ].ID ) );
		}
	}

	/// <summary>
	/// Displays whether the custom match setting for a hero has been modified or if the hero has been disabled.
	/// </summary>
	/// <param name="settingText"> The text prompt displaying the setting. </param>
	/// <param name="customSetting"> The custom match setting for the hero. </param>
	/// <param name="defaultSetting"> The default setting for the hero. </param>
	private void DisplayHeroSetting ( TextMeshProUGUI settingText, UnitSettingData customSetting, UnitSettingData defaultSetting )
	{
		// Display the hero's name
		settingText.text = customSetting.UnitName + " - <size=80%><i>";

		// Check if the hero is disabled
		if ( !customSetting.IsEnabled )
		{
			// Display that the hero is disabled
			settingText.text += "<color=red>Disabled";
		}
		else
		{
			// Display if the hero has been modified
			settingText.text += CheckForDefaultHeroSetting ( customSetting, defaultSetting ) ? "Default" : "<color=#FFFFC8FF>Modified";
		}
	}

	/// <summary>
	/// Checks if any settings for a hero have been modified
	/// </summary>
	/// <param name="customSetting"> The current custom match setting for the hero. </param>
	/// <param name="defaultSetting"> The default setting for the hero. </param>
	/// <returns> Whether or not all of the custom match settings are equal to the default settings for the hero. </returns>
	private bool CheckForDefaultHeroSetting ( UnitSettingData customSetting, UnitSettingData defaultSetting )
	{
		// Check ability 1
		if ( customSetting.Ability1 != null )
		{
			// Check if the ability is enabled
			if ( customSetting.Ability1.IsEnabled != defaultSetting.Ability1.IsEnabled )
				return false;

			// Check if the duration has been modified
			if ( customSetting.Ability1.Duration != defaultSetting.Ability1.Duration )
				return false;

			// Check if the cooldown has been modified
			if ( customSetting.Ability1.Cooldown != defaultSetting.Ability1.Cooldown )
				return false;
		}

		// Check ability 2
		if ( customSetting.Ability2 != null )
		{
			// Check if the ability is enabled
			if ( customSetting.Ability2.IsEnabled != defaultSetting.Ability2.IsEnabled )
				return false;

			// Check if the duration has been modified
			if ( customSetting.Ability2.Duration != defaultSetting.Ability2.Duration )
				return false;

			// Check if the cooldown has been modified
			if ( customSetting.Ability2.Cooldown != defaultSetting.Ability2.Cooldown )
				return false;
		}

		// Check ability 3
		if ( customSetting.Ability3 != null )
		{
			// Check if the ability is enabled
			if ( customSetting.Ability3.IsEnabled != defaultSetting.Ability3.IsEnabled )
				return false;

			// Check if the duration has been modified
			if ( customSetting.Ability3.Duration != defaultSetting.Ability3.Duration )
				return false;

			// Check if the cooldown has been modified
			if ( customSetting.Ability3.Cooldown != defaultSetting.Ability3.Cooldown )
				return false;
		}

		// Return that no settings have changed
		return true;
	}

	#endregion // Private Functions
}
