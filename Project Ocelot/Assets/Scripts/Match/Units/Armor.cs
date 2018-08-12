using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Armor : HeroUnit
{
	/// <summary>
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
	/// </summary>

	// Hero information
	public Sprite withMechSprite;
	public Sprite withoutMechSprite;
	private bool isRecalling;

	// Ability information
	public TileObject selfDestructPrefab;
	public TileObject currentSelfDestruct;
	public TileObject recallPrefab;
	public TileObject currentRecall;
	private const float ARMOR_ATTACK_ANIMATION_TIME = 0.75f;
	private const float RECALL_ANIMATION_TIME = 0.75f;

	// Game objects
	public SpriteRenderer mechAnimation;

	#region Public Unit Override Functions

	public override void InitializeInstance ( GameManager gm, int instanceID, UnitSettingData settingData )
	{
		// Initialize base data
		base.InitializeInstance ( gm, instanceID, settingData );

		// Set current bodyguard duration
		InstanceData.Ability1.CurrentDuration = InstanceData.Ability1.Duration;
	}

	/// <summary>
	/// Calculates all base moves available to a unit.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Find base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

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
						GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( InstanceID, displaySprite );

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

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( AbilityInstanceData ability )
	{
		// Clear the board
		base.StartCommand ( ability );

		// Highlight available adjacent tiles
		GetAdjacentTiles ( );
	}

	/// <summary>
	/// Select the tile for Self-Destruct/Recall.
	/// </summary>
	public override void SelectCommandTile ( Tile t )
	{
		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Board.ResetTiles ( );

		// Check for Recall or Self-Destruct
		if ( activeAbility == InstanceData.Ability3 )
		{
			// Create Recall
			currentRecall = CreateTileOject ( recallPrefab, t, InstanceData.Ability3.Duration, RecallDurationComplete );

			// Set team color
			Color32 c = Util.TeamColor ( Owner.Team );
			currentRecall.Icon.color = new Color32 ( c.r, c.g, c.b, 150 );

			// Set position
			currentRecall.transform.position = t.transform.position;

			// Begin animation
			Sequence s = DOTween.Sequence ( )
				.Append ( currentRecall.Icon.DOFade ( 0, RECALL_ANIMATION_TIME ).From ( ) )
				.OnComplete ( ( ) =>
				{
					// Set that Recall is active
					isRecalling = true;
					MoveList.Clear ( );

					// Start cooldown
					StartCooldown ( InstanceData.Ability3 );

					// Apply status effect
					//Status.AddStatusEffect ( withMechSprite, RECALL_STATUS_PROMPT, this, InstanceData.Ability3.Duration, StatusEffects.StatusType.CAN_MOVE );
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
			currentSelfDestruct = CreateTileOject ( selfDestructPrefab, t, InstanceData.Ability2.Duration, SelfDestructDurationComplete );

			// Set team color
			currentSelfDestruct.Icon.color = Util.TeamColor ( Owner.Team );

			// Remove Armor
			LoseMinion ( false );

			// Begin animation
			Sequence s = DOTween.Sequence ( )
				.Append ( currentSelfDestruct.transform.DOMove ( currentTile.transform.position, MOVE_ANIMATION_TIME ).From ( ) )
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
			//Status.RemoveStatusEffect ( withMechSprite, RECALL_STATUS_PROMPT, this, StatusEffects.StatusType.CAN_MOVE );
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

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Checks if there adjacent unoccupied tiles available for the Self-Destruct and Reconstruct Abilities.
	/// Returns true if at least one adjacent tile is unoccupied.
	/// </summary>
	private bool AdjacentTilesCheck ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( Owner.TeamDirection );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check if the tile is unoccupied
			if ( OccupyTileCheck ( currentTile.neighbors [ i ], null ) )
				return true;
		}

		// Return that all neighboring tiles are occupied
		return false;
	}

	/// <summary>
	/// Marks every adjacent unoccupied tile for selection for Self-Destruct/Recall.
	/// </summary>
	private void GetAdjacentTiles ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( Owner.TeamDirection );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check if the tile is unoccupied
			if ( OccupyTileCheck ( currentTile.neighbors [ i ], null ) )
				currentTile.neighbors [ i ].SetTileState ( TileState.AvailableCommand );
		}
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
				// Remove Self-Destruct
				DestroyTileObject ( currentSelfDestruct );
			} );
		Tween t2 = currentSelfDestruct.Icon.DOFade ( 0, MOVE_ANIMATION_TIME );

		// Add animations to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );

		// Attack any adjacent enemy units
		foreach ( Tile t in currentSelfDestruct.CurrentHex.neighbors )
			if ( t != null && t.CurrentUnit != null && t.CurrentUnit.UnitAttackCheck ( this ) )
				t.CurrentUnit.GetAttacked ( );
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
		// Change sprite
		displaySprite = withoutMechSprite;
		sprite.sprite = displaySprite;

		// Update player HUD
		if ( !isFromAttack )
			GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( InstanceID, displaySprite );

		// Expire Armor's duration
		if ( !isFromAttack )
			InstanceData.Ability1.CurrentDuration = 0;

		// Set Recall cooldown
		StartCooldown ( InstanceData.Ability3, !isFromAttack );
	}

	/// <summary>
	/// Makes all of the necessary changes to the hero when Recall is used.
	/// </summary>
	private void GainMinion ( )
	{
		// Change sprite
		displaySprite = withMechSprite;
		sprite.sprite = displaySprite;
		GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( InstanceID, displaySprite );

		// Replenish Armor's duration
		if ( InstanceData.Ability1.IsEnabled )
			InstanceData.Ability1.CurrentDuration = InstanceData.Ability1.Duration;
	}

	/// <summary>
	/// Reomves Hero 1's Recall.
	/// </summary>
	private void EndReconstruct ( )
	{
		// Remove Recall
		DestroyTileObject ( currentRecall );

		// Cancel Recall
		isRecalling = false;
	}

	#endregion // Private Functions
}
