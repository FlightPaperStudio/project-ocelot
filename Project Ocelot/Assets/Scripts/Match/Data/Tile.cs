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
			HighlightAttacks ( this, TileState.AvailableAttackHover );
			break;

		// Hover over an available special ability move
		case TileState.AvailableSpecial:
			HighlightTile ( TileState.AvailableSpecialHover );
			break;

		// Hover over an available special ability move with a potential attack
		case TileState.AvailableSpecialAttack:
			HighlightTile ( TileState.AvailableSpecialAttackHover );
			HighlightAttacks ( this, TileState.AvailableAttackHover );
			break;

		// Hover over an available tile usable for a command
		case TileState.AvailableCommand:
			HighlightTile ( TileState.AvailableCommandHover );
			break;

		// Hover over a conflict tile
		case TileState.ConflictedTile:
			HighlightTile ( TileState.ConflictedTileHover );
			HighlightAttacks ( this, TileState.AvailableAttackHover );
			GM.UI.conflictPrompt.SetActive ( true );
			break;
		}
	}

	/// <summary>
	/// Highlights the tile to display potential actions when the mouse stops hovering over the tile.
	/// </summary>
	public void MouseExit ( )
	{
		// Return tile color to current state
		HighlightTile ( State );

		// Check if other tiles needs to return their tile color to their current state (in case of potential attacks)
		if ( State == TileState.AvailableMoveAttack || State == TileState.AvailableSpecialAttack )
			HighlightAttacks ( this, TileState.AvailableAttack );

		if ( State == TileState.ConflictedTile )
		{
			GM.UI.conflictPrompt.SetActive ( false );
			HighlightAttacks ( this, TileState.AvailableAttack );
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
			GM.SelectMove ( this );
			break;

		// Select attack move
		case TileState.AvailableMoveAttack:
			SetTileState ( TileState.SelectedMoveAttack );
			HighlightAttacks ( this, TileState.SelectedAttack, true );
			GM.SelectMove ( this );
			break;

		// Select special move
		case TileState.AvailableSpecial:
			SetTileState ( TileState.SelectedSpecial );
			GM.SelectMove ( this );
			break;

		// Select special attack move
		case TileState.AvailableSpecialAttack:
			SetTileState ( TileState.SelectedSpecialAttack );
			HighlightAttacks ( this, TileState.SelectedAttack, true );
			GM.SelectMove ( this );
			break;

		// Select a tile for a command
		case TileState.AvailableCommand:
			SetTileState ( TileState.SelectedCommand );
			HeroUnit h = GM.SelectedUnit as HeroUnit;
			h.SelectCommandTile ( this );
			break;

		// Select conflicted move
		case TileState.ConflictedTile:
			GM.UI.conflictPrompt.SetActive ( false );
			PointerEventData pointerEventData = data as PointerEventData;
			if ( pointerEventData.button == PointerEventData.InputButton.Left )
			{
				MoveData md = GM.SelectedUnit.MoveList.Find ( x => x.Destination == this && x.PriorMove == GM.SelectedMove && ( x.Type != MoveData.MoveType.SPECIAL && x.Type != MoveData.MoveType.SPECIAL_ATTACK ) );
				switch ( md.Type )
				{
				case MoveData.MoveType.MOVE:
				case MoveData.MoveType.MOVE_TO_WIN:
				case MoveData.MoveType.JUMP:
				case MoveData.MoveType.JUMP_TO_WIN:
					SetTileState ( TileState.SelectedMove );
					break;
				case MoveData.MoveType.ATTACK:
				case MoveData.MoveType.ATTACK_TO_WIN:
					SetTileState ( TileState.SelectedMoveAttack );
					HighlightAttacks ( this, TileState.SelectedAttack, true );
					break;
				}
				GM.SelectMove ( this, true, true );
			}
			else if ( pointerEventData.button == PointerEventData.InputButton.Right )
			{
				MoveData md = GM.SelectedUnit.MoveList.Find ( x => x.Destination == this && x.PriorMove == GM.SelectedMove && ( x.Type == MoveData.MoveType.SPECIAL || x.Type == MoveData.MoveType.SPECIAL_ATTACK ) );
				if ( md.Type == MoveData.MoveType.SPECIAL )
				{
					SetTileState ( TileState.SelectedSpecial );
				}
				else
				{
					SetTileState ( TileState.SelectedSpecialAttack );
					HighlightAttacks ( this, TileState.SelectedAttack, true );
				}
				GM.SelectMove ( this, true, false );
			}
			break;
		}
	}

	#endregion // Event Trigger Functions

	#region Public Functions

	/// <summary>
	/// Sets the state of the tile.
	/// </summary>
	public void SetTileState ( TileState s )
	{
		// Set tile state
		State = s;

		// Highlight tile
		HighlightTile ( s );
	}

	/// <summary>
	/// Highlights the tile according to its current state.
	/// </summary>
	public void HighlightTile ( TileState s )
	{
		// Set the tile's color to its corrisponding state
		tileRender.color = board.tileColorDic [ s ];
	}

	#endregion // Public Functions

	// Tile information
	public int ID;

	// Instance information

	

	/// <summary>
	/// Highlights the tiles of each attack for a particular move. 
	/// </summary>
	private void HighlightAttacks ( Tile t, TileState state, bool setTile = false )
	{
		// Get a list of moves for this tile
		List<MoveData> moves = GM.SelectedUnit.MoveList.FindAll ( x => x.Destination == t && x.PriorMove == GM.SelectedMove );

		// Highlight each attack tile
		foreach ( MoveData m in moves )
		{
			foreach ( Tile a in m.Attacks )
			{
				if ( setTile )
					a.SetTileState ( state );
				else
					a.HighlightTile ( state );
			}
		}
	}

	
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