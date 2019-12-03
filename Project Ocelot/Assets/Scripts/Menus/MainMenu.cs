using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using CNG;

namespace ProjectOcelot.Menues
{
	public class MainMenu : Menu
	{
		#region UI Elements

		[SerializeField]
		private TextMeshProUGUI titleText;

		#endregion // UI Elements

		#region Menu Data

		[SerializeField]
		private Menu [ ] menus;

		[SerializeField]
		private PopUpMenu popUp;

		private static bool hasStartUp = false;

		#endregion // Menu Data

		#region MonoBehaviour Functions

		private void Awake ( )
		{
			// Load start up data at the beginning of the game.
			if ( !hasStartUp )
				StartUp ( );

			// Display title and version number
			titleText.text = "Project Ocelot\n<size=45%>" + Application.version;

			// Name generator test
			//for ( int i = 0; i < 50; i++ )
			//	Debug.Log ( "Name: " + NameGenerator.CreateName ( ) + "      Nickname: " + NameGenerator.CreateNickname ( ) );
		}

		private void Update ( )
		{
			// Check for the escape button being pressed
			if ( Input.GetKeyDown ( KeyCode.Escape ) )
			{
				// Find the current open menu and close it
				foreach ( Menu m in menus )
				{
					if ( m.IsOpen )
					{
						// Close the menu
						m.CloseMenu ( );
						break;
					}
				}
			}
		}

		#endregion // MonoBehaviour Functions

		#region Public Functions 

		/// <summary>
		/// Exits the game.
		/// </summary>
		public void ExitGame ( )
		{
			// Prompt user
			popUp.SetConfirmationPopUp ( "Are you sure?", QuitToDesktop, null );
			popUp.OpenMenu ( false );
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Loads all of the necessary start up data.
		/// </summary>
		private void StartUp ( )
		{
			// Load special ability data
			TextAsset heroes = Resources.Load ( "HeroList" ) as TextAsset;
			HeroInfo.SetList ( heroes.text );

			// Load debate data
			TextAsset debateDataJSON = Resources.Load ( "DebateGeneratorList" ) as TextAsset;
			Match.Setup.DebateGenerator.InitializeJSONData ( debateDataJSON.text );

			// Load name generator data
			NameData.LoadNameData ( );
			//TextAsset nameDataJSON = Resources.Load ( "NameGeneratorList" ) as TextAsset;
			//NameGenerator.Init ( nameDataJSON.text );

			// Mark that start up has finished
			hasStartUp = true;
		}

		/// <summary>
		/// Quits to the desktop.
		/// </summary>
		private void QuitToDesktop ( )
		{
			// Quit the game
			Application.Quit ( );
		}

		#endregion // Private Functions
	}
}