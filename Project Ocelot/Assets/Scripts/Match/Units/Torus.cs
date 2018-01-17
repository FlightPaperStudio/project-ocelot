using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Torus : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Run The Ropes
	/// Type: Special Ability
	/// Default Cooldown: 2 Turns
	/// 
	/// Ability 2: Taunt
	/// Type: Command Ability
	/// Default Cooldown: 6 Turns
	/// 
	/// </summary>

	// Ability information
	private Dictionary<Tile, int> tauntTargetDirection = new Dictionary<Tile, int> ( );
	private const int TAUNT_RANGE = 4;

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Run The Ropes moves
		if ( SpecialAvailabilityCheck ( CurrentAbility1, prerequisite ) )
			GetRunTheRopes ( t, prerequisite, returnOnlyJumps );

		// Get Taunt availability
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

		// Check previous moves
		if ( CheckPrequisiteType ( prerequisite ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Marks every tiles available to the Run The Ropes ability.
	/// </summary>
	private void GetRunTheRopes ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.direction );

		// Check each neighboring tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for edge tiles
			if ( t.neighbors [ i ] == null )
			{
				// Get opposite direction
				int direction = Util.GetOppositeDirection ( i );

				// Get opposite edge tile
				Tile torusTile = GetEdgeTile ( t, direction );

				// Check if this unit can move to the edge tile
				if ( !returnOnlyJumps && OccupyTileCheck ( torusTile, prerequisite ) )
				{
					// Add as an available move
					moveList.Add ( new MoveData ( torusTile, prerequisite, MoveData.MoveType.SPECIAL, i ) );
				}
				// Check if this unit can jump the edge tile
				else if ( JumpTileCheck ( torusTile ) && OccupyTileCheck ( torusTile.neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the unit can be attacked
					if ( torusTile.currentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available attack
						m = new MoveData ( torusTile.neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL_ATTACK, i, torusTile );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( torusTile.neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i );
					}

					// Add move to the move list
					moveList.Add ( m );

					// Find additional jumps
					FindMoves ( torusTile.neighbors [ i ], m, true );
				}
			}
			// Check for neighboring edge tiles
			else if ( JumpTileCheck ( t.neighbors [ i ] ) && t.neighbors [ i ].neighbors [ i ] == null )
			{
				// Get opposite direction
				int direction = Util.GetOppositeDirection ( i );

				// Get opposite edge tile
				Tile torusTile = GetEdgeTile ( t, direction );

				// Check if this unit can move to the edge tile
				if ( OccupyTileCheck ( torusTile, prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the unit can be attacked
					if ( t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available attack
						m = new MoveData ( torusTile, prerequisite, MoveData.MoveType.SPECIAL_ATTACK, i, t.neighbors [ i ] );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( torusTile, prerequisite, MoveData.MoveType.SPECIAL, i );
					}

					// Add move to the move list
					moveList.Add ( m );

					// Find additional jumps
					FindMoves ( torusTile, m, true );
				}
			}
		}
	}

	/// <summary>
	/// Navigates the hex grid in a direction until it finds an edge tile and returns it.
	/// </summary>
	private Tile GetEdgeTile ( Tile t, int direction )
	{
		// Check for edge
		if ( t.neighbors [ direction ] == null )
		{
			// Return the edge tile
			return t;
		}
		else
		{
			// Continue searching for the edge tile
			return GetEdgeTile ( t.neighbors [ direction ], direction );
		}
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Get tiles
		Tile startTile;
		Tile endTile = data.Tile;
		if ( data.Prerequisite == null )
			startTile = currentTile;
		else
			startTile = data.Prerequisite.Tile;

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
		Tween t1 = transform.DOMove ( ropesPos1, NumberOfTilesToEdge ( startTile, (int)data.Direction, 1 ) * MOVE_ANIMATION_TIME ); // Move from the start position to the first ropes
		Tween t2 = transform.DOMove ( ropesPos2, ( NumberOfTilesToEdge ( startTile, (int)data.Direction, 1 ) + NumberOfTilesToEdge ( startTile, Util.GetOppositeDirection ( (int)data.Direction ), 1 ) ) * MOVE_ANIMATION_TIME ); // Move across the arena from the first ropes to the second ropes
		Tween t3 = transform.DOMove ( endTile.transform.position, NumberOfTilesToEdge ( endTile, Util.GetOppositeDirection ( (int)data.Direction ), 1 ) * MOVE_ANIMATION_TIME ) // Move from the second ropes to the end position
			.OnComplete ( ( ) =>
			{
				// Start Run The Ropes cooldown
				StartCooldown ( CurrentAbility1, Info.Ability1 );

				// Set unit and tile data
				SetUnitToTile ( data.Tile );
			} );

		// Add animation to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t2, true ) );
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t3, true ) );
	}

	/// <summary>
	/// Calculates the number of tiles between the hero and the edge of the arena in a given direction.
	/// </summary>
	private int NumberOfTilesToEdge ( Tile t, int direction, int count )
	{
		// Check for edge
		if ( t.neighbors [ direction ] != null )
			return NumberOfTilesToEdge ( t.neighbors [ direction ], direction, count + 1 );

		// Return count
		return count;
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

		// Check if a target is within range
		if ( !TauntCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks to see if an enemy unit is within range of and capable of being targeted by the Taunt ability.
	/// Returns true if at least one unit can be targeted by the Taunt ability.
	/// </summary>
	private bool TauntCheck ( )
	{
		// Track potential target
		bool targetFound = false;

		// Check every direction
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Search for target
			if ( GetTaunt ( currentTile.neighbors [ i ], i, 1 ) != null )
			{
				// Mark that a target was found
				targetFound = true;
				break;
			}
		}

		// Return if target was found
		return targetFound;
	}

	/// <summary>
	/// Finds an enemy unit that is within the 2 to 4 tile range and can be moved to an unoccupied tile toward the hero by the Taunt ability.
	/// Only returns the closest available target.
	/// </summary>
	private Tile GetTaunt ( Tile t, int direction, int count )
	{
		// Check tile
		if ( t != null && count <= TAUNT_RANGE )
		{
			// Check target and move location
			if ( t.currentUnit != null && t.currentUnit.owner != owner && t.currentUnit.status.CanBeMoved && OccupyTileCheck ( t.neighbors [ Util.GetOppositeDirection ( direction ) ], null ) )
				return t;
			else
				return GetTaunt ( t.neighbors [ direction ], direction, count + 1 );
		}

		// Return that no target was found
		return null;
	}

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( )
	{
		// Clear the board
		base.StartCommand ( );

		// Clear previous targets
		tauntTargetDirection.Clear ( );

		// Get every Taunt target
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
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
	public override void SelectCommandTile ( Tile t )
	{
		// Pause turn timer
		if ( MatchSettings.turnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Clear board
		GM.board.ResetTiles ( );

		// Store target and destination
		Unit u = t.currentUnit;
		Tile destination = t.neighbors [ Util.GetOppositeDirection ( tauntTargetDirection [ t ] ) ];

		// Interupt target
		u.InteruptUnit ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( )
			.Append ( u.transform.DOMove ( destination.transform.position, MOVE_ANIMATION_TIME ) )
			.OnComplete ( ( ) =>
			{
				// Remove target from previous tile
				t.currentUnit = null;

				// Set the target's new current tile
				u.currentTile = destination;
				destination.currentUnit = u;

				// Start cooldown
				StartCooldown ( CurrentAbility2, Info.Ability2 );

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
}
