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

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Check if it's the start of turn
		if ( !returnOnlyJumps )
		{
			// Get teleport moves
			if ( currentAbility1.enabled && !returnOnlyJumps && currentAbility1.cooldown == 0 )
				GetTeleport ( t, GetBackDirection ( owner.direction ), 2 );

			// Get mad hatter availability
			if ( currentAbility2.enabled && !returnOnlyJumps && currentAbility2.cooldown == 0 )
				currentAbility2.active = true;
			else
				currentAbility2.active = false;
		}
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
				if ( OccupyTileCheck ( t.neighbors [ i ], null ) && !moveList.Exists ( match => match.tile == t.neighbors [ i ] ) )
				{
					// Add as an available special move
					moveList.Add ( new MoveData ( t.neighbors [ i ], null, MoveData.MoveType.Special, i ) );
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
			if ( u != this && !( u is Leader ) )
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
			UseMadHatter ( );
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
	private void UseMadHatter ( )
	{
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
			.Append ( unit1.sprite.DOFade ( 0f, 0.75f ) )
			.Join ( unit2.sprite.DOFade ( 0f, 0.75f ) )
			.AppendCallback ( ( ) =>
			{
				// Reposition units
				unit1.transform.position = unit1.currentTile.transform.position;
				unit2.transform.position = unit2.currentTile.transform.position;
			} )
			.Append ( unit1.sprite.DOFade ( 1f, 0.75f ) )
			.Join ( unit2.sprite.DOFade ( 1f, 0.75f ) )
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
