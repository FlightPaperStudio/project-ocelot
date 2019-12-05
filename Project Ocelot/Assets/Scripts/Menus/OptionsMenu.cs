using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectOcelot.Menues
{
	public class OptionsMenu : Menu
	{
		#region UI Elements

		[SerializeField]
		private UI.CardButton [ ] buttons;

		[SerializeField]
		private LoadingScreen load;

		#endregion // UI Elements

		#region Menu Override Functions

		public override void CloseMenu ( bool openParent = true )
		{
			// Reset buttons
			for ( int i = 0; i < buttons.Length; i++ )
				buttons [ i ].ForceEnlarge ( false, false );

			// Close menu
			base.CloseMenu ( openParent );
		}

		#endregion // Menu Override Functions

		#region Public Functions

		/// <summary>
		/// Opens the credits scene.
		/// </summary>
		public void OpenCredits ( )
		{
			// Load the credits
			load.LoadScene ( Scenes.CREDITS );
		}

		#endregion // Public Functions
	}
}