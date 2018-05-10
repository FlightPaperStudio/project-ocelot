using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public static class UnitDatabase
{
	#region Public Functions

	/// <summary>
	/// Get a unit's data from the database by the unit's ID.
	/// </summary>
	/// <param name="id"> The unit's ID. </param>
	/// <returns> The unit data. </returns>
	public static UnitSettingData GetUnit ( int id )
	{
		// Store unit data
		UnitSettingData unit = null;
		IDataReader data;

		// Query database for the unit by its id
		data = DatabaseManager.Query (
			"SELECT * " +
			"FROM UnitData " +
			"WHERE UnitID = " + id
			);

		// Extract unit from the data
		while ( data.Read ( ) )
			unit = new UnitSettingData
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

		// End query
		DatabaseManager.Close ( data );

		// Query database for the unit's abilities by its id
		data = DatabaseManager.Query (
			"SELECT * " +
			"FROM AbilityData " +
			"WHERE UnitID = " + id + " " +
			"ORDER BY AbilityPosition ASC"
			);

		// Extract abilities from the data
		List<AbilityData> abilities = new List<AbilityData> ( );
		while ( data.Read ( ) )
			abilities.Add ( new AbilityData
			{
				ID = System.Convert.ToInt32 ( data [ "AbilityID" ] ),
				AbilityName = data [ "AbilityName" ].ToString ( ),
				AbilityDescription = data [ "AbilityDescription" ].ToString ( ),
				Icon = Resources.Load<Sprite> ( "Units/Ability Icons/" + data [ "IconFileName" ].ToString ( ) ),
				Type = (AbilityData.AbilityType)( System.Convert.ToInt32 ( data [ "AbilityType" ] ) ),
				IsEnabled = true,
				Duration = System.Convert.ToInt32 ( data [ "Duration" ] ),
				Cooldown = System.Convert.ToInt32 ( data [ "Cooldown" ] ),
				CustomFeatureName = data [ "CustomFeatureName" ].ToString ( ),
				CustomFeatureValue = data [ "CustomFeatureValue" ].ToString ( ) != "" ? System.Convert.ToInt32 ( data [ "CustomFeatureValue" ] ) : -1
			} );
		unit.InitializeAbilities ( abilities.ToArray ( ) );

		// End query
		DatabaseManager.Close ( data );

		// Return unit
		return unit;
	}

	/// <summary>
	/// Get all units' data from the database.
	/// </summary>
	/// <returns> All unit data. </returns>
	public static UnitSettingData [ ] GetUnits ( )
	{
		// Store unit data
		List<UnitSettingData> units = new List<UnitSettingData> ( );
		IDataReader data;

		// Query database for the unit by its id
		data = DatabaseManager.Query (
			"SELECT * " +
			"FROM UnitData"
			);

		// Extract unit from the data
		while ( data.Read ( ) )
			units.Add ( new UnitSettingData
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
			} );

		// End query
		DatabaseManager.Close ( data );

		// Get the abilities for each unit
		for ( int i = 0; i < units.Count; i++ )
		{
			// Query database for the unit's abilities by its id
			data = DatabaseManager.Query (
				"SELECT * " +
				"FROM AbilityData " +
				"WHERE UnitID = " + units [ i ].ID + " " +
				"ORDER BY AbilityPosition ASC"
				);

			// Extract abilities from the data
			List<AbilityData> abilities = new List<AbilityData> ( );
			while ( data.Read ( ) )
				abilities.Add ( new AbilityData
				{
					ID = System.Convert.ToInt32 ( data [ "AbilityID" ] ),
					AbilityName = data [ "AbilityName" ].ToString ( ),
					AbilityDescription = data [ "AbilityDescription" ].ToString ( ),
					Icon = Resources.Load<Sprite> ( "Units/Ability Icons/" + data [ "IconFileName" ].ToString ( ) ),
					Type = (AbilityData.AbilityType)( System.Convert.ToInt32 ( data [ "AbilityType" ] ) ),
					IsEnabled = true,
					Duration = System.Convert.ToInt32 ( data [ "Duration" ] ),
					Cooldown = System.Convert.ToInt32 ( data [ "Cooldown" ] ),
					CustomFeatureName = data [ "CustomFeatureName" ].ToString ( ),
					CustomFeatureValue = data [ "CustomFeatureValue" ].ToString ( ) != "" ? System.Convert.ToInt32 ( data [ "CustomFeatureValue" ] ) : -1
				} );
			units [ i ].InitializeAbilities ( abilities.ToArray ( ) );

			// End query
			DatabaseManager.Close ( data );
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
		List<UnitSettingData> heroes = new List<UnitSettingData> ( );
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
			heroes.Add ( new UnitSettingData
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
			} );

		// End query
		DatabaseManager.Close ( data );

		// Get the abilities for each unit
		for ( int i = 0; i < heroes.Count; i++ )
		{
			// Query database for the unit's abilities by its id
			data = DatabaseManager.Query (
				"SELECT * " +
				"FROM AbilityData " +
				"WHERE UnitID = " + heroes [ i ].ID + " " +
				"ORDER BY AbilityPosition ASC"
				);

			// Extract abilities from the data
			List<AbilityData> abilities = new List<AbilityData> ( );
			while ( data.Read ( ) )
				abilities.Add ( new AbilityData
				{
					ID = System.Convert.ToInt32 ( data [ "AbilityID" ] ),
					AbilityName = data [ "AbilityName" ].ToString ( ),
					AbilityDescription = data [ "AbilityDescription" ].ToString ( ),
					Icon = Resources.Load<Sprite> ( "Units/Ability Icons/" + data [ "IconFileName" ].ToString ( ) ),
					Type = (AbilityData.AbilityType)( System.Convert.ToInt32 ( data [ "AbilityType" ] ) ),
					IsEnabled = true,
					Duration = System.Convert.ToInt32 ( data [ "Duration" ] ),
					Cooldown = System.Convert.ToInt32 ( data [ "Cooldown" ] ),
					CustomFeatureName = data [ "CustomFeatureName" ].ToString ( ),
					CustomFeatureValue = data [ "CustomFeatureValue" ].ToString ( ) != "" ? System.Convert.ToInt32 ( data [ "CustomFeatureValue" ] ) : -1
				} );
			heroes [ i ].InitializeAbilities ( abilities.ToArray ( ) );

			// End query
			DatabaseManager.Close ( data );
		}

		// Return unit
		return heroes;
	}

	#endregion // Public Functions
}
