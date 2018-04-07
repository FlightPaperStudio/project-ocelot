using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Armor : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Armor
	/// Type: Passive Ability
	/// Default Duration: 1 Attack
	/// 
	/// Ability 2: Self-Destruct/Recall
	/// Type: Command Ability
	/// Default Duration: 1 Turn
	/// Default Cooldown: 8 Turns
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
	private const string RECALL_STATUS_PROMPT = "Recall";

	// Game objects
	public SpriteRenderer mechAnimation;

	/// <summary>
	/// Calculates all base moves available to a unit.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Find base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Self-Destruct/Recall availability
		CurrentAbility2.active = CommandAvailabilityCheck ( CurrentAbility2, prerequisite );
	}

	/// <summary>
	/// Attack this unit and remove this unit's armor if it's available or KO this unit if it's not.
	/// Call this function on the unit being attack.
	/// </summary>
	public override void GetAttacked ( bool usePostAnimationQueue = false )
	{
		// Check armor duration
		if ( PassiveAvailabilityCheck ( CurrentAbility1, null ) && !usePostAnimationQueue )
		{
			// Decrement armor duration
			CurrentAbility1.duration--;

			// Check if Armor is destroyed
			if ( CurrentAbility1.duration == 0 )
			{
				// Create animation
				Tween t1 = mechAnimation.transform.DOScale ( new Vector3 ( 3.33f, 3.33f, 3.33f ), MOVE_ANIMATION_TIME )
					.OnStart ( () =>
					{
						// Update sprites
						LoseMech ( true );
						mechAnimation.gameObject.SetActive ( true );
						mechAnimation.transform.localScale = Vector3.one;
						mechAnimation.color = Util.TeamColor ( owner.Team );
					} )
					.OnComplete ( ( ) =>
					{
						// Update player HUD
						GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( instanceID, displaySprite );

						// Hide animation sprite
						mechAnimation.gameObject.SetActive ( false );
					} );
				Tween t2 = mechAnimation.DOFade ( 0, MOVE_ANIMATION_TIME );

				// Add animations to queue
				GM.animationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
				GM.animationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
			}
			else
			{
				// Create animation
				Tween t = transform.DOShakePosition ( ARMOR_ATTACK_ANIMATION_TIME, 0.5f );

				// Add animation to queue
				GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
		}
		else
		{
			// KO this unit
			base.GetAttacked ( usePostAnimationQueue );
		}
	}

	/// <summary>
	/// Checks if the hero is capable of using a passive ability.
	/// Returns true if the passive ability is available.
	/// </summary>
	protected override bool PassiveAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.PassiveAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check if duration has expired
		if ( current.duration == 0 )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if the hero is capable of using a command ability.
	/// Returns true if the command ability is availability.
	/// </summary>
	protected override bool CommandAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check for available tiles
		if ( !SelfDestructRecallCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if there adjacent unoccupied tiles available for the Self-Destruct/Recall Ability.
	/// Returns true if at least one adjacent tile is unoccupied.
	/// </summary>
	private bool SelfDestructRecallCheck ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.TeamDirection );

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
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( )
	{
		// Clear the board
		base.StartCommand ( );

		// Highlight available adjacent tiles
		GetSelfDestructRecall ( );
	}

	/// <summary>
	/// Marks every adjacent unoccupied tile for selection for Self-Destruct/Recall.
	/// </summary>
	private void GetSelfDestructRecall ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.TeamDirection );

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
	/// Select the tile for Self-Destruct/Recall.
	/// </summary>
	public override void SelectCommandTile ( Tile t )
	{
		// Pause turn timer
		if ( MatchSettings.turnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Clear board
		GM.board.ResetTiles ( );

		// Check for Recall or Self-Destruct
		if ( CurrentAbility1.duration == 0 )
		{
			// Create Recall
			currentRecall = CreateTileOject ( recallPrefab, t, Info.Ability2.Duration, RecallDurationComplete );

			// Set team color
			Color32 c = Util.TeamColor ( owner.Team );
			currentRecall.sprite.color = new Color32 ( c.r, c.g, c.b, 150 );

			// Set position
			currentRecall.transform.position = t.transform.position;

			// Begin animation
			Sequence s = DOTween.Sequence ( )
				.Append ( currentRecall.sprite.DOFade ( 0, RECALL_ANIMATION_TIME ).From ( ) )
				.OnComplete ( ( ) =>
				{
					// Set that Recall is active
					isRecalling = true;
					moveList.Clear ( );

					// Start cooldown
					StartCooldown ( CurrentAbility2, Info.Ability2 );

					// Apply status effect
					status.AddStatusEffect ( withMechSprite, RECALL_STATUS_PROMPT, this, CurrentAbility2.duration, StatusEffects.StatusType.CAN_MOVE );
					GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( instanceID, status );

					// Pause turn timer
					if ( MatchSettings.turnTimer )
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
			currentSelfDestruct = CreateTileOject ( selfDestructPrefab, t, Info.Ability2.Duration, SelfDestructDurationComplete );

			// Set team color
			currentSelfDestruct.sprite.color = Util.TeamColor ( owner.Team );

			// Remove Armor
			LoseMech ( false );

			// Begin animation
			Sequence s = DOTween.Sequence ( )
				.Append ( currentSelfDestruct.transform.DOMove ( currentTile.transform.position, MOVE_ANIMATION_TIME ).From ( ) )
				.OnComplete ( ( ) =>
				{
					// Pause turn timer
					if ( MatchSettings.turnTimer )
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
		Tween t2 = currentSelfDestruct.sprite.DOFade ( 0, MOVE_ANIMATION_TIME );

		// Add animations to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );

		// Attack any adjacent enemy units
		foreach ( Tile t in currentSelfDestruct.tile.neighbors )
			if ( t != null && t.currentUnit != null && t.currentUnit.UnitAttackCheck ( this ) )
				t.currentUnit.GetAttacked ( );
	}

	/// <summary>
	/// Delegate for when the duration of the tile object for Recall expires.
	/// </summary>
	private void RecallDurationComplete ( )
	{
		// Create animation
		Tween t = transform.DOMove ( currentRecall.tile.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Set unit and tile data
				SetUnitToTile ( currentRecall.tile );

				// Replenish Armor
				GainMech ( );

				// Set that Recall is complete
				EndRecall ( );
			} );

		// Add animation to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Makes all of the necessary changes to the hero when Armor's duration expires or Self-Destruct is used. 
	/// </summary>
	private void LoseMech ( bool isFromAttack )
	{
		// Change sprite
		displaySprite = withoutMechSprite;
		sprite.sprite = displaySprite;

		// Update player HUD
		if ( !isFromAttack )
			GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( instanceID, displaySprite );

		// Expire Armor's duration
		if ( !isFromAttack )
			CurrentAbility1.duration = 0;

		// Set Recall cooldown
		StartCooldown ( CurrentAbility2, Info.Ability2, !isFromAttack );
	}

	/// <summary>
	/// Makes all of the necessary changes to the hero when Recall is used.
	/// </summary>
	private void GainMech ( )
	{
		// Change sprite
		displaySprite = withMechSprite;
		sprite.sprite = displaySprite;
		GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( instanceID, displaySprite );

		// Replenish Armor's duration
		CurrentAbility1.duration = Info.Ability1.Duration;
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
			EndRecall ( );

			// Interupt status effect
			status.RemoveStatusEffect ( withMechSprite, RECALL_STATUS_PROMPT, this, StatusEffects.StatusType.CAN_MOVE );
			GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( instanceID, status );
		}
	}

	/// <summary>
	/// Reomves Hero 1's Recall.
	/// </summary>
	private void EndRecall ( )
	{
		// Remove Recall
		DestroyTileObject ( currentRecall );

		// Cancel Recall
		isRecalling = false;
	}
}
