using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour 
{
	// Tracks if the settings have been loaded
	private static bool isSettingsLoaded = false;

	/// <summary>
	/// This setting determines whether or not the game is displayed in full screen or in a window.
	/// </summary>
	public static bool Display;
	private const string DISPLAY = "display";
	private const bool DISPLAY_DEFAULT = true;

	/// <summary>
	/// These settings determine the resolution.
	/// </summary>
	public static int ResolutionWidth;
	public static int ResolutionHeight;
	private const string RESOLUTION_WIDTH = "resolutionWidth";
	private const string RESOLUTION_HEIGHT = "resolutionHeight";

	/// <summary>
	/// This setting determines the graphical quality of assets rendered in game.
	/// </summary>
	public static int Quality;
	private const string QUALITY = "quality";
	private const int QUALITY_DEFAULT = 3;

	/// <summary>
	/// This setting determines whether or not V-Sync is active.
	/// </summary>
	public static int Vsync;
	private const string VSYNC = "vsync";
	private const int VSYNC_DEFAULT = 1;

	/// <summary>
	/// This setting determines the absolute volume for the entire game.
	/// </summary>
	public static float MasterVolume;
	private const string MASTER_VOLUME = "masterVolume";
	private const float MASTER_VOLUME_DEFAULT = 1.0f;

	/// <summary>
	/// This setting determines the volume of the music.
	/// </summary>
	public static float MusicVolume;
	private const string MUSIC_VOLUME = "musicVolume";
	private const float MUSIC_VOLUME_DEFAULT = 1.0f;

	/// <summary>
	/// This setting determines the volume of the sound effects.
	/// </summary>
	public static float SoundVolume;
	private const string SOUND_VOLUME = "soundVolume";
	private const float SOUND_VOLUME_DEFAULT = 1.0f;

	/// <summary>
	/// Loads the settings.
	/// </summary>
	public static void LoadSettings ( )
	{
		// Ensure that the settings are only loaded once
		if ( !isSettingsLoaded )
		{
			// Check for display setting
			if ( PlayerPrefs.HasKey ( DISPLAY ) )
			{
				// Load display setting
				Display = PlayerPrefsX.GetBool ( DISPLAY );
			}
			else
			{
				// Set default display setting
				Display = DISPLAY_DEFAULT;
				PlayerPrefsX.SetBool ( DISPLAY, Display );
			}

			// Check for resolution width setting
			if ( PlayerPrefs.HasKey ( RESOLUTION_WIDTH ) )
			{
				// Load resolution width
				ResolutionWidth = PlayerPrefs.GetInt ( RESOLUTION_WIDTH );
			}
			else
			{
				// Set default resolution width
				ResolutionWidth = Screen.currentResolution.width;
				PlayerPrefs.SetInt ( RESOLUTION_WIDTH, ResolutionWidth );
			}

			// Check for resolution height setting
			if ( PlayerPrefs.HasKey ( RESOLUTION_HEIGHT ) )
			{
				// Load resolution height
				ResolutionHeight = PlayerPrefs.GetInt ( RESOLUTION_HEIGHT );
			}
			else
			{
				// Set default resolution height
				ResolutionHeight = Screen.currentResolution.height;
				PlayerPrefs.SetInt ( RESOLUTION_HEIGHT, ResolutionHeight );
			}

			// Check for quality setting
			if ( PlayerPrefs.HasKey ( QUALITY ) )
			{
				// Load quality setting
				Quality = PlayerPrefs.GetInt ( QUALITY );
			}
			else
			{
				// Set default display setting
				Quality = QUALITY_DEFAULT;
				PlayerPrefs.SetInt ( QUALITY, Quality );
			}

			// Check for vsync setting
			if ( PlayerPrefs.HasKey ( VSYNC ) )
			{
				// Load vsync setting
				Vsync = PlayerPrefs.GetInt ( VSYNC );
			}
			else
			{
				// Set default vsync setting
				Vsync = VSYNC_DEFAULT;
				PlayerPrefs.SetInt ( VSYNC, Vsync );
			}

			// Check for master volume setting
			if ( PlayerPrefs.HasKey ( MASTER_VOLUME ) )
			{
				// Load master volume setting
				MasterVolume = PlayerPrefs.GetFloat ( MASTER_VOLUME );
			}
			else
			{
				// Set default master volume setting
				MasterVolume = MASTER_VOLUME_DEFAULT;
				PlayerPrefs.SetFloat ( MASTER_VOLUME, MasterVolume );
			}

			// Check for music volume setting
			if ( PlayerPrefs.HasKey ( MUSIC_VOLUME ) )
			{
				// Load music volume setting
				MusicVolume = PlayerPrefs.GetFloat ( MUSIC_VOLUME );
			}
			else
			{
				// Set default music volume setting
				MusicVolume = MUSIC_VOLUME_DEFAULT;
				PlayerPrefs.SetFloat ( MUSIC_VOLUME, MusicVolume );
			}

			// Check for sound volume setting
			if ( PlayerPrefs.HasKey ( SOUND_VOLUME ) )
			{
				// Load sound volume setting
				SoundVolume = PlayerPrefs.GetFloat ( SOUND_VOLUME );
			}
			else
			{
				// Set default sound volume setting
				SoundVolume = SOUND_VOLUME_DEFAULT;
				PlayerPrefs.SetFloat ( SOUND_VOLUME, SoundVolume );
			}

			// Set that the settings have been loaded
			isSettingsLoaded = true;
		}
	}

	/// <summary>
	/// Resets the video settings to their default values.
	/// </summary>
	public static void RestoreDefaultVideoSettings ( )
	{
		// Reset display
		Display = DISPLAY_DEFAULT;

		// Reset resolution
		ResolutionWidth = Screen.resolutions [ Screen.resolutions.Length - 1 ].width;
		ResolutionHeight = Screen.resolutions [ Screen.resolutions.Length - 1 ].height;

		// Reset quality
		Quality = QUALITY_DEFAULT;

		// Reset vsync
		Vsync = VSYNC_DEFAULT;
	}

	/// <summary>
	/// Saves the video settings.
	/// </summary>
	public static void SaveVideoSettings ( )
	{
		// Save display
		PlayerPrefsX.SetBool ( DISPLAY, Display );

		// Save resolution
		PlayerPrefs.SetInt ( RESOLUTION_WIDTH, ResolutionWidth );
		PlayerPrefs.SetInt ( RESOLUTION_HEIGHT, ResolutionHeight );

		// Save quality
		PlayerPrefs.SetInt ( QUALITY, Quality );

		// Save vsync
		PlayerPrefs.SetInt ( VSYNC, Vsync );
	}

	/// <summary>
	/// Resets the audio settings to their default values.
	/// </summary>
	public static void RestoreDefaultAudioSettings ( )
	{
		// Reset master volume
		MasterVolume = MASTER_VOLUME_DEFAULT;

		// Reset music volume
		MusicVolume = MUSIC_VOLUME_DEFAULT;

		// Reset sound volume
		SoundVolume = SOUND_VOLUME_DEFAULT;
	}

	/// <summary>
	/// Saves the audio settings.
	/// </summary>
	public static void SaveAudioSettings ( )
	{
		// Save master volume
		PlayerPrefs.SetFloat ( MASTER_VOLUME, MasterVolume );

		// Save music volume
		PlayerPrefs.SetFloat ( MUSIC_VOLUME, MusicVolume );

		// Save sound volume
		PlayerPrefs.SetFloat ( SOUND_VOLUME, SoundVolume );
	}

//-------------------------------------------------DELETE EVERYTHING BELOW THIS LINE--------------------------------------------------------------------
//
//	//Checks for the start of the game
//	private static bool GameStart = true;
//
//	/// <summary>
//	/// This setting determines the layout of the board.
//	/// </summary>
//	public static bool LayoutIsVertical;
//
//	/// <summary>
//	/// This setting determines the volume of the music.
//	/// </summary>
//	public static float MusicVolume;
//
//	/// <summary>
//	/// This setting determines the volume of the sound effects.
//	/// </summary>
//	public static float SoundVolume;
//
//	/// <summary>
//	/// This setting determines the start time for each player's game clock.
//	/// </summary>
//	public static int GameClock;
//
//	/// <summary>
//	/// This save data determines which player's turn it is.
//	/// </summary>
//	public static bool SaveDataIsP1Turn;
//
//	/// <summary>
//	/// This save data determines player 1's abilities. X is the ability ID, Y is if it is active, and Z is the piece it's attached to.
//	/// </summary>
//	public static Vector3 [ ] SaveDataP1Abilities = new Vector3 [ 3 ];
//
//	/// <summary>
//	/// This save data determines player 2's abilities. X is the ability ID, Y is if it is active, and Z is the piece it's attached to.
//	/// </summary>
//	public static Vector3 [ ] SaveDataP2Abilities = new Vector3 [ 3 ];
//
//	/// <summary>
//	/// This save data determines the location of all of the players' pieces on the board. X is the player, Y is the piece color, and Z is the tile ID.
//	/// </summary>
//	public static Vector3 [ ] SaveDataPieces = new Vector3 [ 12 ];
//
//	/// <summary>
//	/// This save data determines the pieces player 1 sacrificed.
//	/// </summary>
//	public static int SaveDataP1Sacrifice;
//
//	/// <summary>
//	/// This save data determines the pieces player 2 sacrificed.
//	/// </summary>
//	public static int SaveDataP2Sacrifice;
//
//	/// <summary>
//	/// This save data determines the game clock setting of the save game.
//	/// </summary>
//	public static int SaveDataGameClock;
//
//	/// <summary>
//	/// This save data determines the time remaining on player 1's game clock.
//	/// </summary>
//	public static float SaveDataP1GameClock;
//
//	/// <summary>
//	/// This save data determines the time remaining on player 2's game clock.
//	/// </summary>
//	public static float SaveDataP2GameClock;
//
//	/// <summary>
//	/// Initialize the settings at the start of the game.
//	/// </summary>
//	private void Awake ( )
//	{
//		//Check for game start up
//		if ( GameStart )
//		{
//			//Check for board layout
//			if ( PlayerPrefs.HasKey ( "boardLayout" ) )
//			{
//				//Load layout setting
//				LayoutIsVertical = PlayerPrefsX.GetBool ( "boardLayout" );
//			}
//			else
//			{
//				//Set layout setting
//				LayoutIsVertical = false;
//				PlayerPrefsX.SetBool ( "boardLayout", LayoutIsVertical );
//			}
//
//			//Check for music volume
//			if ( PlayerPrefs.HasKey ( "musicVolume" ) )
//			{
//				//Load music volume
//				MusicVolume = PlayerPrefs.GetFloat ( "musicVolume" );
//			}
//			else
//			{
//				//Set music volume
//				MusicVolume = 1;
//				PlayerPrefs.SetFloat ( "musicVolume", MusicVolume );
//			}
//
//			//Set music volume
//			MusicManager.instance.UpdateMusicVolume ( );
//
//			//Check for sound volume
//			if ( PlayerPrefs.HasKey ( "soundVolume" ) )
//			{
//				//Load sound volume
//				SoundVolume = PlayerPrefs.GetFloat ( "soundVolume" );
//			}
//			else
//			{
//				//Set sound volume
//				SoundVolume = 1;
//				PlayerPrefs.SetFloat ( "soundVolume", SoundVolume );
//			}
//
//			//Set SFX volume
//			SFXManager.instance.UpdateSFXVolume ( );
//
//			//Check for game clock setting
//			if ( PlayerPrefs.HasKey ( "gameClock" ) )
//			{
//				//Load game clock setting
//				GameClock = PlayerPrefs.GetInt ( "gameClock" );
//			}
//			else
//			{
//				//Set game clock setting
//				GameClock = 0;
//				PlayerPrefs.SetInt ( "gameClock", GameClock );
//			}
//
//			//Check for player's turn save data
//			if ( PlayerPrefs.HasKey ( "playerTurn" ) )
//			{
//				//Load save data
//				SaveDataIsP1Turn = PlayerPrefsX.GetBool ( "playerTurn" );
//			}
//			else
//			{
//				//Set empty save data
//				SaveDataIsP1Turn = true;
//				PlayerPrefsX.SetBool ( "playerTurn", SaveDataIsP1Turn );
//			}
//
//			//Check for player 1 ability save data
//			if ( PlayerPrefs.HasKey ( "player1Abilities" ) )
//			{
//				//Load save data
//				SaveDataP1Abilities = PlayerPrefsX.GetVector3Array ( "player1Abilities" );
//			}
//			else
//			{
//				//Set empty save data
//				for ( int i = 0; i < SaveDataP1Abilities.Length; i++ )
//					SaveDataP1Abilities [ i ] = Vector3.zero;
//				PlayerPrefsX.SetVector3Array ( "player1Abilities", SaveDataP1Abilities );
//			}
//
//			//Check for player 2 ability save data
//			if ( PlayerPrefs.HasKey ( "player2Abilities" ) )
//			{
//				//Load save data
//				SaveDataP2Abilities = PlayerPrefsX.GetVector3Array ( "player2Abilities" );
//			}
//			else
//			{
//				//Set empty save data
//				for ( int i = 0; i < SaveDataP2Abilities.Length; i++ )
//					SaveDataP2Abilities [ i ] = Vector3.zero;
//				PlayerPrefsX.SetVector3Array ( "player2Abilities", SaveDataP2Abilities );
//			}
//
//			//Check for piece position save data
//			if ( PlayerPrefs.HasKey ( "pieces" ) )
//			{
//				//Load save data
//				SaveDataPieces = PlayerPrefsX.GetVector3Array ( "pieces" );
//			}
//			else
//			{
//				//Set empty save data
//				for ( int i = 0; i < SaveDataPieces.Length; i++ )
//					SaveDataPieces [ i ] = Vector3.zero;
//				PlayerPrefsX.SetVector3Array ( "pieces", SaveDataPieces );
//			}
//
//			//Check for player 1's sacrificed pieces save data
//			if ( PlayerPrefs.HasKey ( "player1Sacrifice" ) )
//			{
//				//Load save data
//				SaveDataP1Sacrifice = PlayerPrefs.GetInt ( "player1Sacrifice", 0 );
//			}
//			else
//			{
//				//Set empty save data
//				SaveDataP1Sacrifice = 0;
//				PlayerPrefs.SetInt ( "player1Sacrifice", SaveDataP1Sacrifice );
//			}
//
//			//Check for player 2's sacrificed pieces save data
//			if ( PlayerPrefs.HasKey ( "player2Sacrifice" ) )
//			{
//				//Load save data
//				SaveDataP2Sacrifice = PlayerPrefs.GetInt ( "player2Sacrifice", 0 );
//			}
//			else
//			{
//				//Set empty save data
//				SaveDataP2Sacrifice = 0;
//				PlayerPrefs.SetInt ( "player2Sacrifice", SaveDataP2Sacrifice );
//			}
//
//			//Check for game clock setting save data
//			if ( PlayerPrefs.HasKey ( "saveGameClock" ) )
//			{
//				//Load save data
//				SaveDataGameClock = PlayerPrefs.GetInt ( "saveGameClock" );
//			}
//			else
//			{
//				//Set empty save data
//				SaveDataGameClock = 0;
//				PlayerPrefs.SetInt ( "saveGameClock", SaveDataGameClock );
//			}
//
//			//Check of player 1 game clock save data
//			if ( PlayerPrefs.HasKey ( "player1GameClock" ) )
//			{
//				//Load save data
//				SaveDataP1GameClock = PlayerPrefs.GetFloat ( "player1GameClock" );
//			}
//			else
//			{
//				//Set empty save data
//				SaveDataP1GameClock = 0;
//				PlayerPrefs.SetFloat ( "player1GameClock", SaveDataP1GameClock );
//			}
//
//			//Check of player 2 game clock save data
//			if ( PlayerPrefs.HasKey ( "player2GameClock" ) )
//			{
//				//Load save data
//				SaveDataP2GameClock = PlayerPrefs.GetFloat ( "player2GameClock" );
//			}
//			else
//			{
//				//Set empty save data
//				SaveDataP2GameClock = 0;
//				PlayerPrefs.SetFloat ( "player2GameClock", SaveDataP2GameClock );
//			}
//		}
//	}
}
