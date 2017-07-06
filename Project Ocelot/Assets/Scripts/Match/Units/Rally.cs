using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rally : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Rally
	/// Type: Passive Ability
	/// Default Duration: 3 Uses
	/// Default Cooldown: 1 Use Per 2 Turns
	/// 
	/// Ability 2: Backflip
	/// Type: Special Ability
	/// Default Cooldown: 3 Turns
	/// 
	/// </summary>

	// Ability information
	private int rallyRegen = 2;
	private const int RALLY_REGEN_RATE = 2;
	private const float RALLY_ANIMATION_TIME = 0.75f;
	private const string RALLY_STATUS_PROMPT = "Rally";

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Backflip moves
		if ( SpecialAvailabilityCheck ( currentAbility2, prerequisite ) )
			GetBackflip ( t, prerequisite, returnOnlyJumps );
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

		// Check previous moves
		if ( CheckPrequisiteType ( prerequisite ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Determines if any of the prerequisite moves used abilities.
	/// Returns true if an ability was used.
	/// </summary>
	private bool CheckPrequisiteType ( MoveData m )
	{
		// Check for prerequisite move
		if ( m != null )
		{
			// Check if the type matches
			if ( m.type == MoveData.MoveType.Special || m.type == MoveData.MoveType.SpecialAttack )
			{
				// Return that an ability has been used
				return true;
			}
			else
			{
				// Check prerequisite move's type
				return CheckPrequisiteType ( m.prerequisite );
			}
		}

		// Return that no abilities were used in previous moves
		return false;
	}

	/// <summary>
	/// Marks every tile available to the Backflip ability.
	/// </summary>
	private void GetBackflip ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.direction );

		// Check each neighboring tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i != back.FirstInt && i != back.SecondInt )
				continue;

			// Check if this unit can move to the neighboring tile
			if ( !returnOnlyJumps && OccupyTileCheck ( t.neighbors [ i ], prerequisite ) )
			{
				// Add as an available move
				moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.Special, i ) );
			}
			// Check if this unit can jump the neighboring tile
			else if ( JumpTileCheck ( t.neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
			{
				// Track move data
				MoveData m;

				// Check if the neighboring unit can be attacked
				if ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
				{
					// Add as an available attack
					m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.SpecialAttack, i, t.neighbors [ i ] );
				}
				else
				{
					// Add as an available jump
					m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.Special, i );
				}

				// Add move to the move list
				moveList.Add ( m );

				// Find additional jumps
				FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
			}
		}
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Check for attack
		if ( data.type == MoveData.MoveType.SpecialAttack )
		{
			// Create animation
			Tween t = transform.DOMove ( data.tile.transform.position, MOVE_ANIMATION_TIME * 2 )
				.OnComplete ( ( ) =>
				{
						// Start teleport cooldown
						StartCooldown ( currentAbility2, info.ability2 );

						// Set unit and tile data
						SetUnitToTile ( data.tile );
				} );

			// Add animation to queue
			GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );

			// Attack the unit
			AttackUnit ( data );
		}
		else
		{
			// Check for normal move
			if ( data.prerequisite == null && currentTile.neighbors [ (int)data.direction ] == data.tile )
			{
				// Create animation
				Tween t = transform.DOMove ( data.tile.transform.position, MOVE_ANIMATION_TIME )
					.OnComplete ( ( ) =>
					{
						// Start teleport cooldown
						StartCooldown ( currentAbility2, info.ability2 );

						// Set unit and tile data
						SetUnitToTile ( data.tile );
					} );

				// Add animation to queue
				GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
			else
			{
				// Create animation
				Tween t = transform.DOMove ( data.tile.transform.position, MOVE_ANIMATION_TIME * 2 )
					.OnComplete ( ( ) =>
					{
						// Start teleport cooldown
						StartCooldown ( currentAbility2, info.ability2 );

						// Set unit and tile data
						SetUnitToTile ( data.tile );
					} );

				// Add animation to queue
				GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );

				// Check for Rally
				if ( currentAbility1.enabled && currentAbility1.duration > 0 )
				{
					// Get unit
					Unit u;
					if ( data.prerequisite == null )
						u = currentTile.neighbors [ (int)data.direction ].currentUnit;
					else
						u = data.prerequisite.tile.neighbors [ (int)data.direction ].currentUnit;

					// Check unit status
					if ( u.status.canMove )
						ActivateRally ( u );
				}
			}
		}
	}

	/// <summary>
	/// Have the unit jump an adjacent unit.
	/// This function builds the animation queue from the move data.
	/// </summary>
	protected override void Jump ( MoveData data )
	{
		// Move as normal
		base.Jump ( data );

		// Check for Rally
		if ( currentAbility1.enabled && currentAbility1.duration > 0 && data.type != MoveData.MoveType.Attack && data.type != MoveData.MoveType.SpecialAttack )
		{
			// Get unit
			Unit u;
			if ( data.prerequisite == null )
				u = currentTile.neighbors [ (int)data.direction ].currentUnit;
			else
				u = data.prerequisite.tile.neighbors [ (int)data.direction ].currentUnit;

			// Check unit status
			if ( u.status.canMove )
				ActivateRally ( u );
		}
	}

	/// <summary>
	/// Activates the Rally ability. This adds a unit to the unit queue.
	/// This function builds the animation queue.
	/// </summary>
	private void ActivateRally ( Unit u )
	{
		// Create animation
		Tween t = u.sprite.DOColor ( Color.white, RALLY_ANIMATION_TIME )
			.SetLoops ( 2, LoopType.Yoyo )
			.OnComplete ( ( ) =>
			{
				// Decrease Rally duration
				currentAbility1.duration--;

				// Add unit to unit queue
				GM.unitQueue.Add ( u );

				// Apply the unit's status effect
				u.status.AddStatusEffect ( abilitySprite1, RALLY_STATUS_PROMPT, this, 1 );

				// Update HUD
				GM.UI.unitHUD.DisplayAbility ( currentAbility1 );
			} );

		// Add animation to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Decrements the cooldown for the unit's special ability.
	/// </summary>
	public override void Cooldown ( )
	{
		// Check for active ability type for ability 1
		if ( currentAbility1.enabled )
		{
			// Check if current duration is active
			if ( currentAbility1.duration < info.ability1.duration )
			{
				// Check for regen
				if ( rallyRegen > 0 )
				{
					// Decrement regen
					rallyRegen--;

					// Check regen
					if ( rallyRegen == 0 )
					{
						// Increase duration
						currentAbility1.duration++;

						// Reset regen
						rallyRegen = RALLY_REGEN_RATE;
					}
				}
			}
		}

		// Set cooldown for ability 2
		base.Cooldown ( );
	}
}
