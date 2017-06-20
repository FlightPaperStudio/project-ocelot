using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroUnit : Unit 
{
	// Hero information
	public int heroID;
	public Hero info
	{
		get;
		private set;
	}
	public Sprite abilitySprite1;
	public Sprite abilitySprite2;

	// Current ability info
	public AbilitySettings currentAbility1
	{
		get;
		protected set;
	}
	public AbilitySettings currentAbility2
	{
		get;
		protected set;
	}

	/// <summary>
	/// Initializes this hero unit instance.
	/// </summary>
	protected virtual void Start ( )
	{
		// Hero information
		info = HeroInfo.GetHeroByID ( heroID );
		characterName = info.characterName;

		// Set ability settings
		info.ability1.duration = MatchSettings.GetHeroSettingsByID ( heroID ).ability1.duration;
		info.ability1.cooldown = MatchSettings.GetHeroSettingsByID ( heroID ).ability1.cooldown;
		info.ability2.duration = MatchSettings.GetHeroSettingsByID ( heroID ).ability2.duration;
		info.ability2.cooldown = MatchSettings.GetHeroSettingsByID ( heroID ).ability2.cooldown;

		// Set current abilities
		currentAbility1 = new AbilitySettings ( MatchSettings.GetHeroSettingsByID ( heroID ).ability1.enabled, MatchSettings.GetHeroSettingsByID ( heroID ).ability1.type, MatchSettings.GetHeroSettingsByID ( heroID ).ability1.duration, MatchSettings.GetHeroSettingsByID ( heroID ).ability1.cooldown );
		currentAbility2 = new AbilitySettings ( MatchSettings.GetHeroSettingsByID ( heroID ).ability2.enabled, MatchSettings.GetHeroSettingsByID ( heroID ).ability2.type, MatchSettings.GetHeroSettingsByID ( heroID ).ability2.duration, MatchSettings.GetHeroSettingsByID ( heroID ).ability2.cooldown );

		// Set that the ability cooldown is not active at the start
		if ( currentAbility1.type == Ability.AbilityType.Special || currentAbility1.type == Ability.AbilityType.Command )
		{
			currentAbility1.duration = 0;
			currentAbility1.cooldown = 0;
		}
		if ( currentAbility2.type == Ability.AbilityType.Special || currentAbility2.type == Ability.AbilityType.Command )
		{
			currentAbility2.duration = 0;
			currentAbility2.cooldown = 0;
		}
	}

	/// <summary>
	/// Determines how the unit should move based on the Move Data given.
	/// </summary>
	public override void MoveUnit ( MoveData data )
	{
		// Check move data
		switch ( data.type )
		{
		case MoveData.MoveType.Move:
			Move ( data );
			break;
		case MoveData.MoveType.Jump:
			Jump ( data );
			break;
		case MoveData.MoveType.Attack:
			Jump ( data );
			AttackUnit ( data );
			break;
		case MoveData.MoveType.Special:
			UseSpecial ( data );
			break;
		case MoveData.MoveType.SpecialAttack:
			UseSpecial ( data );
			AttackUnit ( data );
			break;
		}
	}

	/// <summary>
	/// Checks if the hero is capable of using a special ability.
	/// Returns true if the special ability is available.
	/// </summary>
	protected virtual bool SpecialAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check movement status effect
		if ( !status.canMove )
			return false;

		// Check ability status effect
		if ( !status.canUseAbility )
			return false;

		// Check if the ability is enabled
		if ( !current.enabled )
			return false;

		// Check if the ability is on cooldown
		if ( current.cooldown > 0 )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// This function builds the animation queue from the move data.
	/// </summary>
	protected virtual void UseSpecial ( MoveData data )
	{

	}

	/// <summary>
	/// Checks if the hero is capable of using a command ability.
	/// Returns true if the command ability is available.
	/// </summary>
	protected virtual bool CommandAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check if its the beginning of a player's turn
		if ( !GM.isStartOfTurn )
			return false;

		// Check if moves have been plotted
		if ( prerequisite != null )
			return false;

		// Check status effects
		if ( !status.canUseAbility )
			return false;

		// Check if the ability is enabled
		if ( !current.enabled )
			return false;

		// Check if the ability is on cooldown
		if ( current.cooldown > 0 )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Sets up the hero's command use.
	/// Base function clears the board for its command state.
	/// </summary>
	public virtual void StartCommand ( )
	{
		// Clear the current board
		GM.board.ResetTiles ( );

		// Highlight current tile
		currentTile.SetTileState ( TileState.SelectedUnit );
	}

	/// <summary>
	/// Selects a particular tile for the setup of a command.
	/// This function should be called as many times as needed until all necessary tils are selected. The command should execute on the last call of this function.
	/// </summary>
	public virtual void SelectCommandTile ( Tile t )
	{

	}

	/// <summary>
	/// Cancels the hero's command use.
	/// Base function returns the board to its non-command state.
	/// </summary>
	public virtual void EndCommand ( )
	{
		// Clear the current board
		GM.board.ResetTiles ( );

		// Get available units
		GM.DisplayAvailableUnits ( );

		// Select current unit
		GM.SelectUnit ( GM.selectedUnit );
	}

	/// <summary>
	/// Starts the cooldown for the unit's special ability.
	/// </summary>
	protected void StartCooldown ( AbilitySettings current, Ability setting, bool updateHUD = true )
	{
		// Set duration
		current.duration = setting.duration;

		// Set cooldown
		current.cooldown = setting.cooldown;

		// Display cooldown
		if ( updateHUD )
			GM.UI.unitHUD.DisplayAbility ( current );
	}

	/// <summary>
	/// Decrements the cooldown for the unit's special ability.
	/// </summary>
	public virtual void Cooldown ( )
	{
		// Check for active ability type for ability 1
		if ( currentAbility1.enabled && currentAbility1.type != Ability.AbilityType.Passive )
		{
			// Check if current duration is active
			if ( currentAbility1.duration > 0 )
			{
				// Decrement duration
				currentAbility1.duration--;

				// Check if duration is complete
				if ( currentAbility1.duration == 0 )
					OnDurationComplete ( currentAbility1 );
			}

			// Check if current cooldown is active
			if ( currentAbility1.cooldown > 0 )
			{
				// Decrement cooldown
				currentAbility1.cooldown--;
			}
		}

		// Check for active ability type for ability 2
		if ( currentAbility2.enabled && currentAbility2.type != Ability.AbilityType.Passive )
		{
			// Check if current duration is active
			if ( currentAbility2.duration > 0 )
			{
				// Decrement duration
				currentAbility2.duration--;

				// Check if duration is complete
				if ( currentAbility2.duration == 0 )
					OnDurationComplete ( currentAbility2 );
			}

			// Check if current cooldown is active
			if ( currentAbility2.cooldown > 0 )
			{
				// Decrement cooldown
				currentAbility2.cooldown--;
			}
		}
	}

	/// <summary>
	/// Callback for when the duration of an ability has expired.
	/// </summary>
	protected virtual void OnDurationComplete ( AbilitySettings current )
	{

	}

	/// <summary>
	/// Creates the hero's tile object in the arena.
	/// </summary>
	protected TileObject CreateTileOject ( TileObject prefab, Tile t, int duration, TileObject.TileObjectDelegate tileObjectDelegate )
	{
		// Create game object
		TileObject obj = Instantiate ( prefab, owner.transform );

		// Set tile object information
		obj.SetTileObject ( this, t, duration, tileObjectDelegate );

		// Add tile object to player's list
		owner.tileObjects.Add ( obj );

		// Add tile object to tile
		t.currentObject = obj;

		// Set sprite direction
		if ( owner.direction == Player.Direction.RightToLeft || owner.direction == Player.Direction.BottomRightToTopLeft || owner.direction == Player.Direction.TopRightToBottomLeft )
			obj.sprite.flipX = true;

		// Return the newly created tile object
		return obj;
	}

	/// <summary>
	/// Removes the hero's tile object from the arena.
	/// </summary>
	protected void DestroyTileObject ( TileObject current )
	{
		// Remove tile object from player's list
		owner.tileObjects.Remove ( current );

		// Remove tile object from tile
		current.tile.currentObject = null;

		// Destroy game object
		Destroy ( current.gameObject );

		// Remove tile object reference
		current = null;
	}
}
