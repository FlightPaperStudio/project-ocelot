using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Credits : MonoBehaviour 
{
	// UI elements
	public RectTransform scroll;
	public RectTransform mask;
	public Button exit;

	// Menu information
	public LoadingScreen load;
	private bool isButtonDisplayed = false;

	/// <summary>
	/// Starts playing the credits.
	/// </summary>
	private void Start ( ) 
	{
		// Set start position
		scroll.anchoredPosition = new Vector2 ( scroll.anchoredPosition.x, scroll.anchoredPosition.y - mask.rect.height );

		// Hide main menu button
		exit.gameObject.SetActive ( false );

		// Play animation
		StartCoroutine ( CreditAnimation ( ) );
	}

	/// <summary>
	/// Waits one frame to begin the animation because Unity is dumb.
	/// The proper rect height has to update first before the animation can play.
	/// </summary>
	private IEnumerator CreditAnimation ( )
	{
		// Wait one frame
		yield return 0;

		// Play credits animation
		Sequence s = DOTween.Sequence ( )
			.AppendInterval ( 0.3f )
			.Append ( scroll.DOAnchorPos ( new Vector2 ( scroll.anchoredPosition.x, scroll.anchoredPosition.y + mask.rect.height + scroll.rect.height ), 45.0f ).SetEase ( Ease.Linear ) )
			.AppendInterval ( 0.3f )
			.OnComplete ( () =>
				{
					// Load the main menu
					load.LoadScene ( Scenes.Menus );
				} )
			.Play ( );
	}

	/// <summary>
	/// Checks for input to exit the credits.
	/// </summary>
	private void Update ( )
	{
		// Check for input
		if ( Input.anyKey && !isButtonDisplayed )
		{
			// Display button
			isButtonDisplayed = true;
			exit.gameObject.SetActive ( true );
		}
	}

	/// <summary>
	/// Exits the credits.
	/// </summary>
	public void MainMenu ( )
	{
		// Load the main menu
		load.LoadScene ( Scenes.Menus );
	}
}
