/* Character Name Generator v.1.0.2
 * --------------------------------------------------------------------------------------------------------------------------------------------------
 * 
 * This file is part of Character Name Generator which is released under the Unity Asset Store End User License Agreement.
 * See file Documentation.pdf or go to https://unity3d.com/legal/as_terms for full license details.
 * 
 * Copyright (c) : 2019
 * Flight Paper Studio LLC
 */

using System;

namespace CNG
{
	/// <summary>
	/// A class for storing origin data for a name.
	/// </summary>
	[System.Serializable]
	public class Origin
	{
		#region Public Enums

		/// <summary>
		/// The types of group for an origin.
		/// </summary>
		public enum CategoryType
		{
			NONE,
			AFRICAN,
			AMERICAN,
			ASIAN,
			EUROPEAN,
			INDIGENOUS,
			OTHER
		}

		/// <summary>
		/// The types of subgroup for an origin.
		/// </summary>
		public enum SubcategoryType
		{
			NONE,
			NORTHERN_AFRICAN,
			SOUTHERN_AFRICAN,
			WESTERN_AFRICAN,
			EASTERN_AFRICAN,
			CENTRAL_AMERICAN,
			NORTHERN_AMERICAN,
			SOUTHERN_AMERICAN,
			CENTRAL_ASIAN,
			SOUTHERN_ASIAN,
			SOUTHEASTERN_ASIAN,
			WESTERN_ASIAN,
			EASTERN_ASIAN,
			NORTHERN_EUROPEAN,
			SOUTHERN_EUROPEAN,
			WESTERN_EUROPEAN,
			EASTERN_EUROPEAN,
			OCEANIC,
			AMERICAN,
			FANTASY,
			SCIENCE_FICTION,
			OTHER
		}

		#endregion // Public Enums

		#region Origin Data

		/// <summary>
		/// The ID of the origin.
		/// </summary>
		public int ID = 0;

		/// <summary>
		/// The name of the origin.
		/// </summary>
		public string Name = "None";

		/// <summary>
		/// The type of the origin's subcategory.
		/// </summary>
		public SubcategoryType Subcategory = SubcategoryType.NONE;

		/// <summary>
		/// The type of the origin's category.
		/// </summary>
		public CategoryType Category = CategoryType.NONE;

		/// <summary>
		/// The weighted percentage value of this origin.
		/// </summary>
		public float Weight = 0f;

		#endregion // Origin Data

		#region Public Functions

		/// <summary>
		/// Converts a subcategory to a readable string.
		/// </summary>
		/// <param name="subcategory"> The subcategory to be converted. </param>
		/// <returns> The subcategory as a readable string. </returns>
		public static string ToString ( SubcategoryType subcategory )
		{
			// Check subcategory
			switch ( subcategory )
			{
			case SubcategoryType.NONE:
				return "None";
			case SubcategoryType.NORTHERN_AFRICAN:
				return "Northern African";
			case SubcategoryType.SOUTHERN_AFRICAN:
				return "Southern African";
			case SubcategoryType.WESTERN_AFRICAN:
				return "Western African";
			case SubcategoryType.EASTERN_AFRICAN:
				return "Eastern African";
			case SubcategoryType.CENTRAL_AMERICAN:
				return "Central American";
			case SubcategoryType.NORTHERN_AMERICAN:
				return "Northern American";
			case SubcategoryType.SOUTHERN_AMERICAN:
				return "Southern American";
			case SubcategoryType.CENTRAL_ASIAN:
				return "Central Asian";
			case SubcategoryType.SOUTHERN_ASIAN:
				return "Southern Asian";
			case SubcategoryType.SOUTHEASTERN_ASIAN:
				return "Southeastern Asian";
			case SubcategoryType.WESTERN_ASIAN:
				return "Western Asian";
			case SubcategoryType.EASTERN_ASIAN:
				return "Eastern Asian";
			case SubcategoryType.NORTHERN_EUROPEAN:
				return "Northern European";
			case SubcategoryType.SOUTHERN_EUROPEAN:
				return "Southern European";
			case SubcategoryType.WESTERN_EUROPEAN:
				return "Western European";
			case SubcategoryType.EASTERN_EUROPEAN:
				return "Eastern European";
			case SubcategoryType.OCEANIC:
				return "Oceanic";
			case SubcategoryType.AMERICAN:
				return "American";
			case SubcategoryType.FANTASY:
				return "Fantasy";
			case SubcategoryType.SCIENCE_FICTION:
				return "Science Fiction";
			case SubcategoryType.OTHER:
				return "Other";
			}

			// Return that no subcategory was found
			return "";
		}

		/// <summary>
		/// Converts a category to a readable string.
		/// </summary>
		/// <param name="category"> The category to be converted. </param>
		/// <returns> The category as a readable string. </returns>
		public static string ToString ( CategoryType category )
		{
			// Check category
			switch ( category )
			{
			case CategoryType.NONE:
				return "None";
			case CategoryType.AFRICAN:
				return "African";
			case CategoryType.AMERICAN:
				return "American";
			case CategoryType.ASIAN:
				return "Asian";
			case CategoryType.EUROPEAN:
				return "European";
			case CategoryType.INDIGENOUS:
				return "Indigenous";
			case CategoryType.OTHER:
				return "Other";
			}

			// Return that no subcategory was found
			return "";
		}

		/// <summary>
		/// Converts a string to a subcategory.
		/// </summary>
		/// <param name="subcategory"> The string to be converted. </param>
		/// <returns> The converted subcategory. </returns>
		public static SubcategoryType SubcategoryFromString ( string subcategory )
		{
			// Format string
			subcategory = subcategory.ToUpper ( );
			subcategory = subcategory.Replace ( " ", "_" );

			// Convert string to enum
			try
			{
				// Convert string
				SubcategoryType data = (SubcategoryType)Enum.Parse ( typeof ( SubcategoryType ), subcategory );

				// Check for valid enum
				if ( Enum.IsDefined ( typeof ( SubcategoryType ), data ) )
				{
					// Return the subcategory
					return data;
				}
				else
				{
					// Return that the string could not be converted.
					return SubcategoryType.NONE;
				}
			}
			catch
			{
				// Return that the string could not be converted.
				return SubcategoryType.NONE;
			}
		}

		/// <summary>
		/// Converts a string to a category.
		/// </summary>
		/// <param name="category"> The string to be converted. </param>
		/// <returns> The converted category. </returns>
		public static CategoryType CategoryFromString ( string category )
		{
			// Format string
			category = category.ToUpper ( );
			category = category.Replace ( " ", "_" );

			// Convert string to enum
			try
			{
				// Convert string
				CategoryType data = (CategoryType)Enum.Parse ( typeof ( CategoryType ), category );

				// Check for valid enum
				if ( Enum.IsDefined ( typeof ( CategoryType ), data ) )
				{
					// Return the category
					return data;
				}
				else
				{
					// Return that the string could not be converted.
					return CategoryType.NONE;
				}
			}
			catch
			{
				// Return that the string could not be converted.
				return CategoryType.NONE;
			}
		}

		#endregion // Public Functions
	}
}