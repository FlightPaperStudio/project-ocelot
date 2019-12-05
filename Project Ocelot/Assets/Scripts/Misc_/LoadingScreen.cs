using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour 
{
	#region UI Elements

	[SerializeField]
	private GameObject screen;

	[SerializeField]
	private Slider progress;

	[SerializeField]
	private TextMeshProUGUI prompt;

	#endregion // UI Elements

	#region Loading Data
	
	private bool isLoading = false;

	private const float LOAD_READY = 0.9f;
	private const string LOAD_TEXT = "Loading";
	private const float TEXT_TIMING = 0.25f;

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

		// Start animation
		StartCoroutine ( AnimateText ( ) );

		// Reset progress bar
		progress.value = 0;

		// Wait until scene has loaded
		while ( !async.isDone )
		{
			// Animate progress bar
			progress.value = async.progress / LOAD_READY;

			// Wait until next frame
			yield return null;
		}

		// Mark the completion of the load
		isLoading = false;

		// Hide loading screen
		screen.SetActive ( false );
	}

	/// <summary>
	/// Plays the animation for the loading text.
	/// </summary>
	/// <returns> The IEnumerator for the coroutine. </returns>
	private IEnumerator AnimateText ( )
	{
		// Store timing data
		float timer = 0f;
		int counter = 0;

		// Reset prompt
		prompt.text = LOAD_TEXT;

		// Animate text
		while ( isLoading )
		{
			// Check for animate frame
			if ( timer > TEXT_TIMING )
			{
				// Increment counter
				counter++;

				// Check for wrap around
				if ( counter >= 4 )
					counter = 0;

				// Display text
				switch ( counter )
				{
				case 0:
					prompt.text = LOAD_TEXT;
					break;
				case 1:
					prompt.text = LOAD_TEXT + ".";
					break;
				case 2:
					prompt.text = LOAD_TEXT + "..";
					break;
				case 3:
					prompt.text = LOAD_TEXT + "...";
					break;
				}

				// Reset timer
				timer = 0f;
			}

			// Wait until next frame
			yield return null;

			// Increment timer
			timer += Time.deltaTime;
		}
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