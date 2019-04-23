/* Character Name Generator v.1.0.0
 * --------------------------------------------------------------------------------------------------------------------------------------------------
 * 
 * This file is part of Character Name Generator which is released under the Unity Asset Store End User License Agreement.
 * See file Documentation.pdf or go to https://unity3d.com/legal/as_terms for full license details.
 * 
 * Copyright (c) : 2019
 * Flight Paper Studio LLC
 */

namespace CNG
{
	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// A class for storing the name entry data loaded from JSON files.
	/// </summary>
	public static class NameData
	{
		#region Public Structures

		/// <summary>
		/// A data structure for storing the data of each name.
		/// </summary>
		[System.Serializable]
		public struct NameEntry
		{
			public string Name;
			public bool IsMale;
			public bool IsFemale;
			public string Type;
			public Origin [ ] Origins;
		}

		#endregion // Public Classes

		#region Private Classes

		/// <summary>
		/// A wrapper class for converting JSON elements into a Name Entry.
		/// </summary>
		[System.Serializable]
		private class GenderedNonTypedEntryWrapper
		{
			public string Name;
			public string Male;
			public string Female;
			public string Origins;
		}

		/// <summary>
		/// A wrapper class for converting JSON elements into a Name Entry.
		/// </summary>
		[System.Serializable]
		private class NonGenderedNonTypedEntryWrapper
		{
			public string Name;
			public string Origins;
		}

		/// <summary>
		/// A wrapper class for converting JSON elements into a Name Entry.
		/// </summary>
		[System.Serializable]
		private class GenderedTypedEntryWrapper
		{
			public string Name;
			public string Male;
			public string Female;
			public string Type;
			public string Origins;
		}

		/// <summary>
		/// A wrapper class for converting JSON elements into an Origin.
		/// </summary>
		[System.Serializable]
		public class OriginWrapper
		{
			public string Name;
			public string Category;
			public string Subcategory;
		}

		#endregion // Private Classes

		#region Name Data

		private static bool isLoaded = false;
		private static NameEntry [ ] givenNames;
		private static NameEntry [ ] familyNames;
		private static NameEntry [ ] nicknames;
		private static NameEntry [ ] namePrefixes;
		private static NameEntry [ ] nameSuffixes;
		private static Origin [ ] origins;

		private const string GIVEN_NAME_JSON_FILE = "Character Name Generator - Given Names";
		private const string FAMILY_NAME_JSON_FILE = "Character Name Generator - Family Names";
		private const string NICKNAME_JSON_FILE = "Character Name Generator - Nicknames";
		private const string NAME_PREFIX_JSON_FILE = "Character Name Generator - Prefixes";
		private const string NAME_SUFFIX_JSON_FILE = "Character Name Generator - Suffixes";
		private const string ORIGIN_JSON_FILE = "Character Name Generator - Origins";

		/// <summary>
		/// Whether or not the name data have been loaded.
		/// </summary>
		public static bool IsNameDataLoaded
		{
			get
			{
				return isLoaded;
			}
		}

		/// <summary>
		/// The name entries for given names.
		/// </summary>
		public static NameEntry [ ] GivenNames
		{
			get
			{
				// Check for loaded data
				if ( !isLoaded )
					LoadNameData ( );

				// Return given names
				return givenNames;
			}
		}

		/// <summary>
		/// The name entries for family names.
		/// </summary>
		public static NameEntry [ ] FamilyNames
		{
			get
			{
				// Check for loaded data
				if ( !isLoaded )
					LoadNameData ( );

				// Return family names
				return familyNames;
			}
		}

		/// <summary>
		/// The name entries for nicknames.
		/// </summary>
		public static NameEntry [ ] Nicknames
		{
			get
			{
				// Check for loaded data
				if ( !isLoaded )
					LoadNameData ( );

				// Return nicknames
				return nicknames;
			}
		}

		/// <summary>
		/// The name entries for prefixes.
		/// </summary>
		public static NameEntry [ ] NamePrefixes
		{
			get
			{
				// Check for loaded data
				if ( !isLoaded )
					LoadNameData ( );

				// Return name prefixes
				return namePrefixes;
			}
		}

