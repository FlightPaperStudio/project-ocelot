using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroSettingSlideMenu : SlideMenu
{
	#region Private Classes

	[System.Serializable]
	private class AbilitySettingControls
	{
		public CarouselButton EnableAbilityCarousel;
		public Slider DurationSlider;
		public TextMeshProUGUI DurationText;
		public string DurationPrompt = "Turn";
		public Slider CooldownSlider;
		public TextMeshProUGUI CooldownText;
	}

	#endregion // Private Classes

	#region UI Elements

	[SerializeField]
	private CarouselButton enableHeroCarousel;

	[SerializeField]
	private AbilitySettingControls [ ] abilityControls;

	#endregion // UI Elements

	#region Menu Data

	[SerializeField]
	private CustomMatchSettingsMenu customSettingsManager;

	[SerializeField]
	private int heroIndex;

	#endregion // Menu Data

	#region SlideMenu Override Functions

	protected override void OpenMenu ( bool playAnimation )
	{
		// Open the menu
		base.OpenMenu ( playAnimation );

		// Display the hero settings
		DisplayHeroSetting ( false );
	}

	#endregion // SlideMenu Override Functions

	#region Public Functions

	/// <summary>
	/// Sets the enable hero setting.
	/// Use this function as a button click event wrapper for the Enable Hero Carousel buttons.
	/// </summary>
	public void UpdateEnableHeroSetting ( )
	{
		// Set the enable hero setting
		customSettingsManager.HeroSettings [ heroIndex ].IsEnabled = enableHeroCarousel.OptionIndex == 1;
	}

	#region Ability 1 Functions

	/// <summary>
	/// Sets the enable ability setting for ability 1.
	/// Use this function as a button click event wrapper for the Enable Ability 1 Carousel buttons.
	/// </summary>
	public void UpdateEnableAbility1 ( )
	{
		// Set the enable ability setting
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability1 != null )
			customSettingsManager.HeroSettings [ heroIndex ].Ability1.IsEnabled = abilityControls [ 0 ].EnableAbilityCarousel.IsOptionTrue;
	}

	/// <summary>
	/// Sets the duration setting for ability 1.
	/// Use this function as a slider update event wrapper for the Ability 1 Duration slider.
	/// </summary>
	/// <param name="value"> The value of the slider. </param>
	public void SetAbility1Duration ( float value )
	{
		// Check for ability 1
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability1 != null )
		{
			// Set ability 1's duration setting
			customSettingsManager.HeroSettings [ heroIndex ].Ability1.Duration = (int)value;

			// Set ability 1's duration slider
			abilityControls [ 0 ].DurationSlider.value = value;

			// Set ability 1's duration text readout
			abilityControls [ 0 ].DurationText.text = customSettingsManager.HeroSettings [ heroIndex ].Ability1.Duration == 1 ? "1 " + abilityControls [ 0 ].DurationPrompt : customSettingsManager.HeroSettings [ heroIndex ].Ability1.Duration + " " + abilityControls [ 0 ].DurationPrompt + "s";
		}
	}

	/// <summary>
	/// Sets the cooldown setting for ability 1.
	/// Use this function as a slider update event wrapper for the Ability 1 Cooldown slider.
	/// </summary>
	/// <param name="value"> The value of the slider. </param>
	public void SetAbility1Cooldown ( float value )
	{
		// Check for ability 1
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability1 != null )
		{
			// Set ability 1's duration setting
			customSettingsManager.HeroSettings [ heroIndex ].Ability1.Cooldown = (int)value;

			// Set ability 1's duration slider
			abilityControls [ 0 ].CooldownSlider.value = value;

			// Set ability 1's duration text readout
			abilityControls [ 0 ].CooldownText.text = customSettingsManager.HeroSettings [ heroIndex ].Ability1.Cooldown == 1 ? "1 Turn" : customSettingsManager.HeroSettings [ heroIndex ].Ability1.Cooldown + " Turns";
		}
	}

	#endregion // Ability 1 Functions

	#region Ability 2 Functions

	/// <summary>
	/// Sets the enable ability setting for ability 2.
	/// Use this function as a button click event wrapper for the Enable Ability 2 Carousel buttons.
	/// </summary>
	public void UpdateEnableAbility2 ( )
	{
		// Set the enable ability setting
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability2 != null )
			customSettingsManager.HeroSettings [ heroIndex ].Ability2.IsEnabled = abilityControls [ 1 ].EnableAbilityCarousel.IsOptionTrue;
	}

	/// <summary>
	/// Sets the duration setting for ability 2.
	/// Use this function as a slider update event wrapper for the Ability 2 Duration slider.
	/// </summary>
	/// <param name="value"> The value of the slider. </param>
	public void SetAbility2Duration ( float value )
	{
		// Check for ability 2
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability2 != null )
		{
			// Set ability 2's duration setting
			customSettingsManager.HeroSettings [ heroIndex ].Ability2.Duration = (int)value;

			// Set ability 2's duration slider
			abilityControls [ 1 ].DurationSlider.value = value;

			// Set ability 2's duration text readout
			abilityControls [ 1 ].DurationText.text = customSettingsManager.HeroSettings [ heroIndex ].Ability2.Duration == 1 ? "1 " + abilityControls [ 1 ].DurationPrompt : customSettingsManager.HeroSettings [ heroIndex ].Ability2.Duration + " " + abilityControls [ 1 ].DurationPrompt + "s";
		}
	}

	/// <summary>
	/// Sets the cooldown setting for ability 2.
	/// Use this function as a slider update event wrapper for the Ability 2 Cooldown slider.
	/// </summary>
	/// <param name="value"> The value of the slider. </param>
	public void SetAbility2Cooldown ( float value )
	{
		// Check for ability 2
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability2 != null )
		{
			// Set ability 2's duration setting
			customSettingsManager.HeroSettings [ heroIndex ].Ability2.Cooldown = (int)value;

			// Set ability 2's duration slider
			abilityControls [ 1 ].CooldownSlider.value = value;

			// Set ability 2's duration text readout
			abilityControls [ 1 ].CooldownText.text = customSettingsManager.HeroSettings [ heroIndex ].Ability2.Cooldown == 1 ? "1 Turn" : customSettingsManager.HeroSettings [ heroIndex ].Ability2.Cooldown + " Turns";
		}
	}

	#endregion // Ability 2 Functions

	#region Ability 3 Functions

	/// <summary>
	/// Sets the enable ability setting for ability 3.
	/// Use this function as a button click event wrapper for the Enable Ability 3 Carousel buttons.
	/// </summary>
	public void UpdateEnableAbility3 ( )
	{
		// Set the enable ability setting
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability3 != null )
			customSettingsManager.HeroSettings [ heroIndex ].Ability3.IsEnabled = abilityControls [ 2 ].EnableAbilityCarousel.OptionIndex == 1;
	}

	/// <summary>
	/// Sets the duration setting for ability 3.
	/// Use this function as a slider update event wrapper for the Ability 3 Duration slider.
	/// </summary>
	/// <param name="value"> The value of the slider. </param>
	public void SetAbility3Duration ( float value )
	{
		// Check for ability 3
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability3 != null )
		{
			// Set ability 3's duration setting
			customSettingsManager.HeroSettings [ heroIndex ].Ability3.Duration = (int)value;

			// Set ability 3's duration slider
			abilityControls [ 2 ].DurationSlider.value = value;

			// Set ability 3's duration text readout
			abilityControls [ 2 ].DurationText.text = customSettingsManager.HeroSettings [ heroIndex ].Ability3.Duration == 1 ? "1 " + abilityControls [ 2 ].DurationPrompt : customSettingsManager.HeroSettings [ heroIndex ].Ability3.Duration + " " + abilityControls [ 2 ].DurationPrompt + "s";
		}
	}

	/// <summary>
	/// Sets the cooldown setting for ability 3.
	/// Use this function as a slider update event wrapper for the Ability 3 Cooldown slider.
	/// </summary>
	/// <param name="value"> The value of the slider. </param>
	public void SetAbility3Cooldown ( float value )
	{
		// Check for ability 3
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability3 != null )
		{
			// Set ability 3's duration setting
			customSettingsManager.HeroSettings [ heroIndex ].Ability3.Cooldown = (int)value;

			// Set ability 3's duration slider
			abilityControls [ 2 ].CooldownSlider.value = value;

			// Set ability 3's duration text readout
			abilityControls [ 2 ].CooldownText.text = customSettingsManager.HeroSettings [ heroIndex ].Ability3.Cooldown == 1 ? "1 Turn" : customSettingsManager.HeroSettings [ heroIndex ].Ability3.Cooldown + " Turns";
		}
	}

	#endregion // Ability 3 Functions

	/// <summary>
	/// Resets the hero's settings to their defaults
	/// Use this funciton as a button click event wrapper.
	/// </summary>
	public void ResetHeroSetting ( )
	{
		// Reset the hero setting
		customSettingsManager.ResetHeroSetting ( heroIndex );

		// Display the hero settings
		DisplayHeroSetting ( true );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Displays the current hero settings.
	/// </summary>
	/// <param name="playAnimations"> Whether or not the slide animations should play for the carousel buttons. </param>
	private void DisplayHeroSetting ( bool playAnimations )
	{
		// Display if the hero is enabled
		enableHeroCarousel.SetOption ( customSettingsManager.HeroSettings [ heroIndex ].IsEnabled, playAnimations );

		// Display if ability 1 is enabled
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability1 != null && abilityControls [ 0 ].EnableAbilityCarousel != null )
			abilityControls [ 0 ].EnableAbilityCarousel.SetOption ( customSettingsManager.HeroSettings [ heroIndex ].Ability1.IsEnabled, playAnimations );

		// Display the duration of ability 1
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability1 != null && abilityControls [ 0 ].DurationSlider != null )
			SetAbility1Duration ( customSettingsManager.HeroSettings [ heroIndex ].Ability1.Duration );

		// Display the cooldown of ability 1
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability1 != null && abilityControls [ 0 ].CooldownSlider != null )
			SetAbility1Cooldown ( customSettingsManager.HeroSettings [ heroIndex ].Ability1.Cooldown );

		// Display if ability 2 is enabled
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability2 != null && abilityControls [ 1 ].EnableAbilityCarousel != null )
			abilityControls [ 1 ].EnableAbilityCarousel.SetOption ( customSettingsManager.HeroSettings [ heroIndex ].Ability2.IsEnabled, playAnimations );

		// Display the duration of ability 2
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability2 != null && abilityControls [ 1 ].DurationSlider != null )
			SetAbility2Duration ( customSettingsManager.HeroSettings [ heroIndex ].Ability2.Duration );

		// Display the cooldown of ability 2
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability2 != null && abilityControls [ 1 ].CooldownSlider != null )
			SetAbility2Cooldown ( customSettingsManager.HeroSettings [ heroIndex ].Ability2.Cooldown );

		// Display if ability 3 is enabled
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability3 != null && abilityControls [ 2 ].EnableAbilityCarousel != null )
			abilityControls [ 2 ].EnableAbilityCarousel.SetOption ( customSettingsManager.HeroSettings [ heroIndex ].Ability3.IsEnabled, playAnimations );

		// Display the duration of ability 3
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability3 != null && abilityControls [ 2 ].DurationSlider != null )
			SetAbility3Duration ( customSettingsManager.HeroSettings [ heroIndex ].Ability3.Duration );

		// Display the cooldown of ability 3
		if ( customSettingsManager.HeroSettings [ heroIndex ].Ability3 != null && abilityControls [ 2 ].CooldownSlider != null )
			SetAbility3Cooldown ( customSettingsManager.HeroSettings [ heroIndex ].Ability3.Cooldown );
	}

	#endregion // Private Functions
}
