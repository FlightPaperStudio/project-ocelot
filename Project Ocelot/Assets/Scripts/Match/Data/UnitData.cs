using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Public Interfaces

public interface IReadOnlyUnitData
{
	/// <summary>
	/// The ID of the unit.
	/// </summary>
	int ID
	{
		get;
	}

	/// <summary>
	/// The name of the unit.
	/// </summary>
	string UnitName
	{
		get;
	}

	/// <summary>
	/// The nickname of the unit.
	/// </summary>
	string UnitNickname
	{
		get;
	}

	/// <summary>
	/// The bio description of the unit.
	/// </summary>
	string UnitBio
	{
		get;
	}

	/// <summary>
	/// The name of the unit's primary attack.
	/// </summary>
	string FinishingMove
	{
		get;
	}

	/// <summary>
	/// The role of the unit.
	/// </summary>
	UnitData.UnitRole Role
	{
		get;
	}

	/// <summary>
	/// The amount of slots the unit occupies in a team.
	/// </summary>
	int Slots
	{
		get;
	}

	/// <summary>
	/// The base sprite for displaying the unit's portrait.
	/// </summary>
	Sprite Portrait
	{
		get;
	}

	/// <summary>
	/// Whether or not the unit is enabled to be used in the match.
	/// </summary>
	bool IsEnabled
	{
		get;
	}
}

public interface IReadOnlyAbilityData
{
	/// <summary>
	/// The ID of the ability.
	/// </summary>
	int ID
	{
		get;
	}

	/// <summary>
	/// The name of the ability.
	/// </summary>
	string AbilityName
	{
		get;
	}

	/// <summary>
	/// The description of the ability.
	/// </summary>
	string AbilityDescription
	{
		get;
	}

	/// <summary>
	/// The sprite for the ability's display icon.
	/// </summary>
	Sprite Icon
	{
		get;
	}

	/// <summary>
	/// The type of the ability.
	/// </summary>
	AbilityData.AbilityType Type
	{
		get;
	}

	/// <summary>
	/// Whether or not the ability is enabled for the match.
	/// </summary>
	bool IsEnabled
	{
		get;
	}

	/// <summary>
	/// The amount of turns before the ability can be used again.
	/// </summary>
	int Cooldown
	{
		get;
	}

	/// <summary>
	/// The amount of turns or uses the ability lasts. 
	/// </summary>
	int Duration
	{
		get;
	}

	/// <summary>
	/// The name of a setting that is unique to the ability.
	/// </summary>
	string PerkName
	{
		get;
	}

	/// <summary>
	/// The value of a setting that is unique to the ability.
	/// For toggle settings, 1 = true and 0 = false.
	/// </summary>
	int PerkValue
	{
		get;
	}
}

#endregion // Public Interfaces

#region Public Base Classes

public class UnitData : IReadOnlyUnitData
{
	/// <summary>
	/// The ID of the unit.
	/// </summary>
	public int ID
	{
		get;
		set;
	}

	/// <summary>
	/// The name of the unit.
	/// </summary>
	public string UnitName
	{
		get;
		set;
	}

	/// <summary>
	/// The nickname of the unit.
	/// </summary>
	public string UnitNickname
	{
		get;
		set;
	}

	/// <summary>
	/// The bio description of the unit.
	/// </summary>
	public string UnitBio
	{
		get;
		set;
	}

	/// <summary>
	/// The name of the unit's primary attack.
	/// </summary>
	public string FinishingMove
	{
		get;
		set;
	}

	/// <summary>
	/// The role of the unit.
	/// </summary>
	public UnitRole Role
	{
		get;
		set;
	}

	/// <summary>
	/// The amount of slots the unit occupies in a team.
	/// </summary>
	public int Slots
	{
		get;
		set;
	}

	/// <summary>
	/// The base sprite for displaying the unit's portrait.
	/// </summary>
	public Sprite Portrait
	{
		get;
		set;
	}

	/// <summary>
	/// Whether or not the unit is enabled to be used in the match.
	/// </summary>
	public bool IsEnabled
	{
		get;
		set;
	}

	public enum UnitRole
	{
		PAWN = 0,
		LEADER = 1,
		OFFENSE = 2,
		DEFENSE = 3,
		SUPPORT = 4,
		PARTIAL = 5
	}
}

public class AbilityData : IReadOnlyAbilityData
{
	/// <summary>
	/// The ID of the ability.
	/// </summary>
	public int ID
	{
		get;
		set;
	}

	/// <summary>
	/// The name of the ability.
	/// </summary>
	public string AbilityName
	{
		get;
		set;
	}

	/// <summary>
	/// The description of the ability.
	/// </summary>
	public string AbilityDescription
	{
		get;
		set;
	}

	/// <summary>
	/// The sprite for the ability's display icon.
	/// </summary>
	public Sprite Icon
	{
		get;
		set;
	}

	/// <summary>
	/// The type of the ability.
	/// </summary>
	public AbilityType Type
	{
		get;
		set;
	}

	/// <summary>
	/// Whether or not the ability is enabled for the match.
	/// </summary>
	public bool IsEnabled
	{
		get;
		set;
	}

	/// <summary>
	/// The amount of turns before the ability can be used again.
	/// </summary>
	public int Cooldown
	{
		get;
		set;
	}

	/// <summary>
	/// The amount of turns or uses the ability lasts. 
	/// </summary>
	public int Duration
	{
		get;
		set;
	}

	/// <summary>
	/// The name of the ability's perk.
	/// </summary>
	public string PerkName
	{
		get;
		set;
	}

	/// <summary>
	/// The value of the ability's perk.
	/// For toggle settings, 1 = true and 0 = false.
	/// </summary>
	public int PerkValue
	{
		get;
		set;
	}

	/// <summary>
	/// Whether or not the perk for the ability is enabled.
	/// </summary>
	public bool IsPerkEnabled
	{
		get
		{
			// Check value
			return PerkValue > 0;
		}
	}

	public enum AbilityType
	{
		PASSIVE = 0,
		SPECIAL = 1,
		COMMAND = 2,
		TOGGLE_COMMAND = 3
	}
}

#endregion // Public Base Classes

#region Public Instance Classes

public class UnitSettingData : UnitData
{
	private AbilityData [ ] abilities = null;

	/// <summary>
	/// The default settings for this unit's first ability.
	/// Leaders and Heroes have at least one ability.
	/// </summary>
	public AbilityData Ability1
	{
		get
		{
			// Check for first ability and return it
			return abilities == null || abilities.Length < 1 || abilities [ 0 ] == null ? null : abilities [ 0 ];
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
			return abilities == null || abilities.Length < 2 || abilities [ 1 ] == null ? null : abilities [ 1 ];
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
			return abilities == null || abilities.Length < 3 || abilities [ 2 ] == null ? null : abilities [ 2 ];
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

public class AbilityInstanceData : AbilityData
{
	public bool IsAvailable;
	public bool IsActive;
	public int CurrentCooldown;
	public int CurrentDuration;
}

#endregion // Public Instance Classes
