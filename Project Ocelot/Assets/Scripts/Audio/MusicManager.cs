using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class MusicManager : MonoBehaviour 
{
	#region Private Classes

	[System.Serializable]
	private class MusicTrack
	{
		public AudioClip [ ] Tracks;

		public bool RandomizedTracks
		{
			get
			{
				return Tracks.Length > 1;
			}
		}
	}

	#endregion // Private Classes

	#region Singleton Pattern

	private static MusicManager instance;

	/// <summary>
	/// The singleton instance of Music Manager.
	/// </summary>
	public static MusicManager Instance
	{
		get
		{
			// Check for an instance of MusicManager
			if ( instance == null )
			{
				// Get an instance
				instance = GameObject.FindObjectOfType<MusicManager> ( );
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
	}

	#endregion // Singleton Pattern

	#region Music Data

	[SerializeField]
	private AudioSource audio;

	private MusicTrack currentTrack;
	private bool isPaused = false;

	private const float FADE_TIME = 0.5f;

	#endregion // Music Data

	#region Music Tracks

	[SerializeField]
	private MusicTrack mainMenuMusic;

	[SerializeField]
	private MusicTrack creditsMusic;

	[SerializeField]
	private MusicTrack matchSetupMusic;

	[SerializeField]
	private MusicTrack matchMusic;

	#endregion // Music Tracks

	#region MonoBehaviour Functions

	private void Update ( )
	{
		// Check if track has ended
		if ( !audio.isPlaying && !isPaused && currentTrack.RandomizedTracks )
		{
			// Randomize the next track
			Shuffle ( currentTrack );
		}
	}

	#endregion // MonoBehaviour Functions

	#region Public Functions

	/// <summary>
	/// Play the music track for the specified scene.
	/// </summary>
	/// <param name="scene"> The current scene. </param>
	public void Play ( Scenes scene )
	{
		// Check scene
		switch ( scene )
		{
		case Scenes.SPLASH_SCREEN:
			Play ( mainMenuMusic, false );
			break;
		case Scenes.MENUS:
			Play ( mainMenuMusic );
			break;
		case Scenes.CREDITS:
			Play ( creditsMusic );
			break;
		case Scenes.MATCH_SETUP:
			Play ( matchSetupMusic );
			break;
		case Scenes.CLASSIC:
		case Scenes.RUMBLE:
			Play ( matchMusic );
			break;
		}
	}

	/// <summary>
	/// Pauses the music.
	/// </summary>
	public void Pause ( )
	{
		// Pause the music
		isPaused = true;
		audio.Pause ( );
	}

	/// <summary>
	/// Resumes playing the music from being paused.
	/// </summary>
	public void Resume ( )
	{
		// Resume music
		isPaused = false;
		audio.UnPause ( );
	}

	/// <summary>
	/// Updates the music volume.
	/// </summary>
	public void AdjustVolume ( )
	{
		// Set if the music muted
		audio.mute = Settings.MuteVolume;

		// Set music volume to the setting
		audio.volume = Settings.MusicVolume;
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Fades out the current track, and plays a new set of tracks.
	/// </summary>
	/// <param name="track"> The music tracks to be played. </param>
	/// <param name="fadeOut"> Whether or not the current music track should be faded out as a transition. </param>
	private void Play ( MusicTrack track, bool fadeOut = true )
	{
		// Store track
		currentTrack = track;

		// Check for fade out
		if ( fadeOut )
		{
			// Fade out and play new track
			Sequence fade = DOTween.Sequence ( )
			.Append ( audio.DOFade ( 0, FADE_TIME ) )
			.AppendCallback ( ( ) =>
			{
				// Stop previous clip
				audio.Stop ( );

				// Reset volume
				AdjustVolume ( );

				// Check if the track is looped or shuffled
				if ( track.RandomizedTracks )
				{
					// Randomize the tracks
					Shuffle ( track );
				}
				else
				{
					// Loop track
					audio.loop = true;
					audio.clip = track.Tracks [ 0 ];
					audio.Play ( );
				}
			} )
			.Play ( );
		}
		else
		{
			// Play new track immediately
			audio.Stop ( );

			// Reset volume
			AdjustVolume ( );

			// Check if the track is looped or shuffled
			if ( track.RandomizedTracks )
			{
				// Randomize the tracks
				Shuffle ( track );
			}
			else
			{
				// Loop track
				audio.loop = true;
				audio.clip = track.Tracks [ 0 ];
				audio.Play ( );
			}
		}
	}

	/// <summary>
	/// Plays a randomly selected music track.
	/// </summary>
	/// <param name="track"> The music tracks to select from. </param>
	private void Shuffle ( MusicTrack track )
	{
		// Get all tracks that are not currently playing
		AudioClip [ ] tracks = track.Tracks.Where ( x => x != audio.clip ).ToArray ( );

		// Get random track
		int randomTrack = Random.Range ( 0, tracks.Length );

		// Play track
		audio.loop = false;
		audio.clip = tracks [ randomTrack ];
		audio.Play ( );
	}

	#endregion // Private Functions
}
