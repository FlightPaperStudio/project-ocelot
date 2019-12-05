using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectOcelot.Menues
{
	public class MatchTypeMenu : Menu
	{
		#region UI Elements

		[SerializeField]
		private UI.CardButton [ ] buttons;

		[SerializeField]
		private PlayerMenu playerMenu;

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
		/// Begins a Classic Match
		/// </summary>
		public void LoadClassicMatch ( )
		{
			// Set the match settings
			Match.MatchSettings.SetMatchSettings ( Match.MatchType.Classic );

			// Open player menu
			playerMenu.OpenMenu ( );
		}

		/// <summary>
		/// Begins a Classic Match
		/// </summary>
		public void LoadRumbleMatch ( )
		{
			// Set the match settings
			Match.MatchSettings.SetMatchSettings ( Match.MatchType.Rumble );

			// Open player menu
			playerMenu.OpenMenu ( );
		}

		#endregion // Public Functions
	}
}