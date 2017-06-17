using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomMatchSettingsMenu : Menu 
{
	// UI elements
	public Scrollbar scroll;
	public TextMeshProUGUI typeDisplay;
	public TextMeshProUGUI turnTimerDisplay;
	public Slider timeSlider;
	public TextMeshProUGUI timeDisplay;
	public Slider sizeSlider;
	public TextMeshProUGUI sizeDisplay;
	public TextMeshProUGUI stackingDisplay;
	public TextMeshProUGUI [ ] selectionDisplay;
	public TextMeshProUGUI [ ] enableAbilityDisplay1;
	public Slider [ ] durationSlider1;
	public TextMeshProUGUI [ ] durationDisplay1;
	public Slider [ ] cooldownSlider1;
	public TextMeshProUGUI [ ] cooldownDisplay1;
	public TextMeshProUGUI [ ] enableAbilityDisplay2;
	public Slider [ ] durationSlider2;
	public TextMeshProUGUI [ ] durationDisplay2;
	public Slider [ ] cooldownSlider2;
	public TextMeshProUGUI [ ] cooldownDisplay2;

	// Menu information
	private MatchType typeValue;
	private bool turnTimerValue;
	private float timeValue;
	private int sizeValue;
	private bool stackingValue;
	private List<HeroSettings> heroValue = new List<HeroSettings> ( );
	public PopUpMenu popUp;
	public LoadingScreen load;

	/// <summary>
	/// Opens the menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true, params object [ ] values )
	{
		// Open the menu
		base.OpenMenu ( closeParent, values );

		// Set the scroll bar to the top of the menu
		scroll.value = 1;

		// Set default match type
		SetMatchType ( MatchType.CustomClassic );

		// Set default turn timer
		SetTurnTimer ( true );
		SetTimePerTurn ( 4f );

		// Set default team size
		SetAbilitiesPerTeam ( 3 );

		// Set default stacking
		SetAbilityStacking ( false );

		// Set default special ability settings
		heroValue.Clear ( );
		for ( int i = 0; i < HeroInfo.list.Count; i++ )
		{
			// Set hero setting
			HeroSettings h = new HeroSettings ( HeroInfo.list [ i ].id, true, true, (Ability.AbilityType)HeroInfo.list [ i ].ability1.type, HeroInfo.list [ i ].ability1.duration, HeroInfo.list [ i ].ability1.cooldown, true, (Ability.AbilityType)HeroInfo.list [ i ].ability2.type, HeroInfo.list [ i ].ability2.duration, HeroInfo.list [ i ].ability2.cooldown );
			heroValue.Add ( h );

			// Set selection
			SetSelection ( h, h.selection, selectionDisplay [ i ] );

			// Set ability 1 enabled
			SetEnableAbility ( h.ability1, true, enableAbilityDisplay1 [ i ] );

			// Set ability 1 duration
			if ( durationSlider1 [ i ] != null )
			{
				// Provide Armor attack prompt
				if ( i == 0 )
					SetDuration ( h.ability1, h.ability1.duration, durationSlider1 [ i ], durationDisplay1 [ i ], " Attack", " Attacks" );
				else
					SetDuration ( h.ability1, h.ability1.duration, durationSlider1 [ i ], durationDisplay1 [ i ] );
			}

			// Set ability 1 cooldown
			if ( cooldownSlider1 [ i ] != null )
				SetCooldown ( h.ability1, h.ability1.cooldown, cooldownSlider1 [ i ], cooldownDisplay1 [ i ] );

			// Set ability 2 enabled
			if ( enableAbilityDisplay2 [ i ] != null )
				SetEnableAbility ( h.ability2, true, enableAbilityDisplay2 [ i ] );

			// Set ability 2 duration
			if ( durationSlider2 [ i ] != null )
				SetDuration ( h.ability2, h.ability2.duration, durationSlider2 [ i ], durationDisplay2 [ i ] );

			// Set ability 2 cooldown
			if ( cooldownSlider2 [ i ] != null )
				SetCooldown ( h.ability2, h.ability2.cooldown, cooldownSlider2 [ i ], cooldownDisplay2 [ i ] );
		}
	}

	/// <summary>
	/// Sets the current match type setting.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetMatchType ( bool increment )
	{
		// Check current type
		switch ( typeValue )
		{
		case MatchType.CustomClassic:
			// Check if incrementing or decrementing
			if ( increment )
				SetMatchType ( MatchType.CustomMirror );
			else
				SetMatchType ( MatchType.CustomRumble );
			break;
		case MatchType.CustomMirror:
			// Check if incrementing or decrementing
			if ( increment )
				SetMatchType ( MatchType.CustomRumble );
			else
				SetMatchType ( MatchType.CustomClassic );
			break;
		case MatchType.CustomRumble:
			// Check if incrementing or decrementing
			if ( increment )
				SetMatchType ( MatchType.CustomClassic );
			else
				SetMatchType ( MatchType.CustomMirror );
			break;
		}
	}

	/// <summary>
	/// Sets the current match type setting.
	/// Does not apply to the actual match settings until the match starts.
	/// </summary>
	private void SetMatchType ( MatchType value )
	{
		// Store match type
		typeValue = value;

		// Display the match type
		switch ( typeValue )
		{
		case MatchType.CustomClassic:
			typeDisplay.text = "Classic";
			break;
		case MatchType.CustomMirror:
			typeDisplay.text = "Mirror";
			break;
		case MatchType.CustomRumble:
			typeDisplay.text = "Rumble";
			break;
		}
	}

	/// <summary>
	/// Toggles the current turn timer setting.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetTurnTimer ( )
	{
		// Toggle turn timer
		SetTurnTimer ( !turnTimerValue );
	}

	/// <summary>
	/// Sets the current turn timer setting.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetTurnTimer ( bool value )
	{
		// Store turn timer
		turnTimerValue = value;

		// Display turn timer
		if ( turnTimerValue )
			turnTimerDisplay.text = "Enabled";
		else
			turnTimerDisplay.text = "Disabled";
	}

	/// <summary>
	/// Sets the currnet time per turn setting.
	/// Does not apply to the actual match setting until the match starts.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetTimePerTurn ( float value )
	{
		// Store time per turn
		timeValue = 30f + ( 15f * value );

		// Set slider
		timeSlider.value = value;

		// Display time per turn
		int min = (int)timeValue / 60;
		int sec = (int)timeValue % 60;
		timeDisplay.text = string.Format ( "{0:0}:{1:00}", min, sec );
	}

	/// <summary>
	/// Sets the current team size setting.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetAbilitiesPerTeam ( float value )
	{
		// Check if enough abilities are enabled
		if ( !stackingValue && value > SelectionCheck ( ) )
		{
			// Prevent the slider from updating
			sizeSlider.value = (float)sizeValue;

			// Prompt the user that there aren't enough abilities enabled
			popUp.OpenMenu ( false, true, "More Special Abilities need to be enabled for selection than the current number of Special Abilities Per Team if Special Ability Stacking is disabled", null, null );
		}
		else
		{
			SetAbilitiesPerTeam ( (int)value );
		}
	}

	/// <summary>
	/// Sets the current team size setting.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetAbilitiesPerTeam ( int value )
	{
		// Store team size
		sizeValue = value;

		// Set slider
		sizeSlider.value = (float)value;

		// Display team size
		sizeDisplay.text = value.ToString ( );
	}

	/// <summary>
	/// Toggles the current ability stacking setting.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetAbilityStacking ( )
	{
		// Check if stacking can be disabled
		if ( stackingValue && SelectionCheck ( ) < sizeValue )
		{
			// Prompt that stacking cannot be disabled
			popUp.OpenMenu ( false, true, "Special Ability Stacking cannot be disabled if there are less than the number of Special Abilities Per Team currently enabled for selection.", null, null );
		}
		else
		{
			// Toggle stacking
			SetAbilityStacking ( !stackingValue );
		}
	}

	/// <summary>
	/// Sets the current ability stacking setting.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetAbilityStacking ( bool value )
	{
		// Store stacking
		stackingValue = value;

		// Display stacking
		if ( stackingValue )
			stackingDisplay.text = "Enabled";
		else
			stackingDisplay.text = "Disabled";
	}

	/// <summary>
	/// Toggles the current selection setting for a hero.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetSelection ( int index )
	{
		// Check stacking and the amount of specials enabled
		if ( !stackingValue && SelectionCheck ( ) == sizeValue )
		{
			// Prompt that at least three specials need to be enabled if stacking is not enabled
			popUp.OpenMenu ( false, true, "At least the number of Special Abilities Per Team needs to be enabled for selection while Special Ability Stacking is disabled.", null, null );
		}
		else if ( stackingValue && SelectionCheck ( ) == 1 && heroValue [ index ].selection && sizeValue != 0 )
		{
			// Prompt that at least one specials needs to be enabled
			popUp.OpenMenu ( false, true, "At least one Special Ability needs to be enabled for selection.", null, null );
		}
		else
		{
			// Toggle selection
			SetSelection ( heroValue [ index ], !heroValue [ index ].selection, selectionDisplay [ index ] );
		}
	}

	/// <summary>
	/// Sets the current selection setting for a hero.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetSelection ( HeroSettings hero, bool value, TextMeshProUGUI display )
	{
		// Store selection setting
		hero.selection = value;

		// Display selection
		if ( hero.selection )
			display.text = "Enabled";
		else
			display.text = "Disabled";
	}

	/// <summary>
	/// Toggles the current ability 1 enable setting for a hero.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetEnableAbility1 ( int index )
	{
		// Set ability 1 enable setting
		SetEnableAbility ( heroValue [ index ].ability1, !heroValue [ index ].ability1.enabled, enableAbilityDisplay1 [ index ] );
	}

	/// <summary>
	/// Toggles the current ability 2 enable setting for a hero.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetEnableAbility2 ( int index )
	{
		// Set ability 2 enable setting
		SetEnableAbility ( heroValue [ index ].ability2, !heroValue [ index ].ability2.enabled, enableAbilityDisplay2 [ index ] );
	}

	/// <summary>
	/// Sets the current ability enable setting for a hero.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetEnableAbility ( AbilitySettings ability, bool value, TextMeshProUGUI display )
	{
		// Store ability enable setting
		ability.enabled = value;

		// Display ability enable setting
		if ( ability.enabled )
			display.text = "Enabled";
		else
			display.text = "Disabled";
	}

	/// <summary>
	/// Sets the current duration setting for Hero 1's Armor ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetArmorDuration ( float value )
	{
		// Set duration
		SetDuration ( heroValue [ 0 ].ability1, (int)value, durationSlider1 [ 0 ], durationDisplay1 [ 0 ], " Attack", " Attacks" );
	}

	/// <summary>
	/// Sets the current duration setting for an ability.
	/// Does not apply to the actual match settings until the match starts.
	/// </summary>
	private void SetDuration ( AbilitySettings ability, int value, Slider slider, TextMeshProUGUI display, string prompt = " Turn", string prompts = " Turns" )
	{
		// Store duration
		ability.duration = value;

		// Set slider
		slider.value = (float)value;

		// Display duration
		if ( ability.duration != 1 )
			display.text = value + prompts;
		else
			display.text = value + prompt;
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 1's Self-Destruct/Recall ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetSelfDestructRecallCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValue [ 0 ].ability2, (int)value, cooldownSlider2 [ 0 ], cooldownDisplay2 [ 0 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 2's Catapult ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetCatapultCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValue [ 1 ].ability1, (int)value, cooldownSlider1 [ 1 ], cooldownDisplay1 [ 1 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 2's Grapple ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetGrappleCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValue [ 1 ].ability2, (int)value, cooldownSlider2 [ 1 ], cooldownDisplay2 [ 1 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 6's Obstruction ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetObstuctionCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValue [ 2 ].ability2, (int)value, cooldownSlider2 [ 2 ], cooldownDisplay2 [ 2 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 8's Blink ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetBlinkCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValue [ 3 ].ability1, (int)value, cooldownSlider1 [ 3 ], cooldownDisplay1 [ 3 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 8's Translocator ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetTranslocatorCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValue [ 3 ].ability2, (int)value, cooldownSlider2 [ 3 ], cooldownDisplay2 [ 3 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 9's Run The Ropes ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetRunTheRopesCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValue [ 4 ].ability1, (int)value, cooldownSlider1 [ 4 ], cooldownDisplay1 [ 4 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 9's Taunt ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetTauntCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValue [ 4 ].ability2, (int)value, cooldownSlider2 [ 4 ], cooldownDisplay2 [ 4 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for a special ability.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetCooldown ( AbilitySettings ability, int value, Slider slider, TextMeshProUGUI display )
	{
		// Store cooldown
		ability.cooldown = value;

		// Set slider
		slider.value = (float)value;

		// Display cooldown
		if ( ability.cooldown == 1 )
			display.text = "1 Turn";
		else
			display.text = value + " Turns";
	}

	/// <summary>
	/// Begins the custom match.
	/// </summary>
	public void BeginMatch ( )
	{
		// Start load
		load.BeginLoad ( );

		// Set match settings
		MatchSettings.SetMatchSettings ( typeValue, turnTimerValue, timeValue, sizeValue, stackingValue, heroValue );

		// Begin the match
		if ( typeValue == MatchType.CustomMirror )
		{
			load.LoadScene ( Scenes.Classic );
		}
		else if ( sizeValue == 0 )
		{
			if ( typeValue == MatchType.CustomClassic )
			{
				load.LoadScene ( Scenes.Classic );
			}
			else if ( typeValue == MatchType.CustomRumble )
			{
				load.LoadScene ( Scenes.Rumble );
			}
		}
		else
		{
			load.LoadScene ( Scenes.MatchSetup );
		}
	}

	/// <summary>
	/// Returns the current number of special abilities currently enabled.
	/// </summary>
	private int SelectionCheck ( )
	{
		// Count the number of enabled heroes
		int count = 0;
		foreach ( HeroSettings h in heroValue )
			if ( h.selection )
				count++;

		// Return the number of enabled heroes
		return count;
	}
}
