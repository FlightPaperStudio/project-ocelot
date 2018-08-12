using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects
{
	#region Public Classes

	public class Effect
	{
		public int ID;
		public string StatusName;
		public Sprite Icon;

		public bool CanMove;
		public bool CanBeMoved;
		public bool CanAssist;
		public bool CanAttack;
		public bool CanBeAttacked;
		public bool CanUseAbility;
		public bool CanBeAffectedByAbility;
		public bool CanBeAffectedPhysically;

		public int Duration;
		public Unit Caster;
	}

	#endregion // Public Classes

	#region Status Effect Data

	public List<Effect> Effects = new List<Effect> ( );

	private int canMoveStack = 0;
	private int canBeMovedStack = 0;
	private int canAssistStack = 0;
	private int canAttackStack = 0;
	private int canBeAttackedStack = 0;
	private int canUseAbilityStack = 0;
	private int canBeAffectedByAbilityStack = 0;
	private int canBeAffectedPhysicallyStack = 0;

	public const int PERMANENT_EFFECT = -10;

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

	/// <summary>
	/// Determines if the unit is able to assist other units in movement.
	/// </summary>
	public bool CanAssist
	{
		get
		{
			return canAssistStack == 0;
		}
	}

	/// <summary>
	/// Determines if the unit is able to attack opponents.
	/// </summary>
	public bool CanAttack
	{
		get
		{
			return canAttackStack == 0;
		}
	}

	/// <summary>
	/// Determines if the unit is able to be attacked by opponents.
	/// </summary>
	public bool CanBeAttacked
	{
		get
		{
			return canBeAttackedStack == 0;
		}
	}

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

	/// <summary>
	/// Determines if the unit is able targeted by abilities.
	/// </summary>
	public bool CanBeAffectedByAbility
	{
		get
		{
			return canBeAffectedByAbilityStack == 0;
		}
	}

	/// <summary>
	/// Determines if the unit is able to be affected by physical abilities.
	/// </summary>
	public bool CanBeAffectedPhysically
	{
		get
		{
			return canBeAffectedPhysicallyStack == 0;
		}
	}


	// Status effect information
	public enum StatusType
	{
		CAN_MOVE,
		CAN_BE_MOVED,
		CAN_USE_ABILITY,
		CAN_ATTACK,
		CAN_BE_ATTACKED
	}	

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

	/// <summary>
	/// Adds a new status effect.
	/// </summary>
	/// <param name="effect"> The type of status effect being added. </param>
	/// <param name="duration"> The duration of the status effect. PERMANENT_EFFECT addes an infinite duration. </param>
	/// <param name="caster"> The unit applying the status effect. </param>
	public void AddStatusEffect ( StatusEffectDatabase.StatusEffectType effect, int duration, Unit caster )
	{
		// Check if the status effect is already present
		if ( Effects.Exists ( x => x.ID == (int)effect && x.Caster == caster ) )
		{
			// Refresh status effect
			Effects.Find ( x => x.ID == (int)effect && x.Caster == caster ).Duration = duration;
		}
		else
		{
			// Get the status effect
			Effect statusEffect = StatusEffectDatabase.GetStatusEffect ( effect );

			// Apply its duration
			statusEffect.Duration = duration;

			// Apply its caster
			statusEffect.Caster = caster;

			// Add effect to list
			AddStatusEffect ( statusEffect );
		}
	}

	/// <summary>
	/// Removes an existing status effect.
	/// </summary>
	/// <param name="effect"> The type of status effect being removed. </param>
	/// <param name="caster"> The unit that originally applied the status effect. </param>
	public void RemoveStatusEffect ( StatusEffectDatabase.StatusEffectType effect, Unit caster )
	{
		// Check if the status effect is already present
		if ( Effects.Exists ( x => x.ID == (int)effect && x.Caster == caster ) )
		{
			// Get the status effect
			Effect statusEffect = StatusEffectDatabase.GetStatusEffect ( effect );

			// Remove effect to the list
			Effects.Remove ( statusEffect );

			// Remove effect
			RemoveStatusEffect ( statusEffect );
		}
	}

	/// <summary>
	/// Updates the Duration of each status effect.
	/// Permanent status effects are not updated.
	/// </summary>
	public void UpdateStatus ( )
	{
		// Update the duration of each status effect
		foreach ( Effect e in Effects )
		{
			// Decrement effect
			if ( e.Duration != PERMANENT_EFFECT )
				e.Duration--;

			// Remove any expired statuses
			if ( e.Duration <= 0 && e.Duration != PERMANENT_EFFECT )
				RemoveStatusEffect ( e );
		}

		// Remove any expired effects
		Effects.RemoveAll ( x => x.Duration <= 0 && x.Duration != PERMANENT_EFFECT );
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
	/// Whether or not this unit currently has a status effect applied.
	/// </summary>
	/// <param name="effect"> The status effect being searched for. </param>
	/// <returns> Whether or not the unit has the status effect applied. </returns>
	public bool HasStatusEffect ( StatusEffectDatabase.StatusEffectType effect )
	{
		// Check for the status effect
		return Effects.Exists ( x => x.ID == (int)effect );
	}

	/// <summary>
	/// Whether or not this unit currently has a status effect applied.
	/// </summary>
	/// <param name="effect"> The status effect being searched for. </param>
	/// <param name="caster"> The unit that applied the status effect. </param>
	/// <returns> Whether or not the unit has the status effect applied. </returns>
	public bool HasStatusEffect ( StatusEffectDatabase.StatusEffectType effect, Unit caster )
	{
		// Check for the status effect
		return Effects.Exists ( x => x.ID == (int)effect && x.Caster == caster );
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
	/// Adds a new status effect.
	/// </summary>
	/// <param name="effect"> The status effect being added. </param>
	private void AddStatusEffect ( Effect effect )
	{
		// Add effect to the list
		Effects.Add ( effect );

		// Update the can move status
		if ( effect.CanMove )
			canMoveStack++;

		// Update the can be moved status
		if ( effect.CanBeMoved )
			canBeMovedStack++;

		// Update the can assist status
		if ( effect.CanAssist )
			canAssistStack++;

		// Update the can attack status
		if ( effect.CanAttack )
			canAttackStack++;

		// Update the can be attacked status
		if ( effect.CanBeAttacked )
			canBeAttackedStack++;

		// Update the can use ability status
		if ( effect.CanUseAbility )
			canUseAbilityStack++;

		// Update the can be affected by ability status
		if ( effect.CanBeAffectedByAbility )
			canBeAffectedByAbilityStack++;

		// Update the can be affected physically status
		if ( effect.CanBeAffectedPhysically )
			canBeAffectedPhysicallyStack++;
	}

	/// <summary>
	/// Removes an existing status effect.
	/// </summary>
	/// <param name="effect"> The status effect being removed. </param>
	private void RemoveStatusEffect ( Effect effect )
	{
		// Update the can move status
		if ( effect.CanMove )
		{
			canMoveStack--;
			if ( canMoveStack < 0 )
				canMoveStack = 0;
		}

		// Update the can be moved status
		if ( effect.CanBeMoved )
		{
			canBeMovedStack--;
			if ( canBeMovedStack < 0 )
				canBeMovedStack = 0;
		}

		// Update the can assist status
		if ( effect.CanAssist )
		{
			canAssistStack--;
			if ( canAssistStack < 0 )
				canAssistStack = 0;
		}

		// Update the can attack status
		if ( effect.CanAttack )
		{
			canAttackStack--;
			if ( canAttackStack < 0 )
				canAttackStack = 0;
		}

		// Update the can be attacked status
		if ( effect.CanBeAttacked )
		{
			canBeAttackedStack--;
			if ( canBeAttackedStack < 0 )
				canBeAttackedStack = 0;
		}

		// Update the can use ability status
		if ( effect.CanUseAbility )
		{
			canUseAbilityStack--;
			if ( canUseAbilityStack < 0 )
				canUseAbilityStack = 0;
		}

		// Update the can be affected by ability status
		if ( effect.CanBeAffectedByAbility )
		{
			canBeAffectedByAbilityStack--;
			if ( canBeAffectedByAbilityStack < 0 )
				canBeAffectedByAbilityStack = 0;
		}

		// Update the can be affected physically status
		if ( effect.CanBeAffectedPhysically )
		{
			canBeAffectedPhysicallyStack--;
			if ( canBeAffectedPhysicallyStack < 0 )
				canBeAffectedPhysicallyStack = 0;
		}
	}



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
