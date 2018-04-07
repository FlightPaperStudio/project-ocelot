using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Catapult : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Catapult
	/// Type: Special Ability
	/// Default Duration: 2 Turns
	/// Default Cooldown: 5 Turns
	/// 
	/// Ability 2: Grapple
	/// Type: Command Ability
	/// Default Duration: 2 Turns
	/// Default Cooldown: 4 Turns
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

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Clear previous move list
		if ( prerequisite == null )
			moveList.Clear ( );

		// Check status effects
		if ( status.CanMove )
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( owner.TeamDirection );

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
					moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i ) );
				}
				// Check if this unit can jump the neighboring tile
				else if ( base.JumpTileCheck ( t.neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the neighboring unit can be attacked
					if ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
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
					moveList.Add ( m );

					// Find additional jumps
					FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
				}
			}
		}

		// Get Catapult moves
		if ( SpecialAvailabilityCheck ( CurrentAbility1, prerequisite ) )
			GetCatapult ( t, GetBackDirection ( owner.TeamDirection ) );

		// Get Grapple availability
		CurrentAbility2.active = CommandAvailabilityCheck ( CurrentAbility2, prerequisite );
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
				if ( ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) || ( t.neighbors [ i ].neighbors [ i ].currentUnit != null && t.neighbors [ i ].neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) )
				{
					// Check if the first neighbor unit can be attacked but the second neighbor unit cannot
					if ( ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].currentUnit == null || !t.neighbors [ i ].neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) )
					{
						// Add as an available special attack move
						moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], null, MoveData.MoveType.SPECIAL_ATTACK, i, t.neighbors [ i ] ) );
					}
					// Check if the second neighbor unit can be attacked but the first neighbor unit cannot
					else if ( ( t.neighbors [ i ].currentUnit == null || !t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].currentUnit != null && t.neighbors [ i ].neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) )
					{
						// Add as an available special attack move
						moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], null, MoveData.MoveType.SPECIAL_ATTACK, i, t.neighbors [ i ].neighbors [ i ] ) );
					}
					// Check if both neighbor units can be attacked
					else if ( ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) && ( t.neighbors [ i ].neighbors [ i ].currentUnit != null && t.neighbors [ i ].neighbors [ i ].currentUnit.UnitAttackCheck ( this ) ) )
					{
						// Add as an available special attack move
						moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], null, MoveData.MoveType.SPECIAL_ATTACK, i, t.neighbors [ i ], t.neighbors [ i ].neighbors [ i ] ) );
					}
				}
				else
				{
					// Add as an available special move
					moveList.Add ( new MoveData ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ], null, MoveData.MoveType.SPECIAL, i ) );
				}
			}
		}
	}

	/// <summary>
	/// Determines if a tile can be jumped by this unit using the Catapult ability.
	/// Returns true if the tile can be jumped.
	/// </summary>
	protected override bool JumpTileCheck ( Tile t )
	{
		// Check if the tile exists
		if ( t == null )
			return false;

		// Check fi the tile has a tile object blocking it
		if ( t.currentObject != null && !t.currentObject.canBeJumped )
			return false;

		// Return that the tile can be jumped by this unit
		return true;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Create animation
		Tween t = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME * 3 )
			.SetRecyclable ( )
			.OnComplete ( () =>
			{
				// Start special ability cooldown
				StartCooldown ( CurrentAbility1, Info.Ability1 );

				// Add status effect
				status.AddStatusEffect ( abilitySprite1, CATAPULT_STATUS_PROMPT, this, CurrentAbility1.duration, StatusEffects.StatusType.CAN_MOVE );
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( instanceID, status );
				GM.UI.unitHUD.UpdateStatusEffects ( );

				// Set unit and tile data
				SetUnitToTile ( data.Tile );
			} );

		// Add animation to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Callback for when the duration of an ability has expired.
	/// </summary>
	protected override void OnDurationComplete ( AbilitySettings current )
	{
		// Check ability
		if ( current == CurrentAbility2 && grappleTarget != null )
		{
			// End the grapple
			EndGrapple ( );
		}
	}

	/// <summary>
	/// Checks if there adjacent unoccupied tiles available for the Self-Destruct/Recall Ability.
	/// Returns true if at least one adjacent tile is unoccupied.
	/// </summary>
	protected override bool CommandAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check if target is available
		if ( !GrappleCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks to see if an enemy unit is within range of and available to be targeted by the Grapple ability.
	/// </summary>
	private bool GrappleCheck ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.TeamDirection );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for target
			if ( currentTile.neighbors [ i ] != null && currentTile.neighbors [ i ].currentUnit != null && currentTile.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
				return true;
		}

		// Return that no targets were found
		return false;
	}

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( )
	{
		// Clear the board
		base.StartCommand ( );

		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.TeamDirection );

		// Get every Taunt target
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for target
			if ( currentTile.neighbors [ i ].currentUnit != null && currentTile.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
				currentTile.neighbors [ i ].SetTileState ( TileState.AvailableCommand );
		}
	}

	/// <summary>
	/// Select the tile for Taunt.
	/// </summary>
	public override void SelectCommandTile ( Tile t )
	{
		// Pause turn timer
		if ( MatchSettings.turnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Clear board
		GM.board.ResetTiles ( );

		// Store target
		grappleTarget = t.currentUnit;

		// Interupt target
		grappleTarget.InteruptUnit ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( )
			.Append ( transform.DOMove ( Vector3.Lerp ( transform.position, grappleTarget.transform.position, 0.25f ), GRAPPLE_ANIMATION_TIME ).SetLoops ( 2, LoopType.Yoyo ) )
			.Join ( grappleTarget.transform.DOMove ( Vector3.Lerp ( transform.position, grappleTarget.transform.position, 0.75f ), GRAPPLE_ANIMATION_TIME ).SetLoops ( 2, LoopType.Yoyo ) )
			.OnComplete ( ( ) =>
			{
				// Display grapple
				currentGrappleDisplay = Instantiate ( grappleDisplayPrefab, owner.transform );
				currentGrappleDisplay.transform.position = Vector3.Lerp ( transform.position, grappleTarget.transform.position, 0.5f );
				Color32 c = Util.TeamColor ( owner.Team );
				currentGrappleDisplay.color = new Color32 ( c.r, c.g, c.b, 150 );
				Util.OrientSpriteToDirection ( currentGrappleDisplay, owner.TeamDirection );

				// Start cooldown
				StartCooldown ( CurrentAbility2, Info.Ability2 );

				// Apply hero's status effect
				status.AddStatusEffect ( abilitySprite2, GRAPPLE_STATUS_PROMPT, this, CurrentAbility2.duration, StatusEffects.StatusType.CAN_MOVE, StatusEffects.StatusType.CAN_USE_ABILITY );
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( instanceID, status );

				// Apply target's status effect
				grappleTarget.status.AddStatusEffect ( abilitySprite2, GRAPPLE_TARGET_STATUS_PROMPT, this, CurrentAbility2.duration, StatusEffects.StatusType.CAN_MOVE, StatusEffects.StatusType.CAN_BE_MOVED, StatusEffects.StatusType.CAN_USE_ABILITY );
				GM.UI.matchInfoMenu.GetPlayerHUD ( grappleTarget ).UpdateStatusEffects ( grappleTarget.instanceID, grappleTarget.status );

				// Set target's KO delegate for interupts
				grappleTarget.koDelegate += EndGrappleDelegate;

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
			status.RemoveStatusEffect ( abilitySprite2, GRAPPLE_STATUS_PROMPT, this, StatusEffects.StatusType.CAN_MOVE );
			GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( instanceID, status );
		}
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
				// Remove the target's status effect
				grappleTarget.status.RemoveStatusEffect ( abilitySprite2, GRAPPLE_TARGET_STATUS_PROMPT, this, StatusEffects.StatusType.CAN_MOVE, StatusEffects.StatusType.CAN_BE_MOVED, StatusEffects.StatusType.CAN_USE_ABILITY );
				GM.UI.matchInfoMenu.GetPlayerHUD ( grappleTarget ).UpdateStatusEffects ( grappleTarget.instanceID, grappleTarget.status );

				// Remove target's KO delegate
				grappleTarget.koDelegate -= EndGrappleDelegate;

				// Clear target
				grappleTarget = null;
			} );

		// Add animations to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
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
		status.RemoveStatusEffect ( abilitySprite2, GRAPPLE_STATUS_PROMPT, this, StatusEffects.StatusType.CAN_MOVE );
		GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( instanceID, status );
	}
}
