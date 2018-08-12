using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Catapult : HeroUnit
{
	/// <summary>
	///
	/// Hero 2 Unit Data
	/// 
	/// ID: 10
	/// Name: Hero 2
	/// Nickname: Catapult
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Offense
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 13
	/// Name: Catapult
	/// Description: Charges forward to KO any opponent in their path, becoming exhausted for a short period
	/// Type: Special
	/// Duration: 2 Rounds
	/// Cooldown: 5 Rounds
	/// 
	/// Ability 2
	/// ID: 14
	/// Name: Grapple
	/// Description: Grabs hold of a nearby opponent, leaving both immobilised for a short period
	/// Type: Command
	/// Duration: 2 Rounds
	/// Cooldown: 4 Rounds
	/// Use During Exhaustion: Enabled
	/// 
	/// </summary>

	// Ability information
	public SpriteRenderer grappleDisplayPrefab;
	private SpriteRenderer currentGrappleDisplay;
	private Unit grappleTarget;
	private const float GRAPPLE_ANIMATION_TIME = 0.25f;
	private const string CATAPULT_STATUS_PROMPT = "Exhausted";
	private const string GRAPPLE_STATUS_PROMPT = "Grapple";
	private const string GRAPPLE_TARGET_STATUS_PROMPT = "Restrained";

	#region Public Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Clear previous move list
		if ( prerequisite == null )
			MoveList.Clear ( );

		// Check status effects
		if ( Status.CanMove )
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( Owner.TeamDirection );

			// Check each neighboring tile
			for ( int i = 0; i < t.neighbors.Length; i++ )
			{
				// Ignore tiles that would allow for backward movement
				if ( i == back.FirstInt || i == back.SecondInt )
					continue;

				// Check if this unit can move to the neighboring tile
				if ( !returnOnlyJumps && OccupyTileCheck ( t.neighbors [ i ], prerequisite ) )
				{
					// Add as an available move
					MoveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i ) );
				}
				// Check if this unit can jump the neighboring tile
				else if ( base.JumpTileCheck ( t.neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the neighboring unit can be attacked
					if ( t.neighbors [ i ].CurrentUnit != null && t.neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available attack
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.ATTACK, i, t.neighbors [ i ] );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i );
					}

					// Add move to the move list
					MoveList.Add ( m );

					// Find additional jumps
					FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
				}
			}
		}

		// Get Catapult moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
			GetCatapult ( t, GetBackDirection ( Owner.TeamDirection ) );

		// Get Grapple availability
		InstanceData.Ability2.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability2, prerequisite );
	}

	#endregion // Public Unit Override Functions

	#region Protected Unit Override Functions

	/// <summary>
	/// Determines if a tile can be jumped by this unit using the Catapult ability.
	/// Returns true if the tile can be jumped.
	/// </summary>
	protected override bool JumpTileCheck ( Tile t )
	{
		// Check if the tile exists
		if ( t == null )
			return false;

		// Check if the tile has a tile object blocking it
		if ( t.CurrentObject != null && !t.CurrentObject.CanBeJumped )
			return false;

		// Return that the tile can be jumped by this unit
		return true;
	}

	#endregion // Protected Unit Override Functions

	#region Public HeroUnit Override Functions

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( AbilityInstanceData ability )
	{
		// Clear the board
		base.StartCommand ( ability );

		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( Owner.TeamDirection );

		// Get every Grapple target
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for target
			if ( currentTile.neighbors [ i ].CurrentUnit != null && currentTile.neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) )
				currentTile.neighbors [ i ].SetTileState ( TileState.AvailableCommand );
		}
	}

	/// <summary>
	/// Select the tile for Taunt.
	/// </summary>
	public override void SelectCommandTile ( Tile t )
	{
		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Board.ResetTiles ( );

		// Store target
		grappleTarget = t.CurrentUnit;

		// Interupt target
		grappleTarget.InteruptUnit ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( )
			.Append ( transform.DOMove ( Vector3.Lerp ( transform.position, grappleTarget.transform.position, 0.25f ), GRAPPLE_ANIMATION_TIME ).SetLoops ( 2, LoopType.Yoyo ) )
			.Join ( grappleTarget.transform.DOMove ( Vector3.Lerp ( transform.position, grappleTarget.transform.position, 0.75f ), GRAPPLE_ANIMATION_TIME ).SetLoops ( 2, LoopType.Yoyo ) )
			.OnComplete ( ( ) =>
			{
				// Display grapple
				currentGrappleDisplay = Instantiate ( grappleDisplayPrefab, Owner.transform );
				currentGrappleDisplay.transform.position = Vector3.Lerp ( transform.position, grappleTarget.transform.position, 0.5f );
				Color32 c = Util.TeamColor ( Owner.Team );
				currentGrappleDisplay.color = new Color32 ( c.r, c.g, c.b, 150 );
				Util.OrientSpriteToDirection ( currentGrappleDisplay, Owner.TeamDirection );

				// Start cooldown
				StartCooldown ( InstanceData.Ability2 );

				// Apply hero's status effect
				Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.GRAPPLE_HOLD, InstanceData.Ability2.Duration, this );
				//Status.AddStatusEffect ( InstanceData.Ability2.Icon, GRAPPLE_STATUS_PROMPT, this, InstanceData.Ability2.Duration, StatusEffects.StatusType.CAN_MOVE, StatusEffects.StatusType.CAN_USE_ABILITY );
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );

				// Apply target's status effect
				grappleTarget.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.GRAPPLED, InstanceData.Ability2.Duration, this );
				//grappleTarget.Status.AddStatusEffect ( InstanceData.Ability2.Icon, GRAPPLE_TARGET_STATUS_PROMPT, this, InstanceData.Ability2.Duration, StatusEffects.StatusType.CAN_MOVE, StatusEffects.StatusType.CAN_BE_MOVED, StatusEffects.StatusType.CAN_USE_ABILITY );
				GM.UI.matchInfoMenu.GetPlayerHUD ( grappleTarget ).UpdateStatusEffects ( grappleTarget.InstanceID, grappleTarget.Status );

				// Set target's KO delegate for interupts
				grappleTarget.koDelegate += EndGrappleDelegate;

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

	/// <summary>
	/// Interupts any actions that take more than one turn to complete that this unit is in the process of doing.
	/// Call this function when this unit is being attacked or being affected by some interupting ability.
	/// IMPORTANT: Be sure to call this function first before the interupting action since Interupts change the status effects of the action being interupted and the interupting action may apply new status effects.
	/// </summary>
	public override void InteruptUnit ( )
	{
		// Check if currently grappling
		if ( grappleTarget != null )
		{
			// End Grapple
			EndGrapple ( );

			// Remove the hero's status effect
			Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.GRAPPLE_HOLD, this );
			//Status.RemoveStatusEffect ( InstanceData.Ability2.Icon, GRAPPLE_STATUS_PROMPT, this, StatusEffects.StatusType.CAN_MOVE );
			GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );
		}
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
	/// Checks if there adjacent unoccupied tiles available for the Grapple Ability.
	/// Returns true if at least one enemy occupies an adjacent tile.
	/// </summary>
	protected override bool CommandAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check the use during exhaustion setting
		if ( !InstanceData.Ability2.IsPerkEnabled && Status.Effects.Exists ( x => x.ID == (int)StatusEffectDatabase.StatusEffectType.EXHAUSTION ) )
			return false;

		// Check if target is available
		if ( !GrappleCheck ( ) )
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
		Tween t = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME * 3 )
			.SetRecyclable ( )
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

				// Start special ability cooldown
				StartCooldown ( InstanceData.Ability1 );

				// Add status effect
				Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.EXHAUSTION, InstanceData.Ability1.Duration, this );
				//Status.AddStatusEffect ( InstanceData.Ability1.Icon, CATAPULT_STATUS_PROMPT, this, InstanceData.Ability1.Duration, StatusEffects.StatusType.CAN_MOVE );
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );
				GM.UI.unitHUD.UpdateStatusEffects ( );

				// Set unit and tile data
				SetUnitToTile ( data.Destination );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Callback for when the duration of an ability has expired.
	/// </summary>
	protected override void OnDurationComplete ( AbilityInstanceData ability )
	{
		// Check ability
		if ( ability == InstanceData.Ability2 && grappleTarget != null )
		{
			// End the grapple
			EndGrapple ( );
		}
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Marks all tiles that this unit could charge to.
	/// </summary>
	private void GetCatapult ( Tile t, IntPair back )
	{
		// Check each neighboring tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check if tile is available
			if ( JumpTileCheck ( t.neighbors [ i ] ) && JumpTileCheck ( t.neighbors [ i ].neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], null ) )
			{
				// Check for available attacks
				if ( ( t.neighbors [ i ].CurrentUnit != null && t.neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) ) || ( t.neighbors [ i ].neighbors [ i ].CurrentUnit != null && t.neighbors [ i ].neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) ) )
				{
					// Check if the first neighbor unit can be attacked but the second neighbor unit cannot
					if ( ( t.neighbors [ i ].CurrentUnit != null && t.neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].CurrentUnit == null || !t.neighbors [ i ].neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) ) )
					{
						// Add as an available special attack move
						MoveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], null, MoveData.MoveType.SPECIAL_ATTACK, i, t.neighbors [ i ] ) );
					}
					// Check if the second neighbor unit can be attacked but the first neighbor unit cannot
					else if ( ( t.neighbors [ i ].CurrentUnit == null || !t.neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].CurrentUnit != null && t.neighbors [ i ].neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) ) )
					{
						// Add as an available special attack move
						MoveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], null, MoveData.MoveType.SPECIAL_ATTACK, i, t.neighbors [ i ].neighbors [ i ] ) );
					}
					// Check if both neighbor units can be attacked
					else if ( ( t.neighbors [ i ].CurrentUnit != null && t.neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].CurrentUnit != null && t.neighbors [ i ].neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) ) )
					{
						// Add as an available special attack move
						MoveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], null, MoveData.MoveType.SPECIAL_ATTACK, i, t.neighbors [ i ], t.neighbors [ i ].neighbors [ i ] ) );
					}
				}
				else
				{
					// Add as an available special move
					MoveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], null, MoveData.MoveType.SPECIAL, i ) );
				}
			}
		}
	}

	/// <summary>
	/// Checks to see if an enemy unit is within range of and available to be targeted by the Grapple ability.
	/// </summary>
	private bool GrappleCheck ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( Owner.TeamDirection );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for target
			if ( currentTile.neighbors [ i ] != null && currentTile.neighbors [ i ].CurrentUnit != null && currentTile.neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) )
				return true;
		}

		// Return that no targets were found
		return false;
	}

	/// <summary>
	/// Ends the effects of the Grapple ability.
	/// This function builds the animation queue.
	/// </summary>
	private void EndGrapple ( )
	{
		// Create animations
		Tween t1 = transform.DOMove ( Vector3.Lerp ( transform.position, grappleTarget.transform.position, 0.25f ), GRAPPLE_ANIMATION_TIME )
			.SetLoops ( 2, LoopType.Yoyo )
			.OnStart ( ( ) =>
			{
				// Remove grapple display
				Destroy ( currentGrappleDisplay );
				currentGrappleDisplay = null;
			} );
		Tween t2 = grappleTarget.transform.DOMove ( Vector3.Lerp ( transform.position, grappleTarget.transform.position, 0.75f ), GRAPPLE_ANIMATION_TIME )
			.SetLoops ( 2, LoopType.Yoyo )
			.OnComplete ( ( ) =>
			{
				// Mark that grapple is no longer active
				InstanceData.Ability2.IsActive = false;

				// Remove the target's status effect
				grappleTarget.Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.GRAPPLED, this );
				//grappleTarget.Status.RemoveStatusEffect ( InstanceData.Ability2.Icon, GRAPPLE_TARGET_STATUS_PROMPT, this, StatusEffects.StatusType.CAN_MOVE, StatusEffects.StatusType.CAN_BE_MOVED, StatusEffects.StatusType.CAN_USE_ABILITY );
				GM.UI.matchInfoMenu.GetPlayerHUD ( grappleTarget ).UpdateStatusEffects ( grappleTarget.InstanceID, grappleTarget.Status );

				// Remove target's KO delegate
				grappleTarget.koDelegate -= EndGrappleDelegate;

				// Clear target
				grappleTarget = null;
			} );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
	}

	/// <summary>
	/// Ends the effects of the Grapple ability.
	/// This function builds the animation queue.
	/// Use this function as a KO delegate wrapper.
	/// </summary>
	private void EndGrappleDelegate ( Unit u )
	{
		// End Grapple
		EndGrapple ( );

		// Remove the hero's status effect
		Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.GRAPPLED, this );
		//Status.RemoveStatusEffect ( InstanceData.Ability2.Icon, GRAPPLE_STATUS_PROMPT, this, StatusEffects.StatusType.CAN_MOVE );
		GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );
	}

	#endregion // Private Functions
}
