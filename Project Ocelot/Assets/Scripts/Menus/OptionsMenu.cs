using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectOcelot.Menues
{
	public class OptionsMenu : Menu
	{
		// UI elements
		public RectTransform [ ] buttons;
		public GameObject [ ] outlines;

		// Menu information
		public LoadingScreen load;

		/// <summary>
		/// Opens the menu.
		/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
		/// </summary>
		public override void OpenMenu ( bool closeParent = true )
		{
			// Open the menu
			base.OpenMenu ( closeParent );

			// Reset each button
			for ( int i = 0; i < buttons.Length; i++ )
				MouseExit ( i );
		}

		/// <summary>
		/// Highlights the game mode button.
		/// </summary>
		public void MouseEnter ( int index )
		{
			// Increase the size of the button
			buttons [ index ].offsetMax = new Vector2 ( 5f, 5f );
			buttons [ index ].offsetMin = new Vector2 ( -5f, -5f );

			// Display outline
			outlines [ index ].SetActive ( true );
		}

		/// <summary>
		/// Unhighlights the game mode button.
		/// </summary>
		public void MouseExit ( int index )
		{
			// Decrease the size of the button
			buttons [ index ].offsetMax = Vector2.zero;
			buttons [ index ].offsetMin = Vector2.zero;

			// Hide outline
			outlines [ index ].SetActive ( false );
		}

		/// <summary>
		/// Opens the credits scene.
		/// </summary>
		public void OpenCredits ( )
		{
			// Load the credits
			load.LoadScene ( Scenes.CREDITS );
		}
	}
}