using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private int MIN_DIVE_RANGE = 2;
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
			GetDivebomb ( );

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
		return base.CommandAvailabilityCheck ( current, prerequisite );
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

	#endregion // Private Functions
}
