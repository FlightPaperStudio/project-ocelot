using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrimReaper : HeroUnit
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
	///
	/// Hero 4 Unit Data
	/// 
	/// ID: 12
	/// Name: Hero 4
	/// Nickname: Grimm
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Defense
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 17
	/// Name: Life Drain
	/// Description: Attacking an opponent provides renewed energy and additional protection for a brief period
	/// Type: Passive
	/// Duration: 1 Turn
	/// Refresh Abilities: Active
	/// 
	/// Ability 2
	/// ID: 18
	/// Name: Reaper
	/// Description: Instantly appear at the location of any KO'd ally
	/// Type: Special
	/// Cooldown: 5 Turns
	/// Target Opponents: Active
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	public SpriteRenderer barrier;
	public List<Hex> grimReaperTiles = new List<Hex> ( );

	private const float LIFE_DRAIN_FADE = 200f / 255f;
	private const float LIFE_DRAIN_ANIMATION_TIME = 0.75f;

	#endregion // Ability Data

	#region Public Unit Override Functions

	public override void InitializeInstance ( GameManager gm, int instanceID, UnitSettingData settingData )
	{
		// Initialize instance
		base.InitializeInstance ( gm, instanceID, settingData );

		// Add Reaper as a global KO delegate
		if ( InstanceData.Ability2.IsEnabled )
			GM.GlobalKOdelegate += AddGrimReaperTile;
	}

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Grim Reaper moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability2, prerequisite ) )
			GetGrimReaper ( );
	}

	/// <summary>
	/// Attack this unit and remove this unit's barrier if it's available or KO this unit if it's not.
	/// Call this function on the unit being attack.
	/// </summary>
	public override void GetAttacked ( bool usePostAnimationQueue = false )
	{
		// Check for barrier
		if ( !usePostAnimationQueue && InstanceData.Ability1.IsEnabled && InstanceData.Ability1.CurrentDuration > 0 )
		{
			// Remove barrier
			DeactivateLifeDrain ( );

			// Remove status effect
			Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.OVERPOWERED, this );
			GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );
		}
		else
		{
			// Call KO delegate
			if ( koDelegate != null )
				koDelegate ( this );

			// Create animation
			Tween t1 = transform.DOScale ( new Vector3 ( 5, 5, 5 ), KO_ANIMATION_TIME )
				.OnStart ( ( ) =>
				{
					// Remove delegates
					if ( InstanceData.Ability2.IsEnabled )
						foreach ( Player player in GM.Players)
							foreach ( Unit unit in player.UnitInstances )
								if ( unit != null )
									unit.koDelegate -= AddGrimReaperTile;

					// Hide barrier
					barrier.gameObject.SetActive ( false );
				} )
				.OnComplete ( ( ) =>
				{
					// Display deactivation
					GM.UI.matchInfoMenu.GetPlayerHUD ( this ).DisplayKO ( InstanceID );

					// Remove unit from the team
					Owner.UnitInstances.Remove ( this );

					// Remove unit reference from the tile
					CurrentHex.Tile.CurrentUnit = null;

					// Delete the unit
					Destroy ( this.gameObject );
				} )
				.Pause ( );
			Tween t2 = sprite.DOFade ( 0, MOVE_ANIMATION_TIME )
				.Pause ( );

			// Add animations to queue
			if ( usePostAnimationQueue )
			{
				GM.PostAnimationQueue.Add ( new GameManager.PostTurnAnimation ( this, Owner, new GameManager.TurnAnimation ( t1, false ), new GameManager.TurnAnimation ( t2, false ) ) );
			}
			else
			{
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
			}
		}
	}

	#endregion // Public Unit Override Functions

	#region Protected Unit Override Functions

	/// <summary>
	/// Attacks the adjacent unit.
	/// Call this function on the attacking unit.
	/// This function builds the animation queue from the move data.
	/// </summary>
	protected override void AttackUnit ( MoveData data )
	{
		// Attack the unit
		base.AttackUnit ( data );

		// Check for Life Drain
		if ( PassiveAvailabilityCheck ( InstanceData.Ability1, data ) )
			ActivateLifeDrain ( );
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
			if ( InstanceData.Ability1.CurrentDuration > 0 )
			{
				// Decrement duration
				InstanceData.Ability1.CurrentDuration--;

				// Check if duration is complete
				if ( InstanceData.Ability1.CurrentDuration == 0 )
					OnDurationComplete ( InstanceData.Ability1 );
			}
		}

		// Set cooldown for ability 2
		base.Cooldown ( );
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
			.OnStart ( ( ) =>
			{
				// Mark that the ability is active
				InstanceData.Ability2.IsActive = true;
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability2 );
			} )
			.OnComplete ( ( ) =>
			{
				// Move unit instantly
				transform.position = data.Destination.transform.position;
			} );
		Tween t2 = barrier.DOFade ( 0, MOVE_ANIMATION_TIME );
		Tween t3 = sprite.DOFade ( 1, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability2.IsActive = false;

				// Start teleport cooldown
				StartCooldown ( InstanceData.Ability2 );

				// Set unit and tile data
				SetUnitToTile ( data.Destination );
			} );
		Tween t4 = barrier.DOFade ( LIFE_DRAIN_FADE, MOVE_ANIMATION_TIME );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t3, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t4, false ) );
	}

	/// <summary>
	/// Callback for when the duration of an ability has expired.
	/// </summary>
	protected override void OnDurationComplete ( AbilityInstanceData ability )
	{
		// Deactivate Life Drain
		DeactivateLifeDrain ( );
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Adds the location of a unit to the list of Grim Reaper tiles when the unit gets KO'd.
	/// </summary>
	/// <param name="unit"> The KO'd unit. </param>
	private void AddGrimReaperTile ( Unit unit )
	{
		// Add tile
		if ( InstanceData.Ability2.IsPerkEnabled || unit.Owner == Owner )
			grimReaperTiles.Add ( unit.CurrentHex );
	}

	/// <summary>
	/// Marks every tile where an ally unit was KO for the Grim Reaper ability.
	/// </summary>
	private void GetGrimReaper ( )
	{
		// Check each KO location
		foreach ( Hex hex in grimReaperTiles )
		{
			// Check if tile is unoccupied
			if ( OccupyTileCheck ( hex, null ) )
			{
				// Create move
				MoveData move = new MoveData ( hex, null, MoveData.MoveType.SPECIAL, MoveData.MoveDirection.DIRECT );

				// Add as an available special move
				MoveList.Add ( move );

				// Continue movement
				FindMoves ( hex, move, false );
			}
		}
	}

	/// <summary>
	/// Activates the Life Drain ability. This refreshes all abilities and adds a protective barrier.
	/// This function builds the animation queue.
	/// </summary>
	private void ActivateLifeDrain ( )
	{
		// Create animation
		Tween t = barrier.DOFade ( LIFE_DRAIN_FADE, LIFE_DRAIN_ANIMATION_TIME )
			.OnStart ( ( ) =>
			{
				// Mark that the ability is active
				InstanceData.Ability1.IsActive = true;
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );

				// Display barrier
				barrier.gameObject.SetActive ( true );
				barrier.color = new Color32 ( 255, 255, 255, 0 );
			} )
			.OnComplete ( ( ) =>
			{
				// Refresh abilities
				InstanceData.Ability1.CurrentDuration = InstanceData.Ability1.Duration;
				if ( InstanceData.Ability1.IsPerkEnabled && InstanceData.Ability2.IsEnabled )
					InstanceData.Ability2.CurrentCooldown = 0;

				// Update HUD
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
				if ( InstanceData.Ability1.IsPerkEnabled && InstanceData.Ability2.IsEnabled )
					GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability2 );

				// Apply status effect
				Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.OVERPOWERED, InstanceData.Ability1.Duration, this );
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );
				GM.UI.unitHUD.UpdateStatusEffects ( );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Deactivates the Life Drain ability. This removes the protective barrier.
	/// This function builds the animation queue.
	/// </summary>
	private void DeactivateLifeDrain ( )
	{
		// Create animation
		Tween t = barrier.DOFade ( 0, LIFE_DRAIN_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability1.IsActive = false;

				// Hide barrier
				barrier.gameObject.SetActive ( false );

				// End ability duration
				InstanceData.Ability1.CurrentDuration = 0;
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	#endregion // Private Functions
}
