using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Torus : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Torus
	/// Type: Special Ability
	/// Default Cooldown: 2 Turns
	/// 
	/// Ability 2: ???
	/// Type: ???
	/// 
	/// </summary>

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get torus moves
		if ( currentAbility1.cooldown == 0 && !CheckPrequisiteType ( prerequisite ) )
			GetTorus ( t, prerequisite, returnOnlyJumps );
	}

	/// <summary>
	/// Marks every tiles this unit could use its Torus special ability to get to.
	/// </summary>
	private void GetTorus ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( team.direction );

		// Check each neighboring tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for edge tiles
			if ( t.neighbors [ i ] == null )
			{
				// Get opposite direction
				int direction = Util.GetOppositeDirection ( i );

				// Get opposite edge tile
				Tile torusTile = GetEdgeTile ( t, direction );

				// Check if this unit can move to the edge tile
				if ( !returnOnlyJumps && OccupyTileCheck ( torusTile, prerequisite ) )
				{
					// Add as an available move
					moveList.Add ( new MoveData ( torusTile, prerequisite, MoveData.MoveType.Special, i ) );
				}
				// Check if this unit can jump the edge tile
				else if ( JumpTileCheck ( torusTile ) && OccupyTileCheck ( torusTile.neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the unit can be attacked
					if ( torusTile.currentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available attack
						m = new MoveData ( torusTile.neighbors [ i ], prerequisite, MoveData.MoveType.SpecialAttack, i, torusTile );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( torusTile.neighbors [ i ], prerequisite, MoveData.MoveType.Special, i );
					}

					// Add move to the move list
					moveList.Add ( m );

					// Find additional jumps
					FindMoves ( torusTile.neighbors [ i ], m, true );
				}
			}
			// Check for neighboring edge tiles
			else if ( JumpTileCheck ( t.neighbors [ i ] ) && t.neighbors [ i ].neighbors [ i ] == null )
			{
				// Get opposite direction
				int direction = Util.GetOppositeDirection ( i );

				// Get opposite edge tile
				Tile torusTile = GetEdgeTile ( t, direction );

				// Check if this unit can move to the edge tile
				if ( OccupyTileCheck ( torusTile, prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the unit can be attacked
					if ( t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available attack
						m = new MoveData ( torusTile, prerequisite, MoveData.MoveType.SpecialAttack, i, t.neighbors [ i ] );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( torusTile, prerequisite, MoveData.MoveType.Special, i );
					}

					// Add move to the move list
					moveList.Add ( m );

					// Find additional jumps
					FindMoves ( torusTile, m, true );
				}
			}
		}
	}

	/// <summary>
	/// Navigates the hex grid in a direction until it finds an edge tile and returns it.
	/// </summary>
	private Tile GetEdgeTile ( Tile t, int direction )
	{
		// Check for edge
		if ( t.neighbors [ direction ] == null )
		{
			// Return the edge tile
			return t;
		}
		else
		{
			// Continue searching for the edge tile
			return GetEdgeTile ( t.neighbors [ direction ], direction );
		}
	}

	/// <summary>
	/// Determines if any of the prerequisite moves used abilities.
	/// Returns true if an ability was used.
	/// </summary>
	private bool CheckPrequisiteType ( MoveData m )
	{
		// Check for prerequisite move
		if ( m != null )
		{
			// Check if the type matches
			if ( m.type == MoveData.MoveType.Special || m.type == MoveData.MoveType.SpecialAttack )
			{
				// Return that an ability has been used
				return true;
			}
			else
			{
				// Check prerequisite move's type
				return CheckPrequisiteType ( m.prerequisite );
			}
		}

		// Return that no abilities were used in previous moves
		return false;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Get movement distances
		Vector3 offOfBoardDistance = Util.GetTileDistance ( data.direction );
		Vector3 ontoBoardDistance = Util.GetTileDistance ( Util.GetOppositeDirection ( data.direction ) );

		// Set off of board movement position
		Vector3 offOfBoardPos = currentTile.transform.position;
		if ( currentTile.neighbors [ (int)data.direction ] == null )
			offOfBoardPos += offOfBoardDistance;
		else
			offOfBoardPos += ( 2 * offOfBoardDistance );

		// Set onto board movement position
		Vector3 ontoBoardPos = data.tile.transform.position;
		if ( data.tile.neighbors [ Util.GetOppositeDirection ( (int)data.direction ) ] == null )
			ontoBoardPos += ontoBoardDistance;
		else
			ontoBoardPos += ( 2 * ontoBoardDistance );

		// Set animation times
		float offOfBoardTime = 0.5f;
		if ( currentTile.neighbors [ (int)data.direction ] != null )
			offOfBoardTime *= 2;
		float ontoBoardTime = 0.5f;
		if ( data.tile.neighbors [ Util.GetOppositeDirection ( (int)data.direction ) ] != null )
			ontoBoardTime *= 2;

		// Create animations
		Tween t1 = transform.DOMove ( offOfBoardPos, offOfBoardTime )
			.OnComplete ( ( ) =>
			{
				// Move unit instantly
				transform.position = ontoBoardPos;
			} );
		Tween t2 = sprite.DOFade ( 0, 0.25f ).SetDelay ( offOfBoardTime - 0.25f );
		Tween t3 = transform.DOMove ( data.tile.transform.position, ontoBoardTime )
			.OnComplete ( ( ) =>
			{
				// Start torus cooldown
				StartCooldown ( currentAbility1, info.ability1 );

				// Set unit and tile data
				SetUnitToTile ( data.tile );
			} );
		Tween t4 = sprite.DOFade ( 1f, 0.25f );

		// Add animations to queue
		GM.endOfTurnAnimations.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.endOfTurnAnimations.Add ( new GameManager.TurnAnimation ( t2, false ) );
		GM.endOfTurnAnimations.Add ( new GameManager.TurnAnimation ( t3, true ) );
		GM.endOfTurnAnimations.Add ( new GameManager.TurnAnimation ( t4, false ) );
	}
}
