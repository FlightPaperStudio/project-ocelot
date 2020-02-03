/* Character Name Generator v.1.0.2
 * --------------------------------------------------------------------------------------------------------------------------------------------------
 * 
 * This file is part of Character Name Generator which is released under the Unity Asset Store End User License Agreement.
 * See file Documentation.pdf or go to https://unity3d.com/legal/as_terms for full license details.
 * 
 * Copyright (c) : 2019
 * Flight Paper Studio LLC
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CNG;
using System.Linq;

/// <summary>
/// A class for controlling the example scene included with the Character Name Generator.
/// </summary>
public class Example : MonoBehaviour
{
	#region Private Classes

	/// <summary>
	/// A class for displaying the character information as a UI element.
	/// </summary>
	[System.Serializable]
	private class CharacterDisplay
	{
		public GameObject Container;
		public Text CharacterName;
		public Text CharacterNickname;
		public Text CharacterGender;
		public Text CharacterOrigin;

		/// <summary>
		/// Displays a character's information.
		/// </summary>
		/// <param name="character"> The character to be displayed. </param>
		public void DisplayCharacter ( Character character )
		{
			// Display character
			Container.SetActive ( true );
			CharacterName.text = character.WesternNameOrder;
			CharacterNickname.text = character.QuotedNickname;
			CharacterGender.text = Gender.ToString ( character.Gender );
			CharacterOrigin.text = character.Origin.Name;
		}
	}

	#endregion // Private Classes

	#region UI Elements

	[SerializeField]
	private CharacterDisplay [ ] displays;

	[SerializeField]
	private Dropdown categoryDropdown;

	[SerializeField]
	private Dropdown subcategoryDropdown;

	[SerializeField]
	private Dropdown originDropdown;

	[SerializeField]
	private Text prefixStat;

	[SerializeField]
	private Text givenStats;

	[SerializeField]
	private Text familyStats;

	[SerializeField]
	private Text suffixStats;

	[SerializeField]
	private Text nicknameStats;

	#endregion // UI Elements

	#region Example Data

	private Gender.Label genderFilter = Gender.Label.NONE;
	private Origin.CategoryType categoryFilter = Origin.CategoryType.NONE;
	private Origin.SubcategoryType subcategoryFilter = Origin.SubcategoryType.NONE;
	private string originFilter = "Any";

	#endregion // Example Data

	#region Public Functions

	/// <summary>
	/// Loads the name generator.
	/// </summary>
	public void Load ( )
	{
		// Load generator
		NameData.LoadNameData ( );

		// Hide character displays
		for ( int i = 0; i < displays.Length; i++ )
			displays [ i ].Container.SetActive ( false );

		// Hide dropdowns
		subcategoryDropdown.gameObject.SetActive ( false );
		originDropdown.gameObject.SetActive ( false );

		// Display stats
		DisplayStats ( );
	}

	/// <summary>
	/// Sets the gender filter.
	/// </summary>
	/// <param name="index"> The selected index from the dropdown. </param>
	public void SetGenderFilter ( int index )
	{
		// Set filter
		genderFilter = (Gender.Label)index;

		// Update stats
		DisplayStats ( );
	}

	/// <summary>
	/// Sets the category filter.
	/// </summary>
	/// <param name="index"> The selected index from the dropdown. </param>
	public void SetCategoryFilter ( int index )
	{
		// Set category
		categoryFilter = index == 0 ? Origin.CategoryType.NONE : Origin.CategoryFromString ( categoryDropdown.options [ index ].text );

		// Set subcategory dropdown
		subcategoryDropdown.gameObject.SetActive ( index != 0 );
		if ( index != 0 )
		{
			// Get data set
			Origin [ ] subset = NameData.Origins;

			// Filter by category
			subset = subset.Where ( x => x.Category == categoryFilter ).ToArray ( );

			// Get subcategories
			List<Origin.SubcategoryType> subcategories = new List<Origin.SubcategoryType> ( );
			for ( int i = 0; i < subset.Length; i++ )
				if ( !subcategories.Contains ( subset [ i ].Subcategory ) )
					subcategories.Add ( subset [ i ].Subcategory );

			// Populate dropdown
			subcategoryDropdown.ClearOptions ( );
			List<string> options = new List<string> ( );
			options.Add ( "Any Subcategory" );
			for ( int i = 0; i < subcategories.Count; i++ )
				options.Add ( Origin.ToString ( subcategories [ i ] ) );
			subcategoryDropdown.AddOptions ( options );
		}

		// Hide origin dropdown
		originDropdown.gameObject.SetActive ( false );

		// Set filters
		subcategoryFilter = Origin.SubcategoryType.NONE;
		subcategoryDropdown.value = 0;
		originFilter = "Any";
		originDropdown.value = 0;

		// Update stats
		DisplayStats ( ); 
	}

	/// <summary>
	/// Sets the subcategory filter.
	/// </summary>
	/// <param name="index"> The selected index of the dropdown. </param>
	public void SetSubcategoryFilter ( int index )
	{
		// Set subcategory
		subcategoryFilter = index == 0 ? Origin.SubcategoryType.NONE : Origin.SubcategoryFromString ( subcategoryDropdown.options [ index ].text );

		// Set origin dropdown
		originDropdown.gameObject.SetActive ( index != 0 );
		if ( index != 0 )
		{
			// Get data set
			Origin [ ] subset = NameData.Origins;

			// Filter by category
			subset = subset.Where ( x => x.Subcategory == subcategoryFilter ).ToArray ( );

			// Get subcategories
			List<string> origins = new List<string> ( );
			origins.Add ( "Any Origin" );
			for ( int i = 0; i < subset.Length; i++ )
				if ( !origins.Contains ( subset [ i ].Name ) )
					origins.Add ( subset [ i ].Name );

			// Populate dropdown
			originDropdown.ClearOptions ( );
			originDropdown.AddOptions ( origins );
		}

		// Set filters
		originFilter = "Any";
		originDropdown.value = 0;

		// Update stats
		DisplayStats ( );
	}

	/// <summary>
	/// Sets the origin filter.
	/// </summary>
	/// <param name="index"> The selected index of the dropdown. </param>
	public void SetOriginFilter ( int index )
	{
		// Set origin
		originFilter = index == 0 ? "Any" : originDropdown.options [ index ].text;

		// Update stats
		DisplayStats ( );
	}

	/// <summary>
	/// Generates ten characters based on the current filters.
	/// </summary>
	public void GenerateCharacters ( )
	{
		// Track performance
		var performanceTracker = System.Diagnostics.Stopwatch.StartNew ( );

		// Generate 10 characters
		for ( int i = 0; i < displays.Length; i++ )
		{
			// Check category
			if ( categoryFilter != Origin.CategoryType.NONE )
			{
				// Check subcategory
				if ( subcategoryFilter != Origin.SubcategoryType.NONE )
				{
					// Check origin
					if ( originFilter != "Any" )
					{
						// Generate by origin
						displays [ i ].DisplayCharacter ( NameGenerator.GetCharacter ( originFilter, genderFilter ) );
					}
					else
					{
						// Generate by subcategory
						displays [ i ].DisplayCharacter ( NameGenerator.GetCharacter ( subcategoryFilter, genderFilter ) );
					}
				}
				else
				{
					// Generate by category
					displays [ i ].DisplayCharacter ( NameGenerator.GetCharacter ( categoryFilter, genderFilter ) );
				}
			}
			else
			{
				// Generate any category
				displays [ i ].DisplayCharacter ( NameGenerator.GetCharacter ( Origin.CategoryType.NONE, genderFilter ) );
			}
		}

		// Output performance
		performanceTracker.Stop ( );
		Debug.Log ( displays.Length + " characters generated in " + performanceTracker.ElapsedMilliseconds + " ms" );
		Debug.Log ( "Average character generation: " + ( performanceTracker.ElapsedMilliseconds / displays.Length ) + " ms" );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Displays stats on the current number of names based on the current filters.
	/// </summary>
	private void DisplayStats ( )
	{
		// Display prefixes
		prefixStat.text = "Total Name Prefixes : " + GetPrefixStats ( );

		// Display given names
		givenStats.text = "Total Given Names : " + GetGivenStats ( );

		// Display family names
		familyStats.text = "Total Family Names : " + GetFamilyStats ( );

		// Display suffixes
		suffixStats.text = "Total Name Suffixes : " + GetSuffixStats ( );

		// Display nicknames
		nicknameStats.text = "Total Nicknames : " + GetNicknameStats ( );
	}

	/// <summary>
	/// Gets the number of prefixs based on the current filters.
	/// </summary>
	/// <returns> The number of prefixes available. </returns>
	private int GetPrefixStats ( )
	{
		// Get data set
		NameData.NameEntry [ ] subset = NameData.NamePrefixes;

		// Filter by gender
		if ( genderFilter != Gender.Label.NONE )
		{
			switch ( genderFilter )
			{
			case Gender.Label.MALE:
				subset = subset.Where ( x => x.IsMale ).ToArray ( );
				break;
			case Gender.Label.FEMALE:
				subset = subset.Where ( x => x.IsFemale ).ToArray ( );
				break;
			case Gender.Label.NON_BINARY:
				subset = subset.Where ( x => x.IsMale && x.IsFemale ).ToArray ( );
				break;
			}
		}

		// Filter by category
		if ( categoryFilter != Origin.CategoryType.NONE )
		{
			// Filter by subcategory
			if ( subcategoryFilter != Origin.SubcategoryType.NONE )
			{
				// Filter by origin
				if ( originFilter != "Any" )
				{
					// Filter data by origin
					subset = subset.Where ( x => x.Origins.Any ( y => y.Name == originFilter || y.Name == "Any" ) ).ToArray ( );
				}
				else
				{
					// Filter data by subcategory
					subset = subset.Where ( x => x.Origins.Any ( y => y.Subcategory == subcategoryFilter || y.Name == "Any" ) ).ToArray ( );
				}
			}
			else
			{
				// Filter data by category
				subset = subset.Where ( x => x.Origins.Any ( y => y.Category == categoryFilter || y.Name == "Any" ) ).ToArray ( );
			}
		}

		// Return total
		return subset.Length;
	}

	/// <summary>
	/// Gets the number of given names based on the current filters.
	/// </summary>
	/// <returns> The number of given names available. </returns>
	private int GetGivenStats ( )
	{
		// Get data set
		NameData.NameEntry [ ] subset = NameData.GivenNames;

		// Filter by gender
		if ( genderFilter != Gender.Label.NONE )
		{
			switch ( genderFilter )
			{
			case Gender.Label.MALE:
				subset = subset.Where ( x => x.IsMale ).ToArray ( );
				break;
			case Gender.Label.FEMALE:
				subset = subset.Where ( x => x.IsFemale ).ToArray ( );
				break;
			case Gender.Label.NON_BINARY:
				subset = subset.Where ( x => x.IsMale && x.IsFemale ).ToArray ( );
				break;
			}
		}

		// Filter by category
		if ( categoryFilter != Origin.CategoryType.NONE )
		{
			// Filter by subcategory
			if ( subcategoryFilter != Origin.SubcategoryType.NONE )
			{
				// Filter by origin
				if ( originFilter != "Any" )
				{
					// Filter data by origin
					subset = subset.Where ( x => x.Origins.Any ( y => y.Name == originFilter || y.Name == "Any" ) ).ToArray ( );
				}
				else
				{
					// Filter data by subcategory
					subset = subset.Where ( x => x.Origins.Any ( y => y.Subcategory == subcategoryFilter || y.Name == "Any" ) ).ToArray ( );
				}
			}
			else
			{
				// Filter data by category
				subset = subset.Where ( x => x.Origins.Any ( y => y.Category == categoryFilter || y.Name == "Any" ) ).ToArray ( );
			}
		}

		// Return total
		return subset.Length;
	}

	/// <summary>
	/// Gets the number of family names based on the current filters.
	/// </summary>
	/// <returns> The number of family names available. </returns>
	private int GetFamilyStats ( )
	{
		// Get data set
		NameData.NameEntry [ ] subset = NameData.FamilyNames;

		// Filter by category
		if ( categoryFilter != Origin.CategoryType.NONE )
		{
			// Filter by subcategory
			if ( subcategoryFilter != Origin.SubcategoryType.NONE )
			{
				// Filter by origin
				if ( originFilter != "Any" )
				{
					// Filter data by origin
					subset = subset.Where ( x => x.Origins.Any ( y => y.Name == originFilter || y.Name == "Any" ) ).ToArray ( );
				}
				else
				{
					// Filter data by subcategory
					subset = subset.Where ( x => x.Origins.Any ( y => y.Subcategory == subcategoryFilter || y.Name == "Any" ) ).ToArray ( );
				}
			}
			else
			{
				// Filter data by category
				subset = subset.Where ( x => x.Origins.Any ( y => y.Category == categoryFilter || y.Name == "Any" ) ).ToArray ( );
			}
		}

		// Return total
		return subset.Length;
	}

	/// <summary>
	/// Gets the number of suffixes based on the current filters.
	/// </summary>
	/// <returns> The number of suffixes available. </returns>
	private int GetSuffixStats ( )
	{
		// Get data set
		NameData.NameEntry [ ] subset = NameData.NameSuffixes;

		// Filter by category
		if ( categoryFilter != Origin.CategoryType.NONE )
		{
			// Filter by subcategory
			if ( subcategoryFilter != Origin.SubcategoryType.NONE )
			{
				// Filter by origin
				if ( originFilter != "Any" )
				{
					// Filter data by origin
					subset = subset.Where ( x => x.Origins.Any ( y => y.Name == originFilter || y.Name == "Any" ) ).ToArray ( );
				}
				else
				{
					// Filter data by subcategory
					subset = subset.Where ( x => x.Origins.Any ( y => y.Subcategory == subcategoryFilter || y.Name == "Any" ) ).ToArray ( );
				}
			}
			else
			{
				// Filter data by category
				subset = subset.Where ( x => x.Origins.Any ( y => y.Category == categoryFilter || y.Name == "Any" ) ).ToArray ( );
			}
		}

		// Return total
		return subset.Length;
	}

	/// <summary>
	/// Gets the number of nicknames based on the current filters.
	/// </summary>
	/// <returns> The number of nicknames available. </returns>
	private int GetNicknameStats ( )
	{
		// Get data set
		NameData.NameEntry [ ] subset = NameData.Nicknames;

		// Filter by gender
		if ( genderFilter != Gender.Label.NONE )
		{
			switch ( genderFilter )
			{
			case Gender.Label.MALE:
				subset = subset.Where ( x => x.IsMale ).ToArray ( );
				break;
			case Gender.Label.FEMALE:
				subset = subset.Where ( x => x.IsFemale ).ToArray ( );
				break;
			case Gender.Label.NON_BINARY:
				subset = subset.Where ( x => x.IsMale && x.IsFemale ).ToArray ( );
				break;
			}
		}

		// Filter by category
		if ( categoryFilter != Origin.CategoryType.NONE )
		{
			// Filter by subcategory
			if ( subcategoryFilter != Origin.SubcategoryType.NONE )
			{
				// Filter by origin
				if ( originFilter != "Any" )
				{
					// Filter data by origin
					subset = subset.Where ( x => x.Origins.Any ( y => y.Name == originFilter || y.Name == "Any" ) ).ToArray ( );
				}
				else
				{
					// Filter data by subcategory
					subset = subset.Where ( x => x.Origins.Any ( y => y.Subcategory == subcategoryFilter || y.Name == "Any" ) ).ToArray ( );
				}
			}
			else
			{
				// Filter data by category
				subset = subset.Where ( x => x.Origins.Any ( y => y.Category == categoryFilter || y.Name == "Any" ) ).ToArray ( );
			}
		}

		// Return total
		return subset.Length;
	}

	#endregion // Private Functions
}
