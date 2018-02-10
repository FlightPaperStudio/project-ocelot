using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dropkick : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Divebomb
	/// Type: Special Ability
	/// Default Cooldown: 3 Turns
	/// 
	/// Ability 2: Dropkick
	/// Type: Command Ability
	/// Default Cooldown: 4 Turns
	/// 
	/// </summary>

	#region Ability Data

	private Dictionary<Tile, int> dropkickTargetDirection = new Dictionary<Tile, int> ( );

	private int MAX_DIVE_RANGE = 4;

	#endregion // Ability Data

	#region Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	/// <param name="t"> The tile who's neighbor will be checked for moves. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach this tile. </param>
	/// <param name="returnOnlyJumps"> Whether or not only jump moves should be stored as available moves. </param>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Divebomb moves
		if ( SpecialAvailabilityCheck ( CurrentAbility1, prerequisite ) )
			GetDivebomb ( t, MAX_DIVE_RANGE );

		// Get Dropkick availability
		CurrentAbility2.active = CommandAvailabilityCheck ( CurrentAbility2, prerequisite );
	}

	#endregion // Unit Override Functions

	#region HeroUnit Override Functions

	/// <summary>
	/// Checks if the hero is capable of using a special ability.
	/// Returns true if the special ability is available.
	/// </summary>
	/// /// <param name="current"> The current ability data for the special ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given special ability. </param>
	/// <returns> Whether or not the special ability can be used. </returns>
	protected override bool SpecialAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.SpecialAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check if any move has been made
		if ( prerequisite != null )
			return false;

		// Check for edge
		if ( !EdgeTileCheck ( currentTile ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if this hero is capable of using a command ability.
	/// Returns true if the command ability is available.
	/// </summary>
	/// <param name="current"> The current ability data for the command ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given command ability. </param>
	/// <returns> Whether or not the command ability can be used. </returns>
	protected override bool CommandAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check if a target is available
		if ( !DropkickCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	/// <param name="data"> The move data required for this move. </param>
	protected override void UseSpecial ( MoveData data )
	{
		
	}

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( )
	{
		// Clear the board
		base.StartCommand ( );

		// Clear previous targets
		dropkickTargetDirection.Clear ( );

		// Get back direction
		IntPair back = GetBackDirection ( owner.direction );

		// Check each neighbor tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Check for back direction
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for target
			if ( currentTile.neighbors [ i ] != null && currentTile.neighbors [ i ].currentUnit != null && currentTile.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) && KnockbackTileCheck ( currentTile.neighbors [ i ], i, true ) )
			{
				// Mark target
				currentTile.neighbors [ i ].SetTileState ( TileState.AvailableCommand );

				// Store direction
				dropkickTargetDirection.Add ( currentTile.neighbors [ i ], i );
			}
		}
	}

	/// <summary>
	/// Selects the unit to dropkick.
	/// </summary>
	/// <param name="t"> The selected tile for the command. </param>
	public override void SelectCommandTile ( Tile t )
	{
		// Pause turn timer
		if ( MatchSettings.turnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Clear board
		GM.board.ResetTiles ( );


	}

	#endregion // HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Checks to see if a tile is an edge tile.
	/// </summary>
	/// <param name="t"> The tile being check as an edge tile. </param>
	/// <returns> Whether or not the given tile is an edge tile. </returns>
	private bool EdgeTileCheck ( Tile t )
	{
		// Check each neighbor tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Check for edge
			if ( t.neighbors [ i ] == null )
				return true;
		}

		// Return that no edge was found
		return false;
	}

	/// <summary>
	/// Gets all tiles within range of the Divebomb ability.
	/// </summary>
	/// <param name="t"> The current tile who's neighbors are being checked. </param>
	/// <param name="count"> The range of tiles left to check. </param>
	private void GetDivebomb ( Tile t, int count )
	{
		// Check each neighbor tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Check if the tile exists
			if ( t.neighbors [ i ] != null )
			{
				// Check if the tile is available to move to
				if ( OccupyTileCheck ( t.neighbors [ i ], null ) && MinimumDiveDistance ( t.neighbors [ i ] ) )
				{
					// Add as an available special move
					moveList.Add ( new MoveData ( t.neighbors [ i ], null, MoveData.MoveType.SPECIAL, i ) );	
				}

				// Check if the max range has been reached
				if ( count > 0 )
				{
					// Continue search
					GetDivebomb ( t.neighbors [ i ], count - 1 );
				}
			}
		}
	}

	/// <summary>
	/// Check is if a given tile is the minimum distance (2 tiles) away to dive to.
	/// </summary>
	/// <param name="t"> The tile being check for distance. </param>
	/// <returns> Whether or not the tile is the minimum distance away. </returns>
	private bool MinimumDiveDistance ( Tile t )
	{
		// Check for tile
		if ( t == null )
			return false;

		// Check current tile
		if ( t == currentTile )
			return false;

		// Check neighboring tiles
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
			if ( t == currentTile.neighbors [ i ] )
				return false;

		// Return that the tile is the minimum dive distance away
		return true;
	}

	/// <summary>
	/// Checks if a tile is available for knocking a unit back.
	/// </summary>
	/// <param name="t"> The tile being checked. </param>
	/// <param name="direction"> The direction the unit is being knocked back. </param>
	/// <param name="isRecursive"> Whether or not tiles should be continuously checked. </param>
	/// <returns> Whether or not a tile is available for knockback. </returns>
	private bool KnockbackTileCheck ( Tile t, int direction, bool isRecursive )
	{
		// Check for tile
		if ( t == null )
			return false;

		// Check for available tile
		if ( t.currentUnit == null )
		{
			// Return that there is room for knockback
			return true;
		}
		else
		{
			// Check if tiles should continue to be checked
			if ( isRecursive )
			{
				// Check next tile in the direction
				return KnockbackTileCheck ( t.neighbors [ direction ], direction, isRecursive );
			}
			else
			{
				// Return that there is not room for knockback
				return false;
			}
		}
	}

	/// <summary>
	/// Checks if any targets are avialable for the Dropkick ability.
	/// </summary>
	/// <returns> Whether or not any targets are available. </returns>
	private bool DropkickCheck ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.direction );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for target
			if ( currentTile.neighbors [ i ] != null && currentTile.neighbors [ i ].currentUnit != null && currentTile.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) && KnockbackTileCheck ( currentTile.neighbors [ i ], i, true ) )
				return true;
		}

		// Return that no targets were found
		return false;
	}

	#endregion // Private Functions
}
