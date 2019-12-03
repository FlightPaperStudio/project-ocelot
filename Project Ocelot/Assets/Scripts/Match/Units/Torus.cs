using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Units
{
	public class Torus : HeroUnit
	{
		/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
		///
		/// Hero 9 Unit Data
		/// 
		/// ID: 19
		/// Name: Hero 9
		/// Nickname: Run the Ropes
		/// Bio: ???
		/// Finishing Move: ???
		/// Role: Offense
		/// Slot: 1
		/// 
		/// Ability 1
		/// ID: 34
		/// Name: Run the Ropes
		/// Description: Launches themself off the ropes to reach the other side of the arena
		/// Type: Special
		/// Cooldown: 2 Rounds
		/// Attack Opponents: Active
		/// 
		/// Ability 2
		/// ID: 35
		/// Name: Taunt
		/// Description: Insults an opponent, forcing the opponent out of position
		/// Type: Command
		/// Cooldown: 6 Rounds
		/// Range: 4 Tiles
		/// 
		/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

		#region Ability Data

		private Dictionary<Hex, int> tauntTargetDirection = new Dictionary<Hex, int> ( );

		#endregion // Ability Data

		#region Public Unit Override Functions

		/// <summary>
		/// Calculates all base moves available to a unit as well as any special ability moves.
		/// </summary>
		public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
		{
			// Get base moves
			base.FindMoves ( hex, prerequisite, returnOnlyJumps );

			// Get Run The Ropes moves
			if ( SpecialAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
				GetRunTheRopes ( hex, prerequisite, returnOnlyJumps );

			// Get Taunt availability
			InstanceData.Ability2.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability2, prerequisite );
		}

		#endregion // Public Unit Override Functions

		#region Public HeroUnit Override Functions

		public override void ExecuteCommand ( )
		{
			// Pause turn timer
			if ( Match.MatchSettings.TurnTimer )
				GM.UI.timer.PauseTimer ( );

			// Hide cancel button
			GM.UI.UnitHUD.HideCancelButton ( InstanceData.Ability2 );

			// Clear board
			GM.Grid.ResetTiles ( );

			// Store target and destination
			Unit unit = GM.SelectedCommand.PrimaryTarget.Tile.CurrentUnit;
			Hex destination = GM.SelectedCommand.PrimaryTarget.Neighbors [ Tools.Util.GetOppositeDirection ( tauntTargetDirection [ GM.SelectedCommand.PrimaryTarget ] ) ];

			// Interupt target
			unit.InteruptUnit ( );

			// Begin animation
			Sequence s = DOTween.Sequence ( )
				.Append ( unit.transform.DOMove ( destination.transform.position, MOVE_ANIMATION_TIME ) )
				.OnComplete ( ( ) =>
				{
				// Mark that the ability is no longer active
				InstanceData.Ability2.IsActive = false;

				// Remove target from previous tile
				GM.SelectedCommand.PrimaryTarget.Tile.CurrentUnit = null;

				// Set the target's new current tile
				unit.CurrentHex = destination;
					destination.Tile.CurrentUnit = unit;

				// Start cooldown
				StartCooldown ( InstanceData.Ability2 );

				// Pause turn timer
				if ( Match.MatchSettings.TurnTimer )
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
		/// Checks if there adjacent unoccupied tiles available for the Self-Destruct/Recall Ability.
		/// Returns true if at least one adjacent tile is unoccupied.
		/// </summary>
		protected override bool CommandAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
		{
			// Check base conditions
			if ( !base.CommandAvailabilityCheck ( ability, prerequisite ) )
				return false;

			// Check if a target is within range
			if ( !TauntCheck ( ) )
				return false;

			// Return that the ability is available
			return true;
		}

		protected override void GetCommandTargets ( )
		{
			// Clear previous targets
			tauntTargetDirection.Clear ( );

			// Get every Taunt target
			for ( int direction = 0; direction < CurrentHex.Neighbors.Length; direction++ )
			{
				// Check each tile within range
				for ( int range = 2; range <= InstanceData.Ability2.PerkValue; range++ )
				{
					// Get the tile
					Hex targetHex = CurrentHex.Neighbor ( (Hex.Direction)direction, range );

					// Check for tile
					if ( targetHex == null )
						continue;

					// Check for unit
					if ( targetHex.Tile.CurrentUnit == null )
						continue;

					// Check for ally
					if ( targetHex.Tile.CurrentUnit.Owner == Owner )
						continue;

					// Check unit status
					if ( !targetHex.Tile.CurrentUnit.Status.CanBeMoved || !targetHex.Tile.CurrentUnit.Status.CanBeAffectedByAbility )
						continue;

					// Get the destination tile
					Hex destinationHex = CurrentHex.Neighbor ( (Hex.Direction)direction, range - 1 );

					// Check for tile
					if ( destinationHex == null )
						continue;

					// Check if the tile is occupied
					if ( destinationHex.Tile.IsOccupied )
						continue;

					// Mark target
					targetHex.Tile.SetTileState ( TileState.AvailableCommand );

					// Store target and direction
					tauntTargetDirection.Add ( targetHex, direction );
				}
			}
		}

		/// <summary>
		/// Uses the unit's special ability.
		/// Override this function to call specific special ability functions for a hero unit.
		/// </summary>
		protected override void UseSpecial ( MoveData data )
		{
			// Get tiles
			Hex startHex = data.PriorMove == null ? CurrentHex : data.PriorMove.Destination;
			Hex endHex = data.Destination;

			// Get rope positions
			Vector3 ropesPos1 = startHex.transform.position;
			if ( startHex.Neighbors [ (int)data.Direction ] != null )
				ropesPos1 = startHex.Neighbors [ (int)data.Direction ].transform.position + Tools.Util.GetTileTransformDistance ( data.Direction );
			else
				ropesPos1 += Tools.Util.GetTileTransformDistance ( data.Direction );
			Vector3 ropesPos2 = endHex.transform.position;
			if ( endHex.Neighbors [ Tools.Util.GetOppositeDirection ( (int)data.Direction ) ] != null )
				ropesPos2 = endHex.Neighbors [ Tools.Util.GetOppositeDirection ( (int)data.Direction ) ].transform.position + Tools.Util.GetTileTransformDistance ( Tools.Util.GetOppositeDirection ( (int)data.Direction ) );
			else
				ropesPos2 += Tools.Util.GetTileTransformDistance ( Tools.Util.GetOppositeDirection ( (int)data.Direction ) );

			// Get animation timing
			float duration1 = ( GM.Grid.GetDistance ( startHex, GetEdgeTile ( Tools.Util.MoveDirectionToHexDirection ( data.Direction ) ) ) + 1 ) * MOVE_ANIMATION_TIME;
			float duration2 = ( GM.Grid.GetDistance ( GetEdgeTile ( Tools.Util.MoveDirectionToHexDirection ( data.Direction ) ), GetEdgeTile ( Tools.Util.MoveDirectionToHexDirection ( Tools.Util.GetOppositeDirection ( data.Direction ) ) ) ) + 2 ) * MOVE_ANIMATION_TIME;
			float duration3 = ( GM.Grid.GetDistance ( endHex, GetEdgeTile ( Tools.Util.MoveDirectionToHexDirection ( Tools.Util.GetOppositeDirection ( data.Direction ) ) ) ) + 1 ) * MOVE_ANIMATION_TIME;

			// Create animations
			Tween t1 = transform.DOMove ( ropesPos1, duration1 ) // Move from the start position to the first ropes
				.OnStart ( ( ) =>
				{
				// Mark that the ability is active
				InstanceData.Ability1.IsActive = true;
					GM.UI.UnitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
				} );
			Tween t2 = transform.DOMove ( ropesPos2, duration2 ); // Move across the arena from the first ropes to the second ropes
			Tween t3 = transform.DOMove ( endHex.transform.position, duration3 ) // Move from the second ropes to the end position
				.OnComplete ( ( ) =>
				{
				// Mark that the ability is no longer active
				InstanceData.Ability1.IsActive = false;

				// Start Run The Ropes cooldown
				StartCooldown ( InstanceData.Ability1 );

				// Set unit and tile data
				SetUnitToTile ( data.Destination );
				} );

			// Add animation to queue
			GM.AnimationQueue.Add ( new Match.GameManager.TurnAnimation ( t1, true ) );
			GM.AnimationQueue.Add ( new Match.GameManager.TurnAnimation ( t2, true ) );
			GM.AnimationQueue.Add ( new Match.GameManager.TurnAnimation ( t3, true ) );
		}

		#endregion // Protected HeroUnit Override Functions

		#region Private Functions

		/// <summary>
		/// Marks every tiles available to the Run The Ropes ability.
		/// </summary>
		private void GetRunTheRopes ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
		{
			// Store which tiles are to be ignored
			//IntPair back = GetBackDirection ( owner.TeamDirection );

			// Check each neighboring tile
			for ( int i = 0; i < hex.Neighbors.Length; i++ )
			{
				// Ignore tiles that would allow for backward movement
				//if ( i == back.FirstInt || i == back.SecondInt )
				//	continue;

				// Check for edge tiles
				if ( hex.Neighbors [ i ] == null )
				{
					// Get opposite direction
					//int direction = Util.GetOppositeDirection ( i );
					Hex.Direction direction = Hex.GetOppositeDirection ( (Hex.Direction)i );

					// Get opposite edge tile
					Hex targetHex = GetEdgeTile ( direction );

					// Check if this unit can move to the edge tile
					if ( !returnOnlyJumps && OccupyTileCheck ( targetHex, prerequisite ) )
					{
						// Add as an available move
						AddMove ( new MoveData ( targetHex, prerequisite, MoveData.MoveType.SPECIAL, i ) );
					}
					// Check if this unit can jump the edge tile
					else if ( JumpTileCheck ( targetHex ) && OccupyTileCheck ( targetHex.Neighbors [ i ], prerequisite ) )
					{
						// Track move data
						MoveData move;

						// Check if the unit can be attacked
						if ( InstanceData.Ability1.IsPerkEnabled && targetHex.Tile.CurrentUnit.UnitAttackCheck ( this ) )
						{
							// Add as an available attack
							move = new MoveData ( targetHex.Neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i, null, targetHex );
						}
						else
						{
							// Add as an available jump
							move = new MoveData ( targetHex.Neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i, targetHex, null );
						}

						// Add move to the move list
						AddMove ( move );

						// Find additional jumps
						FindMoves ( targetHex.Neighbors [ i ], move, true );
					}
				}
				// Check for neighboring edge tiles
				else if ( JumpTileCheck ( hex.Neighbors [ i ] ) && hex.Neighbors [ i ].Neighbors [ i ] == null )
				{
					// Get opposite direction
					Hex.Direction direction = Hex.GetOppositeDirection ( (Hex.Direction)i );

					// Get opposite edge tile
					Hex targetHex = GetEdgeTile ( direction );

					// Check if this unit can move to the edge tile
					if ( OccupyTileCheck ( targetHex, prerequisite ) )
					{
						// Track move data
						MoveData move;

						// Check if the unit can be attacked
						if ( InstanceData.Ability1.IsPerkEnabled && hex.Neighbors [ i ].Tile.CurrentUnit.UnitAttackCheck ( this ) )
						{
							// Add as an available attack
							move = new MoveData ( targetHex, prerequisite, MoveData.MoveType.SPECIAL, i, null, hex.Neighbors [ i ] );
						}
						else
						{
							// Add as an available jump
							move = new MoveData ( targetHex, prerequisite, MoveData.MoveType.SPECIAL, i, hex.Neighbors [ i ], null );
						}

						// Add move to the move list
						AddMove ( move );

						// Find additional jumps
						FindMoves ( targetHex, move, true );
					}
				}
			}
		}

		/// <summary>
		/// Navigates the hex grid in a direction until it finds an edge tile and returns it.
		/// </summary>
		/// <param name="direction"> The direction being searched in. </param>
		/// <returns> The edge tile in the given direction. </returns>
		private Hex GetEdgeTile ( Hex.Direction direction )
		{
			// Get the distance to the edge
			int distance = DistanceToEdge ( direction );

			// Return the edge tile
			return CurrentHex.Neighbor ( direction, distance );
		}

		/// <summary>
		/// Calculates the distance between the hero and the edge of the arena in a given direction.
		/// </summary>
		/// <param name="direction"> The direction being searched in. </param>
		/// <returns> The distance between the hero and the edge of the arena. </returns>
		private int DistanceToEdge ( Hex.Direction direction )
		{
			// Track the number of tiles to the edge
			int count = 1;

			// Check each tile until the edge is found
			while ( CurrentHex.Neighbor ( direction, count ) != null )
				count++;

			// Return the count
			return count - 1;
		}

		/// <summary>
		/// Checks to see if an opposing unit is within Range of and capable of being targeted by Taunt.
		/// Returns true if at least one unit can be targeted by Taunt.
		/// </summary>
		/// <returns> Whether or not an opposing unit can be targeted by Taunt. </returns>
		private bool TauntCheck ( )
		{
			// Check every direction
			for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
			{
				// Search for target
				if ( GetTaunt ( (Hex.Direction)i ) != null )
				{
					// Mark that a target was found
					return true;
				}
			}

			// Return that no target was found
			return false;
		}

		/// <summary>
		/// Finds an opposing unit that is within Range and can be moved to an unoccupied tile toward the hero by Taunt.
		/// </summary>
		/// <param name="direction"> The direction of the tiles to be checked. </param>
		/// <returns> The hex tile of a potential target for Taunt. </returns>
		private Hex GetTaunt ( Hex.Direction direction )
		{
			// Check all tiles within range
			for ( int i = 2; i <= InstanceData.Ability2.PerkValue; i++ )
			{
				// Get the tile
				Hex targetHex = CurrentHex.Neighbor ( direction, i );

				// Check for tile
				if ( targetHex == null )
					continue;

				// Check for unit
				if ( targetHex.Tile.CurrentUnit == null )
					continue;

				// Check for ally
				if ( targetHex.Tile.CurrentUnit.Owner == Owner )
					continue;

				// Check unit status
				if ( !targetHex.Tile.CurrentUnit.Status.CanBeMoved || !targetHex.Tile.CurrentUnit.Status.CanBeAffectedByAbility )
					continue;

				// Get the destination tile
				Hex destinationHex = CurrentHex.Neighbor ( direction, i - 1 );

				// Check for tile
				if ( destinationHex == null )
					continue;

				// Check if the tile is occupied
				if ( destinationHex.Tile.IsOccupied )
					continue;

				// Return the target
				return targetHex;
			}

			// Return that no target was found
			return null;
		}

		#endregion // Private Functions
	}
}