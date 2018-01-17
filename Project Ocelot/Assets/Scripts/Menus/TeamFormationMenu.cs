using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TeamFormationMenu : Menu 
{
	#region UI Elements

	public Button selectButton;
	public GameObject unitInfoPanel;
	public GameObject heroPortaitPanel;
	public GameObject confirmPanel;
	public TeamSlotMeter slotMeter;
	public HeroCard currentCard;
	public HeroCard [ ] previousCards;
	public UnitPortrait [ ] heroPortraits;
	public GameObject pawnCard;
	public TextMeshProUGUI pawnCountText;
	public GameObject pawnUndoButton;

	#endregion // UI Elements

	#region Game Objects

	public GameObject teamFormationObjs;
	public SpriteRenderer [ ] tiles;
	public SpriteRenderer [ ] tileOutlines;
	public SpriteRenderer [ ] tileIcons;

	#endregion // Game Objects

	#region Menu Data

	public TeamSetup setup;
	private bool canSelect = true;
	private int tileIndex = 0;
	private readonly Color32 SELECTED_TILE = new Color32 ( 255, 210, 75, 255 );
	private readonly Color32 UNSELECTED_TILE = new Color32 ( 200, 200, 200, 255 );

	#endregion // Menu Data

	#region Player Data

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
		unitInfoPanel.SetActive ( true );
		heroPortaitPanel.SetActive ( true );
		confirmPanel.SetActive ( false );
		teamFormationObjs.SetActive ( true );

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
			tileOutlines [ i ].color = Util.TeamColor ( player.TeamColor );

			// Set icon color
			tileIcons [ i ].color = Util.TeamColor ( player.TeamColor );

			// Hide icon
			if ( i != 0 )
				tileIcons [ i ].gameObject.SetActive ( false );
		}

		// Set icons
		for ( int i = 0; i < heroPortraits.Length; i++ )
		{
			// Toggle portrait
			heroPortraits [ i ].gameObject.SetActive ( i < player.heroIDs.Count );

			// Display hero
			if ( i < player.heroIDs.Count )
				heroPortraits [ i ].SetUnit ( player.heroIDs [ i ], icons [ player.heroIDs [ i ] ], Util.TeamColor ( player.TeamColor ) );
		}

		// Set cards
		for ( int i = 0; i < previousCards.Length; i++ )
		{
			// Check for hero
			if ( i < player.heroIDs.Count )
			{
				// Set card color
				previousCards [ i ].SetTeamColor ( Util.TeamColor ( player.TeamColor ) );

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
		currentCard.SetTeamColor ( Util.TeamColor ( player.TeamColor ) );

		// Reset slot meter
		slotMeter.ResetMeter ( );

		// Set unit for position selection
		DisplayUnit ( false );

		// Display prompt
		setup.splash.Slide ( "<size=75%>" + player.name + "</size>\n<color=white>Team Formation", Util.TeamColor ( player.TeamColor ), true );
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
		if ( canSelect && player.formation [ index ] == MatchSettings.NO_UNIT && index != tileIndex )
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
		if ( canSelect && player.formation [ index ] == MatchSettings.NO_UNIT && index != tileIndex )
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
		if ( canSelect && player.formation [ index ] == MatchSettings.NO_UNIT && index != tileIndex )
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
			player.formation [ tileIndex ] = MatchSettings.PAWN_UNIT;
			pawnPositions [ pawnIndex ] = tileIndex;
		}
		else
		{
			player.formation [ tileIndex ] = player.heroIDs [ heroIndex ];
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
			unitInfoPanel.SetActive ( false );
			heroPortaitPanel.SetActive ( false );

			// Check if pawns will be auto-filled
			if ( willAutoFillPawns )
			{
				Sequence s = DOTween.Sequence ( );
				for ( int i = 0; i < player.formation.Length; i++ )
				{
					// Display pawns in the formation
					if ( player.formation [ i ] == MatchSettings.NO_UNIT )
					{
						player.formation [ i ] = MatchSettings.PAWN_UNIT;
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
			index = Random.Range ( 1, player.formation.Length );
		} while ( player.formation [ index ] != MatchSettings.NO_UNIT );

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
			unitInfoPanel.SetActive ( true );
			heroPortaitPanel.SetActive ( true );
			confirmPanel.SetActive ( false );

			// Remove pawns if they were auto filled
			if ( willAutoFillPawns )
			{
				// Check each position
				for ( int i = 0; i < player.formation.Length; i++ )
				{
					// Check for pawn
					if ( player.formation [ i ] == MatchSettings.PAWN_UNIT )
					{
						// Remove pawn
						tileIcons [ i ].gameObject.SetActive ( false );
						player.formation [ i ] = MatchSettings.NO_UNIT;
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
			for ( int i = 0; i < player.formation.Length; i++ )
			{
				if ( player.formation [ i ] == player.heroIDs [ heroIndex ] )
				{
					tempTileIndex = i;
					break;
				}
			}
		}

		// Update slot meter
		slotMeter.SetMeter ( slotMeter.FilledSlots - slotMeter.PreviewedSlots );

		// Remove unit from formation
		player.formation [ tempTileIndex ] = MatchSettings.NO_UNIT;

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
		if ( setup.SetNextPlayer ( ) )
		{
			// Close menu and begin the next player's team selection
			teamFormationObjs.SetActive ( false );
			base.CloseMenu ( true, setup.currentPlayer );
		}
		else
		{
			// Load match
			setup.BeginMatch ( );
		}
	}

	#endregion // Public Functions

	#region Private Functions

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
			heroPortraits [ heroIndex - 1 ].SelectToggle ( false );

		// Set current portrait size
		if ( heroIndex < player.heroIDs.Count )
			heroPortraits [ heroIndex ].SelectToggle ( true );

		// Set next portrait size
		if ( heroIndex + 1 < player.heroIDs.Count )
			heroPortraits [ heroIndex + 1 ].SelectToggle ( false );
	}

	#endregion // Private Functions
}
