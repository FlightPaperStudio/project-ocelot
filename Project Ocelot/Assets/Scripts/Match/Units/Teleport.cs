using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Teleport : SpecialUnit
{
	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	public override void FindMoves ( bool returnOnlyJumps = false )
	{
		// Get base moves
		base.FindMoves ( returnOnlyJumps );

		// Get special moves
		if ( !returnOnlyJumps && currentCooldown == 0 )
			GetTeleport ( currentTile, GetBackDirection ( team.direction ), 2 );
	}

	/// <summary>
	/// Marks every tile this unit could teleport to.
	/// </summary>
	private void GetTeleport ( Tile t, IntPair back, int count )
	{
		// Check each neighbor tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check if tile is available
			if ( t.neighbors [ i ] != null )
			{
				// Check if tile already has a move associated with it
				if ( t.neighbors [ i ].currentUnit == null && !IsBlockedTile ( t.neighbors [ i ] ) && !moveList.Exists ( match => match.tile == t.neighbors [ i ] ) )
				{
					// Add as an available special move
					moveList.Add ( new MoveData ( t.neighbors [ i ], MoveData.MoveType.Special, i ) );
				}

				// Check if the maximum range for teleport has been reached
				if ( count > 0 )
				{
					// Continue search
					GetTeleport ( t.neighbors [ i ], back, count - 1 );
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

		// Animate teleport
		Sequence s = DOTween.Sequence ( )
			.Append ( sprite.DOFade ( 0, 0.5f ) )
			.AppendCallback ( () =>
			{
				// Move unit instantly
				transform.position = data.tile.transform.position;
			} )
			.Append ( sprite.DOFade ( 1, 0.5f ) )
			.OnComplete ( () =>
			{
				// Start special ability cooldown
				StartCooldown ( );

				// End the unit's turn after using its special ability
				GM.EndTurn ( );
			} );
	}
}
