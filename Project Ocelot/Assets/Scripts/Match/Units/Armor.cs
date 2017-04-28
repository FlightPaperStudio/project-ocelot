using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : SpecialUnit
{
	// Armor information
	private bool isArmorActive = true;

	/// <summary>
	/// Removes this unit's armor or captures this unit if armor is no longer available.
	/// Call this function on the unit being captured.
	/// </summary>
	public override void GetCaptured ( bool lostMatch = false )
	{
		// Display that the special is deactivated
		GM.UI.hudDic [ team ].DisplayDeactivation ( instanceID );

		// Check armor is available
		if ( isArmorActive && !lostMatch )
		{
			// Remove armor
			isArmorActive = false;

			// Set tile as blocked so that the armor ability can't be negated in one turn
			GM.selectedUnit.AddBlockedTile ( GM.selectedUnit.currentTile, true );
		}
		else
		{
			// Capture this unit
			base.GetCaptured ( );
		}
	}
}
