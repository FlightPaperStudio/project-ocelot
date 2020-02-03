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
	/// A class for storing data for a character.
	/// </summary>
	public class Character
	{
		#region Character Data

		/// <summary>
		/// The name prefix for the character.
		/// </summary>
		public string Prefix;

		/// <summary>
		/// The given name for the character.
		/// If the character has multiple given names, the names will be listed one after the other (e.g. Ashley Marie).
		/// </summary>
		public string GivenName;

		/// <summary>
		/// The family name for the character.
		/// If the character has multiple family names, the names will be listed one after the other (e.g. Johnson Green).
		/// </summary>
		public string FamilyName;

		/// <summary>
		/// The name suffix for the character.
		/// </summary>
		public string Suffix;

		/// <summary>
		/// The nickname for the character.
		/// </summary>
		public string Nickname;

		/// <summary>
		/// The gender for the character.
		/// </summary>
		public Gender.Label Gender;

		/// <summary>
		/// The preferred pronouns for the character.
		/// </summary>
		public Gender.Pronouns Pronouns;

		/// <summary>
		/// The origin for the character.
		/// All names for this character will be assigned based on this origin to some degree.
		/// </summary>
		public Origin Origin;

		/// <summary>
		/// The character's full name constructed in the Western naming order (e.g. Prefix + Given + Family + Suffix).
		/// </summary>
		public string WesternNameOrder
		{
			get
			{
				// Construct name
				string name = "";

				// Add prefix
				name += Prefix;

				// Add given name
				if ( name != "" )
					name += " ";
				name += GivenName;

				// Add family name
				if ( name != "" )
					name += " ";
				name += FamilyName;

				// Add suffix
				if ( Suffix != "" && Suffix [ 0 ] != ',' )
					name += " ";
				name += Suffix;

				// Return name order
				return name;
			}
		}

		/// <summary>
		/// The character's full name constructed in the Eastern naming order (e.g. Prefix + Family + Given + Suffix).
		/// </summary>
		public string EasternNameOrder
		{
			get
			{
				// Construct name
				string name = "";

				// Add prefix
				name += Prefix;

				// Add family name
				if ( name != "" )
					name += " ";
				name += FamilyName;

				// Add given name
				if ( name != "" )
					name += " ";
				name += GivenName;

				// Add suffix
				if ( Suffix != "" && Suffix [ 0 ] != ',' )
					name += " ";
				name += Suffix;

				// Return name order
				return name;
			}
		}

		/// <summary>
		/// The character's nickname placed in quotes (e.g. "Tiger").
		/// </summary>
		public string QuotedNickname
		{
			get
			{
				return "\"" + Nickname + "\"";
			}
		}

		#endregion // Character Data
	}
}