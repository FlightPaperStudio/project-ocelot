using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaroUra : Leader
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
	///
	/// Maro-ura Unit Data
	/// 
	/// ID: 6
	/// Name: Maro-ura
	/// Nickname: The Dense God
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Leader
	/// Act: Face
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 5
	/// Name: Gravity Well
	/// Description: Maro-ura summons a black hole to pull in nearby units
	/// Type: Command
	/// Duration: 3 Rounds
	/// Cooldown: 8 Rounds
	/// Range: 4 Tile Radius
	/// Tile Object: Black Hole
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	[SerializeField]
	private TileObject blackHolePrefab;

	private TileObject currentBlackHole;

	private const float BLACK_HOLE_ANIMATION_TIME = 0.75f;
	private const float BLACK_HOLE_MOVE_ANIMATION_TIME = 0.2f;
	private const float INTERVAL_TIME = 0.3f;

	#endregion // Ability Data

	#region Public Unit Override Functions

	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Find base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Gravity Well availability
		InstanceData.Ability1.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability1, prerequisite );
	}

	#endregion // Public Unit Override Functions

	#region Public HeroUnit Override Functions

	public override void ExecuteCommand ( )
	{
		// Execute base command
		base.ExecuteCommand ( );

		// Check for previous ability cast
		if ( currentBlackHole != null )
		{
			// Destroy the black hole
			Destroy ( currentBlackHole.gameObject );

			// Reset black hole
			currentBlackHole = null;
		}

		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability1 );

		// Clear board
		GM.Grid.ResetTiles ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( );

		// Create the zorya sister
		currentBlackHole = CreateTileOject ( blackHolePrefab, GM.SelectedCommand.PrimaryTarget, InstanceData.Ability1.Duration, RemoveGravityWell );

		// Create spawning animation
		s.Append ( currentBlackHole.gameObject.transform.DOScale ( 0f, BLACK_HOLE_ANIMATION_TIME ).From ( ) );

		// Apply gravity well
		List<Tween> animations = ApplyGravityWell ( GM.SelectedCommand.PrimaryTarget );
		for ( int i = 0; i < animations.Count; i++ )
		{
			if ( i == 0 )
				s.Append ( animations [ i ] );
			else
				s.Join ( animations [ i ] );
		}

		// Complete the animation
		s.OnComplete ( ( ) =>
		{
			// Start cooldown
			StartCooldown ( InstanceData.Ability1 );

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

	public override void Cooldown ( )
	{
		// Set base cooldown
		base.Cooldown ( );

		// Check for black hole
		if ( InstanceData.Ability1.CurrentDuration > 0 )
		{
			// Apply gravity well
			List<Tween> animations = ApplyGravityWell ( currentBlackHole.CurrentHex );
			for ( int i = 0; i < animations.Count; i++ )
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( animations [ i ], i == 0 ) );
		}
	}

	#endregion // Public HeroUnit Override Functions

	#region Protected HeroUnit Override Functions

	protected override void GetCommandTargets ( )
	{
		// Get targets
		GetGravityWell ( );
	}

	protected override void OnDurationComplete ( AbilityInstanceData ability )
	{
		// Destroy the black hole
		RemoveGravityWell ( );
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Marks every unoccupied tile within range as available for selection for Gravity Well.
	/// </summary>
	private void GetGravityWell ( )
	{
		// Get targets within range
		List<Hex> targets = CurrentHex.Range ( InstanceData.Ability1.PerkValue );

		// Check each potential direction
		for ( int dir = 0; dir < CurrentHex.Neighbors.Length; dir++ )
		{
			// Check each target within range
			for ( int i = 1; i <= InstanceData.Ability1.PerkValue; i++ )
			{
				// Get target
				Hex target = CurrentHex.Neighbor ( (Hex.Direction)dir, i );

				// Check that tile exists
				if ( target == null )
					continue;

				// Check that the tile is unoccupied
				if ( target.Tile.IsOccupied )
					continue;

				// Add tile as potential target
				target.Tile.SetTileState ( TileState.AvailableCommand );
			}
		}
	}

	/// <summary>
	/// Draws in any targets within range of the black hole.
	/// </summary>
	/// <param name="hex"> The hex of the black hole. </param>
	/// <returns> The list of targets to animate. </returns>
	private List<Tween> ApplyGravityWell ( Hex hex )
	{
		// Store list of animations
		List<Tween> animations = new List<Tween> ( );

		// Apply in each direction
		for ( int dir = 0; dir < hex.Neighbors.Length; dir++ )
		{
			// Store the current move-to hex
			Queue<Hex> moveTo = new Queue<Hex> ( );

			// Check all tiles within range
			for ( int i = 1; i <= InstanceData.Ability1.PerkValue; i++ )
			{
				// Get target hex
				Hex target = hex.Neighbor ( (Hex.Direction)dir, i );

				// Check for tile
				if ( target == null )
				{
					// Mark that the path has been broken
					moveTo.Clear ( );

					continue;
				}

				// Check if tile is unoccupied
				if ( !target.Tile.IsOccupied )
				{
					// Add hex to queue
					moveTo.Enqueue ( target );

					continue;
				}

				// Check if tile object is unoccupied
				if ( target.Tile.CurrentObject != null && target.Tile.CurrentUnit == null && target.Tile.CurrentObject.CanBeOccupied )
				{
					// Add hex to queue
					moveTo.Enqueue ( target );

					continue;
				}

				// Check if tile object is immovable
				if ( target.Tile.CurrentObject != null && !target.Tile.CurrentObject.CanBeMoved )
				{
					// Mark that the path has been broken
					moveTo.Clear ( );

					continue;
				}

				// Check if unit is immovable
				if ( target.Tile.CurrentUnit != null && !target.Tile.CurrentUnit.Status.CanBeMoved )
				{
					// Mark that the path has been broken
					moveTo.Clear ( );

					continue;
				}

				// Check if unit is immune
				if ( target.Tile.CurrentUnit != null && !target.Tile.CurrentUnit.Status.CanBeAffectedByAbility )
				{
					// Mark that the path has been broken
					moveTo.Clear ( );

					continue;
				}

				// Check if there is no move to hex
				if ( moveTo.Count == 0 )
					continue;

				// Create animation for unit
				if ( target.Tile.CurrentUnit != null )
				{
					// Get unit
					Unit unit = target.Tile.CurrentUnit;

					// Set unit to move to hex
					moveTo.Peek ( ).Tile.CurrentUnit = unit;
					unit.CurrentHex = moveTo.Peek ( );

					// Add hex to move to
					target.Tile.CurrentUnit = null;
					moveTo.Enqueue ( target );

					// Create animation
					animations.Add ( unit.gameObject.transform.DOMove ( moveTo.Peek ( ).gameObject.transform.position, BLACK_HOLE_MOVE_ANIMATION_TIME * hex.Distance ( moveTo.Peek ( ) ) ) );

					// Remove hex from move to
					moveTo.Dequeue ( );

					continue;
				}

				// Create animation for object
				if ( target.Tile.CurrentObject != null )
				{
					// Get unit
					TileObject obj = target.Tile.CurrentObject;

					// Set unit to move to hex
					moveTo.Peek ( ).Tile.CurrentObject = obj;
					obj.CurrentHex = moveTo.Peek ( );

					// Add hex to move to
					target.Tile.CurrentObject = null;
					moveTo.Enqueue ( target );

					// Create animation
					animations.Add ( obj.gameObject.transform.DOMove ( moveTo.Peek ( ).gameObject.transform.position, BLACK_HOLE_MOVE_ANIMATION_TIME * hex.Distance ( moveTo.Peek ( ) ) ) );

					// Remove hex from move to
					moveTo.Dequeue ( );

					continue;
				}
			}
		}

		// Return the list of animations
		return animations;
	}

	/// <summary>
	/// Removes the Chained status effect from each target adjacent to Zorya.
	/// </summary>
	private void RemoveGravityWell ( )
	{
		// Destroy black hole
		if ( currentBlackHole != null )
			Destroy ( currentBlackHole.gameObject );

		// Reset gravity well
		currentBlackHole = null;
	}

	#endregion // Private Functions
}
