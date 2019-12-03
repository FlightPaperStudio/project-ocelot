using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Units
{
	public class Pacifist : HeroUnit
	{
		/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
		///
		/// Hero 6 Unit Data
		/// 
		/// ID: 14
		/// Name: Hero 6
		/// Nickname: Ghost
		/// Bio: ???
		/// Finishing Move: ???
		/// Role: Support
		/// Slots: 1
		/// 
		/// Ability 1
		/// ID: 21
		/// Name: Ghost
		/// Description: Unable to attack or be attacked by opponents
		/// Type: Passive
		/// Haunted House: Active
		/// 
		/// Ability 2
		/// ID: 22
		/// Name: Poltergeist
		/// Description: Possesses an objectto block an area for a short period
		/// Type: Command
		/// Duration: 2 Turns
		/// Cooldown: 5 Turns
		/// Range: 3 Tile Radius
		/// 
		/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

		#region Ability Data

		[SerializeField]
		private TileObject obstructionPrefab;

		private int hauntedHouseStack = 0;
		private List<TileObject> currentDebris = new List<TileObject> ( );

		private const float OBSTRUCTION_ANIMATION_TIME = 0.75f;

		#endregion // Ability Data

		#region Public Unit Override Functions

		public override void InitializeInstance ( Match.GameManager gm, int instanceID, UnitSettingData settingData )
		{
			// Initialize ability data
			base.InitializeInstance ( gm, instanceID, settingData );

			// Apply incorporeal status effect
			if ( InstanceData.Ability1.IsEnabled )
				Status.AddStatusEffect ( Database.StatusEffectDatabase.StatusEffectType.INCORPOREAL, StatusEffects.PERMANENT_EFFECT, this );
		}

		/// <summary>
		/// Calculates all base moves available to a unit without marking any potential captures.
		/// </summary>
		public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
		{
			// Check for ghost ability
			if ( InstanceData.Ability1.IsEnabled )
			{
				// Cleare previous move list
				if ( prerequisite == null )
					MoveList.Clear ( );

				// Check status effects
				if ( Status.CanMove )
				{
					// Store which tiles are to be ignored
					//IntPair back = GetBackDirection ( Owner.TeamDirection );

					// Check each neighboring tile
					for ( int i = 0; i < hex.Neighbors.Length; i++ )
					{
						// Ignore tiles that would allow for backward movement
						//if ( i == back.FirstInt || i == back.SecondInt )
						//	continue;

						// Check if this unit can move to the neighboring tile
						if ( !returnOnlyJumps && OccupyTileCheck ( hex.Neighbors [ i ], prerequisite ) )
						{
							// Add as an available move
							AddMove ( new MoveData ( hex.Neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i ) );
						}
						// Check if this unit can jump the neighboring tile
						else if ( JumpTileCheck ( hex.Neighbors [ i ] ) && OccupyTileCheck ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite ) )
						{
							// Add as an available jump
							MoveData move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i, hex.Neighbors [ i ], null );
							AddMove ( move );

							// Find additional jumps
							FindMoves ( hex.Neighbors [ i ].Neighbors [ i ], move, true );
						}
					}
				}
			}
			else
			{
				// Return base moves
				base.FindMoves ( hex, prerequisite, returnOnlyJumps );
			}

			// Get obstruction availability
			InstanceData.Ability2.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability2, prerequisite );
		}

		/// <summary>
		/// Determines if this unit can be attaced by another unit.
		/// Always returns false since this unit's Pacifist Ability prevents it from being attacked.
		/// </summary>
		//public override bool UnitAttackCheck ( Unit attacker, bool friendlyFire = false )
		//{
		//	// Prevent any attacks with the Ghost ability
		//	if ( PassiveAvailabilityCheck ( InstanceData.Ability1, null ) )
		//		return false;

		//	// Return normal values if the Ghost ability is disabled
		//	return base.UnitAttackCheck ( attacker );
		//}

		public override void Assist ( )
		{
			// Track stats
			base.Assist ( );

			// Add to haunted house perk
			if ( InstanceData.Ability1.IsEnabled && InstanceData.Ability1.IsPerkEnabled )
				hauntedHouseStack++;
		}

		#endregion // Public Unit Override Functions

		#region Public HeroUnit Override Functions

		public override void ExecuteCommand ( )
		{
			// Execute base command
			base.ExecuteCommand ( );

			// Check for previous possessions
			if ( currentDebris.Count > 0 )
			{
				// Remove previous possessions
				for ( int i = 0; i < currentDebris.Count; i++ )
					if ( currentDebris [ i ] != null )
						DestroyTileObject ( currentDebris [ i ] );
			}

			// Reset data
			currentDebris.Clear ( );

			// Pause turn timer
			if ( Match.MatchSettings.TurnTimer )
				GM.UI.timer.PauseTimer ( );

			// Hide cancel button
			GM.UI.UnitHUD.HideCancelButton ( InstanceData.Ability2 );

			// Clear board
			GM.Grid.ResetTiles ( );

			// Begin animation
			Sequence s = DOTween.Sequence ( );

			// Add each target to the animation
			for ( int i = 0; i < GM.SelectedCommand.Targets.Count; i++ )
			{
				// Create the debris
				TileObject debris = CreateTileOject ( obstructionPrefab, GM.SelectedCommand.Targets [ i ], InstanceData.Ability2.Duration, PoltergeistDurationComplete );

				// Add debris to the list of targets
				currentDebris.Add ( debris );

				// Create spawning animation
				s.Append ( debris.Icon.DOFade ( 0f, OBSTRUCTION_ANIMATION_TIME ).From ( ) );
			}

			// Complete the animation
			s.OnComplete ( ( ) =>
			{
				// Reset haunted house
				if ( InstanceData.Ability1.IsEnabled && InstanceData.Ability1.IsPerkEnabled )
					hauntedHouseStack = 0;

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

		protected override CommandData SetCommandData ( )
		{
			// Check for Haunted House perk
			if ( InstanceData.Ability1.IsEnabled && InstanceData.Ability1.IsPerkEnabled )
			{
				// Set how many objects can be possessed
				return new CommandData ( this, 1, 1 + ( hauntedHouseStack / 2 ) );
			}
			else
			{
				// Set that only one object can be possessed
				return new CommandData ( this, 1 );
			}
		}

		protected override void GetCommandTargets ( )
		{
			// Get targets
			GetPoltergeist ( );
		}

		#endregion // Protected HeroUnit Override Functions

		#region Private Functions

		/// <summary>
		/// Marks every unoccupied tile within range as available for selection for Poltergeist.
		/// </summary>
		private void GetPoltergeist ( )
		{
			// Get targets within range
			List<Hex> targets = CurrentHex.Range ( InstanceData.Ability2.PerkValue );

			// Check each potential target
			for ( int i = 0; i < targets.Count; i++ )
			{
				// Check that tile exists
				if ( targets [ i ] == null )
					continue;

				// Check that the tile is unoccupied
				if ( targets [ i ].Tile.IsOccupied )
					continue;

				// Check for existing selections
				if ( targets [ i ].Tile.State == TileState.SelectedCommand )
					continue;

				// Add tile as potential target
				targets [ i ].Tile.SetTileState ( TileState.AvailableCommand );
			}
		}

		/// <summary>
		/// Delegate for when the duration of the tile object for Obstruction expires.
		/// </summary>
		private void PoltergeistDurationComplete ( )
		{
			// Check for debris
			if ( currentDebris.Count > 0 )
			{
				// Track debris
				bool isFirst = true;

				// Get each possessed object
				foreach ( TileObject debris in currentDebris )
				{
					// Create animation
					Tween t = debris.Icon.DOFade ( 0f, OBSTRUCTION_ANIMATION_TIME )
						.OnComplete ( ( ) =>
						{
						// Remove obstruction from player data
						Owner.TileObjects.Remove ( debris );

						// Remove obstruction from the board
						debris.CurrentHex.Tile.CurrentObject = null;

						// Remove obstruction
						Destroy ( debris.gameObject );
						} );

					// Add animation to queue
					GM.AnimationQueue.Add ( new Match.GameManager.TurnAnimation ( t, isFirst ) );
					isFirst = false;
				}

				// Clear debris
				currentDebris.Clear ( );
			}

		}

		#endregion // Private Functions
	}
}