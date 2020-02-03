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
	/// <summary>
	/// A class for handling data associated with a character's gender.
	/// </summary>
	public class Gender
	{
		#region Public Enums

		/// <summary>
		/// A typed set of gender labels.
		/// </summary>
		public enum Label
		{
			NONE,
			MALE,
			FEMALE,
			NON_BINARY
		}

		/// <summary>
		/// A typed set of pronouns to be associated with a gender(s). 
		/// </summary>
		public enum Pronouns
		{
			NONE,
			HE_HIM_HIS,
			SHE_HER_HERS,
			THEY_THEM_THEIR
		}

		#endregion // Public Enums

		#region Public Functions

		/// <summary>
		/// Get a string of the gender label.
		/// </summary>
		/// <param name="gender"> The gender label to be converted into a string. </param>
		/// <param name="isAbbreviated"> Whether or not the gender label should be abbreviated. </param>
		/// <returns> The gender label as the gender label. </returns>
		public static string ToString ( Label gender, bool isAbbreviated = false )
		{
			// Check gender
			switch ( gender )
			{
			case Label.MALE:
				return isAbbreviated ? "M" : "Male";
			case Label.FEMALE:
				return isAbbreviated ? "F" : "Female";
			case Label.NON_BINARY:
				return isAbbreviated ? "NB" : "Non-Binary";
			case Label.NONE:
				return isAbbreviated ? "N/A" : "None";
			}

			// Return that no gender was found
			return "";
		}

		/// <summary>
		/// Get the third-person subjective pronoun (e.g. he, she, they).
		/// </summary>
		/// <param name="pronoun"> The associated set of pronouns. </param>
		/// <param name="isCapitalized"> Whether or not the string returned should be capitalized. </param>
		/// <returns> The third-person subjective pronoun. </returns>
		public static string GetSubjectivePronoun ( Pronouns pronoun, bool isCapitalized = true )
		{
			// Check pronoun
			switch ( pronoun )
			{
			case Pronouns.HE_HIM_HIS:
				return isCapitalized ? "He" : "he";
			case Pronouns.SHE_HER_HERS:
				return isCapitalized ? "She" : "she";
			case Pronouns.THEY_THEM_THEIR:
				return isCapitalized ? "They" : "they";
			}

			// Return that no pronoun was found
			return "";
		}

		/// <summary>
		/// Get the third-person objective pronoun (e.g. him, her, them).
		/// </summary>
		/// <param name="pronoun"> The associated set of pronouns. </param>
		/// <param name="isCapitalized"> Whether or not the string returned should be capitalized. </param>
		/// <returns> The third-person objective pronoun. </returns>
		public static string GetObjectivePronoun ( Pronouns pronoun, bool isCapitalized = true )
		{
			// Check pronoun
			switch ( pronoun )
			{
			case Pronouns.HE_HIM_HIS:
				return isCapitalized ? "Him" : "him";
			case Pronouns.SHE_HER_HERS:
				return isCapitalized ? "Her" : "her";
			case Pronouns.THEY_THEM_THEIR:
				return isCapitalized ? "Them" : "them";
			}

			// Return that no pronoun was found
			return "";
		}

		/// <summary>
		/// Get the third-person possessive pronoun (e.g. his, hers, their).
		/// </summary>
		/// <param name="pronoun"> The associated set of pronouns. </param>
		/// <param name="isCapitalized"> Whether or not the string returned should be capitalized. </param>
		/// <returns> The third-person possessive pronoun. </returns>
		public static string GetPossessivePronoun ( Pronouns pronoun, bool isCapitalized = true )
		{
			// Check pronoun
			switch ( pronoun )
			{
			case Pronouns.HE_HIM_HIS:
				return isCapitalized ? "His" : "his";
			case Pronouns.SHE_HER_HERS:
				return isCapitalized ? "Hers" : "hers";
			case Pronouns.THEY_THEM_THEIR:
				return isCapitalized ? "Their" : "their";
			}

			// Return that no pronoun was found
			return "";
		}

		#endregion // Public Functions
	}
}