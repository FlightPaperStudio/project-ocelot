using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Zorya : Leader
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
	///
	/// Zorya Unit Data
	/// 
	/// ID: 2
	/// Name: Zorya
	/// Nickname: The Binary God
	/// Bio: She’s the raving sun blister but you can’t resist her, she chained down the Doomsday Hound and it’s for you she’s found reason to smash out all the lights 
	///		 tonight. She’s Zorya, the Binary God, and if you’ve been recycling, she’ll favor your round.
	/// Finishing Move: ???
	/// Role: Leader
	/// Act: Antihero
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 1
	/// Name: Chains of Simargl
	/// Description: Zorya summons her sister to chain any adjacent units in place
	/// Type: Command
	/// Duration: 2 Rounds
	/// Cooldown: 8 Rounds
	/// Range: 3 Tile Radius
	/// Tile Object: Zorya Sister
	/// Status Effect (Self): Chained
	/// Status Effect (Target): Chained
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	[SerializeField]
	private TileObject sisterPrefab;

	[SerializeField]
	private SpriteRenderer chainedPrefab;

	private TileObject sister;
	private List<Hex> zoryaTargets = new List<Hex> ( );
	private List<Hex> sisterTargets = new List<Hex> ( );

	private const float SISTER_ANIMATION_TIME = 1.25f;
	private const float CHAIN_ANIMATION_TIME = 0.75f;
	private const float INTERVAL_TIME = 0.3f;

	#endregion // Ability Data

	#region Public Unit Override Functions

	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Find base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Chains of Simargl availability
		InstanceData.Ability1.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability1, prerequisite );
	}

	#endregion // Public Unit Override Functions

	#region Public HeroUnit Override Functions

	public override void ExecuteCommand ( )
	{
		// Execute base command
		base.ExecuteCommand ( );

		// Check for previous ability cast
		if ( Status.HasStatusEffect ( StatusEffectDatabase.StatusEffectType.CHAINED, this ) )
			RemoveZoryaChains ( );
		if ( sister != null )
			RemoveSisterChains ( );

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
		sister = CreateTileOject ( sisterPrefab, GM.SelectedCommand.PrimaryTarget, InstanceData.Ability1.Duration, RemoveSisterChains );
		sister.Icon.color = Util.AccentColor ( Owner.Team );

		// Create spawning animation
		s.Append ( sister.Icon.DOFade ( 0f, SISTER_ANIMATION_TIME ).From ( ) );

		// Apply chains from zorya
		s.AppendInterval ( INTERVAL_TIME );
		SetChainsOfSimargl ( CurrentHex, s, zoryaTargets );

		// Apply chains from sister
		s.AppendInterval ( INTERVAL_TIME );
		SetChainsOfSimargl ( GM.SelectedCommand.PrimaryTarget, s, sisterTargets );

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

	public override void InteruptUnit ( )
	{
		// Remove chains
		if ( Status.HasStatusEffect ( StatusEffectDatabase.StatusEffectType.CHAINED, this ) )
			RemoveZoryaChains ( );
	}

	#endregion // Public HeroUnit Override Functions

	#region Protected HeroUnit Override Functions

	protected override void GetCommandTargets ( )
	{
		// Get targets
		GetChainsOfSimargl ( );
	}

	protected override void OnDurationComplete ( AbilityInstanceData ability )
	{
		// Remove chains from zorya
		RemoveZoryaChains ( );

		// Remove chains from sister
		if ( sister != null )
			RemoveSisterChains ( );
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Marks every unoccupied tile within range as available for selection for Chains of Simargl.
	/// </summary>
	private void GetChainsOfSimargl ( )
	{
		// Get targets within range
		List<Hex> targets = CurrentHex.Range ( InstanceData.Ability1.PerkValue );

		// Check each potential target
		for ( int i = 0; i < targets.Count; i++ )
		{
			// Check that tile exists
			if ( targets [ i ] == null )
				continue;

			// Check that the tile is unoccupied
			if ( targets [ i ].Tile.IsOccupied )
				continue;

			// Add tile as potential target
			targets [ i ].Tile.SetTileState ( TileState.AvailableCommand );
		}
	}

	/// <summary>
	/// Applies the Chained status effect to each target adjacent to a Zorya Sister.
	/// </summary>
	/// <param name="hex"> The hex of a Zorya Sister. </param>
	/// <param name="s"> The animation sequence for Chains of Simargl. </param>
	/// <param name="targetList"> The list of targets for this Zorya Sister. </param>
	private void SetChainsOfSimargl ( Hex hex, Sequence s, List<Hex> targetList )
	{
		// Set delay
		s.AppendInterval ( CHAIN_ANIMATION_TIME );

		// Apply status effect to caster
		if ( hex.Tile.CurrentUnit != null )
			ApplyChains ( hex, s );

		// Check each adjacent hex
		for ( int i = 0; i < hex.Neighbors.Length; i++ )
		{
			// Check for hex
			if ( hex.Neighbors [ i ] == null )
				continue;

			// Check for unit
			if ( hex.Neighbors [ i ].Tile.CurrentUnit == null )
				continue;

			// Check if the unit can be affected
			if ( !hex.Neighbors [ i ].Tile.CurrentUnit.Status.CanBeAffectedByAbility )
				continue;

			// Add target to list
			targetList.Add ( hex.Neighbors [ i ] );

			// Apply status effect
			ApplyChains ( hex.Neighbors [ i ], s );
		}
	}

	/// <summary>
	/// Applies the Chained status effect to a target.
	/// </summary>
	/// <param name="hex"> The hex of the target. </param>
	/// <param name="s"> The animation sequence for Chains of Simargl. </param>
	private void ApplyChains ( Hex hex, Sequence s )
	{
		// Apply status effect
		hex.Tile.CurrentUnit.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.CHAINED, InstanceData.Ability1.Duration, this );

		// Create status effect icon
		SpriteRenderer chain = Instantiate ( chainedPrefab, hex.transform.position, Quaternion.identity, Owner.transform );

		// Display icon
		Color32 c = Util.AccentColor ( Owner.Team );
		chain.color = new Color32 ( c.r, c.g, c.b, 125 );
		Util.OrientSpriteToDirection ( chain, Owner.TeamDirection );

		// Animate status effect
		s.Join ( chain.DOFade ( 0, CHAIN_ANIMATION_TIME ).From ( ) );
		s.Join ( chain.transform.DOScale ( new Vector3 ( 5, 5, 5 ), CHAIN_ANIMATION_TIME ).From ( )
			.OnComplete ( ( ) =>
			{
				// Destroy icon
				Destroy ( chain.gameObject );
			} ) );
	}

	/// <summary>
	/// Removes the Chained status effect from each target adjacent to Zorya.
	/// </summary>
	private void RemoveZoryaChains ( )
	{
		// Remove chains from zorya
		Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.CHAINED, this );

		// Remove chains from each target
		for ( int i = 0; i < zoryaTargets.Count; i++ )
			zoryaTargets [ i ].Tile.CurrentUnit.Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.CHAINED, this );
		zoryaTargets.Clear ( );
	}

	/// <summary>
	/// Removes the Chained status effect from each target adjacent to Zorya's sister.
	/// </summary>
	private void RemoveSisterChains ( )
	{
		// Check for sister
		if ( sister != null )
		{
			// Create animation
			Tween t1 = sister.transform.DOScale ( new Vector3 ( 5, 5, 5 ), KO_ANIMATION_TIME )
				.OnComplete ( ( ) =>
				{
					// Remove sister
					Destroy ( sister.gameObject );
					sister = null;
				} )
				.Pause ( );
			Tween t2 = sister.Icon.DOFade ( 0, MOVE_ANIMATION_TIME )
				.Pause ( );

			// Add animations to queue
			GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
			GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
		}

		// Remove chains from each target
		for ( int i = 0; i < sisterTargets.Count; i++ )
			sisterTargets [ i ].Tile.CurrentUnit.Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.CHAINED, this );
		sisterTargets.Clear ( );
	}

	#endregion // Private Functions
}
