using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Armor : HeroUnit
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
	///
	/// Hero 1 Unit Data
	/// 
	/// ID: 8
	/// Name: Hero 1
	/// Nickname: Armor
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Defense
	/// Slots: 2
	/// 
	/// Ability 1 
	/// ID: 7
	/// Name: Bodyguard
	/// Description: Robotic minion sacrifices itself to block a single attack.
	/// Type: Passive
	/// Duration: 1 Attack
	/// 
	/// Ability 2
	/// ID: 8
	/// Name: Self-Destruct
	/// Description: Detonate robotic minion after a brief period to KO any nearby opponents
	/// Type: Toggle Command
	/// Duration: 1 Round
	/// Cooldown: 3 Rounds
	/// Allies Immune: Active
	/// 
	/// Ability 3
	/// ID: 9
	/// Name: Reconstruct
	/// Description: Construct a new robotic minion after a brief period.
	/// Type: Toggle Command
	/// Duration: 1 Round
	/// Cooldown: 6 Rounds
	/// 
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data

	[SerializeField]
	private SpriteRenderer mechAnimation;

	[SerializeField]
	private TileObject selfDestructPrefab;

	[SerializeField]
	private TileObject recallPrefab;

	private IReadOnlyUnitData duoData;
	private IReadOnlyUnitData singleData;
	private TileObject currentSelfDestruct;
	private TileObject currentRecall;
	private bool isRecalling;

	private const float ARMOR_ATTACK_ANIMATION_TIME = 0.75f;
	private const float RECALL_ANIMATION_TIME = 0.75f;

	#endregion // Ability Data

	#region Public Unit Override Functions

	public override void InitializeInstance ( GameManager gm, int instanceID, UnitSettingData settingData )
	{
		// Initialize base data
		base.InitializeInstance ( gm, instanceID, settingData );

		// Set current bodyguard duration
		InstanceData.Ability1.CurrentDuration = InstanceData.Ability1.Duration;

		// Store unit data for transitions
		duoData = UnitDatabase.GetUnit ( InstanceData.ID );
		singleData = UnitDatabase.GetUnit ( InstanceData.ID + 1 );
	}

	/// <summary>
	/// Calculates all base moves available to a unit.
	/// </summary>
	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Find base moves
		base.FindMoves ( hex, prerequisite, returnOnlyJumps );

		// Get Self-Destruct availability
		InstanceData.Ability2.IsAvailable = ToggleCommand1AvailabilityCheck ( InstanceData.Ability2, prerequisite );

		// Get Reconstruct availability
		InstanceData.Ability3.IsAvailable = ToggleCommand2AvailabilityCheck ( InstanceData.Ability3, prerequisite );
	}

	/// <summary>
	/// Attack this unit and remove this unit's armor if it's available or KO this unit if it's not.
	/// Call this function on the unit being attack.
	/// </summary>
	public override void GetAttacked ( bool usePostAnimationQueue = false )
	{
		// Check armor duration
		if ( PassiveAvailabilityCheck ( InstanceData.Ability1, null ) && !usePostAnimationQueue )
		{
			// Decrement armor duration
			InstanceData.Ability1.CurrentDuration--;

			// Check if Armor is destroyed
			if ( InstanceData.Ability1.CurrentDuration == 0 )
			{
				// Create animation
				Tween t1 = mechAnimation.transform.DOScale ( new Vector3 ( 3.33f, 3.33f, 3.33f ), MOVE_ANIMATION_TIME )
					.OnStart ( ( ) =>
					{
						// Update sprites
						LoseMinion ( true );
						mechAnimation.gameObject.SetActive ( true );
						mechAnimation.transform.localScale = Vector3.one;
						mechAnimation.color = Util.TeamColor ( Owner.Team );
					} )
					.OnComplete ( ( ) =>
					{
						// Update player HUD
						GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( this );

						// Hide animation sprite
						mechAnimation.gameObject.SetActive ( false );
					} );
				Tween t2 = mechAnimation.DOFade ( 0, MOVE_ANIMATION_TIME );

				// Add animations to queue
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
			}
			else
			{
				// Create animation
				Tween t = transform.DOShakePosition ( ARMOR_ATTACK_ANIMATION_TIME, 0.5f );

				// Add animation to queue
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
		}
		else
		{
			// KO this unit
			base.GetAttacked ( usePostAnimationQueue );
		}
	}

	#endregion // Public Unit Override Functions

	#region Public HeroUnit Override Functions

	public override void ExecuteCommand ( )
	{
		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( activeAbility );

		// Clear board
		GM.Grid.ResetTiles ( );

		// Check for Reconstruct or Self-Destruct
		if ( activeAbility == InstanceData.Ability3 )
		{
			// Create Recall
			currentRecall = CreateTileOject ( recallPrefab, GM.SelectedCommand.PrimaryTarget, InstanceData.Ability3.Duration, RecallDurationComplete, EndReconstruct );

			// Set team color
			Color32 c = Util.TeamColor ( Owner.Team );
			currentRecall.Icon.color = new Color32 ( c.r, c.g, c.b, 150 );

			// Set position
			currentRecall.transform.position = GM.SelectedCommand.PrimaryTarget.transform.position;

			// Begin animation
			Sequence s = DOTween.Sequence ( )
				.Append ( currentRecall.Icon.DOFade ( 0, RECALL_ANIMATION_TIME ).From ( ) )
				.OnComplete ( ( ) =>
				{
					// Set that Recall is active
					isRecalling = true;
					MoveList.Clear ( );

					// Start cooldown
					StartCooldown ( InstanceData.Ability2 );
					StartCooldown ( InstanceData.Ability3 );

					// Apply status effect
					Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.CRAFTING, InstanceData.Ability3.Duration, this );
					GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );

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
		else
		{
			// Create Self-Destruct
			currentSelfDestruct = CreateTileOject ( selfDestructPrefab, GM.SelectedCommand.PrimaryTarget, InstanceData.Ability2.Duration, SelfDestructDurationComplete );

			// Set team color
			currentSelfDestruct.Icon.color = Util.TeamColor ( Owner.Team );

			// Remove Armor
			LoseMinion ( false );

			// Begin animation
			Sequence s = DOTween.Sequence ( )
				.Append ( currentSelfDestruct.transform.DOMove ( CurrentHex.transform.position, MOVE_ANIMATION_TIME ).From ( ) )
				.OnComplete ( ( ) =>
				{
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
	}

	/// <summary>
	/// Interupts any actions that take more than one turn to complete that this unit is in the process of doing.
	/// Call this function when this unit is being attacked or being affected by some interupting ability.
	/// IMPORTANT: Be sure to call this function first before the interupting action since Interupts change the status effects of the action being interupted and the interupting action may apply new status effects.
	/// </summary>
	public override void InteruptUnit ( )
	{
		// Check if hero can be interupted
		if ( isRecalling )
		{
			// End recall
			EndReconstruct ( );

			// Interupt status effect
			GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );
		}
	}

	#endregion // Public HeroUnit Override Functions

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

		// Check if duration has expired
		if ( ability.CurrentDuration == 0 )
			return false;

		// Return that the ability is available
		return true;
	}

	protected override bool ToggleCommand1AvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.ToggleCommand1AvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check for bodyguard
		if ( InstanceData.Ability1.IsEnabled && InstanceData.Ability1.CurrentDuration == 0 )
			return false;

		// Check for available tiles
		if ( !AdjacentTilesCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	protected override bool ToggleCommand2AvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.ToggleCommand1AvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check for bodyguard
		if ( InstanceData.Ability1.IsEnabled && InstanceData.Ability1.CurrentDuration > 0 )
			return false;

		// Check for available tiles
		if ( !AdjacentTilesCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	protected override void GetCommandTargets ( )
	{
		// Check each neighboring tile
		for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
		{
			// Check if the tile is unoccupied
			if ( OccupyTileCheck ( CurrentHex.Neighbors [ i ], null ) )
				CurrentHex.Neighbors [ i ].Tile.SetTileState ( TileState.AvailableCommand );
		}
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Checks if there adjacent unoccupied tiles available for the Self-Destruct and Reconstruct Abilities.
	/// Returns true if at least one adjacent tile is unoccupied.
	/// </summary>
	private bool AdjacentTilesCheck ( )
	{
		// Check each neighboring tile
		for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
		{
			// Check if the tile is unoccupied
			if ( OccupyTileCheck ( CurrentHex.Neighbors [ i ], null ) )
				return true;
		}

		// Return that all neighboring tiles are occupied
		return false;
	}

	/// <summary>
	/// Delegate for when the duration of the tile object for Self-Destruct expires.
	/// </summary>
	private void SelfDestructDurationComplete ( )
	{
		// Create animation
		Tween t1 = currentSelfDestruct.transform.DOScale ( new Vector3 ( 5, 5, 5 ), MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability2.IsActive = false;

				// Remove Self-Destruct
				DestroyTileObject ( currentSelfDestruct );
			} );
		Tween t2 = currentSelfDestruct.Icon.DOFade ( 0, MOVE_ANIMATION_TIME );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );

		// Attack any adjacent enemy units
		foreach ( Hex hex in currentSelfDestruct.CurrentHex.Neighbors )
		{
			// Check for tile
			if ( hex == null )
				continue;

			// Check if the tile is occupied
			if ( !hex.Tile.IsOccupied )
				continue;

			// Check for unit
			if ( hex.Tile.CurrentUnit != null )
			{
				// Check if the unit can be attacked
				if ( !hex.Tile.CurrentUnit.UnitAttackCheck ( this, !InstanceData.Ability2.IsPerkEnabled ) )
					continue;

				// Check if the unit can be affected by abilities
				if ( !hex.Tile.CurrentUnit.Status.CanBeAffectedByAbility )
					continue;

				// Check if the unit can be affected physically
				if ( !hex.Tile.CurrentUnit.Status.CanBeAffectedPhysically )
					continue;

				// Attack unit
				hex.Tile.CurrentUnit.GetAttacked ( );
			}
			else if ( hex.Tile.CurrentObject != null )
			{
				// Check if the tile object can be attacked
				if ( !hex.Tile.CurrentObject.CanBeAttacked )
					continue;

				// Check if the tile object is on the same team
				if ( InstanceData.Ability2.IsPerkEnabled && hex.Tile.CurrentObject.Caster.Owner == Owner )
					continue;

				// Attack tile object
				hex.Tile.CurrentObject.GetAttacked ( );
			}
		}
	}

	/// <summary>
	/// Delegate for when the duration of the tile object for Recall expires.
	/// </summary>
	private void RecallDurationComplete ( )
	{
		// Create animation
		Tween t = transform.DOMove ( currentRecall.CurrentHex.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Set unit and tile data
				SetUnitToTile ( currentRecall.CurrentHex );

				// Replenish Armor
				GainMinion ( );

				// Set that Recall is complete
				EndReconstruct ( );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Makes all of the necessary changes to the hero when Armor's duration expires or Self-Destruct is used. 
	/// </summary>
	private void LoseMinion ( bool isFromAttack )
	{
		// Update unit data for single
		UpdateUnitData ( singleData );

		// Change sprite
		displaySprite = InstanceData.Portrait;
		sprite.sprite = InstanceData.Portrait;

		// Update player HUD
		if ( !isFromAttack )
			GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( this );

		// Expire Armor's duration
		if ( !isFromAttack )
			InstanceData.Ability1.CurrentDuration = 0;

		// Set Recall cooldown
		StartCooldown ( InstanceData.Ability2, !isFromAttack );
		StartCooldown ( InstanceData.Ability3, !isFromAttack );
	}

	/// <summary>
	/// Makes all of the necessary changes to the hero when Recall is used.
	/// </summary>
	private void GainMinion ( )
	{
		// Update unit data for duo
		UpdateUnitData ( duoData );

		// Change sprite
		displaySprite = InstanceData.Portrait;
		sprite.sprite = InstanceData.Portrait;
		GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( this );

		// Replenish Armor's duration
		if ( InstanceData.Ability1.IsEnabled )
			InstanceData.Ability1.CurrentDuration = InstanceData.Ability1.Duration;
	}

	/// <summary>
	/// Reomves Hero 1's Recall.
	/// </summary>
	private void EndReconstruct ( )
	{
		// Create animation
		Tween t1 = currentRecall.transform.DOScale ( new Vector3 ( 5, 5, 5 ), MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability3.IsActive = false;

				// Remove Recall
				DestroyTileObject ( currentRecall );

				// Cancel Recall
				isRecalling = false;
				currentRecall = null;
			} );
		Tween t2 = currentRecall.Icon.DOFade ( 0, MOVE_ANIMATION_TIME );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
	}

	#endregion // Private Functions
}
