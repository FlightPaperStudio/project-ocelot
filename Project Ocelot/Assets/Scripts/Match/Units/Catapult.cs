using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Catapult : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Catapult
	/// Type: Special Ability
	/// Default Duration: 2 Turns
	/// Default Cooldown: 3 Turns
	/// 
	/// Ability 2: ???
	/// Type: ???
	/// 
	/// </summary>

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( bool returnOnlyJumps = false )
	{
		// Check if this unit can move this turn
		if ( currentAbility1.enabled && currentAbility1.duration > 0 )
		{
			// Clear move list
			moveList.Clear ( );
			SetMoveList ( );
		}
		else
		{
			// Get base moves
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

				// Check if this unit can move to the neighboring tile
				if ( !returnOnlyJumps && OccupyTileCheck ( currentTile.neighbors [ i ] ) )
				{
					// Add as an available move
					moveList.Add ( new MoveData ( currentTile.neighbors [ i ], MoveData.MoveType.Move, i ) );
				}
				// Check if this unit can jump the neighboring tile
				else if ( base.JumpTileCheck ( currentTile.neighbors [ i ] ) && OccupyTileCheck ( currentTile.neighbors [ i ].neighbors [ i ] ) )
				{
					// Check if the neighboring unit can be attacked
					if ( currentTile.neighbors [ i ].currentUnit != null && currentTile.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available attack
						moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.Attack, i, currentTile.neighbors [ i ] ) );
					}
					else
					{
						// Add as an available jump
						moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.Jump, i ) );
					}
				}
			}

			// Get special moves
			if ( currentAbility1.enabled && !returnOnlyJumps && currentAbility1.cooldown == 0 )
				GetCatapult ( currentTile, GetBackDirection ( team.direction ) );
		}
	}

	/// <summary>
	/// Marks all tiles that this unit could charge to.
	/// </summary>
	private void GetCatapult ( Tile t, IntPair back )
	{
		// Check each neighboring tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check if tile is available
			if ( JumpTileCheck ( t.neighbors [ i ] ) && JumpTileCheck ( t.neighbors [ i ].neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ] ) )
			{
				// Check for available attacks
				if ( ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) || ( t.neighbors [ i ].neighbors [ i ].currentUnit != null && t.neighbors [ i ].neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) )
				{
					// Check if the first neighbor unit can be attacked but the second neighbor unit cannot
					if ( ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].currentUnit == null || !t.neighbors [ i ].neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) )
					{
						// Add as an available special attack move
						moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], MoveData.MoveType.SpecialAttack, i, t.neighbors [ i ] ) );
					}
					// Check if the second neighbor unit can be attacked but the first neighbor unit cannot
					else if ( ( t.neighbors [ i ].currentUnit == null || !t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].currentUnit != null && t.neighbors [ i ].neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) )
					{
						// Add as an available special attack move
						moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], MoveData.MoveType.SpecialAttack, i, t.neighbors [ i ].neighbors [ i ] ) );
					}
					// Check if both neighbor units can be attacked
					else if ( ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].currentUnit != null && t.neighbors [ i ].neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) )
					{
						// Add as an available special attack move
						moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], MoveData.MoveType.SpecialAttack, i, t.neighbors [ i ], t.neighbors [ i ].neighbors [ i ] ) );
					}
				}
				else
				{
					// Add as an available special move
					moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], MoveData.MoveType.Special, i ) );
				}
			}
		}
	}

	/// <summary>
	/// Determines if a tile can be jumped by this unit using the Catapult ability.
	/// Returns true if the tile can be jumped.
	/// </summary>
	protected override bool JumpTileCheck ( Tile t )
	{
		// Check if the tile exists
		if ( t == null )
			return false;

		// Check if the tile is blocked
		if ( IsBlockedTile ( t ) )
			return false;

		// Check fi the tile has a tile object blocking it
		if ( t.currentObject != null && !t.currentObject.canBeJumped )
			return false;

		// Return that the tile can be jumped by this unit
		return true;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Set unit and tile data
		SetUnitToTile ( data.tile );

		// Animate the unit's move
		transform.DOMove ( data.tile.transform.position, 1.5f )
			.SetRecyclable ( )
			.OnComplete ( () =>
			{
				// Start special ability cooldown
				StartCooldown ( currentAbility1, info.ability1 );

				// End the player's turn after the unit's move
				GM.EndTurn ( );
			} );
	}
}
