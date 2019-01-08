using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MindControl : HeroUnit
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
	///
	/// Hero 3 Unit Data
	/// 
	/// ID: 11
	/// Name: Hero 3
	/// Nickname: Mind Control
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Offense
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 15
	/// Name: Mind Control
	/// Description: Attacked opponents can be controlled for a short period before they are eventually KO'd
	/// Type: Passive
	/// Duration: 3 Rounds
	/// Refresh Mind Control: Active
	/// 
	/// Ability 2
	/// ID: 16
	/// Name: Clone
	/// Description: Creates a temporary clone of itself for an assist
	/// Type: Special
	/// Duration: 1 Round
	/// Cooldown: 4 Rounds
	/// Continue Movement: Active
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data
	
	[SerializeField]
	private TileObject clonePrefab;

	private List<Unit> mindControlledUnits = new List<Unit> ( );
	private TileObject currentClone;

	private const float MIND_CONTROL_ANIMATION_TIME = 0.75f;
	private const string MIND_CONTROL_STATUS_PROMPT = "Mind Control";

	#endregion // Ability Data

	#region Public Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Clone Assist moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability2, prerequisite ) )
			GetCloneAssist ( hex, prerequisite );
	}

	/// <summary>
	/// Attack and KO this unit.
	/// Call this function on the unit being attacked.
	/// This function builds the animation queue from the move data.
	/// </summary>
	public override void GetAttacked ( bool usePostAnimationQueue = false )
	{
		// Check if post-animation queue is being used to prevent animation queue sync issues and check if the Mind Control ability is active
		if ( !usePostAnimationQueue && InstanceData.Ability1.IsEnabled )
		{
			// Remove all Mind Controlled units
			DeactivateAllMindControls ( );
		}

		// Get KO'd
		base.GetAttacked ( usePostAnimationQueue );
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
		// Check Mind Control conditions
		if ( PassiveAvailabilityCheck ( InstanceData.Ability1, data ) )
		{
			// Mind Control the opponent
			foreach ( Hex hex in data.AttackTargets )
			{
				// Interupt unit
				hex.Tile.CurrentUnit.InteruptUnit ( );

				// Activate Mind Control
				ActivateMindControl ( hex.Tile.CurrentUnit );
			}
		}
		else
		{
			// Attack the unit as normal
			base.AttackUnit ( data );
		}
	}

	protected override void GetAssisted ( MoveData data )
	{
		// Get assisted by unit
		base.GetAssisted ( data );

		// Check each target
		foreach ( Hex hex in data.AssistTargets )
		{
			// Check for perk and mind controlled unit
			if ( InstanceData.Ability1.IsPerkEnabled && mindControlledUnits.Contains ( hex.Tile.CurrentUnit ) )
			{
				// Refresh status effect
				hex.Tile.CurrentUnit.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.MIND_CONTROLLED, InstanceData.Ability1.Duration, this );
			}
		}
	}

	#endregion // Protected Unit Override Functions

	#region Public HeroUnit Override Functions

	public override void Cooldown ( )
	{
		// Set cooldowns
		base.Cooldown ( );

		// Check if mind control is enabled
		if ( InstanceData.Ability1.IsEnabled )
		{
			// Get expired mind controls
			List<Unit> expiredUnits = mindControlledUnits.FindAll ( x => !x.Status.HasStatusEffect ( StatusEffectDatabase.StatusEffectType.MIND_CONTROLLED ) );

			// KO expired mind controls
			for ( int i = 0; i < expiredUnits.Count; i++ )
			{
				// Remove mind control
				DeactivateMindControl ( expiredUnits [ i ] );

				// Remove unit from the list of Mind Controlled units
				RemoveMindControlledUnit ( expiredUnits [ i ] );

				// Get original owner
				Player player = GetOriginalOwner ( expiredUnits [ i ] );

				// Check owner
				if ( player.IsEliminated )
					expiredUnits [ i ].UnitKO ( );
				else if ( player != Owner )
					expiredUnits [ i ].GetAttacked ( );
			}
		}
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

		// Check ability status effect
		if ( !Status.CanUseAbility )
			return false;

		// Check the opponent
		foreach ( Hex hex in prerequisite.AttackTargets )
		{
			// Check for unit
			if ( hex.Tile.CurrentUnit == null )
				return false;

			// Check for Leader
			if ( hex.Tile.CurrentUnit is Leader )
				return false;

			// Check status
			if ( !hex.Tile.CurrentUnit.Status.CanBeAffectedPhysically )
				return false;
		}

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

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Check for existing clone
		if ( currentClone != null )
			RemoveClone ( );

		// Get clone destination
		Hex priorDestination = data.PriorMove != null ? data.PriorMove.Destination : CurrentHex;
		Hex destination = priorDestination.Neighbors [ (int)data.Direction ];

		// Create clone
		currentClone = CreateTileOject ( clonePrefab, destination, InstanceData.Ability2.Duration, RemoveClone );

		// Set clone appearance
		Color32 c = Util.TeamColor ( Owner.Team );
		currentClone.Icon.color = new Color32 ( c.r, c.g, c.b, 150 );
		currentClone.gameObject.SetActive ( false );

		// Create animations
		Tween t1 = currentClone.transform.DOMove ( priorDestination.transform.position, MOVE_ANIMATION_TIME )
			.From ( )
			.OnStart ( ( ) =>
			{
				// Display clone
				currentClone.gameObject.SetActive ( true );
			} );
		Tween t2 = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME * 2 )
			.OnComplete ( ( ) =>
			{
				// Start teleport cooldown
				StartCooldown ( InstanceData.Ability2 );

				// Set unit and tile data
				SetUnitToTile ( data.Destination );
			} );
		

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, true ) );
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Marks every tile available to the Clone Assist ability.
	/// </summary>
	/// <param name="hex"> The hex being moved from. </param>
	/// <param name="prerequisite"> The moves required to make this move. </param>
	private void GetCloneAssist ( Hex hex, MoveData prerequisite )
	{
		// Check each neighboring tile
		for ( int i = 0; i < hex.Neighbors.Length; i++ )
		{
			// Check if this unit can jump the unoccupied neighboring tile
			if ( OccupyTileCheck ( hex.Neighbors [ i ], prerequisite ) && OccupyTileCheck ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite ) )
			{
				// Add as an available jump
				MoveData move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i, hex.Neighbors [ i ], null );

				// Add move to the move list
				MoveList.Add ( move );

				// Find additional jumps
				if ( InstanceData.Ability2.IsPerkEnabled )
					FindMoves ( hex.Neighbors [ i ].Neighbors [ i ], move, true );
			}
		}
	}

	/// <summary>
	/// Activates the Mind Control ability. This adds an enemy unit to the player's team.
	/// This function builds the animation queue.
	/// </summary>
	/// <param name="unit"> The unit being targeted. </param>
	private void ActivateMindControl ( Unit unit )
	{
		// Create animation
		Tween t = unit.sprite.DOColor ( Util.TeamColor ( Owner.Team ), MIND_CONTROL_ANIMATION_TIME )
			.OnStart ( ( ) =>
			{
				// Check for pre-existing Mind Control
				if ( unit.Status.Effects.Exists ( x => x.ID == (int)StatusEffectDatabase.StatusEffectType.MIND_CONTROLLED ) )
				{
					// Remove pre-existing Mind Control
					MindControl original = unit.Status.Effects.Find ( x => x.ID == (int)StatusEffectDatabase.StatusEffectType.MIND_CONTROLLED ).Caster as MindControl; 
					original.RemoveMindControlledUnit ( unit );
				}

				// Check for unit being Mind Controlled is Hero 3
				if ( unit is MindControl )
				{
					// Remove all Mind Controlled units by this hero
					MindControl hero3 = unit as MindControl;
					hero3.DeactivateAllMindControls ( );
				}

				// Mark that the ability is active
				InstanceData.Ability1.IsActive = true;
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
			} )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability1.IsActive = false;
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );

				// Apply status effect
				unit.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.MIND_CONTROLLED, InstanceData.Ability1.Duration, this );

				// Store unit
				mindControlledUnits.Add ( unit );

				// Update HUD
				GM.UI.matchInfoMenu.GetPlayerHUD ( unit ).UpdateStatusEffects ( unit.InstanceID, unit.Status );

				// Switch teams
				UnitTeamSwitch ( unit, Owner );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Deactivates the Mind Control ability. This removes the targeted unit from the player's team.
	/// This function builds the animation queue.
	/// </summary>
	/// <param name="unit"> The unit being targeted. </param>
	private void DeactivateMindControl ( Unit unit, bool usePostAnimationQueue = false )
	{
		// Get the original owner
		Player player = GetOriginalOwner ( unit );

		// Create animation
		Tween t = unit.sprite.DOColor ( Util.TeamColor ( player.Team ), MIND_CONTROL_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Apply status effect
				unit.Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.MIND_CONTROLLED, this );

				// Update HUD
				GM.UI.matchInfoMenu.GetPlayerHUD ( unit ).UpdateStatusEffects ( unit.InstanceID, unit.Status );

				// Switch teams
				UnitTeamSwitch ( unit, player );
			} );

		// Add animation to queue
		if ( usePostAnimationQueue )
			GM.PostAnimationQueue.Add ( new GameManager.PostTurnAnimation ( unit, player, new GameManager.TurnAnimation ( t, true ) ) );
		else
			GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Removes and KO's all currently Mind Controlled units.
	/// </summary>
	private void DeactivateAllMindControls ( )
	{
		// Get each unit
		foreach ( Unit unit in mindControlledUnits )
		{
			// Remove mind control
			DeactivateMindControl ( unit );

			// Remove KO delegate
			unit.koDelegate -= MindControlKO;

			// Get original owner
			Player player = GetOriginalOwner ( unit );

			// Check owner
			if ( player.IsEliminated )
				unit.UnitKO ( );
			else if ( player != Owner )
				unit.GetAttacked ( );

			// KO unit
			unit.GetAttacked ( true );
		}

		// Clear list of Mind Controlled units
		mindControlledUnits.Clear ( );
	}

	/// <summary>
	/// Returns the Mind Controlled unit back to its original owner.
	/// This function builds the animation queue.
	/// </summary>
	/// <param name="unit"> The unit being KO'd. </param>
	private void MindControlKO ( Unit unit )
	{
		// Deactivate mind control
		DeactivateMindControl ( unit );

		// Remove unit from the list of Mind Controlled units
		RemoveMindControlledUnit ( unit );
	}

	/// <summary>
	/// Removes a unit from the list of currently Mind Controlled units by this hero.
	/// </summary>
	/// <param name="unit"> The unit being removed. </param>
	private void RemoveMindControlledUnit ( Unit unit )
	{
		// Remove unit from list of Mind Controlled units
		mindControlledUnits.RemoveAll ( match => match == unit );

		// Remove KO delegate
		unit.koDelegate -= MindControlKO;
	}

	/// <summary>
	/// Returns the original owner of the unit before any mind control.
	/// </summary>
	/// <param name="unit"> The unit being searched for. </param>
	/// <returns> The original owner of the unit. </returns>
	private Player GetOriginalOwner ( Unit unit )
	{
		// Check each player
		foreach ( Player player in GM.Players )
			foreach ( int id in player.StarterInstanceIDs )
				if ( id == unit.InstanceID )
					return player;

		// Return that a player could not be found
		return null;
	}

	/// <summary>
	/// Switches a unit from one team to another.
	/// </summary>
	/// <param name="unit"> The unit switching teams. </param>
	/// <param name="player"> The new team the unit is switching to. </param>
	private void UnitTeamSwitch ( Unit unit, Player player )
	{
		// Remove unit from unit's current team
		unit.Owner.UnitInstances.Remove ( unit );
		if ( unit.Owner.StandardKOdelegate != null )
			unit.koDelegate -= unit.Owner.StandardKOdelegate;

		// Add unit to the new player's team
		player.UnitInstances.Add ( unit );
		unit.Owner = player;
		if ( player.StandardKOdelegate != null )
			unit.koDelegate += Owner.StandardKOdelegate;

		// Face unit in correct direction
		Util.OrientSpriteToDirection ( unit.sprite, player.TeamDirection );

		// Update HUD
		GM.UI.matchInfoMenu.GetPlayerHUD ( unit ).UpdatePortrait ( unit );
	}

	/// <summary>
	/// Removes the clone from the arena.
	/// </summary>
	private void RemoveClone ( )
	{
		// Create animation
		Tween t1 = currentClone.transform.DOScale ( new Vector3 ( 5, 5, 5 ), KO_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Remove clone from tile
				currentClone.CurrentHex.Tile.CurrentObject = null;

				// Remove clone from the team
				Owner.TileObjects.Remove ( currentClone );

				// Delete the clone
				Destroy ( currentClone.gameObject );
				currentClone = null;
			} );
		Tween t2 = currentClone.Icon.DOFade ( 0, MOVE_ANIMATION_TIME );

		// Add animation to 
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
	}

	#endregion // Private Functions
}
