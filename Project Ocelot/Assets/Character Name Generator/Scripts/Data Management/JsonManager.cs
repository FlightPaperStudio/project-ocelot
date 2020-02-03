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

namespace CNG
{
	/// <summary>
	/// A class for reading an array of elements from a JSON file without needing to create a custom wrapper class to hold the data.
	/// </summary>
	public static class JsonManager
	{
		#region Private Classes

		/// <summary>
		/// A class for wrapping a JSON string in order to create an array of elements.
		/// </summary>
		/// <typeparam name="T"> The class the JSON elements will be converted to. </typeparam>
		[System.Serializable]
		private class Wrapper<T>
		{
			public T [ ] Items;
		}

		#endregion // Private Classes

		#region Public Functions

		/// <summary>
		/// Loads a JSON file from the Resources folder.
		/// </summary>
		/// <param name="path"> The path to the file within Resources (File extension not needed). </param>
		/// <returns> The text of the JSON file. </returns>
		public static string GetJsonFromResources ( string path )
		{
			// Remove file extension if included
			if ( path.Contains ( ".json" ) )
				path = path.Replace ( ".json", "" );

			// Get text from a resources file
			TextAsset file = Resources.Load<TextAsset> ( path );
			
			// Return text from file
			return file.text;
		}

		/// <summary>
		/// Converts a JSON string into an array of <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"> The class the JSON elements will be converted to. </typeparam>
		/// <param name="json"> The JSON string. </param>
		/// <returns> An array of <typeparamref name="T"/>. </returns>
		public static T [ ] FromJson<T> ( string json )
		{
			// Wrap JSON string
			json = "{\"Items\":" + json + "}";

			// Create JSON wrapper
			Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> ( json );

			// Return items from JSON
			return wrapper.Items;
		}

		#endregion // Public Functions
	}
}
