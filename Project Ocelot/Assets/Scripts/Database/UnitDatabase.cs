using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public static class UnitDatabase
{
	#region Private Classes

	private class UnitConstructor
	{
		public UnitSettingData Unit = null;
		public int Ability1 = 0;
		public int Ability2 = 0;
		public int Ability3 = 0;
	}

	#endregion // Private Classes

	#region Public Functions

	/// <summary>
	/// Get a unit's data from the database by the unit's ID.
	/// </summary>
	/// <param name="id"> The unit's ID. </param>
	/// <returns> The unit data. </returns>
	public static UnitSettingData GetUnit ( int id )
	{
		// Store unit data
		UnitConstructor unit = null;
		IDataReader data;

		// Query database for the unit by its id
		data = DatabaseManager.Query (
			"SELECT * " +
			"FROM UnitData " +
			"WHERE UnitID = " + id
			);

		// Extract unit from the data
		while ( data.Read ( ) )
		{
			unit.Unit = new UnitSettingData
			{
				ID = System.Convert.ToInt32 ( data [ "UnitID" ] ),
				UnitName = data [ "UnitName" ].ToString ( ),
				UnitNickname = data [ "UnitNickname" ].ToString ( ),
				UnitBio = data [ "UnitBio" ].ToString ( ),
				FinishingMove = data [ "FinishingMove" ].ToString ( ),
				Role = (UnitSettingData.UnitRole)( System.Convert.ToInt32 ( data [ "Role" ] ) ),
				Slots = System.Convert.ToInt32 ( data [ "Slots" ] ),
				Portrait = Resources.Load<Sprite> ( "Units/Portraits/" + data [ "PortraitFileName" ].ToString ( ) ),
				IsEnabled = true
			};
			unit.Ability1 = System.Convert.ToInt32 ( data [ "Ability1" ] );
			unit.Ability2 = System.Convert.ToInt32 ( data [ "Ability2" ] );
			unit.Ability3 = System.Convert.ToInt32 ( data [ "Ability3" ] );
		}

		// End query
		DatabaseManager.Close ( data );

		// Extract abilities from the data
		List<AbilityData> abilities = new List<AbilityData> ( );
		if ( unit.Ability1 != 0 )
			abilities.Add ( GetAbilityData ( unit.Ability1 ) );
		if ( unit.Ability2 != 0 )
			abilities.Add ( GetAbilityData ( unit.Ability2 ) );
		if ( unit.Ability3 != 0 )
			abilities.Add ( GetAbilityData ( unit.Ability3 ) );
		unit.Unit.InitializeAbilities ( abilities.ToArray ( ) );

		// Return unit
		return unit.Unit;
	}

	/// <summary>
	/// Get all units' data from the database.
	/// </summary>
	/// <returns> All unit data. </returns>
	public static UnitSettingData [ ] GetUnits ( )
	{
		// Store unit data
		List<UnitConstructor> constructors = new List<UnitConstructor> ( );
		List<UnitSettingData> units = new List<UnitSettingData> ( );
		IDataReader data;

		// Query database for the unit by its id
		data = DatabaseManager.Query (
			"SELECT * " +
			"FROM UnitData"
			);

		// Extract unit from the data
		while ( data.Read ( ) )
		{
			constructors.Add ( new UnitConstructor
			{
				Unit = new UnitSettingData
				{
					ID = System.Convert.ToInt32 ( data [ "UnitID" ] ),
					UnitName = data [ "UnitName" ].ToString ( ),
					UnitNickname = data [ "UnitNickname" ].ToString ( ),
					UnitBio = data [ "UnitBio" ].ToString ( ),
					FinishingMove = data [ "FinishingMove" ].ToString ( ),
					Role = (UnitSettingData.UnitRole)( System.Convert.ToInt32 ( data [ "Role" ] ) ),
					Slots = System.Convert.ToInt32 ( data [ "Slots" ] ),
					Portrait = Resources.Load<Sprite> ( "Units/Portraits/" + data [ "PortraitFileName" ].ToString ( ) ),
					IsEnabled = true
				},
				Ability1 = System.Convert.ToInt32 ( data [ "Ability1" ] ),
				Ability2 = System.Convert.ToInt32 ( data [ "Ability2" ] ),
				Ability3 = System.Convert.ToInt32 ( data [ "Ability3" ] )
		} );
		}

		// End query
		DatabaseManager.Close ( data );

		// Get the abilities for each unit
		for ( int i = 0; i < constructors.Count; i++ )
		{
			// Extract abilities from the data
			List<AbilityData> abilities = new List<AbilityData> ( );
			if ( constructors [ i ].Ability1 != 0 )
				abilities.Add ( GetAbilityData ( constructors [ i ].Ability1 ) );
			if ( constructors [ i ].Ability2 != 0 )
				abilities.Add ( GetAbilityData ( constructors [ i ].Ability2 ) );
			if ( constructors [ i ].Ability3 != 0 )
				abilities.Add ( GetAbilityData ( constructors [ i ].Ability3 ) );
			constructors [ i ].Unit.InitializeAbilities ( abilities.ToArray ( ) );
			units.Add ( constructors [ i ].Unit );
		}

		// Return unit
		return units.ToArray ( );
	}

	/// <summary>
	/// Get all heroes' data from the database.
	/// </summary>
	/// <returns> All hero data. </returns>
	public static List<UnitSettingData> GetHeroes ( )
	{
		// Store unit data
		List<UnitConstructor> constructors = new List<UnitConstructor> ( );
		List<UnitSettingData> units = new List<UnitSettingData> ( );
		IDataReader data;

		// Query database for the unit by its id
		data = DatabaseManager.Query (
			"SELECT * " +
			"FROM UnitData " +
			"WHERE Role = " + (int)UnitData.UnitRole.OFFENSE + " " +
			"OR Role = " + (int)UnitData.UnitRole.DEFENSE + " " +
			"OR Role = " + (int)UnitData.UnitRole.SUPPORT
			);

		// Extract unit from the data
		while ( data.Read ( ) )
		{
			constructors.Add ( new UnitConstructor
			{
				Unit = new UnitSettingData
				{
					ID = System.Convert.ToInt32 ( data [ "UnitID" ] ),
					UnitName = data [ "UnitName" ].ToString ( ),
					UnitNickname = data [ "UnitNickname" ].ToString ( ),
					UnitBio = data [ "UnitBio" ].ToString ( ),
					FinishingMove = data [ "FinishingMove" ].ToString ( ),
					Role = (UnitSettingData.UnitRole)( System.Convert.ToInt32 ( data [ "Role" ] ) ),
					Slots = System.Convert.ToInt32 ( data [ "Slots" ] ),
					Portrait = Resources.Load<Sprite> ( "Units/Portraits/" + data [ "PortraitFileName" ].ToString ( ) ),
					IsEnabled = true
				},
				Ability1 = System.Convert.ToInt32 ( data [ "Ability1" ] ),
				Ability2 = System.Convert.ToInt32 ( data [ "Ability2" ] ),
				Ability3 = System.Convert.ToInt32 ( data [ "Ability3" ] )
			} );
		}

		// End query
		DatabaseManager.Close ( data );

		// Get the abilities for each unit
		for ( int i = 0; i < constructors.Count; i++ )
		{
			// Extract abilities from the data
			List<AbilityData> abilities = new List<AbilityData> ( );
			if ( constructors [ i ].Ability1 != 0 )
				abilities.Add ( GetAbilityData ( constructors [ i ].Ability1 ) );
			if ( constructors [ i ].Ability2 != 0 )
				abilities.Add ( GetAbilityData ( constructors [ i ].Ability2 ) );
			if ( constructors [ i ].Ability3 != 0 )
				abilities.Add ( GetAbilityData ( constructors [ i ].Ability3 ) );
			constructors [ i ].Unit.InitializeAbilities ( abilities.ToArray ( ) );
			units.Add ( constructors [ i ].Unit );
		}

		// Return unit
		return units;
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Gets an ability's data from the database by the abiltiy's ID.
	/// </summary>
	/// <param name="id"> The ID of the ability. </param>
	/// <returns> The ability data. </returns>
	private static AbilityData GetAbilityData ( int id )
	{
		// Query database for the unit's abilities by its id
		IDataReader data = DatabaseManager.Query (
			"SELECT * " +
			"FROM AbilityData " +
			"WHERE AbilityID = " + id
			);

		// Extract abilities from the data
		AbilityData ability = null;
		while ( data.Read ( ) )
			ability = new AbilityData
			{
				ID = System.Convert.ToInt32 ( data [ "AbilityID" ] ),
				AbilityName = data [ "AbilityName" ].ToString ( ),
				AbilityDescription = data [ "AbilityDescription" ].ToString ( ),
				Icon = Resources.Load<Sprite> ( "Units/Ability Icons/" + data [ "IconFileName" ].ToString ( ) ),
				Type = (AbilityData.AbilityType)( System.Convert.ToInt32 ( data [ "AbilityType" ] ) ),
				IsEnabled = true,
				Duration = System.Convert.ToInt32 ( data [ "Duration" ] ),
				Cooldown = System.Convert.ToInt32 ( data [ "Cooldown" ] ),
				PerkName = data [ "PerkName" ].ToString ( ),
				PerkValue = data [ "PerkValue" ].ToString ( ) != "" ? System.Convert.ToInt32 ( data [ "PerkValue" ] ) : -1
			};

		// End query
		DatabaseManager.Close ( data );

		// Return unit
		return ability;
	}

	#endregion // Private Functions
}
