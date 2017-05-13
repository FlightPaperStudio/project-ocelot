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
	/// Ability 1: Teleport
	/// Type: Special Ability
	/// Default Cooldown: 2 Turns
	/// 
	/// Ability 2: Mad Hatter
	/// Type: Command
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
	public override void FindMoves ( bool returnOnlyJumps = false )
	{
		// Get base moves
		base.FindMoves ( returnOnlyJumps );

		// Get teleport moves
		if ( currentAbility1.enabled && !returnOnlyJumps && currentAbility1.cooldown == 0 )
			GetTeleport ( currentTile, GetBackDirection ( team.direction ), 2 );

		// Get mad hatter availability
		if ( currentAbility2.enabled && !returnOnlyJumps && currentAbility2.cooldown == 0 )
			currentAbility2.active = true;
		else
			currentAbility2.active = false;
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
				// Start teleport cooldown
				StartCooldown ( currentAbility1, info.ability1 );

				// End the unit's turn after using its special ability
				GM.EndTurn ( );
			} );
	}

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( )
	{
		// Clear board
		base.StartCommand ( );

		// Highlight team members
		foreach ( Unit u in team.units )
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
			t.SetTileState ( TileState.AvailableCommandSelected );
		}
		else
		{
			// Set command data
			tile2 = t;
			unit2 = t.currentUnit;

			// Set tile as selected
			t.SetTileState ( TileState.AvailableCommandSelected );

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

				// Get moves
				GM.GetTeamMoves ( false );

				// Display team
				GM.DisplayAvailableUnits ( );
				GM.SelectUnit ( this );
			} );
	}
}
