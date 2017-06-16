﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pacifist : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Ghost
	/// Type: Passive Ability
	/// Default Duration: Permanent
	/// 
	/// Ability 2: Obstruction
	/// Type: Command
	/// Default Duration: 2 Turns
	/// Default Cooldown: 5 Turns
	/// 
	/// </summary>

	// Ability information
	public TileObject obstructionPrefab;
	public TileObject currentObstruction;
	private const float OBSTRUCTION_ANIMATION_TIME = 0.75f;

	/// <summary>
	/// Calculates all base moves available to a unit without marking any potential captures.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Cleare previous move list
		if ( prerequisite == null )
			moveList.Clear ( );

		// Check status effects
		if ( canMove )
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( owner.direction );

			// Check each neighboring tile
			for ( int i = 0; i < t.neighbors.Length; i++ )
			{
				// Ignore tiles that would allow for backward movement
				if ( i == back.FirstInt || i == back.SecondInt )
					continue;

				// Check if this unit can move to the neighboring tile
				if ( !returnOnlyJumps && OccupyTileCheck ( t.neighbors [ i ], prerequisite ) )
				{
					// Add as an available move
					moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.Move, i ) );
				}
				// Check if this unit can jump the neighboring tile
				else if ( JumpTileCheck ( t.neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
				{
					// Add as an available jump
					MoveData m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.Jump, i );
					moveList.Add ( m );

					// Find additional jumps
					FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
				}
			}
		}

		// Get obstruction availability
		currentAbility2.active = CommandAvailabilityCheck ( currentAbility2, prerequisite );
	}

	/// <summary>
	/// Determines if this unit can be attaced by another unit.
	/// Always returns false since this unit's Pacifist Ability prevents it from being attacked.
	/// </summary>
	public override bool UnitAttackCheck ( Unit attacker )
	{
		// Prevent any attacks with the Ghost ability
		if ( currentAbility1.enabled )
			return false;

		// Return normal values if the Ghost ability is disabled
		return base.UnitAttackCheck ( attacker );
	}

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( )
	{
		// Clear the board
		base.StartCommand ( );

		// Highlight empty tiles within a 3 tile radius of the hero
		GetObstruction ( currentTile, 2 );
	}

	/// <summary>
	/// Marks every unoccupied tile in a 3 tile radius as available for selection for Obstruction.
	/// </summary>
	private void GetObstruction ( Tile t, int count )
	{
		// Check each adjacent tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Check for tile
			if ( t.neighbors [ i ] != null )
			{
				// Mark as available if unoccupied and not previously marked
				if ( OccupyTileCheck ( t.neighbors [ i ], null ) && t.neighbors [ i ].state == TileState.Default )
					t.neighbors [ i ].SetTileState ( TileState.AvailableCommand );

				// Continue navigation
				if ( count > 0 )
					GetObstruction ( t.neighbors [ i ], count - 1 );
			}
		}
	}

	/// <summary>
	/// Selects the tile to place an obstruction.
	/// </summary>
	public override void SelectCommandTile ( Tile t )
	{
		// Check for previous obstruction
		if ( currentObstruction != null )
		{
			// Remove previous Obstruction
			DestroyTileObject ( currentObstruction );
		}

		// Pause turn timer
		if ( MatchSettings.turnTimer )
			GM.UI.timer.PauseTimer ( );

		// Create Obstruction
		currentObstruction = CreateTileOject ( obstructionPrefab, t, info.ability2.duration, ObstructionDurationComplete );

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Clear board
		GM.board.ResetTiles ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( )
			.Append ( currentObstruction.sprite.DOFade ( 0f, OBSTRUCTION_ANIMATION_TIME ).From ( ) )
			.OnComplete ( ( ) =>
			{
				// Start cooldown
				StartCooldown ( currentAbility2, info.ability2 );

				// Pause turn timer
				if ( MatchSettings.turnTimer )
					GM.UI.timer.ResumeTimer ( );

				// Get moves
				GM.GetTeamMoves ( );

				// Display team
				GM.DisplayAvailableUnits ( );
				GM.SelectUnit ( this );
			} );
	}

	/// <summary>
	/// Delegate for when the duration of the tile object for Obstruction expires.
	/// </summary>
	private void ObstructionDurationComplete ( )
	{
		// Create animation
		Tween t = currentObstruction.sprite.DOFade ( 0f, OBSTRUCTION_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Remove obstruction from player data
				owner.tileObjects.Remove ( currentObstruction );

				// Remove obstruction from the board
				currentObstruction.tile.currentObject = null;

				// Remove obstruction
				Destroy ( currentObstruction.gameObject );
				currentObstruction = null;
			} );

		// Add animation to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}
}
