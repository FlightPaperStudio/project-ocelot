using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Units
{
	public class Aerio : Leader
	{
		/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
		///
		/// Aerio Unit Data
		/// 
		/// ID: 5
		/// Name: Aerio
		/// Nickname: The Giant God
		/// Bio: Sorcerer in the stars, manipulator of the lightning arts; the colossal pain in your asteroid is here to make a storm on the board. If being big-headed is a 
		///		 sin, then you don’t deserve to win. Get ready! Hold onto your hats and make way for Aerio, the Giant God!
		/// Finishing Move: ???
		/// Role: Leader
		/// Act: Heel
		/// Slots: 1
		/// 
		/// Ability 1
		/// ID: 4
		/// Name: Sublimation
		/// Description: Aerio absorbs energy from nearby units to phase their physical forms into gas
		/// Type: Command
		/// Duration: 3 Rounds
		/// Cooldown: 8 Rounds
		/// Range: 2 Tile Radius
		/// Status Effect (Self): Incorporeal
		/// Status Effect (Target): Incorporeal
		/// 
		/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

		#region Ability Data

		[SerializeField]
		private SpriteRenderer gasPrefab;

		private List<Unit> sublimationTargets = new List<Unit> ( );

		private const float SUBLIMATION_ANIMATION_TIME = 1.5f;
		private const float SUBLIMATION_DELAY = 0.5f;
		private const float ANIMATION_INTERVAL = 0.1f;

		#endregion // Ability Data

		#region Public Unit Override Functions

		public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
		{
			// Find base moves
			base.FindMoves ( hex, prerequisite, returnOnlyJumps );

			// Get sublimation availability
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

			// Clear previous targets
			sublimationTargets.Clear ( );

			// Begin animation
			Sequence s = DOTween.Sequence ( );

			// Add delay
			s.AppendInterval ( ANIMATION_INTERVAL );
			s.AppendInterval ( ANIMATION_INTERVAL );

			// Check each tile within range of the target tile
			List<Hex> tiles = GetSublimation ( GM.SelectedCommand.PrimaryTarget );

			// Check each tile
			for ( int i = 0; i < tiles.Count; i++ )
			{
				// Create gas cloud icon
				SpriteRenderer gas = Instantiate ( gasPrefab, tiles [ i ].transform.position, Quaternion.identity, Owner.transform );

				// Display icon
				Color32 c = Tools.Util.AccentColor ( Owner.Team );
				gas.color = new Color32 ( c.r, c.g, c.b, 125 );
				Tools.Util.OrientSpriteToDirection ( gas, Owner.TeamDirection );

				// Animate status effect
				s.Join ( gas.DOFade ( 0, SUBLIMATION_ANIMATION_TIME ) );
				s.Join ( gas.transform.DOScale ( new Vector3 ( 5, 5, 5 ), SUBLIMATION_ANIMATION_TIME )
					.OnComplete ( ( ) =>
					{
					// Destroy icon
					Destroy ( gas.gameObject );
					} ) );

				// Check for affected target
				if ( tiles [ i ].Tile.CurrentUnit != null )
				{
					// Apply status affect
					tiles [ i ].Tile.CurrentUnit.Status.AddStatusEffect ( Database.StatusEffectDatabase.StatusEffectType.INCORPOREAL, InstanceData.Ability1.Duration, this );

					// Add unit to list
					sublimationTargets.Add ( tiles [ i ].Tile.CurrentUnit );
				}
			}

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
			GetSublimation ( );
		}

		protected override void OnDurationComplete ( AbilityInstanceData ability )
		{
			// Remove status effect from all targets
			for ( int i = 0; i < sublimationTargets.Count; i++ )
				if ( sublimationTargets [ i ] != null )
					sublimationTargets [ i ].Status.RemoveStatusEffect ( Database.StatusEffectDatabase.StatusEffectType.INCORPOREAL, this );

			// Clear list of targets
			sublimationTargets.Clear ( );
		}

		#endregion // Protected HeroUnit Override Functions

		#region Private Functions

		/// <summary>
		/// Marks every tile within range as available for selection for Sublimation.
		/// </summary>
		private void GetSublimation ( )
		{
			// Get targets within range
			List<Hex> targets = CurrentHex.Range ( InstanceData.Ability1.PerkValue );

			// Check each potential target
			for ( int i = 0; i < targets.Count; i++ )
			{
				// Check that tile exists
				if ( targets [ i ] == null )
					continue;

				// Add tile as potential target
				targets [ i ].Tile.SetTileState ( TileState.AvailableCommand );
			}
		}

		/// <summary>
		/// Gets every tile within range that can be affected by Sublimation.
		/// </summary>
		/// <param name="hex"> The targeted tile. </param>
		/// <returns> All tiles within range of the targeted tile. </returns>
		private List<Hex> GetSublimation ( Hex hex )
		{
			// Get all tiles within range
			List<Hex> tiles = hex.Range ( InstanceData.Ability1.PerkValue );
			tiles.Add ( hex );

			// Remove all missing tiles
			tiles.RemoveAll ( x => x == null );

			// Remove all tiles with units immune to abilities
			tiles.RemoveAll ( x => x.Tile.CurrentUnit != null && !x.Tile.CurrentUnit.Status.CanBeAffectedByAbility );

			// Return list of viable targets
			return tiles;
		}

		#endregion // Private Functions
	}
}