using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour 
{
	#region Singleton Pattern

	private static SFXManager instance;

	/// <summary>
	/// The singleton instance of Music Manager.
	/// </summary>
	public static SFXManager Instance
	{
		get
		{
			// Check for an instance of MusicManager
			if ( instance == null )
			{
				// Get an instance
				instance = GameObject.FindObjectOfType<SFXManager> ( );
				DontDestroyOnLoad ( instance.gameObject );
			}

			// Return the instance
			return instance;
		}
	}

	private void Awake ( )
	{
		// Check for an instance of MusicManager
		if ( instance == null )
		{
			// Assign this as the instance
			instance = this;
			DontDestroyOnLoad ( this );
		}
		else
		{
			// Destroy other instances
			if ( this != instance )
				Destroy ( this.gameObject );
		}

		// Clear scene sfx players
		sceneSFXPlayers.Clear ( );
	}

	#endregion // Singleton Pattern

	#region Audio Components

	[SerializeField]
	private AudioSource audio;

	private List<AudioSource> sceneSFXPlayers = new List<AudioSource> ( );

	#endregion // Audio Components

	#region Public Functions

	/// <summary>
	/// Plays a SFX from the global SFX Manager.
	/// </summary>
	/// <param name="sfx"> The sound to be played. </param>
	/// <param name="volume"> The volume setting for the SFX. </param>
	public void Play ( AudioClip sfx, float volume )
	{
		// Check if sound is muted
		if ( !Settings.MuteVolume && sfx != null )
		{
			// Play sfx
			audio.PlayOneShot ( sfx, volume );
		}
	}

	/// <summary>
	/// Adds a SFX player to the list of players in the scene to update Settings.
	/// </summary>
	/// <param name="sfxPlayer"></param>
	public void AddScenePlayer ( AudioSource sfxPlayer )
	{
		// Add player to list
		sceneSFXPlayers.Add ( sfxPlayer );

		// Set player settings
		sfxPlayer.mute = Settings.MuteVolume;
		sfxPlayer.volume = Settings.SoundVolume;
	}

	/// <summary>
	/// Updates all of the SFX players in the scene.
	/// Use this function when updating the audio settings. 
	/// </summary>
	public void UpdateSound ( )
	{
		// Update each player in the scene
		for ( int i = 0; i < sceneSFXPlayers.Count; i++ )
		{
			// Set player settings
			sceneSFXPlayers [ i ].mute = Settings.MuteVolume;
			sceneSFXPlayers [ i ].volume = Settings.SoundVolume;
		}
	}

	#endregion // Public Functions
}