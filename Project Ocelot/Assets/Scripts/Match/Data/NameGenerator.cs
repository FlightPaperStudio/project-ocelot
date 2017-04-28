using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameGenerator
{
	// Data structure for storing all of the name elements
	private class Names
	{
		public string [ ] firstNames;
		public string [ ] nicknames;
		public string [ ] lastNames;
	}

	// A list of random name elements
	private static Names names;

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
	/// The name consists of a first name, a title, and a last name (e.g. Ethan "Bthan" Caraway).
	/// </summary>
	public static string CreateName ( )
	{
		// Generate first name
		string first = names.firstNames [ Random.Range ( 0, names.firstNames.Length ) ];

		// Generate title
		string nick = names.nicknames [ Random.Range ( 0, names.nicknames.Length ) ];

		// Generate last name
		string last = names.lastNames [ Random.Range ( 0, names.lastNames.Length ) ];

		// Return full name with title
		return first + " \"" + nick + "\" " + last;
	}
}