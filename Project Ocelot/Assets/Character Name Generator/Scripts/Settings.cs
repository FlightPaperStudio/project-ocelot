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
	/// <summary>
	/// A class containing data settings for the Character Name Generator.
	/// </summary>
	public static class Settings
	{
		#region Settings Data

		/// <summary>
		/// The probability of a character/name will be randomly generated as male.
		/// All gender chance settings should add to 1.0f to function accurately.
		/// </summary>
		public static float GenderMaleChance = GENDER_MALE_CHANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated as female.
		/// All gender chance settings should add to 1.0f to function accurately.
		/// </summary>
		public static float GenderFemaleChance = GENDER_FEMALE_CHANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated as non-binary.
		/// All gender chance settings should add to 1.0f to function accurately.
		/// </summary>
		public static float GenderNonBinaryChance = GENDER_NON_BINARY_CHANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated to have a prefix in their name.
		/// This setting should be set between 0f and 1.0f to function properly.
		/// </summary>
		public static float NamePrefixChance = NAME_PREFIX_CHANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated to have a suffix in their name.
		/// This setting should be set between 0f and 1.0f to function properly.
		/// </summary>
		public static float NameSuffixChance = NAME_SUFFIX_CHANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated to have two given names (e.g. Elizabeth Marie Smith).
		/// This setting should be set between 0f and 1.0f to function properly.
		/// </summary>
		public static float DoubleGivenNameChance = DOUBLE_GIVEN_NAME_CHANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated to have two family names (e.g. Jacob Washington Smith).
		/// This setting should be set between 0f and 1.0f to function properly.
		/// </summary>
		public static float DoubleFamilyNameChance = DOUBLE_FAMILY_NAME_CHANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated to match the origin of an existing or provided origin.
		/// If a matching origin is not available, a matching subcategory will be searched next.
		/// All origin tolerance settings should add to 1.0f to function accurately.
		/// </summary>
		public static float OriginTolerance = ORIGIN_TOLERANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated to match the subcategory of an existing or provided origin.
		/// If a matching subcategory is not available, a matching category will be searched next.
		/// All origin tolerance settings should add to 1.0f to function accurately.
		/// </summary>
		public static float OriginSubcategoryTolerance = ORIGIN_SUBCATEGORY_TOLERANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated to match the category of an existing or provided origin.
		/// If a matching category is not available, any available origin/subcategory/category will be searched next. 
		/// All origin tolerance settings should add to 1.0f to function accurately.
		/// </summary>
		public static float OriginCategoryTolerance = ORIGIN_CATEGORY_TOLERANCE_DEFAULT;

		/// <summary>
		/// The probability of a character/name will be randomly generated to match any origin/subcategory/category regardless of an existing or provided origin.
		/// All origin tolerance settings should add to 1.0f to function accurately.
		/// </summary>
		public static float OriginAnyTolerance = ORIGIN_ANY_TOLERANCE_DEFAULT;

		/// <summary>
		/// Whether or not special characters contianed within names should be converted into their associated unicode character or defaulted to their basic ASCII character.
		/// </summary>
		public static bool UseUnicodeCharacters = USE_UNICODE_CHARACTERS_DEFAULT;

		/// <summary>
		/// Whether or not the generator should avoid giving a character the same given, family, or nickname as any other past 100 characters generated 
		/// (e.g. James "Tank" Prescott would prevent John "Tank" Smith and Jane "Inferno" Prescott but would allow Jessica "Nova" James).
		/// Disable this setting for faster performance from the generator.
		/// </summary>
		public static bool PreventRecentRepeats = PREVENT_RECENT_REPEATS_DEFAULT;

		#endregion // Settings Data

		#region Settings Defaults

		/// <summary>
		/// The default value for the Gender Male Chance Setting.
		/// </summary>
		public const float GENDER_MALE_CHANCE_DEFAULT = 0.45f;

		/// <summary>
		/// The default value for the Gender Female Chance Setting.
		/// </summary>
		public const float GENDER_FEMALE_CHANCE_DEFAULT = 0.45f;

		/// <summary>
		/// The default value for the Gender Non-Binary Chance Setting.
		/// </summary>
		public const float GENDER_NON_BINARY_CHANCE_DEFAULT = 0.1f;

		/// <summary>
		/// The default value for the Name Prefix Chance Setting.
		/// </summary>
		public const float NAME_PREFIX_CHANCE_DEFAULT = 0.2f;

		/// <summary>
		/// The default value for the Name Suffix Chance Setting.
		/// </summary>
		public const float NAME_SUFFIX_CHANCE_DEFAULT = 0.1f;

		/// <summary>
		/// The default value for the Double Given Name Chance Setting.
		/// </summary>
		public const float DOUBLE_GIVEN_NAME_CHANCE_DEFAULT = 0.2f;

		/// <summary>
		/// The default value for the Double Family Name Chance Setting.
		/// </summary>
		public const float DOUBLE_FAMILY_NAME_CHANCE_DEFAULT = 0.2f;

		/// <summary>
		/// The default value for the Origin Tolerance Setting.
		/// </summary>
		public const float ORIGIN_TOLERANCE_DEFAULT = 0.9f;

		/// <summary>
		/// The default value for the Origin Subcategory Tolerance Setting.
		/// </summary>
		public const float ORIGIN_SUBCATEGORY_TOLERANCE_DEFAULT = 0.07f;

		/// <summary>
		/// The default value for the Origin Category Tolerance Setting.
		/// </summary>
		public const float ORIGIN_CATEGORY_TOLERANCE_DEFAULT = 0.03f;

		/// <summary>
		/// The default value for the Origin Any Tolerance Setting.
		/// </summary>
		public const float ORIGIN_ANY_TOLERANCE_DEFAULT = 0f;

		/// <summary>
		/// The default value for the Unicode Characters Setting.
		/// </summary>
		public const bool USE_UNICODE_CHARACTERS_DEFAULT = true;

		/// <summary>
		/// The default value for the Recent Repeat Setting.
		/// </summary>
		public const bool PREVENT_RECENT_REPEATS_DEFAULT = true;

		#endregion // Settings Defaults
	}
}
