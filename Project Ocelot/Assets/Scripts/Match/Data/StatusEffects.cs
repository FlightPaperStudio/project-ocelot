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
			Effect statusEffect = Effects.Find ( x => x.ID == (int)effect && x.Caster == caster ); 
			
			// Remove effect
			RemoveStatusEffect ( statusEffect );

			// Remove effect from the list
			Effects.Remove ( statusEffect );
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
	/// Removes all existing status effects for this unit.
	/// </summary>
	public void ClearStatusEffects ( )
	{
		// Remove existing effects
		Effects.Clear ( );

		// Reset effects
		canMoveStack = 0;
		canBeMovedStack = 0;
		canAssistStack = 0;
		canAttackStack = 0;
		canBeAttackedStack = 0;
		canUseAbilityStack = 0;
		canBeAffectedByAbilityStack = 0;
		canBeAffectedPhysicallyStack = 0;
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

	#endregion // Private Functions
}
