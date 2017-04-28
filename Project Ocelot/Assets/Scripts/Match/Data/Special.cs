using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Special 
{
	// Special information
	public int id;
	public string name;
	public string description;
	public int type;
	public int cooldown;

	public enum SpecialType
	{
		Hero,
		Command
	}
}

public class SpecialJSONHolder
{
	public Special [ ] list;
}

public class SpecialInfo
{
	// Special ability inforomation
	public static Special [ ] list;
	private static SpecialJSONHolder holder;
	private static Dictionary<int, Special> dic = new Dictionary<int, Special> ( );

	/// <summary>
	/// Sets the list storing all of the special ability information.
	/// </summary>
	public static void SetList ( string json )
	{
		// Store data from JSON file
		holder = JsonUtility.FromJson<SpecialJSONHolder> ( json );
		list = holder.list;

		// Set data in dictionary
		for ( int i = 0; i < list.Length; i++ )
			dic.Add ( list [ i ].id, list [ i ] );
	}

	/// <summary>
	/// Returns the special ability information for the given special ability ID.
	/// </summary>
	public static Special GetSpecialByID ( int id )
	{
		// Return special ability information
		return dic [ id ];
	}
}