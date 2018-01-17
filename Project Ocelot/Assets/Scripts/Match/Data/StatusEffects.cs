using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects
{
	#region Status Effect Data

	// Status effect information
	public enum StatusType
	{
		CAN_MOVE,
		CAN_BE_MOVED,
		CAN_USE_ABILITY
	}

	/// <summary>
	/// Determines if the unit is able to move.
	/// </summary>
	public bool CanMove
	{
		get
		{
			return canMoveStack == 0;
		}
	}
	private int canMoveStack = 0;

	/// <summary>
	/// Determines if the unit is able to be moved by abilities.
	/// </summary>
	public bool CanBeMoved
	{
		get
		{
			return canBeMovedStack == 0;
		}
	}
	private int canBeMovedStack = 0;

	/// <summary>
	/// Determines if the unit is able to use abilities.
	/// </summary>
	public bool CanUseAbility
	{
		get
		{
			return canUseAbilityStack == 0;
		}
	}
	private int canUseAbilityStack = 0;

	// All of the information needed to add a new status effect to a unit.
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

	#endregion // Status Effect Data

	#region Public Functions

	public StatusEffects ( )
	{
		// Set default values
		canMoveStack = 0;
		canBeMovedStack = 0;
		canUseAbilityStack = 0;
	}

	/// <summary>
	/// Adds a new status effect to the stack.
	/// </summary>
	/// <param name="_icon"> The icon representing the status effect. </param>
	/// <param name="_text"> The text representing the status effect. </param>
	/// <param name="_caster"> The unit applying the status effect to the affected unit. </param>
	/// <param name="_duration"> The number of turns the status effect lasts. </param>
	/// <param name="_effects"> Any standard effects applied by the status effect. </param>
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
		}
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
	}

	/// <summary>
	/// Finds and removes an applied status effect.
	/// Use this in case of interupts.
	/// </summary>
	/// <param name="_icon"> The icon representing the status effect. </param>
	/// <param name="_text"> The text representing the status effect. </param>
	/// <param name="_caster"> The unit that applied the status effect to the affect unit. </param>
	/// <param name="_effects"> Any standard effects applied by the status effect. </param>
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

			// Remove the status effect
			effects.Remove ( e );
		}
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Increments the current stack of a particular status type.
	/// </summary>
	/// <param name="_type"> The status effect type being incremented. </param>
	private void IncrementEffect ( StatusType _type )
	{
		// Check type and increment the corrisponding stack
		switch ( _type )
		{
		case StatusType.CAN_MOVE:
			canMoveStack++;
			break;
		case StatusType.CAN_BE_MOVED:
			canBeMovedStack++;
			break;
		case StatusType.CAN_USE_ABILITY:
			canUseAbilityStack++;
			break;
		}
	}

	/// <summary>
	/// Decrements the current stack of a particular status type.
	/// </summary>
	/// <param name="_type"> The status effect type being decremented. </param>
	private void DecrementEffect ( StatusType _type )
	{
		// Check type and decrement the corrisponding stack
		switch ( _type )
		{
		case StatusType.CAN_MOVE:
			canMoveStack = DecrementStack ( canMoveStack );
			break;
		case StatusType.CAN_BE_MOVED:
			canBeMovedStack = DecrementStack ( canBeMovedStack );
			break;
		case StatusType.CAN_USE_ABILITY:
			canUseAbilityStack = DecrementStack ( canUseAbilityStack );
			break;
		}
	}

	/// <summary>
	/// Decrements a stack and checks for negative overflow.
	/// </summary>
	/// <param name="_stack"> The starting value of the stack. </param>
	/// <returns> The decremented value of the stack. </returns>
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

	#endregion // Private Functions
}
