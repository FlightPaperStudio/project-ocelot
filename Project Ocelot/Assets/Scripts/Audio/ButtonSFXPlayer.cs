using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent ( typeof ( AudioSource ) )]
public class ButtonSFXPlayer : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
	#region SFX Clips

	[SerializeField]
	private AudioClip hoverSFX;

	[SerializeField]
	private AudioClip clickSFX;

	[SerializeField]
	private AudioClip globalClickSFX;

	#endregion // SFX Clips

	#region SFX Components

	private AudioSource audio;
	private Button button;

	#endregion // SFX Components

	#region MonoBehaviour Functions

	private void Awake ( )
	{
		#if UNITY_EDITOR

		// Check for audio source
		if ( audio == null )
		{
			// Get audio source component
			if ( gameObject.GetComponent<AudioSource> ( ) != null )
			{
				// Set audio source component
				audio = gameObject.GetComponent<AudioSource> ( );
			}
			else
			{
				// Add audio source component
				audio = gameObject.AddComponent<AudioSource> ( );
			}
		}

		// Set audio source settings
		audio.playOnAwake = false;
		audio.loop = false;

		// Check for button
		if ( button == null && gameObject.GetComponent<Button> ( ) != null )
		{
			// Get button component
			button = gameObject.GetComponent<Button> ( );
		}

		#endif
	}

	#endregion // MonoBehaviour Functions

	#region IPointerEnterHandler Functions

	public void OnPointerEnter ( PointerEventData eventData )
	{
		// Play sfx
		if ( button == null || button.interactable )
			PlayHoverSFX ( );
	}

	#endregion // IPointerEnterHandler Functions

	#region IPointerUpHandler Functions

	public void OnPointerDown ( PointerEventData eventData )
	{
		// Play sfx
		if ( clickSFX != null && ( button == null || button.interactable ) )
			PlayClickSFX ( );

		// Play global sfx
		if ( globalClickSFX != null && ( button == null || button.interactable ) )
			SFXManager.Instance.Play ( globalClickSFX, Settings.UIVolume );
	}

	#endregion // IPointerUpHandler Functions

	#region Public Functions

	/// <summary>
	/// Plays the SFX for when the curser hovers over this button.
	/// </summary>
	public void PlayHoverSFX ( )
	{
		// Check the mute setting and for sfx
		if ( !Settings.MuteVolume && hoverSFX != null )
		{
			// Play sfx
			audio.PlayOneShot ( hoverSFX, Settings.UIVolume );
		}
	}

	/// <summary>
	/// Plays the SFX for when the cursor clicks this button.
	/// </summary>
	public void PlayClickSFX ( )
	{
		// Check the mute setting and for sfx
		if ( !Settings.MuteVolume && clickSFX != null )
		{
			// Play sfx
			audio.PlayOneShot ( clickSFX, Settings.UIVolume );
		}
	}

	#endregion // Public Functions
}
