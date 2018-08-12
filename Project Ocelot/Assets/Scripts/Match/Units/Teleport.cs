using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Teleport : HeroUnit
{
	/// <summary>
	///
	/// Hero 8 Unit Data
	/// 
	/// ID: 18
	/// Name: Hero 8
	/// Nickname: Teleport
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Support
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 32
	/// Name: Blink
	/// Description: Instantly teleports a short distance
	/// Type: Special
	/// Cooldown: 2 Rounds
	/// Range: 3 Tiles
	/// 
	/// Ability 2
	/// ID: 33
	/// Name: Translocator
	/// Description: Swaps the position of two allies
	/// Type: Command
	/// Cooldown: 4 Rounds
	/// Affect Leader: Active
	/// 
	/// </summary>

	// Ability information
	private Tile tile1 = null;
	private Tile tile2 = null;
	private Unit unit1 = null;
	private Unit unit2 = null;
	private const float BLINK_ANIMATION_TIME = 0.75f;

	#region Public Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Blink moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
			GetBlink ( t, GetBackDirection ( Owner.TeamDirection ), InstanceData.Ability1.PerkValue - 1 );

		// Get Translocator availability
		InstanceData.Ability2.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability2, prerequisite );
	}

	#endregion // Public Unit Override Functions

	#region Public HeroUnit Override Functions

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( AbilityInstanceData ability )
	{
		// Clear board
		base.StartCommand ( ability );

		// Highlight team members
		foreach ( Unit u in Owner.UnitInstances )
		{
			// Check status effects
			if ( u != this && u.Status.CanBeMoved && u.Status.CanBeAffectedByAbility && ( InstanceData.Ability2.IsPerkEnabled || !( u is Leader ) ) )
				u.currentTile.SetTileState ( TileState.AvailableCommand );
		}
	}

	/// <summary>
	/// Selects the unit's to swap places.
	/// </summary>
	public override void SelectCommandTile ( Tile t )
	{
		// Check if any selections have been made
		if ( tile1 == null )
		{
			// Set command data
			tile1 = t;
			unit1 = t.CurrentUnit;

			// Set tile as selected
			t.SetTileState ( TileState.SelectedCommand );
		}
		else
		{
			// Set command data
			tile2 = t;
			unit2 = t.CurrentUnit;

			// Set tile as selected
			t.SetTileState ( TileState.SelectedCommand );

			// Activate Mad Hatter
			UseTranslocator ( );
		}
	}

	/// <summary>
	/// Cancel's the hero's command use.
	/// </summary>
	public override void EndCommand ( )
	{
		// Clear command data
		tile1 = null;
		tile2 = null;
		unit1 = null;
		unit2 = null;

		// Cancel the command
		base.EndCommand ( );
	}

	#endregion // Public HeroUnit Override Functions

	#region Protected HeroUnit Override Functions

	/// <summary>
	/// Checks if the hero is capable of using a special ability.
	/// Returns true if the special ability is available.
	/// </summary>
	protected override bool SpecialAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.SpecialAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check if any moves have been made
		if ( prerequisite != null )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if there adjacent unoccupied tiles available for the Self-Destruct/Recall Ability.
	/// Returns true if at least one adjacent tile is unoccupied.
	/// </summary>
	protected override bool CommandAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check for enough targets
		if ( !TranslocatorCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Create animation
		Tween t1 = sprite.DOFade ( 0, MOVE_ANIMATION_TIME )
			.OnStart ( ( ) =>
			{
				// Mark that the ability is active
				InstanceData.Ability1.IsActive = true;
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
			} )
			.OnComplete ( ( ) =>
			{
				// Move unit instantly
				transform.position = data.Destination.transform.position;
			} );
		Tween t2 = sprite.DOFade ( 1, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability1.IsActive = false;

				// Start teleport cooldown
				StartCooldown ( InstanceData.Ability1 );

				// Set unit and tile data
				SetUnitToTile ( data.Destination );
			} );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, true ) );
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Marks every tile within range of the Blink ability.
	/// </summary>
	private void GetBlink ( Tile t, IntPair back, int count )
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
				if ( OccupyTileCheck ( t.neighbors [ i ], null ) && !MoveList.Exists ( match => match.Destination == t.neighbors [ i ] && match.PriorMove == null ) )
				{
					// Add as an available special move
					MoveList.Add ( new MoveData ( t.neighbors [ i ], null, MoveData.MoveType.SPECIAL, i ) );
				}

				// Check if the maximum range for teleport has been reached
				if ( count > 0 )
				{
					// Continue search
					GetBlink ( t.neighbors [ i ], back, count - 1 );
				}
			}
		}
	}

	/// <summary>
	/// Checks if there are a minimum number of allies to use Translocator.
	/// </summary>
	/// <returns> Whether or not there are enough targets. </returns>
	private bool TranslocatorCheck ( )
	{
		// Start counter
		int targetCounter = 0;

		// Check each ally
		foreach ( Unit u in Owner.UnitInstances )
		{
			// Check if the unit is this hero
			if ( u == this )
				continue;

			// Check if the unit can be moved
			if ( !u.Status.CanBeMoved )
				continue;

			// Check if the unit can be affected by abilities
			if ( !u.Status.CanBeAffectedByAbility )
				continue;

			// Increment counter
			targetCounter++;

			// Check for minimum number of targets
			if ( targetCounter >= 2 )
				return true;
		}

		// Return that there are not enough targets
		return false;
	}

	/// <summary>
	/// Swaps the positions of two teammates.
	/// </summary>
	private void UseTranslocator ( )
	{
		// Interupt both units
		unit1.InteruptUnit ( );
		unit2.InteruptUnit ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Set units to their new tiles
		tile1.CurrentUnit = unit2;
		tile2.CurrentUnit = unit1;
		unit1.currentTile = tile2;
		unit2.currentTile = tile1;

		// Clear the board
		GM.Board.ResetTiles ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( )
			.Append ( unit1.sprite.DOFade ( 0f, BLINK_ANIMATION_TIME ) )
			.Join ( unit2.sprite.DOFade ( 0f, BLINK_ANIMATION_TIME ) )
			.AppendCallback ( ( ) =>
			{
				// Reposition units
				unit1.transform.position = unit1.currentTile.transform.position;
				unit2.transform.position = unit2.currentTile.transform.position;
			} )
			.Append ( unit1.sprite.DOFade ( 1f, BLINK_ANIMATION_TIME ) )
			.Join ( unit2.sprite.DOFade ( 1f, BLINK_ANIMATION_TIME ) )
			.OnComplete ( ( ) =>
			{
				// Clear command data
				tile1 = null;
				tile2 = null;
				unit1 = null;
				unit2 = null;

				// Mark that the ability is no longer active
				InstanceData.Ability2.IsActive = false;

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

	#endregion // Private Functions
}
