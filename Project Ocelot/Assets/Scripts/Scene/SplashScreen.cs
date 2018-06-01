using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SplashScreen : MonoBehaviour 
{
	#region Splash Screen Data

	[SerializeField]
	private SpriteRenderer logo;

	[SerializeField]
	private LoadingScreen load;

	#endregion // Splash Screen Data

	#region MonoBehaviour Functions
	
	/// <summary>
	/// Loads the settings at the start of the game.
	/// </summary>
	private void Awake ( )
	{
		// Load the settings
		Settings.LoadSettings ( );

		// Set display and resolution
		Screen.SetResolution ( Settings.ResolutionWidth, Settings.ResolutionHeight, Settings.Display );

		// Set quality
		QualitySettings.SetQualityLevel ( Settings.Quality );

		// Set vsync
		QualitySettings.vSyncCount = Settings.Vsync;

		// Set music volume
		MusicManager.Instance.AdjustVolume ( );
		MusicManager.Instance.Play ( Scenes.SPLASH_SCREEN );
	}

	/// <summary>
	/// Plays the splash screen animations.
	/// </summary>
	private void Start ( )
	{
		// Play logo animation
		Sequence s = DOTween.Sequence ( )
			.AppendInterval ( 0.5f )
			.Append ( logo.DOFade ( 0, 1.5f ).From ( ) )
			.AppendInterval ( 2.0f )
			.OnComplete ( () =>
			{
				// Load start menu
				load.LoadScene ( Scenes.MENUS );
			} )
			.Play ( );
	}

	#endregion // MonoBehaviour Functions
}
