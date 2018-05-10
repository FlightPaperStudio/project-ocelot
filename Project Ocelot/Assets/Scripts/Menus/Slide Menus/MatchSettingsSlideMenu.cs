using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchSettingsSlideMenu : SlideMenu
{
	#region UI Elements

	[SerializeField]
	private CarouselButton matchTypeCarousel;

	[SerializeField]
	private CarouselButton turnTimerCarousel;

	[SerializeField]
	private GameObject timePerTurnPanel;

	[SerializeField]
	private Slider timePerTurnSlider;

	[SerializeField]
	private TextMeshProUGUI timePerTurnText;

	#endregion // UI Elements

	#region Menu Data

	[SerializeField]
	private CustomMatchSettingsMenu customSettingsManager;

	#endregion // Menu Data

	#region SlideMenu Override Functions

	protected override void OpenMenu ( bool playAnimation )
	{
		// Open the menu
		base.OpenMenu ( playAnimation );

		// Display the match settings
		DisplayMatchSettings ( false );
	}

	#endregion // SlideMenu Override Functions

	#region Public Functions

	/// <summary>
	/// Sets the match type setting based on the carousel option.
	/// Use this function as a button click event wrapper for the Match Type Carousel buttons.
	/// </summary>
	public void UpdateMatchType ( )
	{
		// Check carousel index
		switch ( matchTypeCarousel.OptionIndex )
		{
		case 0:
			customSettingsManager.MatchTypeSetting = MatchType.CustomClassic;
			break;
		case 1:
			customSettingsManager.MatchTypeSetting = MatchType.CustomMirror;
			break;
		case 2:
			customSettingsManager.MatchTypeSetting = MatchType.CustomRumble;
			break;
		}
	}

	/// <summary>
	/// Sets the turn timer setting based on the carousel option.
	/// Use this function as a button click event wrapper for the Turn Timer Carousel buttons.
	/// </summary>
	public void UpdateTurnTimer ( )
	{
		// Set the turn timer setting
		customSettingsManager.EnableTurnTimerSetting = turnTimerCarousel.OptionIndex == 1;

		// Display or hide the time per turn controls based on whether or not the turn timer is enabled
		timePerTurnPanel.SetActive ( customSettingsManager.EnableTurnTimerSetting );
	}

	/// <summary>
	/// Sets the time per turn setting base on the slider value.
	/// Use this function as a slider update event wrapper.
	/// </summary>
	/// <param name="value"> The value of the slider. </param>
	public void SetTimePerTurn ( float value )
	{
		// Set the time per turn setting
		customSettingsManager.TimePerTurnSetting = 30f + ( 15f * value );

		// Set slider postion
		timePerTurnSlider.value = value;

		// Display the amount of time per turn
		timePerTurnText.text = customSettingsManager.TimePerTurnSetting < 60f ? customSettingsManager.TimePerTurnSetting + " sec" : string.Format ( "{0:0}:{1:00} min", customSettingsManager.TimePerTurnSetting / 60, customSettingsManager.TimePerTurnSetting % 60 );
	}

	/// <summary>
	/// Resets the match settings to their defaults.
	/// Use this function as a button click event wrapper.
	/// </summary>
	public void ResetMatchSettings ( )
	{
		// Reset match settings to their defaults
		customSettingsManager.ResetMatchSettings ( );

		// Display current match settings
		DisplayMatchSettings ( true );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Displays the current match settings.
	/// </summary>
	/// <param name="playAnimations"> Whether or not slide animations should be played for the carousel buttons. </param>
	private void DisplayMatchSettings ( bool playAnimations )
	{
		// Display the match type setting
		switch ( customSettingsManager.MatchTypeSetting )
		{
		case MatchType.CustomClassic:
			matchTypeCarousel.SetOption ( 0, playAnimations );
			break;
		case MatchType.CustomMirror:
			matchTypeCarousel.SetOption ( 1, playAnimations );
			break;
		case MatchType.CustomRumble:
			matchTypeCarousel.SetOption ( 2, playAnimations );
			break;
		}

		// Display the turn timer setting
		turnTimerCarousel.SetOption ( customSettingsManager.EnableTurnTimerSetting, playAnimations );

		// Display or hide the time per turn controls based on whether or not the turn timer is enabled
		timePerTurnPanel.SetActive ( customSettingsManager.EnableTurnTimerSetting );

		// Display the time per turn setting
		SetTimePerTurn ( ( customSettingsManager.TimePerTurnSetting - 30f ) / 15f );
	}

	#endregion // Private Functions
}
