using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour 
{
	// UI elements
	public GameObject screen;

	// Loading information
	private bool isLoading = false;

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
	public void LoadScene ( Scenes scene )
	{
		// Display loading screen if it has already
		if ( !isLoading )
			BeginLoad ( );

		// Get scene
		string s = "";
		switch ( scene )
		{
		case Scenes.Menus:
			s = "Menus";
			break;
		case Scenes.MatchSetup:
			s = "Match Setup";
			break;
		case Scenes.Classic:
			s = "Classic";
			break;
		case Scenes.Rumble:
			s = "Rumble";
			break;
		case Scenes.Credits:
			s = "Credits";
			break;
		}

		// Begin load
		StartCoroutine ( LoadingScene ( s ) );
	}

	/// <summary>
	/// Wait at the loading screen until the scene has loaded.
	/// </summary>
	private IEnumerator LoadingScene ( string scene )
	{
		// Begin loading scene
		AsyncOperation async = SceneManager.LoadSceneAsync ( scene );

		// Wait until scene has loaded
		while ( !async.isDone )
			yield return null;
	}
}

public enum Scenes
{
	Menus,
	MatchSetup,
	Classic,
	Rumble,
	Credits
}