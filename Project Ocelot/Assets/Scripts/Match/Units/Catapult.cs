using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Catapult : HeroUnit
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
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
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	[SerializeField]
	private SpriteRenderer grappleDisplayPrefab;

	private SpriteRenderer currentGrappleDisplay;
	private Unit grappleTarget;

	private const float GRAPPLE_ANIMATION_TIME = 0.25f;
	private const string CATAPULT_STATUS_PROMPT = "Exhausted";
	private const string GRAPPLE_STATUS_PROMPT = "Grapple";
	private const string GRAPPLE_TARGET_STATUS_PROMPT = "Restrained";

	#endregion // Ability Data

	#region Public Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Catapult moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
			GetCatapult ( hex );

		// Get Grapple availability
		InstanceData.Ability2.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability2, prerequisite );
	}

	#endregion // Public Unit Override Functions

	#region Public HeroUnit Override Functions

	public override void ExecuteCommand ( )
	{
		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Grid.ResetTiles ( );

		// Store target
		grappleTarget = GM.SelectedCommand.PrimaryTarget.Tile.CurrentUnit;

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
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );

				// Apply target's status effect
				grappleTarget.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.GRAPPLED, InstanceData.Ability2.Duration + 1, this );
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
		if ( !InstanceData.Ability2.IsPerkEnabled && Status.HasStatusEffect ( StatusEffectDatabase.StatusEffectType.EXHAUSTION ) )
			return false;

		// Check if target is available
		if ( !GrappleCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	protected override void GetCommandTargets ( )
	{
		// Check each neighboring tile
		for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
			if ( GrappleTargetCheck ( CurrentHex.Neighbors [ i ] ) )
				CurrentHex.Neighbors [ i ].Tile.SetTileState ( TileState.AvailableCommand );
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
	/// <param name="hex"> The hex being moved from. </param>
	private void GetCatapult ( Hex hex )
	{
		// Check each neighboring tile
		for ( int i = 0; i < hex.Neighbors.Length; i++ )
		{
			// Check for tiles
			if ( !CatapultCheck ( hex, i ) )
				continue;

			// Store first target
			Hex target1 = hex.Neighbors [ i ];

			// Store second target
			Hex target2 = hex.Neighbors [ i ].Neighbors [ i ];

			// Store destination
			Hex destination = hex.Neighbors [ i ].Neighbors [ i ].Neighbors [ i ];

			// Check if the first tile can be jumped
			if ( !CatapultTargetCheck ( target1 ) )
				continue;

			// Check if the second tile can be jumped
			if ( !CatapultTargetCheck ( target2 ) )
				continue;

			// Check if destination is availabled
			if ( !OccupyTileCheck ( destination, null ) )
				continue;

			// Check for available attacks
			if ( ( target1.Tile.CurrentUnit != null && target1.Tile.CurrentUnit.UnitAttackCheck ( this ) ) || ( target2.Tile.CurrentUnit != null && target2.Tile.CurrentUnit.UnitAttackCheck ( this ) ) )
			{
				// Check if the first neighbor unit can be attacked but the second neighbor unit cannot
				if ( ( target1.Tile.CurrentUnit != null && target1.Tile.CurrentUnit.UnitAttackCheck ( this ) ) && ( target2.Tile.CurrentUnit == null || !target2.Tile.CurrentUnit.UnitAttackCheck ( this ) ) )
				{
					// Add as an available special attack move
					MoveList.Add ( new MoveData ( destination, null, MoveData.MoveType.SPECIAL, i, null, target1 ) );
				}
				// Check if the second neighbor unit can be attacked but the first neighbor unit cannot
				else if ( ( target1.Tile.CurrentUnit == null || !target1.Tile.CurrentUnit.UnitAttackCheck ( this ) ) && target2.Tile.CurrentUnit != null && target2.Tile.CurrentUnit.UnitAttackCheck ( this ) )
				{
					// Add as an available special attack move
					MoveList.Add ( new MoveData ( destination, null, MoveData.MoveType.SPECIAL, i, null, target2 ) );
				}
				// Check if both neighbor units can be attacked
				else if ( target1.Tile.CurrentUnit != null && target1.Tile.CurrentUnit.UnitAttackCheck ( this ) && target2.Tile.CurrentUnit != null && target2.Tile.CurrentUnit.UnitAttackCheck ( this ) ) 
				{
					// Add as an available special attack move
					MoveList.Add ( new MoveData ( destination, null, MoveData.MoveType.SPECIAL, i, null, new Hex [ ] { target1, target2 } ) );
				}
			}
			else
			{
				// Add as an available special move
				MoveList.Add ( new MoveData ( destination, null, MoveData.MoveType.SPECIAL, i ) );
			}
		}
	}

	/// <summary>
	/// Checks if all tiles for Catapult exist.
	/// </summary>
	/// <param name="hex"> The hex being moved from. </param>
	/// <param name="direction"> The direction being moved in. </param>
	/// <returns> Whether or not all tiles involved exist. </returns>
	private bool CatapultCheck ( Hex hex, int direction )
	{
		// Check for starting tile
		if ( hex == null )
			return false;

		// Check for first target
		if ( hex.Neighbor ( (Hex.Direction)direction, 1 ) == null )
			return false;

		// Check for second target
		if ( hex.Neighbor ( (Hex.Direction)direction, 2 ) == null )
			return false;

		// Check for destination
		if ( hex.Neighbor ( (Hex.Direction)direction, 3 ) == null )
			return false;

		// Return that all tiles are available
		return true;
	}

	/// <summary>
	/// Checks if a potential tile can be targeted by the Catapult ability.
	/// </summary>
	/// <param name="hex"> The target tile </param>
	/// <returns> Whether or not the tile can be targeted. </returns>
	private bool CatapultTargetCheck ( Hex hex )
	{
		// Check if the tile exists
		if ( hex == null )
			return false;

		// Check if the tile has a tile object blocking it
		if ( hex.Tile.CurrentObject != null && !hex.Tile.CurrentObject.CanAssist )
			return false;

		// Return that the tile can be jumped by this unit
		return true;
	}

	/// <summary>
	/// Checks to see if an enemy unit is within range of and available to be targeted by the Grapple ability.
	/// </summary>
	/// <returns> Whether or not a target is available for the Grapple ability. </returns>
	private bool GrappleCheck ( )
	{
		// Check each neighboring tile
		for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
			if ( GrappleTargetCheck ( CurrentHex.Neighbors [ i ] ) )
				return true;

		// Return that no targets were found
		return false;
	}

	/// <summary>
	/// Checks an individual tile if an enemy unit can be targeted by the Grapple ability.
	/// </summary>
	/// <param name="hex"> The tile being checked. </param>
	/// <returns> Whether or not the tile has a target. </returns>
	private bool GrappleTargetCheck ( Hex hex )
	{
		// Check for tile
		if ( hex == null )
			return false;

		// Check for unit
		if ( hex.Tile.CurrentUnit == null )
			return false;

		// Check if unit is on the same team
		if ( hex.Tile.CurrentUnit.Owner == Owner )
			return false;

		// Check if unit can be affected by abilities
		if ( !hex.Tile.CurrentUnit.Status.CanBeAffectedByAbility )
			return false;

		// Check if unit can be affected physically
		if ( !hex.Tile.CurrentUnit.Status.CanBeAffectedPhysically )
			return false;

		// Return that a target is available
		return true;
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
	/// <param name="unit"> The unit that was being grappled. </param>
	private void EndGrappleDelegate ( Unit unit )
	{
		// End Grapple
		EndGrapple ( );

		// Remove the hero's status effect
		Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.GRAPPLED, this );
		GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );
	}

	#endregion // Private Functions
}
