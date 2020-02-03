/* Character Name Generator v.1.0.2
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
		#region Public Structures and Enums

		public enum EntryDataset
		{
			NONE,
			GIVEN_NAMES,
			FAMILY_NAMES,
			NICKNAMES,
			NAME_PREFIXES,
			NAME_SUFFIXES
		}

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

		[System.Serializable]
		private struct FilteredEntries
		{
			public Dictionary<int, List<NameEntry>> OriginEntries;
			public NameEntry [ ] AllEntries;
		}

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
			public int ID;
			public string Name;
			public string Category;
			public string Subcategory;
			public float Weight;
		}

		#endregion // Private Classes

		#region Name Data

		private static bool isLoaded = false;
		private static FilteredEntries givenNames;
		private static FilteredEntries familyNames;
		private static FilteredEntries nicknames;
		private static FilteredEntries namePrefixes;
		private static FilteredEntries nameSuffixes;
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
				return givenNames.AllEntries;
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
				return familyNames.AllEntries;
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
				return nicknames.AllEntries;
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
				return namePrefixes.AllEntries;
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
				return nameSuffixes.AllEntries;
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
			// Track performance
			System.Diagnostics.Stopwatch performanceTracker = System.Diagnostics.Stopwatch.StartNew ( );

			// Load origins
			string originsJson = JsonManager.GetJsonFromResources ( ORIGIN_JSON_FILE );
			OriginWrapper [ ] originWrapper = JsonManager.FromJson<OriginWrapper> ( originsJson );
			origins = ConvertWrapper ( originWrapper );

			// Load given names
			string givenNamesJson = JsonManager.GetJsonFromResources ( GIVEN_NAME_JSON_FILE );
			GenderedNonTypedEntryWrapper [ ] givenNamesWrapper = JsonManager.FromJson<GenderedNonTypedEntryWrapper> ( givenNamesJson );
			givenNames = Filter ( ConvertWrapper ( givenNamesWrapper ) );

			//// Load family names
			string familyNamesJson = JsonManager.GetJsonFromResources ( FAMILY_NAME_JSON_FILE );
			NonGenderedNonTypedEntryWrapper [ ] familyNamesWrapper = JsonManager.FromJson<NonGenderedNonTypedEntryWrapper> ( familyNamesJson );
			familyNames = Filter ( ConvertWrapper ( familyNamesWrapper ) );

			//// Load nicknames
			string nicknamesJson = JsonManager.GetJsonFromResources ( NICKNAME_JSON_FILE );
			GenderedTypedEntryWrapper [ ] nicknamesWrapper = JsonManager.FromJson<GenderedTypedEntryWrapper> ( nicknamesJson );
			nicknames = Filter ( ConvertWrapper ( nicknamesWrapper ) );

			//// Load name prefixes
			string prefixesJson = JsonManager.GetJsonFromResources ( NAME_PREFIX_JSON_FILE );
			GenderedNonTypedEntryWrapper [ ] prefixesWrapper = JsonManager.FromJson<GenderedNonTypedEntryWrapper> ( prefixesJson );
			namePrefixes = Filter ( ConvertWrapper ( prefixesWrapper ) );

			//// Load name suffixes
			string suffixesJson = JsonManager.GetJsonFromResources ( NAME_SUFFIX_JSON_FILE );
			NonGenderedNonTypedEntryWrapper [ ] suffixesWrapper = JsonManager.FromJson<NonGenderedNonTypedEntryWrapper> ( suffixesJson );
			nameSuffixes = Filter ( ConvertWrapper ( suffixesWrapper ) );

			// Store that the data is loaded
			isLoaded = true;

			// Output performance
			performanceTracker.Stop ( );
			if ( Settings.OutputPerformance )
				Debug.Log ( "Name Data loaded in " + performanceTracker.ElapsedMilliseconds + " ms" );
			
		}

		/// <summary>
		/// Gets the filtered name entries from a specific dataset.
		/// </summary>
		/// <param name="dataset"> The dataset to filter from. </param>
		/// <returns> The filtered dataset of name entries. </returns>
		public static NameEntry [ ] GetFilteredEntries ( EntryDataset dataset )
		{
			// Check dataset
			switch ( dataset )
			{
			// Return the filtered given names
			case EntryDataset.GIVEN_NAMES:
				return givenNames.AllEntries;

			// Return the filtered family names
			case EntryDataset.FAMILY_NAMES:
				return familyNames.AllEntries;

			// Return the filtered nicknames
			case EntryDataset.NICKNAMES:
				return nicknames.AllEntries;

			// Return the filtered name prefixes
			case EntryDataset.NAME_PREFIXES:
				return namePrefixes.AllEntries;

			// Return the filtered name suffixes
			case EntryDataset.NAME_SUFFIXES:
				return nameSuffixes.AllEntries;
			}

			// Return null as error
			return null;
		}

		/// <summary>
		/// Gets the filtered name entries from a specific dataset by origin.
		/// </summary>
		/// <param name="dataset"> The dataset to filter from. </param>
		/// <param name="origin"> The origin to filter by. </param>
		/// <returns> The filtered subset of name entries. </returns>
		public static NameEntry [ ] GetFilteredEntries ( EntryDataset dataset, Origin origin )
		{
			// Check dataset
			switch ( dataset )
			{
			// Return the filtered given names
			case EntryDataset.GIVEN_NAMES:
				if ( origin.ID == 0 || !givenNames.OriginEntries.ContainsKey ( origin.ID ) )
					return givenNames.AllEntries;
				else
					return givenNames.OriginEntries [ origin.ID ].ToArray ( );

			// Return the filtered family names
			case EntryDataset.FAMILY_NAMES:
				if ( origin.ID == 0 || !familyNames.OriginEntries.ContainsKey ( origin.ID ) )
					return familyNames.AllEntries;
				else
					return familyNames.OriginEntries [ origin.ID ].ToArray ( );

			// Return the filtered nicknames
			case EntryDataset.NICKNAMES:
				if ( origin.ID == 0 || !nicknames.OriginEntries.ContainsKey ( origin.ID ) )
					return nicknames.AllEntries;
				else
					return nicknames.OriginEntries [ origin.ID ].ToArray ( );

			// Return the filtered name prefixes
			case EntryDataset.NAME_PREFIXES:
				if ( origin.ID == 0 || !namePrefixes.OriginEntries.ContainsKey ( origin.ID ) )
					return namePrefixes.AllEntries;
				else
					return namePrefixes.OriginEntries [ origin.ID ].ToArray ( );

			// Return the filtered name suffixes
			case EntryDataset.NAME_SUFFIXES:
				if ( origin.ID == 0 || !nameSuffixes.OriginEntries.ContainsKey ( origin.ID ) )
					return nameSuffixes.AllEntries;
				else
					return nameSuffixes.OriginEntries [ origin.ID ].ToArray ( );
			}

			// Return null as error
			return null;
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Filters a set of name entries into subsets of dictionaries based on origin, subcategory, and category.
		/// </summary>
		/// <param name="entries"> The set of name entries. </param>
		/// <returns> The filtered set of name entries. </returns>
		private static FilteredEntries Filter ( NameEntry [ ] entries )
		{
			// Declare data structure
			FilteredEntries data = new FilteredEntries
			{
				OriginEntries = new Dictionary<int, List<NameEntry>> ( ),
				//SubcategoryEntries = new Dictionary<Origin.SubcategoryType, List<NameEntry>> ( ),
				//CategoryEntries = new Dictionary<Origin.CategoryType, List<NameEntry>> ( ),
				AllEntries = entries
			};

			// Add each origin
			for ( int i = 0; i < origins.Length; i++ )
				data.OriginEntries.Add ( origins [ i ].ID, new List<NameEntry> ( ) );

			// Filter each entry
			for ( int i = 0; i < entries.Length; i++ )
			{
				// Filter each origin
				for ( int j = 0; j < entries [ i ].Origins.Length; j++ )
				{
					// Add value to origin key
					if ( data.OriginEntries.ContainsKey ( entries [ i ].Origins [ j ].ID ) )
						data.OriginEntries [ entries [ i ].Origins [ j ].ID ].Add ( entries [ i ] );
				}
			}

			// Return filtered entries
			return data;
		}

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
					ID = 0,
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
				ID = 0,
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
					ID = wrapper [ i ].ID,
					Name = wrapper [ i ].Name,
					Subcategory = Origin.SubcategoryFromString ( wrapper [ i ].Subcategory ),
					Category = Origin.CategoryFromString ( wrapper [ i ].Category ),
					Weight = wrapper [ i ].Weight
				};

			// Return the converted data
			return data;
		}

		#endregion // Private Functions
	}
}
