using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rally : HeroUnit
{
	/// <summary>
	///
	/// Hero 5 Unit Data
	/// 
	/// ID: 13
	/// Name: Hero 5
	/// Nickname: Rally
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Support
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 19
	/// Name: Rally
	/// Description: Assisted allies are inspired to gain additional movement
	/// Type: Passive
	/// Duration: 3 Uses
	/// Regeneration: 1 Use Per 2 Turns
	/// 
	/// Ability 2
	/// ID: 20
	/// Name: Backflip
	/// Description: Jumps backwards for increased mobility
	/// Type: Special
	/// Cooldown: 3 Turns
	/// KO Opponents: Active
	/// 
	/// </summary>

	// Ability information
	private int rallyRegen = 2;
	private const int RALLY_REGEN_RATE = 2;
	private const float RALLY_ANIMATION_TIME = 0.75f;
	private const string RALLY_STATUS_PROMPT = "Rally";

	#region Public Unit Override Functions

	public override void InitializeInstance ( GameManager gm, int instanceID, UnitSettingData settingData )
	{
		// Initialize the instance
		base.InitializeInstance ( gm, instanceID, settingData );

		// Set the duration of Rally
		InstanceData.Ability1.CurrentDuration = InstanceData.Ability1.Duration;
	}

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Backflip moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability2, prerequisite ) )
			GetBackflip ( t, prerequisite, returnOnlyJumps );
	}

	#endregion // Public Unit Override Functions

	#region Protected Unit Override Functions

	/// <summary>
	/// Have the unit jump an adjacent unit.
	/// This function builds the animation queue from the move data.
	/// </summary>
	protected override void Jump ( MoveData data )
	{
		// Move as normal
		base.Jump ( data );

		// Check for Rally
		if ( PassiveAvailabilityCheck ( InstanceData.Ability1, data ) )
		{
			// Get unit
			Unit u;
			if ( data.Prerequisite == null )
				u = currentTile.neighbors [ (int)data.Direction ].currentUnit;
			else
				u = data.Prerequisite.Tile.neighbors [ (int)data.Direction ].currentUnit;

			// Check unit status
			if ( u.Status.CanMove )
				ActivateRally ( u );
		}
	}

	#endregion // Protected Unit Override Functions

	#region Public HeroUnit Override Functions

	/// <summary>
	/// Decrements the cooldown for the unit's special ability.
	/// </summary>
	public override void Cooldown ( )
	{
		// Check for active ability type for ability 1
		if ( InstanceData.Ability1.IsEnabled )
		{
			// Check if current duration is active
			if ( InstanceData.Ability1.CurrentDuration < InstanceData.Ability1.Duration )
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
						InstanceData.Ability1.CurrentDuration++;

						// Reset regen
						rallyRegen = RALLY_REGEN_RATE;
					}
				}
			}
		}

		// Set cooldown for ability 2
		base.Cooldown ( );
	}

	#endregion // Public HeroUnit Override Functions

	#region Protected HeroUnit Override Functions

	/// <summary>
	/// Checks if the hero is capable of using a passive ability.
	/// Returns true if the passive ability is available.
	/// </summary>
	protected override bool PassiveAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.PassiveAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check if duration has expired
		if ( ability.CurrentDuration == 0 )
			return false;

		// Check for attack
		if ( prerequisite.Type == MoveData.MoveType.ATTACK || prerequisite.Type == MoveData.MoveType.SPECIAL_ATTACK )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if the hero is capable of using a special ability.
	/// Returns true if the special ability is available.
	/// </summary>
	protected override bool SpecialAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.SpecialAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check previous moves
		if ( CheckPrequisiteType ( prerequisite ) )
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
		// Check for attack
		if ( data.Type == MoveData.MoveType.SPECIAL_ATTACK )
		{
			// Create animation
			Tween t = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME * 2 )
				.OnComplete ( ( ) =>
				{
					// Start teleport cooldown
					StartCooldown ( InstanceData.Ability2 );

					// Set unit and tile data
					SetUnitToTile ( data.Tile );
				} );

			// Add animation to queue
			GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );

			// Attack the unit
			AttackUnit ( data );
		}
		else
		{
			// Check for normal move
			if ( data.Prerequisite == null && currentTile.neighbors [ (int)data.Direction ] == data.Tile )
			{
				// Create animation
				Tween t = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME )
					.OnComplete ( ( ) =>
					{
						// Start teleport cooldown
						StartCooldown ( InstanceData.Ability2 );

						// Set unit and tile data
						SetUnitToTile ( data.Tile );
					} );

				// Add animation to queue
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
			else
			{
				// Create animation
				Tween t = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME * 2 )
					.OnComplete ( ( ) =>
					{
						// Start teleport cooldown
						StartCooldown ( InstanceData.Ability2 );

						// Set unit and tile data
						SetUnitToTile ( data.Tile );
					} );

				// Add animation to queue
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );

				// Check for Rally
				if ( InstanceData.Ability1.IsEnabled && InstanceData.Ability1.CurrentDuration > 0 )
				{
					// Get unit
					Unit u;
					if ( data.Prerequisite == null )
						u = currentTile.neighbors [ (int)data.Direction ].currentUnit;
					else
						u = data.Prerequisite.Tile.neighbors [ (int)data.Direction ].currentUnit;

					// Check unit status
					if ( u.Status.CanMove )
						ActivateRally ( u );
				}
			}
		}
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

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
				InstanceData.Ability1.CurrentDuration--;

				// Add unit to unit queue
				GM.UnitQueue.Add ( u );

				// Apply the unit's status effect
				u.Status.AddStatusEffect ( InstanceData.Ability1.Icon, RALLY_STATUS_PROMPT, this, 1 );

				// Update HUD
				GM.UI.matchInfoMenu.GetPlayerHUD ( u ).UpdateStatusEffects ( u.InstanceID, u.Status );
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Marks every tile available to the Backflip ability.
	/// </summary>
	private void GetBackflip ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.TeamDirection );

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
				MoveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i ) );
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
					m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL_ATTACK, i, t.neighbors [ i ] );
				}
				else
				{
					// Add as an available jump
					m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i );
				}

				// Add move to the move list
				MoveList.Add ( m );

				// Find additional jumps
				FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
			}
		}
	}

	#endregion // Private Functions
}
