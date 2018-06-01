using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour 
{
	#region UI Elements

	[SerializeField]
	private GameObject screen;

	#endregion // UI Elements

	#region Loading Data
	
	private bool isLoading = false;

	#endregion // Loading Data

	#region Public Functions

	/// <summary>
	/// Begin loading a new scene by showing the loading screen.
	/// Use this before loading or storing data that is done before the scene starts loading.
	/// </summary>
	public void BeginLoad ( )
	{
		// Display loading screen
		screen.SetActive ( true );

		// Store that loading has begun
		isLoading = true;
	}

	/// <summary>
	/// Loads the next scene.
	/// </summary>
	/// <param name="scene"> The scene being loaded. </param>
	public void LoadScene ( Scenes scene )
	{
		// Display loading screen if it has already
		if ( !isLoading )
			BeginLoad ( );

		// Play the music for the scene
		if ( SceneManager.GetActiveScene ( ).name != "Splash Screen" )
			MusicManager.Instance.Play ( scene );

		// Get scene
		string s = "";
		switch ( scene )
		{
		case Scenes.MENUS:
			s = "Menus";
			break;
		case Scenes.MATCH_SETUP:
			s = "Match Setup";
			break;
		case Scenes.CLASSIC:
			s = "Classic";
			break;
		case Scenes.RUMBLE:
			s = "Rumble";
			break;
		case Scenes.CREDITS:
			s = "Credits";
			break;
		}

		// Begin load
		StartCoroutine ( LoadingScene ( s ) );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Wait at the loading screen until the scene has loaded.
	/// </summary>
	/// <param name="scene"> The name of the scene being loaded. </param>
	/// <returns> When the scene is completed loading. </returns>
	private IEnumerator LoadingScene ( string scene )
	{
		// Begin loading scene
		AsyncOperation async = SceneManager.LoadSceneAsync ( scene );

		// Wait until scene has loaded
		while ( !async.isDone )
			yield return null;
	}

	#endregion // Private Functions
}

public enum Scenes
{
	SPLASH_SCREEN,
	MENUS,
	MATCH_SETUP,
	CLASSIC,
	RUMBLE,
	CREDITS
}