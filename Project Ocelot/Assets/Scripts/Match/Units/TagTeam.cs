using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TagTeam : HeroUnit
{
	/// <summary>
	///
	/// Hero 7 Unit Data
	/// 
	/// ID: 15
	/// Name: Hero 7
	/// Nickname: Tag Team
	/// Bio: ???
	/// Finishing Move: ???
	/// Role: Offense
	/// Slots: 2
	/// 
	/// Ability 1
	/// ID: 23
	/// Name: Tag Team
	/// Description: Can move twice in a single turn
	/// Type: Passive
	/// Continue Movement: Active
	/// Applied Status Effects (Self): Haste
	/// 
	/// Ability 2
	/// ID: 24
	/// Name: Split Up
	/// Description: The duo goes their separate ways with extra mobility for a short period
	/// Type: Toggle Command
	/// Duration: 3 Rounds
	/// Cooldown: 3 Rounds
	/// Partner Movement: Active
	/// Applied Status Effects (Self): In Unison
	/// Applied Status Effects (Ally): In Unison
	/// 
	/// Ability 3
	/// ID: 25
	/// Name: Regroup
	/// Description: The duo gets back together
	/// Type: Toggle Command
	/// Cooldown: 5 Rounds
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

	#endregion // Ability Data

	#region MonoBehaviour Functions

	/// <summary>
	/// Sets up both abilities at the start of a match.
	/// </summary>
	private void Start ( )
	{
		
	}

	#endregion // MonoBehaviour Functions

	#region Public Unit Override Functions

	public override void InitializeInstance ( GameManager gm, int instanceID, UnitSettingData settingData )
	{
		// Initialize the instance
		base.InitializeInstance ( gm, instanceID, settingData );

		// Apply status effect
		if ( InstanceData.Ability1.IsEnabled && !isSplit )
			Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.HASTE, StatusEffects.PERMANENT_EFFECT, this );

		// Set Tag Team
		if ( InstanceData.Ability1.IsEnabled )
			InstanceData.Ability1.IsAvailable = !isSplit;

		// Set Divide And Conquer
		if ( InstanceData.Ability3.IsEnabled && isSplit )
			StartToggleCooldown ( InstanceData.Ability2, InstanceData.Ability3 );
	}

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
			MoveList.Clear ( );

		// Check status effects
		if ( Status.CanMove )
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( Owner.TeamDirection );

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
						if ( PassiveAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
						{
							// Add as an available Tag Team move
							MoveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i ) );
						}
					}
					else
					{
						// Add as an available move
						MoveData m = new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i );
						MoveList.Add ( m );

						// Check for additional move with the Tag Team ability
						if ( PassiveAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
							FindMoves ( t.neighbors [ i ], m, true );
					}
				}
				// Check if this unit can jump the neighboring tile
				else if ( JumpTileCheck ( t.neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the neighboring unit can be attacked
					if ( t.neighbors [ i ].CurrentUnit != null && t.neighbors [ i ].CurrentUnit.UnitAttackCheck ( this ) )
					{
						// Check for Tag Team move
						if ( PassiveAvailabilityCheck ( InstanceData.Ability1, prerequisite ) && prerequisite != null && prerequisite.Type == MoveData.MoveType.MOVE )
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
						if ( PassiveAvailabilityCheck ( InstanceData.Ability1, prerequisite ) && prerequisite != null && prerequisite.Type == MoveData.MoveType.MOVE )
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
					MoveList.Add ( m );

					// Find additional jumps
					if ( InstanceData.Ability1.IsPerkEnabled || ( m.Type != MoveData.MoveType.SPECIAL && m.Type != MoveData.MoveType.SPECIAL_ATTACK ) )
						FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
				}
			}
		}

		// Get Translocator availability
		InstanceData.Ability2.IsAvailable = ToggleCommand1AvailabilityCheck ( InstanceData.Ability2, prerequisite );
		InstanceData.Ability3.IsAvailable = ToggleCommand2AvailabilityCheck ( InstanceData.Ability3, prerequisite );
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

	#endregion // Public Unit Override Functions

	#region Public HeroUnit Override Functions

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
	public override void StartCommand ( AbilityInstanceData ability )
	{
		// Clear board
		base.StartCommand ( ability );

		// Check if the hero is split
		if ( isSplit )
		{
			// Highlight partner
			tagTeamPartner.currentTile.SetTileState ( TileState.AvailableCommand );
		}
		else
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( Owner.TeamDirection );

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

	#endregion // Public HeroUnit Override Functions

	#region Protected HeroUnit Override Functions

	/// <summary>
	/// Checks if this hero is capable of using a passive ability.
	/// Returns true if the passive ability is available.
	/// </summary>
	/// <param name="ability"> The current ability data for the passive ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given passive ability. </param>
	/// <returns> Whether or not the passive ability can be used. </returns>
	protected override bool PassiveAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.PassiveAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check if the ability is active
		if ( !ability.IsAvailable )
			return false;

		// Check previous moves
		if ( PriorMoveTypeCheck ( prerequisite ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if this hero is capable of using a command ability.
	/// Returns true if the command ability is available.
	/// </summary>
	/// <param name="ability"> The current ability data for the command ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given command ability. </param>
	/// <returns> Whether or not the command ability can be used. </returns>
	protected override bool ToggleCommand1AvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check if the hero can move
		if ( !Status.CanMove )
			return false;

		// Check if the hero is split up
		if ( isSplit )
			return false;

		// Check if the ability can be used
		if ( !DivideAndConquerCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if this hero is capable of using a command ability.
	/// Returns true if the command ability is available.
	/// </summary>
	/// <param name="ability"> The current ability data for the command ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given command ability. </param>
	/// <returns> Whether or not the command ability can be used. </returns>
	protected override bool ToggleCommand2AvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( ability, prerequisite ) )
			return false;

		// Check if the hero can move
		if ( !Status.CanMove )
			return false;

		// Check if the hero is split up
		if ( !isSplit )
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
			Tween t = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME * 2 )
				.OnStart ( ( ) =>
				{
					// Mark that the ability is active
					InstanceData.Ability1.IsActive = true;
					GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
				} )
				.OnComplete ( ( ) =>
				{
					// Mark that the ability is no longer active
					InstanceData.Ability1.IsActive = false;
					GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );

					// Set unit and tile data
					SetUnitToTile ( data.Destination );
				} );

			// Add animation to queue
			GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );

			// Attack the unit
			AttackUnit ( data );
		}
		else
		{
			// Check for normal move
			if ( data.PriorMove == null && currentTile.neighbors [ (int)data.Direction ] == data.Destination )
			{
				// Create animation
				Tween t = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME )
					.OnStart ( ( ) =>
					{
						// Mark that the ability is active
						InstanceData.Ability1.IsActive = true;
						GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
					} )
					.OnComplete ( ( ) =>
					{
						// Mark that the ability is no longer active
						InstanceData.Ability1.IsActive = false;
						GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );

						// Set unit and tile data
						SetUnitToTile ( data.Destination );
					} );

				// Add animation to queue
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
			else
			{
				// Create animation
				Tween t = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME * 2 )
					.OnStart ( ( ) =>
					{
						// Mark that the ability is active
						InstanceData.Ability1.IsActive = true;
						GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
					} )
					.OnComplete ( ( ) =>
					{
						// Mark that the ability is no longer active
						InstanceData.Ability1.IsActive = false;
						GM.UI.unitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );

						// Set unit and tile data
						SetUnitToTile ( data.Destination );
					} );

				// Add animation to queue
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
		}
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Checks if the conditions are met to use the Divide And Conquer ability.
	/// Returns true if the ability can be used.
	/// </summary>
	/// <returns> Whether or not the Divided And Conquer ability can be used. </returns>
	private bool DivideAndConquerCheck ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( Owner.TeamDirection );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Check if the hero is split
			if ( isSplit )
			{
				// Check for partner
				if ( tagTeamPartner != null && tagTeamPartner.Owner == Owner && currentTile.neighbors [ i ] != null && currentTile.neighbors [ i ].CurrentUnit != null && currentTile.neighbors [ i ].CurrentUnit == tagTeamPartner )
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
		if ( !InstanceData.Ability2.IsEnabled )
			return false;
		
		// Check if partners are split
		if ( !isSplit )
			return false;
		
		// Check duration
		if ( InstanceData.Ability2.CurrentDuration == 0 )
			return false;
		
		// Check if partner still exists
		if ( tagTeamPartner == null )
			return false;
		
		// Check if partner is still on the same team
		if ( tagTeamPartner.Owner != Owner )
			return false;
		
		// Check that both partners can use abilities
		if ( !Status.CanUseAbility || !tagTeamPartner.Status.CanUseAbility )
			return false;
		
		// Check that both partners can move
		if ( !Status.CanMove || !tagTeamPartner.Status.CanMove )
			return false;
		
		// Check if its the first move
		if ( data.PriorMove != null )
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
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Board.ResetTiles ( );

		// Update hero as being split
		displaySprite = splitDisplay;
		sprite.sprite = splitDisplay;
		GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( InstanceID, splitDisplay );
		isSplit = true;
		originalPartner = this;

		// Create partner
		tagTeamPartner = Instantiate ( this, Owner.transform );
		tagTeamPartner.isSplit = true;
		tagTeamPartner.tagTeamPartner = this;
		tagTeamPartner.originalPartner = this;
		originalPartner = this;
		tagTeamPartner.transform.position = transform.position;
		Owner.UnitInstances.Add ( tagTeamPartner );
		tagTeamPartner.InitializeInstance ( GM, InstanceID * 10, Owner.Units.Find ( x => x.ID == ID ) );

		// Create animation
		Tween t = tagTeamPartner.transform.DOMove ( tile.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Set unit and tile data
				//tagTeamPartner.characterName = "Hero 7.5";
				//tagTeamPartner.InstanceID *= 10;
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).AddPortrait ( tagTeamPartner, InstanceID );
				tagTeamPartner.currentTile = tile;
				tile.CurrentUnit = tagTeamPartner;

				// Start cooldown
				StartToggleCooldown ( InstanceData.Ability2, InstanceData.Ability3 );
				if ( InstanceData.Ability1.IsEnabled )
					InstanceData.Ability1.IsAvailable = false;

				// Remove status effect
				Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.HASTE, this );
				tagTeamPartner.Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.HASTE, tagTeamPartner );

				// Apply status effect
				Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.IN_UNISON, InstanceData.Ability2.Duration, this );
				tagTeamPartner.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.IN_UNISON, InstanceData.Ability2.Duration, this );

				// Update HUDs
				GM.UI.unitHUD.UpdateStatusEffects ( );
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );
				GM.UI.matchInfoMenu.GetPlayerHUD ( tagTeamPartner ).UpdateStatusEffects ( tagTeamPartner.InstanceID, tagTeamPartner.Status );

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
		GM.UI.unitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Board.ResetTiles ( );

		// Create animation
		Tween t = transform.DOMove ( tile.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Check for original
				if ( originalPartner == this )
				{
					// Mark that the ability is no longer active
					InstanceData.Ability3.IsActive = false;

					// Reunite heroes
					isSplit = false;
					displaySprite = unitedDisplay;
					sprite.sprite = unitedDisplay;
					GM.UI.matchInfoMenu.GetPlayerHUD ( tagTeamPartner ).RemovePortrait ( tagTeamPartner.InstanceID );

					// Remove partner
					Owner.UnitInstances.Remove ( tagTeamPartner );
					tile.CurrentUnit = null;
					Destroy ( tagTeamPartner.gameObject );
					tagTeamPartner = null;
					originalPartner = null;

					// Remove status effect
					Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.IN_UNISON, this );

					// Apply status effect
					Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.HASTE, StatusEffects.PERMANENT_EFFECT, this );

					// Update player hud
					GM.UI.matchInfoMenu.GetPlayerHUD ( Owner ).UpdateStatusEffects ( InstanceID, Status );

					// Set unit and tile data
					SetUnitToTile ( tile );

					// Start cooldown
					if ( InstanceData.Ability2.IsEnabled )
						StartToggleCooldown ( InstanceData.Ability3, InstanceData.Ability2 );
					if ( InstanceData.Ability1.IsEnabled )
						InstanceData.Ability1.IsAvailable = true;

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
					// Mark that the ability is no longer active
					tagTeamPartner.InstanceData.Ability3.IsActive = false;

					// Reunite heroes
					tagTeamPartner.isSplit = false;
					tagTeamPartner.displaySprite = unitedDisplay;
					tagTeamPartner.sprite.sprite = unitedDisplay;
					GM.UI.matchInfoMenu.GetPlayerHUD ( this ).RemovePortrait ( InstanceID );

					// Start cooldown
					if ( tagTeamPartner.InstanceData.Ability2.IsEnabled )
					tagTeamPartner.StartToggleCooldown ( tagTeamPartner.InstanceData.Ability3, tagTeamPartner.InstanceData.Ability2, false );
					if ( tagTeamPartner.InstanceData.Ability1.IsEnabled )
						tagTeamPartner.InstanceData.Ability1.IsAvailable = true;

					// Remove status effect
					tagTeamPartner.Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.IN_UNISON, this );

					// Apply status effect
					tagTeamPartner.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.HASTE, StatusEffects.PERMANENT_EFFECT, this );

					// Update player hud
					GM.UI.matchInfoMenu.GetPlayerHUD ( Owner ).UpdateStatusEffects ( tagTeamPartner.InstanceID, tagTeamPartner.Status );

					// Pause turn timer
					if ( MatchSettings.TurnTimer )
						GM.UI.timer.ResumeTimer ( );

					// Remove hero
					Owner.UnitInstances.Remove ( this );
					currentTile.CurrentUnit = null;
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
