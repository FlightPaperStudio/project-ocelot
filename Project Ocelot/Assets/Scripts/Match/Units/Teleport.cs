using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Teleport : HeroUnit
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
	///
	/// Hero 8 Unit Data
	/// 
	/// ID: 18
	/// Name: Hero 8
	/// Nickname: Teleport
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Support
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 32
	/// Name: Blink
	/// Description: Instantly teleports a short distance
	/// Type: Special
	/// Cooldown: 2 Rounds
	/// Range: 3 Tile Radius
	/// 
	/// Ability 2
	/// ID: 33
	/// Name: Dimensional Shift
	/// Description: Enters a parallel dimension where two allies have swapped positions
	/// Type: Command
	/// Cooldown: 4 Rounds
	/// Range: 3 Tile Radius
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	private const float BLINK_ANIMATION_TIME = 0.75f;

	#endregion // Ability Data

	#region Public Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Blink moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
			GetBlink ( );

		// Get Translocator availability
		InstanceData.Ability2.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability2, prerequisite );
	}

	#endregion // Public Unit Override Functions

	#region Public HeroUnit Override Functions

	public override void ExecuteCommand ( )
	{
		// Execute base command
		base.ExecuteCommand ( );

		// Use ability
		UseDimensionalShift ( );
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
	/// Checks if there adjacent unoccupied tiles available for the Self-Destruct/Recall Ability.
	/// Returns true if at least one adjacent tile is unoccupied.
	/// </summary>
	protected override bool CommandAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check for enough targets
		if ( !DimensionalShiftCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	protected override CommandData SetCommandData ( )
	{
		// Set default command data
		return new CommandData ( this, 2 );
	}

	protected override void GetCommandTargets ( )
	{
		// Check each team member
		foreach ( Unit unit in Owner.UnitInstances )
		{
			// Check for self
			if ( unit == this )
				continue;

			// Check if the unit can be moved
			if ( !unit.Status.CanBeMoved )
				continue;

			// Check if the unit can be affected by abilities
			if ( !unit.Status.CanBeAffectedByAbility )
				continue;

			// Check distance from unit
			if ( CurrentHex.Distance ( unit.CurrentHex ) > InstanceData.Ability2.PerkValue )
				continue;

			// Check if unit is already selected
			if ( unit.CurrentHex.Tile.State == TileState.SelectedCommand )
				continue;

			// Add unit as potential target
			unit.CurrentHex.Tile.SetTileState ( TileState.AvailableCommand );
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
			.OnStart ( ( ) =>
			{
				// Mark that the ability is active
				InstanceData.Ability1.IsActive = true;
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
			} )
			.OnComplete ( ( ) =>
			{
				// Move unit instantly
				transform.position = data.Destination.transform.position;
			} );
		Tween t2 = sprite.DOFade ( 1, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability1.IsActive = false;

				// Start teleport cooldown
				StartCooldown ( InstanceData.Ability1 );

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
	/// Marks every tile within range of the Blink ability.
	/// </summary>
	private void GetBlink ( )
	{
		// Get tiles within range
		List<Hex> targets = CurrentHex.Range ( InstanceData.Ability1.PerkValue );

		// Check each tile
		for ( int i = 0; i < targets.Count; i++ )
		{
			// Check for tile exists
			if ( targets [ i ] == null )
				continue;

			// Check if the tile is occupied
			if ( !OccupyTileCheck ( targets [ i ], null ) )
				continue;

			// Check for existing move
			if ( MoveList.Exists ( match => match.Destination == targets [ i ] && match.PriorMove == null ) )
				continue;

			// Add tile as an available special move
			MoveList.Add ( new MoveData ( targets [ i ], null, MoveData.MoveType.SPECIAL, MoveData.MoveDirection.DIRECT ) );
		}
	}

	/// <summary>
	/// Checks if there are a minimum number of allies to use Dimensional Shift.
	/// </summary>
	/// <returns> Whether or not there are enough targets. </returns>
	private bool DimensionalShiftCheck ( )
	{
		// Start counter
		int targetCounter = 0;

		// Check each ally
		foreach ( Unit unit in Owner.UnitInstances )
		{
			// Check if the unit is this hero
			if ( unit == this )
				continue;

			// Check if the unit can be moved
			if ( !unit.Status.CanBeMoved )
				continue;

			// Check if the unit can be affected by abilities
			if ( !unit.Status.CanBeAffectedByAbility )
				continue;

			// Check if the unit is within range
			if ( CurrentHex.Distance ( unit.CurrentHex ) > InstanceData.Ability2.PerkValue )
				continue;

			// Increment counter
			targetCounter++;

			// Check for minimum number of targets
			if ( targetCounter >= 2 )
				return true;
		}

		// Return that there are not enough targets
		return false;
	}

	/// <summary>
	/// Swaps the positions of two teammates.
	/// </summary>
	private void UseDimensionalShift ( )
	{
		// Get tiles
		Hex hex1 = GM.SelectedCommand.Targets [ 0 ];
		Hex hex2 = GM.SelectedCommand.Targets [ 1 ];

		// Get units
		Unit unit1 = hex1.Tile.CurrentUnit;
		Unit unit2 = hex2.Tile.CurrentUnit;

		// Interupt both units
		unit1.InteruptUnit ( );
		unit2.InteruptUnit ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Set units to their new tiles
		hex1.Tile.CurrentUnit = unit2;
		hex2.Tile.CurrentUnit = unit1;
		unit1.CurrentHex = hex2;
		unit2.CurrentHex = hex1;

		// Clear the board
		GM.Grid.ResetTiles ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( )
			.Append ( unit1.sprite.DOFade ( 0f, BLINK_ANIMATION_TIME ) )
			.Join ( unit2.sprite.DOFade ( 0f, BLINK_ANIMATION_TIME ) )
			.AppendCallback ( ( ) =>
			{
				// Reposition units
				unit1.transform.position = unit1.CurrentHex.transform.position;
				unit2.transform.position = unit2.CurrentHex.transform.position;
			} )
			.Append ( unit1.sprite.DOFade ( 1f, BLINK_ANIMATION_TIME ) )
			.Join ( unit2.sprite.DOFade ( 1f, BLINK_ANIMATION_TIME ) )
			.OnComplete ( ( ) =>
			{
				// Clear command data
				hex1 = null;
				hex2 = null;
				unit1 = null;
				unit2 = null;

				// Mark that the ability is no longer active
				InstanceData.Ability2.IsActive = false;

				// Start cooldown
				StartCooldown ( InstanceData.Ability2 );

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

	#endregion // Private Functions
}
