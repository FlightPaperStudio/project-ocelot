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
		ability.name = "Smite";
		ability.description = "???";
		ability.type = 0;
		ability.duration = 1;
		ability.cooldown = 0;
		currentAbility = new AbilitySettings ( true, (Ability.AbilityType)ability.type, ability.duration, ability.cooldown );
	}

	/// <summary>
	/// Calculates all base moves available to a unit.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Cleare previous move list
		if ( !returnOnlyJumps )
			moveList.Clear ( );

		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( team.direction );

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
				if ( team.startArea.IsGoalTile ( t.neighbors [ i ] ) )
				{
					// Add as an available move to win
					moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.MoveToWin, i ) );
				}
				else
				{
					// Add as an available move
					moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.Move, i ) );
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
					if ( team.startArea.IsGoalTile ( t.neighbors [ i ].neighbors [ i ] ) )
					{
						// Add as an available attack to win
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.AttackToWin, i, t.neighbors [ i ] );
					}
					else
					{
						// Add as an available attack
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.Attack, i, t.neighbors [ i ] );
					}
				}
				else
				{
					// Check for goal tile
					if ( team.startArea.IsGoalTile ( t.neighbors [ i ].neighbors [ i ] ) )
					{
						// Add as an available jump to win
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.JumpToWin, i );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.Jump, i );
					}
				}

				// Add move to the move list
				moveList.Add ( m );

				// Find additional jumps
				FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
			}
		}
	}

	/// <summary>
	/// Determines how the unit should move based on the Move Data given.
	/// </summary>
	public override void MoveUnit ( MoveData data )
	{
		// Check move data
		switch ( data.type )
		{
		case MoveData.MoveType.Move:
		case MoveData.MoveType.MoveToWin:
			Move ( data );
			break;
		case MoveData.MoveType.Jump:
		case MoveData.MoveType.JumpToWin:
			Jump ( data );
			break;
		case MoveData.MoveType.Attack:
		case MoveData.MoveType.AttackToWin:
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

		// Check if tile is a goal tile
		if ( team.startArea.IsGoalTile ( data.tile ) )
		{
			// Have the player win the match
			GM.WinMatch ( team );
		}
	}

	/// <summary>
	/// Has the unit jump an adjacent unit.
	/// </summary>
	protected override void Jump ( MoveData data )
	{
		// Create animation
		base.Jump ( data );

		// Check if tile is a goal tile
		if ( team.startArea.IsGoalTile ( data.tile ) )
		{
			// Have the player win the match
			GM.WinMatch ( team );
		}
	}

	/// <summary>
	/// Attack and K.O. this unit.
	/// If the Leader is K.O.-ed, then all remaining units are removed from the match.
	/// </summary>
	public override void GetAttacked ( bool lostMatch = false )
	{
		// Create animation
		base.GetAttacked ( lostMatch );

		// Have the player lose the match
		if ( !lostMatch )
			GM.LoseMatch ( team );
	}
}
