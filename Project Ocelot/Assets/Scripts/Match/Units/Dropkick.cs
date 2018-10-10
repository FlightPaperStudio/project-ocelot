using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dropkick : HeroUnit
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
	///
	/// Hero 10 Unit Data
	/// 
	/// ID: 20
	/// Name: Hero 10
	/// Nickname: Divebomb
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Defense
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 36
	/// Name: Divebomb
	/// Description: Jumps off the ropes, knocking back any nearby opponents upon landing
	/// Type: Special
	/// Duration: 1 Round
	/// Cooldown: 3 Rounds
	/// Range: 4 Tiles
	/// Status Effects (Self): Stun
	/// 
	/// Ability 2
	/// ID: 37
	/// Name: Dropkick
	/// Description: Kicks a nearby opponent to knock back the opponent and anyone behind
	/// Type: Command
	/// Duration: 1 Round
	/// Cooldown: 4 Rounds
	/// Target Allies: Active
	/// Status Effects (Self): Exhaustion
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	private Dictionary<Hex, int> dropkickTargetDirection = new Dictionary<Hex, int> ( );

	private const float DIVE_ANIMATION_TIME = 1.0f;
	private const float DIVE_ANIMATION_SCALE = 2.0f;

	#endregion // Ability Data

	#region Public Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	/// <param name="hex"> The tile who's neighbor will be checked for moves. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach this tile. </param>
	/// <param name="returnOnlyJumps"> Whether or not only jump moves should be stored as available moves. </param>
	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Divebomb moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
			GetDivebomb ( );

		// Get Dropkick availability
		InstanceData.Ability2.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability2, prerequisite );
	}

	#endregion // Unit Override Functions

	#region Public HeroUnit Override Functions

	public override void ExecuteCommand ( )
	{
		// Execute base command
		base.ExecuteCommand ( );

		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Grid.ResetTiles ( );

		// Create animation
		Sequence s = DOTween.Sequence ( );

		// Animate dropkick
		s.Append ( transform.DOMove ( CurrentHex.transform.position + ( ( GM.SelectedCommand.PrimaryTarget.transform.position - CurrentHex.transform.position ) * 0.5f ), MOVE_ANIMATION_TIME / 2 ) );
		s.Append ( transform.DOMove ( CurrentHex.transform.position, MOVE_ANIMATION_TIME / 2 ) );

		// Knockback targets
		DropkickKnockback ( GM.SelectedCommand.PrimaryTarget, dropkickTargetDirection [ GM.SelectedCommand.PrimaryTarget ], s );

		// End animation
		s.OnComplete ( ( ) =>
		{
			// Mark that the ability is no longer active
			InstanceData.Ability2.IsActive = false;

			// Start cooldown
			StartCooldown ( InstanceData.Ability2 );

			// Apply status effect
			Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.EXHAUSTION, InstanceData.Ability2.Duration, this );
			GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );

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

	#endregion // Public HeroUnit Override Functions

	#region Protected HeroUnit Override Functions

	protected override void GetCommandTargets ( )
	{
		// Clear previous targets
		dropkickTargetDirection.Clear ( );

		// Get back direction
		//IntPair back = GetBackDirection ( Owner.TeamDirection );

		// Check each neighbor tile
		for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
		{
			// Check for back direction
			//if ( i == back.FirstInt || i == back.SecondInt )
			//	continue;
			
			// Check for tile
			if ( CurrentHex.Neighbors [ i ] == null )
				continue;
			
			// Check if occupied
			if ( !CurrentHex.Neighbors [ i ].Tile.IsOccupied )
				continue;
			
			// Check for object
			if ( CurrentHex.Neighbors [ i ].Tile.CurrentObject != null )
			{
				// Check if the object can be moved
				if ( !CurrentHex.Neighbors [ i ].Tile.CurrentObject.CanBeMoved )
					continue;
				
				// Check if the object is an ally
				if ( !InstanceData.Ability2.IsPerkEnabled && CurrentHex.Neighbors [ i ].Tile.CurrentObject.Caster.Owner == Owner )
					continue;

				// Check for knockback tile
				if ( !KnockbackTileCheck ( CurrentHex.Neighbors [ i ].Neighbors [ i ], i, true ) )
					continue;

				// Mark target
				CurrentHex.Neighbors [ i ].Tile.SetTileState ( TileState.AvailableCommand );

				// Store direction
				dropkickTargetDirection.Add ( CurrentHex.Neighbors [ i ], i );
			}

			// Check for unit
			if ( CurrentHex.Neighbors [ i ].Tile.CurrentUnit != null )
			{
				// Check if the unit can be moved
				if ( !CurrentHex.Neighbors [ i ].Tile.CurrentUnit.Status.CanBeMoved )
					continue;

				// Check if the unit is incorporeal
				if ( !CurrentHex.Neighbors [ i ].Tile.CurrentUnit.Status.CanBeAffectedPhysically )
					continue;

				// Check if the unit is an ally
				if ( !InstanceData.Ability2.IsPerkEnabled && CurrentHex.Neighbors [ i ].Tile.CurrentUnit.Owner == Owner )
					continue;

				// Check for knockback tile
				if ( !KnockbackTileCheck ( CurrentHex.Neighbors [ i ].Neighbors [ i ], i, true ) )
					continue;

				// Mark target
				CurrentHex.Neighbors [ i ].Tile.SetTileState ( TileState.AvailableCommand );

				// Store direction
				dropkickTargetDirection.Add ( CurrentHex.Neighbors [ i ], i );
			}
		}
	}

	/// <summary>
	/// Checks if the hero is capable of using a special ability.
	/// Returns true if the special ability is available.
	/// </summary>
	/// /// <param name="ability"> The current ability data for the special ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given special ability. </param>
	/// <returns> Whether or not the special ability can be used. </returns>
	protected override bool SpecialAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.SpecialAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check if any move has been made
		if ( prerequisite != null )
			return false;

		// Check for edge
		if ( !CurrentHex.IsEdge )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if this hero is capable of using a command ability.
	/// Returns true if the command ability is available.
	/// </summary>
	/// <param name="ability"> The current ability data for the command ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given command ability. </param>
	/// <returns> Whether or not the command ability can be used. </returns>
	protected override bool CommandAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check if a target is available
		if ( !DropkickCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	/// <param name="data"> The move data required for this move. </param>
	protected override void UseSpecial ( MoveData data )
	{
		// Create animation for dive
		Tween t1 = transform.DOMove ( data.Destination.transform.position, DIVE_ANIMATION_TIME )
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

				// Start teleport cooldown
				StartCooldown ( InstanceData.Ability1 );

				// Apply status effect
				Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.STUNNED, InstanceData.Ability1.Duration, this );
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );

				// Set unit and tile data
				SetUnitToTile ( data.Destination );
			} );
		Tween t2 = transform.DOScale ( DIVE_ANIMATION_SCALE, DIVE_ANIMATION_TIME / 2 )
			.SetLoops ( 2, LoopType.Yoyo );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );

		int targetCount = 0;

		// Get knockback targets
		for ( int i = 0; i < data.Destination.Neighbors.Length; i++ )
		{
			// Check for tile
			if ( data.Destination.Neighbors [ i ] == null )
				continue;

			// Check if tile is occupied
			if ( !data.Destination.Neighbors [ i ].Tile.IsOccupied )
				continue;

			// Check for unit
			if ( data.Destination.Neighbors [ i ].Tile.CurrentUnit != null )
			{
				// Check if the unit can be attacked
				if ( !data.Destination.Neighbors [ i ].Tile.CurrentUnit.UnitAttackCheck ( this ) )
					continue;

				// Check if the unit can be moved
				if ( !data.Destination.Neighbors [ i ].Tile.CurrentUnit.Status.CanBeMoved )
					continue;

				// Check if the unit can be affected by abilities
				if ( !data.Destination.Neighbors [ i ].Tile.CurrentUnit.Status.CanBeAffectedByAbility )
					continue;

				// Check if the unit can be affected physically
				if ( !data.Destination.Neighbors [ i ].Tile.CurrentUnit.Status.CanBeAffectedPhysically )
					continue;

				// Check for knockback destination
				if ( !KnockbackTileCheck ( data.Destination.Neighbors [ i ].Neighbors [ i ], i, false ) )
					continue;

				// Create animation for knockback
				targetCount++;
				DivebombKnockback ( data.Destination.Neighbors [ i ].Tile.CurrentUnit, data.Destination.Neighbors [ i ].Neighbors [ i ], targetCount <= 1 );
			}

			// Check for object
			if ( data.Destination.Neighbors [ i ].Tile.CurrentObject != null )
			{
				// Check if the object can be moved
				if ( !data.Destination.Neighbors [ i ].Tile.CurrentObject.CanBeMoved )
					continue;

				// Check for knockback destination
				if ( !KnockbackTileCheck ( data.Destination.Neighbors [ i ].Neighbors [ i ], i, false ) )
					continue;

				// Create animation for knockback
				targetCount++;
				DivebombKnockback ( data.Destination.Neighbors [ i ].Tile.CurrentObject, data.Destination.Neighbors [ i ].Neighbors [ i ], targetCount <= 1 );
			}
		}
	}

	#endregion // HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Checks to see if a tile is an edge tile.
	/// </summary>
	/// <param name="hex"> The tile being check as an edge tile. </param>
	/// <returns> Whether or not the given tile is an edge tile. </returns>
	//private bool EdgeCheck ( Hex hex )
	//{
	//	// Check each neighbor tile
	//	for ( int i = 0; i < hex.Neighbors.Length; i++ )
	//	{
	//		// Check for edge
	//		if ( hex.Neighbors [ i ] == null )
	//			return true;
	//	}

	//	// Return that no edge was found
	//	return false;
	//}

	/// <summary>
	/// Gets all tiles within range of the Divebomb ability.
	/// </summary>
	private void GetDivebomb ( )
	{
		// Get all tiles within range
		List<Hex> hexes = CurrentHex.Range ( InstanceData.Ability1.PerkValue );

		// Check each hex
		for ( int i = 0; i < hexes.Count; i++ )
		{
			// Check if the tile is occupied
			if ( !OccupyTileCheck ( hexes [ i ], null ) )
				continue;

			// Check if the tile is the minimum distance away
			if ( CurrentHex.Distance ( hexes [ i ] ) == 1 )
				continue;

			// Add as an available special move
			MoveList.Add ( new MoveData ( hexes [ i ], null, MoveData.MoveType.SPECIAL, i ) );
		}
	}

	/// <summary>
	/// Checks if a tile is available for knocking a unit back.
	/// </summary>
	/// <param name="hex"> The tile being checked. </param>
	/// <param name="direction"> The direction the unit is being knocked back. </param>
	/// <param name="isRecursive"> Whether or not tiles should be continuously checked. </param>
	/// <returns> Whether or not a tile is available for knockback. </returns>
	private bool KnockbackTileCheck ( Hex hex, int direction, bool isRecursive )
	{
		// Check for tile
		if ( hex == null )
			return false;

		// Check for unit
		if ( hex.Tile.CurrentUnit != null )
		{
			// Check if recusive
			if ( !isRecursive )
				return false;

			// Check if the unit can not be moved
			if ( !hex.Tile.CurrentUnit.Status.CanBeMoved )
				return false;

			// Check if the unit can be affected by abilities
			if ( !hex.Tile.CurrentUnit.Status.CanBeAffectedByAbility )
				return false;

			// Check if the unit can be affected by physical abilities
			if ( !hex.Tile.CurrentUnit.Status.CanBeAffectedPhysically )
				return false;

			// Check next tile in the direction
			return KnockbackTileCheck ( hex.Neighbors [ direction ], direction, isRecursive );
		}

		// Check for object
		if ( hex.Tile.CurrentObject != null )
		{
			// Check if recusive
			if ( !isRecursive )
				return false;

			// Check if the unit can not be moved
			if ( !hex.Tile.CurrentObject.CanBeMoved )
				return false;

			// Check next tile in the direction
			return KnockbackTileCheck ( hex.Neighbors [ direction ], direction, isRecursive );
		}

		// Return that there is room for knockback
		return true;
	}

	/// <summary>
	/// Knocks a unit back from the Divebomb ability
	/// </summary>
	/// <param name="target"> The unit affected by the Divebomb ability. </param>
	/// <param name="destination"> The tile the unit is being knocked back to. </param>
	private void DivebombKnockback ( Unit target, Hex destination, bool isAppend )
	{
		// Create animation
		Tween t = target.transform.DOMove ( destination.transform.position, MOVE_ANIMATION_TIME )
			.OnStart ( ( ) =>
			{
				// Interupt target
				target.InteruptUnit ( );
			} )
			.OnComplete ( ( ) =>
			{
				// Remove target from previous tile
				target.CurrentHex.Tile.CurrentUnit = null;

				// Set the target's new current tile
				target.CurrentHex = destination;
				destination.Tile.CurrentUnit = target;
			} );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, isAppend ) );
	}

	/// <summary>
	/// Knocks a unit back from the Divebomb ability
	/// </summary>
	/// <param name="target"> The object affected by the Divebomb ability. </param>
	/// <param name="destination"> The tile the unit is being knocked back to. </param>
	private void DivebombKnockback ( TileObject target, Hex destination, bool isAppend )
	{
		// Create animation
		Tween t = target.transform.DOMove ( destination.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Remove target from previous tile
				target.CurrentHex.Tile.CurrentObject = null;

				// Set the target's new current tile
				target.CurrentHex = destination;
				destination.Tile.CurrentObject = target;
			} );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, isAppend ) );
	}

	/// <summary>
	/// Checks if any targets are avialable for the Dropkick ability.
	/// </summary>
	/// <returns> Whether or not any targets are available. </returns>
	private bool DropkickCheck ( )
	{
		// Store which tiles are to be ignored
		//IntPair back = GetBackDirection ( Owner.TeamDirection );

		// Check each neighboring tile
		for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			//if ( i == back.FirstInt || i == back.SecondInt )
			//	continue;

			// Check for tile
			if ( CurrentHex.Neighbors [ i ] == null )
				continue;

			// Check for object
			if ( CurrentHex.Neighbors [ i ].Tile.CurrentObject != null )
			{
				// Check if the object can be moved
				if ( !CurrentHex.Neighbors [ i ].Tile.CurrentObject.CanBeMoved )
					continue;

				// Check if the object is an ally
				if ( !InstanceData.Ability2.IsPerkEnabled && CurrentHex.Neighbors [ i ].Tile.CurrentObject.Caster.Owner == Owner )
					continue;

				// Check for knockback tile
				if ( !KnockbackTileCheck ( CurrentHex.Neighbors [ i ].Neighbors [ i ], i, true ) )
					continue;

				// Return that an object target has been found
				return true;
			}

			// Check for unit
			if ( CurrentHex.Neighbors [ i ].Tile.CurrentUnit != null )
			{
				// Check if the unit can be moved
				if ( !CurrentHex.Neighbors [ i ].Tile.CurrentUnit.Status.CanBeMoved )
					continue;

				// Check if the unit can be affected by abilities
				if ( !CurrentHex.Neighbors [ i ].Tile.CurrentUnit.Status.CanBeAffectedByAbility )
					continue;

				// Check if the unit can be affected by physical abilities
				if ( !CurrentHex.Neighbors [ i ].Tile.CurrentUnit.Status.CanBeAffectedPhysically )
					continue;

				// Check if the unit is an ally
				if ( !InstanceData.Ability2.IsPerkEnabled && CurrentHex.Neighbors [ i ].Tile.CurrentUnit.Owner == Owner )
					continue;

				// Check for knockback tile
				if ( !KnockbackTileCheck ( CurrentHex.Neighbors [ i ].Neighbors [ i ], i, true ) )
					continue;

				// Return that a unit target has been found
				return true;
			}
		}

		// Return that no targets were found
		return false;
	}

	/// <summary>
	/// Knocks back a target from the Dropkick ability.
	/// </summary>
	/// <param name="hex"> The current tile of the target. </param>
	/// <param name="direction"> The direction of the dropkick. </param>
	/// <param name="s"> The animation sequence for the dropkick. </param>
	private void DropkickKnockback ( Hex hex, int direction, Sequence s )
	{
		// Check for additional targets
		if ( hex.Neighbors [ direction ] != null && ( hex.Neighbors [ direction ].Tile.CurrentUnit != null || hex.Neighbors [ direction ].Tile.CurrentObject != null ) )
			DropkickKnockback ( GetNextDropkickTarget ( hex.Neighbors [ direction ], direction ), direction, s );

		// Check for unit
		if ( hex.Tile.CurrentUnit != null )
		{
			// Store target
			Unit target = hex.Tile.CurrentUnit;

			// Clear target's current tile
			hex.Tile.CurrentUnit = null;

			// Interupt target
			target.InteruptUnit ( );

			// Get destination
			Hex destination = GetNextDropkickTarget ( hex.Neighbors [ direction ], direction );

			// Move target
			Tween animation = target.transform.DOMove ( destination.transform.position, MOVE_ANIMATION_TIME )
				.OnComplete ( ( ) =>
				{
				// Set target to tile
				target.CurrentHex = destination;
					destination.Tile.CurrentUnit = target;
				} );

			// Add animation
			s.Join ( animation );
		}

		// Check for object
		if ( hex.Tile.CurrentObject != null )
		{
			// Store target
			TileObject target = hex.Tile.CurrentObject;

			// Clear target's current tile
			hex.Tile.CurrentObject = null;

			// Get destination
			Hex destination = GetNextDropkickTarget ( hex.Neighbors [ direction ], direction );

			// Move target
			Tween animation = target.transform.DOMove ( destination.transform.position, MOVE_ANIMATION_TIME )
				.OnComplete ( ( ) =>
				{
					// Set target to tile
					target.CurrentHex = destination;
					destination.Tile.CurrentObject = target;
				} );

			// Add animation
			s.Join ( animation );
		}
	}

	/// <summary>
	/// Gets the next tile in line for a Dropkick chain knock back.
	/// </summary>
	/// <param name="hex"> The tile being checked as a potential destination. </param>
	/// <param name="direction"> The direction of the dropkick </param>
	/// <returns> The available tile for knockback. </returns>
	private Hex GetNextDropkickTarget ( Hex hex, int direction )
	{
		// Check for object
		if ( hex.Tile.CurrentObject != null )
			return hex;

		// Check for unit
		if ( hex.Tile.CurrentUnit != null )
		{
			// Check if the unit is incorporeal
			if ( hex.Tile.CurrentUnit.Status.Effects.Exists ( x => x.ID == (int)StatusEffectDatabase.StatusEffectType.INCORPOREAL ) )
				return GetNextDropkickTarget ( hex.Neighbors [ direction ], direction );
			else
				return hex;
		}
		

		// Return this empty tile
		return hex;
	}

	#endregion // Private Functions
}
