using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Neaera : Leader
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
	///
	/// Neaera Unit Data
	/// 
	/// ID: 4
	/// Name: Neaera
	/// Nickname: The Solar God
	/// Bio: She's the heat too hot for these streets, the light you can’t fight, and answer to the night. It’s Neaera, the God of the Sun, unequivically, universally, #1!
	/// Finishing Move: ???
	/// Role: Leader
	/// Act: Face
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 3
	/// Name: Solar Flare
	/// Description: Neaera creates a blinding solar flare that ejects plasma to immobilize and protect a unit
	/// Type: Command
	/// Duration: 2 Rounds
	/// Cooldown: 8 Rounds
	/// Range: 3 Tile Radius
	/// Tile Object: Solar Flare
	/// Status Effect (Target): Plasma Shield, Blind
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	[SerializeField]
	private TileObject solarFlarePrefab;

	[SerializeField]
	private SpriteRenderer blindPrefab;

	private TileObject currentSolarFlare;

	private const float SOLAR_FLARE_ANIMATION_TIME = 1.75f;
	private const float BLIND_ANIMATION_TIME = 0.5f;
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

		// Check for previous ability cast
		if ( currentSolarFlare != null )
			RemoveSolarFlare ( );

		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.UnitHUD.HideCancelButton ( InstanceData.Ability1 );

		// Clear board
		GM.Grid.ResetTiles ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( );

		// Check for unit
		if ( GM.SelectedCommand.PrimaryTarget.Tile.CurrentUnit != null )
		{
			// Interupt unit
			GM.SelectedCommand.PrimaryTarget.Tile.CurrentUnit.InteruptUnit ( );

			// Apply shield
			GM.SelectedCommand.PrimaryTarget.Tile.CurrentUnit.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.PLASMA_SHIELD, InstanceData.Ability1.Duration, this );
		}
			
		// Create the solar flare
		currentSolarFlare = CreateTileOject ( solarFlarePrefab, GM.SelectedCommand.PrimaryTarget, InstanceData.Ability1.Duration, RemoveSolarFlare );
		currentSolarFlare.Icon.color = Util.TeamColor ( Owner.Team );

		// Create spawning animation
		s.Append ( currentSolarFlare.Icon.transform.DOScale ( Vector3.one * 10, SOLAR_FLARE_ANIMATION_TIME ).From ( ) );
		s.Join ( currentSolarFlare.Icon.DOFade ( 0f, SOLAR_FLARE_ANIMATION_TIME ).From ( ) );

		// Add interval
		s.AppendInterval ( INTERVAL_TIME );
		s.AppendInterval ( BLIND_ANIMATION_TIME );

		// Apply gravity well
		List<Tween> animations = ApplyBlind ( GM.SelectedCommand.PrimaryTarget );
		for ( int i = 0; i < animations.Count; i++ )
			s.Join ( animations [ i ] );

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

	#endregion // Public HeroUnit Override Functions

	#region Protected HeroUnit Override Functions

	protected override void GetCommandTargets ( )
	{
		// Get targets
		GetSolarFlare ( );
	}

	protected override void OnDurationComplete ( AbilityInstanceData ability )
	{
		// Destroy the black hole
		RemoveSolarFlare ( );
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Marks every unoccupied tile within range as available for selection for Solar Flare.
	/// </summary>
	private void GetSolarFlare ( )
	{
		// Get targets within range
		List<Hex> targets = CurrentHex.Range ( InstanceData.Ability1.PerkValue );

		// Check each potential direction
		for ( int i = 0; i < targets.Count; i++ )
		{
			// Check that tile exists
			if ( targets [ i ] == null )
				continue;

			// Check that the tile is occupied by a tile object
			if ( targets [ i ].Tile.CurrentObject != null )
				continue;

			// Add tile as potential target
			targets [ i ].Tile.SetTileState ( TileState.AvailableCommand );	
		}
	}

	/// <summary>
	/// Blinds in any targets within range of the solar flare.
	/// </summary>
	/// <param name="hex"> The hex of the solar flare. </param>
	/// <returns> The list of targets to animate. </returns>
	private List<Tween> ApplyBlind ( Hex hex )
	{
		// Store list of animations
		List<Tween> animations = new List<Tween> ( );

		// Get all tiles within range
		List<Hex> targets = hex.Range ( InstanceData.Ability1.PerkValue );

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

			// Interupt unit
			targets [ i ].Tile.CurrentUnit.InteruptUnit ( );

			// Apply blind
			targets [ i ].Tile.CurrentUnit.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.BLIND, InstanceData.Ability1.Duration, this );

			// Create icon
			SpriteRenderer effect = Instantiate ( blindPrefab, targets [ i ].transform.position, Quaternion.identity, Owner.transform );
			effect.color = Util.AccentColor ( Owner.Team );

			// Create animation
			animations.Add ( effect.transform.DOScale ( Vector3.one * 5, BLIND_ANIMATION_TIME ) );
			animations.Add ( effect.DOFade ( 0, BLIND_ANIMATION_TIME )
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
	/// Removes the Chained status effect from each target adjacent to Zorya.
	/// </summary>
	private void RemoveSolarFlare ( )
	{
		// Check for protected unit
		if ( currentSolarFlare.CurrentHex.Tile.CurrentUnit != null )
			currentSolarFlare.CurrentHex.Tile.CurrentUnit.Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.PLASMA_SHIELD, this );

		// Destroy the flare
		Destroy ( currentSolarFlare.gameObject );

		// Reset flare
		currentSolarFlare = null;
	}

	#endregion // Private Functions
}