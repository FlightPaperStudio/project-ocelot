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
		if ( currentAbility1.enabled && currentAbility1.cooldown > 0 )
		{
			// Clear move list
			moveList.Clear ( );
			SetMoveList ( );
		}
		else
		{
			// Get base moves
			base.FindMoves ( returnOnlyJumps );

			// Get special moves
			if ( currentAbility1.enabled && !returnOnlyJumps )
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
			if ( t.neighbors [ i ] != null && !IsBlockedTile ( t.neighbors [ i ] ) && t.neighbors [ i ].neighbors [ i ] != null && !IsBlockedTile ( t.neighbors [ i ].neighbors [ i ] ) && t.neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && !IsBlockedTile ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ] ) && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].currentUnit == null )
			{
				// Check for available captures
				if ( ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) ) || ( t.neighbors [ i ].neighbors [ i ].currentUnit != null && t.neighbors [ i ].neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) ) )
				{
					// Check if the first neighbor tile has a potential capture but the second neighbor tile does not
					if ( ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].currentUnit == null || !t.neighbors [ i ].neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) ) )
					{
						// Add as an available special move with captures
						moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], MoveData.MoveType.SpecialCapture, i, t.neighbors [ i ] ) );
					}
					// Check if the second neighbor tile has a potential capture but the first neighbor tile does not
					else if ( ( t.neighbors [ i ].currentUnit == null || !t.neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].currentUnit != null && t.neighbors [ i ].neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) ) )
					{
						// Add as an available special move with captures
						moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], MoveData.MoveType.SpecialCapture, i, t.neighbors [ i ].neighbors [ i ] ) );
					}
					// Check if both neighbor tiles have potential captures
					else if ( ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].currentUnit != null && t.neighbors [ i ].neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) ) )
					{
						// Add as an available special move with captures
						moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], MoveData.MoveType.SpecialCapture, i, t.neighbors [ i ], t.neighbors [ i ].neighbors [ i ] ) );
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
