using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MusicManager : MonoBehaviour 
{
	//Audio sources
	public AudioSource music;

	//Audio tracks
	public AudioClip titleScreen;
	public AudioClip howToPlay;
	public AudioClip selectAbilities;
	public AudioClip [ ] gameplayTracks;
	public AudioClip results;

	//Audio manager
	private const float FADE_TIME = 1.4f;

	//Music singleton
	private static MusicManager _instance;
	public static MusicManager instance
	{
		get
		{
			//Check for existing singleton
			if ( _instance == null )
			{
				//Set initial singleton
				_instance = GameObject.FindObjectOfType < MusicManager > ( );

				//Make the singleton persistent
				DontDestroyOnLoad ( _instance.gameObject );
			}

			//Set singleton
			return _instance;
		}
	}

	/// <summary>
	/// Makes sure that only one Music Manager exists in each scene.
	/// </summary>
	private void Awake ( )
	{
		//Check for existing music manager
		if ( _instance == null )
		{
			//Set the initial music manager
			_instance = this;
			DontDestroyOnLoad ( this );
		}
		else
		{
			//Delete duplicate music managers
			if ( this != _instance )
				Destroy ( this.gameObject );
		}
	}

	/// <summary>
	/// Changes the music to a new track by crossfading.
	/// </summary>
	public void ChangeMusic ( AudioContext destination )
	{
		//Store new track
		AudioClip track;

		//Check destination
		switch ( destination )
		{
			case AudioContext.MainMenu:
				track = titleScreen;
				break;
			case AudioContext.HowToPlay:
				track = howToPlay;
				break;
			case AudioContext.AbilitySelection:
				track = selectAbilities;
				break;
			case AudioContext.Gameplay:
				track = howToPlay;//gameplayTracks [ Random.Range ( 0, gameplayTracks.Length ) ];
				break;
			case AudioContext.Results:
				track = results;
				break;
			default:
				track = titleScreen;
				break;
		}

		//Fade music in and out
		Sequence fade = DOTween.Sequence ( )
			.Append ( GetComponent<AudioSource>().DOFade ( 0, FADE_TIME ) )
			.AppendCallback ( () => 
			{ 
				music.Stop ( );
				music.clip = track; 
				music.Play ( );
			} )
			.Append ( GetComponent<AudioSource>().DOFade ( Settings.MusicVolume, FADE_TIME ) )
			.SetRecyclable ( )
			.Play ( );
	}

	/// <summary>
	/// Updates the music volume.
	/// </summary>
	public void UpdateMusicVolume ( )
	{
		//Check current audio source playing
		music.volume = Settings.MusicVolume;
	}
}

public enum AudioContext
{
	MainMenu,
	HowToPlay,
	AbilitySelection,
	Gameplay,
	Results
};
