using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacifist : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Pacifist
	/// Type: Passive Ability
	/// 
	/// Ability 2: Obstruction
	/// Type: Command
	/// Default Duration: 2 Turns
	/// Default Cooldown: 5 Turns
	/// 
	/// </summary>

	/// <summary>
	/// Calculates all base moves available to a unit without marking any potential captures.
	/// </summary>
	public override void FindMoves ( bool returnOnlyJumps = false )
	{
		// Cleare previous move list
		moveList.Clear ( );

		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( team.direction );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for existing tile
			if ( currentTile.neighbors [ i ] != null )
			{
				// Check if tile is occupied by a unit
				if ( currentTile.neighbors [ i ].currentUnit != null )
				{
					// Check if tile is available for a jump
					if ( currentTile.neighbors [ i ].neighbors [ i ] != null && currentTile.neighbors [ i ].neighbors [ i ].currentUnit == null && !IsBlockedTile ( currentTile.neighbors [ i ].neighbors [ i ] ) )
					{
						// Add as an available jump
						moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.Jump, i ) );
					}
				}
				else
				{
					// Check if moves are being returned
					if ( !returnOnlyJumps && !IsBlockedTile ( currentTile.neighbors [ i ] ) )
					{
						// Add as an available move
						moveList.Add ( new MoveData ( currentTile.neighbors [ i ], MoveData.MoveType.Move, i ) );
					}
				}
			}
		}
	}

	/// <summary>
	/// Determines if this unit can be captured by another unit jumping it.
	/// Always returns false since this unit cannot be captured.
	/// </summary>
	public override bool UnitCaptureCheck ( Unit jumpingUnit )
	{
		// Prevent being marked as being able to be captured
		return false;
	}
}
