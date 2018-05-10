using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TagTeam : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Tag Team
	/// Type: Passive Ability
	/// Default Duration: Perminant
	/// 
	/// Ability 2: Divide And Conquer
	/// Type: Command Ability
	/// Default Duration: 3 Turns
	/// Default Cooldown: 5 Turns
	/// 
	/// </summary>

	#region Hero Data
	// Hero information
	public Sprite unitedDisplay;
	public Sprite splitDisplay;

	#endregion // Hero Data

	#region Ability Data
	
	public TagTeam tagTeamPartner;
	public TagTeam originalPartner;
	public bool isSplit = false;
	public bool hasDivideAndConquerMove = false;
	private const string DIVIDE_AND_CONQUER_STATUS_PROMPT = "Divide And Conquer";

	#endregion // Ability Data

	#region Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	/// <param name="t"> The tile who's neighbor will be checked for moves. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach this tile. </param>
	/// <param name="returnOnlyJumps"> Whether or not only jump moves should be stored as available moves. </param>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Clear previous move list
		if ( prerequisite == null )
			moveList.Clear ( );

		// Check status effects
		if ( status.CanMove )
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( owner.TeamDirection );

			// Check each neighboring tile
			for ( int i = 0; i < t.neighbors.Length; i++ )
			{
				// Ignore tiles that would allow for backward movement
				if ( i == back.FirstInt || i == back.SecondInt )
					continue;

				// Check if this unit can move to the neighboring tile
				if ( OccupyTileCheck ( t.neighbors [ i ], prerequisite ) )
				{
					// Check if only jumps are being returned
					if ( returnOnlyJumps )
					{
						// Check for Tag Team move
						if ( PassiveAvailabilityCheck ( CurrentAbility1, prerequisite ) )
						{
							// Add as an available Tag Team move
							moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i ) );
						}
					}
					else
					{
						// Add as an available move
						MoveData m = new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i );
						moveList.Add ( m );

						// Check for additional move with the Tag Team ability
						if ( PassiveAvailabilityCheck ( CurrentAbility1, prerequisite ) )
							FindMoves ( t.neighbors [ i ], m, true );
					}
				}
				// Check if this unit can jump the neighboring tile
				else if ( JumpTileCheck ( t.neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the neighboring unit can be attacked
					if ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
					{
						// Check for Tag Team move
						if ( PassiveAvailabilityCheck ( CurrentAbility1, prerequisite ) && prerequisite != null && prerequisite.Type == MoveData.MoveType.MOVE )
						{
							// Add as an available attack with Tag Team
							m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL_ATTACK, i, t.neighbors [ i ] );
						}
						else
						{
							// Add as an available attack
							m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.ATTACK, i, t.neighbors [ i ] );
						}
					}
					else
					{
						// Check for Tag Team move
						if ( PassiveAvailabilityCheck ( CurrentAbility1, prerequisite ) && prerequisite != null && prerequisite.Type == MoveData.MoveType.MOVE )
						{
							// Add as an available jump with Tag Team
							m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i );
						}
						else
						{
							// Add as an available jump
							m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i );
						}
					}

					// Add move to the move list
					moveList.Add ( m );

					// Find additional jumps
					FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
				}
			}
		}

		// Get Translocator availability
		CurrentAbility2.active = CommandAvailabilityCheck ( CurrentAbility2, prerequisite );
	}

	#endregion // Unit Override Functions

	#region HeroUnit Override Functions

	/// <summary>
	/// Sets up both abilities at the start of a match.
	/// </summary>
	protected override void Start ( )
	{
		// Set up hero
		base.Start ( );

		// Set Tag Team
		if ( CurrentAbility1.enabled )
		{
			CurrentAbility1.active = !isSplit;
		}

		// Set Divide And Conquer
		if ( CurrentAbility2.enabled && isSplit )
		{
			StartCooldown ( CurrentAbility2, Info.Ability2 );
		}
	}

	/// <summary>
	/// Determines how the unit should move based on the Move Data given.
	/// </summary>
	/// <param name="data"> The Move Data for the selected move. </param>
	public override void MoveUnit ( MoveData data )
	{
		// Move unit
		base.MoveUnit ( data );

		// Check if tag team partner needs to be added to the unit queue
		if ( DivideAndConquerMoveCheck ( data ) )
		{
			// Mark that the divide and conquer move has been used
			hasDivideAndConquerMove = true;
			tagTeamPartner.hasDivideAndConquerMove = true;

			// Add tag team partner to unit queue
			GM.UnitQueue.Add ( tagTeamPartner );
		}
	}

	/// <summary>
	/// Decrements the cooldown for the unit's special ability.
	/// </summary>
	public override void Cooldown ( )
	{
		// Decrement cooldowns and durations
		base.Cooldown ( );

		// Mark that the the Divide And Conquer move has not been used
		hasDivideAndConquerMove = false;
	}

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( )
	{
		// Clear board
		base.StartCommand ( );

		// Check if the hero is split
		if ( isSplit )
		{
			// Highlight partner
			tagTeamPartner.currentTile.SetTileState ( TileState.AvailableCommand );
		}
		else
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( owner.TeamDirection );

			// Check each neighboring tile
			for ( int i = 0; i < currentTile.neighbors.Length; i++ )
			{
				// Check for back direction
				if ( i == back.FirstInt || i == back.SecondInt )
					continue;

				// Highlight available tiles
				if ( OccupyTileCheck ( currentTile.neighbors [ i ], null ) )
					currentTile.neighbors [ i ].SetTileState ( TileState.AvailableCommand );
			}
		}
	}

	/// <summary>
	/// Selects the unit's to swap places.
	/// </summary>
	/// <param name="t"> The selected tile for the command. </param>
	public override void SelectCommandTile ( Tile t )
	{
		// Check if the hero is split
		if ( isSplit )
		{
			// Reunite heroes
			UniteTagTeam ( t );
		}
		else
		{
			// Split heroes
			SplitTagTeam ( t );
		}
	}

	/// <summary>
	/// Checks if this hero is capable of using a passive ability.
	/// Returns true if the passive ability is available.
	/// </summary>
	/// <param name="current"> The current ability data for the passive ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given passive ability. </param>
	/// <returns> Whether or not the passive ability can be used. </returns>
	protected override bool PassiveAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.PassiveAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check if the ability is active
		if ( !current.active )
			return false;

		// Check previous moves
		if ( CheckPrequisiteType ( prerequisite ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if this hero is capable of using a command ability.
	/// Returns true if the command ability is available.
	/// </summary>
	/// <param name="current"> The current ability data for the command ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given command ability. </param>
	/// <returns> Whether or not the command ability can be used. </returns>
	protected override bool CommandAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check if the hero can move
		if ( !status.CanMove )
			return false;

		// Check if the ability can be used
		if ( !DivideAndConquerCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Uses this hero unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// This function builds the animation queue from the Move Data.
	/// </summary>
	/// <param name="data"> The Move Data for the selected move. </param>
	protected override void UseSpecial ( MoveData data )
	{
		// Check for attack
		if ( data.Type == MoveData.MoveType.SPECIAL_ATTACK )
		{
			// Create animation
			Tween t = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME * 2 )
				.OnComplete ( ( ) =>
				{
					// Set unit and tile data
					SetUnitToTile ( data.Tile );
				} );

			// Add animation to queue
			GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );

			// Attack the unit
			AttackUnit ( data );
		}
		else
		{
			// Check for normal move
			if ( data.Prerequisite == null && currentTile.neighbors [ (int)data.Direction ] == data.Tile )
			{
				// Create animation
				Tween t = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME )
					.OnComplete ( ( ) =>
					{
						// Set unit and tile data
						SetUnitToTile ( data.Tile );
					} );

				// Add animation to queue
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
			else
			{
				// Create animation
				Tween t = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME * 2 )
					.OnComplete ( ( ) =>
					{
						// Set unit and tile data
						SetUnitToTile ( data.Tile );
					} );

				// Add animation to queue
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
		}
	}

	#endregion // HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Checks if the conditions are met to use the Divide And Conquer ability.
	/// Returns true if the ability can be used.
	/// </summary>
	/// <returns> Whether or not the Divided And Conquer ability can be used. </returns>
	private bool DivideAndConquerCheck ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.TeamDirection );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Check if the hero is split
			if ( isSplit )
			{
				// Check for partner
				if ( tagTeamPartner != null && tagTeamPartner.owner == owner && currentTile.neighbors [ i ] != null && currentTile.neighbors [ i ].currentUnit != null && currentTile.neighbors [ i ].currentUnit == tagTeamPartner )
					return true;
			}
			else
			{
				// Check for available tile
				if ( i != back.FirstInt && i != back.SecondInt && OccupyTileCheck ( currentTile.neighbors [ i ], null ) )
					return true;
			}
		}

		// Return that the ability cannot be used
		return false;
	}

	/// <summary>
	/// Checks if the conditions are met for the tag team partner to be given an additional turn.
	/// Returns true if the tag team partner should be added to the unit queue.
	/// </summary>
	/// <param name="data"> The Move Data for the selected move. </param>
	/// <returns> Whether or not the tag team partner should be added to the unit queue. </returns>
	private bool DivideAndConquerMoveCheck ( MoveData data )
	{
		// Check if ability is enabled
		if ( !CurrentAbility2.enabled )
			return false;

		// Check if partners are split
		if ( !isSplit )
			return false;

		// Check duration
		if ( CurrentAbility2.duration == 0 )
			return false;

		// Check if partner still exists
		if ( tagTeamPartner == null )
			return false;

		// Check if partner is still on the same team
		if ( tagTeamPartner.owner != owner )
			return false;

		// Check that both partners can use abilities
		if ( !status.CanUseAbility || !tagTeamPartner.status.CanUseAbility )
			return false;

		// Check if its the first move
		if ( data.Prerequisite != null )
			return false;

		// Check if the Divide And Conquer move has already be used this turn
		if ( hasDivideAndConquerMove )
			return false;

		// Return that the tag team partner needs to be added to the unit queue
		return true;
	}

	/// <summary>
	/// Splits the hero into two individual hero units.
	/// </summary>
	/// <param name="tile"> The tile where the seperate tag team partner will be placed. </param>
	private void SplitTagTeam ( Tile tile )
	{
		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Clear board
		GM.Board.ResetTiles ( );

		// Update hero as being split
		displaySprite = splitDisplay;
		sprite.sprite = splitDisplay;
		GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( instanceID, splitDisplay );
		isSplit = true;
		originalPartner = this;

		// Create partner
		tagTeamPartner = Instantiate ( this, owner.transform );
		tagTeamPartner.isSplit = true;
		tagTeamPartner.tagTeamPartner = this;
		tagTeamPartner.originalPartner = this;
		originalPartner = this;
		tagTeamPartner.transform.position = transform.position;
		owner.UnitInstances.Add ( tagTeamPartner );

		// Create animation
		Tween t = tagTeamPartner.transform.DOMove ( tile.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Set unit and tile data
				tagTeamPartner.characterName = "Hero 7.5";
				tagTeamPartner.instanceID *= 10;
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).AddPortrait ( tagTeamPartner, instanceID );
				tagTeamPartner.currentTile = tile;
				tile.currentUnit = tagTeamPartner;

				// Start cooldown
				StartCooldown ( CurrentAbility2, Info.Ability2 );
				if ( CurrentAbility1.enabled )
					CurrentAbility1.active = false;

				// Apply status effect
				status.AddStatusEffect ( abilitySprite2, DIVIDE_AND_CONQUER_STATUS_PROMPT, this, CurrentAbility2.duration );
				tagTeamPartner.status.AddStatusEffect ( abilitySprite2, DIVIDE_AND_CONQUER_STATUS_PROMPT, this, CurrentAbility2.duration );

				// Update HUDs
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( instanceID, status );
				GM.UI.matchInfoMenu.GetPlayerHUD ( tagTeamPartner ).UpdateStatusEffects ( tagTeamPartner.instanceID, tagTeamPartner.status );

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

	/// <summary>
	/// Unites the two individual hero units into one hero unit.
	/// </summary>
	/// <param name="tile"> The tile where the two tag team partners are reuniting. </param>
	private void UniteTagTeam ( Tile tile )
	{
		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Clear board
		GM.Board.ResetTiles ( );

		// Create animation
		Tween t = transform.DOMove ( tile.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Check for original
				if ( originalPartner == this )
				{
					// Reunite heroes
					isSplit = false;
					displaySprite = unitedDisplay;
					sprite.sprite = unitedDisplay;
					GM.UI.matchInfoMenu.GetPlayerHUD ( tagTeamPartner ).RemovePortrait ( tagTeamPartner.instanceID );

					// Remove partner
					owner.UnitInstances.Remove ( tagTeamPartner );
					tile.currentUnit = null;
					Destroy ( tagTeamPartner.gameObject );
					tagTeamPartner = null;
					originalPartner = null;

					// Set unit and tile data
					SetUnitToTile ( tile );

					// Start cooldown
					StartCooldown ( CurrentAbility2, Info.Ability2 );
					if ( CurrentAbility1.enabled )
						CurrentAbility1.active = true;

					// Pause turn timer
					if ( MatchSettings.TurnTimer )
						GM.UI.timer.ResumeTimer ( );

					// Get moves
					GM.GetTeamMoves ( );

					// Display team
					GM.DisplayAvailableUnits ( );
					GM.SelectUnit ( this );
				}
				else
				{
					// Reunite heroes
					tagTeamPartner.isSplit = false;
					tagTeamPartner.displaySprite = unitedDisplay;
					tagTeamPartner.sprite.sprite = unitedDisplay;
					GM.UI.matchInfoMenu.GetPlayerHUD ( this ).RemovePortrait ( instanceID );

					// Start cooldown
					tagTeamPartner.StartCooldown ( tagTeamPartner.CurrentAbility2, tagTeamPartner.Info.Ability2, false );
					if ( tagTeamPartner.CurrentAbility1.enabled )
						tagTeamPartner.CurrentAbility1.active = true;

					// Pause turn timer
					if ( MatchSettings.TurnTimer )
						GM.UI.timer.ResumeTimer ( );

					// Remove hero
					owner.UnitInstances.Remove ( this );
					currentTile.currentUnit = null;
					tagTeamPartner.tagTeamPartner = null;
					tagTeamPartner.originalPartner = null;

					// Get moves
					GM.GetTeamMoves ( );

					// Display team
					GM.DisplayAvailableUnits ( );
					GM.SelectUnit ( tagTeamPartner );

					// Destroy game object
					Destroy ( gameObject );
				}
			} );
	}

	#endregion // Private Functions
}
