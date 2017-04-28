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
	public Slider [ ] cooldownSlider;
	public TextMeshProUGUI [ ] cooldownDisplay;

	// Menu information
	private MatchType typeValue;
	private bool turnTimerValue;
	private float timeValue;
	private int sizeValue;
	private bool stackingValue;
	private List<SpecialSettings> specialValue = new List<SpecialSettings> ( );
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
		specialValue.Clear ( );
		for ( int i = 0; i < SpecialInfo.list.Length; i++ )
		{
			// Set special setting
			SpecialSettings s = new SpecialSettings ( SpecialInfo.list [ i ].id, true, SpecialInfo.list [ i ].cooldown );
			specialValue.Add ( s );

			// Set selection
			SetSelection ( s, s.selection, selectionDisplay [ i ] );

			// Set cooldown
			if ( cooldownSlider [ i ] != null )
				SetCooldown ( s, s.cooldown, cooldownSlider [ i ], cooldownDisplay [ i ] );
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
	/// <param name="value">Value.</param>
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
	/// Toggles the current selection setting for a special ability.
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
		else if ( stackingValue && SelectionCheck ( ) == 1 && specialValue [ index ].selection && sizeValue != 0 )
		{
			// Prompt that at least one specials needs to be enabled
			popUp.OpenMenu ( false, true, "At least one Special Ability needs to be enabled for selection.", null, null );
		}
		else
		{
			// Toggle selection
			SetSelection ( specialValue [ index ], !specialValue [ index ].selection, selectionDisplay [ index ] );
		}
	}

	/// <summary>
	/// Sets the current selection setting for a special ability.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetSelection ( SpecialSettings special, bool value, TextMeshProUGUI display )
	{
		// Store selection setting
		special.selection = value;

		// Display selection
		if ( special.selection )
			display.text = "Enabled";
		else
			display.text = "Disabled";
	}

	/// <summary>
	/// Sets the current cooldown setting for the Catapult unit.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetCatapultCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( specialValue [ 1 ], (int)value, cooldownSlider [ 1 ], cooldownDisplay [ 1 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for the Teleport unit.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetTeleportCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( specialValue [ 3 ], (int)value, cooldownSlider [ 3 ], cooldownDisplay [ 3 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for the Torus unit.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetTorusCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( specialValue [ 4 ], (int)value, cooldownSlider [ 4 ], cooldownDisplay [ 4 ] );
	}

	/// <summary>
	/// Sets the current cooldown setting for a special ability.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetCooldown ( SpecialSettings special, int value, Slider slider, TextMeshProUGUI display )
	{
		// Store cooldown
		special.cooldown = value;

		// Set slider
		slider.value = (float)value;

		// Display cooldown
		if ( special.cooldown == 1 )
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
		MatchSettings.SetMatchSettings ( typeValue, turnTimerValue, timeValue, sizeValue, stackingValue, specialValue );

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
		// Count the number of enabled specials
		int count = 0;
		foreach ( SpecialSettings s in specialValue )
			if ( s.selection )
				count++;

		// Return the number of enabled specials
		return count;
	}
}
