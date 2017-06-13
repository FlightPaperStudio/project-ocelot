﻿using System.Collections;
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

	// Game objects
	public SpriteRenderer mechAnimation;

	/// <summary>
	/// Calculates all base moves available to a unit.
	/// </summary>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Check for Recall
		if ( !isRecalling )
		{
			// Find base moves
			base.FindMoves ( t, prerequisite, returnOnlyJumps );

			// Get Self-Destruct/Recall availability
			if ( !returnOnlyJumps )
			{
				if ( currentAbility2.enabled && currentAbility2.cooldown == 0 && SelfDestructRecallCheck ( ) )
					currentAbility2.active = true;
				else
					currentAbility2.active = false;
			}
		}
	}

	/// <summary>
	/// Attack this unit and remove this unit's armor if it's available or KO this unit if it's not.
	/// Call this function on the unit being attack.
	/// </summary>
	public override void GetAttacked ( bool lostMatch = false )
	{
		// Check armor duration
		if ( currentAbility1.enabled && currentAbility1.duration > 0 && !lostMatch )
		{
			// Decrement armor duration
			currentAbility1.duration--;

			// Check if Armor is destroyed
			if ( currentAbility1.duration == 0 )
			{
				// Create animation
				Tween t1 = mechAnimation.transform.DOScale ( new Vector3 ( 3.33f, 3.33f, 3.33f ), MOVE_ANIMATION_TIME )
					.OnStart ( () =>
					{
						// Update sprites
						LoseMech ( true );
						mechAnimation.gameObject.SetActive ( true );
						mechAnimation.transform.localScale = Vector3.one;
						mechAnimation.color = Util.TeamColor ( owner.team );
					} )
					.OnComplete ( ( ) =>
					{
						// Update player HUD
						GM.UI.hudDic [ owner ].UpdateIcon ( instanceID, displaySprite );

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
				Tween t = transform.DOShakePosition ( 0.75f, 0.5f );

				// Add animation to queue
				GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
		}
		else
		{
			// Check for Recall
			if ( isRecalling )
				EndRecall ( );

			// KO this unit
			base.GetAttacked ( lostMatch );
		}
	}

	/// <summary>
	/// Checks if there adjacent unoccupied tiles available for the Self-Destruct/Recall Ability.
	/// Returns true if at least one adjacent tile is unoccupied.
	/// </summary>
	private bool SelfDestructRecallCheck ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.direction );

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
		IntPair back = GetBackDirection ( owner.direction );

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
		if ( currentAbility1.duration == 0 )
		{
			// Create Recall
			currentRecall = CreateTileOject ( recallPrefab, t, info.ability2.duration, RecallDurationComplete );

			// Set team color
			Color32 c = Util.TeamColor ( owner.team );
			currentRecall.sprite.color = new Color32 ( c.r, c.g, c.b, 150 );

			// Set position
			currentRecall.transform.position = t.transform.position;

			// Begin animation
			Sequence s = DOTween.Sequence ( )
				.Append ( currentRecall.sprite.DOFade ( 0, 0.75f ).From ( ) )
				.OnComplete ( ( ) =>
				{
					// Set that Recall is active
					isRecalling = true;
					moveList.Clear ( );

					// Start cooldown
					StartCooldown ( currentAbility2, info.ability2 );

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
			currentSelfDestruct = CreateTileOject ( selfDestructPrefab, t, info.ability2.duration, SelfDestructDurationComplete );

			// Set team color
			currentSelfDestruct.sprite.color = Util.TeamColor ( owner.team );

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
			if ( t.currentUnit != null && t.currentUnit.UnitAttackCheck ( this ) )
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
			GM.UI.hudDic [ owner ].UpdateIcon ( instanceID, displaySprite );

		// Expire Armor's duration
		if ( !isFromAttack )
			currentAbility1.duration = 0;

		// Set Recall cooldown
		StartCooldown ( currentAbility2, info.ability2, !isFromAttack );
	}

	/// <summary>
	/// Makes all of the necessary changes to the hero when Recall is used.
	/// </summary>
	private void GainMech ( )
	{
		// Change sprite
		displaySprite = withMechSprite;
		sprite.sprite = displaySprite;
		GM.UI.hudDic [ owner ].UpdateIcon ( instanceID, displaySprite );

		// Replenish Armor's duration
		currentAbility1.duration = info.ability1.duration;
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
