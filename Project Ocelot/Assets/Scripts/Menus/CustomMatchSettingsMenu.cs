using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomMatchSettingsMenu : Menu 
{
	#region Private Classes

	[System.Serializable]
	private class HeroCustomizationUI
	{
		public TextMeshProUGUI EnableDisplay;
		public AbilityCustomizationUI [ ] Abilities;

		/// <summary>
		/// The UI elements for the hero's first ability.
		/// </summary>
		public AbilityCustomizationUI Ability1
		{
			get
			{
				// Return the UI elements for ability 1 if they are present
				return Abilities.Length < 1 || Abilities [ 0 ] == null ? null : Abilities [ 0 ];
			}
		}

		/// <summary>
		/// The UI elements for the hero's second ability.
		/// </summary>
		public AbilityCustomizationUI Ability2
		{
			get
			{
				// Return the UI elements for ability 2 if they are present
				return Abilities.Length < 2 || Abilities [ 1 ] == null ? null : Abilities [ 1 ];
			}
		}

		/// <summary>
		/// The UI elements for the hero's third ability.
		/// </summary>
		public AbilityCustomizationUI Ability3
		{
			get
			{
				// Return the UI elements for ability 3 if they are present
				return Abilities.Length < 3 || Abilities [ 2 ] == null ? null : Abilities [ 2 ];
			}
		}
	}

	[System.Serializable]
	private class AbilityCustomizationUI
	{
		public TextMeshProUGUI EnableDisplay;
		public Slider DurationSlider;
		public TextMeshProUGUI DuractionDisplay;
		public Slider CooldownSlider;
		public TextMeshProUGUI CooldownDisplay;
	}

	#endregion // Private Classes

	#region UI Elements

	[SerializeField]
	private Scrollbar scroll;

	[SerializeField]
	private TextMeshProUGUI typeDisplay;

	[SerializeField]
	private TextMeshProUGUI turnTimerDisplay;

	[SerializeField]
	private Slider timeSlider;

	[SerializeField]
	private TextMeshProUGUI timeDisplay;

	[SerializeField]
	private Slider sizeSlider;

	[SerializeField]
	private TextMeshProUGUI sizeDisplay;

	[SerializeField]
	private TextMeshProUGUI stackingDisplay;

	[SerializeField]
	private HeroCustomizationUI [ ] heroes;

	#endregion // UI Elements

	#region Custom Match Settings Data

	private MatchType typeValue;
	private bool turnTimerValue;
	private float timeValue;
	private int sizeValue;
	private bool stackingValue;
	private List<UnitDefaultData> heroValues = new List<UnitDefaultData> ( );

	#endregion // Custom Match Settings Data

	// UI elements

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
	
	private List<HeroSettings> heroValue = new List<HeroSettings> ( );
	public PopUpMenu popUp;
	public LoadingScreen load;

	#region Menu Override Functions

	/// <summary>
	/// Opens the menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true )
	{
		// Open the menu
		base.OpenMenu ( closeParent );

		// Set the scroll bar to the top of the menu
		scroll.value = 1;

		// Set default match type
		SetMatchType ( MatchType.CustomClassic );

		// Set default turn timer
		SetTurnTimer ( true );
		SetTimePerTurn ( 4f );

		// Set default team size
		SetHeroesPerTeam ( 3 );

		// Set default stacking
		SetHeroStacking ( false );

		// Set default special ability settings
		heroValue.Clear ( );
		for ( int i = 0; i < HeroInfo.list.Count; i++ )
		{
			// Set hero setting
			HeroSettings h = new HeroSettings ( HeroInfo.list [ i ].ID, true, true, (Ability.AbilityType)HeroInfo.list [ i ].Ability1.Type, HeroInfo.list [ i ].Ability1.Duration, HeroInfo.list [ i ].Ability1.Cooldown, true, (Ability.AbilityType)HeroInfo.list [ i ].Ability2.Type, HeroInfo.list [ i ].Ability2.Duration, HeroInfo.list [ i ].Ability2.Cooldown );
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

	#endregion // Menu Override Functions

	#region Public Functions

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
	/// Toggles the current turn timer setting.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetTurnTimer ( )
	{
		// Toggle turn timer
		SetTurnTimer ( !turnTimerValue );
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
		int min = ( int ) timeValue / 60;
		int sec = ( int ) timeValue % 60;
		timeDisplay.text = string.Format ( "{0:0}:{1:00}", min, sec );
	}

	/// <summary>
	/// Sets the current team size setting.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetHeroesPerTeam ( float value )
	{
		// Check if enough abilities are enabled
		if ( !stackingValue && value > EnabledHeroesCheck ( ) )
		{
			// Prevent the slider from updating
			sizeSlider.value = ( float ) sizeValue;

			// Prompt the user that there aren't enough abilities enabled
			popUp.SetAcknowledgementPopUp ( "More Heroes need to be enabled for selection than the current number of Heroes Per Team if Hero Stacking is disabled", null );
			popUp.OpenMenu ( false );
		}
		else
		{
			SetHeroesPerTeam ( ( int ) value );
		}
	}

	/// <summary>
	/// Toggles the current hero stacking setting.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetHeroStacking ( )
	{
		// Check if stacking can be disabled
		if ( stackingValue && EnabledHeroesCheck ( ) < sizeValue )
		{
			// Prompt that stacking cannot be disabled
			popUp.SetAcknowledgementPopUp ( "Hero Stacking cannot be disabled if there are less than the number of Hero Per Team currently enabled for selection.", null );
			popUp.OpenMenu ( false );
		}
		else
		{
			// Toggle stacking
			SetHeroStacking ( !stackingValue );
		}
	}

	/// <summary>
	/// Toggles the current selection setting for a hero.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetEnableHero ( int index )
	{
		// Check stacking and the amount of specials enabled
		if ( !stackingValue && EnabledHeroesCheck ( ) == sizeValue )
		{
			// Prompt that at least three specials need to be enabled if stacking is not enabled
			popUp.SetAcknowledgementPopUp ( "At least the number of Heroes Per Team needs to be enabled for selection while Hero Stacking is disabled.", null );
			popUp.OpenMenu ( false );
		}
		else if ( stackingValue && EnabledHeroesCheck ( ) == 1 && heroValues [ index ].IsEnabled && sizeValue != 0 )
		{
			// Prompt that at least one specials needs to be enabled
			popUp.SetAcknowledgementPopUp ( "At least one Hero needs to be enabled for selection.", null );
			popUp.OpenMenu ( false );
		}
		else
		{
			// Toggle selection
			SetEnableHero ( heroValues [ index ], !heroValues [ index ].IsEnabled, heroes [ index ].EnableDisplay );
		}
	}

	/// <summary>
	/// Toggles the current ability 1 enable setting for a hero.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetEnableAbility1 ( int index )
	{
		// Set ability 1 enable setting
		SetEnableAbility ( heroValues [ index ].Ability1, !heroValues [ index ].Ability1.IsEnabled, heroes [ index ].Ability1.EnableDisplay );
	}

	/// <summary>
	/// Toggles the current ability 2 enable setting for a hero.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetEnableAbility2 ( int index )
	{
		// Set ability 2 enable setting
		SetEnableAbility ( heroValues [ index ].Ability2, !heroValues [ index ].Ability2.IsEnabled, heroes [ index ].Ability2.EnableDisplay );
	}

	/// <summary>
	/// Toggles the current ability 3 enable setting for a hero.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void SetEnableAbility3 ( int index )
	{
		// Set ability 1 enable setting
		SetEnableAbility ( heroValues [ index ].Ability3, !heroValues [ index ].Ability3.IsEnabled, heroes [ index ].Ability3.EnableDisplay );
	}

	#region Ability Duration Functions

	/// <summary>
	/// Sets the current duration setting for Hero 1's Armor ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetArmorDuration ( float value )
	{
		// Set duration
		SetDuration ( heroValues [ 0 ].Ability1, ( int ) value, heroes [ 0 ].Ability1.DurationSlider, heroes [ 0 ].Ability1.DuractionDisplay, " Attack", " Attacks" );
	}

	/// <summary>
	/// Sets the current duration setting for Hero 3's Mind Control ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetMindControlDuration ( float value )
	{
		// Set duration
		SetDuration ( heroValues [ 2 ].Ability1, ( int ) value, heroes [ 2 ].Ability1.DurationSlider, heroes [ 2 ].Ability1.DuractionDisplay );
	}

	/// <summary>
	/// Sets the current duration setting for Hero 4's Life Drain ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetLifeDrainDuration ( float value )
	{
		// Set duration
		SetDuration ( heroValues [ 3 ].Ability1, ( int ) value, heroes [ 3 ].Ability1.DurationSlider, heroes [ 3 ].Ability1.DuractionDisplay );
	}

	/// <summary>
	/// Sets the current duration setting for Hero 5's Rally ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetRallyDuration ( float value )
	{
		// Set duration
		SetDuration ( heroValues [ 4 ].Ability1, ( int ) value, heroes [ 4 ].Ability1.DurationSlider, heroes [ 4 ].Ability1.DuractionDisplay, "Use", "Uses" );
	}

	/// <summary>
	/// Sets the current duration setting for Hero 7's Split Up ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetSplitUpDuration ( float value )
	{
		// Set duration
		SetDuration ( heroValues [ 6 ].Ability2, ( int ) value, heroes [ 6 ].Ability2.DurationSlider, heroes [ 6 ].Ability2.DuractionDisplay );
	}

	#endregion // Ability Duration Functions

	#region Ability Cooldown Functions

	/// <summary>
	/// Sets the current cooldown setting for Hero 1's Self-Destruct ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetSelfDestructCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 0 ].Ability2, ( int ) value, heroes [ 0 ].Ability2.CooldownSlider, heroes [ 0 ].Ability2.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 1's Reconstruct ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetReconstructCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 0 ].Ability3, ( int ) value, heroes [ 0 ].Ability3.CooldownSlider, heroes [ 0 ].Ability3.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 2's Catapult ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetCatapultCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 1 ].Ability1, ( int ) value, heroes [ 1 ].Ability1.CooldownSlider, heroes [ 1 ].Ability1.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 2's Grapple ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetGrappleCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 1 ].Ability2, ( int ) value, heroes [ 1 ].Ability2.CooldownSlider, heroes [ 1 ].Ability2.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 3's Clone Assist ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetCloneAssistCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 2 ].Ability2, ( int ) value, heroes [ 2 ].Ability2.CooldownSlider, heroes [ 2 ].Ability2.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 4's Grim Reaper ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetGrimReaperCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 3 ].Ability2, ( int ) value, heroes [ 3 ].Ability2.CooldownSlider, heroes [ 3 ].Ability2.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 5's Backflip ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetBackflipCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 4 ].Ability2, ( int ) value, heroes [ 4 ].Ability2.CooldownSlider, heroes [ 4 ].Ability2.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 6's Obstruction ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetObstuctionCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 5 ].Ability2, ( int ) value, heroes [ 5 ].Ability2.CooldownSlider, heroes [ 5 ].Ability2.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 7's Split Up ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetSplitUpCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 6 ].Ability2, ( int ) value, heroes [ 6 ].Ability2.CooldownSlider, heroes [ 6 ].Ability2.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 7's Regroup ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetRegroupCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 6 ].Ability3, ( int ) value, heroes [ 6 ].Ability3.CooldownSlider, heroes [ 6 ].Ability3.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 8's Blink ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetBlinkCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 7 ].Ability1, ( int ) value, heroes [ 7 ].Ability1.CooldownSlider, heroes [ 7 ].Ability1.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 8's Translocator ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetTranslocatorCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 7 ].Ability2, ( int ) value, heroes [ 7 ].Ability2.CooldownSlider, heroes [ 7 ].Ability2.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 9's Run The Ropes ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetRunTheRopesCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 8 ].Ability1, ( int ) value, heroes [ 8 ].Ability1.CooldownSlider, heroes [ 8 ].Ability1.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 9's Taunt ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetTauntCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 8 ].Ability2, ( int ) value, heroes [ 8 ].Ability2.CooldownSlider, heroes [ 8 ].Ability2.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 10's Divebomb ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetDivebombCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 9 ].Ability1, ( int ) value, heroes [ 9 ].Ability1.CooldownSlider, heroes [ 9 ].Ability1.CooldownDisplay );
	}

	/// <summary>
	/// Sets the current cooldown setting for Hero 10's Dropkick ability.
	/// Use this as a slider update event wrapper.
	/// </summary>
	public void SetDropkickCooldown ( float value )
	{
		// Set cooldown
		SetCooldown ( heroValues [ 9 ].Ability2, ( int ) value, heroes [ 9 ].Ability2.CooldownSlider, heroes [ 9 ].Ability2.CooldownDisplay );
	}

	#endregion // Ability Cooldown Functions

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
		load.LoadScene ( Scenes.MatchSetup );
	}

	#endregion // Public Functions

	#region Private Functions

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
	/// Sets the current team size setting.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetHeroesPerTeam ( int value )
	{
		// Store team size
		sizeValue = value;

		// Set slider
		sizeSlider.value = ( float ) value;

		// Display team size
		sizeDisplay.text = value.ToString ( );
	}

	/// <summary>
	/// Sets the current hero stacking setting.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetHeroStacking ( bool value )
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
	/// Sets the current selection setting for a hero.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetEnableHero ( UnitDefaultData hero, bool value, TextMeshProUGUI display )
	{
		// Store selection setting
		hero.IsEnabled = value;

		// Display selection
		if ( hero.IsEnabled )
			display.text = "Enabled";
		else
			display.text = "Disabled";
	}

	/// <summary>
	/// Sets the current ability enable setting for a hero.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetEnableAbility ( AbilityData ability, bool value, TextMeshProUGUI display )
	{
		// Store ability enable setting
		ability.IsEnabled = value;

		// Display ability enable setting
		if ( ability.IsEnabled )
			display.text = "Enabled";
		else
			display.text = "Disabled";
	}

	/// <summary>
	/// Sets the current duration setting for an ability.
	/// Does not apply to the actual match settings until the match starts.
	/// </summary>
	private void SetDuration ( AbilityData ability, int value, Slider slider, TextMeshProUGUI display, string prompt = " Turn", string prompts = " Turns" )
	{
		// Store duration
		ability.Duration = value;

		// Set slider
		slider.value = ( float ) value;

		// Display duration
		if ( ability.Duration != 1 )
			display.text = value + prompts;
		else
			display.text = value + prompt;
	}

	/// <summary>
	/// Sets the current cooldown setting for a special ability.
	/// Does not apply to the actual match setting until the match starts.
	/// </summary>
	private void SetCooldown ( AbilityData ability, int value, Slider slider, TextMeshProUGUI display )
	{
		// Store cooldown
		ability.Cooldown = value;

		// Set slider
		slider.value = ( float ) value;

		// Display cooldown
		if ( ability.Cooldown == 1 )
			display.text = "1 Turn";
		else
			display.text = value + " Turns";
	}

	/// <summary>
	/// Returns the current number of heroes currently enabled.
	/// </summary>
	private int EnabledHeroesCheck ( )
	{
		// Count the number of enabled heroes
		int count = 0;
		foreach ( UnitDefaultData h in heroValues )
			if ( h.IsEnabled )
				count++;

		// Return the number of enabled heroes
		return count;
	}

	#endregion // Private Functions
}
