using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class StatusEffectDatabase
{
	#region Status Effect Data

	public enum StatusEffectType
	{
		NONE = 0,
		CHAINED = 1,
		BLIND = 2,
		PLASMA_SHIELD = 3,
		NEGATIVE_MASS = 4,
		RADIATION_POISONING = 5,
		CRAFTING = 6,
		EXHAUSTION = 7,
		GRAPPLE_HOLD = 8,
		GRAPPLED = 9,
		MIND_CONTROLLED = 10,
		OVERPOWERED = 11,
		RALLIED = 12,
		INCORPOREAL = 13,
		HASTE = 14,
		IN_UNISON = 15,
		STUNNED = 16
	}

	#endregion // Status Effect Data

	#region Public Functions

	/// <summary>
	/// Gets a status effect from the database.
	/// </summary>
	/// <param name="status"> The type of status effect being retrieved. </param>
	/// <returns> The data for the status effect. </returns>
	public static StatusEffects.Effect GetStatusEffect ( StatusEffectType status )
	{
		// Store status data
		StatusEffects.Effect effect = null;
		IDataReader data;
		
		// Query database for status effect by its id
		data = DatabaseManager.Query (
			"SELECT * " +
			"FROM StatusEffectData " +
			"WHERE StatusID = " + (int)status
			);

		// Extract the status effect from the data
		while ( data.Read ( ) )
			effect = new StatusEffects.Effect
			{
				ID = System.Convert.ToInt32 ( data [ "StatusID" ] ),
				StatusName = data [ "StatusName" ].ToString ( ),
				Icon = Resources.Load<Sprite> ( "Units/Status Effects/" + data [ "IconFileName" ].ToString ( ) ),
				CanMove = System.Convert.ToInt32 ( data [ "CanMove" ] ) == 1,
				CanBeMoved = System.Convert.ToInt32 ( data [ "CanBeMoved" ] ) == 1,
				CanAssist = System.Convert.ToInt32 ( data [ "CanAssist" ] ) == 1,
				CanAttack = System.Convert.ToInt32 ( data [ "CanAttack" ] ) == 1,
				CanBeAttacked = System.Convert.ToInt32 ( data [ "CanBeAttacked" ] ) == 1,
				CanUseAbility = System.Convert.ToInt32 ( data [ "CanUseAbility" ] ) == 1,
				CanBeAffectedByAbility = System.Convert.ToInt32 ( data [ "CanBeAffectedByAbility" ] ) == 1,
				CanBeAffectedPhysically = System.Convert.ToInt32 ( data [ "CanBeAffectedPhysically" ] ) == 1
			};

		// End query
		DatabaseManager.Close ( data );

		// Return status
		return effect;
	}

	#endregion // Public Functions
}
