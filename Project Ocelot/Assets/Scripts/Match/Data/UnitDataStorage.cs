using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class UnitDataStorage
{
	#region Private JSON Classes

	[Serializable]
	private class UnitJSONData
	{
		public int ID;
		public string UnitName;
		public string UnitDescription;
		public string FinishingMove;
		public string Portrait;
		public int Type;
		public int Slots;
		public AbilityJSONData [ ] Abilities;
	}

	[Serializable]
	private class AbilityJSONData
	{
		public string AbilityName;
		public string AbilityDescription;
		public string Icon;
		public int Type;
		public int Cooldown;
		public int Duration;
	}

	[Serializable]
	private class ExtraUnitJSONData
	{
		public string UnitName;
		public string UnitDescription;
		public string FinishingMove;
		public string Portrait;
	}

	private class UnitJSONHolder
	{
		public ExtraUnitJSONData [ ] LeaderData;
		public ExtraUnitJSONData [ ] ExtraUnitData;
		public UnitJSONData [ ] UnitData;
	}

	#endregion // Private JSON Classes

	#region Unit Data

	private static ReadOnlyCollection<ExtraUnitJSONData> leaderData;
	private static ReadOnlyDictionary<Player.TeamColor, ExtraUnitJSONData> leaderDictionary;
	private static ReadOnlyCollection<ExtraUnitJSONData> extraUnitData;
	private static ReadOnlyCollection<UnitDefaultData> unitDefaults;
	private static ReadOnlyDictionary<int, UnitDefaultData> unitDefaultsDictionary;

	#endregion // Unit Data

	#region Public Functions

	/// <summary>
	/// Initializes all the unit default data from a JSON file.
	/// </summary>
	/// <param name="json"> The text of the JSON file.</param>
	public static void InitializeJSONData ( string json )
	{
		// Read JSON data
		UnitJSONHolder holder = JsonUtility.FromJson<UnitJSONHolder> ( json );

		// Store extra leader data
		leaderData = new ReadOnlyCollection<ExtraUnitJSONData> (holder.LeaderData);
		Dictionary<Player.TeamColor, ExtraUnitJSONData> tempLeaderDictionary = new Dictionary<Player.TeamColor, ExtraUnitJSONData> ( );
		for ( int i = 0; i < leaderData.Count; i++ )
			tempLeaderDictionary.Add ( (Player.TeamColor)i, leaderData [ i ] );
		leaderDictionary = new ReadOnlyDictionary<Player.TeamColor, ExtraUnitJSONData> ( tempLeaderDictionary );

		// Store extra unit data
		extraUnitData = new ReadOnlyCollection<ExtraUnitJSONData> (holder.ExtraUnitData);

		// Store unit defaults
		UnitDefaultData [ ] tempUnitDefaults = new UnitDefaultData [ holder.UnitData.Length ];
		Dictionary<int, UnitDefaultData> tempUnitDefaultsDictionary = new Dictionary<int, UnitDefaultData> ( );
		for ( int i = 0; i < holder.UnitData.Length; i++ )
		{
			// Set unit data
			tempUnitDefaults [ i ] = new UnitDefaultData
			{
				ID = holder.UnitData [ i ].ID,
				UnitName = holder.UnitData [ i ].UnitName,
				UnitDescription = holder.UnitData [ i ].UnitDescription,
				FinishingMove = holder.UnitData [ i ].FinishingMove,
				Portrait = Resources.Load<Sprite> ( "Units/Portraits/" + holder.UnitData [ i ].Portrait ),
				Type = (UnitData.UnitType)holder.UnitData [ i ].Type,
				Slots = holder.UnitData [ i ].Slots,
				IsEnabled = true
			};

			// Set ability data
			if ( holder.UnitData [ i ].Abilities [ 0 ].AbilityName == "" )
			{
				tempUnitDefaults [ i ].InitializeAbilities ( null );
			}
			else
			{
				AbilityData [ ] abilityDefaults = new AbilityData [ holder.UnitData [ i ].Abilities.Length ];
				for ( int j = 0; j < holder.UnitData [ i ].Abilities.Length; j++ )
				{
					abilityDefaults [ j ] = new AbilityData
					{
						AbilityName = holder.UnitData [ i ].Abilities [ j ].AbilityName,
						AbilityDescription = holder.UnitData [ i ].Abilities [ j ].AbilityDescription,
						Icon = Resources.Load<Sprite> ( "Units/Ability Icons/" + holder.UnitData [ i ].Abilities [ j ].Icon ),
						Type = (AbilityData.AbilityType)holder.UnitData [ i ].Type,
						IsEnabled = true,
						Cooldown = holder.UnitData [ i ].Abilities [ j ].Cooldown,
						Duration = holder.UnitData [ i ].Abilities [ j ].Duration
					};
				}
			}

			// Add unit defaults to dictionary
			tempUnitDefaultsDictionary.Add ( tempUnitDefaults [ i ].ID, tempUnitDefaults [ i ] );
		}

		// Store data as immutable
		unitDefaults = new ReadOnlyCollection<UnitDefaultData> ( tempUnitDefaults );
		unitDefaultsDictionary = new ReadOnlyDictionary<int, UnitDefaultData> ( tempUnitDefaultsDictionary );
	}

	/// <summary>
	/// Get the default data for a unit by its ID.
	/// </summary>
	/// <param name="id"> The ID of the unit whose data is being retrieved </param>
	/// <returns> The default data for the unit. </returns>
	public static UnitDefaultData GetUnitDefault ( int id )
	{
		// Get unit by id
		return unitDefaultsDictionary [ id ];
	}

	/// <summary>
	/// Get the default data for a leader unit by its team color.
	/// </summary>
	/// <param name="team"> The team color of the leader unit. </param>
	/// <returns> The default data for the leader unit. </returns>
	public static UnitDefaultData GetLeaderDefault ( Player.TeamColor team )
	{
		// Get leader data
		UnitDefaultData leader = new UnitDefaultData ( )
		{
			ID = unitDefaultsDictionary [ 1 ].ID,
			UnitName = leaderDictionary [ team ].UnitName,
			UnitDescription = leaderDictionary [ team ].UnitDescription,
			FinishingMove = leaderDictionary [ team ].FinishingMove,
			Portrait = Resources.Load<Sprite> ( "Units/Portraits/" + leaderDictionary [ team ].Portrait ),
			Type = unitDefaultsDictionary [ 1 ].Type,
			Slots = unitDefaultsDictionary [ 1 ].Slots,
			IsEnabled = unitDefaultsDictionary [ 1 ].IsEnabled
		};

		// Get leader ability data
		leader.InitializeAbilities ( new AbilityData [ ] { unitDefaultsDictionary [ 1 ].Ability1, unitDefaultsDictionary [ 1 ].Ability2, unitDefaultsDictionary [ 1 ].Ability3 } );

		// Return team leader
		return leader;
	}

	/// <summary>
	/// Get the default data for a pawn unit.
	/// </summary>
	/// <param name="team"> The team color of the pawn unit. </param>
	/// <returns> The default data for the pawn unit. </returns>
	public static UnitDefaultData GetPawnDefault ( Player.TeamColor team )
	{
		// Get pawn data
		UnitDefaultData pawn = new UnitDefaultData ( )
		{
			ID = unitDefaultsDictionary [ 0 ].ID,
			UnitName = NameGenerator.CreateName(),
			UnitDescription = unitDefaultsDictionary [ 0 ].UnitDescription,
			FinishingMove = unitDefaultsDictionary [ 0 ].FinishingMove,
			Portrait = Resources.Load<Sprite> ( "Units/Portraits/" + unitDefaultsDictionary [ 0 ].Portrait ),
			Type = unitDefaultsDictionary [ 0 ].Type,
			Slots = unitDefaultsDictionary [ 0 ].Slots,
			IsEnabled = unitDefaultsDictionary [ 0 ].IsEnabled
		};

		// Get pawn ability data
		pawn.InitializeAbilities ( null );

		// Return new pawn
		return pawn;
	}

	#endregion // Public Functions
}