		/// <summary>
		/// The name entries for suffixes.
		/// </summary>
		public static NameEntry [ ] NameSuffixes
		{
			get
			{
				// Check for loaded data
				if ( !isLoaded )
					LoadNameData ( );

				// Return name suffixes
				return nameSuffixes;
			}
		}

		/// <summary>
		/// The data for origins.
		/// </summary>
		public static Origin [ ] Origins
		{
			get
			{
				// Check for loaded data
				if ( !isLoaded )
					LoadNameData ( );

				// Return origins
				return origins;
			}
		}

		#endregion // Name Data

		#region Public Functions

		/// <summary>
		/// Loads the name data for the generator.
		/// </summary>
		public static void LoadNameData ( )
		{
			// Load origins
			string originsJson = JsonManager.GetJsonFromResources ( ORIGIN_JSON_FILE );
			OriginWrapper [ ] originWrapper = JsonManager.FromJson<OriginWrapper> ( originsJson );
			origins = ConvertWrapper ( originWrapper );

			// Load given names
			string givenNamesJson = JsonManager.GetJsonFromResources ( GIVEN_NAME_JSON_FILE );
			GenderedNonTypedEntryWrapper [ ] givenNamesWrapper = JsonManager.FromJson<GenderedNonTypedEntryWrapper> ( givenNamesJson );
			givenNames = ConvertWrapper ( givenNamesWrapper );

			// Load family names
			string familyNamesJson = JsonManager.GetJsonFromResources ( FAMILY_NAME_JSON_FILE );
			NonGenderedNonTypedEntryWrapper [ ] familyNamesWrapper = JsonManager.FromJson<NonGenderedNonTypedEntryWrapper> ( familyNamesJson );
			familyNames = ConvertWrapper ( familyNamesWrapper );

			// Load nicknames
			string nicknamesJson = JsonManager.GetJsonFromResources ( NICKNAME_JSON_FILE );
			GenderedTypedEntryWrapper [ ] nicknamesWrapper = JsonManager.FromJson<GenderedTypedEntryWrapper> ( nicknamesJson );
			nicknames = ConvertWrapper ( nicknamesWrapper );

			// Load name prefixes
			string prefixesJson = JsonManager.GetJsonFromResources ( NAME_PREFIX_JSON_FILE );
			GenderedNonTypedEntryWrapper [ ] prefixesWrapper = JsonManager.FromJson<GenderedNonTypedEntryWrapper> ( prefixesJson );
			namePrefixes = ConvertWrapper ( prefixesWrapper );

			// Load name suffixes
			string suffixesJson = JsonManager.GetJsonFromResources ( NAME_SUFFIX_JSON_FILE );
			NonGenderedNonTypedEntryWrapper [ ] suffixesWrapper = JsonManager.FromJson<NonGenderedNonTypedEntryWrapper> ( suffixesJson );
			nameSuffixes = ConvertWrapper ( suffixesWrapper );

			// Store that the data is loaded
			isLoaded = true;
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Gets an origin by name.
		/// </summary>
		/// <param name="origin"> The name of the origin. </param>
		/// <returns> The origin with the matching name. </returns>
		private static Origin GetOrigin ( string origin )
		{
			// Check for any
			if ( origin == "" || origin == "Any" || origin == "None" )
				return new Origin
				{
					Name = "Any",
					Subcategory = Origin.SubcategoryType.NONE,
					Category = Origin.CategoryType.NONE
				};

			// Check each origin by name
			for ( int i = 0; i < origins.Length; i++ )
				if ( origins [ i ].Name == origin )
					return origins [ i ];

			// Return that no origin was found
			return new Origin
			{
				Name = "Error",
				Subcategory = Origin.SubcategoryType.NONE,
				Category = Origin.CategoryType.NONE
			};
		}

		/// <summary>
		/// Gets an array of origins by name.
		/// </summary>
		/// <param name="origin"> The array of names of origins. </param>
		/// <returns> The array of origins with the matching names. </returns>
		private static Origin [ ] GetOrigin ( string [ ] origin )
		{
			// Get origins
			Origin [ ] data = new Origin [ origin.Length ];
			for ( int i = 0; i < data.Length; i++ )
				data [ i ] = GetOrigin ( origin [ i ] );

			// Return array
			return data;
		}

		/// <summary>
		/// Converts a set of wrappers into a set of name entries. 
		/// </summary>
		/// <param name="wrapper"> The name entry wrapper to be converted. </param>
		/// <returns> The array of converted name entries. </returns>
		private static NameEntry [ ] ConvertWrapper ( GenderedNonTypedEntryWrapper [ ] wrapper )
		{
			// Store the new entry data
			NameEntry [ ] data = new NameEntry [ wrapper.Length ];

			// Convert each wrapper entry into a finalized name entry
			for ( int i = 0; i < wrapper.Length; i++ )
				data [ i ] = new NameEntry
				{
					Name = wrapper [ i ].Name,
					IsMale = wrapper [ i ].Male == "Y",
					IsFemale = wrapper [ i ].Female == "Y",
					Type = "General",
					Origins = GetOrigin ( wrapper [ i ].Origins.Split ( ',' ) )
				};
				

			// Return the converted data
			return data;
		}

		/// <summary>
		/// Converts a set of wrappers into a set of name entries. 
		/// </summary>
		/// <param name="wrapper"> The name entry wrapper to be converted. </param>
		/// <returns> The array of converted name entries. </returns>
		private static NameEntry [ ] ConvertWrapper ( NonGenderedNonTypedEntryWrapper [ ] wrapper )
		{
			// Store the new entry data
			NameEntry [ ] data = new NameEntry [ wrapper.Length ];

			// Convert each wrapper entry into a finalized name entry
			for ( int i = 0; i < wrapper.Length; i++ )
				data [ i ] = new NameEntry
				{
					Name = wrapper [ i ].Name,
					IsMale = true,
					IsFemale = true,
					Type = "General",
					Origins = GetOrigin ( wrapper [ i ].Origins.Split ( ',' ) )
				};

			// Return the converted data
			return data;
		}

		/// <summary>
		/// Converts a set of wrappers into a set of name entries. 
		/// </summary>
		/// <param name="wrapper"> The name entry wrapper to be converted. </param>
		/// <returns> The array of converted name entries. </returns>
		private static NameEntry [ ] ConvertWrapper ( GenderedTypedEntryWrapper [ ] wrapper )
		{
			// Store the new entry data
			NameEntry [ ] data = new NameEntry [ wrapper.Length ];

			// Convert each wrapper entry into a finalized name entry
			for ( int i = 0; i < wrapper.Length; i++ )
				data [ i ] = new NameEntry
				{
					Name = wrapper [ i ].Name,
					IsMale = wrapper [ i ].Male == "Y",
					IsFemale = wrapper [ i ].Female == "Y",
					Type = wrapper [ i ].Type,
					Origins = GetOrigin ( wrapper [ i ].Origins.Split ( ',' ) )
				};

			// Return the converted data
			return data;
		}

		/// <summary>
		/// Converts a set of wrappers into a set of origins. 
		/// </summary>
		/// <param name="wrapper"> The origin wrapper to be converted. </param>
		/// <returns> The array of converted origins. </returns>
		private static Origin [ ] ConvertWrapper ( OriginWrapper [ ] wrapper )
		{
			// Store the new origin data
			Origin [ ] data = new Origin [ wrapper.Length ];

			// Convert each wrapper into a finalized origin
			for ( int i = 0; i < wrapper.Length; i++ )
				data [ i ] = new Origin
				{
					Name = wrapper [ i ].Name,
					Subcategory = Origin.SubcategoryFromString ( wrapper [ i ].Subcategory ),
					Category = Origin.CategoryFromString ( wrapper [ i ].Category )
				};

			// Return the converted data
			return data;
		}

		#endregion // Private Functions
	}
}
