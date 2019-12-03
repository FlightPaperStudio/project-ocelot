using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Units
{
	public class HeroUnit : Unit
	{
		#region Hero Data

		protected AbilityInstanceData activeAbility;

		#endregion // Hero Data

		#region Public Unit Override Functions

		/// <summary>
		/// Determines how the unit should move based on the Move Data given.
		/// </summary>
		/// <param name="data"> The Move Data for the selected move. </param>
		public override void MoveUnit ( MoveData data )
		{
			// Check move data
			switch ( data.Type )
			{
			case MoveData.MoveType.MOVE:
				Move ( data );
				break;
			case MoveData.MoveType.JUMP:
				Jump ( data );
				if ( data.IsAssist )
					GetAssisted ( data );
				else if ( data.IsAttack )
					AttackUnit ( data );
				break;
			case MoveData.MoveType.SPECIAL:
				UseSpecial ( data );
				if ( data.IsAttack )
					AttackUnit ( data );
				break;
			}
		}

		#endregion // Unit Override Functions

		#region Public Virtual Functions

		/// <summary>
		/// Executes the command for the hero.
		/// </summary>
		public virtual void ExecuteCommand ( )
		{

		}

		/// <summary>
		/// Cancels the hero's command use.
		/// Base function returns the board to its non-command state.
		/// </summary>
		public virtual void CancelCommand ( )
		{
			// Mark that the ability is no longer active
			activeAbility.IsActive = false;
			activeAbility = null;

			// Clear the current board
			GM.Grid.ResetTiles ( );

			// Get available units
			GM.DisplayAvailableUnits ( );

			// Select current unit
			GM.SelectUnit ( GM.SelectedUnit );
		}

		/// <summary>
		/// Decrements the cooldown and duration for this unit's special ability or command ability.
		/// </summary>
		public virtual void Cooldown ( )
		{
			// Set the cooldown for ability 1
			if ( InstanceData.Ability1 != null )
				Cooldown ( InstanceData.Ability1 );

			// Set the cooldown for ability 2
			if ( InstanceData.Ability2 != null )
				Cooldown ( InstanceData.Ability2 );

			// Set the cooldown for ability 3
			if ( InstanceData.Ability3 != null )
				Cooldown ( InstanceData.Ability3 );
		}

		#endregion // Public Virtual Functions

		#region Public Functions

		/// <summary>
		/// Sets up the hero's command use.
		/// Base function clears the board for its command state.
		/// </summary>
		public void StartCommand ( AbilityInstanceData ability )
		{
			// Clear the current board
			GM.Grid.ResetTiles ( true );

			// Set the active ability
			ability.IsActive = true;
			activeAbility = ability;

			// Set the command data
			GM.SelectedCommand = SetCommandData ( );

			// Get targets
			GetCommandTargets ( );
		}

		/// <summary>
		/// Selects a particular tile for the setup of a command.
		/// This function should be called as many times as needed until all necessary tils are selected. The command should execute on the last call of this function.
		/// </summary>
		/// <param name="hex"> The selected tile for the command. </param>
		public void SelectCommandTile ( Hex hex )
		{
			// Mark tile as selected
			hex.Tile.SetTileState ( TileState.SelectedCommand );

			// Reset tiles
			GM.Grid.ResetTiles ( TileState.SelectedCommand );

			// Check for remaining targets to select
			if ( GM.SelectedCommand.Targets.Count < GM.SelectedCommand.MaxTargets )
				GetCommandTargets ( );
		}

		#endregion // Public Functions

		#region Protected Virtual Functions

		/// <summary>
		/// Sets the command data for the currently active command.
		/// Override this function to specify the number of targets for a command.
		/// </summary>
		/// <returns></returns>
		protected virtual CommandData SetCommandData ( )
		{
			// Set default command data
			return new CommandData ( this, 1 );
		}

		/// <summary>
		/// Determines any and all potential targets from a command ability.
		/// Override this function to specify how targets are selected.
		/// </summary>
		protected virtual void GetCommandTargets ( )
		{

		}

		/// <summary>
		/// Uses this hero unit's special ability.
		/// Override this function to call specific special ability functions for a hero unit.
		/// This function builds the animation queue from the Move Data.
		/// </summary>
		/// <param name="data"> The Move Data for the selected move. </param>
		protected virtual void UseSpecial ( MoveData data )
		{

		}

		/// <summary>
		/// Decrements the duration and cooldown for an ability.
		/// </summary>
		/// <param name="ability"> The instance data for the ability. </param>
		protected virtual void Cooldown ( AbilityInstanceData ability )
		{
			// Check for an active ability
			if ( ability.IsEnabled && ability.Type != AbilityData.AbilityType.PASSIVE )
			{
				// Check for active duration
				if ( ability.CurrentDuration > 0 )
				{
					// Decrement duration
					ability.CurrentDuration--;

					// Check if duration is complete
					if ( ability.CurrentDuration == 0 )
						OnDurationComplete ( ability );
				}

				// Check for active cooldown
				if ( ability.CurrentCooldown > 0 )
				{
					// Decrement cooldown
					ability.CurrentCooldown--;
				}
			}
		}

		/// <summary>
		/// Checks if this hero is capable of using a passive ability.
		/// Returns true if the passive ability is available.
		/// </summary>
		/// <param name="ability"> The ability data for the passive ability being checked. </param>
		/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given passive ability. </param>
		/// <returns> Whether or not the passive ability can be used. </returns>
		protected virtual bool PassiveAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
		{
			// Check if the ability is enabled
			if ( !ability.IsEnabled )
				return false;

			// Return that the ability is available
			return true;
		}

		/// <summary>
		/// Checks if this hero is capable of using a special ability.
		/// Returns true if the special ability is available.
		/// </summary>
		/// <param name="ability"> The ability data for the special ability being checked. </param>
		/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given special ability. </param>
		/// <returns> Whether or not the special ability can be used. </returns>
		protected virtual bool SpecialAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
		{
			// Check movement status effect
			if ( !Status.CanMove )
				return false;

			// Check ability status effect
			if ( !Status.CanUseAbility )
				return false;

			// Check if the ability is enabled
			if ( !ability.IsEnabled )
				return false;

			// Check if the ability is on cooldown
			if ( ability.CurrentCooldown > 0 )
				return false;

			// Return that the ability is available
			return true;
		}

		/// <summary>
		/// Checks if this hero is capable of using a command ability.
		/// Returns true if the command ability is available.
		/// </summary>
		/// <param name="ability"> The ability data for the command ability being checked. </param>
		/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given command ability. </param>
		/// <returns> Whether or not the command ability can be used. </returns>
		protected virtual bool CommandAvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
		{
			// Check if its the beginning of a player's turn
			if ( !GM.IsStartOfTurn )
				return false;

			// Check if moves have been plotted
			if ( prerequisite != null )
				return false;

			// Check status effects
			if ( !Status.CanUseAbility )
				return false;

			// Check if the ability is enabled
			if ( !ability.IsEnabled )
				return false;

			// Check if the ability is on cooldown
			if ( ability.CurrentCooldown > 0 )
				return false;

			// Return that the ability is available
			return true;
		}

		/// <summary>
		/// Checks if this hero is capable of using the first toggle command ability.
		/// Returns true if the toggle command ability is available.
		/// </summary>
		/// <param name="ability"> The ability data for the first toggle command ability. </param>
		/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given command ability. </param>
		/// <returns> Whether or not the command ability can be used. </returns>
		protected virtual bool ToggleCommand1AvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
		{
			// Check if its the beginning of a player's turn
			if ( !GM.IsStartOfTurn )
				return false;

			// Check if moves have been plotted
			if ( prerequisite != null )
				return false;

			// Check status effects
			if ( !Status.CanUseAbility )
				return false;

			// Check if the ability is enabled
			if ( !ability.IsEnabled )
				return false;

			// Check if the ability is on cooldown
			if ( ability.CurrentCooldown > 0 )
				return false;

			// Return that the ability is available
			return true;
		}

		/// <summary>
		/// Checks if this hero is capable of using the second toggle command ability.
		/// Returns true if the toggle command ability is available.
		/// </summary>
		/// <param name="ability"> The ability data for the second toggle command ability. </param>
		/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given command ability. </param>
		/// <returns> Whether or not the command ability can be used. </returns>
		protected virtual bool ToggleCommand2AvailabilityCheck ( AbilityInstanceData ability, MoveData prerequisite )
		{
			// Check if its the beginning of a player's turn
			if ( !GM.IsStartOfTurn )
				return false;

			// Check if moves have been plotted
			if ( prerequisite != null )
				return false;

			// Check status effects
			if ( !Status.CanUseAbility )
				return false;

			// Check if the ability is enabled
			if ( !ability.IsEnabled )
				return false;

			// Check if the ability is on cooldown
			if ( ability.CurrentCooldown > 0 )
				return false;

			// Return that the ability is available
			return true;
		}

		/// <summary>
		/// Callback for when the duration of an ability has expired.
		/// </summary>
		/// <param name="ability"> The current ability data for the ability whose duration has expired. </param>
		protected virtual void OnDurationComplete ( AbilityInstanceData ability )
		{
			ability.IsActive = false;
		}

		#endregion // Protected Virtual Functions

		#region Protected Functions

		/// <summary>
		/// Determines if any of the prerequisite moves used a special ability to move.
		/// Returns true if an special ability move was used.
		/// </summary>
		/// <param name="m"> The Move Data for any moves required for this move. </param>
		/// <returns> Whether or not a special ability move was used for the given move. </returns>
		protected bool PriorMoveTypeCheck ( MoveData m )
		{
			// Check for prerequisite move
			if ( m != null )
			{
				// Check if the type matches
				if ( m.Type == MoveData.MoveType.SPECIAL )
				{
					// Return that an ability has been used
					return true;
				}
				else
				{
					// Check prerequisite move's type
					return PriorMoveTypeCheck ( m.PriorMove );
				}
			}

			// Return that no abilities were used in previous moves
			return false;
		}

		/// <summary>
		/// Starts the cooldown and duration for this unit's special ability or command ability.
		/// </summary>
		/// <param name="ability"> The instance data for the ability being used. </param>
		/// <param name="updateHUD"> Whether or not the Unit HUD should be updated for this unit. </param>
		protected void StartCooldown ( AbilityInstanceData ability, bool updateHUD = true )
		{
			// Set duration
			ability.CurrentDuration = ability.Duration;

			// Set cooldown
			ability.CurrentCooldown = ability.Cooldown;

			// Display cooldown
			if ( updateHUD )
				GM.UI.UnitHUD.UpdateAbilityHUD ( ability );
		}

		/// <summary>
		/// Starts the cooldown and duration for this unit's toggle command ability.
		/// </summary>
		/// <param name="durationAbility"> The instance data for the toggle command ability being used. </param>
		/// <param name="cooldownAbility"> The instance data for the toggle command ability not being used. </param>
		/// <param name="updateHUD"> Whether or not the Unit HUD should be updated for this unit. </param>
		//protected void StartToggleCooldown ( AbilityInstanceData durationAbility, AbilityInstanceData cooldownAbility, bool updateHUD = true )
		//{
		//	// Set duration
		//	if ( durationAbility.IsEnabled )
		//		durationAbility.CurrentDuration = durationAbility.Duration;

		//	// Set cooldown
		//	if ( cooldownAbility.IsEnabled )
		//		cooldownAbility.CurrentCooldown = cooldownAbility.Cooldown;

		//	// Display cooldown
		//	if ( updateHUD )
		//	{
		//		if ( durationAbility.IsEnabled )
		//			GM.UI.unitHUD.UpdateAbilityHUD ( durationAbility );
		//		if ( cooldownAbility.IsEnabled )
		//			GM.UI.unitHUD.UpdateAbilityHUD ( cooldownAbility );
		//	}	
		//}

		/// <summary>
		/// Creates the hero's tile object in the arena.
		/// </summary>
		/// <param name="prefab"> The tile object to be created. </param>
		/// <param name="hex"> The tile where the tile object is being placed. </param>
		/// <param name="duration"> The amount of turns the tile object will exist for. </param>
		/// <param name="durationDelegate"> The delegate for when the tile object's duration expires. </param>
		/// <param name="attackDelegate"> The delegate for when the tile object is attack. </param>
		protected TileObject CreateTileOject ( TileObject prefab, Hex hex, int duration, TileObject.TileObjectDelegate durationDelegate, TileObject.TileObjectDelegate attackDelegate = null )
		{
			// Create game object
			TileObject obj = Instantiate ( prefab, Owner.transform );

			// Set tile object information
			obj.SetTileObject ( this, hex, duration, durationDelegate, attackDelegate );

			// Add tile object to player's list
			Owner.TileObjects.Add ( obj );

			// Add tile object to tile
			hex.Tile.CurrentObject = obj;

			// Set sprite direction
			Tools.Util.OrientSpriteToDirection ( obj.Icon, Owner.TeamDirection );

			// Return the newly created tile object
			return obj;
		}

		/// <summary>
		/// Removes the hero's tile object from the arena.
		/// </summary>
		/// <param name="current"> The tile object instance being destroyed. </param>
		protected void DestroyTileObject ( TileObject current )
		{
			// Remove tile object from player's list
			Owner.TileObjects.Remove ( current );

			// Remove tile object from tile
			current.CurrentHex.Tile.CurrentObject = null;

			// Destroy game object
			Destroy ( current.gameObject );

			// Remove tile object reference
			current = null;
		}

		#endregion // Protected Functions
	}
}