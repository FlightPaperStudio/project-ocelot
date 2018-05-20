using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MindControl : HeroUnit
{
	/// <summary>
	///
	/// Hero 3 Unit Data
	/// 
	/// ID: 11
	/// Name: Hero 3
	/// Nickname: Mind Control
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Offense
	/// Slots: 1
	/// 
	/// Ability 1
	/// ID: 15
	/// Name: Mind Control
	/// Description: Attacked opponents can be controlled for a short period before they are eventually KO'd
	/// Type: Passive
	/// Duration: 3 Turns
	/// 
	/// Ability 2
	/// ID: 16
	/// Name: Clone
	/// Description: Creates a temporary clone of itself for an assist
	/// Type: Special
	/// Duration: 1 Turn
	/// Cooldown: 4 Turns
	/// Continue Movement: Active
	/// 
	/// </summary>

	// Ability information
	//private class MindControlledUnit
	//{
	//	public Unit unit;
	//	public Player originalOwner;
	//	public int duration;

	//	public MindControlledUnit ( Unit _unit, Player _originalOwner, int _duration )
	//	{
	//		unit = _unit;
	//		originalOwner = _originalOwner;
	//		duration = _duration;
	//	}
	//}
	//private List<MindControlledUnit> mindControlledUnits = new List<MindControlledUnit> ( );
	private List<Unit> mindControlledUnits = new List<Unit> ( );
	public SpriteRenderer cloneDisplayPrefab;
	private SpriteRenderer currentCloneDisplay;
	private const float MIND_CONTROL_ANIMATION_TIME = 0.75f;
	private const string MIND_CONTROL_STATUS_PROMPT = "Mind Control";

	#region Public Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves available.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Clone Assist moves
		if ( SpecialAvailabilityCheck ( InstanceData.Ability2, prerequisite ) )
			GetCloneAssist ( t, prerequisite );
	}

	/// <summary>
	/// Attack and KO this unit.
	/// Call this function on the unit being attacked.
	/// This function builds the animation queue from the move data.
	/// </summary>
	public override void GetAttacked ( bool usePostAnimationQueue = false )
	{
		// Check if post-animation queue is being used to prevent animation queue sync issues and check if the Mind Control ability is active
		if ( !usePostAnimationQueue && InstanceData.Ability1.IsEnabled )
		{
			// Remove all Mind Controlled units
			DeactivateMindControl ( );
		}

		// Get KO'd
		base.GetAttacked ( usePostAnimationQueue );
	}

	#endregion // Public Unit Override Functions

	#region Protected Unit Override Functions

	/// <summary>
	/// Attacks the adjacent unit.
	/// Call this function on the attacking unit.
	/// This function builds the animation queue from the move data.
	/// </summary>
	protected override void AttackUnit ( MoveData data )
	{
		// Check Mind Control conditions
		if ( PassiveAvailabilityCheck ( InstanceData.Ability1, data ) )
		{
			// Mind Control the opponent
			foreach ( Tile t in data.Attacks )
			{
				// Interupt unit
				t.currentUnit.InteruptUnit ( );

				// Activate Mind Control
				ActivateMindControl ( t.currentUnit );
			}
		}
		else
		{
			// Attack the unit as normal
			base.AttackUnit ( data );
		}
	}

	#endregion // Protected Unit Override Functions

	#region Protected HeroUnit Override Functions

	/// <summary>
	/// Checks if the hero is capable of using a passive ability.
	/// Returns true if the passive ability is available.
	/// </summary>
	protected override bool PassiveAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.PassiveAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check the opponent
		foreach ( Tile t in prerequisite.Attacks )
		{
			// Check for Leader
			if ( t.currentUnit is Leader )
				return false;

			// Check for Hero 6's Ghost ability
			if ( t.currentUnit is Pacifist )
			{
				HeroUnit h = t.currentUnit as HeroUnit;
				if ( h.InstanceData.Ability1.IsEnabled )
					return false;
			}
		}

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if the hero is capable of using a special ability.
	/// Returns true if the special ability is available.
	/// </summary>
	protected override bool SpecialAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.SpecialAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check previous moves
		if ( CheckPrequisiteType ( prerequisite ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected override void UseSpecial ( MoveData data )
	{
		// Create clone
		currentCloneDisplay = Instantiate ( cloneDisplayPrefab, owner.transform );
		currentCloneDisplay.transform.position = data.Tile.neighbors [ Util.GetOppositeDirection ( (int)data.Direction ) ].neighbors [ Util.GetOppositeDirection ( (int)data.Direction ) ].transform.position;
		Color32 c = Util.TeamColor ( owner.Team );
		currentCloneDisplay.color = new Color32 ( c.r, c.g, c.b, 150 );
		Util.OrientSpriteToDirection ( currentCloneDisplay, owner.TeamDirection );
		currentCloneDisplay.gameObject.SetActive ( false );

		// Create animations
		Tween t1 = currentCloneDisplay.transform.DOMove ( data.Tile.neighbors [ Util.GetOppositeDirection ( (int)data.Direction ) ].transform.position, MOVE_ANIMATION_TIME )
			.OnStart ( ( ) =>
			{
				// Display clone
				currentCloneDisplay.gameObject.SetActive ( true );
			} );
		Tween t2 = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME * 2 )
			.OnComplete ( ( ) =>
			{
				// Start teleport cooldown
				StartCooldown ( InstanceData.Ability2 );

				// Set unit and tile data
				SetUnitToTile ( data.Tile );
			} );
		Tween t3 = currentCloneDisplay.transform.DOScale ( new Vector3 ( 5, 5, 5 ), KO_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Delete the clone
				Destroy ( currentCloneDisplay.gameObject );
				currentCloneDisplay = null;
			} );
		Tween t4 = currentCloneDisplay.DOFade ( 0, MOVE_ANIMATION_TIME );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t3, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t4, false ) );
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Marks every tile available to the Clone Assist ability.
	/// </summary>
	private void GetCloneAssist ( Tile t, MoveData prerequisite )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.TeamDirection );

		// Check each neighboring tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check if this unit can jump the unoccupied neighboring tile
			if ( OccupyTileCheck ( t.neighbors [ i ], prerequisite ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
			{
				// Add as an available jump
				MoveData m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i );

				// Add move to the move list
				MoveList.Add ( m );

				// Find additional jumps
				FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
			}
		}
	}

	/// <summary>
	/// Activates the Mind Control ability. This adds an enemy unit to the player's team.
	/// This function builds the animation queue.
	/// </summary>
	private void ActivateMindControl ( Unit u )
	{
		// Create animation
		Tween t = u.sprite.DOColor ( Util.TeamColor ( owner.Team ), MIND_CONTROL_ANIMATION_TIME )
			.OnStart ( ( ) =>
			{
				// Check for pre-existing Mind Control
				if ( u.Status.effects.Exists ( match => match.info.icon == InstanceData.Ability1.Icon && match.info.text == MIND_CONTROL_STATUS_PROMPT ) )
				{
					// Remove pre-existing Mind Control
					MindControl original = u.Status.effects.Find ( match => match.info.icon == InstanceData.Ability1.Icon && match.info.text == MIND_CONTROL_STATUS_PROMPT ).info.caster as MindControl;
					original.RemoveMindControlledUnit ( u );
				}

				// Check for unit being Mind Controlled is Hero 3
				if ( u is MindControl )
				{
					// Remove all Mind Controlled units by this hero
					MindControl hero3 = u as MindControl;
					hero3.DeactivateMindControl ( );
				}
			} )
			.OnComplete ( ( ) =>
			{
				// Store unit
				//mindControlledUnits.Add ( new MindControlledUnit ( u, u.owner, currentAbility1.duration ) );

				// Remove unit from unit's player's team
				u.owner.UnitInstances.Remove ( u );
				if ( u.owner.standardKOdelegate != null )
					u.koDelegate -= u.owner.standardKOdelegate;

				// Add unit to player's team
				owner.UnitInstances.Add ( u );
				u.owner = owner;
				if ( owner.standardKOdelegate != null )
					u.koDelegate += owner.standardKOdelegate;

				// Apply status effect
				u.Status.AddStatusEffect ( InstanceData.Ability1.Icon, MIND_CONTROL_STATUS_PROMPT, this, InstanceData.Ability1.Duration );

				// Add KO delegate
				//u.koDelegate += MindControlKO;

				// Face unit in correct direction
				Util.OrientSpriteToDirection ( u.sprite, u.owner.TeamDirection );

				// Update HUD
				GM.UI.matchInfoMenu.GetPlayerHUD ( u ).UpdateStatusEffects ( u.InstanceID, u.Status );
				GM.UI.matchInfoMenu.GetPlayerHUD ( u ).UpdatePortrait ( u.InstanceID, Util.TeamColor ( owner.Team ) );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Returns the Mind Controlled unit back to its original owner.
	/// This function builds the animation queue.
	/// </summary>
	private void MindControlKO ( Unit u )
	{
		// Remove unit from the list of Mind Controlled units
		RemoveMindControlledUnit ( u );
	}

	/// <summary>
	/// Removes a unit from the list of currently Mind Controlled units by this hero.
	/// </summary>
	private void RemoveMindControlledUnit ( Unit u )
	{
		// Remove unit from list of Mind Controlled units
		mindControlledUnits.RemoveAll ( match => match == u );

		// Remove KO delegate
		u.koDelegate -= MindControlKO;
	}

	/// <summary>
	/// Removes and KO's all currently Mind Controlled units.
	/// </summary>
	private void DeactivateMindControl ( )
	{
		foreach ( Unit u in mindControlledUnits )
		{
			// Remove KO delegate to prevent list removal errors during a loop
			//u.unit.koDelegate -= MindControlKO;

			// KO unit
			u.GetAttacked ( true );
		}

		// Clear list of Mind Controlled units
		mindControlledUnits.Clear ( );
	}

	#endregion // Private Functions
}
