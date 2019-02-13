using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Tile : MonoBehaviour 
{
	#region Tile Data

	[SerializeField]
	private GameManager GM;

	[SerializeField]
	private SpriteRenderer tileRender;

	[SerializeField]
	private SpriteRenderer borderRender;

	public Hex Hex;

	[HideInInspector]
	public Unit CurrentUnit;

	[HideInInspector]
	public TileObject CurrentObject;

	/// <summary>
	/// Whether or not this tile is currently occupied by a unit or tile object.
	/// </summary>
	public bool IsOccupied
	{
		get
		{
			// Return if the tile is occupied
			return CurrentUnit != null || ( CurrentObject != null && !CurrentObject.CanBeOccupied );
		}
	}

	/// <summary>
	/// This tile's current state.
	/// </summary>
	public TileState State
	{
		get;
		private set;
	}

	#endregion // Tile Data

	#region Event Trigger Functions

	/// <summary>
	/// Highlights the tile to display potential actions when the mouse starts hovering over the tile.
	/// </summary>
	public void MouseEnter ( )
	{
		// Display elements
		DisplayGridElements ( );

		// Check state
		switch ( State )
		{
		// Hover over a friendly unit
		case TileState.AvailableUnit:
			HighlightTile ( TileState.AvailableUnitHover );
			break;

		// Hover over an available move
		case TileState.AvailableMove:
			HighlightTile ( TileState.AvailableMoveHover );
			break;

		// Hover over an available move with a potential attack
		case TileState.AvailableMoveAttack:
			HighlightTile ( TileState.AvailableMoveAttackHover );
			SetTargetState ( TileState.AvailableAttackHover, true );
			break;

		// Hover over an available special ability move
		case TileState.AvailableSpecial:
			HighlightTile ( TileState.AvailableSpecialHover );
			break;

		// Hover over an available special ability move with a potential attack
		case TileState.AvailableSpecialAttack:
			HighlightTile ( TileState.AvailableSpecialAttackHover );
			SetTargetState ( TileState.AvailableAttackHover, true );
			break;

		// Hover over an available tile usable for a command
		case TileState.AvailableCommand:
			HighlightTile ( TileState.AvailableCommandHover );
			break;

		// Hover over a conflict tile
		case TileState.ConflictedTile:
			HighlightTile ( TileState.ConflictedTileHover );
			SetTargetState ( TileState.AvailableAttackHover, true );
			GM.UI.SetControlPrompt ( GameManager.TurnState.UNIT_SELECTED, true );
			break;
		}
	}

	/// <summary>
	/// Highlights the tile to display potential actions when the mouse stops hovering over the tile.
	/// </summary>
	public void MouseExit ( )
	{
		// Hide elements
		HideGridElements ( );

		// Return tile color to current state
		HighlightTile ( State );

		// Check if other tiles needs to return their tile color to their current state (in case of potential attacks)
		if ( State == TileState.AvailableMoveAttack || State == TileState.AvailableSpecialAttack )
			SetTargetState ( TileState.AvailableAttack, true );

		if ( State == TileState.ConflictedTile )
		{
			GM.UI.SetControlPrompt ( GameManager.TurnState.UNIT_SELECTED, false );
			SetTargetState ( TileState.AvailableAttack, true );
		}
	}

	/// <summary>
	/// Performs the appropriate action upon a mouse click.
	/// </summary>
	public void MouseClick ( BaseEventData data )
	{
		// Check state
		switch ( State )
		{
		// Select unit
		case TileState.AvailableUnit:
			GM.SelectUnit ( CurrentUnit );
			break;

		// Select move
		case TileState.AvailableMove:
			SetTileState ( TileState.SelectedMove );
			GM.SelectMove ( Hex );
			break;

		// Select attack move
		case TileState.AvailableMoveAttack:
			SetTileState ( TileState.SelectedMoveAttack );
			SetTargetState ( TileState.SelectedAttack );
			GM.SelectMove ( Hex );
			break;

		// Select special move
		case TileState.AvailableSpecial:
			SetTileState ( TileState.SelectedSpecial );
			GM.SelectMove ( Hex );
			break;

		// Select special attack move
		case TileState.AvailableSpecialAttack:
			SetTileState ( TileState.SelectedSpecialAttack );
			SetTargetState ( TileState.SelectedAttack );
			GM.SelectMove ( Hex );
			break;

		// Select a tile for a command
		case TileState.AvailableCommand:
			SetTileState ( TileState.SelectedCommand );
			GM.SelectCommandTarget ( Hex );
			break;

		// Select conflicted move
		case TileState.ConflictedTile:
			GM.UI.SetControlPrompt ( GameManager.TurnState.UNIT_SELECTED, false );
			PointerEventData pointerEventData = data as PointerEventData;
			if ( pointerEventData.button == PointerEventData.InputButton.Left )
			{
				// Get move data
				MoveData md = GM.SelectedUnit.MoveList.Find ( x => x.Destination.Tile == this && x.PriorMove == GM.SelectedMove && x.Type != MoveData.MoveType.SPECIAL );

				// Set highlight
				SetTileState ( TileState.SelectedMove );

				// Set target highlights
				if ( md.IsAttack )
					SetTargetState ( TileState.SelectedAttack );

				// Select move
				GM.SelectMove ( Hex, true, true );
			}
			else if ( pointerEventData.button == PointerEventData.InputButton.Right )
			{
				// Get move data
				MoveData md = GM.SelectedUnit.MoveList.Find ( x => x.Destination.Tile == this && x.PriorMove == GM.SelectedMove && x.Type == MoveData.MoveType.SPECIAL );

				// Set highlight
				SetTileState ( TileState.SelectedSpecialAttack );

				// Set target highlights
				if ( md.IsAttack )
					SetTargetState ( TileState.SelectedAttack );

				// Select move
				GM.SelectMove ( Hex, true, false );
			}
			break;
		}
	}

	#endregion // Event Trigger Functions

	#region Public Functions

	/// <summary>
	/// Sets the state of the tile.
	/// </summary>
	public void SetTileState ( TileState state )
	{
		// Set tile state
		State = state;

		// Highlight tile
		HighlightTile ( state );
	}

	/// <summary>
	/// Highlights the tile according to its current state.
	/// </summary>
	public void HighlightTile ( TileState state )
	{
		// Set the tile's color to its corrisponding state
		tileRender.color = GetColor ( state );
	}

	/// <summary>
	/// Toggles whether to border is displayed.
	/// </summary>
	/// <param name="isActive"> Whether or not the border should be displayed. </param>
	public void SetBorderActive ( bool isActive )
	{
		// Hide or display border
		borderRender.gameObject.SetActive ( isActive );
	}

	/// <summary>
	/// Sets the color of the border.
	/// </summary>
	/// <param name="color"> The color of the border. </param>
	public void SetBorderColor ( Color32 color )
	{
		// Set the color of the border
		borderRender.color = color;
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Sets the state of each target tile for a move.
	/// </summary>
	/// <param name="state"> The state the tile is being set to. </param>
	/// <param name="highlightOnly"> Whethor or not the tile should ignore setting the state and only display the color of the state. </param>
	private void SetTargetState ( TileState state, bool highlightOnly = false )
	{
		// Get a list of moves for this tile
		List<MoveData> moves = GM.SelectedUnit.MoveList.FindAll ( x => x.Destination.Tile == this && x.PriorMove == GM.SelectedMove );

		// Highlight each attack tile
		foreach ( MoveData move in moves )
		{
			// Check for attacks
			if ( !move.IsAttack )
				continue;

			foreach ( Hex hex in move.AttackTargets )
			{
				// Check for hex
				if ( hex == null )
					continue;

				if ( highlightOnly )
					hex.Tile.HighlightTile ( state );
				else
					hex.Tile.SetTileState ( state );
			}
		}
	}

	/// <summary>
	/// Gets the appropriate color of the tile state.
	/// </summary>
	/// <param name="state"> The current tile state. </param>
	/// <returns> The corrisponding tile state color. </returns>
	private Color32 GetColor ( TileState state )
	{
		switch ( state )
		{
		case TileState.Default:                     return new Color32 ( 200, 200, 200, 255 ); // Light grey
		case TileState.AvailableUnit:               return new Color32 ( 255, 255, 200, 255 ); // Light yellow
		case TileState.AvailableUnitHover:          return new Color32 ( 255, 210,  75, 255 ); // Gold
		case TileState.SelectedUnit:                return new Color32 ( 255, 210,  75, 255 ); // Gold
		case TileState.AvailableMove:               return new Color32 ( 150, 255, 255, 255 ); // Light cyan
		case TileState.AvailableMoveHover:          return new Color32 (   0, 165, 255, 255 ); // Dark cyan
		case TileState.SelectedMove:                return new Color32 (   0, 165, 255, 255 ); // Dark cyan
		case TileState.AvailableMoveAttack:         return new Color32 ( 150, 255, 255, 255 ); // Light cyan
		case TileState.AvailableMoveAttackHover:    return new Color32 (   0, 165, 255, 255 ); // Dark cyan
		case TileState.SelectedMoveAttack:          return new Color32 (   0, 165, 255, 255 ); // Dark cyan
		case TileState.AvailableAttack:             return new Color32 ( 255, 150, 150, 255 ); // Light red
		case TileState.AvailableAttackHover:        return new Color32 ( 200,  50,  50, 255 ); // Dark red
		case TileState.SelectedAttack:              return new Color32 ( 200,  50,  50, 255 ); // Dark red
		case TileState.AvailableSpecial:            return new Color32 ( 255, 125, 255, 255 ); // Light purple
		case TileState.AvailableSpecialHover:       return new Color32 ( 125,   0, 125, 255 ); // Purple
		case TileState.SelectedSpecial:             return new Color32 ( 125,   0, 125, 255 ); // Purple
		case TileState.AvailableSpecialAttack:      return new Color32 ( 255, 125, 255, 255 ); // Light purple
		case TileState.AvailableSpecialAttackHover: return new Color32 ( 125,   0, 125, 255 ); // Purple
		case TileState.SelectedSpecialAttack:       return new Color32 ( 125,   0, 125, 255 ); // Purple
		case TileState.AvailableCommand:            return new Color32 ( 255, 125, 255, 255 ); // Light purple
		case TileState.AvailableCommandHover:       return new Color32 ( 125,   0, 125, 255 ); // Purple
		case TileState.SelectedCommand:             return new Color32 ( 125,   0, 125, 255 ); // Purple
		case TileState.ConflictedTile:              return new Color32 ( 202, 190, 255, 255 ); // Light lavender
		case TileState.ConflictedTileHover:         return new Color32 ( 130, 130, 255, 255 ); // Dark lavender
		default:                                    return new Color32 ( 200, 200, 200, 255 ); // Light grey
		}
	}

	/// <summary>
	/// Displays all of the grid elements for this tile.
	/// </summary>
	private void DisplayGridElements ( )
	{
		// Display cursor
		GM.UI.Cursor.OnHexEnter ( Hex );

		// Check for selected unit
		if ( GM.IsUnitSelected )
		{

		}
		else
		{
			// Check for unit
			if ( CurrentUnit != null )
			{
				// Display unit in hud
				GM.UI.UnitHUD.DisplayUnit ( CurrentUnit, false );
			}
		}
	}

	/// <summary>
	/// Hides all of the grid elements for this tile.
	/// </summary>
	private void HideGridElements ( )
	{
		// Hide cursor
		GM.UI.Cursor.OnHexExit ( Hex );

		// Check for selected unit
		if ( GM.IsUnitSelected )
		{

		}
		else
		{
			// Check for unit
			if ( CurrentUnit != null )
			{
				// Hide hud
				GM.UI.UnitHUD.HideHUD ( );
			}
		}
	}

	#endregion // Private Functions
}

public enum TileState
{
	Default,
	AvailableUnit,
	AvailableUnitHover,
	SelectedUnit,
	AvailableMove,
	AvailableMoveHover,
	SelectedMove,
	AvailableMoveAttack,
	AvailableMoveAttackHover,
	SelectedMoveAttack,
	AvailableAttack,
	AvailableAttackHover,
	SelectedAttack,
	AvailableSpecial,
	AvailableSpecialHover,
	SelectedSpecial,
	AvailableSpecialAttack,
	AvailableSpecialAttackHover,
	SelectedSpecialAttack,
	AvailableCommand,
	AvailableCommandHover,
	SelectedCommand,
	ConflictedTile,
	ConflictedTileHover,
	Error
}