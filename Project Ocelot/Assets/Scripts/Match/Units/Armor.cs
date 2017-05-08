using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Armor
	/// Type: Passive Ability
	/// Default Duration: 1 Attack
	/// 
	/// Ability 2: ???
	/// Type: ???
	/// 
	/// </summary>

	/// <summary>
	/// Removes this unit's armor or captures this unit if armor is no longer available.
	/// Call this function on the unit being captured.
	/// </summary>
	public override void GetCaptured ( bool lostMatch = false )
	{
		// Check armor duration
		if ( currentAbility1.enabled && currentAbility1.duration > 0 && !lostMatch )
		{
			// Decrement armor duration
			currentAbility1.duration--;

			// Set tile as blocked so that the armor ability can't be negated in one turn
			GM.selectedUnit.AddBlockedTile ( GM.selectedUnit.currentTile, true );

			// Check if armor is destroyed
			//if ( currentAbility1.duration == 0 )
		}
		else
		{
			// Display that the special is deactivated
			GM.UI.hudDic [ team ].DisplayDeactivation ( instanceID );

			// Capture this unit
			base.GetCaptured ( );
		}
	}
}
