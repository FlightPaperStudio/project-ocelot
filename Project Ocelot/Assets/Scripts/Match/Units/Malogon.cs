using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Units
{
	public class Malogon : Leader
	{
		/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
		///
		/// Malogon Unit Data
		/// 
		/// ID: 3
		/// Name: Malogon
		/// Nickname: The Nebula God
		/// Bio: Its greed is legendary. Its need is necessary. The creator of mass and starlight seeks souls to fill the empty night! It’s Malogon, God of Nebulas, and if 
		///		 you get caught in it’s grip, "see ya wouldn’t wanna be ya!"
		/// Finishing Move: ???
		/// Role: Leader
		/// Act: Antihero
		/// Slots: 1
		/// 
		/// Ability 1
		/// ID: 2
		/// Name: Cosmic Ray
		/// Description: Malogon uses a blast of cosmic radiation to temporarily disable nearby units
		/// Type: Command
		/// Duration: 3 Rounds
		/// Cooldown: 8 Rounds
		/// Range: 4 Tile Radius
		/// Status Effect (Target): Radiation Poisoning
		/// 
		/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

		#region Private Classes

		private class EffectData
		{
			public Unit Unit;
			public StatusEffects.Effect Effect;
		}

		#endregion // Private Classes

		#region Ability Data

		[SerializeField]
		private SpriteRenderer poisonPrefab;

		[SerializeField]
		private SpriteRenderer energyPrefab;

		private List<EffectData> removedEffects = new List<EffectData> ( );

		private const float RADIATION_ANIMATION_TIME = 0.35f;
		private const float POISON_ANIMATION_TIME = 0.75f;
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

			// Pause turn timer
			if ( Match.MatchSettings.TurnTimer )
				GM.UI.timer.PauseTimer ( );

			// Hide cancel button
			GM.UI.UnitHUD.HideCancelButton ( InstanceData.Ability1 );

			// Clear board
			GM.Grid.ResetTiles ( );

			// Begin animation
			Sequence s = DOTween.Sequence ( );

			// Get area of effect
			List<Hex> targets = GetComsicRayAOE ( System.Array.IndexOf ( CurrentHex.Neighbors, GM.SelectedCommand.PrimaryTarget ) );

			// Create animation for all targets within range
			for ( int distance = 1; distance <= InstanceData.Ability1.PerkValue; distance++ )
			{
				// Track whether or not the animation needs to be appended
				bool isFirst = true;

				// Get the current wave
				for ( int i = 0; i < targets.Count; i++ )
				{
					// Check distance for the wave
					if ( targets [ i ].Distance ( CurrentHex ) == distance )
					{
						// Create radiation icon
						SpriteRenderer energy = Instantiate ( energyPrefab, targets [ i ].transform.position, Quaternion.identity, Owner.transform );
						energy.color = Tools.Util.AccentColor ( Owner.Team );

						// Create the animation
						Tween t = energy.DOFade ( 0, RADIATION_ANIMATION_TIME ).From ( )
							.OnComplete ( ( ) =>
							{
							// Remove icon
							Destroy ( energy.gameObject );
							} );

						// Check if first animation of the wave
						if ( isFirst )
						{
							// Add animation
							s.Append ( t );
							isFirst = false;
						}
						else
						{
							// Add animation
							s.Join ( t );
						}
					}
				}
			}

			// Add interval
			s.AppendInterval ( INTERVAL_TIME );
			s.AppendInterval ( POISON_ANIMATION_TIME );

			// Apply poison
			List<Tween> animations = ApplyPoison ( targets );
			for ( int i = 0; i < animations.Count; i++ )
				s.Join ( animations [ i ] );

			// Complete the animation
			s.OnComplete ( ( ) =>
			{
				// Start cooldown
				StartCooldown ( InstanceData.Ability1 );

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

		protected override void GetCommandTargets ( )
		{
			// Get targets
			GetCosmicRayDirection ( );
		}

		#endregion // Protected HeroUnit Override Functions

		#region Private Functions

		/// <summary>
		/// Marks every adjacent tile as available for selection for a direction for Cosmic Ray.
		/// </summary>
		private void GetCosmicRayDirection ( )
		{
			// Check each potential direction
			for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
			{
				// Check that tile exists
				if ( CurrentHex.Neighbors [ i ] == null )
					continue;

				// Add tile as potential target
				CurrentHex.Neighbors [ i ].Tile.SetTileState ( TileState.AvailableCommand );
			}
		}

		/// <summary>
		/// Gets the area of effect of Cosmic Ray.
		/// </summary>
		/// <param name="direction"> The direction of cosmic ray. </param>
		/// <returns> The list of targeted tiles. </returns>
		private List<Hex> GetComsicRayAOE ( int direction )
		{
			// Store any targets
			List<Hex> targets = new List<Hex> ( );

			// Get neighboring tiles
			int left = direction - 1 < 0 ? 0 : direction - 1;
			int right = direction + 1 >= CurrentHex.Neighbors.Length ? CurrentHex.Neighbors.Length - 1 : direction + 1;

			// Add left line
			for ( int i = 0; i < InstanceData.Ability1.PerkValue; i++ )
				if ( CurrentHex.Neighbors [ left ] != null && CurrentHex.Neighbors [ left ].Neighbor ( (Hex.Direction)direction, i ) != null )
					targets.Add ( CurrentHex.Neighbors [ left ].Neighbor ( (Hex.Direction)direction, i ) );

			// Add middle line
			for ( int i = 0; i < InstanceData.Ability1.PerkValue; i++ )
				if ( CurrentHex.Neighbors [ direction ] != null && CurrentHex.Neighbors [ direction ].Neighbor ( (Hex.Direction)direction, i ) != null )
					targets.Add ( CurrentHex.Neighbors [ direction ].Neighbor ( (Hex.Direction)direction, i ) );

			// Add right line
			for ( int i = 0; i < InstanceData.Ability1.PerkValue; i++ )
				if ( CurrentHex.Neighbors [ right ] != null && CurrentHex.Neighbors [ right ].Neighbor ( (Hex.Direction)direction, i ) != null )
					targets.Add ( CurrentHex.Neighbors [ right ].Neighbor ( (Hex.Direction)direction, i ) );

			// Remove immune units
			targets.RemoveAll ( x => x.Tile.CurrentUnit != null && !x.Tile.CurrentUnit.Status.CanBeAffectedByAbility );

			// Return area
			return targets;
		}

		/// <summary>
		/// Poisons any targets within range of the cosmic ray.
		/// </summary>
		/// <param name="t"> The hex of the cosmic ray. </param>
		/// <returns> The list of targets to animate. </returns>
		private List<Tween> ApplyPoison ( List<Hex> targets )
		{
			// Store list of animations
			List<Tween> animations = new List<Tween> ( );

			// Apply in each direction
			for ( int i = 0; i < targets.Count; i++ )
			{
				// Check for tile
				if ( targets [ i ] == null )
					continue;

				// Check if tile is occupied
				if ( targets [ i ].Tile.CurrentUnit == null )
					continue;

				// Check if unit is immune
				if ( !targets [ i ].Tile.CurrentUnit.Status.CanBeAffectedByAbility )
					continue;

				// Apply poison to unit
				ApplyPoison ( targets [ i ].Tile.CurrentUnit );

				// Create icon
				SpriteRenderer effect = Instantiate ( poisonPrefab, targets [ i ].transform.position, Quaternion.identity, Owner.transform );
				effect.color = Tools.Util.AccentColor ( Owner.Team );

				// Create animation
				animations.Add ( effect.transform.DOScale ( Vector3.one * 5, POISON_ANIMATION_TIME ) );
				animations.Add ( effect.DOFade ( 0, POISON_ANIMATION_TIME )
					.OnComplete ( ( ) =>
					{
					// Destroy icon
					Destroy ( effect.gameObject );
					} ) );
			}

			// Return the list of animations
			return animations;
		}

		/// <summary>
		/// Applies the poison status effect to a unit.
		/// </summary>
		/// <param name="unit"> The targeted unit. </param>
		private void ApplyPoison ( Unit unit )
		{
			//// Check for permanent status effects
			//for ( int i = 0; i < unit.Status.Effects.Count; i++ )
			//{
			//	// Check if effect is permanent
			//	if ( unit.Status.Effects [ i ].Duration == StatusEffects.PERMANENT_EFFECT )
			//	{
			//		// Add effect to list
			//		removedEffects.Add ( new EffectData
			//		{
			//			Unit = unit,
			//			Effect = unit.Status.Effects [ i ]
			//		} );
			//	}
			//}

			//// Remove all status effects
			//unit.Status.ClearStatusEffects ( );

			// Interupt unit
			unit.InteruptUnit ( );

			// Apply poison
			unit.Status.AddStatusEffect ( Database.StatusEffectDatabase.StatusEffectType.RADIATION_POISONING, InstanceData.Ability1.Duration, this );
		}

		#endregion // Private Functions
	}
}