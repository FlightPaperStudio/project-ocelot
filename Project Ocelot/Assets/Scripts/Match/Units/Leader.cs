using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Units
{
	public class Leader : HeroUnit
	{
		#region Public Unit Override Functions

		/// <summary>
		/// Calculates all base moves available to a unit.
		/// </summary>
		public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
		{
			// Cleare previous move list
			if ( prerequisite == null )
				MoveList.Clear ( );

			// Check status effects
			if ( Status.CanMove )
			{
				// Check each neighboring tile
				for ( int i = 0; i < hex.Neighbors.Length; i++ )
				{
					// Check if this unit can move to the neighboring tile
					if ( !returnOnlyJumps && OccupyTileCheck ( hex.Neighbors [ i ], prerequisite ) )
					{
						// Track move data
						MoveData move;

						// Check for goal tile
						if ( Owner.Objective.Contains ( hex.Neighbors [ i ] ) )
						{
							// Add as an available move to win
							move = new MoveData ( hex.Neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i );
							move.IsVictoryMove = true;
						}
						else
						{
							// Add as an available move
							move = new MoveData ( hex.Neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i );
						}

						// Add move to the move list
						AddMove ( move );
					}
					// Check if this unit can jump the neighboring tile
					else if ( JumpTileCheck ( hex.Neighbors [ i ] ) && OccupyTileCheck ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite ) )
					{
						// Track move data
						MoveData move;

						// Check if the neighboring unit can be attacked
						if ( AttackTileCheck ( hex.Neighbors [ i ] ) )
						{
							// Add as an available jump
							move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i, null, hex.Neighbors [ i ] );

							// Check for goal
							move.IsVictoryMove = Owner.Objective.Contains ( hex.Neighbors [ i ].Neighbors [ i ] );
						}
						else
						{
							// Add as an available jump
							move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i, hex.Neighbors [ i ], null );

							// Check for goal
							move.IsVictoryMove = Owner.Objective.Contains ( hex.Neighbors [ i ].Neighbors [ i ] );
						}

						// Add move to the move list
						AddMove ( move );

						// Find additional jumps
						FindMoves ( hex.Neighbors [ i ].Neighbors [ i ], move, true );
					}
				}
			}
		}

		#endregion // Public Unit Override Functions

		#region Protected Unit Override Functions

		/// <summary>
		/// Moves the unit to an adjecent tile.
		/// If the adjacent tile is a goal tile, the Leader's team wins the match.
		/// </summary>
		protected override void Move ( MoveData data )
		{
			// Create animation
			base.Move ( data );

			// Check if tile is a goal tile and if tile is the final destination
			if ( Owner.Objective.Contains ( data.Destination ) && data == GM.SelectedMove )
			{
				// Have the player win the match
				GM.WinMatch ( Owner );
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
			if ( Owner.Objective.Contains ( data.Destination ) && data == GM.SelectedMove )
			{
				// Have the player win the match
				GM.WinMatch ( Owner );
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
				GM.LoseMatch ( Owner );
		}

		#endregion // Protected Unit Override Functions
	}
}