using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace ProjectOcelot.Match.Setup
{
	public class TeamFormationMenu : Menues.Menu
	{
		#region Private Classes

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
		private Button undoButton;

		[SerializeField]
		private Button randomButton;

		[SerializeField]
		private Button selectButton;

		[SerializeField]
		private Button confirmButton;

		#endregion // UI Elements

		#region Game Objects

		[SerializeField]
		private GameObject teamFormationObjs;

		[SerializeField]
		private FormationTiles [ ] tiles;

		#endregion // Game Objects

		#region Menu Data

		[SerializeField]
		private TeamSetup setupManager;

		private int cardIndex = 0;
		private int unitIndex = 0;
		private int selectTileIndex = -1;

		/// <summary>
		/// Tracks whether or not all units have been positioned.
		/// </summary>
		private bool IsFormationComplete
		{
			get
			{
				// Return true if all units are positioned
				return unitIndex >= setupManager.CurrentPlayer.Units.Count;
			}
		}

		private readonly Color32 SELECTED_TILE = new Color32 ( 255, 210, 75, 255 );
		private readonly Color32 UNSELECTED_TILE = new Color32 ( 200, 200, 200, 255 );

		#endregion // Menu Data

		#region Menu Override Functions

		/// <summary>
		/// Opens the team formation menu.
		/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
		/// </summary>
		public override void OpenMenu ( bool closeParent = true )
		{
			// Open the menu
			base.OpenMenu ( closeParent );
			teamFormationObjs.SetActive ( true );

			// Set instructions
			setupManager.DisplayInstructions ( "Select your formation" );

			// Set tiles
			for ( int i = 0; i < tiles.Length; i++ )
			{
				// Set outline color
				tiles [ i ].Outline.color = Tools.Util.TeamColor ( setupManager.CurrentPlayer.Team );

				// Set tiles to not have any unit displayed
				tiles [ i ].HasUnit = false;
			}

			// Set indexes
			cardIndex = 0;
			unitIndex = 0;

			// Display first unit
			setupManager.DisplayUnit ( setupManager.CurrentPlayer.Units [ unitIndex ], setupManager.CurrentPlayer.Team );

			// Highlight first unit
			setupManager.HighlightCardInLineup ( cardIndex, true, setupManager.CurrentPlayer.Team );

			// Enable/disable buttons until a tile is selected for the unit
			undoButton.interactable = false;
			randomButton.interactable = true;
			selectButton.interactable = false;
			confirmButton.interactable = false;

			// Display prompt
			setupManager.Splash.Slide ( "<size=75%>" + setupManager.CurrentPlayer.PlayerName + "</size>\n<color=white>Team Formation", Tools.Util.TeamColor ( setupManager.CurrentPlayer.Team ), true );
		}

		public override void CloseMenu ( bool openParent = true )
		{
			// Hide the tiles
			teamFormationObjs.SetActive ( false );

			// Close the menu
			base.CloseMenu ( openParent );
		}

		#endregion // Menu Functions

		#region Public Functions

		/// <summary>
		/// Highlights an available tile when the mouse starts hovering over the tile.
		/// </summary>
		/// <param name="index"> The tile's index in the array. </param>
		public void MouseEnter ( int index )
		{
			// Check if unit is currently being displayed at this tile
			if ( !tiles [ index ].HasUnit && !IsFormationComplete )
			{
				// Display unit on tile while hovering over tile
				tiles [ index ].Unit.gameObject.SetActive ( true );
				tiles [ index ].Unit.sprite = setupManager.CurrentPlayer.Units [ unitIndex ].Portrait;
				tiles [ index ].Unit.color = Tools.Util.TeamColor ( setupManager.CurrentPlayer.Team );
			}
		}

		/// <summary>
		/// Unhighlights an available tile when the mouse stops hovering over the tile.
		/// </summary>
		/// <param name="index"> The tile's index in the array. </param>
		public void MouseExit ( int index )
		{
			// Check if unit currently being displayed at this tile
			if ( !tiles [ index ].HasUnit && !IsFormationComplete )
			{
				// Hide any temporary unit displayed now that the mouse is no longer hovering over the tile
				tiles [ index ].HasUnit = false;
			}
		}

		/// <summary>
		/// Selects the starting position for the current unit.
		/// </summary>
		/// <param name="index"> The tile's index in the array. </param>
		public void SelectPosition ( int index )
		{
			// Check if unit is currently being displayed at this tile
			if ( !tiles [ index ].HasUnit && !IsFormationComplete )
			{
				// Check if a different tile is currently selected
				if ( selectTileIndex != -1 )
				{
					// Remove unit
					tiles [ selectTileIndex ].HasUnit = false;

					// Remove highlight
					tiles [ selectTileIndex ].Tile.color = UNSELECTED_TILE;
				}

				// Store the currently selected
				selectTileIndex = index;

				// Display unit on the selected tile
				tiles [ index ].HasUnit = true;
				tiles [ index ].Unit.sprite = setupManager.CurrentPlayer.Units [ unitIndex ].Portrait;
				tiles [ index ].Unit.color = Tools.Util.TeamColor ( setupManager.CurrentPlayer.Team );

				// Highlight the tile to indicate that it is selected
				tiles [ index ].Tile.color = SELECTED_TILE;

				// Enable the select button now that a tile is selected
				selectButton.interactable = true;
			}
		}

		/// <summary>
		/// Undoes the last unit placement.
		/// </summary>
		public void Undo ( )
		{
			// Check if a tile has been selected for the current unit
			if ( selectTileIndex != -1 )
			{
				// Remove unit
				tiles [ selectTileIndex ].HasUnit = false;

				// Remove highlight
				tiles [ selectTileIndex ].Tile.color = UNSELECTED_TILE;
			}

			// Unhighlight current unit
			setupManager.HighlightCardInLineup ( cardIndex, false, setupManager.CurrentPlayer.Team );

			// Move to the previous unit
			cardIndex -= setupManager.CurrentPlayer.Units [ unitIndex ].Slots;
			unitIndex--;

			// Display previous unit
			setupManager.DisplayUnit ( setupManager.CurrentPlayer.Units [ unitIndex ], setupManager.CurrentPlayer.Team );

			// Highlight previous unit
			setupManager.HighlightCardInLineup ( cardIndex, true, setupManager.CurrentPlayer.Team );

			// Store the tile of the previous unit
			selectTileIndex = setupManager.CurrentPlayer.UnitFormation [ setupManager.CurrentPlayer.Units [ unitIndex ] ];

			// Highlight the tile of the previous unit
			tiles [ selectTileIndex ].HasUnit = false;
			SelectPosition ( selectTileIndex );

			// Remove previous unit from the formation
			setupManager.CurrentPlayer.UnitFormation.Remove ( setupManager.CurrentPlayer.Units [ unitIndex ] );

			// Enable/disable buttons
			undoButton.interactable = unitIndex > 0;
			randomButton.interactable = true;
			selectButton.interactable = true;
			confirmButton.interactable = false;
		}

		/// <summary>
		/// Randomly selects a tile to place the next unit.
		/// </summary>
		public void RandomPosition ( )
		{
			// Get the subset of remaining tiles without a unit positioned
			FormationTiles [ ] availableTiles = tiles.Where ( x => !x.HasUnit ).ToArray ( );

			// Check for available tiles
			if ( availableTiles.Length > 0 )
			{
				// Get an available tile at random
				FormationTiles randomTile = availableTiles [ Random.Range ( 0, availableTiles.Length ) ];

				// Select the random tile for the current unit
				SelectPosition ( System.Array.IndexOf ( tiles, randomTile ) );
			}
		}

		/// <summary>
		/// Confirms the starting position for the current unit.
		/// </summary>
		public void ConfirmPosition ( )
		{
			// Store tile as the current unit's starting position
			setupManager.CurrentPlayer.UnitFormation.Add ( setupManager.CurrentPlayer.Units [ unitIndex ], selectTileIndex );

			// Remove highlight from tile
			tiles [ selectTileIndex ].Tile.color = UNSELECTED_TILE;

			// Reset selected tile
			selectTileIndex = -1;

			// Unhighlight current
			setupManager.HighlightCardInLineup ( cardIndex, false, setupManager.CurrentPlayer.Team );

			// Move to the next unit
			cardIndex += setupManager.CurrentPlayer.Units [ unitIndex ].Slots;
			unitIndex++;

			// Check for completion
			if ( !IsFormationComplete )
			{
				// Display the next unit
				setupManager.DisplayUnit ( setupManager.CurrentPlayer.Units [ unitIndex ], setupManager.CurrentPlayer.Team );

				// Highlight next unit
				setupManager.HighlightCardInLineup ( cardIndex, true, setupManager.CurrentPlayer.Team );
			}

			// Enable/disable buttons upon completion
			undoButton.interactable = true;
			randomButton.interactable = !IsFormationComplete;
			selectButton.interactable = false;
			confirmButton.interactable = IsFormationComplete;
		}

		/// <summary>
		/// Confirms the player's formation.
		/// </summary>
		public void ConfirmFormation ( )
		{
			// Move to next player or begin match
			setupManager.SetNextPlayer ( );
		}

		#endregion // Public Functions
	}
}