using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TagTeam : HeroUnit
{
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///
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
	/// ----------------------------------------------------------------------------------------------------------------------------------------------------------------- ///

	#region Ability Data
	
	private TagTeam tagTeamPartner;
	private TagTeam originalPartner;
	private bool isSplit = false;
	private bool hasInUnisonMove = false;

	private IReadOnlyUnitData duoData;
	private IReadOnlyUnitData partner1Data;
	private IReadOnlyUnitData partner2Data;

	#endregion // Ability Data

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
		{
			StartCooldown ( InstanceData.Ability2 );
			StartCooldown ( InstanceData.Ability3 );
		}

		// Store unit data for transitions
		duoData = MatchSettings.GetUnitData ( ID );
		partner1Data = MatchSettings.GetUnitData ( ID + 1 );
		partner2Data = MatchSettings.GetUnitData ( ID + 2 );

		// Update unit data
		if ( isSplit )
			UpdateUnitData ( partner2Data );
	}

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	/// <param name="hex"> The tile who's neighbor will be checked for moves. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach this tile. </param>
	/// <param name="returnOnlyJumps"> Whether or not only jump moves should be stored as available moves. </param>
	public override void FindMoves ( Hex hex, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Clear previous move list
		if ( prerequisite == null )
			MoveList.Clear ( );

		// Check status effects
		if ( Status.CanMove )
		{
			// Check each neighboring tile
			for ( int i = 0; i < hex.Neighbors.Length; i++ )
			{
				// Check if this unit can move to the neighboring tile
				if ( OccupyTileCheck ( hex.Neighbors [ i ], prerequisite ) )
				{
					// Check if only jumps are being returned
					if ( returnOnlyJumps )
					{
						// Check for Tag Team move
						if ( PassiveAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
						{
							// Add as an available Tag Team move
							AddMove ( new MoveData ( hex.Neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i ) );
						}
					}
					else
					{
						// Add as an available move
						MoveData move = new MoveData ( hex.Neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i );
						AddMove ( move );

						// Check for additional move with the Tag Team ability
						if ( PassiveAvailabilityCheck ( InstanceData.Ability1, prerequisite ) )
							FindMoves ( hex.Neighbors [ i ], move, true );
					}
				}
				// Check if this unit can jump the neighboring tile
				else if ( JumpTileCheck ( hex.Neighbors [ i ] ) && OccupyTileCheck ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData move;

					// Check if the neighboring unit can be attacked
					if ( AttackTileCheck ( hex.Neighbors [ i ] ) )
					{
						// Check for Tag Team move
						if ( PassiveAvailabilityCheck ( InstanceData.Ability1, prerequisite ) && prerequisite != null && prerequisite.Type == MoveData.MoveType.MOVE )
						{
							// Add as an available attack with Tag Team
							move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i, null, hex.Neighbors [ i ] );
						}
						else
						{
							// Add as an available attack
							move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i, null, hex.Neighbors [ i ] );
						}
					}
					else
					{
						// Check for Tag Team move
						if ( PassiveAvailabilityCheck ( InstanceData.Ability1, prerequisite ) && prerequisite != null && prerequisite.Type == MoveData.MoveType.MOVE )
						{
							// Add as an available jump with Tag Team
							move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite, MoveData.MoveType.SPECIAL, i, hex.Neighbors [ i ].Neighbors [ i ], null );
						}
						else
						{
							// Add as an available jump
							move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i, hex.Neighbors [ i ].Neighbors [ i ], null );
						}
					}

					// Add move to the move list
					AddMove ( move );

					// Find additional jumps
					if ( InstanceData.Ability1.IsPerkEnabled || ( move.Type != MoveData.MoveType.SPECIAL ) )
						FindMoves ( hex.Neighbors [ i ].Neighbors [ i ], move, true );
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
		if ( PartnerMoveCheck ( data ) )
		{
			// Mark that the divide and conquer move has been used
			hasInUnisonMove = true;
			tagTeamPartner.hasInUnisonMove = true;

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

		// Mark that the the In Unison move has not been used
		hasInUnisonMove = false;
	}

	public override void ExecuteCommand ( )
	{
		// Check if the hero is split
		if ( isSplit )
		{
			// Reunite heroes
			Regroup ( GM.SelectedCommand.PrimaryTarget );
		}
		else
		{
			// Split heroes
			SplitUp ( GM.SelectedCommand.PrimaryTarget );
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
		if ( !ToggleCommandCheck ( ) )
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
		if ( !ToggleCommandCheck ( ) )
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
		if ( data.IsAttack )
		{
			// Create animation
			Tween t = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME * 2 )
				.OnStart ( ( ) =>
				{
					// Mark that the ability is active
					InstanceData.Ability1.IsActive = true;
					GM.UI.UnitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
				} )
				.OnComplete ( ( ) =>
				{
					// Mark that the ability is no longer active
					InstanceData.Ability1.IsActive = false;
					GM.UI.UnitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );

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
			if ( data.PriorMove == null && CurrentHex.Neighbors [ (int)data.Direction ] == data.Destination )
			{
				// Create animation
				Tween t = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME )
					.OnStart ( ( ) =>
					{
						// Mark that the ability is active
						InstanceData.Ability1.IsActive = true;
						GM.UI.UnitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
					} )
					.OnComplete ( ( ) =>
					{
						// Mark that the ability is no longer active
						InstanceData.Ability1.IsActive = false;
						GM.UI.UnitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );

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
						GM.UI.UnitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );
					} )
					.OnComplete ( ( ) =>
					{
						// Mark that the ability is no longer active
						InstanceData.Ability1.IsActive = false;
						GM.UI.UnitHUD.UpdateAbilityHUD ( InstanceData.Ability1 );

						// Set unit and tile data
						SetUnitToTile ( data.Destination );
					} );

				// Add animation to queue
				GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
			}
		}
	}

	protected override void GetCommandTargets ( )
	{
		// Check if the hero is split
		if ( isSplit )
		{
			// Highlight partner
			tagTeamPartner.CurrentHex.Tile.SetTileState ( TileState.AvailableCommand );
		}
		else
		{
			// Check each neighboring tile
			for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
			{
				// Highlight available tiles
				if ( OccupyTileCheck ( CurrentHex.Neighbors [ i ], null ) )
					CurrentHex.Neighbors [ i ].Tile.SetTileState ( TileState.AvailableCommand );
			}
		}
	}

	#endregion // Protected HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Checks if the conditions are met to use the Split Up or Regroup ability.
	/// Returns true if the ability can be used.
	/// </summary>
	/// <returns> Whether or not the Divided And Conquer ability can be used. </returns>
	private bool ToggleCommandCheck ( )
	{
		// Check each neighboring tile
		for ( int i = 0; i < CurrentHex.Neighbors.Length; i++ )
		{
			// Check if the hero is split
			if ( isSplit )
			{
				// Check if partner exists
				if ( tagTeamPartner == null )
					continue;

				// Check if partner is still on the same team
				if ( tagTeamPartner.Owner != Owner )
					continue;

				// Check for tile
				if ( CurrentHex.Neighbors [ i ] == null )
					continue;

				// Check if the tile is occupied
				if ( CurrentHex.Neighbors [ i ].Tile.CurrentUnit == null )
					continue;

				// Check if partner is on tile
				if ( CurrentHex.Neighbors [ i ].Tile.CurrentUnit != tagTeamPartner )
					continue;

				// Return that the command is available
				return true;
			}
			else
			{
				// Check for available tile
				if ( OccupyTileCheck ( CurrentHex.Neighbors [ i ], null ) )
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
	private bool PartnerMoveCheck ( MoveData data )
	{
		// Check if ability is enabled
		if ( !InstanceData.Ability2.IsEnabled )
			return false;

		// Check if perk is enabled
		if ( !InstanceData.Ability2.IsPerkEnabled )
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

		// Check for status effect
		if ( !Status.HasStatusEffect ( StatusEffectDatabase.StatusEffectType.IN_UNISON ) || !tagTeamPartner.Status.HasStatusEffect ( StatusEffectDatabase.StatusEffectType.IN_UNISON ) )
			return false;
		
		// Check if its the first move
		if ( data.PriorMove != null )
			return false;
		
		// Check if the Divide And Conquer move has already be used this turn
		if ( hasInUnisonMove )
			return false;
		
		// Return that the tag team partner needs to be added to the unit queue
		return true;
	}

	/// <summary>
	/// Splits the hero into two individual hero units.
	/// </summary>
	/// <param name="hex"> The tile where the seperate tag team partner will be placed. </param>
	private void SplitUp ( Hex hex )
	{
		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.UnitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Grid.ResetTiles ( );

		// Create partner instance
		tagTeamPartner = Instantiate ( this, Owner.transform );
		tagTeamPartner.transform.position = transform.position;

		// Add partner to team
		Owner.UnitInstances.Add ( tagTeamPartner );

		// Set that both partners are split
		isSplit = true;
		tagTeamPartner.isSplit = true;

		// Set original partner
		originalPartner = this;
		tagTeamPartner.originalPartner = this;

		// Set new partner
		tagTeamPartner.tagTeamPartner = this;

		// Update each partner's unit 
		UpdateUnitData ( partner1Data );
		GM.UI.matchInfoMenu.GetPlayerHUD ( this ).AddPortrait ( tagTeamPartner, InstanceID * 10 );
		tagTeamPartner.InitializeInstance ( GM, InstanceID * 10, Owner.Units.Find ( x => x.ID == duoData.ID ) );

		// Create animation
		Tween t = tagTeamPartner.transform.DOMove ( hex.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Mark that the ability is no longer active
				InstanceData.Ability2.IsActive = false;
				
				// Set unit and tile data
				tagTeamPartner.CurrentHex = hex;
				hex.Tile.CurrentUnit = tagTeamPartner;
				
				// Start cooldown
				StartCooldown ( InstanceData.Ability2 );
				StartCooldown ( InstanceData.Ability3 );
				if ( InstanceData.Ability1.IsEnabled )
					InstanceData.Ability1.IsAvailable = false;
				
				// Remove status effect
				Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.HASTE, this );
				tagTeamPartner.Status.RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType.HASTE, tagTeamPartner );
				
				// Apply status effect
				Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.IN_UNISON, InstanceData.Ability2.Duration, this );
				tagTeamPartner.Status.AddStatusEffect ( StatusEffectDatabase.StatusEffectType.IN_UNISON, InstanceData.Ability2.Duration, this );
				
				// Update HUDs
				GM.UI.UnitHUD.UpdateStatusEffects ( );
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdateStatusEffects ( InstanceID, Status );
				//GM.UI.matchInfoMenu.GetPlayerHUD ( tagTeamPartner ).UpdateStatusEffects ( tagTeamPartner.InstanceID, tagTeamPartner.Status );
				
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
	/// <param name="hex"> The tile where the two tag team partners are reuniting. </param>
	private void Regroup ( Hex hex )
	{
		// Pause turn timer
		if ( MatchSettings.TurnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.UnitHUD.HideCancelButton ( InstanceData.Ability2 );

		// Clear board
		GM.Grid.ResetTiles ( );

		// Create animation
		Tween t = transform.DOMove ( hex.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Check for original
				if ( originalPartner == this )
				{
					// Mark that the ability is no longer active
					InstanceData.Ability3.IsActive = false;

					// Reunite heroes
					isSplit = false;
					UpdateUnitData ( duoData );
					displaySprite = InstanceData.Portrait;
					sprite.sprite = InstanceData.Portrait;
					GM.UI.matchInfoMenu.GetPlayerHUD ( tagTeamPartner ).RemovePortrait ( tagTeamPartner.InstanceID );

					// Remove partner
					Owner.UnitInstances.Remove ( tagTeamPartner );
					hex.Tile.CurrentUnit = null;
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
					SetUnitToTile ( hex );

					// Start cooldown
					if ( InstanceData.Ability2.IsEnabled )
					{
						StartCooldown ( InstanceData.Ability2 );
						StartCooldown ( InstanceData.Ability3 );
					}
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
					tagTeamPartner.UpdateUnitData ( duoData );
					tagTeamPartner.displaySprite = tagTeamPartner.InstanceData.Portrait;
					tagTeamPartner.sprite.sprite = tagTeamPartner.InstanceData.Portrait;
					GM.UI.matchInfoMenu.GetPlayerHUD ( this ).RemovePortrait ( InstanceID );

					// Start cooldown
					if ( tagTeamPartner.InstanceData.Ability2.IsEnabled )
					{
						tagTeamPartner.StartCooldown ( InstanceData.Ability2 );
						tagTeamPartner.StartCooldown ( InstanceData.Ability3 );
					}
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
					CurrentHex.Tile.CurrentUnit = null;
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
