using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour 
{
	// All tiles in the board
	public Tile [ ] tiles;

	// A reference for tile colors given a tile's state
	public Dictionary<TileState, Color32> tileColorDic = new Dictionary<TileState, Color32> ( );

	/// <summary>
	/// Set the tile reference colors at the start of the match.
	/// </summary>
	private void Start ( )
	{
		// Set tile colors to their associated tile state
		tileColorDic.Add ( TileState.Default,                     new Color32 ( 200, 200, 200, 255 ) ); // Light grey
		tileColorDic.Add ( TileState.AvailableUnit,               new Color32 ( 255, 255, 200, 255 ) ); // Light yellow
		tileColorDic.Add ( TileState.AvailableUnitHover,          new Color32 ( 255, 210,  75, 255 ) ); // Gold
		tileColorDic.Add ( TileState.SelectedUnit,                new Color32 ( 255, 210,  75, 255 ) ); // Gold
		tileColorDic.Add ( TileState.AvailableMove,               new Color32 ( 150, 255, 255, 255 ) ); // Light cyan
		tileColorDic.Add ( TileState.AvailableMoveHover,          new Color32 (   0, 165, 255, 255 ) ); // Dark cyan
		tileColorDic.Add ( TileState.SelectedMove,                new Color32 (   0, 165, 255, 255 ) ); // Dark cyan
		tileColorDic.Add ( TileState.AvailableMoveAttack,         new Color32 ( 150, 255, 255, 255 ) ); // Light cyan
		tileColorDic.Add ( TileState.AvailableMoveAttackHover,    new Color32 (   0, 165, 255, 255 ) ); // Dark cyan
		tileColorDic.Add ( TileState.SelectedMoveAttack,          new Color32 (   0, 165, 255, 255 ) ); // Dark cyan
		tileColorDic.Add ( TileState.AvailableAttack,             new Color32 ( 255, 150, 150, 255 ) ); // Light red
		tileColorDic.Add ( TileState.AvailableAttackHover,        new Color32 ( 200,  50,  50, 255 ) ); // Dark red
		tileColorDic.Add ( TileState.SelectedAttack,              new Color32 ( 200,  50,  50, 255 ) ); // Dark red
		tileColorDic.Add ( TileState.AvailableSpecial,            new Color32 ( 255, 125, 255, 255 ) ); // Light purple
		tileColorDic.Add ( TileState.AvailableSpecialHover,       new Color32 ( 125,   0, 125, 255 ) ); // Purple
		tileColorDic.Add ( TileState.SelectedSpecial,             new Color32 ( 125,   0, 125, 255 ) ); // Purple
		tileColorDic.Add ( TileState.AvailableSpecialAttack,      new Color32 ( 255, 125, 255, 255 ) ); // Light purple
		tileColorDic.Add ( TileState.AvailableSpecialAttackHover, new Color32 ( 125,   0, 125, 255 ) ); // Purple
		tileColorDic.Add ( TileState.SelectedSpecialAttack,       new Color32 ( 125,   0, 125, 255 ) ); // Purple
		tileColorDic.Add ( TileState.AvailableCommand,            new Color32 ( 255, 125, 255, 255 ) ); // Light purple
		tileColorDic.Add ( TileState.AvailableCommandHover,       new Color32 ( 125,   0, 125, 255 ) ); // Purple
		tileColorDic.Add ( TileState.SelectedCommand,             new Color32 ( 125,   0, 125, 255 ) ); // Purple
		tileColorDic.Add ( TileState.ConflictedTile,              new Color32 ( 202, 190, 255, 255 ) ); // Light lavender
		tileColorDic.Add ( TileState.ConflictedTileHover,         new Color32 ( 130, 130, 255, 255 ) ); // Dark lavender
	}

	/// <summary>
	/// Resets every tile on the board to its default state.
	/// </summary>
	public void ResetTiles ( )
	{
		// Reset each tile
		for ( int i = 0; i < tiles.Length; i++ )
		{
			// Set tile to default
			tiles [ i ].SetTileState ( TileState.Default );
		}
	}

	/// <summary>
	/// Resets every tile on the board to its default state except for the selected unit and it's selected moves.
	/// </summary>
	public void ResetTiles ( Tile unitTile )
	{
		// Reset each tile
		for ( int i = 0; i < tiles.Length; i++ )
		{
			// Set tile to default
			if ( tiles [ i ] != unitTile && tiles [ i ].state != TileState.SelectedMove && tiles [ i ].state != TileState.SelectedMoveAttack && tiles [ i ].state != TileState.SelectedAttack && tiles [ i ].state != TileState.SelectedSpecial && tiles [ i ].state != TileState.SelectedSpecialAttack && tiles [ i ].state != TileState.SelectedCommand )
				tiles [ i ].SetTileState ( TileState.Default );
		}
	}
}
