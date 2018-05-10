using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class UnitDataStorage
{
	//#region Private JSON Classes

	//[Serializable]
	//private class UnitJSONData
	//{
	//	public int ID;
	//	public string UnitName;
	//	public string UnitDescription;
	//	public string FinishingMove;
	//	public string Portrait;
	//	public int Type;
	//	public int Slots;
	//	public AbilityJSONData [ ] Abilities;
	//}

	//[Serializable]
	//private class AbilityJSONData
	//{
	//	public string AbilityName;
	//	public string AbilityDescription;
	//	public string Icon;
	//	public int Type;
	//	public int Cooldown;
	//	public int Duration;
	//}

	//private class UnitJSONHolder
	//{
	//	public UnitJSONData [ ] UnitData;
	//}

	//#endregion // Private JSON Classes

	//#region Unit Data

	//private static ReadOnlyCollection<UnitSettingData> unitDefaults;
	//private static ReadOnlyDictionary<int, UnitSettingData> unitDefaultsDictionary;

	//private const int PAWN_UNIT_ID = 0;
	//private const int LEADER_UNIT_ID = 1;

	//#endregion // Unit Data

	//#region Public Functions

	///// <summary>
	///// Initializes all the unit default data from a JSON file.
	///// </summary>
	///// <param name="json"> The text of the JSON file.</param>
	//public static void InitializeJSONData ( string json )
	//{
	//	// Read JSON data
	//	UnitJSONHolder holder = JsonUtility.FromJson<UnitJSONHolder> ( json );

	//	// Store unit defaults
	//	UnitSettingData [ ] tempUnitDefaults = new UnitSettingData [ holder.UnitData.Length ];
	//	Dictionary<int, UnitSettingData> tempUnitDefaultsDictionary = new Dictionary<int, UnitSettingData> ( );
	//	for ( int i = 0; i < holder.UnitData.Length; i++ )
	//	{
	//		// Set unit data
	//		tempUnitDefaults [ i ] = new UnitSettingData
	//		{
	//			ID = holder.UnitData [ i ].ID,
	//			UnitName = holder.UnitData [ i ].UnitName,
	//			UnitBio = holder.UnitData [ i ].UnitDescription,
	//			FinishingMove = holder.UnitData [ i ].FinishingMove,
	//			Portrait = Resources.Load<Sprite> ( "Units/Portraits/" + holder.UnitData [ i ].Portrait ),
	//			Role = (UnitData.UnitRole)holder.UnitData [ i ].Type,
	//			Slots = holder.UnitData [ i ].Slots,
	//			IsEnabled = true
	//		};

	//		// Set ability data
	//		if ( holder.UnitData [ i ].Abilities [ 0 ].AbilityName == "" )
	//		{
	//			tempUnitDefaults [ i ].InitializeAbilities ( null );
	//		}
	//		else
	//		{
	//			AbilityData [ ] abilityDefaults = new AbilityData [ holder.UnitData [ i ].Abilities.Length ];
	//			for ( int j = 0; j < holder.UnitData [ i ].Abilities.Length; j++ )
	//			{
	//				abilityDefaults [ j ] = new AbilityData
	//				{
	//					AbilityName = holder.UnitData [ i ].Abilities [ j ].AbilityName,
	//					AbilityDescription = holder.UnitData [ i ].Abilities [ j ].AbilityDescription,
	//					Icon = Resources.Load<Sprite> ( "Units/Ability Icons/" + holder.UnitData [ i ].Abilities [ j ].Icon ),
	//					Type = (AbilityData.AbilityType)holder.UnitData [ i ].Type,
	//					IsEnabled = true,
	//					Cooldown = holder.UnitData [ i ].Abilities [ j ].Cooldown,
	//					Duration = holder.UnitData [ i ].Abilities [ j ].Duration
	//				};
	//			}
	//		}

	//		// Add unit defaults to dictionary
	//		tempUnitDefaultsDictionary.Add ( tempUnitDefaults [ i ].ID, tempUnitDefaults [ i ] );
	//	}

	//	// Store data as immutable
	//	unitDefaults = new ReadOnlyCollection<UnitSettingData> ( tempUnitDefaults );
	//	unitDefaultsDictionary = new ReadOnlyDictionary<int, UnitSettingData> ( tempUnitDefaultsDictionary );
	//}

	//public static UnitSettingData [ ] GetUnits ( )
	//{
	//	UnitSettingData [ ] units = new UnitSettingData [ unitDefaults.Count ];
	//	for ( int i = 0; i < units.Length; i++ )
	//		units [ i ] = new UnitSettingData
	//		{
	//			ID = unitDefaults [ i ].ID,
	//			UnitName = unitDefaults [ i ].UnitName,
	//			UnitBio = unitDefaults [ i ].UnitBio,
	//			FinishingMove = unitDefaults [ i ].FinishingMove,
	//			Portrait = unitDefaults [ i ].Portrait,
	//			Role = unitDefaults [ i ].Role,
	//			Slots = unitDefaults [ i ].Slots,
	//			IsEnabled = unitDefaults [ i ].IsEnabled
	//		};

	//	return units;
	//}

	///// <summary>
	///// Get the default data for a unit by its ID.
	///// </summary>
	///// <param name="id"> The ID of the unit whose data is being retrieved </param>
	///// <returns> The default data for the unit. </returns>
	//public static UnitSettingData GetUnitDefault ( int id )
	//{
	//	// Get unit by id
	//	return unitDefaultsDictionary [ id ];
	//}

	///// <summary>
	///// Get the default data for a leader unit by its team color.
	///// </summary>
	///// <param name="team"> The team color of the leader unit. </param>
	///// <returns> The default data for the leader unit. </returns>
	//public static UnitSettingData GetLeaderDefault ( Player.TeamColor team )
	//{
	//	// Get leader data
	//	UnitSettingData leader = new UnitSettingData ( )
	//	{
	//		ID = unitDefaultsDictionary [ (int)team + 2 ].ID,
	//		UnitName = unitDefaultsDictionary [ (int)team + 2 ].UnitName,
	//		UnitBio = unitDefaultsDictionary [ (int)team + 2 ].UnitBio,
	//		FinishingMove = unitDefaultsDictionary [ (int)team + 2 ].FinishingMove,
	//		Portrait = Resources.Load<Sprite> ( "Units/Portraits/" + unitDefaultsDictionary [ (int)team + 2 ].Portrait ),
	//		Role = unitDefaultsDictionary [ (int)team + 2 ].Role,
	//		Slots = unitDefaultsDictionary [ (int)team + 2 ].Slots,
	//		IsEnabled = unitDefaultsDictionary [ (int)team + 2 ].IsEnabled
	//	};

	//	// Get leader ability data
	//	leader.InitializeAbilities ( new AbilityData [ ] { unitDefaultsDictionary [ (int)team + 2 ].Ability1, unitDefaultsDictionary [ (int)team + 2 ].Ability2, unitDefaultsDictionary [ (int)team + 2 ].Ability3 } );

	//	// Return team leader
	//	return leader;
	//}

	///// <summary>
	///// Get the default data for a pawn unit.
	///// </summary>
	///// <returns> The default data for the pawn unit. </returns>
	//public static UnitSettingData GetPawnDefault ( )
	//{
	//	// Get pawn data
	//	UnitSettingData pawn = new UnitSettingData ( )
	//	{
	//		ID = unitDefaultsDictionary [ 0 ].ID,
	//		UnitName = NameGenerator.CreateName(),
	//		UnitBio = unitDefaultsDictionary [ 0 ].UnitBio,
	//		FinishingMove = unitDefaultsDictionary [ 0 ].FinishingMove,
	//		Portrait = Resources.Load<Sprite> ( "Units/Portraits/" + unitDefaultsDictionary [ 0 ].Portrait ),
	//		Role = unitDefaultsDictionary [ 0 ].Role,
	//		Slots = unitDefaultsDictionary [ 0 ].Slots,
	//		IsEnabled = unitDefaultsDictionary [ 0 ].IsEnabled
	//	};

	//	// Get pawn ability data
	//	pawn.InitializeAbilities ( null );

	//	// Return new pawn
	//	return pawn;
	//}

	///// <summary>
	///// Get the list of default data for the list of hero units.
	///// </summary>
	///// <returns> The list of default data for the hero units. </returns>
	//public static List<UnitSettingData> GetHeroesDefault ( )
	//{
	//	// Create list for storing heroes
	//	List<UnitSettingData> heroes = new List<UnitSettingData> ( );

	//	// Check each unit for heroes
	//	foreach ( UnitSettingData unit in unitDefaults )
	//	{
	//		// Check if the unit is a hero
	//		if ( unit.Role == UnitData.UnitRole.OFFENSE || unit.Role == UnitData.UnitRole.DEFENSE || unit.Role == UnitData.UnitRole.SUPPORT )
	//		{
	//			// Create a new instance of hero data
	//			UnitSettingData hero = new UnitSettingData ( )
	//			{
	//				ID = unit.ID,
	//				UnitName = unit.UnitName,
	//				UnitBio = unit.UnitBio,
	//				FinishingMove = unit.FinishingMove,
	//				Portrait = unit.Portrait,
	//				Role = unit.Role,
	//				Slots = unit.Slots,
	//				IsEnabled = unit.IsEnabled
	//			};

	//			// Add the hero to the list
	//			heroes.Add ( hero );
	//		}
	//	}

	//	// Return the list of hero data
	//	return heroes;
	//}

	//#endregion // Public Functions
}
