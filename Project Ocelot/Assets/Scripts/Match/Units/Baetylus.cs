using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Baetylus : Leader
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
	///
	/// Baetylus Unit Data
	/// 
	/// ID: 7
	/// Name: Baetylus
	/// Nickname: The Stone God
	/// Bio: She busts through the asteroid cluster, she skates on your planet’s face and leaves a crater in her wake. It’s the meteor shower of terror, the blinding 
	///		 comet and the big bang! It’s Baetylus, the God of Stone, and she's ready to take the throne!
	/// Finishing Move: ???
	/// Role: Leader
	/// Act: Heel
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 6
	/// Name: Meteor Smash
	/// Description: Baetylus summons a meteor that crashes into the arena to send any nearby units flying
	/// Type: Command
	/// Duration: 2 Rounds
	/// Cooldown: 8 Rounds
	/// Range: 2 Tile Radius
	/// Status Effect (Target): Stunned
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	[SerializeField]
	private SpriteRenderer meteorPrefab;

	private const float METEOR_ANIMATION_TIME = 0.75f;
	private const float PUSH_BACK_ANIMATION_TIME = 0.2f;
	private const float METEOR_START_POSITION = 15f;

	#endregion // Ability Data

	#region Public Unit Override Functions

	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Find base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Meteor Smash availability
		InstanceData.Ability1.IsAvailable = CommandAvailabilityCheck ( InstanceData.Ability1, prerequisite );
	}

	#endregion // Public Unit Override Functions

	#region Public HeroUnit Override Functions

	public override void ExecuteCommand ( )
	{
		// Execute base command
		base.ExecuteCommand ( );

		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability1 );

		// Clear board
		GM.Grid.ResetTiles ( );

		// Begin animation
		Sequence s = DOTween.Sequence ( );

		// Create the meteor
		SpriteRenderer meteor = Instantiate ( meteorPrefab, GM.SelectedCommand.PrimaryTarget.transform.position, Quaternion.identity, Owner.transform );
		meteor.color = Util.TeamColor ( Owner.Team );

		// Create meteor animation
		s.Append ( meteor.gameObject.transform.DOMove ( meteor.transform.position + ( Vector3.up * METEOR_START_POSITION ), METEOR_ANIMATION_TIME ).From ( ) );
		s.Append ( meteor.gameObject.transform.DOScale ( new Vector3 ( 5, 5, 5 ), PUSH_BACK_ANIMATION_TIME ) );
		s.Join ( meteor.DOFade ( 0f, PUSH_BACK_ANIMATION_TIME ) )
			.OnComplete ( ( ) =>
			{
				// Destroy meteor
				Destroy ( meteor.gameObject );
			} );

		// Apply gravity well
		List<Tween> animations = ApplyMeteorSmash ( GM.SelectedCommand.PrimaryTarget );
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
		GetMeteorSmash ( );
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Marks every unoccupied tile within range as available for selection for Meteor Smash.
	/// </summary>
	private void GetMeteorSmash ( )
	{
		// Get targets within range
		List<Hex> targets = CurrentHex.Range ( InstanceData.Ability1.PerkValue );

		// Check each potential direction
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
	/// Pushes back any targets within range of the meteor.
	/// </summary>
	/// <param name="hex"> The hex of the meteor. </param>
	/// <returns> The list of targets to animate. </returns>
	private List<Tween> ApplyMeteorSmash ( Hex hex )
	{
		// Store list of animations
		List<Tween> animations = new List<Tween> ( );

		// Apply in each direction
		for ( int i = 0; i < hex.Neighbors.Length; i++ )
		{
			// Check for hex
			if ( hex.Neighbors [ i ] == null )
				continue;

			// Check if hex is occupied
			if ( !hex.Neighbors [ i ].Tile.IsOccupied )
				continue;

			// Check if hex is occupied by a unit or tile object
			if ( hex.Neighbors [ i ].Tile.CurrentUnit != null )
			{
				// Get destination
				Hex destination = GetPushBackHex ( hex.Neighbors [ i ].Neighbors [ i ], i, hex.Neighbors [ i ] );

				// Get unit
				Unit unit = hex.Neighbors [ i ].Tile.CurrentUnit;

				// Interupt unit
				unit.InteruptUnit ( );

				// Set unit to hex
				hex.Neighbors [ i ].Tile.CurrentUnit = null;
				unit.CurrentHex = destination;
				destination.Tile.CurrentUnit = unit;

				// Apply status effect
				unit.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.STUNNED, InstanceData.Ability1.Duration, this );

				// Push unit to destination
				Tween t = unit.transform.DOMove ( destination.transform.position, PUSH_BACK_ANIMATION_TIME * hex.Neighbors [ i ].Distance ( destination ) );

				// Add animation to list
				animations.Add ( t );
			}
			else if ( hex.Neighbors [ i ].Tile.CurrentObject != null && hex.Neighbors [ i ].Tile.CurrentObject.CanBeMoved )
			{
				// Get destination
				Hex destination = GetPushBackHex ( hex.Neighbors [ i ].Neighbors [ i ], i, hex.Neighbors [ i ] );

				// Get object
				TileObject obj = hex.Neighbors [ i ].Tile.CurrentObject;

				// Set object to hex
				hex.Neighbors [ i ].Tile.CurrentObject = null;
				obj.CurrentHex = destination;
				destination.Tile.CurrentObject = obj;

				// Push object to destination
				Tween t = obj.transform.DOMove ( destination.transform.position, PUSH_BACK_ANIMATION_TIME * hex.Neighbors [ i ].Distance ( destination ) );

				// Add animation to list
				animations.Add ( t );
			}
		}

		// Return the list of animations
		return animations;
	}

	/// <summary>
	/// Gets the destination for infinite push back.
	/// </summary>
	/// <param name="current"> The current hex being checked as a potential destination. </param>
	/// <param name="direction"> The direction of the push back. </param>
	/// <param name="destination"> The current valid destination. </param>
	/// <returns> The valid destination. </returns>
	private Hex GetPushBackHex ( Hex current, int direction, Hex destination )
	{
		// Check if hex exists
		if ( current == null )
			return destination;

		// Check if hex is unoccupied
		if ( !current.Tile.IsOccupied )
			return GetPushBackHex ( current.Neighbors [ direction ], direction, current );

		// Check for incorporeal unit
		if ( current.Tile.CurrentUnit != null && current.Tile.CurrentUnit.Status.HasStatusEffect ( StatusEffectDatabase.StatusEffectType.INCORPOREAL ) )
			return GetPushBackHex ( current.Neighbors [ direction ], direction, destination );

		// Return that the destination has been reached
		return destination;
	}

	#endregion // Private Functions
}
