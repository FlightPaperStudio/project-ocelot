using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameGenerator
{
	// Data structure for storing all of the name elements
	[System.Serializable]
	private struct Names
	{
		public string [ ] FirstNames;
		public string [ ] Nicknames;
		public string [ ] LastNames;
		public string [ ] Suffixes;
	}

	// A list of random name elements
	private static Names names;

	// The percentage chance of a suffix being added to a name
	private const float SUFFIX_PERCENTAGE = 0.05f;

	/// <summary>
	/// Initializes the lists of first names, last names, and titles for the random name generator from a JSON file.
	/// </summary>
	public static void Init ( string json )
	{
		// Set list of random names
		names = JsonUtility.FromJson<Names> ( json );
	}

	/// <summary>
	/// Creates a randomly generated name.
	/// The name consists of a first name, a last name, and sometimes a suffix at a 5% chance (e.g. Ethan Caraway or Sam Ange III).
	/// </summary>
	public static string CreateName ( )
	{
		// Generate first name
		string first = names.FirstNames [ Random.Range ( 0, names.FirstNames.Length ) ];

		// Generate last name
		string last = names.LastNames [ Random.Range ( 0, names.LastNames.Length ) ];

		// Check for suffix
		if ( Random.Range ( 0f, 1f ) <= SUFFIX_PERCENTAGE )
		{
			// Generate suffix
			string suffix = names.Suffixes [ Random.Range ( 0, names.Suffixes.Length ) ];

			// Check if suffix begins with a comma
			if (suffix[0] == ',')
				return first + " " + last + suffix; // Return full name with suffix separated by a comma
			else
				return first + " " + last + " " + suffix; // Return full name with suffix separated by a space
		}

		// Return full name
		return first + " " + last;
	}

	/// <summary>
	/// Creates a randomly generated nickname.
	/// The nickname is returned in quotation (e.g. "Bthan").
	/// </summary>
	/// <returns></returns>
	public static string CreateNickname ( )
	{
		// Generate nickname
		string nickname = names.Nicknames [ Random.Range ( 0, names.Nicknames.Length ) ];

		// Return formated nickname
		return "\"" + nickname + "\""; 
	}
}