/* Character Name Generator v.1.0.2
 * --------------------------------------------------------------------------------------------------------------------------------------------------
 * 
 * This file is part of Character Name Generator which is released under the Unity Asset Store End User License Agreement.
 * See file Documentation.pdf or go to https://unity3d.com/legal/as_terms for full license details.
 * 
 * Copyright (c) : 2019
 * Flight Paper Studio LLC
 */

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CNG
{
	/// <summary>
	/// A class for generating random characters, genders, origins, formats, prefixes, given names, family names, suffixes, and nicknames.
	/// </summary>
	public static class NameGenerator
	{
		#region Public Enums

		/// <summary>
		/// The types of name structures for a character.
		/// </summary>
		public enum Format
		{
			NONE,
			GIVEN_FAMILY,
			GIVEN_GIVEN_FAMILY,
			GIVEN_FAMILY_FAMILY,
			GIVEN_GIVEN_FAMILY_FAMILY
		}

		#endregion // Public Enums

		#region Private Classes

		/// <summary>
		/// A class for filtering and story subsets of name entries for the generator.
		/// </summary>
		private class EntrySubset
		{
			public List<NameData.NameEntry> Entries = new List<NameData.NameEntry> ( );
			public NameData.EntryDataset FilteredDataset = NameData.EntryDataset.NONE;
			public Gender.Label FilterdGender = Gender.Label.NONE;
			public int FilteredOrigin = 0;
		}

		#endregion // Private Classes

		#region Generator Data

		private static string [ ] recentGivenNames = new string [ 100 ];
		private static string [ ] recentFamilyNames = new string [ 100 ];
		private static string [ ] recentNicknames = new string [ 100 ];

		private static int recentGivenNameIndex = 0;
		private static int recentFamilyNameIndex = 0;
		private static int recentNicknameIndex = 0;

		private static List<Origin> origins = new List<Origin> ( );
		private static EntrySubset originSubset = new EntrySubset ( );
		private static EntrySubset subcategorySubset = new EntrySubset ( );
		private static EntrySubset categorySubset = new EntrySubset ( );
		private static EntrySubset anySubset = new EntrySubset ( );

		#endregion // Generator Data

		#region Public Character Generation Functions Functions

		/// <summary>
		/// Generates a random character.
		/// </summary>
		/// <returns> The generated character. </returns>
		public static Character GetCharacter ( )
		{
			// Get character
			return GetCharacter ( GetOrigin ( "Any" ) );
		}

		/// <summary>
		/// Generates a random character based on a specific origin name.
		/// </summary>
		/// <param name="origin"> The origin name the character should be based on. Passing "", "None", or "Any" will generate a character with a random origin. </param>
		/// <param name="gender"> The gender the character should be based on. Passing Gender.Label.NONE will generate a character with a random gender. </param>
		/// <param name="format"> The name format the character should match. Passing Format.NONE will generate a character to match a random format. </param>
		/// <returns> The generated character. </returns>
		public static Character GetCharacter ( string origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get character
			return GetCharacter ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random character based on a specific origin subcategory.
		/// </summary>
		/// <param name="origin"> The origin subcategory the character should be based on. Passing "", "None", or "Any" will generate a character with a random origin. </param>
		/// <param name="gender"> The gender the character should be based on. Passing Gender.Label.NONE will generate a character with a random gender. </param>
		/// <param name="format"> The name format the character should match. Passing Format.NONE will generate a character to match a random format. </param>
		/// <returns> The generated character. </returns>
		public static Character GetCharacter ( Origin.SubcategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get character
			return GetCharacter ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random character based on a specific origin category.
		/// </summary>
		/// <param name="origin"> The origin category the character should be based on. Passing "", "None", or "Any" will generate a character with a random origin. </param>
		/// <param name="gender"> The gender the character should be based on. Passing Gender.Label.NONE will generate a character with a random gender. </param>
		/// <param name="format"> The name format the character should match. Passing Format.NONE will generate a character to match a random format. </param>
		/// <returns> The generated character. </returns>
		public static Character GetCharacter ( Origin.CategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get character
			return GetCharacter ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random character based on a specific origin.
		/// </summary>
		/// <param name="origin"> The origin the character should be based on. Passing an invalid origin will generate a character with a random origin. </param>
		/// <param name="gender"> The gender the character should be based on. Passing Gender.Label.NONE will generate a character with a random gender. </param>
		/// <param name="format"> The name format the character should match. Passing Format.NONE will generate a character to match a random format. </param>
		/// <returns> The generated character. </returns>
		public static Character GetCharacter ( Origin origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Track performance
			System.Diagnostics.Stopwatch performanceTracker = System.Diagnostics.Stopwatch.StartNew ( );

			// Create a new character
			Character character = new Character ( );

			// Assign gender
			character.Gender = gender == Gender.Label.NONE ? GetGender ( ) : gender;
			character.Pronouns = GetPronouns ( character.Gender );

			// Assign origin
			character.Origin = origin;

			// Assign prefix
			float prefixChance = Random.Range ( 0f, 1f );
			character.Prefix = prefixChance <= Settings.NamePrefixChance ? GetPrefix ( character.Origin, character.Gender, format ) : "";

			// Assign given name
			character.GivenName = GetGivenName ( character.Origin, character.Gender, format );

			// Assign family name
			character.FamilyName = GetFamilyName ( character.Origin, character.Gender, format );

			// Assign suffix
			float suffixChance = Random.Range ( 0f, 1f );
			character.Suffix = suffixChance <= Settings.NameSuffixChance ? GetSuffix ( character.Origin, character.Gender, format ) : "";

			// Assign nickname
			character.Nickname = GetNickname ( character.Origin, character.Gender, format );

			// Output performance
			performanceTracker.Stop ( );
			if ( Settings.OutputPerformance )
				Debug.Log ( "Character " + character.WesternNameOrder + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

			// Return the newly created character
			return character;
		}

		#endregion // Public Character Generation Functions

		#region Public Gender Generation Functions

		/// <summary>
		/// Randomly generates a gender based on the gender chance settings.
		/// </summary>
		/// <returns> The randomly generated gender. </returns>
		public static Gender.Label GetGender ( )
		{
			// Get a random percentage
			float random = Random.Range ( 0, Settings.GenderMaleChance + Settings.GenderFemaleChance + Settings.GenderNonBinaryChance );

			// Check outcome
			if ( random < Settings.GenderMaleChance )
				return Gender.Label.MALE;
			else if ( random >= Settings.GenderMaleChance && random < Settings.GenderMaleChance + Settings.GenderFemaleChance )
				return Gender.Label.FEMALE;
			else if ( random >= Settings.GenderMaleChance + Settings.GenderFemaleChance )
				return Gender.Label.NON_BINARY;

			// Return that no gender was generated
			return Gender.Label.NONE;
		}

		/// <summary>
		/// Assigns a set of pronouns for a gender.
		/// </summary>
		/// <param name="gender"> The gender to assign pronouns to. </param>
		/// <returns> The associated set of pronouns. </returns>
		public static Gender.Pronouns GetPronouns ( Gender.Label gender )
		{
			// Check gender
			switch ( gender )
			{
			case Gender.Label.MALE:
				return Gender.Pronouns.HE_HIM_HIS;
			case Gender.Label.FEMALE:
				return Gender.Pronouns.SHE_HER_HERS;
			case Gender.Label.NON_BINARY:
				return Gender.Pronouns.THEY_THEM_THEIR;
			}

			// Return that no pronoun was generated
			return Gender.Pronouns.NONE;
		}

		#endregion // Public Gender Generation Functions

		#region Public Origin Generation Functions

		/// <summary>
		/// Generates an origin from any or an existing origin name.
		/// </summary>
		/// <param name="origin"> The name of the origin. Passing "", "None", or "Any" will generate a random origin. </param>
		/// <returns> The generated origin. </returns>
		public static Origin GetOrigin ( string origin )
		{
			// Check for loaded data
			if ( !NameData.IsNameDataLoaded )
				NameData.LoadNameData ( );

			// Check for defined origin
			if ( origin != "" && origin != "None" && origin != "Any" )
			{
				// Get origin
				for ( int i = 0; i < NameData.Origins.Length; i++ )
					if ( NameData.Origins [ i ].Name == origin )
						return new Origin
						{
							Name = NameData.Origins [ i ].Name,
							Subcategory = NameData.Origins [ i ].Subcategory,
							Category = NameData.Origins [ i ].Category
						};
			}

			// Store random origin index
			int index = 0;

			// Check weighted setting
			if ( Settings.EnableWeightedOrigins )
			{
				// Generate origin by weighted values
				float random = Random.Range ( 0.0f, 1.0f );

				// Find the generated origin from its weight
				for ( int i = 0; i < NameData.Origins.Length; i++ )
				{
					// Check if the random value falls within the weighted range
					random -= NameData.Origins [ i ].Weight;
					if ( random <= 0.0f )
					{
						// Store random index
						index = i;
						break;
					}
				}
			}
			else
			{
				// Generate origin with evenly weighted values
				index = Random.Range ( 0, NameData.Origins.Length );
			}

			// Return origin
			return new Origin
			{
				Name = NameData.Origins [ index ].Name,
				Subcategory = NameData.Origins [ index ].Subcategory,
				Category = NameData.Origins [ index ].Category
			};
		}

		/// <summary>
		/// Generates an origin from any or an existing origin subcategory.
		/// </summary>
		/// <param name="origin"> The origin subcategory. Passing Origin.SubcategoryType.NONE will generate a random origin. </param>
		/// <returns> The generated origin. </returns>
		public static Origin GetOrigin ( Origin.SubcategoryType subcategory )
		{
			// Check for loaded data
			if ( !NameData.IsNameDataLoaded )
				NameData.LoadNameData ( );

			// Check for defined origin
			if ( subcategory != Origin.SubcategoryType.NONE )
			{
				// Clear previous origins
				origins.Clear ( );

				// Get subset of origins
				float weightedTotal = 0f;
				for ( int i = 0; i < NameData.Origins.Length; i++ )
				{
					if ( NameData.Origins [ i ].Subcategory == subcategory )
					{
						origins.Add ( NameData.Origins [ i ] );
						weightedTotal += NameData.Origins [ i ].Weight;
					}
						
				}

				// Store random origin index
				int index = 0;

				// Check weighted setting
				if ( Settings.EnableWeightedOrigins )
				{
					// Generate origin by weighted values
					float random = Random.Range ( 0.0f, weightedTotal );

					// Find the generated origin from its weight
					for ( int i = 0; i < origins.Count; i++ )
					{
						// Check if the random value falls within the weighted range
						random -= origins [ i ].Weight;
						if ( random <= 0.0f )
						{
							// Store random index
							index = i;
							break;
						}
					}
				}
				else
				{
					// Generate origin with evenly weighted values
					index = Random.Range ( 0, origins.Count );
				}

				// Return origin
				return new Origin
				{
					Name = origins [ index ].Name,
					Subcategory = origins [ index ].Subcategory,
					Category = origins [ index ].Category
				};
			}
			else
			{
				// Store random origin index
				int index = 0;

				// Check weighted setting
				if ( Settings.EnableWeightedOrigins )
				{
					// Generate origin by weighted values
					float random = Random.Range ( 0.0f, 1.0f );

					// Find the generated origin from its weight
					for ( int i = 0; i < NameData.Origins.Length; i++ )
					{
						// Check if the random value falls within the weighted range
						random -= NameData.Origins [ i ].Weight;
						if ( random <= 0.0f )
						{
							// Store random index
							index = i;
							break;
						}
					}
				}
				else
				{
					// Generate origin with evenly weighted values
					index = Random.Range ( 0, NameData.Origins.Length );
				}

				// Return origin
				return new Origin
				{
					Name = NameData.Origins [ index ].Name,
					Subcategory = NameData.Origins [ index ].Subcategory,
					Category = NameData.Origins [ index ].Category
				};
			}
		}

		/// <summary>
		/// Generates an origin from any or an existing origin Category.
		/// </summary>
		/// <param name="origin"> The origin category. Passing Origin.CategoryType.NONE will generate a random origin. </param>
		/// <returns> The generated origin. </returns>
		public static Origin GetOrigin ( Origin.CategoryType category )
		{
			// Check for loaded data
			if ( !NameData.IsNameDataLoaded )
				NameData.LoadNameData ( );

			// Check for defined origin
			if ( category != Origin.CategoryType.NONE )
			{
				// Clear previous origins
				origins.Clear ( );

				// Get subset of origins
				float weightedTotal = 0f;
				for ( int i = 0; i < NameData.Origins.Length; i++ )
				{
					if ( NameData.Origins [ i ].Category == category )
					{
						origins.Add ( NameData.Origins [ i ] );
						weightedTotal += NameData.Origins [ i ].Weight;
					}

				}

				// Store random origin index
				int index = 0;

				// Check weighted setting
				if ( Settings.EnableWeightedOrigins )
				{
					// Generate origin by weighted values
					float random = Random.Range ( 0.0f, weightedTotal );

					// Find the generated origin from its weight
					for ( int i = 0; i < origins.Count; i++ )
					{
						// Check if the random value falls within the weighted range
						random -= origins [ i ].Weight;
						if ( random <= 0.0f )
						{
							// Store random index
							index = i;
							break;
						}
					}
				}
				else
				{
					// Generate origin with evenly weighted values
					index = Random.Range ( 0, origins.Count );
				}

				// Return origin
				return new Origin
				{
					Name = origins [ index ].Name,
					Subcategory = origins [ index ].Subcategory,
					Category = origins [ index ].Category
				};
			}
			else
			{
				// Store random origin index
				int index = 0;

				// Check weighted setting
				if ( Settings.EnableWeightedOrigins )
				{
					// Generate origin by weighted values
					float random = Random.Range ( 0.0f, 1.0f );

					// Find the generated origin from its weight
					for ( int i = 0; i < NameData.Origins.Length; i++ )
					{
						// Check if the random value falls within the weighted range
						random -= NameData.Origins [ i ].Weight;
						if ( random <= 0.0f )
						{
							// Store random index
							index = i;
							break;
						}
					}
				}
				else
				{
					// Generate origin with evenly weighted values
					index = Random.Range ( 0, NameData.Origins.Length );
				}

				// Return origin
				return new Origin
				{
					Name = NameData.Origins [ index ].Name,
					Subcategory = NameData.Origins [ index ].Subcategory,
					Category = NameData.Origins [ index ].Category
				};
			}
		}

		#endregion // Public Origin Generation Functions

		#region Public Format Generation Functions

		/// <summary>
		/// Generates a random name format based on the Double Name Chance Settings.
		/// </summary>
		/// <returns> The generated format. </returns>
		public static Format GetFormat ( )
		{
			// Get random chance for double given names
			float givenChance = Random.Range ( 0f, 1.0f );
			bool hasDoubleGiven = givenChance <= Settings.DoubleGivenNameChance;

			// Get random chance for double family names
			float familyChance = Random.Range ( 0f, 1.0f );
			bool hasDoubleFamily = familyChance <= Settings.DoubleFamilyNameChance;

			// Check outcomes
			if ( hasDoubleGiven && hasDoubleFamily )
				return Format.GIVEN_GIVEN_FAMILY_FAMILY;
			else if ( hasDoubleGiven && !hasDoubleFamily )
				return Format.GIVEN_GIVEN_FAMILY;
			else if ( !hasDoubleGiven && hasDoubleFamily )
				return Format.GIVEN_FAMILY_FAMILY;
			else
				return Format.GIVEN_FAMILY;
		}

		#endregion // Public Format Generation Functions

		#region Public Prefix Generation Functions

		/// <summary>
		/// Generates a random name prefix.
		/// </summary>
		/// <returns> The generated prefix. </returns>
		public static string GetPrefix ( )
		{
			// Get prefix
			return GetPrefix ( GetOrigin ( "Any" ) );
		}

		/// <summary>
		/// Generates a random name prefix based on a specific origin name.
		/// </summary>
		/// <param name="origin"> The origin name the prefix should be based on. Passing "", "None", or "Any" will generate a prefix with a random origin. </param>
		/// <param name="gender"> The gender the prefix should be based on. Passing Gender.Label.NONE will generate a prefix with a random gender. </param>
		/// <param name="format"> The name format the prefix should match. Passing Format.NONE will generate a prefix to match a random format. </param>
		/// <returns> The generated prefix. </returns>
		public static string GetPrefix ( string origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get prefix
			return GetPrefix ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random name prefix based on a specific origin subcategory.
		/// </summary>
		/// <param name="origin"> The origin subcategory the prefix should be based on. Passing Origin.SubcategoryType.NONE will generate a prefix with a random origin. </param>
		/// <param name="gender"> The gender the prefix should be based on. Passing Gender.Label.NONE will generate a prefix with a random gender. </param>
		/// <param name="format"> The name format the prefix should match. Passing Format.NONE will generate a prefix to match a random format. </param>
		/// <returns> The generated prefix. </returns>
		public static string GetPrefix ( Origin.SubcategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get prefix
			return GetPrefix ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random name prefix based on a specific origin category.
		/// </summary>
		/// <param name="origin"> The origin category the prefix should be based on. Passing Origin.CategoryType.NONE will generate a prefix with a random origin. </param>
		/// <param name="gender"> The gender the prefix should be based on. Passing Gender.Label.NONE will generate a prefix with a random gender. </param>
		/// <param name="format"> The name format the prefix should match. Passing Format.NONE will generate a prefix to match a random format. </param>
		/// <returns> The generated prefix. </returns>
		public static string GetPrefix ( Origin.CategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get prefix
			return GetPrefix ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random name prefix based on a specific origin.
		/// </summary>
		/// <param name="origin"> The origin the prefix should be based on. </param>
		/// <param name="gender"> The gender the prefix should be based on. Passing Gender.Label.NONE will generate a prefix with a random gender. </param>
		/// <param name="format"> The name format the prefix should match. Passing Format.NONE will generate a prefix to match a random format. </param>
		/// <returns> The generated prefix. </returns>
		public static string GetPrefix ( Origin origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Check for loaded data
			if ( !NameData.IsNameDataLoaded )
				NameData.LoadNameData ( );

			// Track performance
			System.Diagnostics.Stopwatch performanceTracker = System.Diagnostics.Stopwatch.StartNew ( );

			// Randomly generate origin tolerance
			float tolerance = Random.Range ( 0, Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance + Settings.OriginAnyTolerance );
			bool useOrigin = tolerance <= Settings.OriginTolerance;
			bool useSubcategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance;
			bool useCategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance;

			// Cache prefix
			int random = 0;
			string prefix = "";

			// Check for origin
			if ( !ValidOrigin ( origin ) )
				origin = GetOrigin ( origin.Name );

			// Check for gender
			if ( gender == Gender.Label.NONE )
				gender = GetGender ( );

			// Check for matching origin
			if ( useOrigin )
			{
				// Filter data
				Filter ( NameData.EntryDataset.NAME_PREFIXES, originSubset, gender, origin );

				// Check data set
				if ( originSubset.Entries.Count > 0 )
				{
					// Generate random prefix
					random = Random.Range ( 0, originSubset.Entries.Count );
					prefix = originSubset.Entries [ random ].Name;

					// Convert special characters
					if ( UnicodeManager.SpecialCharacterCheck ( prefix ) )
						prefix = UnicodeManager.ConvertSpecialCharacter ( prefix, Settings.UseUnicodeCharacters );

					// Output performance
					performanceTracker.Stop ( );
					if ( Settings.OutputPerformance )
						Debug.Log ( "Prefix " + prefix + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

					// Return generated prefix
					return prefix;
				}
			}

			// Check for matching subcategory
			if ( useSubcategory )
			{
				// Filter data
				Filter ( NameData.EntryDataset.NAME_PREFIXES, subcategorySubset, gender, origin.Subcategory );

				// Check data set
				if ( subcategorySubset.Entries.Count > 0 )
				{
					// Generate random prefix
					random = Random.Range ( 0, subcategorySubset.Entries.Count );
					prefix = subcategorySubset.Entries [ random ].Name;

					// Convert special characters
					if ( UnicodeManager.SpecialCharacterCheck ( prefix ) )
						prefix = UnicodeManager.ConvertSpecialCharacter ( prefix, Settings.UseUnicodeCharacters );

					// Output performance
					performanceTracker.Stop ( );
					if ( Settings.OutputPerformance )
						Debug.Log ( "Prefix " + prefix + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

					// Return generated prefix
					return prefix;
				}
			}

			// Check for matching category
			if ( useCategory )
			{
				// Filter data
				Filter ( NameData.EntryDataset.NAME_PREFIXES, categorySubset, gender, origin.Category );

				// Check data set
				if ( categorySubset.Entries.Count > 0 )
				{
					// Generate random prefix
					random = Random.Range ( 0, categorySubset.Entries.Count );
					prefix = categorySubset.Entries [ random ].Name;

					// Convert special characters
					if ( UnicodeManager.SpecialCharacterCheck ( prefix ) )
						prefix = UnicodeManager.ConvertSpecialCharacter ( prefix, Settings.UseUnicodeCharacters );

					// Output performance
					performanceTracker.Stop ( );
					if ( Settings.OutputPerformance )
						Debug.Log ( "Prefix " + prefix + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

					// Return generated prefix
					return prefix;
				}
			}

			// Filter data
			Filter ( NameData.EntryDataset.NAME_PREFIXES, anySubset, gender );

			// Check data set
			if ( anySubset.Entries.Count > 0 )
			{
				// Generate random prefix
				random = Random.Range ( 0, anySubset.Entries.Count );
				prefix = anySubset.Entries [ random ].Name;

				// Convert special characters
				if ( UnicodeManager.SpecialCharacterCheck ( prefix ) )
					prefix = UnicodeManager.ConvertSpecialCharacter ( prefix, Settings.UseUnicodeCharacters );

				// Output performance
				performanceTracker.Stop ( );
				if ( Settings.OutputPerformance )
					Debug.Log ( "Prefix " + prefix + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

				// Return generated prefix
				return prefix;
			}
			else
			{
				// Output performance
				performanceTracker.Stop ( );
				if ( Settings.OutputPerformance )
					Debug.Log ( "No prefix generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

				// Return no prefix
				return "";
			}
		}

		#endregion // Public Prefix Generation Functions

		#region Public Given Name Generation Functions

		/// <summary>
		/// Generates a random given name.
		/// </summary>
		/// <returns> The generated given name. </returns>
		public static string GetGivenName ( )
		{
			// Get given name
			return GetGivenName ( GetOrigin ( "Any" ) );
		}

		/// <summary>
		/// Generates a random given name based on a specific origin name.
		/// </summary>
		/// <param name="origin"> The origin name the given name should be based on. Passing "", "None", or "Any" will generate a given name with a random origin. </param>
		/// <param name="gender"> The gender the given name should be based on. Passing Gender.Label.NONE will generate a given name with a random gender. </param>
		/// <param name="format"> The name format the given name should match. Passing Format.NONE will generate a given name to match a random format. </param>
		/// <returns> The generated given name. </returns>
		public static string GetGivenName ( string origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get given name
			return GetGivenName ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random given name based on a specific origin subcategory.
		/// </summary>
		/// <param name="origin"> The origin subcategory the given name should be based on. Passing Origin.SubcategoryType.NONE will generate a given name with a random origin. </param>
		/// <param name="gender"> The gender the given name should be based on. Passing Gender.Label.NONE will generate a given name with a random gender. </param>
		/// <param name="format"> The name format the given name should match. Passing Format.NONE will generate a given name to match a random format. </param>
		/// <returns> The generated given name. </returns>
		public static string GetGivenName ( Origin.SubcategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get given name
			return GetGivenName ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random given name based on a specific origin category.
		/// </summary>
		/// <param name="origin"> The origin category the given name should be based on. Passing Origin.CategoryType.NONE will generate a given name with a random origin. </param>
		/// <param name="gender"> The gender the given name should be based on. Passing Gender.Label.NONE will generate a given name with a random gender. </param>
		/// <param name="format"> The name format the given name should match. Passing Format.NONE will generate a given name to match a random format. </param>
		/// <returns> The generated given name. </returns>
		public static string GetGivenName ( Origin.CategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get given name
			return GetGivenName ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random given name based on a specific origin.
		/// </summary>
		/// <param name="origin"> The origin the given name should be based on. </param>
		/// <param name="gender"> The gender the given name should be based on. Passing Gender.Label.NONE will generate a given name with a random gender. </param>
		/// <param name="format"> The name format the given name should match. Passing Format.NONE will generate a given name to match a random format. </param>
		/// <returns> The generated given name. </returns>
		public static string GetGivenName ( Origin origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Check for loaded data
			if ( !NameData.IsNameDataLoaded )
				NameData.LoadNameData ( );

			// Track performance
			System.Diagnostics.Stopwatch performanceTracker = System.Diagnostics.Stopwatch.StartNew ( );

			// Randomly generate origin tolerance
			float tolerance = Random.Range ( 0, Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance + Settings.OriginAnyTolerance );
			bool useOrigin = tolerance <= Settings.OriginTolerance;
			bool useSubcategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance;
			bool useCategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance;

			// Cache given name
			int random = 0;
			string given = "";
			string name = "";

			// Check for origin
			if ( !ValidOrigin ( origin ) )
				origin = GetOrigin ( origin.Name );

			// Check for gender
			if ( gender == Gender.Label.NONE )
				gender = GetGender ( );

			// Check for format
			if ( format == Format.NONE )
				format = GetFormat ( );

			// Generate given name format
			bool useDouble = format == Format.GIVEN_GIVEN_FAMILY || format == Format.GIVEN_GIVEN_FAMILY_FAMILY;
			
			// Generate all given names
			for ( int i = 0; i < ( useDouble ? 2 : 1 ); i++ )
			{
				// Check for matching origin
				if ( useOrigin )
				{
					// Filter data
					if ( Settings.PreventRecentRepeats )
						Filter ( NameData.EntryDataset.GIVEN_NAMES, originSubset, gender, origin, recentGivenNames );
					else
						Filter ( NameData.EntryDataset.GIVEN_NAMES, originSubset, gender, origin );

					// Check data set
					if ( originSubset.Entries.Count > 0 )
					{
						// Generate random given name
						random = Random.Range ( 0, originSubset.Entries.Count );
						given = originSubset.Entries [ random ].Name;

						// Add name to recent list
						recentGivenNames [ recentGivenNameIndex ] = given;
						recentGivenNameIndex++;
						if ( recentGivenNameIndex >= recentGivenNames.Length )
							recentGivenNameIndex = 0;

						// Reset subset if recent names have been added
						if ( Settings.PreventRecentRepeats )
							originSubset.FilteredDataset = NameData.EntryDataset.NONE;

						// Return generated given name
						name += ( i > 0 ) ? " " + given : given;
						continue;
					}
				}

				// Check for matching subcategory
				if ( useSubcategory )
				{
					// Filter data
					if ( Settings.PreventRecentRepeats )
						Filter ( NameData.EntryDataset.GIVEN_NAMES, subcategorySubset, gender, origin.Subcategory, recentGivenNames );
					else
						Filter ( NameData.EntryDataset.GIVEN_NAMES, subcategorySubset, gender, origin.Subcategory );

					// Check data set
					if ( subcategorySubset.Entries.Count > 0 )
					{
						// Generate random given name
						random = Random.Range ( 0, subcategorySubset.Entries.Count );
						given = subcategorySubset.Entries [ random ].Name;

						// Add name to recent list
						recentGivenNames [ recentGivenNameIndex ] = given;
						recentGivenNameIndex++;
						if ( recentGivenNameIndex >= recentGivenNames.Length )
							recentGivenNameIndex = 0;

						// Reset subset if recent names have been added
						if ( Settings.PreventRecentRepeats )
							subcategorySubset.FilteredDataset = NameData.EntryDataset.NONE;

						// Return generated given name
						name += ( i > 0 ) ? " " + given : given;
						continue;
					}
				}

				// Check for matching category
				if ( useCategory )
				{
					// Filter data
					if ( Settings.PreventRecentRepeats )
						Filter ( NameData.EntryDataset.GIVEN_NAMES, categorySubset, gender, origin.Category, recentGivenNames );
					else
						Filter ( NameData.EntryDataset.GIVEN_NAMES, categorySubset, gender, origin.Category );

					// Check data set
					if ( categorySubset.Entries.Count > 0 )
					{
						// Generate random given name
						random = Random.Range ( 0, categorySubset.Entries.Count );
						given = categorySubset.Entries [ random ].Name;

						// Add name to recent list
						recentGivenNames [ recentGivenNameIndex ] = given;
						recentGivenNameIndex++;
						if ( recentGivenNameIndex >= recentGivenNames.Length )
							recentGivenNameIndex = 0;

						// Reset subset if recent names have been added
						if ( Settings.PreventRecentRepeats )
							categorySubset.FilteredDataset = NameData.EntryDataset.NONE;

						// Return generated given name
						name += ( i > 0 ) ? " " + given : given;
						continue;
					}
				}

				// Filter data
				if ( Settings.PreventRecentRepeats )
					Filter ( NameData.EntryDataset.GIVEN_NAMES, anySubset, gender, recentGivenNames );
				else
					Filter ( NameData.EntryDataset.GIVEN_NAMES, anySubset, gender );

				// Check data set
				if ( anySubset.Entries.Count > 0 )
				{
					// Generate random given name
					random = Random.Range ( 0, anySubset.Entries.Count );
					given = anySubset.Entries [ random ].Name;

					// Add name to recent list
					recentGivenNames [ recentGivenNameIndex ] = given;
					recentGivenNameIndex++;
					if ( recentGivenNameIndex >= recentGivenNames.Length )
						recentGivenNameIndex = 0;

					// Reset subset if recent names have been added
					if ( Settings.PreventRecentRepeats )
						anySubset.FilteredDataset = NameData.EntryDataset.NONE;

					// Return generated given name
					name += ( i > 0 ) ? " " + given : given;
					continue;
				}
			}

			// Convert special characters
			if ( UnicodeManager.SpecialCharacterCheck ( name ) )
				name = UnicodeManager.ConvertSpecialCharacter ( name, Settings.UseUnicodeCharacters );

			// Output performance
			performanceTracker.Stop ( );
			if ( Settings.OutputPerformance )
				Debug.Log ( "Given name " + name + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

			// Return generated given name
			return name;
		}

		#endregion // Public Given Name Generation Functions

		#region Public Family Name Generation Functions

		/// <summary>
		/// Generates a random family name.
		/// </summary>
		/// <returns> The generated family name. </returns>
		public static string GetFamilyName ( )
		{
			// Get family name
			return GetFamilyName ( GetOrigin ( "Any" ) );
		}

		/// <summary>
		/// Generates a random family name based on a specific origin name.
		/// </summary>
		/// <param name="origin"> The origin name the family name should be based on. Passing "", "None", or "Any" will generate a family name with a random origin. </param>
		/// <param name="gender"> The gender the family name should be based on. Passing Gender.Label.NONE will generate a family name with a random gender. </param>
		/// <param name="format"> The name format the family name should match. Passing Format.NONE will generate a family name to match a random format. </param>
		/// <returns> The generated family name. </returns>
		public static string GetFamilyName ( string origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get family name
			return GetFamilyName ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random family name based on a specific origin subcategory.
		/// </summary>
		/// <param name="origin"> The origin subcategory the family name should be based on. Passing Origin.SubcategoryType.NONE will generate a family name with a random origin. </param>
		/// <param name="gender"> The gender the family name should be based on. Passing Gender.Label.NONE will generate a family name with a random gender. </param>
		/// <param name="format"> The name format the family name should match. Passing Format.NONE will generate a family name to match a random format. </param>
		/// <returns> The generated family name. </returns>
		public static string GetFamilyName ( Origin.SubcategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get family name
			return GetFamilyName ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random family name based on a specific origin category.
		/// </summary>
		/// <param name="origin"> The origin category the family name should be based on. Passing Origin.CategoryType.NONE will generate a family name with a random origin. </param>
		/// <param name="gender"> The gender the family name should be based on. Passing Gender.Label.NONE will generate a family name with a random gender. </param>
		/// <param name="format"> The name format the family name should match. Passing Format.NONE will generate a family name to match a random format. </param>
		/// <returns> The generated family name. </returns>
		public static string GetFamilyName ( Origin.CategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get family name
			return GetFamilyName ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random family name based on a specific origin.
		/// </summary>
		/// <param name="origin"> The origin the family name should be based on. </param>
		/// <param name="gender"> The gender the family name should be based on. Passing Gender.Label.NONE will generate a family name with a random gender. </param>
		/// <param name="format"> The name format the family name should match. Passing Format.NONE will generate a family name to match a random format. </param>
		/// <returns> The generated family name. </returns>
		public static string GetFamilyName ( Origin origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Check for loaded data
			if ( !NameData.IsNameDataLoaded )
				NameData.LoadNameData ( );

			// Track performance
			System.Diagnostics.Stopwatch performanceTracker = System.Diagnostics.Stopwatch.StartNew ( );

			// Randomly generate origin tolerance
			float tolerance = Random.Range ( 0, Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance + Settings.OriginAnyTolerance );
			bool useOrigin = tolerance <= Settings.OriginTolerance;
			bool useSubcategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance;
			bool useCategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance;

			// Cache family name
			int random = 0;
			string family = "";
			string name = "";

			// Check for origin
			if ( !ValidOrigin ( origin ) )
				origin = GetOrigin ( origin.Name );

			// Check for gender
			if ( gender == Gender.Label.NONE )
				gender = GetGender ( );

			// Check for format
			if ( format == Format.NONE )
				format = GetFormat ( );

			// Generate family name format
			bool useDouble = format == Format.GIVEN_FAMILY_FAMILY || format == Format.GIVEN_GIVEN_FAMILY_FAMILY;

			// Generate all family names
			for ( int i = 0; i < ( useDouble ? 2 : 1 ); i++ )
			{
				// Check for matching origin
				if ( useOrigin )
				{
					// Filter data
					if ( Settings.PreventRecentRepeats )
						Filter ( NameData.EntryDataset.FAMILY_NAMES, originSubset, gender, origin, recentFamilyNames );
					else
						Filter ( NameData.EntryDataset.FAMILY_NAMES, originSubset, gender, origin );

					// Check data set
					if ( originSubset.Entries.Count > 0 )
					{
						// Generate random family name
						random = Random.Range ( 0, originSubset.Entries.Count );
						family = originSubset.Entries [ random ].Name;

						// Add name to recent list
						recentFamilyNames [ recentFamilyNameIndex ] = family;
						recentFamilyNameIndex++;
						if ( recentFamilyNameIndex >= recentFamilyNames.Length )
							recentFamilyNameIndex = 0;

						// Reset subset if recent names have been added
						if ( Settings.PreventRecentRepeats )
							originSubset.FilteredDataset = NameData.EntryDataset.NONE;

						// Return generated family name
						name += ( i > 0 ) ? " " + family : family;
						continue;
					}
				}

				// Check for matching subcategory
				if ( useSubcategory )
				{
					// Filter data
					if ( Settings.PreventRecentRepeats )
						Filter ( NameData.EntryDataset.FAMILY_NAMES, subcategorySubset, gender, origin.Subcategory, recentFamilyNames );
					else
						Filter ( NameData.EntryDataset.FAMILY_NAMES, subcategorySubset, gender, origin.Subcategory );

					// Check data set
					if ( subcategorySubset.Entries.Count > 0 )
					{
						// Generate random family name
						random = Random.Range ( 0, subcategorySubset.Entries.Count );
						family = subcategorySubset.Entries [ random ].Name;

						// Add name to recent list
						recentFamilyNames [ recentFamilyNameIndex ] = family;
						recentFamilyNameIndex++;
						if ( recentFamilyNameIndex >= recentFamilyNames.Length )
							recentFamilyNameIndex = 0;

						// Reset subset if recent names have been added
						if ( Settings.PreventRecentRepeats )
							subcategorySubset.FilteredDataset = NameData.EntryDataset.NONE;

						// Return generated family name
						name += ( i > 0 ) ? " " + family : family;
						continue;
					}
				}

				// Check for matching category
				if ( useCategory )
				{
					// Filter data
					if ( Settings.PreventRecentRepeats )
						Filter ( NameData.EntryDataset.FAMILY_NAMES, categorySubset, gender, origin.Category, recentFamilyNames );
					else
						Filter ( NameData.EntryDataset.FAMILY_NAMES, categorySubset, gender, origin.Category );

					// Check data set
					if ( categorySubset.Entries.Count > 0 )
					{
						// Generate random family name
						random = Random.Range ( 0, categorySubset.Entries.Count );
						family = categorySubset.Entries [ random ].Name;

						// Add name to recent list
						recentFamilyNames [ recentFamilyNameIndex ] = family;
						recentFamilyNameIndex++;
						if ( recentFamilyNameIndex >= recentFamilyNames.Length )
							recentFamilyNameIndex = 0;

						// Reset subset if recent names have been added
						if ( Settings.PreventRecentRepeats )
							categorySubset.FilteredDataset = NameData.EntryDataset.NONE;

						// Return generated family name
						name += ( i > 0 ) ? " " + family : family;
						continue;
					}
				}

				// Filter data
				if ( Settings.PreventRecentRepeats )
					Filter ( NameData.EntryDataset.FAMILY_NAMES, anySubset, gender, recentFamilyNames );
				else
					Filter ( NameData.EntryDataset.FAMILY_NAMES, anySubset, gender );

				// Check data set
				if ( anySubset.Entries.Count > 0 )
				{
					// Generate random family name
					random = Random.Range ( 0, anySubset.Entries.Count );
					family = anySubset.Entries [ random ].Name;

					// Add name to recent list
					recentFamilyNames [ recentFamilyNameIndex ] = family;
					recentFamilyNameIndex++;
					if ( recentFamilyNameIndex >= recentFamilyNames.Length )
						recentFamilyNameIndex = 0;

					// Reset subset if recent names have been added
					if ( Settings.PreventRecentRepeats )
						anySubset.FilteredDataset = NameData.EntryDataset.NONE;

					// Return generated family name
					name += ( i > 0 ) ? " " + family : family;
					continue;
				}
			}

			// Convert special characters
			if ( UnicodeManager.SpecialCharacterCheck ( name ) )
				name = UnicodeManager.ConvertSpecialCharacter ( name, Settings.UseUnicodeCharacters );

			// Output performance
			performanceTracker.Stop ( );
			if ( Settings.OutputPerformance )
				Debug.Log ( "Family name " + name + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

			// Return generated family name
			return name;
		}

		#endregion // Public Family Name Generation Functions

		#region Public Suffix Generation Functions

		/// <summary>
		/// Generates a random name suffix.
		/// </summary>
		/// <returns> The generated suffix. </returns>
		public static string GetSuffix ( )
		{
			// Get suffix
			return GetSuffix ( GetOrigin ( "Any" ) );
		}

		/// <summary>
		/// Generates a random name suffix based on a specific origin name.
		/// </summary>
		/// <param name="origin"> The origin name the suffix should be based on. Passing "", "None", or "Any" will generate a suffix with a random origin. </param>
		/// <param name="gender"> The gender the suffix should be based on. Passing Gender.Label.NONE will generate a suffix with a random gender. </param>
		/// <param name="format"> The name format the suffix should match. Passing Format.NONE will generate a suffix to match a random format. </param>
		/// <returns> The generated suffix. </returns>
		public static string GetSuffix ( string origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get suffix
			return GetSuffix ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random name suffix based on a specific origin subcategory.
		/// </summary>
		/// <param name="origin"> The origin subcategory the suffix should be based on. Passing Origin.SubcategoryType.NONE will generate a suffix with a random origin. </param>
		/// <param name="gender"> The gender the suffix should be based on. Passing Gender.Label.NONE will generate a suffix with a random gender. </param>
		/// <param name="format"> The name format the suffix should match. Passing Format.NONE will generate a suffix to match a random format. </param>
		/// <returns> The generated suffix. </returns>
		public static string GetSuffix ( Origin.SubcategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get suffix
			return GetSuffix ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random name suffix based on a specific origin category.
		/// </summary>
		/// <param name="origin"> The origin category the suffix should be based on. Passing Origin.CategoryType.NONE will generate a suffix with a random origin. </param>
		/// <param name="gender"> The gender the suffix should be based on. Passing Gender.Label.NONE will generate a suffix with a random gender. </param>
		/// <param name="format"> The name format the suffix should match. Passing Format.NONE will generate a suffix to match a random format. </param>
		/// <returns> The generated suffix. </returns>
		public static string GetSuffix ( Origin.CategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get suffix
			return GetSuffix ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random name suffix based on a specific origin.
		/// </summary>
		/// <param name="origin"> The origin the suffix should be based on. </param>
		/// <param name="gender"> The gender the suffix should be based on. Passing Gender.Label.NONE will generate a suffix with a random gender. </param>
		/// <param name="format"> The name format the suffix should match. Passing Format.NONE will generate a suffix to match a random format. </param>
		/// <returns> The generated suffix. </returns>
		public static string GetSuffix ( Origin origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Check for loaded data
			if ( !NameData.IsNameDataLoaded )
				NameData.LoadNameData ( );

			// Track performance
			System.Diagnostics.Stopwatch performanceTracker = System.Diagnostics.Stopwatch.StartNew ( );

			// Randomly generate origin tolerance
			float tolerance = Random.Range ( 0, Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance + Settings.OriginAnyTolerance );
			bool useOrigin = tolerance <= Settings.OriginTolerance;
			bool useSubcategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance;
			bool useCategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance;

			// Cache suffix
			int random = 0;
			string suffix = "";

			// Check for origin
			if ( !ValidOrigin ( origin ) )
				origin = GetOrigin ( origin.Name );

			// Check for gender
			if ( gender == Gender.Label.NONE )
				gender = GetGender ( );

			// Check for matching origin
			if ( useOrigin )
			{
				// Filter data
				Filter ( NameData.EntryDataset.NAME_SUFFIXES, originSubset, gender, origin );

				// Check data set
				if ( originSubset.Entries.Count > 0 )
				{
					// Generate random suffix
					random = Random.Range ( 0, originSubset.Entries.Count );
					suffix = originSubset.Entries [ random ].Name;

					// Convert special characters
					if ( UnicodeManager.SpecialCharacterCheck ( suffix ) )
						suffix = UnicodeManager.ConvertSpecialCharacter ( suffix, Settings.UseUnicodeCharacters );

					// Output performance
					performanceTracker.Stop ( );
					if ( Settings.OutputPerformance )
						Debug.Log ( "Suffix " + suffix + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

					// Return generated suffix
					return suffix;
				}
			}

			// Check for matching subcategory
			if ( useSubcategory )
			{
				// Filter data
				Filter ( NameData.EntryDataset.NAME_SUFFIXES, subcategorySubset, gender, origin.Subcategory );

				// Check data set
				if ( subcategorySubset.Entries.Count > 0 )
				{
					// Generate random suffix
					random = Random.Range ( 0, subcategorySubset.Entries.Count );
					suffix = subcategorySubset.Entries [ random ].Name;

					// Convert special characters
					if ( UnicodeManager.SpecialCharacterCheck ( suffix ) )
						suffix = UnicodeManager.ConvertSpecialCharacter ( suffix, Settings.UseUnicodeCharacters );

					// Output performance
					performanceTracker.Stop ( );
					if ( Settings.OutputPerformance )
						Debug.Log ( "Suffix " + suffix + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

					// Return generated suffix
					return suffix;
				}
			}

			// Check for matching category
			if ( useCategory )
			{
				// Filter data
				Filter ( NameData.EntryDataset.NAME_SUFFIXES, categorySubset, gender, origin.Category );

				// Check data set
				if ( categorySubset.Entries.Count > 0 )
				{
					// Generate random suffix
					random = Random.Range ( 0, categorySubset.Entries.Count );
					suffix = categorySubset.Entries [ random ].Name;

					// Convert special characters
					if ( UnicodeManager.SpecialCharacterCheck ( suffix ) )
						suffix = UnicodeManager.ConvertSpecialCharacter ( suffix, Settings.UseUnicodeCharacters );

					// Output performance
					performanceTracker.Stop ( );
					if ( Settings.OutputPerformance )
						Debug.Log ( "Suffix " + suffix + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

					// Return generated suffix
					return suffix;
				}
			}

			// Filter data
			Filter ( NameData.EntryDataset.NAME_SUFFIXES, anySubset, gender );

			// Check data set
			if ( anySubset.Entries.Count > 0 )
			{
				// Generate random suffix
				random = Random.Range ( 0, anySubset.Entries.Count );
				suffix = anySubset.Entries [ random ].Name;

				// Convert special characters
				if ( UnicodeManager.SpecialCharacterCheck ( suffix ) )
					suffix = UnicodeManager.ConvertSpecialCharacter ( suffix, Settings.UseUnicodeCharacters );

				// Output performance
				performanceTracker.Stop ( );
				if ( Settings.OutputPerformance )
					Debug.Log ( "Suffix " + suffix + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

				// Return generated suffix
				return suffix;
			}
			else
			{
				// Output performance
				performanceTracker.Stop ( );
				if ( Settings.OutputPerformance )
					Debug.Log ( "No suffix generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

				// Return no suffix
				return "";
			}
		}

		#endregion // Public Suffix Generation Functions

		#region Public Nickname Generation Functions

		/// <summary>
		/// Generates a random nickname.
		/// </summary>
		/// <returns> The generated nickname. </returns>
		public static string GetNickname ( )
		{
			// Get nickname
			return GetNickname ( GetOrigin ( "Any" ) );
		}

		/// <summary>
		/// Generates a random nickname based on a specific origin name.
		/// </summary>
		/// <param name="origin"> The origin name the nickname should be based on. Passing "", "None", or "Any" will generate a nickname with a random origin. </param>
		/// <param name="gender"> The gender the nickname should be based on. Passing Gender.Label.NONE will generate a nickname with a random gender. </param>
		/// <param name="format"> The name format the nickname should match. Passing Format.NONE will generate a nickname to match a random format. </param>
		/// <returns> The generated nickname. </returns>
		public static string GetNickname ( string origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get nickname
			return GetNickname ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random nickname based on a specific origin subcategory.
		/// </summary>
		/// <param name="origin"> The origin subcategory the nickname should be based on. Passing Origin.SubcategoryType.NONE will generate a nickname with a random origin. </param>
		/// <param name="gender"> The gender the nickname should be based on. Passing Gender.Label.NONE will generate a nickname with a random gender. </param>
		/// <param name="format"> The name format the nickname should match. Passing Format.NONE will generate a nickname to match a random format. </param>
		/// <returns> The generated nickname. </returns>
		public static string GetNickname ( Origin.SubcategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get nickname
			return GetNickname ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random nickname based on a specific origin category.
		/// </summary>
		/// <param name="origin"> The origin category the nickname should be based on. Passing Origin.CategoryType.NONE will generate a nickname with a random origin. </param>
		/// <param name="gender"> The gender the nickname should be based on. Passing Gender.Label.NONE will generate a nickname with a random gender. </param>
		/// <param name="format"> The name format the nickname should match. Passing Format.NONE will generate a nickname to match a random format. </param>
		/// <returns> The generated nickname. </returns>
		public static string GetNickname ( Origin.CategoryType origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Get nickname
			return GetNickname ( GetOrigin ( origin ), gender, format );
		}

		/// <summary>
		/// Generates a random nickname based on a specific origin.
		/// </summary>
		/// <param name="origin"> The origin the nickname should be based on. </param>
		/// <param name="gender"> The gender the nickname should be based on. Passing Gender.Label.NONE will generate a nickname with a random gender. </param>
		/// <param name="format"> The name format the nickname should match. Passing Format.NONE will generate a nickname to match a random format. </param>
		/// <returns> The generated nickname. </returns>
		public static string GetNickname ( Origin origin, Gender.Label gender = Gender.Label.NONE, Format format = Format.NONE )
		{
			// Check for loaded data
			if ( !NameData.IsNameDataLoaded )
				NameData.LoadNameData ( );

			// Track performance
			System.Diagnostics.Stopwatch performanceTracker = System.Diagnostics.Stopwatch.StartNew ( );

			// Randomly generate origin tolerance
			float tolerance = Random.Range ( 0, Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance + Settings.OriginAnyTolerance );
			bool useOrigin = tolerance <= Settings.OriginTolerance;
			bool useSubcategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance;
			bool useCategory = tolerance <= Settings.OriginTolerance + Settings.OriginSubcategoryTolerance + Settings.OriginCategoryTolerance;

			// Cache nickname
			int random = 0;
			string nickname = "";

			// Check for origin
			if ( !ValidOrigin ( origin ) )
				origin = GetOrigin ( origin.Name );

			// Check for gender
			if ( gender == Gender.Label.NONE )
				gender = GetGender ( );

			// Check for format
			if ( format == Format.NONE )
				format = GetFormat ( );

			// Check for matching origin
			if ( useOrigin )
			{
				// Filter data
				if ( Settings.PreventRecentRepeats )
					Filter ( NameData.EntryDataset.NICKNAMES, originSubset, gender, origin, recentNicknames );
				else
					Filter ( NameData.EntryDataset.NICKNAMES, originSubset, gender, origin );

				// Check data set
				if ( originSubset.Entries.Count > 0 )
				{
					// Generate random nickname
					random = Random.Range ( 0, originSubset.Entries.Count );
					nickname = originSubset.Entries [ random ].Name;

					// Add name to recent list
					recentNicknames [ recentNicknameIndex ] = nickname;
					recentNicknameIndex++;
					if ( recentNicknameIndex >= recentNicknames.Length )
						recentNicknameIndex = 0;

					// Reset subset if recent names have been added
					if ( Settings.PreventRecentRepeats )
						originSubset.FilteredDataset = NameData.EntryDataset.NONE;

					// Convert special characters
					if ( UnicodeManager.SpecialCharacterCheck ( nickname ) )
						nickname = UnicodeManager.ConvertSpecialCharacter ( nickname, Settings.UseUnicodeCharacters );

					// Output performance
					performanceTracker.Stop ( );
					if ( Settings.OutputPerformance )
						Debug.Log ( "Nickname " + nickname + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

					// Return generated nickname
					return nickname;
				}
			}

			// Check for matching subcategory
			if ( useSubcategory )
			{
				// Filter data
				if ( Settings.PreventRecentRepeats )
					Filter ( NameData.EntryDataset.NICKNAMES, subcategorySubset, gender, origin.Subcategory, recentNicknames );
				else
					Filter ( NameData.EntryDataset.NICKNAMES, subcategorySubset, gender, origin.Subcategory );

				// Check data set
				if ( subcategorySubset.Entries.Count > 0 )
				{
					// Generate random nickname
					random = Random.Range ( 0, subcategorySubset.Entries.Count );
					nickname = subcategorySubset.Entries [ random ].Name;

					// Add name to recent list
					recentNicknames [ recentNicknameIndex ] = nickname;
					recentNicknameIndex++;
					if ( recentNicknameIndex >= recentNicknames.Length )
						recentNicknameIndex = 0;

					// Reset subset if recent names have been added
					if ( Settings.PreventRecentRepeats )
						subcategorySubset.FilteredDataset = NameData.EntryDataset.NONE;

					// Convert special characters
					if ( UnicodeManager.SpecialCharacterCheck ( nickname ) )
						nickname = UnicodeManager.ConvertSpecialCharacter ( nickname, Settings.UseUnicodeCharacters );

					// Output performance
					performanceTracker.Stop ( );
					if ( Settings.OutputPerformance )
						Debug.Log ( "Nickname " + nickname + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

					// Return generated nickname
					return nickname;
				}
			}

			// Check for matching category
			if ( useCategory )
			{
				// Filter data
				if ( Settings.PreventRecentRepeats )
					Filter ( NameData.EntryDataset.NICKNAMES, categorySubset, gender, origin.Category, recentNicknames );
				else
					Filter ( NameData.EntryDataset.NICKNAMES, categorySubset, gender, origin.Category );

				// Check data set
				if ( categorySubset.Entries.Count > 0 )
				{
					// Generate random nickname
					random = Random.Range ( 0, categorySubset.Entries.Count );
					nickname = categorySubset.Entries [ random ].Name;

					// Add name to recent list
					recentNicknames [ recentNicknameIndex ] = nickname;
					recentNicknameIndex++;
					if ( recentNicknameIndex >= recentNicknames.Length )
						recentNicknameIndex = 0;

					// Reset subset if recent names have been added
					if ( Settings.PreventRecentRepeats )
						categorySubset.FilteredDataset = NameData.EntryDataset.NONE;

					// Convert special characters
					if ( UnicodeManager.SpecialCharacterCheck ( nickname ) )
						nickname = UnicodeManager.ConvertSpecialCharacter ( nickname, Settings.UseUnicodeCharacters );

					// Output performance
					performanceTracker.Stop ( );
					if ( Settings.OutputPerformance )
						Debug.Log ( "Nickname " + nickname + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

					// Return generated nickname
					return nickname;
				}
			}

			// Filter data
			if ( Settings.PreventRecentRepeats )
				Filter ( NameData.EntryDataset.NICKNAMES, anySubset, gender, recentNicknames );
			else
				Filter ( NameData.EntryDataset.NICKNAMES, anySubset, gender );

			// Check data set
			if ( anySubset.Entries.Count > 0 )
			{
				// Generate random nickname
				random = Random.Range ( 0, anySubset.Entries.Count );
				nickname = anySubset.Entries [ random ].Name;

				// Add name to recent list
				recentNicknames [ recentNicknameIndex ] = nickname;
				recentNicknameIndex++;
				if ( recentNicknameIndex >= recentNicknames.Length )
					recentNicknameIndex = 0;

				// Reset subset if recent names have been added
				if ( Settings.PreventRecentRepeats )
					anySubset.FilteredDataset = NameData.EntryDataset.NONE;

				// Convert special characters
				if ( UnicodeManager.SpecialCharacterCheck ( nickname ) )
					nickname = UnicodeManager.ConvertSpecialCharacter ( nickname, Settings.UseUnicodeCharacters );

				// Output performance
				performanceTracker.Stop ( );
				if ( Settings.OutputPerformance )
					Debug.Log ( "Nickname " + nickname + " generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

				// Return generated nickname
				return nickname;
			}
			else
			{
				// Output performance
				performanceTracker.Stop ( );
				if ( Settings.OutputPerformance )
					Debug.Log ( "No nickname generated in " + performanceTracker.ElapsedMilliseconds + " ms" );

				// Return no nickname
				return "";
			}
		}

		#endregion // Public Nickname Generation Functions

		#region Private Functions

		/// <summary>
		/// Filters a set of name entries into the appropriate subsets based on gender.
		/// </summary>
		/// <param name="dataset"> The set of name entries being filtered. </param>
		/// <param name="subset"> The subset of name entries used for filtering. </param>
		/// <param name="gender"> The gender to filter name entries by. </param>
		private static void Filter ( NameData.EntryDataset dataset, EntrySubset subset, Gender.Label gender )
		{
			// Check if the same filter is being used as the last filter
			if ( subset.FilteredDataset == dataset &&
				 subset.FilteredDataset != NameData.EntryDataset.NONE &&
				 subset.FilterdGender == gender &&
				 subset.FilterdGender != Gender.Label.NONE )
			{
				// Reuse the same subsets as the last filter
				return;
			}
			else
			{
				// Clear previous subsets
				subset.Entries.Clear ( );

				// Store filter
				subset.FilteredDataset = dataset;
				subset.FilterdGender = gender;
				subset.FilteredOrigin = 0;
			}

			// Get pre-filtered entries
			NameData.NameEntry [ ] temp = NameData.GetFilteredEntries ( dataset );

			// Filter data
			for ( int i = 0; i < temp.Length; i++ )
			{
				// Filter by gender
				if ( FilterByGender ( temp [ i ], gender ) )
					subset.Entries.Add ( temp [ i ] );
			}
		}

		/// <summary>
		/// Filters a set of name entries into the appropriate subsets based on gender and recently used name entries.
		/// </summary>
		/// <param name="dataset"> The set of name entries being filtered. </param>
		/// <param name="subset"> The subset of name entries used for filtering. </param>
		/// <param name="gender"> The gender to filter name entries by. </param>
		/// <param name="origin"> The origin to filter name entries by. </param>
		/// <param name="recent"> The set of recently used name entries. </param>
		private static void Filter ( NameData.EntryDataset dataset, EntrySubset subset, Gender.Label gender, string [ ] recent )
		{
			// Check if the same filter is being used as the last filter
			if ( subset.FilteredDataset == dataset &&
				 subset.FilteredDataset != NameData.EntryDataset.NONE &&
				 subset.FilterdGender == gender &&
				 subset.FilterdGender != Gender.Label.NONE )
			{
				// Reuse the same subsets as the last filter
				return;
			}
			else
			{
				// Clear previous subsets
				subset.Entries.Clear ( );

				// Store filter
				subset.FilteredDataset = dataset;
				subset.FilterdGender = gender;
				subset.FilteredOrigin = 0;
			}

			// Get pre-filtered entries
			NameData.NameEntry [ ] temp = NameData.GetFilteredEntries ( dataset );

			// Filter data
			for ( int i = 0; i < temp.Length; i++ )
			{
				// Filter by recent
				if ( !FilterByRecent ( temp [ i ], recent ) )
					continue;

				// Filter by gender
				if ( FilterByGender ( temp [ i ], gender ) )
					subset.Entries.Add ( temp [ i ] );
			}
		}

		/// <summary>
		/// Filters a set of name entries into the appropriate subsets based on gender and origin.
		/// </summary>
		/// <param name="dataset"> The set of name entries being filtered. </param>
		/// <param name="subset"> The subset of name entries used for filtering. </param>
		/// <param name="gender"> The gender to filter name entries by. </param>
		/// <param name="origin"> The origin to filter name entries by. </param>
		private static void Filter ( NameData.EntryDataset dataset, EntrySubset subset, Gender.Label gender, Origin origin )
		{
			// Check if the same filter is being used as the last filter
			if ( subset.FilteredDataset == dataset &&
				 subset.FilteredDataset != NameData.EntryDataset.NONE &&
				 subset.FilterdGender == gender &&
				 subset.FilterdGender != Gender.Label.NONE &&
				 subset.FilteredOrigin == origin.ID && 
				 subset.FilteredOrigin != 0 )
			{
				// Reuse the same subsets as the last filter
				return;
			}
			else
			{
				// Clear previous subsets
				subset.Entries.Clear ( );

				// Store filter
				subset.FilteredDataset = dataset;
				subset.FilterdGender = gender;
				subset.FilteredOrigin = origin.ID;
			}

			// Get pre-filtered entries
			NameData.NameEntry [ ] temp = NameData.GetFilteredEntries ( dataset, origin );

			// Filter data
			for ( int i = 0; i < temp.Length; i++ )
			{
				// Filter by gender
				if ( FilterByGender ( temp [ i ], gender ) )
					subset.Entries.Add ( temp [ i ] );
			}
		}

		/// <summary>
		/// Filters a set of name entries into the appropriate subsets based on gender, origin, and recently used name entries.
		/// </summary>
		/// <param name="dataset"> The set of name entries being filtered. </param>
		/// <param name="subset"> The subset of name entries used for filtering. </param>
		/// <param name="gender"> The gender to filter name entries by. </param>
		/// <param name="origin"> The origin to filter name entries by. </param>
		/// <param name="recent"> The set of recently used name entries. </param>
		private static void Filter ( NameData.EntryDataset dataset, EntrySubset subset, Gender.Label gender, Origin origin, string [ ] recent )
		{
			// Check if the same filter is being used as the last filter
			if ( subset.FilteredDataset == dataset &&
				 subset.FilteredDataset != NameData.EntryDataset.NONE &&
				 subset.FilterdGender == gender &&
				 subset.FilterdGender != Gender.Label.NONE &&
				 subset.FilteredOrigin == origin.ID &&
				 subset.FilteredOrigin != 0 )
			{
				// Reuse the same subsets as the last filter
				return;
			}
			else
			{
				// Clear previous subsets
				subset.Entries.Clear ( );

				// Store filter
				subset.FilteredDataset = dataset;
				subset.FilterdGender = gender;
				subset.FilteredOrigin = origin.ID;
			}

			// Get pre-filtered entries
			NameData.NameEntry [ ] temp = NameData.GetFilteredEntries ( dataset, origin );

			// Filter data
			for ( int i = 0; i < temp.Length; i++ )
			{
				// Filter by recent
				if ( !FilterByRecent ( temp [ i ], recent ) )
					continue;

				// Filter by gender
				if ( FilterByGender ( temp [ i ], gender ) )
					subset.Entries.Add ( temp [ i ] );
			}
		}

		/// <summary>
		/// Filters a set of name entries into the appropriate subsets based on gender and origin subcategory.
		/// </summary>
		/// <param name="dataset"> The set of name entries being filtered. </param>
		/// <param name="subset"> The subset of name entries used for filtering. </param>
		/// <param name="gender"> The gender to filter name entries by. </param>
		/// <param name="subcategory"> The origin subcategory to filter name entries by. </param>
		private static void Filter ( NameData.EntryDataset dataset, EntrySubset subset, Gender.Label gender, Origin.SubcategoryType subcategory )
		{
			// Check if the same filter is being used as the last filter
			if ( subset.FilteredDataset == dataset &&
				 subset.FilteredDataset != NameData.EntryDataset.NONE &&
				 subset.FilterdGender == gender &&
				 subset.FilterdGender != Gender.Label.NONE &&
				 subset.FilteredOrigin == ( int ) subcategory &&
				 subset.FilteredOrigin != ( int ) Origin.SubcategoryType.NONE )
			{
				// Reuse the same subsets as the last filter
				return;
			}
			else
			{
				// Clear previous subsets
				subset.Entries.Clear ( );

				// Store filter
				subset.FilteredDataset = dataset;
				subset.FilterdGender = gender;
				subset.FilteredOrigin = ( int )subcategory;
			}

			// Get pre-filtered entries
			Origin origin = GetOrigin ( subcategory );
			NameData.NameEntry [ ] temp = NameData.GetFilteredEntries ( dataset, origin );

			// Filter data
			for ( int i = 0; i < temp.Length; i++ )
			{
				// Filter by gender
				if ( FilterByGender ( temp [ i ], gender ) )
					subset.Entries.Add ( temp [ i ] );
			}
		}

		/// <summary>
		/// Filters a set of name entries into the appropriate subsets based on gender, origin subcategory, and recently used name entries.
		/// </summary>
		/// <param name="dataset"> The set of name entries being filtered. </param>
		/// <param name="subset"> The subset of name entries used for filtering. </param>
		/// <param name="gender"> The gender to filter name entries by. </param>
		/// <param name="subcategory"> The origin subcategory to filter name entries by. </param>
		/// <param name="recent"> The set of recently used name entries. </param>
		private static void Filter ( NameData.EntryDataset dataset, EntrySubset subset, Gender.Label gender, Origin.SubcategoryType subcategory, string [ ] recent )
		{
			// Check if the same filter is being used as the last filter
			if ( subset.FilteredDataset == dataset &&
				 subset.FilteredDataset != NameData.EntryDataset.NONE &&
				 subset.FilterdGender == gender &&
				 subset.FilterdGender != Gender.Label.NONE &&
				 subset.FilteredOrigin == ( int ) subcategory &&
				 subset.FilteredOrigin != ( int ) Origin.SubcategoryType.NONE )
			{
				// Reuse the same subsets as the last filter
				return;
			}
			else
			{
				// Clear previous subsets
				subset.Entries.Clear ( );

				// Store filter
				subset.FilteredDataset = dataset;
				subset.FilterdGender = gender;
				subset.FilteredOrigin = ( int ) subcategory;
			}

			// Get pre-filtered entries
			Origin origin = GetOrigin ( subcategory );
			NameData.NameEntry [ ] temp = NameData.GetFilteredEntries ( dataset, origin );

			// Filter data
			for ( int i = 0; i < temp.Length; i++ )
			{
				// Filter by recent
				if ( !FilterByRecent ( temp [ i ], recent ) )
					continue;

				// Filter by gender
				if ( FilterByGender ( temp [ i ], gender ) )
					subset.Entries.Add ( temp [ i ] );
			}
		}

		/// <summary>
		/// Filters a set of name entries into the appropriate subsets based on gender and origin category.
		/// </summary>
		/// <param name="dataset"> The set of name entries being filtered. </param>
		/// <param name="subset"> The subset of name entries used for filtering. </param>
		/// <param name="gender"> The gender to filter name entries by. </param>
		/// <param name="category"> The origin category to filter name entries by. </param>
		private static void Filter ( NameData.EntryDataset dataset, EntrySubset subset, Gender.Label gender, Origin.CategoryType category )
		{
			// Check if the same filter is being used as the last filter
			if ( subset.FilteredDataset == dataset &&
				 subset.FilteredDataset != NameData.EntryDataset.NONE &&
				 subset.FilterdGender == gender &&
				 subset.FilterdGender != Gender.Label.NONE &&
				 subset.FilteredOrigin == ( int ) category &&
				 subset.FilteredOrigin != ( int ) Origin.CategoryType.NONE )
			{
				// Reuse the same subsets as the last filter
				return;
			}
			else
			{
				// Clear previous subsets
				subset.Entries.Clear ( );

				// Store filter
				subset.FilteredDataset = dataset;
				subset.FilterdGender = gender;
				subset.FilteredOrigin = ( int ) category;
			}

			// Get pre-filtered entries
			Origin origin = GetOrigin ( category );
			NameData.NameEntry [ ] temp = NameData.GetFilteredEntries ( dataset, origin );

			// Filter data
			for ( int i = 0; i < temp.Length; i++ )
			{
				// Filter by gender
				if ( FilterByGender ( temp [ i ], gender ) )
					subset.Entries.Add ( temp [ i ] );
			}
		}

		/// <summary>
		/// Filters a set of name entries into the appropriate subsets based on gender, origin category, and recently used name entries.
		/// </summary>
		/// <param name="dataset"> The set of name entries being filtered. </param>
		/// <param name="subset"> The subset of name entries used for filtering. </param>
		/// <param name="gender"> The gender to filter name entries by. </param>
		/// <param name="category"> The origin category to filter name entries by. </param>
		/// <param name="recent"> The set of recently used name entries. </param>
		private static void Filter ( NameData.EntryDataset dataset, EntrySubset subset, Gender.Label gender, Origin.CategoryType category, string [ ] recent )
		{
			// Check if the same filter is being used as the last filter
			if ( subset.FilteredDataset == dataset &&
				 subset.FilteredDataset != NameData.EntryDataset.NONE &&
				 subset.FilterdGender == gender &&
				 subset.FilterdGender != Gender.Label.NONE &&
				 subset.FilteredOrigin == ( int ) category &&
				 subset.FilteredOrigin != ( int ) Origin.CategoryType.NONE )
			{
				// Reuse the same subsets as the last filter
				return;
			}
			else
			{
				// Clear previous subsets
				subset.Entries.Clear ( );

				// Store filter
				subset.FilteredDataset = dataset;
				subset.FilterdGender = gender;
				subset.FilteredOrigin = ( int ) category;
			}

			// Get pre-filtered entries
			Origin origin = GetOrigin ( category );
			NameData.NameEntry [ ] temp = NameData.GetFilteredEntries ( dataset, origin );

			// Filter data
			for ( int i = 0; i < temp.Length; i++ )
			{
				// Filter by recent
				if ( !FilterByRecent ( temp [ i ], recent ) )
					continue;

				// Filter by gender
				if ( FilterByGender ( temp [ i ], gender ) )
					subset.Entries.Add ( temp [ i ] );
			}
		}

		/// <summary>
		/// Filters a name entry to match a given gender.
		/// </summary>
		/// <param name="dataSet"> The name entry to filter. </param>
		/// <param name="gender"> The gender to filter by. </param>
		/// <returns> Whether or not name entry passes the filter. </returns>
		private static bool FilterByGender ( NameData.NameEntry data, Gender.Label gender )
		{
			// Check gender
			switch ( gender )
			{
			case Gender.Label.NONE:
				return true;
			case Gender.Label.MALE:
				return data.IsMale;
			case Gender.Label.FEMALE:
				return data.IsFemale;
			case Gender.Label.NON_BINARY:
				return data.IsMale && data.IsFemale;
			}

			// Return that the gender could not be filtered
			return false;
		}

		/// <summary>
		/// Filters a name entry to not match a set of recently used name entries.
		/// </summary>
		/// <param name="data"> The name entry to filter. </param>
		/// <param name="recent"> The recently used name entries. </param>
		/// <returns> Whether or not the name entry passes the filter. </returns>
		private static bool FilterByRecent ( NameData.NameEntry data, string [ ] recent )
		{
			// Check for data in recent data
			for ( int i = 0; i < recent.Length; i++ )
				if ( data.Name == recent [ i ] )
					return false;

			// Return that the data passed the recent filter
			return true;
		}

		/// <summary>
		/// Checks whether or not a given origin exists and is properly formated.
		/// </summary>
		/// <param name="origin"> The origin to validate. </param>
		/// <returns> Whether or not the origin is valid. </returns>
		private static bool ValidOrigin ( Origin origin )
		{
			// Check for valid any origin format
			if ( origin.Name == "Any" )
				return origin.Subcategory == Origin.SubcategoryType.NONE && origin.Category == Origin.CategoryType.NONE;

			// Search for matching origin names
			for ( int i = 0; i < NameData.Origins.Length; i++ )
				if ( NameData.Origins [ i ].Name == origin.Name )
					return NameData.Origins [ i ].Subcategory == origin.Subcategory && NameData.Origins [ i ].Category == origin.Category;

			// Return that no valid match was found
			return false;
		}

		#endregion // Private Functions
	}
}
