using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class StartGameMenu : MonoBehaviour 
{
//	//Menu game objects
//	public RectTransform sideBar;
//	public Button [ ] menuButtons;
//	public CanvasGroup [ ] menuPanels;
//
//	//Menu information
//	private Dictionary < Button, CanvasGroup > menuInfo = new Dictionary < Button, CanvasGroup > ( );
//
//	//New game menu game objects
//	public Text layoutPrompt;
//	public Text gameClockPrompt;
//
//	//Load game menu game objects
//	public GameObject saveData;
//	[ Serializable ]
//	public class playerUI
//	{
//		public Text [ ] abilityName;
//		public Image [ ] abilityPiece;
//		public Image [ ] p1NonagressionPiece;
//		public Image [ ] p2NonagressionPiece;
//		public Text gameClock;
//		public SpriteRenderer [ ] pieces;
//	}
//	public playerUI player1;
//	public playerUI player2;
//	public SpriteRenderer [ ] tiles;
//
//	//Settings game objects
//	public Toggle horizontalLayout;
//	public Toggle verticalLayout;
//	public Slider musicSlider;
//	public Text musicValue;
//	public Slider soundSlider;
//	public Text soundValue;
//	public Slider timerSlider;
//	public Text timerValue;
//
//	//Button colors
//	private ColorBlock selected;
//	private ColorBlock unselected;
//
//	//Animation information
//	private const float SLIDE_TIME = 1f;
//	private const float FADE_TIME = 0.4f;
//	private bool allowInput = true;
//	private enum MenuDestinations
//	{
//		MainMenu,
//		AbilitySelection,
//		LoadGame
//	};
//
//	/// <summary>
//	/// Checks for any save data.
//	/// </summary>
//	private void Start ( )
//	{
//		//Set selected/unselected colors
//		selected = menuButtons [ 0 ].colors;
//		selected.normalColor = new Color32 ( 255, 210, 75, 255 );
//		unselected = menuButtons [ 0 ].colors;
//		unselected.normalColor = new Color32 ( 255, 255, 200, 255 );
//
//		//Set menu information
//		saveData.SetActive ( false );
//		for ( int i = 0; i < menuButtons.Length; i++ )
//		{
//			menuInfo.Add ( menuButtons [ i ], menuPanels [ i ] );
//			menuPanels [ i ].gameObject.SetActive ( false );
//		}
//
//		//Check for a saved game
//		if ( Settings.SaveDataP1Abilities [ 0 ] != Vector3.zero )
//			menuButtons [ 1 ].interactable = true;
//		else
//			menuButtons [ 1 ].interactable = false;
//
//		//Set the loading save game check
//		Info.IsLoadingSavedGame = false;
//
//		//Open new game menu
//		OnMenuButtonClick ( menuButtons [ 0 ], false );
//
//		//Play intro
//		AnimateIntro ( );
//	}
//
//	/// <summary>
//	/// Plays the intro animation.
//	/// </summary>
//	private void AnimateIntro ( )
//	{
//		//Disable input
//		allowInput = false;
//
//		//Play animation
//		Sequence s = DOTween.Sequence ( )
//			.Append ( sideBar.DOAnchorPos ( new Vector2 ( -sideBar.rect.width, 0 ), SLIDE_TIME ).From ( ) )
//			.Append ( menuPanels [ 0 ].DOFade ( 0, FADE_TIME ).From ( ) )
//			.OnComplete ( () =>
//		    {
//				//Enable input
//				allowInput = true;
//			} )
//			.SetRecyclable ( )
//			.Play ( );
//	}
//
//	/// <summary>
//	/// Plays the outro animation.
//	/// </summary>
//	private void AnimateOutro ( MenuDestinations d )
//	{
//		//Disable input
//		allowInput = false;
//		
//		//Play animation
//		Sequence s = DOTween.Sequence ( );
//		if ( menuPanels [ 0 ].gameObject.activeSelf )
//			s.Append ( menuPanels [ 0 ].DOFade ( 0, FADE_TIME ) );
//		else if ( menuPanels [ 1 ].gameObject.activeSelf )
//		{
//			s.Append ( menuPanels [ 1 ].DOFade ( 0, FADE_TIME ) );
//			for ( int i = 0; i < tiles.Length; i++ )
//				s.Insert ( 0, tiles [ i ].DOFade ( 0, FADE_TIME ) );
//			for ( int i = 0; i < player1.pieces.Length; i++ )
//				s.Insert ( 0, player1.pieces [ i ].DOFade ( 0, FADE_TIME ) );
//			for ( int i = 0; i < player2.pieces.Length; i++ )
//				s.Insert ( 0, player2.pieces [ i ].DOFade ( 0, FADE_TIME ) );
//		}
//		else if ( menuPanels [ 2 ].gameObject.activeSelf )
//			s.Append ( menuPanels [ 2 ].DOFade ( 0, FADE_TIME ) );
//		s.Append ( sideBar.DOAnchorPos ( new Vector2 ( -sideBar.rect.width, 0 ), SLIDE_TIME ) )
//			.OnComplete ( () =>
//			{
//				//Check destination
//				switch ( d )
//				{
//					case MenuDestinations.AbilitySelection:
//						Application.LoadLevel ( "Select Abilities" );
//						break;
//					case MenuDestinations.LoadGame:
//						Application.LoadLevel ( "Game Board" );
//						break;
//					case MenuDestinations.MainMenu:
//						Application.LoadLevel ( "Main Menu" );
//						break;
//				}
//			} )
//			.SetRecyclable ( )
//			.Play ( );
//	}
//
//	/// <summary>
//	/// Opens a menu in the Start Game menu screen.
//	/// </summary>
//	public void OnMenuButtonClick ( Button b )
//	{
//		//Check input
//		if ( allowInput )
//			OnMenuButtonClick ( b, true );
//	}
//
//	/// <summary>
//	/// Opens a menu in the Start Game menu screen.
//	/// </summary>
//	private void OnMenuButtonClick ( Button b, bool playSFX )
//	{
//		//Play SFX
//		if ( playSFX )
//			SFXManager.instance.Click ( );
//
//		//Unselect any previously selected button
//		foreach ( Button a in menuButtons )
//			a.colors = unselected;
//		
//		//Select current button
//		b.colors = selected;
//
//		//Display information panel
//		foreach ( CanvasGroup p in menuPanels )
//		{
//			if ( p != menuInfo [ b ] )
//				p.gameObject.SetActive ( false );
//			else
//				p.gameObject.SetActive ( true );
//		}
//
//		//Hide save data
//		saveData.SetActive ( false );
//
//		//Check button
//		if ( b == menuButtons [ 0 ] )
//		{
//			//Display layout
//			if ( Settings.LayoutIsVertical )
//				layoutPrompt.text = "Vertical Board Layout";
//			else
//				layoutPrompt.text = "Horizontal Board Layout";
//
//			//Display game clock
//			if ( Settings.GameClock != 0 )
//				gameClockPrompt.text = Settings.GameClock + ":00 Game Clock";
//			else
//				gameClockPrompt.text = "No Game Clock";
//		}
//		else if ( b == menuButtons [ 1 ] )
//		{
//			//Display save data
//			saveData.SetActive ( true );
//
//			//Load player 1's abilities
//			LoadGamePlayerPreview ( player1, Settings.SaveDataP1Abilities, Settings.SaveDataP1GameClock );
//
//			//Load player 2's abilities
//			LoadGamePlayerPreview ( player2, Settings.SaveDataP2Abilities, Settings.SaveDataP2GameClock );
//
//			//Load board
//			LoadGameBoardPreview ( );
//		}
//		else if ( b == menuButtons [ 3 ] )
//		{
//			//Check layout settings
//			if ( Settings.LayoutIsVertical )
//			{
//				//Set toggle to vertical
//				ExecuteEvents.Execute ( verticalLayout.gameObject, new PointerEventData ( EventSystem.current ), ExecuteEvents.pointerClickHandler );
//			}
//			else
//			{
//				//Set toggle to horizontal
//				ExecuteEvents.Execute ( horizontalLayout.gameObject, new PointerEventData ( EventSystem.current ), ExecuteEvents.pointerClickHandler );
//			}
//			
//			//Load music volume
//			OnMusicSettingChange ( Settings.MusicVolume * 100 );
//			
//			//Load sound volume
//			OnSoundSettingChange ( Settings.SoundVolume * 100 );
//			
//			//Load game clock start time
//			float startTime = 0;
//			if ( Settings.GameClock != 0 )
//				startTime = ( 35 - Settings.GameClock ) / 5;
//			OnTimerSettingChange ( startTime );
//		}
//	}
//
//	/// <summary>
//	/// Exits the Start Game menu screen.
//	/// </summary>
//	public void OnExitClick ( )
//	{
//		//Load main menu
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.Click ( );
//			
//			//Transition to the main menu
//			MusicManager.instance.ChangeMusic ( AudioContext.MainMenu );
//			AnimateOutro ( MenuDestinations.MainMenu );
//		}
//	}
//
//	/// <summary>
//	/// Starts a new game.
//	/// </summary>
//	public void OnStartGameClick ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.Click ( );
//
//			//Play outro
//			AnimateOutro ( MenuDestinations.AbilitySelection );
//		}
//	}
//
//	/// <summary>
//	/// Displays a preview of a player's abilities and game clock in a saved game in the Load Game Menu.
//	/// </summary>
//	private void LoadGamePlayerPreview ( playerUI player, Vector3 [ ] data, float gameClock )
//	{
//		//Load each of the player's abilities
//		for ( int i = 0; i < data.Length; i++ )
//		{
//			//Store the ability
//			Ability a = Ability.AbilityList.GetAbilityByID ( (int)data [ i ].x );
//			
//			//Load ability name
//			player.abilityName [ i ].text = a.Name;
//			
//			//Check if the ability is active
//			if ( data [ i ].y == 0 )
//				player.abilityName [ i ].color = new Color32 ( 200, 200, 200, 255 );
//			
//			//Check if the ability is attached to a piece
//			if ( a.AttachesToPiece )
//			{	
//				//Display the attached piece
//				int piece = (int)data [ i ].z;
//				switch ( (PieceColor)piece )
//				{
//					case PieceColor.Red:
//						player.abilityPiece [ i ].color = Piece.Display.Red;
//						break;
//					case PieceColor.Blue:
//						player.abilityPiece [ i ].color = Piece.Display.Blue;
//						break;
//					case PieceColor.Green:
//						player.abilityPiece [ i ].color = Piece.Display.Green;
//						break;
//					case PieceColor.Yellow:
//						player.abilityPiece [ i ].color = Piece.Display.Yellow;
//						break;
//					case PieceColor.Black:
//						player.abilityPiece [ i ].color = Piece.Display.Black;
//						break;
//				}
//			}
//			else if ( a.ID == Ability.AbilityList.NonagressionPact.ID && data [ i ].y == 0 )
//			{
//				//Display pieces
//				int p1Piece = (int)data [ i ].z / 10;
//				int p2Piece = (int)data [ i ].z % 10;
//				player.abilityPiece [ i ].gameObject.SetActive ( false );
//				player.p1NonagressionPiece [ i ].gameObject.SetActive ( true );
//				player.p2NonagressionPiece [ i ].gameObject.SetActive ( true );
//				switch ( (PieceColor)p1Piece )
//				{
//					case PieceColor.Red:
//						player.p1NonagressionPiece [ i ].color = Piece.Display.Red;
//						break;
//					case PieceColor.Blue:
//						player.p1NonagressionPiece [ i ].color = Piece.Display.Blue;
//						break;
//					case PieceColor.Green:
//						player.p1NonagressionPiece [ i ].color = Piece.Display.Green;
//						break;
//					case PieceColor.Yellow:
//						player.p1NonagressionPiece [ i ].color = Piece.Display.Yellow;
//						break;
//					case PieceColor.Black:
//						player.p1NonagressionPiece [ i ].color = Piece.Display.Black;
//						break;
//					case PieceColor.Grey:
//						player.p1NonagressionPiece [ i ].color = Piece.Display.Grey;
//						break;
//				}
//				switch ( (PieceColor)p2Piece )
//				{
//					case PieceColor.Red:
//						player.p2NonagressionPiece [ i ].color = Piece.Display.Red;
//						break;
//					case PieceColor.Blue:
//						player.p2NonagressionPiece [ i ].color = Piece.Display.Blue;
//						break;
//					case PieceColor.Green:
//						player.p2NonagressionPiece [ i ].color = Piece.Display.Green;
//						break;
//					case PieceColor.Yellow:
//						player.p2NonagressionPiece [ i ].color = Piece.Display.Yellow;
//						break;
//					case PieceColor.Black:
//						player.p2NonagressionPiece [ i ].color = Piece.Display.Black;
//						break;
//					case PieceColor.Grey:
//						player.p2NonagressionPiece [ i ].color = Piece.Display.Grey;
//						break;
//				}
//			}
//			else
//			{
//				//Hide piece
//				player.abilityPiece [ i ].gameObject.SetActive ( false );
//			}
//		}
//
//		//Load each player's game clock
//		if ( Settings.SaveDataGameClock != 0 )
//		{
//			//Display game clock
//			int roundedTime = Mathf.CeilToInt ( gameClock );
//			int sec = roundedTime % 60;
//			int min = roundedTime / 60;
//			player.gameClock.text = String.Format ( "{0:00}:{1:00}", min, sec );
//		}
//		else
//		{
//			//Hide game clock
//			player.gameClock.text = "";
//		}
//	}
//
//	/// <summary>
//	/// Displays a preview of the board in a saved game in the Load Game Menu.
//	/// </summary>
//	private void LoadGameBoardPreview ( )
//	{
//		//Hide each piece
//		foreach ( SpriteRenderer p in player1.pieces )
//			p.gameObject.SetActive ( false );
//		foreach ( SpriteRenderer p in player2.pieces )
//			p.gameObject.SetActive ( false );
//		
//		//Store any grey pieces
//		int p1GreyPos = 0;
//		int p2GreyPos = 0;
//		
//		//Position each piece
//		for ( int i = 0; i < Settings.SaveDataPieces.Length; i++ )
//		{
//			//Store data
//			Vector3 data = Settings.SaveDataPieces [ i ];
//			
//			//Skip empty data
//			if ( data == Vector3.zero )
//				continue;
//			
//			//Check owner
//			if ( data.x == 1 )
//			{
//				//Store piece
//				int c = (int)data.y;
//				if ( c == (int)PieceColor.Grey )
//				{
//					p1GreyPos = (int)data.z + 1;
//					continue;
//				}
//				
//				//Display piece
//				player1.pieces [ c - 1 ].gameObject.SetActive ( true );
//				
//				//Move piece
//				player1.pieces [ c - 1 ].transform.position = tiles [ (int)data.z - 1 ].transform.position;
//			}
//			else if ( data.x == 2 )
//			{
//				//Store piece
//				int c = (int)data.y;
//				if ( c == (int)PieceColor.Grey )
//				{
//					p2GreyPos = (int)data.z + 1;
//					continue;
//				}
//				
//				//Display piece
//				player2.pieces [ c - 1 ].gameObject.SetActive ( true );
//				
//				//Move piece
//				player2.pieces [ c - 1 ].transform.position = tiles [ (int)data.z - 1 ].transform.position;
//			}
//		}
//		
//		//Position grey pieces
//		if ( p1GreyPos != 0 )
//		{
//			foreach ( SpriteRenderer p in player2.pieces )
//			{
//				if ( p.gameObject.activeSelf )
//					continue;
//				
//				//Display piece
//				p.gameObject.SetActive ( true );
//				
//				//Convert piece
//				p.color = Piece.Display.Grey;
//				p.transform.eulerAngles = new Vector3 ( 0, 0, 45 );
//				
//				//Move piece
//				p.transform.position = tiles [ p1GreyPos ].transform.position;
//				break;
//			}
//		}
//		if ( p2GreyPos != 0 )
//		{
//			foreach ( SpriteRenderer p in player1.pieces )
//			{
//				if ( p.gameObject.activeSelf )
//					continue;
//				
//				//Display piece
//				p.gameObject.SetActive ( true );
//				
//				//Convert piece
//				p.color = Piece.Display.Grey;
//				p.transform.eulerAngles = new Vector3 ( 0, 0, 225 );
//				
//				//Move piece
//				p.transform.position = tiles [ p2GreyPos ].transform.position;
//				break;
//			}
//		}
//	}
//
//	/// <summary>
//	/// Loads a saved game.
//	/// </summary>
//	public void OnLoadGameClick ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Set the loading save game check
//			Info.IsLoadingSavedGame = true;
//		
//			//Play SFX
//			SFXManager.instance.Click ( );
//
//			//Load save game
//			MusicManager.instance.ChangeMusic ( AudioContext.Gameplay );
//
//			//Play outro
//			AnimateOutro ( MenuDestinations.LoadGame );
//		}
//	}
//
//	/// <summary>
//	/// Deletes a saved game.
//	/// </summary>
//	public void OnDeleteGameClick ( Button b )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.Click ( );
//
//			//Set empty save data
//			Settings.SaveDataIsP1Turn = true;
//			PlayerPrefsX.SetBool ( "playerTurn", Settings.SaveDataIsP1Turn );
//			for ( int i = 0; i < Settings.SaveDataP1Abilities.Length; i++ )
//				Settings.SaveDataP1Abilities [ i ] = Vector3.zero;
//			PlayerPrefsX.SetVector3Array ( "player1Abilities", Settings.SaveDataP1Abilities );
//			for ( int i = 0; i < Settings.SaveDataP2Abilities.Length; i++ )
//				Settings.SaveDataP2Abilities [ i ] = Vector3.zero;
//			PlayerPrefsX.SetVector3Array ( "player2Abilities", Settings.SaveDataP2Abilities );
//			for ( int i = 0; i < Settings.SaveDataPieces.Length; i++ )
//				Settings.SaveDataPieces [ i ] = Vector3.zero;
//			PlayerPrefsX.SetVector3Array ( "pieces", Settings.SaveDataPieces );
//			Settings.SaveDataGameClock = 0;
//			PlayerPrefs.SetInt ( "saveGameClock", Settings.SaveDataGameClock );
//			Settings.SaveDataP1GameClock = 0;
//			PlayerPrefs.SetFloat ( "player1GameClock", Settings.SaveDataP1GameClock );
//			Settings.SaveDataP2GameClock = 0;
//			PlayerPrefs.SetFloat ( "player2GameClock", Settings.SaveDataP2GameClock );
//			
//			//Disable load game button
//			b.interactable = false;
//			
//			//Hide load game panel
//			menuInfo [ b ].gameObject.SetActive ( false );
//
//			//Open new game menu
//			OnMenuButtonClick ( menuButtons [ 0 ], false );
//		}
//	}
//
//	/// <summary>
//	/// Starts the movement tutorial.
//	/// </summary>
//	public void OnMovementTutorialClick ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Set movement tutorial as playing
//			Tutorial.isPlaying = true;
//			Tutorial.isGame1 = true;
//			
//			//Play SFX
//			SFXManager.instance.Click ( );
//			
//			//Load tutorial
//			MusicManager.instance.ChangeMusic ( AudioContext.Gameplay );
//			
//			//Play outro
//			AnimateOutro ( MenuDestinations.LoadGame );
//		}
//	}
//
//	/// <summary>
//	/// Starts the ability tutorial.
//	/// </summary>
//	public void OnAbilityTutorialClick ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Set movement tutorial as playing
//			Tutorial.isPlaying = true;
//			Tutorial.isGame1 = false;
//			
//			//Play SFX
//			SFXManager.instance.Click ( );
//			
//			//Load tutorial
//			MusicManager.instance.ChangeMusic ( AudioContext.Gameplay );
//			
//			//Play outro
//			AnimateOutro ( MenuDestinations.LoadGame );
//		}
//	}
//
//	/// <summary>
//	/// Sets the board layout to horizontal.
//	/// </summary>
//	public void OnHorizontalSettingClick ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.Click ( );
//
//			//Set board layout as horizontal
//			Settings.LayoutIsVertical = false;
//			
//			//Save setting
//			PlayerPrefsX.SetBool ( "boardLayout", Settings.LayoutIsVertical );
//		}
//	}
//	
//	/// <summary>
//	/// Sets the board layout to vertical.
//	/// </summary>
//	public void OnVerticalSettingClick ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.Click ( );
//
//			//Set board layout as vertical
//			Settings.LayoutIsVertical = true;
//			
//			//Save setting
//			PlayerPrefsX.SetBool ( "boardLayout", Settings.LayoutIsVertical );
//		}
//	}
//	
//	/// <summary>
//	/// Sets the music volume.
//	/// </summary>
//	public void OnMusicSettingChange ( float value )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Update slider
//			musicSlider.value = value;
//			
//			//Display music volume
//			musicValue.text = value + "%";
//			
//			//Set music volume
//			Settings.MusicVolume = value / 100;
//			MusicManager.instance.UpdateMusicVolume ( );
//			
//			//Save setting
//			PlayerPrefs.SetFloat ( "musicVolume", Settings.MusicVolume );
//		}
//	}
//	
//	/// <summary>
//	/// Sets the sound volume.
//	/// </summary>
//	public void OnSoundSettingChange ( float value )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Update slider
//			soundSlider.value = value;
//			
//			//Display sound volume
//			soundValue.text = value + "%";
//			
//			//Set sound volume
//			Settings.SoundVolume = value / 100;
//			SFXManager.instance.UpdateSFXVolume ( );
//			
//			//Save setting
//			PlayerPrefs.SetFloat ( "soundVolume", Settings.SoundVolume );
//		}
//	}
//
//	/// <summary>
//	/// Sets the starting time for the game clock.
//	/// </summary>
//	public void OnTimerSettingChange ( float value )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Update the slider
//			timerSlider.value = value;
//			
//			//Store game clock start time
//			int startTime = 0;
//			if ( value != 0 )
//				startTime = 35 - ( (int)value * 5 );
//			
//			//Display game clock setting
//			if ( value != 0 )
//				timerValue.text = startTime + ":00";
//			else
//				timerValue.text = "N/A";
//			
//			//Set game clock setting
//			Settings.GameClock = startTime;
//			
//			//Save setting
//			PlayerPrefs.SetInt ( "gameClock", startTime );
//		}
//	}
}
