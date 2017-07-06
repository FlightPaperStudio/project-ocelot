using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrimReaper : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Life Drain
	/// Type: Passive Ability
	/// Default Duration: 1 Turn
	/// 
	/// Ability 2: Grim Reaper
	/// Type: Special Ability
	/// Default Cooldown: 5 Turns
	/// 
	/// </summary>

	// Ability information
	public SpriteRenderer barrier;
	public List<Tile> grimReaperTiles = new List<Tile> ( );
	private const float LIFE_DRAIN_FADE = 200f / 255f;
	private const float LIFE_DRAIN_ANIMATION_TIME = 0.75f;
	private const string LIFE_DRAIN_STATUS_PROMPT = "Life Drain";

	/// <summary>
	/// Sets up both abilities at the start of a match.
	/// </summary>
	protected override void Start ( )
	{
		// Set up hero
		base.Start ( );

		// Set Life Drain
		if ( currentAbility1.enabled )
		{
			currentAbility1.duration = 0;
			currentAbility1.active = true;
		}

		// Set Grim Reaper
		if ( currentAbility2.enabled )
			foreach ( Unit u in owner.units )
				if ( u != this )
					u.koDelegate += AddGrimReaperTile;
	}

	/// <summary>
	/// Adds the location of a unit to the list of Grim Reaper tiles when the unit gets KO'd.
	/// </summary>
	private void AddGrimReaperTile ( Unit u )
	{
		// Add tile
		grimReaperTiles.Add ( u.currentTile );
	}

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Grim Reaper moves
		if ( SpecialAvailabilityCheck ( currentAbility2, prerequisite ) )
			GetGrimReaper ( );
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
	/// Marks every tile where an ally unit was KO for the Grim Reaper ability.
	/// </summary>
	private void GetGrimReaper ( )
	{
		// Check each KO location
		foreach ( Tile t in grimReaperTiles )
		{
			// Check if tile is unoccupied
			if ( OccupyTileCheck ( t, null ) )
			{
				// Create move
				MoveData m = new MoveData ( t, null, MoveData.MoveType.Special, 0 );

				// Add as an available special move
				moveList.Add ( m );

				// Continue movement
				FindMoves ( t, m, false );
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
		Tween t2 = barrier.DOFade ( 0, MOVE_ANIMATION_TIME );
		Tween t3 = sprite.DOFade ( 1, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Start teleport cooldown
				StartCooldown ( currentAbility2, info.ability2 );

				// Set unit and tile data
				SetUnitToTile ( data.tile );
			} );
		Tween t4 = barrier.DOFade ( LIFE_DRAIN_FADE, MOVE_ANIMATION_TIME );

		// Add animations to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t3, true ) );
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t4, false ) );
	}

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
		if ( currentAbility1.enabled )
			ActivateLifeDrain ( );
	}

	/// <summary>
	/// Activates the Life Drain ability. This refreshes all abilities and adds a protective barrier.
	/// This function builds the animation queue.
	/// </summary>
	private void ActivateLifeDrain ( )
	{
		// Display barrier
		barrier.gameObject.SetActive ( true );
		barrier.color = new Color32 ( 255, 255, 255, 0 );

		// Create animation
		Tween t = barrier.DOFade ( LIFE_DRAIN_FADE, LIFE_DRAIN_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Refresh abilities
				currentAbility1.duration = info.ability1.duration;
				if ( currentAbility2.enabled )
						currentAbility2.cooldown = 0;

				// Update HUD
				GM.UI.unitHUD.DisplayAbility ( currentAbility1 );
				if ( currentAbility2.enabled )
					GM.UI.unitHUD.DisplayAbility ( currentAbility2 );

				// Apply status effect
				status.AddStatusEffect ( abilitySprite1, LIFE_DRAIN_STATUS_PROMPT, this, currentAbility1.duration );
				GM.UI.unitHUD.UpdateStatusEffects ( );
			} );

		// Add animation to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Attack this unit and remove this unit's barrier if it's available or KO this unit if it's not.
	/// Call this function on the unit being attack.
	/// </summary>
	public override void GetAttacked ( bool usePostAnimationQueue = false )
	{
		// Check for barrier
		if ( !usePostAnimationQueue && currentAbility1.enabled && currentAbility1.duration > 0 )
		{
			// Remove barrier
			DeactivateLifeDrain ( );

			// Remove status effect
			status.RemoveStatusEffect ( abilitySprite1, LIFE_DRAIN_STATUS_PROMPT, this );
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
					if ( currentAbility2.enabled )
						foreach ( Unit u in owner.units )
							if ( u != this )
								u.koDelegate -= AddGrimReaperTile;

					// Hide barrier
					barrier.gameObject.SetActive ( false );
				} )
				.OnComplete ( ( ) =>
				{
				// Display deactivation
				GM.UI.GetPlayerHUD ( this ).DisplayDeactivation ( instanceID );

				// Remove unit from the team
				owner.units.Remove ( this );

				// Remove unit reference from the tile
				currentTile.currentUnit = null;

				// Delete the unit
				Destroy ( this.gameObject );
				} )
				.Pause ( );
			Tween t2 = sprite.DOFade ( 0, MOVE_ANIMATION_TIME )
				.Pause ( );

			// Add animations to queue
			if ( usePostAnimationQueue )
			{
				GM.postAnimationQueue.Add ( new GameManager.PostTurnAnimation ( this, owner, new GameManager.TurnAnimation ( t1, false ), new GameManager.TurnAnimation ( t2, false ) ) );
			}
			else
			{
				GM.animationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
				GM.animationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
			}
		}
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
				// Hide barrier
				barrier.gameObject.SetActive ( false );

				// End ability duration
				currentAbility1.duration = 0;
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
			if ( currentAbility1.duration > 0 )
			{
				// Decrement duration
				currentAbility1.duration--;

				// Check if duration is complete
				if ( currentAbility1.duration == 0 )
					OnDurationComplete ( currentAbility1 );
			}
		}

		// Set cooldown for ability 2
		base.Cooldown ( );
	}

	/// <summary>
	/// Callback for when the duration of an ability has expired.
	/// </summary>
	protected override void OnDurationComplete ( AbilitySettings current )
	{
		// Deactivate Life Drain
		DeactivateLifeDrain ( );
	}
}
