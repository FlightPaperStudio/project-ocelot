using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pacifist : HeroUnit
{
	/// <summary>
	///
	/// Hero 6 Unit Data
	/// 
	/// ID: 14
	/// Name: Hero 6
	/// Nickname: Ghost
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Support
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 21
	/// Name: Ghost
	/// Description: Unable to attack or be attacked by opponents
	/// Type: Passive
	/// 
	/// Ability 2
	/// ID: 22
	/// Name: Poltergeist
	/// Description: Possesses an objectto block an area for a short period
	/// Type: Command
	/// Duration: 2 Turns
	/// Cooldown: 5 Turns
	/// Area: 2 Tiles
	/// 
	/// </summary>

	// Ability information
	public TileObject obstructionPrefab;
	public TileObject currentObstruction;
	private const float OBSTRUCTION_ANIMATION_TIME = 0.75f;

	#region Public Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit without marking any potential captures.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Cleare previous move list
		if ( prerequisite == null )
			MoveList.Clear ( );

		// Check status effects
		if ( Status.CanMove )
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( Owner.TeamDirection );

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
					MoveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i ) );
				}
				// Check if this unit can jump the neighboring tile
				else if ( JumpTileCheck ( t.neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
				{
					// Add as an available jump
					MoveData m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i );
					MoveList.Add ( m );

					// Find additional jumps
					FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
				}
			}
		}

		// Get obstruction availability
		InstanceData.Ability2.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability2, prerequisite );
	}

	/// <summary>
	/// Determines if this unit can be attaced by another unit.
	/// Always returns false since this unit's Pacifist Ability prevents it from being attacked.
	/// </summary>
	public override bool UnitAttackCheck ( Unit attacker )
	{
		// Prevent any attacks with the Ghost ability
		if ( PassiveAvailabilityCheck ( InstanceData.Ability1, null ) )
			return false;

		// Return normal values if the Ghost ability is disabled
		return base.UnitAttackCheck ( attacker );
	}

	#endregion // Public Unit Override Functions

	#region Public HeroUnit Override Functions

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( AbilityInstanceData ability )
	{
		// Clear the board
		base.StartCommand ( ability );

		// Highlight empty tiles within a 3 tile radius of the hero
		GetObstruction ( currentTile, 2 );
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
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Create Obstruction
		currentObstruction = CreateTileOject ( obstructionPrefab, t, InstanceData.Ability2.Duration, ObstructionDurationComplete );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Board.ResetTiles ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( )
			.Append ( currentObstruction.Icon.DOFade ( 0f, OBSTRUCTION_ANIMATION_TIME ).From ( ) )
			.OnComplete ( ( ) =>
			{
				// Start cooldown
				StartCooldown ( InstanceData.Ability2 );

				// Pause turn timer
				if ( MatchSettings.TurnTimer )
					GM.UI.timer.ResumeTimer ( );

				// Get moves
				GM.GetTeamMoves ( );

				// Display team
				GM.DisplayAvailableUnits ( );
				GM.SelectUnit ( this );
			} );
	}

	#endregion // Public HeroUnit Override Functions

	#region Private Functions

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
				if ( OccupyTileCheck ( t.neighbors [ i ], null ) && t.neighbors [ i ].State == TileState.Default )
					t.neighbors [ i ].SetTileState ( TileState.AvailableCommand );

				// Continue navigation
				if ( count > 0 )
					GetObstruction ( t.neighbors [ i ], count - 1 );
			}
		}
	}

	/// <summary>
	/// Delegate for when the duration of the tile object for Obstruction expires.
	/// </summary>
	private void ObstructionDurationComplete ( )
	{
		// Create animation
		Tween t = currentObstruction.Icon.DOFade ( 0f, OBSTRUCTION_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Remove obstruction from player data
				Owner.tileObjects.Remove ( currentObstruction );

				// Remove obstruction from the board
				currentObstruction.CurrentHex.CurrentObject = null;

				// Remove obstruction
				Destroy ( currentObstruction.gameObject );
				currentObstruction = null;
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	#endregion // Private Functions
}
