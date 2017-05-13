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
		case MoveData.MoveType.JumpCapture:
			CaptureUnit ( data );
			Jump ( data );
			break;
		case MoveData.MoveType.Special:
			UseSpecial ( data );
			break;
		case MoveData.MoveType.SpecialCapture:
			CaptureUnit ( data );
			UseSpecial ( data );
			break;
		}
	}

	/// <summary>
	/// Captures this unit.
	/// Call this function on the unit being captured.
	/// </summary>
	public override void GetCaptured ( bool lostMatch = false )
	{
		// Display deactivation
		GM.UI.hudDic [ team ].DisplayDeactivation ( instanceID );

		// Get captured
		base.GetCaptured ( lostMatch );
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	protected virtual void UseSpecial ( MoveData data )
	{

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
	protected void StartCooldown ( AbilitySettings current, Ability setting )
	{
		// Set duration
		current.duration = setting.duration;

		// Set cooldown
		current.cooldown = setting.cooldown;

		// Display cooldown
		GM.UI.unitHUD.DisplayAbility ( current );
	}

	/// <summary>
	/// Decrements the cooldown for the unit's special ability.
	/// </summary>
	public virtual void Cooldown ( )
	{
		// Check for active ability type for ability 1
		if ( currentAbility1.type != Ability.AbilityType.Passive )
		{
			// Check if current duration is active
			if ( currentAbility1.duration > 0 )
			{
				// Decrement duration
				currentAbility1.duration--;

				// Check if duration is complete
				//if ( currentAbility1.duration == 0 )
			}

			// Check if current cooldown is active
			if ( currentAbility1.cooldown > 0 )
			{
				// Decrement cooldown
				currentAbility1.cooldown--;
			}
		}

		// Check for active ability type for ability 2
		if ( currentAbility2.type != Ability.AbilityType.Passive )
		{
			// Check if current duration is active
			if ( currentAbility2.duration > 0 )
			{
				// Decrement duration
				currentAbility2.duration--;

				// Check if duration is complete
				//if ( currentAbility2.duration == 0 )
			}

			// Check if current cooldown is active
			if ( currentAbility2.cooldown > 0 )
			{
				// Decrement cooldown
				currentAbility2.cooldown--;
			}
		}
	}
}
