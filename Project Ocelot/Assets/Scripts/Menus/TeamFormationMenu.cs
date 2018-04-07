using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TeamFormationMenu : Menu 
{
	#region Private Classes

	[System.Serializable]
	private class FormationCards
	{
		public UnitCard Card;
		public Button UndoButton;

		private bool isEnabled;
		private bool isSelected;
		private bool isLastSelected;

		/// <summary>
		/// Whether or not the cards are to be displayed for units.
		/// </summary>
		public bool IsEnabled
		{
			get
			{
				// Return value
				return isEnabled;
			}
			set
			{
				// Store value
				isEnabled = value;

				// Display or hide card
				Card.IsEnabled = isEnabled && isSelected;

				// Display or hide button
				UndoButton.gameObject.SetActive ( isEnabled ? isLastSelected : false );
			}
		}

		/// <summary>
		/// Whether or not the cards should be displayed for units already positioned.
		/// </summary>
		public bool IsSelected
		{
			get
			{
				// Return value
				return isSelected;
			}
			set
			{
				// Store value
				isSelected = value;

				// Display or hide card
				Card.IsEnabled = isSelected && isEnabled;

				// Display or hide button
				UndoButton.gameObject.SetActive ( isSelected ? isLastSelected : false );
			}
		}

		/// <summary>
		/// Whether or not the undo button should be displayed for the last unit positioned.
		/// </summary>
		public bool IsLastSelected
		{
			get
			{
				// Return value
				return isLastSelected;
			}
			set
			{
				// Store value
				isLastSelected = value;

				// Display or hide button
				UndoButton.gameObject.SetActive ( isLastSelected );
			}
		}
	}

	[System.Serializable]
	private class FormationTiles
	{
		public SpriteRenderer Tile;
		public SpriteRenderer Outline;
		public SpriteRenderer Unit;

		private bool hasUnit;

		/// <summary>
		/// Whether or not a unit has been positioned on this tile.
		/// </summary>
		public bool HasUnit
		{
			get
			{
				// Return value
				return hasUnit;
			}
			set
			{
				// Store value
				hasUnit = value;

				// Display or hide unit
				Unit.gameObject.SetActive ( hasUnit );
			}
		}
	}

	#endregion // Private Classes

	#region UI Elements

	[SerializeField]
	private GameObject cardsPanel;

	[SerializeField]
	private UnitCard _currentCard;

	[SerializeField]
	private FormationCards [ ] _previousCards;

	[SerializeField]
	private GameObject portaitsPanel;

	[SerializeField]
	private GameObject confirmPanel;

	[SerializeField]
	private UnitPortrait [ ] portraits;

	[SerializeField]
	private Button selectButton;

	
	public TeamSlotMeter slotMeter;
	public HeroCard currentCard;
	public HeroCard [ ] previousCards;
	
	public GameObject pawnCard;
	public TextMeshProUGUI pawnCountText;
	public GameObject pawnUndoButton;

	#endregion // UI Elements

	#region Game Objects

	[SerializeField]
	private GameObject teamFormationObjs;

	[SerializeField]
	private FormationTiles [ ] _tiles;


	[SerializeField]
	private SpriteRenderer [ ] tiles;

	[SerializeField]
	private SpriteRenderer [ ] tileOutlines;

	[SerializeField]
	private SpriteRenderer [ ] tileIcons;

	#endregion // Game Objects

	#region Menu Data

	[SerializeField]
	private TeamSetup setupManager;

	private bool canSelect = true;
	private int tileIndex = 0;
	private readonly Color32 SELECTED_TILE = new Color32 ( 255, 210, 75, 255 );
	private readonly Color32 UNSELECTED_TILE = new Color32 ( 200, 200, 200, 255 );

	#endregion // Menu Data

	#region Player Data

	private int unitIndex;

	private PlayerSettings player;
	private int heroIndex = 0;
	private int pawnIndex = -1;
	private int pawnTotal = 0;
	private int [ ] pawnPositions;
	private bool willAutoFillPawns = true;

	#endregion // Player Data

	// HACK
	public Sprite [ ] icons;

	#region Menu Override Functions

	/// <summary>
	/// Opens the team formation menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true, params object [ ] values )
	{
		// Open the menu
		base.OpenMenu ( closeParent );
		cardsPanel.SetActive ( true );
		portaitsPanel.SetActive ( true );
		confirmPanel.SetActive ( false );
		teamFormationObjs.SetActive ( true );

		// Set unit portraits
		for ( int i = 0; i < portraits.Length; i++ )
		{
			// Set whether or not the portrait has a unit to display
			portraits [ i ].IsEnabled = i < setupManager.CurrentPlayer.Units.Count;

			// Check for portrait
			if ( portraits [ i ].IsEnabled )
			{
				// Display unit in portrait
				portraits [ i ].SetPortrait ( setupManager.CurrentPlayer.Units [ i ], setupManager.CurrentPlayer.Team );
			}
		}

		// Set tiles
		for ( int i = 0; i < _tiles.Length; i++ )
		{
			// Set outline color
			_tiles [ i ].Outline.color = Util.TeamColor ( setupManager.CurrentPlayer.Team );


		}


		// Set the player
		player = values [ 0 ] as PlayerSettings;
		heroIndex = 0;
		pawnIndex = 0;
		tileIndex = 0;
		canSelect = true;

		// Set pawns
		int slotCount = 1;
		willAutoFillPawns = true;
		for ( int i = 0; i < player.heroIDs.Count; i++ )
		{
			// Count slots
			slotCount += HeroInfo.GetHeroByID ( player.heroIDs [ i ] ).Slots;

			// Check for multi-slot heroes
			if ( HeroInfo.GetHeroByID ( player.heroIDs [ i ] ).Slots > 1 )
				willAutoFillPawns = false;
		}
		pawnTotal = slotMeter.TotalSlots - slotCount;
		pawnPositions = new int [ pawnTotal ];

		// Disable selection button
		selectButton.interactable = false;

		// Set tiles
		for ( int i = 0; i < tiles.Length; i++ )
		{
			// Set outline color
			tileOutlines [ i ].color = Util.TeamColor ( player.Team );

			// Set icon color
			tileIcons [ i ].color = Util.TeamColor ( player.Team );

			// Hide icon
			if ( i != 0 )
				tileIcons [ i ].gameObject.SetActive ( false );
		}

		// Set icons
		for ( int i = 0; i < portraits.Length; i++ )
		{
			// Toggle portrait
			portraits [ i ].gameObject.SetActive ( i < player.heroIDs.Count );

			// Display hero
			if ( i < player.heroIDs.Count )
				portraits [ i ].SetUnit ( player.heroIDs [ i ], icons [ player.heroIDs [ i ] ], Util.TeamColor ( player.Team ) );
		}

		// Set cards
		for ( int i = 0; i < previousCards.Length; i++ )
		{
			// Check for hero
			if ( i < player.heroIDs.Count )
			{
				// Set card color
				previousCards [ i ].SetTeamColor ( Util.TeamColor ( player.Team ) );

				// Display card
				previousCards [ i ].DisplayCardWithoutHero ( );
				previousCards [ i ].SetControls ( HeroCard.CardControls.NONE );
			}
			else
			{
				// Hide card
				previousCards [ i ].HideCard ( );
			}
		}

		// Set current card color
		currentCard.SetTeamColor ( Util.TeamColor ( player.Team ) );

		// Reset slot meter
		slotMeter.ResetMeter ( );

		// Set unit for position selection
		DisplayUnit ( false );

		// Display prompt
		setupManager.splash.Slide ( "<size=75%>" + player.PlayerName + "</size>\n<color=white>Team Formation", Util.TeamColor ( player.Team ), true );
	}

	#endregion // Menu Functions

	#region Public Functions

	#region Mouse Input Functions

	/// <summary>
	/// Highlights an available tile when the mouse starts hovering over the tile.
	/// </summary>
	/// <param name="index"> The tile's index in the array. </param>
	public void OnTileEnter ( int index )
	{
		// Check if the position is available
		if ( canSelect && player.Formation [ index ] == MatchSettings.NO_UNIT && index != tileIndex )
		{
			// Display icon
			tileIcons [ index ].gameObject.SetActive ( true );

			// Check index
			if ( heroIndex < player.heroIDs.Count )
				tileIcons [ index ].sprite = icons [ player.heroIDs [ heroIndex ] ]; // Display hero
			else
				tileIcons [ index ].sprite = icons [ MatchSettings.PAWN_UNIT ]; // Display pawn
		}
	}

	/// <summary>
	/// Unhighlights an available tile when the mouse stops hovering over the tile.
	/// </summary>
	/// <param name="index"> The tile's index in the array. </param>
	public void OnTileExit ( int index )
	{
		// Check if the position is available
		if ( canSelect && player.Formation [ index ] == MatchSettings.NO_UNIT && index != tileIndex )
		{
			// Hide icon
			tileIcons [ index ].gameObject.SetActive ( false );
		}
	}

	/// <summary>
	/// Selects the starting position for the current unit.
	/// </summary>
	/// <param name="index"> The tile's index in the array. </param>
	public void OnTileClick ( int index )
	{
		// Check if the position is available
		if ( canSelect && player.Formation [ index ] == MatchSettings.NO_UNIT && index != tileIndex )
		{
			// Clear previous tile
			if ( tileIndex != 0 )
			{
				tileIcons [ tileIndex ].gameObject.SetActive ( false );
				tiles [ tileIndex ].color = UNSELECTED_TILE;
			}

			// Display icon
			tileIcons [ index ].gameObject.SetActive ( true );

			// Check index
			if ( heroIndex < player.heroIDs.Count )
				tileIcons [ index ].sprite = icons [ player.heroIDs [ heroIndex ] ]; // Display hero
			else
				tileIcons [ index ].sprite = icons [ MatchSettings.PAWN_UNIT ]; // Display pawn

			// Highlight the tile
			tiles [ index ].color = SELECTED_TILE;

			// Store tile
			tileIndex = index;

			// Enable select button
			selectButton.interactable = true;
		}
	}

	#endregion // Mouse Input Functions

	/// <summary>
	/// Confirms the starting position for the current unit.
	/// </summary>
	public void SelectPosition ( )
	{
		// Store position
		if ( !willAutoFillPawns && heroIndex == player.heroIDs.Count )
		{
			player.Formation [ tileIndex ] = MatchSettings.PAWN_UNIT;
			pawnPositions [ pawnIndex ] = tileIndex;
		}
		else
		{
			player.Formation [ tileIndex ] = player.heroIDs [ heroIndex ];
		}

		// Disable selection button
		selectButton.interactable = false;

		// Unhighlight tile
		tiles [ tileIndex ].color = UNSELECTED_TILE;

		// Fill slots
		slotMeter.SetMeter ( slotMeter.FilledSlots + slotMeter.PreviewedSlots );

		// Increment index
		if ( heroIndex < player.heroIDs.Count )
			heroIndex++;
		else
			pawnIndex++;

		// Clear tile index
		tileIndex = 0;

		// Check if more units need to be positioned
		if ( heroIndex < player.heroIDs.Count || ( !willAutoFillPawns && pawnIndex < pawnTotal ) )
		{
			// Display next unit
			DisplayUnit ( false );
		}
		else
		{
			// Disable selection
			canSelect = false;

			// Hide unit controls
			cardsPanel.SetActive ( false );
			portaitsPanel.SetActive ( false );

			// Check if pawns will be auto-filled
			if ( willAutoFillPawns )
			{
				Sequence s = DOTween.Sequence ( );
				for ( int i = 0; i < player.Formation.Length; i++ )
				{
					// Display pawns in the formation
					if ( player.Formation [ i ] == MatchSettings.NO_UNIT )
					{
						player.Formation [ i ] = MatchSettings.PAWN_UNIT;
						tileIcons [ i ].gameObject.SetActive ( true );
						tileIcons [ i ].sprite = icons [ MatchSettings.PAWN_UNIT ];
						s.AppendInterval ( 0.1f );
						s.Append ( tileIcons [ i ].DOFade ( 0, 0.25f ).From ( ) );
					}
				}
				s.AppendInterval ( 0.1f )
					.OnComplete ( ( ) =>
					{
						// Display confirmation controls
						confirmPanel.SetActive ( true );
					} )
					.Play ( );
			}
			else
			{
				// Display confirmation controls
				confirmPanel.SetActive ( true );
			}
		}
	}

	/// <summary>
	/// Randomly selects a tile to place the next unit.
	/// </summary>
	public void RandomPlacement ( )
	{
		// Get a random position
		int index;
		do
		{
			index = Random.Range ( 1, player.Formation.Length );
		} while ( player.Formation [ index ] != MatchSettings.NO_UNIT );

		// Select the position
		OnTileClick ( index );
		SelectPosition ( );
	}

	/// <summary>
	/// Unselects the position for the last unit that was positioned.
	/// </summary>
	public void UnselectPosition ( )
	{
		// Check for cancelling the confirmation
		if ( ( willAutoFillPawns && heroIndex == player.heroIDs.Count ) || ( !willAutoFillPawns && pawnIndex == pawnTotal ) )
		{
			// Display controls
			cardsPanel.SetActive ( true );
			portaitsPanel.SetActive ( true );
			confirmPanel.SetActive ( false );

			// Remove pawns if they were auto filled
			if ( willAutoFillPawns )
			{
				// Check each position
				for ( int i = 0; i < player.Formation.Length; i++ )
				{
					// Check for pawn
					if ( player.Formation [ i ] == MatchSettings.PAWN_UNIT )
					{
						// Remove pawn
						tileIcons [ i ].gameObject.SetActive ( false );
						player.Formation [ i ] = MatchSettings.NO_UNIT;
					}
				}
					
			}

			// Enable selection
			canSelect = true;
		}

		// Clear previous tile
		if ( tileIndex != 0 )
		{
			tileIcons [ tileIndex ].gameObject.SetActive ( false );
			tiles [ tileIndex ].color = UNSELECTED_TILE;
		}
		tileIndex = 0;
		int tempTileIndex = 0;

		// Check if a pawn or hero was the previous unit
		if ( !willAutoFillPawns && pawnIndex != 0 )
		{
			// Remove pawn position
			if ( pawnIndex < pawnTotal )
				pawnPositions [ pawnIndex ] = 0;

			// Decrement to last pawn
			pawnIndex--;

			// Get tile index
			tempTileIndex = pawnPositions [ pawnIndex ];
		}
		else
		{
			// Decrement to last hero
			heroIndex--;

			// Get tile index
			for ( int i = 0; i < player.Formation.Length; i++ )
			{
				if ( player.Formation [ i ] == player.heroIDs [ heroIndex ] )
				{
					tempTileIndex = i;
					break;
				}
			}
		}

		// Update slot meter
		slotMeter.SetMeter ( slotMeter.FilledSlots - slotMeter.PreviewedSlots );

		// Remove unit from formation
		player.Formation [ tempTileIndex ] = MatchSettings.NO_UNIT;

		// Display the unit as selected
		OnTileClick ( tempTileIndex );

		// Display current unit
		DisplayUnit ( true );

		// Disable select button
		//selectButton.interactable = false;
	}

	/// <summary>
	/// Confirms the player's formation.
	/// </summary>
	public void ConfirmFormation ( )
	{
		// Get next player
//		if ( setup.SetNextPlayer ( ) )
//		{
//			// Close menu and begin the next player's team selection
//			teamFormationObjs.SetActive ( false );
////			base.CloseMenu ( true, setup.currentPlayer );
//		}
//		else
//		{
//			// Load match
//			setup.BeginMatch ( );
//		}
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Sets the size and color of each unit portrait for the current unit being positioned.
	/// </summary>
	/// <param name="index"> The index of the unit being positioned in the player settings unit list. </param>
	private void SetPortraits ( int index )
	{
		// Set each portrait to the appropriate size and color
		for ( int i = 0; i < setupManager.CurrentPlayer.Units.Count; i++ )
		{
			// Set size of the portrait to be larger if its the current unit
			if ( i == index )
				portraits [ i ].ChangeSize ( 5 );
			else
				portraits [ i ].ResetSize ( );

			// Set the color of the border to be highlighted if its the current unit
			portraits [ i ].IsBorderHighlighted = i == index;
		}
	}

	/// <summary>
	/// Displays or hides all cards in the menu for the current unit being positioned.
	/// </summary>
	/// <param name="index"> The index of the unit being positioned in the player settings unit list. </param>
	private void SetCards ( int index )
	{
		// Display current unit in main card
		_currentCard.SetCard ( setupManager.CurrentPlayer.Units [ index ], setupManager.CurrentPlayer.Team );

		// Display each previous unit in the previous cards
		for ( int i = 0; i < _previousCards.Length; i++ )
		{
			// Set whether or not the card will display a unit
			_previousCards [ i ].IsEnabled = i < setupManager.CurrentPlayer.Units.Count - 2;

			// Check for card
			if ( _previousCards [ i ].IsEnabled )
			{
				// Set whether or not the unit has been positioned for this card
				_previousCards [ i ].IsSelected = i <= index - 2;
				_previousCards [ i ].IsLastSelected = i == index - 2;

				// Check if the card is displayed
				if ( _previousCards [ i ].IsSelected )
				{
					// Display unit
					_previousCards [ i ].Card.SetCard ( setupManager.CurrentPlayer.Units [ i + 1 ], setupManager.CurrentPlayer.Team );
				}
			}
		}
	}

	/// <summary>
	/// Sets the cards and portraits to display the current unit.
	/// Call this after the index has been set.
	/// </summary>
	/// <param name="isUndo"> Whether or not this is being called due to an undo. </param>
	private void DisplayUnit ( bool isUndo )
	{
		// Check for undo
		if ( isUndo )
		{
			// Add undo to previous hero
			if ( heroIndex - 1 >= 0 && pawnIndex < 1 )
				previousCards [ heroIndex - 1 ].SetControls ( HeroCard.CardControls.UNDO );

			// Remove undo from current hero and hide card
			if ( heroIndex < player.heroIDs.Capacity )
			{
				previousCards [ heroIndex ].SetControls ( HeroCard.CardControls.NONE );
				previousCards [ heroIndex ].DisplayCardWithoutHero ( );
			}
		}
		else
		{
			// Remove undo from two prior heroes ago
			if ( heroIndex - 2 >= 0 && pawnIndex <= 1 )
				previousCards [ heroIndex - 2 ].SetControls ( HeroCard.CardControls.NONE );

			// Add undo to previous hero and display card
			if ( heroIndex - 1 >= 0 && pawnIndex < 1 )
			{
				previousCards [ heroIndex - 1 ].SetControls ( HeroCard.CardControls.UNDO );
				previousCards [ heroIndex - 1 ].SetHero ( player.heroIDs [ heroIndex - 1 ], icons [ player.heroIDs [ heroIndex - 1 ] ] );
			}
		}

		// Check if hero or pawn is being displayed
		if ( heroIndex < player.heroIDs.Count )
		{
			// Display current card
			currentCard.gameObject.SetActive ( true );

			// Hide pawn card
			pawnCard.SetActive ( false );

			// Display hero in current card
			currentCard.SetHero ( player.heroIDs [ heroIndex ], icons [ player.heroIDs [ heroIndex ] ] );

			// Preview slots for the currend unit
			slotMeter.PreviewSlots ( HeroInfo.GetHeroByID ( player.heroIDs [ heroIndex ] ).Slots );
		}
		else
		{
			// Hide current card
			currentCard.gameObject.SetActive ( false );

			// Display pawn card
			pawnCard.SetActive ( true );

			// Display pawn count
			pawnCountText.text = ( pawnIndex + 1 ) + " / " + pawnTotal;

			// Display undo button
			pawnUndoButton.SetActive ( pawnIndex > 0 );

			// Preview slots for the currend unit
			slotMeter.PreviewSlots ( 1 );
		}

		// Set previous portrait size
		if ( heroIndex - 1 >= 0 )
			portraits [ heroIndex - 1 ].SelectToggle ( false );

		// Set current portrait size
		if ( heroIndex < player.heroIDs.Count )
			portraits [ heroIndex ].SelectToggle ( true );

		// Set next portrait size
		if ( heroIndex + 1 < player.heroIDs.Count )
			portraits [ heroIndex + 1 ].SelectToggle ( false );
	}

	#endregion // Private Functions
}
