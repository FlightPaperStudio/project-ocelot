using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rally : HeroUnit
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
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
	/// Regeneration: 1 Use Per 2 Rounds
	/// 
	/// Ability 2
	/// ID: 20
	/// Name: Dual Assist
	/// Description: Moves diagonally by two assisting allies.
	/// Type: Special
	/// Cooldown: 5 Rounds
	/// Opponent Assist: Active
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	private int rallyRegen = 0;

	private const float RALLY_ANIMATION_TIME = 0.75f;

	#endregion // Ability Data

	#region Public Unit Override Functions

	public override void InitializeInstance ( GameManager gm, int instanceID, UnitSettingData settingData )
	{
		// Initialize the instance
		base.InitializeInstance ( gm, instanceID, settingData );

		// Set the duration of Rally
		InstanceData.Ability1.CurrentDuration = InstanceData.Ability1.Duration;

		// Set the regeneration rate of Rally
		rallyRegen = InstanceData.Ability1.PerkValue;
	}

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Backflip moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability2, prerequisite ) )
			GetDualAssist ( hex, prerequisite );
	}

	#endregion // Public Unit Override Functions

	#region Protected Unit Override Functions

	protected override void GetAssisted ( MoveData data )
	{
		// Check for Rally
		if ( PassiveAvailabilityCheck ( InstanceData.Ability1, data ) )
		{
			// Check for Dual Assist
			if ( data.Type == MoveData.MoveType.SPECIAL )
			{
				// Track if Rally has been used
				bool usedRally = false;

				// Check each target
				for ( int i = 0; i < data.AttackTargets.Length; i++ )
				{
					// Check for unit
					if ( data.AttackTargets [ i ].Tile.CurrentUnit == null )
						continue;

					// Get assisted unit
					Unit unit = data.AttackTargets [ i ].Tile.CurrentUnit;

					// Assist unit
					unit.Assist ( );

					// Check status
					if ( unit.Owner == Owner && unit.Status.CanMove && unit.Status.CanBeAffectedByAbility )
					{
						// Rally unit
						ActivateRally ( unit, !usedRally );

						// Mark that rally has been used
						usedRally = true;
					}
				}
			}
			else
			{
				// Get assisted unit
				Unit unit = data.AttackTargets [ 0 ].Tile.CurrentUnit;

				// Assist unit
				unit.Assist ( );

				// Check status
				if ( unit.Status.CanMove && unit.Status.CanBeAffectedByAbility )
					ActivateRally ( unit );
			}
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
						rallyRegen = InstanceData.Ability1.PerkValue;
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
		if ( prerequisite.IsAttack )
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
		if ( PriorMoveTypeCheck ( prerequisite ) )
			return false;

		// Check for destination
		if ( !DualAssistCheck ( prerequisite == null ? CurrentHex : prerequisite.Destination, prerequisite ) )
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
		Tween t = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME * 2 )
			.OnStart ( ( ) =>
			{
				// Mark that the ability is active
				InstanceData.Ability2.IsActive = true;
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability2 );
			} )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability2.IsActive = false;

				// Start teleport cooldown
				StartCooldown ( InstanceData.Ability2 );

				// Set unit and tile data
				SetUnitToTile ( data.Destination );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Activates the Rally ability. This adds a unit to the unit queue.
	/// This function builds the animation queue.
	/// </summary>
	/// <param name="unit"> The assisting ally. </param>
	private void ActivateRally ( Unit unit, bool consumeDuration = true )
	{
		// Create animation
		Tween t = unit.sprite.DOColor ( Color.white, RALLY_ANIMATION_TIME )
			.SetLoops ( 2, LoopType.Yoyo )
			.OnStart ( ( ) =>
			{
				// Mark that the ability is active
				InstanceData.Ability1.IsActive = true;
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
			} )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability1.IsActive = false;

				// Decrease Rally duration
				if ( consumeDuration )
					InstanceData.Ability1.CurrentDuration--;

				// Add unit to unit queue
				GM.UnitQueue.Add ( unit );

				// Apply the unit's status effect
				unit.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.RALLIED, 1, this );

				// Update HUD
				GM.UI.matchInfoMenu.GetPlayerHUD ( unit ).UpdateStatusEffects ( unit.InstanceID, unit.Status );
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Checks if a destination for the Dual Assist ability is available.
	/// </summary>
	/// <param name="hex"> The tile being moved from. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach this tile. </param>
	/// <returns> Whether or not a destination for the Dual Assist ability is available. </returns>
	private bool DualAssistCheck ( Hex hex, MoveData prerequisite )
	{
		// Check each neighboring tile
		for ( int i = 0; i < hex.Neighbors.Length; i++ )
		{
			// Check each direction
			if ( DualAssistDirectionCheck ( hex, prerequisite, i ) )
				return true;
		}

		// Return that are no units in position
		return false;
	}

	/// <summary>
	/// Marks every tile available for the Dual Assist ability.
	/// </summary>
	/// <param name="hex"> The tile being moved from. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach this tile. </param>
	private void GetDualAssist ( Hex hex, MoveData prerequisite )
	{
		// Check each neighboring tile
		for ( int i = 0; i < hex.Neighbors.Length; i++ )
		{
			// Check for destination
			if ( DualAssistDirectionCheck ( hex, prerequisite, i ) )
			{
				// Track move data
				MoveData move = new MoveData ( hex.Diagonals [ i ], prerequisite, MoveData.MoveType.SPECIAL, i, new Hex [ ] { hex.Neighbors [ i ], hex.Neighbors [ ( i + 1 ) % hex.Neighbors.Length ] }, null );

				// Add move to move list
				MoveList.Add ( move );

				// Find additional moves
				FindMoves ( hex.Diagonals [ i ], move, true );
			}
		}
	}

	/// <summary>
	/// Check if a destination for the Dual Assist ability is available in particular direction.
	/// </summary>
	/// <param name="hex"> The tile being moved from. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach this tile. </param>
	/// <param name="direction"> The direction toward the diagnol hex. The diagnol is a 1/6th clockwise turn from the direciton. </param>
	/// <returns></returns>
	private bool DualAssistDirectionCheck ( Hex hex, MoveData prerequisite, int direction )
	{
		// Calculate secondary direction for dual assist
		int secondaryDirection = ( direction + 1 ) % hex.Neighbors.Length;

		// Check for tile
		if ( hex.Neighbors [ direction ] == null || hex.Neighbors [ secondaryDirection ] == null )
			return false;

		// Check if tile is occupied
		if ( !hex.Neighbors [ direction ].Tile.IsOccupied || !hex.Neighbors [ secondaryDirection ].Tile.IsOccupied )
			return false;

		// Check for first unit
		if ( hex.Neighbors [ direction ].Tile.CurrentUnit != null )
		{
			// Check perk
			if ( !InstanceData.Ability2.IsPerkEnabled && hex.Neighbors [ direction ].Tile.CurrentUnit.Owner != Owner )
				return false;

			// Check if unit can assist
			if ( !hex.Neighbors [ direction ].Tile.CurrentUnit.Status.CanAssist )
				return false;
		}

		// Check for first object
		if ( hex.Neighbors [ direction ].Tile.CurrentObject != null )
		{
			// Check perk
			if ( !InstanceData.Ability2.IsPerkEnabled && hex.Neighbors [ direction ].Tile.CurrentObject.Caster.Owner != Owner )
				return false;

			// Check if object can be jumped
			if ( !hex.Neighbors [ direction ].Tile.CurrentObject.CanAssist )
				return false;
		}

		// Check for second unit
		if ( hex.Neighbors [ secondaryDirection ].Tile.CurrentUnit != null )
		{
			// Check perk
			if ( !InstanceData.Ability2.IsPerkEnabled && hex.Neighbors [ secondaryDirection ].Tile.CurrentUnit.Owner != Owner )
				return false;

			// Check if unit can assist
			if ( !hex.Neighbors [ secondaryDirection ].Tile.CurrentUnit.Status.CanAssist )
				return false;
		}

		// Check for second object
		if ( hex.Neighbors [ secondaryDirection ].Tile.CurrentObject != null )
		{
			// Check perk
			if ( !InstanceData.Ability2.IsPerkEnabled && hex.Neighbors [ secondaryDirection ].Tile.CurrentObject.Caster.Owner != Owner )
				return false;

			// Check if object can be jumped
			if ( !hex.Neighbors [ secondaryDirection ].Tile.CurrentObject.CanAssist )
				return false;
		}

		// Check for destination
		if ( hex.Diagonals [ direction ] == null )
			return false;

		// Check if destination can be occupied
		if ( !OccupyTileCheck ( hex.Diagonals [ direction ], prerequisite ) )
			return false;

		// Return that dual assit is available in this direction
		return true;
	}

	#endregion // Private Functions
}
