using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TeamSelectionMenu : Menu 
{
	#region UI Elements

	public TextMeshProUGUI teamName;
	public TeamSlotMeter slotMeter;
	public HeroCard [ ] cards;
	public GameObject selectPanel;
	public GameObject confirmPanel;
	public UnitPortrait [ ] heroPortraits;

	#endregion // UI Elements

	#region Menu Data

	public TeamSetup setup;
	public Menu teamFormation;
	private int selectedHeroID;
	private UnitPortrait selectedPortrait;
	private int heroIndex;
	private int heroTotal;
	private int disabledCardIndex;
	private List<Tween> slotAnimations = new List<Tween> ( );
	private Color32 slotColor = new Color32 ( 255, 210, 75, 255 );

	#endregion // Menu Data

	#region Player Data

	private PlayerSettings player;

	#endregion // Player Data

	// HACK
	public Sprite [ ] icons;

	#region Menu Override Functions

	/// <summary>
	/// Opens the team selection menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true, params object [ ] values )
	{
		// Open the menu
		base.OpenMenu ( closeParent );
		selectPanel.SetActive ( true );
		confirmPanel.SetActive ( false );

		// Set the player
		player = values [ 0 ] as PlayerSettings;

		// Display team name
		teamName.text = player.name;
		teamName.color = Util.TeamColor ( player.TeamColor );

		// Reset resource meter to only display on slot for the Leader unit
		slotMeter.SetMeter ( 1 );

		// Set the total number of heroes to be selected
		heroTotal = MatchSettings.teamSize;

		// Start hero selection from the beginning
		heroIndex = 0;
		disabledCardIndex = slotMeter.TotalSlots - heroTotal - 1;

		// Display cards for the number of heroes to be selected
		for ( int i = 0; i < cards.Length; i++ )
		{
			// Hide excess cards
			cards [ i ].gameObject.SetActive ( i < heroTotal );

			// Set displayed cards
			if ( i < heroTotal )
			{
				// Set card color
				cards [ i ].SetTeamColor ( Util.TeamColor ( player.TeamColor ) );

				// Set cards to unselected
				cards [ i ].DisplayCardWithoutHero ( );
				if ( i == 0 )
					cards [ i ].SetControls ( HeroCard.CardControls.RANDOM );
				else
					cards [ i ].SetControls ( HeroCard.CardControls.NONE );
			}
		}

		// Set hero select buttons
		for ( int i = 0; i < heroPortraits.Length; i++ )
			heroPortraits [ i ].SetUnit ( i + 1, icons [ i ], Util.TeamColor ( player.TeamColor ) );

		// Start by selecting a random hero
		SelectRandomUnit ( false );

		// Display prompt
		setup.splash.Slide ( "<size=75%>" + player.name + "</size>\n<color=white>Team Selection", Util.TeamColor ( player.TeamColor ), true );
	}

	#endregion // Menu Override Functions

	#region Public Functions

	/// <summary>
	/// Selects the unit for display and potential addition to the team.
	/// </summary>
	public void SelectUnit ( int index )
	{
		// Check if hero is enabled
		if ( heroPortraits [ index ].IsEnabled )
		{
			// Check for previously selected buttons
			if ( selectedPortrait != null && selectedPortrait != heroPortraits [ index ] )
			{
				// Reset the button
				selectedPortrait.SelectToggle ( false );
			}

			// Store current hero ID
			selectedHeroID = index + 1;

			// Store selected button
			selectedPortrait = heroPortraits [ index ];

			// Display hero in card
			cards [ heroIndex ].SetHero ( selectedHeroID, icons [ index ] );

			// Update slot meter
			slotMeter.PreviewSlots ( HeroInfo.GetHeroByID ( selectedHeroID ).Slots );
		}
	}

	/// <summary>
	/// Selects an active unit at random.
	/// </summary>
	public void SelectRandomUnit ( bool confirmSelection )
	{
		// Get random hero from the enabled buttons
		UnitPortrait [ ] enabledPortraits = heroPortraits.Where ( x => x.IsEnabled ).ToArray ( );
		UnitPortrait randomPortrait = enabledPortraits [ Random.Range ( 0, enabledPortraits.Length ) ];

		// Select button
		randomPortrait.MouseClick ( );
		SelectUnit ( System.Array.IndexOf ( heroPortraits, randomPortrait ) );
		if ( confirmSelection )
			ConfirmUnit ( );
	}

	/// <summary>
	/// Confirms the unit to be added to the team.
	/// </summary>
	public void ConfirmUnit ( )
	{
		// Add hero to the team
		player.heroIDs.Add ( selectedHeroID );

		// Update slot meter
		slotMeter.SetMeter ( slotMeter.FilledSlots + slotMeter.PreviewedSlots );

		// Increment hero index
		heroIndex++;

		// Check if hero stacking is enabled
		if ( !MatchSettings.stacking )
		{
			// Disable hero button
			selectedPortrait.SelectToggle ( false );
			selectedPortrait.EnableToggle ( false );
			selectedPortrait = null;
		}

		// Check the hero takes up multiple slots
		if ( HeroInfo.GetHeroByID ( selectedHeroID ).Slots > 1 )
		{
			// Subtract the number of available slots
			disabledCardIndex -= ( HeroInfo.GetHeroByID ( selectedHeroID ).Slots - 1 );

			// Check if a hero slot was removed
			if ( disabledCardIndex < 0 )
			{
				// Disable the hero card to indicate the loss of a hero slot
				cards [ heroTotal + disabledCardIndex ].DisableCard ( );
			}
		}

		// Update controls
		if ( heroIndex - 2 >= 0 )
			cards [ heroIndex - 2 ].SetControls ( HeroCard.CardControls.NONE );
		if ( heroIndex < heroTotal && slotMeter.FilledSlots < slotMeter.TotalSlots )
			cards [ heroIndex - 1 ].SetControls ( HeroCard.CardControls.UNDO );
		else
			cards [ heroIndex - 1 ].SetControls ( HeroCard.CardControls.NONE );
		if ( heroIndex < heroTotal && slotMeter.FilledSlots < slotMeter.TotalSlots )
			cards [ heroIndex ].SetControls ( HeroCard.CardControls.RANDOM );

		// Check if hero selection is complete
		if ( heroIndex < heroTotal && slotMeter.FilledSlots < slotMeter.TotalSlots )
		{
			// Disable heroes that won't fit within the team with the remaining resources
			for ( int i = 0; i < heroPortraits.Length; i++ )
				heroPortraits [ i ].EnableToggle ( heroPortraits [ i ].IsEnabled && slotMeter.FilledSlots + HeroInfo.list [ i ].Slots <= slotMeter.TotalSlots );

			// Select next hero
			SelectRandomUnit ( false );
		}
		else
		{
			// Display confirmation panel
			confirmPanel.SetActive ( true );
			selectPanel.SetActive ( false );
		}
	}

	/// <summary>
	/// Removes the last unit added to the team.
	/// </summary>
	public void UnselectUnit ( )
	{
		// Check if confirmation is being cancelled
		if ( heroIndex == heroTotal || slotMeter.FilledSlots == slotMeter.TotalSlots )
		{
			// Display selection panel
			selectPanel.SetActive ( true );
			confirmPanel.SetActive ( false );
		}

		// Decrement hero index
		heroIndex--;

		// Store selected hero
		selectedHeroID = player.heroIDs [ heroIndex ];

		// Remove hero from player list
		player.heroIDs.Remove ( selectedHeroID );

		// Remove last hero from slot meter
		slotMeter.SetMeter ( slotMeter.FilledSlots - HeroInfo.GetHeroByID ( selectedHeroID ).Slots );

		// Check if the hero takes up multiples slots
		if ( HeroInfo.GetHeroByID ( selectedHeroID ).Slots > 1 )
		{
			// Add the number of available slots
			disabledCardIndex += ( HeroInfo.GetHeroByID ( selectedHeroID ).Slots - 1 );

			// Check if a hero slot was added
			if ( disabledCardIndex <= 0 )
			{
				// Enable the hero card to indicate the addition of a hero slot
				cards [ heroTotal + ( disabledCardIndex - ( HeroInfo.GetHeroByID ( selectedHeroID ).Slots - 1 ) ) ].DisplayCardWithoutHero ( );
			}
		}

		// Update cards and controls
		if ( heroIndex - 1 >= 0 )
			cards [ heroIndex - 1 ].SetControls ( HeroCard.CardControls.UNDO );
		cards [ heroIndex ].SetControls ( HeroCard.CardControls.RANDOM );
		if ( heroIndex + 1 < heroTotal && slotMeter.FilledSlots + HeroInfo.GetHeroByID ( selectedHeroID ).Slots < slotMeter.TotalSlots )
		{
			cards [ heroIndex + 1 ].DisplayCardWithoutHero ( );
			cards [ heroIndex + 1 ].SetControls ( HeroCard.CardControls.NONE );
		}
			

		// Enable any hero buttons that were removed from the last hero's selection
		for ( int i = 0; i < heroPortraits.Length; i++ )
			heroPortraits [ i ].EnableToggle ( MatchSettings.heroSettings [ i ].selection && ( MatchSettings.stacking || !player.heroIDs.Contains ( HeroInfo.list [ i ].ID ) ) && slotMeter.FilledSlots + HeroInfo.list [ i ].Slots <= slotMeter.TotalSlots );

		// Enable previous hero button
		heroPortraits [ selectedHeroID - 1 ].EnableToggle ( true );

		// Select previous hero
		heroPortraits [ selectedHeroID - 1 ].MouseClick ( );
		SelectUnit ( selectedHeroID - 1 );
	}

	/// <summary>
	/// Confirms the team selection.
	/// </summary>
	public void ConfirmTeam ( )
	{
		// Open the team formation menu
		teamFormation.OpenMenu ( true, player );
	}

	#endregion // Public Functions
}
