using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[System.Serializable]
public class Hero 
{
	// Hero information
	public int id;
	public string characterName;
	public string characterDescription;

	// Ability 1 information
	public Ability ability1;

	// Ability 2 information
	public Ability ability2;
}

[System.Serializable]
public class Ability
{
	public string name;
	public string description;
	public int type;
	public int duration;
	public int cooldown;

	public enum AbilityType
	{
		Passive,
		Special,
		Command
	}
}

public class HeroJSONHolder
{
	public Hero [ ] list;
}

public class HeroInfo
{
	// Heroes inforomation
	private static Hero [ ] actualList;
	public static ReadOnlyCollection<Hero> list
	{
		get
		{
			return Array.AsReadOnly<Hero> ( actualList );
		}
	}
	private static HeroJSONHolder holder;
	private static Dictionary<int, Hero> dic = new Dictionary<int, Hero> ( );
	private static ReadOnlyDictionary<int, Hero> readOnlyDic = new ReadOnlyDictionary<int, Hero> ( dic );

	/// <summary>
	/// Sets the list storing all of the hero information.
	/// </summary>
	public static void SetList ( string json )
	{
		// Store data from JSON file
		holder = JsonUtility.FromJson<HeroJSONHolder> ( json );
		actualList = holder.list;

		// Set data in dictionary
		for ( int i = 0; i < list.Count; i++ )
			dic.Add ( list [ i ].id, list [ i ] );
	}

	/// <summary>
	/// Returns the hero information for the given ID.
	/// </summary>
	public static Hero GetHeroByID ( int id )
	{
		// Return hero information
		return readOnlyDic [ id ];
	}
}