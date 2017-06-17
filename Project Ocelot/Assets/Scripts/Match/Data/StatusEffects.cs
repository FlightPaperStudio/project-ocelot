using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects
{
	// Status effect type
	public enum StatusType
	{
		On,
		Neutral,
		Off
	}

	// Determines if the unit can move
	public bool canMove
	{
		get;
		private set;
	}
	private int canMoveCount = 0;

	// Determines if the unit can be moved by abilities
	public bool canBeMoved
	{
		get;
		private set;
	}
	private int canBeMovedCount = 0;

	// Determines if the unit can use abilities
	public bool canUseAbility
	{
		get;
		private set;
	}
	private int canUseAbilityCount = 0;

	// Determines if the unit can receive the effects of abilities from ally untis
	public bool canReceiveAbilityEffectsFriendly
	{
		get;
		private set;
	}
	private int canReceiveAbilityEffectsFriendlyCount = 0;

	// Determines if the unit can receive the effects of abilities from enemy units
	public bool canReceiveAbilityEffectsHostile
	{
		get;
		private set;
	}
	private int canReceiveAbilityEffectsHostileCount = 0;

	public StatusEffects ( )
	{
		// Set default values
		canMove = true;
		canBeMoved = true;
		canUseAbility = true;
		canReceiveAbilityEffectsFriendly = true;
		canReceiveAbilityEffectsHostile = true;
	}

	/// <summary>
	/// Updates the current status by adding new status effects to the stack.
	/// On adds a positive effect to the stack.
	/// Off adds a negative effect to the stack.
	/// Neutral adds no effect to the stack.
	/// The default state of a status is true, which is when the stack is empty at 0.
	/// </summary>
	public void UpdateStatus ( StatusType _canMove, StatusType _canBeMoved, StatusType _canUseAbility, StatusType _canReceiveAbilityEffectsFriendly, StatusType _canReceiveAbilityEffectsHostile )
	{
		// Set Can Move status
		canMoveCount = CalculateStatus ( canMoveCount, _canMove );
		canMove = canMoveCount == 0;

		// Set Can Be Moved status
		canBeMovedCount = CalculateStatus ( canBeMovedCount, _canBeMoved );
		canBeMoved = canBeMovedCount == 0;

		// Set Can Use Ability status
		canUseAbilityCount = CalculateStatus ( canUseAbilityCount, _canUseAbility );
		canUseAbility = canUseAbilityCount == 0;

		// Set Can Receive Friendly Ability Effects status
		canReceiveAbilityEffectsFriendlyCount = CalculateStatus ( canReceiveAbilityEffectsFriendlyCount, _canReceiveAbilityEffectsFriendly );
		canReceiveAbilityEffectsFriendly = canReceiveAbilityEffectsFriendlyCount == 0;

		// Set Can Receive Hostile Ability Effects status
		canReceiveAbilityEffectsHostileCount = CalculateStatus ( canReceiveAbilityEffectsHostileCount, _canReceiveAbilityEffectsHostile );
		canReceiveAbilityEffectsHostile = canReceiveAbilityEffectsHostileCount == 0;
	}

	/// <summary>
	/// Calculates how a new status effect adds to the current status stack.
	/// </summary>
	private int CalculateStatus ( int _count, StatusType _type )
	{
		// Check type
		if ( _type == StatusType.On )
			_count++;
		else if ( _type == StatusType.Off )
			_count--;

		// Check for positive values
		if ( _count > 0 )
			_count = 0;

		// Return status
		return _count;
	}
}
