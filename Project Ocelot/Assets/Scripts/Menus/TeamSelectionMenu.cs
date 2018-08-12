using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TeamSelectionMenu : Menu 
{
	#region Private Classes

	[System.Serializable]
	private class SelectionCards
	{
		[HideInInspector]
		public Player.TeamColor Team;

		public UnitCard Card;
		public RectTransform PromptContainer;
		public Image PromptBorder;
		public TextMeshProUGUI PromptText;
		public TextMeshProUGUI PromptNumber;
		public Button RandomButton;
		public Button UnselectButton;

		public enum CardState
		{
			DISABLED,
			UNAVAILABLE,
			SELECTED,
			LAST_SELECTED,
			ON_DECK,
			UNSELECTED
		}

		private CardState state;

		/// <summary>
		/// Sets how the card should be displayed during selection.
		/// </summary>
		public CardState State
		{
			get
			{
				// Return value
				return state;
			}
			set
			{
				// Store value
				state = value;

				// Display or hide card
				Card.gameObject.SetActive ( state != CardState.DISABLED );

				// Display or hide unit card
				Card.IsEnabled = state == CardState.SELECTED || state == CardState.LAST_SELECTED || state == CardState.ON_DECK;

				// Display or hide card prompt
				PromptContainer.gameObject.SetActive ( state == CardState.UNAVAILABLE || state == CardState.UNSELECTED );

				// Display or hide random button
				RandomButton.gameObject.SetActive ( state == CardState.ON_DECK );

				// Display or hide unselect button
				UnselectButton.gameObject.SetActive ( state == CardState.LAST_SELECTED );

				// Set card prompt size
				PromptContainer.offsetMax = state == CardState.UNAVAILABLE ? new Vector2 ( -10f, -10f ) : Vector2.zero;
				PromptContainer.offsetMin = state == CardState.UNAVAILABLE ? new Vector2 ( 10f, 10f ) : Vector2.zero;

				// Set card prompt colors
				PromptBorder.color = state == CardState.UNAVAILABLE ? (Color32)Color.grey : Util.TeamColor ( Team );
				PromptText.color = state == CardState.UNAVAILABLE ? Color.grey : Color.white;
				PromptNumber.color = state == CardState.UNAVAILABLE ? Color.grey : Color.white;
			}
		}
	}

	[System.Serializable]
	private class SelectionPortraits
	{
		public UnitPortrait Portrait;
		public int UnitID;
	}

	#endregion // Private Classes

	#region UI Elements

	[SerializeField]
	private SelectionCards [ ] cards;

	[SerializeField]
	private SelectionPortraits [ ] portraits;

	[SerializeField]
	private GameObject selectPanel;

	[SerializeField]
	private GameObject confirmPanel;

	#endregion // UI Elements

	#region Menu Data

	[SerializeField]
	private TeamSetup setupManager;

	[SerializeField]
	private Menu teamFormationMenu;

	private UnitSettingData selectedHero;
	private int confirmedHeroesCounter;
	private int slotDeficitCounter;

	#endregion // Menu Data

	#region Menu Override Functions

	/// <summary>
	/// Opens the team selection menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true )
	{
		// Open the menu
		base.OpenMenu ( closeParent );

		// Set prompts
		selectPanel.SetActive ( true );
		confirmPanel.SetActive ( false );

		// Set team color for cards
		for ( int i = 0; i < cards.Length; i++ )
			cards [ i ].Team = setupManager.CurrentPlayer.Team;

		// Set unit selection portraits
		for ( int i = 0; i < portraits.Length; i++ )
		{
			// Set unit to portrait
			portraits [ i ].Portrait.SetPortrait ( MatchSettings.GetHero ( portraits [ i ].UnitID ), setupManager.CurrentPlayer.Team );

			// Set whether or not the hero is available from the settings
			portraits [ i ].Portrait.IsEnabled = true;
			portraits [ i ].Portrait.IsAvailable = MatchSettings.GetUnitData ( portraits [ i ].UnitID ).IsEnabled;
		}

		// Set cards for the first hero selection
		confirmedHeroesCounter = 0;
		slotDeficitCounter = 0;
		SetCardState ( confirmedHeroesCounter );

		// Start by selecting a random hero
		SelectRandomUnit ( false );

		// Display prompt
		setupManager.Splash.Slide ( "<size=75%>" + setupManager.CurrentPlayer.PlayerName + "</size>\n<color=white>Team Selection", Util.TeamColor ( setupManager.CurrentPlayer.Team ), true );
	}

	#endregion // Menu Override Functions

	#region Event Trigger Functions

	public void OnPointerEnter ( int index )
	{
		// Check if the portrait is interactable
		if ( portraits [ index ].Portrait.IsAvailable )
		{
			// Enlarge the portrait to indicate that portrait is being interacted with
			portraits [ index ].Portrait.ChangeSize ( 5 );
		}
	}

	public void OnPointerExit ( int index )
	{
		// Check if the portrait is interable and not selected
		if ( portraits [ index ].Portrait.IsAvailable && portraits [ index ].UnitID != selectedHero.ID )
		{
			// Reset the portrait to its default size to indicate that the portrait is no longer being interacted with
			portraits [ index ].Portrait.ResetSize ( );
		}
	}

	#endregion // Event Trigger Functions

	#region Public Functions

	/// <summary>
	/// Selects the unit for display and potential addition to the team.
	/// </summary>
	public void SelectUnit ( int index )
	{
		// Check if the hero is available via its portrait
		if ( portraits [ index ].Portrait.IsAvailable )
		{
			// Unselect the previously selected hero
			if ( selectedHero != null )
			{
				SelectionPortraits p = portraits.First ( x => x.UnitID == selectedHero.ID );
				p.Portrait.ResetSize ( );
				p.Portrait.IsBorderHighlighted = false;
			}
				

			// Store the currently selected hero
			selectedHero = MatchSettings.GetHero ( portraits [ index ].UnitID );

			// Enlarge the portrait to indicate that the hero is selected
			portraits [ index ].Portrait.ChangeSize ( 5 );

			// Highlight border
			portraits [ index ].Portrait.IsBorderHighlighted = true;

			// Display hero in card
			cards [ confirmedHeroesCounter ].Card.SetCard ( selectedHero, setupManager.CurrentPlayer.Team );

			// Preview the amount of slots the unit would add
			setupManager.SlotMeter.PreviewSlots ( selectedHero.Slots );
		}
	}

	/// <summary>
	/// Selects an available unit at random.
	/// </summary>
	/// <param name="confirmSelection"> Whether or not the randomly selected hero should be confirmed. </param>
	public void SelectRandomUnit ( bool confirmSelection )
	{
		// Get the subset of remaining available heroes based on their portraits
		SelectionPortraits [ ] availableHeroes = portraits.Where ( x => x.Portrait.IsEnabled && x.Portrait.IsAvailable ).ToArray ( );

		// Get a random hero from that subset
		SelectionPortraits randomHero = availableHeroes [ Random.Range ( 0, availableHeroes.Length ) ];

		// Select the random hero with the index of the portrait
		SelectUnit ( System.Array.IndexOf ( portraits, randomHero ) );

		// Check if the random hero should be confirmed as well
		if ( confirmSelection )
			ConfirmUnit ( );
	}

	/// <summary>
	/// Confirms the unit to be added to the team.
	/// </summary>
	public void ConfirmUnit ( )
	{
		// Add hero to the team
		setupManager.CurrentPlayer.Units.Add ( selectedHero );

		// Update slot meter
		setupManager.SlotMeter.SetMeter ( setupManager.SlotMeter.FilledSlots + selectedHero.Slots );

		// Increment the selected hero counter
		confirmedHeroesCounter++;

		// Increment the slot deficit if the hero occupies multiple slots
		if ( selectedHero.Slots > 1 )
			slotDeficitCounter += selectedHero.Slots - 1;

		// Set whether or not the confirmed hero is still available for selection based on the match's stacking setting
		portraits.First ( x => x.UnitID == selectedHero.ID ).Portrait.IsAvailable = !MatchSettings.HeroLimit;

		// Set the cards for the next selection
		SetCardState ( confirmedHeroesCounter );

		// Set the portraits of any heros over the limit as unavailable
		for ( int i = 0; i < portraits.Length; i++ )
			if ( MatchSettings.GetUnitData ( portraits [ i ].UnitID ).Slots + setupManager.SlotMeter.FilledSlots > setupManager.SlotMeter.TotalSlots )
				portraits [ i ].Portrait.IsAvailable = false;

		// Check if more heroes can be selected
		if ( confirmedHeroesCounter < MatchSettings.HeroesPerTeam && confirmedHeroesCounter < cards.Length - slotDeficitCounter )
		{
			// Select a hero at random to start the next selection
			SelectRandomUnit ( false );
		}
		else
		{
			// Hide unselect card from the last card
			SetCardState ( confirmedHeroesCounter - 1 );
			cards [ confirmedHeroesCounter - 1 ].State = SelectionCards.CardState.SELECTED;
			cards [ confirmedHeroesCounter - 2 ].State = SelectionCards.CardState.SELECTED;

			// Display confirmation button
			selectPanel.SetActive ( false );
			confirmPanel.SetActive ( true );

		}
	}

	/// <summary>
	/// Removes the last unit added to the team.
	/// </summary>
	public void UnselectUnit ( )
	{
		// Display selection panel
		selectPanel.SetActive ( true );
		confirmPanel.SetActive ( false );

		// Decrement hero counter
		confirmedHeroesCounter--;

		// Get last confirmed hero
		UnitSettingData previousHero = setupManager.CurrentPlayer.Units [ confirmedHeroesCounter + 1 ];

		// Remove hero from the player's roster
		setupManager.CurrentPlayer.Units.Remove ( previousHero );

		// Decrement slot deficit if the hero occupies multiple slots
		if ( previousHero.Slots > 1 )
			slotDeficitCounter -= previousHero.Slots - 1;

		// Remove the hero from the slot meter
		setupManager.SlotMeter.SetMeter ( setupManager.SlotMeter.FilledSlots - previousHero.Slots );

		// Set cards to previous selection
		SetCardState ( confirmedHeroesCounter );

		// Enable any portraits that were disabled for being over the limit from the previous hero
		for ( int i = 0; i < portraits.Length; i++ )
		{
			if ( MatchSettings.GetUnitData ( portraits [ i ].UnitID ).Slots + setupManager.SlotMeter.FilledSlots <= setupManager.SlotMeter.TotalSlots && ( !MatchSettings.HeroLimit || ( MatchSettings.HeroLimit && !setupManager.CurrentPlayer.Units.Exists ( x => x.ID == portraits [ i ].UnitID ) ) ) )
			{
				portraits [ i ].Portrait.IsAvailable = true;
				portraits [ i ].Portrait.IsBorderHighlighted = false;
			}
		}
			

		// Select previous hero
		SelectUnit ( System.Array.IndexOf ( portraits, portraits.First ( x => x.UnitID == previousHero.ID ) ) );
	}

	/// <summary>
	/// Confirms the team selection.
	/// </summary>
	public void ConfirmTeam ( )
	{
		// Add remaining pawns to team
		for ( int i = 0; i < setupManager.SlotMeter.TotalSlots - setupManager.SlotMeter.FilledSlots; i++ )
		{
			// Get new unit data
			UnitSettingData newPawn = MatchSettings.GetPawn ( );

			// Add unit to team
			setupManager.CurrentPlayer.Units.Add ( newPawn );
		}

		// Open the team formation menu
		teamFormationMenu.OpenMenu ( );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Sets the states of each card based on how many heroes have been selected thus far.
	/// </summary>
	/// <param name="index"> The index for the hero currently being selected. </param>
	private void SetCardState ( int index )
	{
		// Set the state of each card
		for ( int i = 0; i < cards.Length; i++ )
		{
			// Check if the card is for more than the number of heroes for the match 
			if ( i >= MatchSettings.HeroesPerTeam )
			{
				// Set the card to disabled
				cards [ i ].State = SelectionCards.CardState.DISABLED;
			}
			// Check if the card is unavailable from there not being enough slots remaining
			else if ( i >= cards.Length - slotDeficitCounter )
			{
				// Set the card to unavailable
				cards [ i ].State = SelectionCards.CardState.UNAVAILABLE;
			}
			// Check if the card for the hero that is currently being selected
			else if ( i == index )
			{
				// Set the card to on deck
				cards [ i ].State = SelectionCards.CardState.ON_DECK;
			}
			// Check if the card is for the hero that was last selected
			else if ( i == index - 1 && index - 1 >= 0 )
			{
				// Set the card to last selected
				cards [ i ].State = SelectionCards.CardState.LAST_SELECTED;
			}
			// Check if the card is for a hero that has been selected
			else if ( i < index && index < cards.Length )
			{
				// Set the card to selected
				cards [ i ].State = SelectionCards.CardState.SELECTED;
			}
			// Set all leftover cards to display the prompt
			else
			{
				// Set the card to unselected
				cards [ i ].State = SelectionCards.CardState.UNSELECTED;
			}
		}
	}

	#endregion // Private Functions
}
