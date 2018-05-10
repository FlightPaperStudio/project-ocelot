using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSettingsSlideMenu : SlideMenu
{
	#region UI Elements

	[SerializeField]
	private Slider heroesPerTeamSlider;

	[SerializeField]
	private TextMeshProUGUI heroesPerTeamText;

	[SerializeField]
	private GameObject heroLimitPanel;

	[SerializeField]
	private CarouselButton heroLimitCarousel;

	#endregion // UI Elemnts

	#region Menu Data

	[SerializeField]
	private CustomMatchSettingsMenu customSettingsManager;

	#endregion // Menu Data

	#region SlideMenu Override Functions

	protected override void OpenMenu ( bool playAnimation )
	{
		// Open the menu
		base.OpenMenu ( playAnimation );

		// Display the team settings
		DisplayTeamSettings ( false );
	}

	#endregion SlideMenu Override Functions

	#region Public Functions

	/// <summary>
	/// Sets the heroes per team setting.
	/// Use this function as a slider update event wrapper.
	/// </summary>
	/// <param name="value"> The value of the slider. </param>
	public void SetHeroesPerTeam ( float value )
	{
		// Set the heroes per team setting
		customSettingsManager.HeroesPerTeamSetting = (int)value;

		// Set slider position
		heroesPerTeamSlider.value = value;

		// Display how many heroes are available per team
		if ( customSettingsManager.HeroesPerTeamSetting == 0 )
			heroesPerTeamText.text = "No Heroes";
		else
			heroesPerTeamText.text = customSettingsManager.HeroesPerTeamSetting == 1 ? "1 Hero" : customSettingsManager.HeroesPerTeamSetting + " Heroes";

		// Display or hide the hero limit controls based on whether or not any heroes are available
		heroLimitPanel.SetActive ( customSettingsManager.HeroesPerTeamSetting > 0 );
	}

	/// <summary>
	/// Sets the hero limit setting based on the carousel.
	/// Use this function as button click event wrapper for the Hero Limit Carousel buttons.
	/// </summary>
	public void UpdateHeroLimit ( )
	{
		// Set the hero limit setting
		customSettingsManager.HeroLimitSetting = heroLimitCarousel.OptionIndex == 1;
	}

	/// <summary>
	/// Resets the team settings to their defaults.
	/// Use this function as a button click event wrapper.
	/// </summary>
	public void ResetTeamSettings ( )
	{
		// Reset the team settings to their defaults
		customSettingsManager.ResetTeamSettings ( );

		// Display the team settings 
		DisplayTeamSettings ( true );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Displays the current team settings.
	/// </summary>
	/// <param name="playAnimations"> Whether or not the slide animations should be played for the carousel buttons. </param>
	private void DisplayTeamSettings ( bool playAnimations )
	{
		// Display the heroes per team setting
		SetHeroesPerTeam ( customSettingsManager.HeroesPerTeamSetting );

		// Display the hero limit setting
		heroLimitCarousel.SetOption ( customSettingsManager.HeroLimitSetting, playAnimations );
	}

	#endregion // Private Functions
}
