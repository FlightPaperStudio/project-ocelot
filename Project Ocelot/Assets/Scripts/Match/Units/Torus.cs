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
	public override void FindMoves ( bool returnOnlyJumps = false )
	{
		// Get base moves
		base.FindMoves ( returnOnlyJumps );

		// Get torus moves
		if ( currentAbility1.cooldown == 0 )
			GetTorus ( returnOnlyJumps );
	}

	/// <summary>
	/// Marks every tiles this unit could use its Torus special ability to get to.
	/// </summary>
	private void GetTorus ( bool returnOnlyJumps )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( team.direction );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for edge tiles
			if ( currentTile.neighbors [ i ] == null )
			{
				// Get opposite direction
				int direction = Util.GetOppositeDirection ( i );

				// Get opposite edge tile
				Tile t = GetEdgeTile ( currentTile, direction );

				// Check if this unit can move to the edge tile
				if ( !returnOnlyJumps && OccupyTileCheck ( t ) )
				{
					// Add as an available move
					moveList.Add ( new MoveData ( t, MoveData.MoveType.Special, i ) );
				}
				// Check if this unit can jump the edge tile
				else if ( JumpTileCheck ( t ) && OccupyTileCheck ( t.neighbors [ i ] ) )
				{
					// Check if the unit can be attacked
					if ( t.currentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available capture
						moveList.Add ( new MoveData ( t.neighbors [ i ], MoveData.MoveType.SpecialAttack, i, t ) );
					}
					else
					{
						// Add as an available jump
						moveList.Add ( new MoveData ( t.neighbors [ i ], MoveData.MoveType.Special, i ) );
					}
				}
			}
			// Check for neighboring edge tiles
			else if ( JumpTileCheck ( currentTile.neighbors [ i ] ) && currentTile.neighbors [ i ].neighbors [ i ] == null )
			{
				// Get opposite direction
				int direction = Util.GetOppositeDirection ( i );

				// Get opposite edge tile
				Tile t = GetEdgeTile ( currentTile, direction );

				// Check if this unit can move to the edge tile
				if ( OccupyTileCheck ( t ) )
				{
					// Check if the unit can be attacked
					if ( currentTile.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available capture
						moveList.Add ( new MoveData ( t, MoveData.MoveType.SpecialAttack, i, currentTile.neighbors [ i ] ) );
					}
					else
					{
						// Add as an available jump
						moveList.Add ( new MoveData ( t, MoveData.MoveType.Special, i ) );
					}
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
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Check for jump
		bool isJump = true;
		if ( currentTile.neighbors [ (int)data.direction ] == null && data.tile.neighbors [ Util.GetOppositeDirection ( (int)data.direction ) ] == null )
			isJump = false;

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

		// Set unit and tile data
		SetUnitToTile ( data.tile );

		// Animate torus
		Sequence s = DOTween.Sequence ( )
			.Append ( transform.DOMove ( offOfBoardPos, offOfBoardTime ) )
			.Join ( sprite.DOFade ( 0, 0.25f ).SetDelay ( offOfBoardTime - 0.25f ) )
			.AppendCallback ( () =>
			{
				// Move unit instantly
				transform.position = ontoBoardPos;
			} )
			.AppendInterval ( 0.01f )
			.Append ( transform.DOMove ( data.tile.transform.position, ontoBoardTime ) )
			.Join ( sprite.DOFade ( 1f, 0.25f ) )
			.OnComplete ( () =>
			{
				// Start torus cooldown
				StartCooldown ( currentAbility1, info.ability1 );

				// Check for any additional jumps
				FindMoves ( true );
				SetMoveList ( );

				// End the player's turn if there are no jumps available
				if ( moveList.Count > 0 && isJump )
				{
					// Continue the player's turn
					GM.ContinueTurn ( );
				}
				else
				{
					// End the player's turn after the unit's jump
					GM.EndTurn ( );
				}	
			} )
			.Play ( );
	}
}
