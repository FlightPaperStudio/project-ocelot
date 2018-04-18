using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData
{
	public int ID;
	public string UnitName;
	public string UnitDescription;
	public string FinishingMove;
	public Sprite Portrait;
	public UnitType Type;
	public int Slots;
	public bool IsEnabled;

	public enum UnitType
	{
		PAWN = 0,
		LEADER = 1,
		OFFENSE = 2,
		DEFENSE = 3,
		SUPPORT = 4,
		PARTIAL = 5
	}
}

public class UnitDefaultData : UnitData
{
	private AbilityData [ ] abilities;

	/// <summary>
	/// The default settings for this unit's first ability.
	/// Leaders and Heroes have at least one ability.
	/// </summary>
	public AbilityData Ability1
	{
		get
		{
			// Check for first ability and return it
			return abilities.Length < 1 || abilities [ 0 ] == null ? null : abilities [ 0 ];
		}
	}

	/// <summary>
	/// The default settings for this unit's second ability.
	/// Heroes have at least two abilities.
	/// </summary>
	public AbilityData Ability2
	{
		get
		{
			// Check for second ability and return it
			return abilities.Length < 2 || abilities [ 1 ] == null ? null : abilities [ 1 ];
		}
	}

	/// <summary>
	/// The default settings for this unit's third ability.
	/// Heroes with Toggle Commands have at least three abilities.
	/// </summary>
	public AbilityData Ability3
	{
		get
		{
			// Check for third ability and return it
			return abilities.Length < 3 || abilities [ 2 ] == null ? null : abilities [ 2 ];
		}
	}

	/// <summary>
	/// Set the default ability settings.
	/// </summary>
	/// <param name="abilityData"> The abilities for this unit. </param>
	public void InitializeAbilities ( AbilityData [ ] abilityData )
	{
		abilities = abilityData;
	}
}

public class UnitInstanceData : UnitData
{
	private AbilityInstanceData [ ] abilities;

	/// <summary>
	/// The current settings for this unit's first ability.
	/// Leaders and Heroes have at least one ability.
	/// </summary>
	public AbilityInstanceData Ability1
	{
		get
		{
			// Check for first ability and return it
			return abilities.Length < 1 || abilities [ 0 ] == null ? null : abilities [ 0 ];
		}
	}

	/// <summary>
	/// The current settings for this unit's second ability.
	/// Heroes have at least two abilities.
	/// </summary>
	public AbilityInstanceData Ability2
	{
		get
		{
			// Check for second ability and return it
			return abilities.Length < 2 || abilities [ 1 ] == null ? null : abilities [ 1 ];
		}
	}

	/// <summary>
	/// The current settings for this unit's third ability.
	/// Heroes with Toggle Commands have at least three abilities.
	/// </summary>
	public AbilityInstanceData Ability3
	{
		get
		{
			// Check for third ability and return it
			return abilities.Length < 3 || abilities [ 2 ] == null ? null : abilities [ 2 ];
		}
	}

	/// <summary>
	/// Set the current ability settings.
	/// </summary>
	/// <param name="abilityData"> The abilities for this unit. </param>
	public void InitializeAbilities ( AbilityInstanceData [ ] abilityData )
	{
		abilities = abilityData;
	}
}

public class AbilityData
{
	public string AbilityName;
	public string AbilityDescription;
	public Sprite Icon;
	public AbilityType Type;
	public bool IsEnabled;
	public int Cooldown;
	public int Duration;

	public enum AbilityType
	{
		PASSIVE = 0,
		SPECIAL = 1,
		COMMAND = 2,
		TOGGLE_COMMAND = 3
	}
}

public class AbilityInstanceData : AbilityData
{
	public bool IsAvailable;
	public int CurrentCooldown;
	public int CurrentDuration;
}
