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

			// Check if this unit can move to the neighboring tile
			if ( !returnOnlyJumps && OccupyTileCheck ( currentTile.neighbors [ i ] ) )
			{
				// Add as an available move
				moveList.Add ( new MoveData ( currentTile.neighbors [ i ], MoveData.MoveType.Move, i ) );
			}
			// Check if this unit can jump the neighboring tile
			else if ( JumpTileCheck ( currentTile.neighbors [ i ] ) && OccupyTileCheck ( currentTile.neighbors [ i ].neighbors [ i ] ) )
			{
				// Add as an available jump
				moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.Jump, i ) );
			}
		}

		// Get obstruction availability
		if ( currentAbility2.enabled && !returnOnlyJumps && currentAbility2.cooldown == 0 )
			currentAbility2.active = true;
		else
			currentAbility2.active = false;
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

		// Highlight empty tiles
		foreach ( Tile t in GM.board.tiles )
			if ( t.currentUnit == null && t.currentObject == null )
				t.SetTileState ( TileState.AvailableCommand );
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
		// Remove obstruction from the board
		currentObstruction.tile.currentObject = null;

		// Create animation
		Tween t = currentObstruction.sprite.DOFade ( 0f, 0.75f )
			.OnComplete ( ( ) =>
			{
				// Check for Obstruciton ability
				if ( current == currentAbility2 )
				{
					// Remove obstruction
					Destroy ( currentObstruction.gameObject );
					currentObstruction = null;
				}
			} );

		// Add animation to queue
		GM.startOfTurnAnimations.Add ( t );
	}
}
