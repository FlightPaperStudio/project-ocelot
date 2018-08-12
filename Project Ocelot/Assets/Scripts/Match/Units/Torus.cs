using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Torus : HeroUnit
{
	/// <summary>
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
	/// </summary>

	// Ability information
	private Dictionary<Tile, int> tauntTargetDirection = new Dictionary<Tile, int> ( );

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

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( AbilityInstanceData ability )
	{
		// Clear the board
		base.StartCommand ( ability );

		// Clear previous targets
		tauntTargetDirection.Clear ( );

		// Get every Taunt target
		for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
		{
			// Get target
			Tile t = GetTaunt ( currentTile.neighbors [ i ], i, 1 );

			// Check if target was found
			if ( t != null )
			{
				// Mark target
				t.SetTileState ( TileState.AvailableCommand );

				// Store target and direction
				tauntTargetDirection.Add ( t, i );
			}
		}
	}

	/// <summary>
	/// Select the tile for Taunt.
	/// </summary>
	public override void SelectCommandTile ( Hex hex )
	{
		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Board.ResetTiles ( );

		// Store target and destination
		Unit u = hex.Tile.CurrentUnit;
		Tile destination = hex.Neighbors [ Util.GetOppositeDirection ( tauntTargetDirection [ hex ] ) ];

		// Interupt target
		u.InteruptUnit ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( )
			.Append ( u.transform.DOMove ( destination.transform.position, MOVE_ANIMATION_TIME ) )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability2.IsActive = false;

				// Remove target from previous tile
				hex.CurrentUnit = null;

				// Set the target's new current tile
				u.currentTile = destination;
				destination.CurrentUnit = u;

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

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Get tiles
		Tile startTile;
		Tile endTile = data.Destination;
		if ( data.PriorMove == null )
			startTile = currentTile;
		else
			startTile = data.PriorMove.Destination;

		// Get rope positions
		Vector3 ropesPos1 = startTile.transform.position;
		if ( startTile.neighbors [ (int)data.Direction ] != null )
			ropesPos1 = startTile.neighbors [ (int)data.Direction ].transform.position + Util.GetTileDistance ( data.Direction );
		else
			ropesPos1 += Util.GetTileDistance ( data.Direction );
		Vector3 ropesPos2 = endTile.transform.position;
		if ( endTile.neighbors [ Util.GetOppositeDirection ( (int)data.Direction ) ] != null )
			ropesPos2 = endTile.neighbors [ Util.GetOppositeDirection ( (int)data.Direction ) ].transform.position + Util.GetTileDistance ( Util.GetOppositeDirection ( (int)data.Direction ) );
		else
			ropesPos2 += Util.GetTileDistance ( Util.GetOppositeDirection ( (int)data.Direction ) );

		// Create animations
		Tween t1 = transform.DOMove ( ropesPos1, NumberOfTilesToEdge ( startTile, (int)data.Direction, 1 ) * MOVE_ANIMATION_TIME ) // Move from the start position to the first ropes
			.OnStart ( ( ) =>
			{
				// Mark that the ability is active
				InstanceData.Ability1.IsActive = true;
				GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
			} );
		Tween t2 = transform.DOMove ( ropesPos2, ( NumberOfTilesToEdge ( startTile, (int)data.Direction, 1 ) + NumberOfTilesToEdge ( startTile, Util.GetOppositeDirection ( (int)data.Direction ), 1 ) ) * MOVE_ANIMATION_TIME ); // Move across the arena from the first ropes to the second ropes
		Tween t3 = transform.DOMove ( endTile.transform.position, NumberOfTilesToEdge ( endTile, Util.GetOppositeDirection ( (int)data.Direction ), 1 ) * MOVE_ANIMATION_TIME ) // Move from the second ropes to the end position
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
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t3, true ) );
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
				Hex.Direction direction = hex.Grid.GetOppositeDirection ( (Hex.Direction)i );

				// Get opposite edge tile
				Hex targetHex = GetEdgeTile ( direction );

				// Check if this unit can move to the edge tile
				if ( !returnOnlyJumps && OccupyTileCheck ( targetHex, prerequisite ) )
				{
					// Add as an available move
					MoveList.Add ( new MoveData ( targetHex, prerequisite, MoveData.MoveType.SPECIAL, i ) );
				}
				// Check if this unit can jump the edge tile
				else if ( JumpTileCheck ( targetHex ) && OccupyTileCheck ( targetHex.Neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the unit can be attacked
					if ( InstanceData.Ability1.IsPerkEnabled && targetHex.Tile.CurrentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available attack
						m = new MoveData ( targetHex.Neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL_ATTACK, i, targetHex );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( targetHex.Neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i );
					}

					// Add move to the move list
					MoveList.Add ( m );

					// Find additional jumps
					FindMoves ( targetHex.Neighbors [ i ], m, true );
				}
			}
			// Check for neighboring edge tiles
			else if ( JumpTileCheck ( hex.Neighbors [ i ] ) && hex.Neighbors [ i ].Neighbors [ i ] == null )
			{
				// Get opposite direction
				//int direction = Util.GetOppositeDirection ( i );
				Hex.Direction direction = hex.Grid.GetOppositeDirection ( (Hex.Direction)i );

				// Get opposite edge tile
				Hex targetHex = GetEdgeTile ( direction );

				// Check if this unit can move to the edge tile
				if ( OccupyTileCheck ( targetHex, prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the unit can be attacked
					if ( InstanceData.Ability1.IsPerkEnabled && hex.neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available attack
						m = new MoveData ( targetHex, prerequisite, MoveData.MoveType.SPECIAL_ATTACK, i, hex.neighbors [ i ] );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( targetHex, prerequisite, MoveData.MoveType.SPECIAL, i );
					}

					// Add move to the move list
					MoveList.Add ( m );

					// Find additional jumps
					FindMoves ( targetHex, m, true );
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
