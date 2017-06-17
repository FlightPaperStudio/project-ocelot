using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Teleport : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Blink
	/// Type: Special Ability
	/// Default Cooldown: 2 Turns
	/// 
	/// Ability 2: Translocator
	/// Type: Command Ability
	/// Default Cooldown: 4 Turns
	/// 
	/// </summary>

	// Ability information
	private Tile tile1 = null;
	private Tile tile2 = null;
	private Unit unit1 = null;
	private Unit unit2 = null;
	private const float BLINK_ANIMATION_TIME = 0.75f;

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Blink moves
		if ( SpecialAvailabilityCheck ( currentAbility1, prerequisite ) )
			GetBlink ( t, GetBackDirection ( owner.direction ), 2 );

		// Get Translocator availability
		currentAbility2.active = CommandAvailabilityCheck ( currentAbility2, prerequisite );
	}

	/// <summary>
	/// Checks if the hero is capable of using a special ability.
	/// Returns true if the special ability is available.
	/// </summary>
	protected override bool SpecialAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.SpecialAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check if any moves have been made
		if ( prerequisite != null )
			return false;

		// Return that the ability is available
		return true;
	}

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
				if ( OccupyTileCheck ( t.neighbors [ i ], null ) && !moveList.Exists ( match => match.tile == t.neighbors [ i ] && match.prerequisite == null ) )
				{
					// Add as an available special move
					moveList.Add ( new MoveData ( t.neighbors [ i ], null, MoveData.MoveType.Special, i ) );
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
				transform.position = data.tile.transform.position;
			} );
		Tween t2 = sprite.DOFade ( 1, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Start teleport cooldown
				StartCooldown ( currentAbility1, info.ability1 );

				// Set unit and tile data
				SetUnitToTile ( data.tile );
			} );

		// Add animations to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t2, true ) );
	}

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( )
	{
		// Clear board
		base.StartCommand ( );

		// Highlight team members
		foreach ( Unit u in owner.units )
		{
			// Check status effects
			if ( u != this && !( u is Leader ) && u.status.canBeMoved && u.status.canReceiveAbilityEffectsFriendly )
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

	/// <summary>
	/// Swaps the positions of two teammates.
	/// </summary>
	private void UseTranslocator ( )
	{
		// Interupt both units
		unit1.InteruptUnit ( );
		unit2.InteruptUnit ( );

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Pause turn timer
		if ( MatchSettings.turnTimer )
			GM.UI.timer.PauseTimer ( );

		// Set units to their new tiles
		tile1.currentUnit = unit2;
		tile2.currentUnit = unit1;
		unit1.currentTile = tile2;
		unit2.currentTile = tile1;

		// Clear the board
		GM.board.ResetTiles ( );

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
}
