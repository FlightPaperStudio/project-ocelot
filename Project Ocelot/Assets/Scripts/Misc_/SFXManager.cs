using UnityEngine;
using System.Collections;

public class SFXManager : MonoBehaviour 
{
	//Audio sources
	public AudioSource sfx;
	
	//Audio tracks
	public AudioClip click;
	public AudioClip selectAbility;
	public AudioClip acceptAbility;
	
	//SFX singleton
	private static SFXManager _instance;
	public static SFXManager instance
	{
		get
		{
			//Check for existing singleton
			if ( _instance == null )
			{
				//Set initial singleton
				_instance = GameObject.FindObjectOfType < SFXManager > ( );
				
				//Make the singleton persistent
				DontDestroyOnLoad ( _instance.gameObject );
			}
			
			//Set singleton
			return _instance;
		}
	}
	
	/// <summary>
	/// Makes sure that only one SFX Manager exists in each scene.
	/// </summary>
	private void Awake ( )
	{
		//Check for existing SFX manager
		if ( _instance == null )
		{
			//Set the initial SFX manager
			_instance = this;
			DontDestroyOnLoad ( this );
		}
		else
		{
			//Delete duplicate SFX managers
			if ( this != _instance )
				Destroy ( this.gameObject );
		}
	}

	/// <summary>
	/// Plays the default click sound effect.
	/// </summary>
	public void Click ( )
	{
		//Play SFX
		sfx.clip = click;
		sfx.Play ( );
	}

	/// <summary>
	/// Plays the sound effect for selecting an ability.
	/// </summary>
	public void SelectAbility ( )
	{
		//Play SFX
		sfx.clip = selectAbility;
		sfx.Play ( );
	}

	/// <summary>
	/// Plays the sound effect for the player accepting their selected abilities.
	/// </summary>
	public void AcceptAbilities ( )
	{
		//Play SFX
		sfx.clip = acceptAbility;
		sfx.Play ( );
	}

	/// <summary>
	/// Updates the SFX volume.
	/// </summary>
	public void UpdateSFXVolume ( )
	{
		//Check current audio source playing
		sfx.volume = Settings.SoundVolume;
	}
}