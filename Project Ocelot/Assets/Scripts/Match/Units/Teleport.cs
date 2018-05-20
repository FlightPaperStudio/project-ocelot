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
	/// Cooldown: 2 Turns
	/// Range: 3 Tiles
	/// 
	/// Ability 2
	/// ID: 33
	/// Name: Translocator
	/// Description: Swaps the position of two allies
	/// Type: Command
	/// Cooldown: 4 Turns
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
			GetBlink ( t, GetBackDirection ( owner.TeamDirection ), 2 );

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
		foreach ( Unit u in owner.UnitInstances )
		{
			// Check status effects
			if ( u != this && !( u is Leader ) && u.Status.CanBeMoved )
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
			unit1 = t.currentUnit;

			// Set tile as selected
			t.SetTileState ( TileState.SelectedCommand );
		}
		else
		{
			// Set command data
			tile2 = t;
			unit2 = t.currentUnit;

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
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Create animation
		Tween t1 = sprite.DOFade ( 0, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Move unit instantly
				transform.position = data.Tile.transform.position;
			} );
		Tween t2 = sprite.DOFade ( 1, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Start teleport cooldown
				StartCooldown ( InstanceData.Ability1 );

				// Set unit and tile data
				SetUnitToTile ( data.Tile );
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
				if ( OccupyTileCheck ( t.neighbors [ i ], null ) && !MoveList.Exists ( match => match.Tile == t.neighbors [ i ] && match.Prerequisite == null ) )
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
		tile1.currentUnit = unit2;
		tile2.currentUnit = unit1;
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
