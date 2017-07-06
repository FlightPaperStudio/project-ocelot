using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects
{
	// Status effect information
	public enum StatusType
	{
		CanMove,
		CanBeMoved,
		CanUseAbility
	}

	// Determines if the unit can move
	public bool canMove
	{
		get;
		private set;
	}
	private int canMoveStack = 0;

	// Determines if the unit can be moved by abilities
	public bool canBeMoved
	{
		get;
		private set;
	}
	private int canBeMovedStack = 0;

	// Determines if the unit can use abilities
	public bool canUseAbility
	{
		get;
		private set;
	}
	private int canUseAbilityStack = 0;

	// All of the information to needed to add a new status effect to a unit.
	public struct EffectInfo
	{
		public Sprite icon;
		public string text;
		public Unit caster;

		public EffectInfo ( Sprite _icon, string _text, Unit _caster )
		{
			icon = _icon;
			text = _text;
			caster = _caster;
		}
	}
	public class StatusEffect
	{
		public EffectInfo info;
		public int duration;
		public StatusType [ ] effects;

		public StatusEffect ( EffectInfo _info, int _duration, params StatusType [ ] _effects )
		{
			info = _info;
			duration = _duration;
			effects = _effects;
		}
	}
	public List<StatusEffect> effects = new List<StatusEffect> ( );

	public StatusEffects ( )
	{
		// Set default values
		canMove = true;
		canBeMoved = true;
		canUseAbility = true;
	}

	/// <summary>
	/// Adds a new status effect to the stack.
	/// </summary>
	public void AddStatusEffect ( Sprite _icon, string _text, Unit _caster, int _duration, params StatusType [ ] _effects )
	{
		// Create new status effect
		EffectInfo effectInfo = new EffectInfo ( _icon, _text, _caster );
		StatusEffect statusEffect = new StatusEffect ( effectInfo, _duration, _effects );

		// Check for existing status effect
		if ( effects.Exists ( match => match.info.icon == _icon && match.info.text == _text && match.effects == _effects ) )
		{
			// Refressh status effect
			StatusEffect e = effects.Find ( match => match.info.icon == _icon && match.info.text == _text && match.effects == _effects );
			e.info = effectInfo;
			e.duration = _duration;
		}
		else
		{
			// Apply status effect
			effects.Add ( statusEffect );

			// Increment each effect
			foreach ( StatusType type in statusEffect.effects )
				IncrementEffect ( type );

			// Update the current status
			UpdateStatus ( );
		}
	}

	/// <summary>
	/// Increments the current stack of a particular status type.
	/// </summary>
	private void IncrementEffect ( StatusType _type )
	{
		// Check type and increment the corrisponding stack
		switch ( _type )
		{
		case StatusType.CanMove:
			canMoveStack++;
			break;
		case StatusType.CanBeMoved:
			canBeMovedStack++;
			break;
		case StatusType.CanUseAbility:
			canUseAbilityStack++;
			break;
		}
	}

	/// <summary>
	/// Updates the current status by calculating the current stack of each status.
	/// The default state of each status is true, which is when its current stack is empty at 0.
	/// </summary>
	private void UpdateStatus ( )
	{
		// Set Can Move status
		canMove = canMoveStack == 0;

		// Set Can Be Moved status
		canBeMoved = canBeMovedStack == 0;

		// Set Can Use Ability status
		canUseAbility = canUseAbilityStack == 0;
	}

	/// <summary>
	/// Decrements the duration of each status effect currently applied.
	/// </summary>
	public void UpdateDurations ( )
	{
		// Check each applied status effect
		foreach ( StatusEffect e in effects )
		{
			// Decrement duration
			e.duration--;

			// Check for expired duration
			if ( e.duration <= 0 )
			{
				// Decrement each effect for the expired status effect
				foreach ( StatusType type in e.effects )
					DecrementEffect ( type );
			}
		}

		// Remove any expired status effects
		effects.RemoveAll ( match => match.duration == 0 );

		// Update the current status
		UpdateStatus ( );
	}

	/// <summary>
	/// Decrements the current stack of a particular status type.
	/// </summary>
	private void DecrementEffect ( StatusType _type )
	{
		// Check type and decrement the corrisponding stack
		switch ( _type )
		{
		case StatusType.CanMove:
			canMoveStack = DecrementStack ( canMoveStack );
			break;
		case StatusType.CanBeMoved:
			canBeMovedStack = DecrementStack ( canBeMovedStack );
			break;
		case StatusType.CanUseAbility:
			canUseAbilityStack = DecrementStack ( canUseAbilityStack );
			break;
		}
	}

	/// <summary>
	/// Decrements a stack and checks for negative overflow.
	/// </summary>
	private int DecrementStack ( int _stack )
	{
		// Decrement stack
		_stack--;

		// Check for negative overflow
		if ( _stack < 0 )
			_stack = 0;

		// Return stack value
		return _stack;
	}

	/// <summary>
	/// Finds and removes an applied status effect.
	/// Use this in case of interupts.
	/// </summary>
	public void RemoveStatusEffect ( Sprite _icon, string _text, Unit _caster, params StatusType [ ] _effects )
	{
		// Find status effect
		EffectInfo effectInfo = new EffectInfo ( _icon, _text, _caster );
		if ( effects.Exists ( match => match.info.Equals ( effectInfo ) && match.effects == _effects ) )
		{
			// Get the status effect
			StatusEffect e = effects.Find ( match => match.info.Equals ( effectInfo ) && match.effects == _effects );

			// Decrement each effect for status effect
			foreach ( StatusType type in e.effects )
				DecrementEffect ( type );

			// Update the current status
			UpdateStatus ( );

			// Remove the status effect
			effects.Remove ( e );
		}
	}
}
