using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SplashPrompt : MonoBehaviour 
{
	// UI elements
	public GameObject container;
	public TextMeshProUGUI prompt;

	// Animation information
	private bool closeOnComplete = true;
	private Sequence slide;
	private Sequence shake;

	private void Awake ( )
	{
		// Create the slide animation
		slide = DOTween.Sequence ( )
			.Append ( prompt.rectTransform.DOAnchorPos ( new Vector2 (  150, 0 ), 0.75f ).SetEase ( Ease.OutExpo ).From ( ) )
			.Append ( prompt.rectTransform.DOAnchorPos ( new Vector2 ( -150, 0 ), 0.75f ).SetEase ( Ease.InExpo ) )
			.OnComplete ( () =>
			{
				// Reset prompt position
				prompt.rectTransform.anchoredPosition = Vector2.zero;

				// Hide prompt
				if ( closeOnComplete )
					container.SetActive ( false );
			} )
			.SetAutoKill ( false )
			.Pause ( );

		// Create the shake animation
		shake = DOTween.Sequence ( )
			.Append ( prompt.rectTransform.DOShakeAnchorPos ( 1.0f, 10 ).SetEase ( Ease.OutBack ) )
			.AppendInterval ( 0.1f )
			.OnComplete ( () => 
			{
				// Reset prompt position
				prompt.rectTransform.anchoredPosition = Vector2.zero;

				// Hide prompt
				if ( closeOnComplete )
					container.SetActive ( false );
			} )
			.SetAutoKill ( false )
			.Pause ( );
	}


	/// <summary>
	/// Plays the slide animation.
	/// Rich text is used, so color information can be encoded into the text, however the color parameter is still the base text color.
	/// </summary>
	public Sequence Slide ( string text, Color32 color, bool close )
	{
		// Display the prompt
		container.SetActive ( true );

		// Set prompt
		prompt.text = text;
		prompt.color = color;

		// Determine if the prompt should close on completion
		closeOnComplete = close;

		// Play the slide animation
		if ( slide.IsComplete ( ) )
			slide.Restart ( );
		else
			slide.Play ( );

		// Return the animation so that the coroutines can wait
		return slide;
	}

	/// <summary>
	/// Plays the shake animation.
	/// Rich text is used, so color information can be encoded into the text, however the color parameter is still the base text color.
	/// </summary>
	public Sequence Shake ( string text, Color32 color, bool close )
	{
		// Display the prompt
		container.SetActive ( true );

		// Set prompt
		prompt.text = text;
		prompt.color = color;

		// Determine if the prompt should close on completion
		closeOnComplete = close;

		// Play the slide animation
		if ( shake.IsComplete ( ) )
			shake.Restart ( );
		else
			shake.Play ( );

		// Return the animation so that the coroutines can wait
		return shake;
	}
}
