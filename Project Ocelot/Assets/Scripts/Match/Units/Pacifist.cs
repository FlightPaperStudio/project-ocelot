using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

	// Command information
	public TileObject obstructionPrefab;
	public TileObject currentObstruction;

	/// <summary>
	/// Calculates all base moves available to a unit without marking any potential captures.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Cleare previous move list
		if ( !returnOnlyJumps )
			moveList.Clear ( );

		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( team.direction );

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

		// Get obstruction availability
		if ( !returnOnlyJumps )
		{
			if ( currentAbility2.enabled && currentAbility2.cooldown == 0 )
				currentAbility2.active = true;
			else
				currentAbility2.active = false;
		}
	}

	/// <summary>
	/// Determines if this unit can be attaced by another unit.
	/// Always returns false since this unit's Pacifist Ability prevents it from being attacked.
	/// </summary>
	public override bool UnitAttackCheck ( Unit attacker )
	{
		// Prevent any attacks
		return false;
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
			// Remove previous obstruction
			currentObstruction.tile.currentObject = null;
			Destroy ( currentObstruction.gameObject );
			currentObstruction = null;
		}

		// Pause turn timer
		if ( MatchSettings.turnTimer )
			GM.UI.timer.PauseTimer ( );

		// Create new obstruction
		currentObstruction = Instantiate ( obstructionPrefab, team.transform );
		currentObstruction.transform.position = t.transform.position;
		currentObstruction.tile = t;
		currentObstruction.owner = this;
		t.currentObject = currentObstruction;

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Clear board
		GM.board.ResetTiles ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( )
			.Append ( currentObstruction.sprite.DOFade ( 0f, 0.75f ).From ( ) )
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
	/// Callback for when the duration of an ability has expired.
	/// Use this for when obstructions have expired.
	/// </summary>
	protected override void OnDurationComplete ( AbilitySettings current )
	{
		// Create animation
		Tween t = currentObstruction.sprite.DOFade ( 0f, 0.75f )
			.OnComplete ( ( ) =>
			{
				// Check for Obstruciton ability
				if ( current == currentAbility2 )
				{
					// Remove obstruction from the board
					currentObstruction.tile.currentObject = null;

					// Remove obstruction
					Destroy ( currentObstruction.gameObject );
					currentObstruction = null;
				}
			} );

		// Add animation to queue
		GM.startOfTurnAnimations.Add ( new GameManager.TurnAnimation ( t, true ) );
	}
}
