using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Leader : Unit 
{
	/// <summary>
	/// Calculates all base moves available to a unit.
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
			if ( currentTile.neighbors [ i ] != null && !IsBlockedTile ( currentTile.neighbors [ i ] ) )
			{
				// Check if tile is occupied by a unit
				if ( currentTile.neighbors [ i ].currentUnit != null )
				{
					// Check if tile is available for a jump
					if ( currentTile.neighbors [ i ].neighbors [ i ] != null && currentTile.neighbors [ i ].neighbors [ i ].currentUnit == null && !IsBlockedTile ( currentTile.neighbors [ i ].neighbors [ i ] ) )
					{
						// Check if the unit being jumped is capable of being captured
						if ( currentTile.neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) )
						{
							// Check for goal tile
							if ( team.startArea.IsGoalTile ( currentTile.neighbors [ i ].neighbors [ i ] ) )
							{
								// Add as an available capture to win
								moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.JumpCaptureToWin, i, currentTile.neighbors [ i ] ) );
							}
							else
							{
								// Add as an available capture
								moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.JumpCapture, i, currentTile.neighbors [ i ] ) );
							}
						}
						else
						{
							// Check for goal tile
							if ( team.startArea.IsGoalTile ( currentTile.neighbors [ i ].neighbors [ i ] ) )
							{
								// Add as an available jump to win
								moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.JumpToWin, i ) );
							}
							else
							{
								// Add as an available jump
								moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.Jump, i ) );
							}
						}
					}
				}
				else
				{
					// Check if moves are being returned
					if ( !returnOnlyJumps )
					{
						// Check for goal tile
						if ( team.startArea.IsGoalTile ( currentTile.neighbors [ i ] ) )
						{
							// Add as an available move to win
							moveList.Add ( new MoveData ( currentTile.neighbors [ i ], MoveData.MoveType.MoveToWin, i ) );
						}
						else
						{
							// Add as an available move
							moveList.Add ( new MoveData ( currentTile.neighbors [ i ], MoveData.MoveType.Move, i ) );
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Determines how the unit should move based on the Move Data given.
	/// </summary>
	public override void MoveUnit ( MoveData data )
	{
		// Check move data
		switch ( data.type )
		{
		case MoveData.MoveType.Move:
		case MoveData.MoveType.MoveToWin:
			Move ( data );
			break;
		case MoveData.MoveType.Jump:
		case MoveData.MoveType.JumpToWin:
			Jump ( data );
			break;
		case MoveData.MoveType.JumpCapture:
		case MoveData.MoveType.JumpCaptureToWin:
			CaptureUnit ( data );
			Jump ( data );
			break;
		}
	}

	/// <summary>
	/// Moves the unit to an adjecent tile.
	/// If the adjacent tile is a goal tile, the Leader's team wins the match.
	/// </summary>
	protected override void Move ( MoveData data )
	{
		// Set unit and tile data
		SetUnitToTile ( data.tile );

		// Animate the unit's move
		transform.DOMove ( data.tile.transform.position, 0.5f )
			.SetRecyclable ( )
			.OnComplete ( () =>
			{
				// Check if tile is a goal tile
				if ( team.startArea.IsGoalTile ( data.tile ) )
				{
					// Have the player win the match
					GM.WinMatch ( team );
				}
				else
				{
					// End the player's turn after the unit's move
					GM.EndTurn ( );
				}
			} );
	}

	/// <summary>
	/// Has the unit jump an adjacent unit.
	/// </summary>
	protected override void Jump ( MoveData data )
	{
		// Set unit and tile data
		SetUnitToTile ( data.tile );

		// Animate the unit's jump
		transform.DOMove ( data.tile.transform.position, 1.0f )
			.SetRecyclable ( )
			.OnComplete ( () =>
			{
				// Check if tile is a goal tile
				if ( team.startArea.IsGoalTile ( data.tile ) )
				{
					// Have the player win the match
					GM.WinMatch ( team );
				}
				else
				{						
					// Check for any additional jumps
					FindMoves ( true );
					SetMoveList ( );

					// End the player's turn if there are no jumps available
					if ( moveList.Count > 0 )
					{
						// Continue the player's turn
						GM.ContinueTurn ( );
					}
					else
					{
						// End the player's turn after the unit's jump
						GM.EndTurn ( );
					}		
				}
			} );
	}

	/// <summary>
	/// Captures this unit.
	/// If the Leader is captured, then the opposing team wins the match.
	/// </summary>
	public override void GetCaptured ( bool lostMatch = false )
	{
		// Remove unit from the team
		team.units.Remove ( this );

		// Remove unit reference from the tile
		currentTile.currentUnit = null;

		// Animate this unit being captured
		Sequence s = DOTween.Sequence ( )
			.AppendInterval ( 0.5f )
			.Append ( transform.DOScale ( new Vector3 ( 5, 5, 5 ), 0.5f ) )
			.Insert ( 0.5f, sprite.DOFade ( 0, 0.5f ) )
			.SetRecyclable ( )
			.OnComplete ( () =>
			{
				// Have the player lose the match
				GM.LoseMatch ( team );

				// Delete the unit
				Destroy ( this.gameObject );
			} )
			.Play ( );
	}
}
