using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUnit : Unit 
{
	// Special ability information
	public int specialID;
	public Special specialAbility
	{
		get;
		private set;
	}
	protected int currentCooldown = 0;

	/// <summary>
	/// Initializes this special unit instance.
	/// </summary>
	protected virtual void Start ( )
	{
		// Special ability information
		specialAbility = SpecialInfo.GetSpecialByID ( specialID );
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
	/// Starts the cooldown for the unit's special ability.
	/// </summary>
	protected void StartCooldown ( )
	{
		// Set cooldown
		currentCooldown = MatchSettings.GetSpecialSettingsByID ( specialID ).cooldown;

		// Display cooldown
		GM.UI.hudDic [ team ].DisplayCooldown ( instanceID, currentCooldown );
	}

	/// <summary>
	/// Decrements the cooldown for the unit's special ability.
	/// </summary>
	public virtual void Cooldown ( )
	{
		// Check if current cooldown is active
		if ( currentCooldown > 0 )
		{
			// Decrement cooldown
			currentCooldown--;

			// Display current cooldown
			GM.UI.hudDic [ team ].DisplayCooldown ( instanceID, currentCooldown );
		}
	}
}
