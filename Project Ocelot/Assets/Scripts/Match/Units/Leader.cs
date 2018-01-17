using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Leader : Unit 
{
	/// <summary>
	/// 
	/// Hero Ability Information 
	/// 
	/// Ability 1: Smite
	/// Type: Passive Ability
	/// 
	/// </summary>

	// Hero information
	public Ability ability;
	public AbilitySettings currentAbility;
	public Sprite abilitySprite;

	private void Start ( )
	{
		// Set name
		characterName = NameGenerator.CreateName ( );

		// Set ability information
		ability.Name = "Smite";
		ability.Description = "???";
		ability.Type = 0;
		ability.Duration = 1;
		ability.Cooldown = 0;
		currentAbility = new AbilitySettings ( true, (Ability.AbilityType)ability.Type, ability.Duration, ability.Cooldown );
	}

	/// <summary>
	/// Calculates all base moves available to a unit.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Cleare previous move list
		if ( prerequisite == null )
			moveList.Clear ( );

		// Check status effects
		if ( status.CanMove )
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( owner.direction );

			// Check each neighboring tile
			for ( int i = 0; i < t.neighbors.Length; i++ )
			{
				// Ignore tiles that would allow for backward movement
				if ( i == back.FirstInt || i == back.SecondInt )
					continue;

				// Check if this unit can move to the neighboring tile
				if ( !returnOnlyJumps && OccupyTileCheck ( t.neighbors [ i ], prerequisite ) )
				{
					// Check for goal tile
					if ( owner.startArea.IsGoalTile ( t.neighbors [ i ] ) )
					{
						// Add as an available move to win
						moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.MOVE_TO_WIN, i ) );
					}
					else
					{
						// Add as an available move
						moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i ) );
					}
				}
				// Check if this unit can jump the neighboring tile
				else if ( JumpTileCheck ( t.neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the neighboring unit can be attacked
					if ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
					{
						// Check for goal tile
						if ( owner.startArea.IsGoalTile ( t.neighbors [ i ].neighbors [ i ] ) )
						{
							// Add as an available attack to win
							m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.ATTACK_TO_WIN, i, t.neighbors [ i ] );
						}
						else
						{
							// Add as an available attack
							m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.ATTACK, i, t.neighbors [ i ] );
						}
					}
					else
					{
						// Check for goal tile
						if ( owner.startArea.IsGoalTile ( t.neighbors [ i ].neighbors [ i ] ) )
						{
							// Add as an available jump to win
							m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.JUMP_TO_WIN, i );
						}
						else
						{
							// Add as an available jump
							m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i );
						}
					}

					// Add move to the move list
					moveList.Add ( m );

					// Find additional jumps
					FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
				}
			}
		}
	}

	/// <summary>
	/// Determines how the unit should move based on the Move Data given.
	/// </summary>
	public override void MoveUnit ( MoveData data )
	{
		// Check move data
		switch ( data.Type )
		{
		case MoveData.MoveType.MOVE:
		case MoveData.MoveType.MOVE_TO_WIN:
			Move ( data );
			break;
		case MoveData.MoveType.JUMP:
		case MoveData.MoveType.JUMP_TO_WIN:
			Jump ( data );
			break;
		case MoveData.MoveType.ATTACK:
		case MoveData.MoveType.ATTACK_TO_WIN:
			Jump ( data );
			AttackUnit ( data );
			break;
		}
	}

	/// <summary>
	/// Moves the unit to an adjecent tile.
	/// If the adjacent tile is a goal tile, the Leader's team wins the match.
	/// </summary>
	protected override void Move ( MoveData data )
	{
		// Create animation
		base.Move ( data );

		// Check if tile is a goal tile and if tile is the final destination
		if ( owner.startArea.IsGoalTile ( data.Tile ) && data == GM.selectedMove )
		{
			// Have the player win the match
			GM.WinMatch ( owner );
		}
	}

	/// <summary>
	/// Has the unit jump an adjacent unit.
	/// </summary>
	protected override void Jump ( MoveData data )
	{
		// Create animation
		base.Jump ( data );

		// Check if tile is a goal tile and if tile is the final destination
		if ( owner.startArea.IsGoalTile ( data.Tile ) && data == GM.selectedMove )
		{
			// Have the player win the match
			GM.WinMatch ( owner );
		}
	}

	/// <summary>
	/// Attack and KO this unit.
	/// If the Leader is KOed, then all remaining units are removed from the match.
	/// </summary>
	public override void GetAttacked ( bool usePostAnimationQueue = false )
	{
		// Create animation
		base.GetAttacked ( usePostAnimationQueue );

		// Have the player lose the match
		if ( !usePostAnimationQueue )
			GM.LoseMatch ( owner );
	}
}
