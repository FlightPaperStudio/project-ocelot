using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Info : MonoBehaviour 
{
//	//Player information
//	public Player player1;
//	public Player player2;
//	[ HideInInspector ]
//	public Player currentPlayer;
//	[ HideInInspector ]
//	public Player opponent;
//
//	//Ability information
//	public static Ability [ ] player1AbilityList = new Ability [ 3 ];
//	public static PieceColor player1Sacrifice;
//	public static Ability [ ] player2AbilityList = new Ability [ 3 ];
//	public static PieceColor player2Sacrifice;
//	public Ability abilityInUse;
//
//	//Board information
//	public List < Tile > board = new List < Tile > ( );
//	public Tutorial tutor;
//
//	//UI information
//	public Camera cam;
//	private Button currentAbilityButton;
//
//	//Turn information
//	[ HideInInspector ]
//	public bool beginningOfTurn = true;
//	public bool isGameOver = false;
//	public Text turnText;
//	public Text P1TurnText;
//	public Text p2TurnText;
//
//	//Ability information
//	[ HideInInspector ]
//	public List < Tile > abilityTileSelection = new List < Tile > ( );
//	[ HideInInspector ]
//	public List < CabooseList > cabooseList = new List < CabooseList > ( );
//	[ HideInInspector ]
//	public CabooseList selectedCaboose;
//	[ HideInInspector ]
//	public bool multiCabooseTile = false;
//	[ HideInInspector ]
//	public Tile grimReaperTile;
//	[ HideInInspector ]
//	public List < Tile > joltList = new List < Tile > ( );
//	public Tile sacrificeArmorTile;
//
//	//Piece information
//	[ HideInInspector ]
//	public Piece selectedPiece;
//
//	//Popup information
//	public Image popupPanel;
//	public Image winPanel;
//	public GameObject pausePanel;
//	public Button saveButton;
//	public Text savePrompt;
//	public Button settingsButton;
//	public GameObject settingsPanel;
//	public Text winPrompt;
//	public Image rematchButton;
//	public Text rematchText;
//	public Image mainMenuButton;
//	public Text mainMenuText;
//	public Image exitFade;
//
//	//Settings game objects
//	public Toggle horizontalLayout;
//	public Toggle verticalLayout;
//	public Slider musicSlider;
//	public Text musicValue;
//	public Slider soundSlider;
//	public Text soundValue;
//	private bool isSetUp = true;
//
//	//Save data information
//	public static bool IsLoadingSavedGame = false;
//	private bool tempSaveIsP1Turn;
//	private Vector3 [ ] tempSaveP1Abilities = new Vector3 [ 3 ];
//	private Vector3 [ ] tempSaveP2Abilities = new Vector3 [ 3 ];
//	private Vector3 [ ] tempSavePieces = new Vector3 [ 12 ];
//	private int tempSaveP1Sacrifice;
//	private int tempSaveP2Sacrifice;
//	private float tempSaveP1GameClock;
//	private float tempSaveP2GameClock;
//
//	//Animation information
//	private const float WAIT_TIME = 0.4f;
//	private const float WAVE_TIME = 0.2f;
//	private const float DELAY_TIME = 1f / 3f;
//	private const float SLIDE_TIME = 1f;
//	private const float FADE_TIME = 0.4f;
//	private const float EXIT_TIME = 1.4f;
//	private const float ANIMATE_TIME = 0.5f;
//
//	/// <summary>
//	/// Sets up the start of a game.
//	/// </summary>
//	private void Start ( ) 
//	{
//		//Set game clock start time
//		if ( Settings.GameClock != 0 )
//		{
//			player1.gameClock = (float)Settings.GameClock * 60;
//			player2.gameClock = (float)Settings.GameClock * 60;
//		}
//
//		//Check if loading a save game
//		if ( IsLoadingSavedGame )
//		{
//			//Load save game
//			LoadSaveGame ( );
//		}
//		else
//		{
//			//Load abilities
//			LoadAbilities ( );
//
//			//Start with player 1's turn
//			currentPlayer = player1;
//			opponent = player2;
//
//			//Sacrifice any pieces on the list
//			if ( player1Sacrifice != null && player1Sacrifice != PieceColor.None )
//			{
//				//Store piece
//				Piece p = player1.pieces.Find ( x => x.color == player1Sacrifice );
//				player1.pieces.Remove ( p );
//				p.Capture ( );
//			}
//			if ( player2Sacrifice != null && player2Sacrifice != PieceColor.None )
//			{
//				//Store piece
//				Piece p = player2.pieces.Find ( x => x.color == player2Sacrifice );
//				player2.pieces.Remove ( p );
//				p.Capture ( );
//			}
//		}
//
//		//Hide goal areas
//		player1.goalArea.SetActive ( false );
//		player2.goalArea.SetActive ( false );
//
//		//Store UI
//		player1.endButton.SetActive ( false );
//		player2.endButton.SetActive ( false );
//		player1.cancelButton.SetActive ( false );
//		player2.cancelButton.SetActive ( false );
//		player1.acceptButton.gameObject.SetActive ( false );
//		player2.acceptButton.gameObject.SetActive ( false );
//		player1.prompt.gameObject.SetActive ( false );
//		player2.prompt.gameObject.SetActive ( false );
//		if ( !Tutorial.isPlaying && Settings.GameClock != 0 )
//		{
//			//Display game clock
//			int roundedTime = Mathf.CeilToInt ( player1.gameClock );
//			int sec = roundedTime % 60;
//			int min = roundedTime / 60;
//			player1.gameClockDisplay.text = String.Format ( "{0:00}:{1:00}", min, sec );
//			roundedTime = Mathf.CeilToInt ( player2.gameClock );
//			sec = roundedTime % 60;
//			min = roundedTime / 60;
//			player2.gameClockDisplay.text = String.Format ( "{0:00}:{1:00}", min, sec );
//		}
//		else
//		{
//			//Hide game clock
//			player1.gameClockDisplay.gameObject.SetActive ( false );
//			player2.gameClockDisplay.gameObject.SetActive ( false );
//		}
//
//		//Open settings menu
//		PauseGame ( );
//
//		//Check layout settings
//		if ( Settings.LayoutIsVertical )
//		{
//			//Set toggle to vertical
//			SettingsMenu ( );
//			ExecuteEvents.Execute ( verticalLayout.gameObject, new PointerEventData ( EventSystem.current ), ExecuteEvents.pointerClickHandler );
//		}
//		
//		//Load music volume
//		OnMusicSettingChange ( Settings.MusicVolume * 100 );
//		
//		//Load sound volume
//		OnSoundSettingChange ( Settings.SoundVolume * 100 );
//
//		//Display the current player's ability buttons
//		for ( int i = 0; i < currentPlayer.abilities.Length; i++ )
//		{
//			//Check for non-attached abilities
//			if ( !currentPlayer.abilities [ i ].AttachesToPiece )
//			{
//				//Check for an inactive Nonagression Pact
//				if ( currentPlayer.abilities [ i ].ID == Ability.AbilityList.NonagressionPact.ID && !currentPlayer.abilities [ i ].IsActive )
//					continue;
//				
//				//Display use ability button
//				currentPlayer.abilityButtons [ i ].gameObject.SetActive ( true );
//			}
//		}
//
//		//Disable the save button in tutorials
//		if ( Tutorial.isPlaying )
//			saveButton.interactable = false;
//
//		//Start game
//		isSetUp = false;
//		ResumeGame ( false );
//		EnableAbilityButtons ( );
//		beginningOfTurn = true;
//		AnimateIntro ( );
//	}
//
//
//	/// <summary>
//	/// Plays the intro animation.
//	/// </summary>
//	private void AnimateIntro ( )
//	{
//		//Initialize animation
//		Sequence s = DOTween.Sequence ( );
//
//		//Make tiles ripple
//		for ( int i = 0; i < board.Count; i++ )
//			s.Insert ( WAIT_TIME + ( board [ i ].wave * DELAY_TIME * WAVE_TIME ), board [ i ].transform.DOPunchScale ( new Vector3 ( -1, -1, -1 ), WAVE_TIME, 1, 1 ) );
//
//		//Make pieces fade in
//		for ( int i = 0; i < player1.pieces.Count; i++ )
//			s.Insert ( WAIT_TIME + WAVE_TIME + ( board [ 0 ].wave * DELAY_TIME * WAVE_TIME ), player1.pieces [ i ].sprite.DOFade ( 0, FADE_TIME ).From ( ) );
//		for ( int i = 0; i < player2.pieces.Count; i++ )
//			s.Insert ( WAIT_TIME + WAVE_TIME + ( board [ 0 ].wave * DELAY_TIME * WAVE_TIME ), player2.pieces [ i ].sprite.DOFade ( 0, FADE_TIME ).From ( ) );
//
//		//Set up callback
//		s.OnComplete ( () =>
//		{
//			//Start game
//			ResetBoardColor ( );
//			HighlightCurrentPlayerPieces ( );
//
//			//Start tutorial
//			if ( Tutorial.isPlaying )
//				tutor.NextMove ( );
//
//			//Start clock
//			if ( !Tutorial.isPlaying && Settings.GameClock != 0 )
//				StartClock ( currentPlayer );
//		} )
//		.SetRecyclable ( )
//		.Play ( );
//	}
//
//	/// <summary>
//	/// Checks for the ESC key being pressed to open or close the pause menu.
//	/// </summary>
//	private void Update ( )
//	{
//		//Check for esc key press
//		if ( Input.GetKeyDown ( KeyCode.Escape ) && !winPanel.gameObject.activeSelf )
//		{
//			//Pause/Unpause the game
//			if ( popupPanel.gameObject.activeSelf )
//				ResumeGame ( false );
//			else
//				PauseGame ( );
//		}
//		
//		//Check for game clock
//		if ( currentPlayer.clockIsActive )
//		{
//			//Check game clock
//			if ( currentPlayer.gameClock > 0 )
//			{
//				//Decrease game clock
//				currentPlayer.gameClock -= Time.deltaTime;
//				
//				//Display game clock
//				int roundedTime = Mathf.CeilToInt ( currentPlayer.gameClock );
//				int sec = roundedTime % 60;
//				int min = roundedTime / 60;
//				currentPlayer.gameClockDisplay.text = String.Format ( "{0:00}:{1:00}", min, sec );
//			}
//			else
//			{
//				//The current player loses the game
//				StopClock ( currentPlayer );
//				currentPlayer = opponent;
//				WinGame ( );
//			}
//		}
//	}
//
//	/// <summary>
//	/// Loads a saved game.
//	/// </summary>
//	private void LoadSaveGame ( )
//	{
//		//Load which player's turn it is
//		if ( Settings.SaveDataIsP1Turn )
//		{
//			//Start on player 1's turn
//			currentPlayer = player1;
//			opponent = player2;
//			turnText.text = "Player 1's\nTurn";
//		}
//		else
//		{
//			//Start on player 2's turn
//			currentPlayer = player2;
//			opponent = player1;
//			turnText.text = "Player 2's\nTurn";
//		}
//
//		//Load player 1's abilities and game clock
//		LoadSavedPlayerAbilities ( player1, player1AbilityList, Settings.SaveDataP1Abilities, Settings.SaveDataP1Sacrifice );
//
//		//Load player 2's abilities and game clock
//		LoadSavedPlayerAbilities ( player2, player2AbilityList, Settings.SaveDataP2Abilities, Settings.SaveDataP2Sacrifice );
//
//		//Remove any tile references from the pieces
//		foreach ( Piece p in player1.pieces )
//		{
//			p.currentTile.currentPiece = null;
//			p.currentTile = null;
//		}
//		foreach ( Piece p in player2.pieces )
//		{
//			p.currentTile.currentPiece = null;
//			p.currentTile = null;
//		}
//
//		//Store any potential grey pieces
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
//					p1GreyPos = (int)data.z;
//					continue;
//				}
//				Piece p = player1.GetPieceByColor ( (PieceColor)c );
//
//				//Store tile
//				Tile t = board.Find ( x => x.ID == (int)data.z );
//
//				//Move piece
//				p.Move ( t.transform.position );
//				p.currentTile = t;
//				t.currentPiece = p;
//			}
//			else if ( data.x == 2 )
//			{
//				//Store piece
//				int c = (int)data.y;
//				if ( c == (int)PieceColor.Grey )
//				{
//					p2GreyPos = (int)data.z;
//					continue;
//				}
//				Piece p = player2.GetPieceByColor ( (PieceColor)c );
//				
//				//Store tile
//				Tile t = board.Find ( x => x.ID == (int)data.z );
//				
//				//Move piece
//				p.Move ( t.transform.position );
//				p.currentTile = t;
//				t.currentPiece = p;
//			}
//		}
//
//		//Position grey pieces
//		if ( p1GreyPos != 0 )
//		{
//			foreach ( Piece p in player2.pieces )
//			{
//				if ( p.currentTile != null )
//					continue;
//
//				//Convert piece
//				currentPlayer = player1;
//				opponent = player2;
//				Ability.AbilityList.UseConversion ( p, this );
//
//				//Store tile
//				Tile t = board.Find ( x => x.ID == p1GreyPos );
//
//				//Move piece
//				p.Move ( t.transform.position );
//				p.currentTile = t;
//				t.currentPiece = p;
//				break;
//			}
//		}
//		if ( p2GreyPos != 0 )
//		{
//			foreach ( Piece p in player1.pieces )
//			{
//				if ( p.currentTile != null )
//					continue;
//
//				//Convert piece
//				currentPlayer = player2;
//				opponent = player1;
//				Ability.AbilityList.UseConversion ( p, this );
//					
//				//Store tile
//				Tile t = board.Find ( x => x.ID == p2GreyPos );
//					
//				//Move piece
//				p.Move ( t.transform.position );
//				p.currentTile = t;
//				t.currentPiece = p;
//				break;
//			}
//		}
//
//		//Delete any remaining pieces
//		for ( int i = player1.pieces.Count - 1; i >= 0; i-- )
//		{
//			if ( player1.pieces [ i ].currentTile != null )
//				continue;
//
//			//Delete piece
//			player1.pieces [ i ].Capture ( );
//			player1.pieces.RemoveAt ( i );
//		}
//		for ( int i = player2.pieces.Count - 1; i >= 0; i-- )
//		{
//			if ( player2.pieces [ i ].currentTile != null )
//				continue;
//
//			//Delete piece
//			player2.pieces [ i ].Capture ( );
//			player2.pieces.RemoveAt ( i );
//		}
//
//		//Load each player's game clock
//		Settings.GameClock = Settings.SaveDataGameClock;
//		if ( Settings.SaveDataGameClock != 0 )
//		{
//			player1.gameClock = Settings.SaveDataP1GameClock;
//			player2.gameClock = Settings.SaveDataP2GameClock;
//		}
//
//		//Finish loading save game
//		IsLoadingSavedGame = false;
//	}
//
//	/// <summary>
//	/// Loads the player's abilities and game clock from the save data.
//	/// </summary>
//	private void LoadSavedPlayerAbilities ( Player player, Ability [ ] abilities, Vector3 [ ] data, int sacrificeData )
//	{
//		//Load each of the player's abilities
//		for ( int i = 0; i < data.Length; i++ )
//		{
//			//Store the ability
//			abilities [ i ] = Ability.AbilityList.GetAbilityByID ( (int)data [ i ].x );
//			player.abilities [ i ] = abilities [ i ];
//			
//			//Load name
//			player.abilityNames [ i ].text = player.abilities [ i ].Name;
//			
//			//Check if the ability is active
//			if ( data [ i ].y == 0 )
//				DisableAbility ( player.abilities [ i ].ID, player );
//			
//			//Check if attached
//			if ( player.abilities [ i ].AttachesToPiece )
//			{
//				//Make piece visible
//				player.abilityPieces [ i ].gameObject.SetActive ( true );
//				player.abilityButtons [ i ].gameObject.SetActive ( false );
//				
//				//Store the piece
//				int piece = (int)data [ i ].z;
//				abilities [ i ].AttachedPiece = (PieceColor)piece;
//				player.abilities [ i ].AttachedPiece = abilities [ i ].AttachedPiece;
//				
//				//Check color for display
//				switch ( player.abilities [ i ].AttachedPiece )
//				{
//					case PieceColor.Red:
//						player.abilityPieces [ i ].color = Piece.Display.Red;
//						break;
//					case PieceColor.Blue:
//						player.abilityPieces [ i ].color = Piece.Display.Blue;
//						break;
//					case PieceColor.Green:
//						player.abilityPieces [ i ].color = Piece.Display.Green;
//						break;
//					case PieceColor.Yellow:
//						player.abilityPieces [ i ].color = Piece.Display.Yellow;
//						break;
//					case PieceColor.Black:
//						player.abilityPieces [ i ].color = Piece.Display.Black;
//						break;
//				}
//				
//				//Attach ability to piece
//				player.GetPieceByColor ( player.abilities [ i ].AttachedPiece ).ability = player.abilities [ i ];
//
//				//Check for sacrifice
//				if ( player.abilities [ i ].ID == Ability.AbilityList.Sacrifice.ID )
//				{
//					//Store save data
//					if ( player == player1 )
//						player1Sacrifice = (PieceColor)sacrificeData;
//					else
//						player2Sacrifice = (PieceColor)sacrificeData;
//				}
//			}
//			else if ( player.abilities [ i ].ID == Ability.AbilityList.NonagressionPact.ID && !player.abilities [ i ].IsActive )
//			{
//				//Store the pieces
//				int p1Piece = (int)data [ i ].z / 10;
//				int p2Piece = (int)data [ i ].z % 10;
//				
//				//Display pieces
//				player.abilityPieces [ i ].gameObject.SetActive ( false );
//				player.p1NonagressionPieces [ i ].gameObject.SetActive ( true );
//				player.p2NonagressionPieces [ i ].gameObject.SetActive ( true );
//				switch ( (PieceColor)p1Piece )
//				{
//					case PieceColor.Red:
//						player.p1NonagressionPieces [ i ].color = Piece.Display.Red;
//						break;
//					case PieceColor.Blue:
//						player.p1NonagressionPieces [ i ].color = Piece.Display.Blue;
//						break;
//					case PieceColor.Green:
//						player.p1NonagressionPieces [ i ].color = Piece.Display.Green;
//						break;
//					case PieceColor.Yellow:
//						player.p1NonagressionPieces [ i ].color = Piece.Display.Yellow;
//						break;
//					case PieceColor.Black:
//						player.p1NonagressionPieces [ i ].color = Piece.Display.Black;
//						break;
//					case PieceColor.Grey:
//						player.p1NonagressionPieces [ i ].color = Piece.Display.Grey;
//						break;
//				}
//				switch ( (PieceColor)p2Piece )
//				{
//					case PieceColor.Red:
//						player.p2NonagressionPieces [ i ].color = Piece.Display.Red;
//						break;
//					case PieceColor.Blue:
//						player.p2NonagressionPieces [ i ].color = Piece.Display.Blue;
//						break;
//					case PieceColor.Green:
//						player.p2NonagressionPieces [ i ].color = Piece.Display.Green;
//						break;
//					case PieceColor.Yellow:
//						player.p2NonagressionPieces [ i ].color = Piece.Display.Yellow;
//						break;
//					case PieceColor.Black:
//						player.p2NonagressionPieces [ i ].color = Piece.Display.Black;
//						break;
//					case PieceColor.Grey:
//						player.p2NonagressionPieces [ i ].color = Piece.Display.Grey;
//						break;
//				}
//				
//				//Apply nonagression pact
//				player1.GetPieceByColor ( (PieceColor)p1Piece ).nonagressionPartners.Add ( (PieceColor)p2Piece );
//				player2.GetPieceByColor ( (PieceColor)p2Piece ).nonagressionPartners.Add ( (PieceColor)p1Piece );
//			}
//			else
//			{
//				//Hide piece
//				player.abilityPieces [ i ].gameObject.SetActive ( false );
//			}
//		}
//	}
//
//	/// <summary>
//	/// Loads the players' abilities.
//	/// </summary>
//	private void LoadAbilities ( )
//	{
//		//Reenable all abilites, this is to prevent abilities from being unusable during a rematch
//		foreach ( Ability a in player1AbilityList )
//			a.IsActive = true;
//
//		foreach ( Ability a in player2AbilityList )
//			a.IsActive = true;
//
//		//Store player abilities
//		player1.abilities = player1AbilityList;
//		player2.abilities = player2AbilityList;
//
//		//Load player abilities
//		InitPlayerAbilities ( player1 );
//		InitPlayerAbilities ( player2 );
//	}
//
//	/// <summary>
//	/// Initializess the player's abilities.
//	/// </summary>
//	private void InitPlayerAbilities ( Player p )
//	{
//		//Display the player's abilities
//		for ( int i = 0; i < p.abilities.Length; i++ )
//		{
//			//Load name
//			p.abilityNames [ i ].text = p.abilities [ i ].Name;
//			
//			//Check if attached
//			if ( p.abilities [ i ].AttachesToPiece )
//			{
//				//Make piece visible
//				p.abilityPieces [ i ].gameObject.SetActive ( true );
//				p.abilityButtons [ i ].gameObject.SetActive ( false );
//
//				//Check color for display
//				switch ( p.abilities [ i ].AttachedPiece )
//				{
//					case PieceColor.Black:
//						p.abilityPieces [ i ].color = Piece.Display.Black;
//						break;
//					case PieceColor.Blue:
//						p.abilityPieces [ i ].color = Piece.Display.Blue;
//						break;
//					case PieceColor.Green:
//						p.abilityPieces [ i ].color = Piece.Display.Green;
//						break;
//					case PieceColor.Red:
//						p.abilityPieces [ i ].color = Piece.Display.Red;
//						break;
//					case PieceColor.Yellow:
//						p.abilityPieces [ i ].color = Piece.Display.Yellow;
//						break;
//				}
//				
//				//Attach ability to piece
//				p.GetPieceByColor ( p.abilities [ i ].AttachedPiece ).ability = p.abilities [ i ];
//			}
//			else
//			{
//				//Hide piece
//				p.abilityPieces [ i ].gameObject.SetActive ( false );
//
//				//Check for player 1
//				if ( p == player1 )
//					p.abilityButtons [ i ].gameObject.SetActive ( true );
//			}
//		}
//	}
//
//	/// <summary>
//	/// Resets the color of the board to the default grey.
//	/// </summary>
//	public void ResetBoardColor ( )
//	{
//		//Reset each tile
//		foreach ( Tile t in board )
//			t.ResetColor ( );
//
//		//Hide goal areas
//		player1.goalArea.SetActive ( false );
//		player2.goalArea.SetActive ( false );
//	}
//
//	/// <summary>
//	/// Highlights the current player's pieces.
//	/// </summary>
//	 public void HighlightCurrentPlayerPieces ( )
//	{
//		//Check each tile for the current player's pieces
//		foreach ( Piece p in currentPlayer.pieces )
//			p.currentTile.HighlightPlayerPiece ( );
//	}
//
//	/// <summary>
//	/// Highlights pieces for selection for using an ability.
//	/// </summary>
//	public void HighlightPiecesAbilitySelection ( bool isPlayer1 )
//	{
//		//Check player
//		if ( isPlayer1 )
//		{
//			//Check each tile for the current player's pieces
//			foreach ( Tile t in board )
//				if ( t.currentPiece != null && t.currentPiece.owner == player1 && t.currentPiece.color != PieceColor.White )
//					t.HighlightAbilityPieceSelection ( );
//		}
//		else
//		{
//			//Check each tile for the current player's pieces
//			foreach ( Tile t in board )
//				if ( t.currentPiece != null && t.currentPiece.owner == player2 && t.currentPiece.color != PieceColor.White )
//					t.HighlightAbilityPieceSelection ( );
//		}
//	}
//
//	/// <summary>
//	/// Disables the ability.
//	/// </summary>
//	public void DisableAbility ( int abilityID, Player player )
//	{
//		//Check each of the player's abilities
//		for ( int i = 0; i < player.abilities.Length; i++ )
//		{
//			//Check the ability
//			if ( player.abilities [ i ].ID == abilityID )
//			{
//				//Deactive ability
//				player.abilities [ i ].IsActive = false;
//				player.abilityNames [ i ].color = new Color32 ( 200, 200, 200, 255 );
//
//				//End function
//				return;
//			}
//		}
//	}
//
//	/// <summary>
//	/// Enables the ability buttons.
//	/// </summary>
//	public void EnableAbilityButtons ( )
//	{
//		//Enable all abilities that can only be used at the start of a turn
//		for ( int i = 0; i < currentPlayer.abilityButtons.Length; i++ )
//		{
//			//Check ability
//			if ( !currentPlayer.abilities [ i ].IsActive || ( currentPlayer.abilities [ i ].ID == Ability.AbilityList.Caboose.ID && !Ability.AbilityList.HasCaboose ( this ) ) )
//				currentPlayer.abilityButtons [ i ].interactable = false;
//			else
//				currentPlayer.abilityButtons [ i ].interactable = true;
//		}
//	}
//
//	/// <summary>
//	/// Disables the ability buttons to prevent abilities from being used before or after the start of a turn.
//	/// </summary>
//	public void DisableAbilityButtons ( )
//	{
//		//Disable all abilities that can only be used at the start of a turn
//		foreach ( Button b in currentPlayer.abilityButtons )
//			b.interactable = false;
//	}
//
//	/// <summary>
//	/// Displays the ability description.
//	/// </summary>
//	public void DisplayAbilityDescription ( Text t )
//	{
//		//Check for player 1 ability
//		for ( int i = 0; i < player1.abilityNames.Length; i++ )
//		{
//			//Check text
//			if ( player1.abilityNames [ i ] == t )
//			{
//				//Display the description
//				player1.descPanel [ i ].SetActive ( true );
//				player1.desc [ i ].text = player1.abilities [ i ].Desc;
//				return;
//			}
//		}
//
//		//Check for player 2 ability
//		for ( int i = 0; i < player2.abilityNames.Length; i++ )
//		{
//			//Check text
//			if ( player2.abilityNames [ i ] == t )
//			{
//				//Display the description
//				player2.descPanel [ i ].SetActive ( true );
//				player2.desc [ i ].text = player2.abilities [ i ].Desc;
//				return;
//			}
//		}
//	}
//
//	/// <summary>
//	/// Hides the ability description.
//	/// </summary>
//	public void HideAbilityDescription ( Text t )
//	{
//		//Check for player 1 ability
//		for ( int i = 0; i < player1.abilityNames.Length; i++ )
//		{
//			//Check text
//			if ( player1.abilityNames [ i ] == t )
//			{
//				//Hide the description
//				player1.descPanel [ i ].SetActive ( false );
//				return;
//			}
//		}
//		
//		//Check for player 2 ability
//		for ( int i = 0; i < player2.abilityNames.Length; i++ )
//		{
//			//Check text
//			if ( player2.abilityNames [ i ] == t )
//			{
//				//Hide the description
//				player2.descPanel [ i ].SetActive ( false );
//				return;
//			}
//		}
//	}
//
//	/// <summary>
//	/// Activates a non-attached ability.
//	/// </summary>
//	public void UseAbility ( int slot )
//	{
//		//Check for tutorial
//		if ( Tutorial.isPlaying )
//		{
//			//Check for current object
//			if ( tutor.CheckObject ( currentPlayer.abilityButtons [ slot ].gameObject ) )
//				tutor.NextMove ( );
//			else
//				return;
//		}
//
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Unselect current piece
//		selectedPiece = null;
//
//		//Disable ability button
//		DisableAbilityButtons ( );
//
//		//Display ability prompt
//		currentPlayer.cancelButton.SetActive ( true );
//		currentPlayer.acceptButton.gameObject.SetActive ( true );
//		currentPlayer.prompt.gameObject.SetActive ( true );
//
//		//Store ability button
//		currentAbilityButton = currentPlayer.abilityButtons [ slot ];
//		
//		//Store ability
//		abilityInUse = currentPlayer.abilities [ slot ];
//
//		//Disable the accept button
//		currentPlayer.acceptButton.interactable = false;
//
//		//Reset ability selection list
//		abilityTileSelection.Clear ( );
//
//		//Clear board
//		ResetBoardColor ( );
//
//		//Check ability
//		if ( currentPlayer.abilities [ slot ].ID == Ability.AbilityList.Caboose.ID )
//		{
//			//Load prompt
//			currentPlayer.prompt.text = "Choose a caboose to move.";
//
//			//Get the list of tiles
//			Ability.AbilityList.GetCaboose ( this );
//		}
//		else if ( currentPlayer.abilities [ slot ].ID == Ability.AbilityList.MadHatter.ID )
//		{
//			//Load prompt
//			currentPlayer.prompt.text = "Choose two pieces to swap places.";
//
//			//Check current player
//			if ( currentPlayer == player1 )
//			{
//				//Highlight pieces
//				HighlightPiecesAbilitySelection ( true );
//			}
//			else
//			{
//				//Highlight pieces
//				HighlightPiecesAbilitySelection ( false );
//			}
//		}
//		else if ( currentPlayer.abilities [ slot ].ID == Ability.AbilityList.NonagressionPact.ID )
//		{
//			//Highlight pieces
//			HighlightPiecesAbilitySelection ( true );
//			HighlightPiecesAbilitySelection ( false );
//
//			//Load prompt
//			currentPlayer.prompt.text = "Choose two pieces to prevent from interacting with one another.";
//		}
//	}
//
//	/// <summary>
//	/// Cancel the use of an ability.
//	/// </summary>
//	public void CancelAbilityUse ( )
//	{
//		//Check for tutorial
//		if ( Tutorial.isPlaying )
//			return;
//
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Enable ability use button
//		EnableAbilityButtons ( );
//
//		//Hide cancel buttons
//		currentPlayer.cancelButton.SetActive ( false );
//
//		//Hide accept buttons
//		currentPlayer.acceptButton.gameObject.SetActive ( false );
//
//		//Hide prompt
//		currentPlayer.prompt.gameObject.SetActive ( false );
//
//		//Reset ability selection list
//		abilityTileSelection.Clear ( );
//
//		//Reset caboose list
//		cabooseList.Clear ( );
//		multiCabooseTile = false;
//
//		//Reset board
//		ResetBoardColor ( );
//		
//		//Highlight pieces
//		HighlightCurrentPlayerPieces ( );
//	}
//
//	/// <summary>
//	/// Accepts the use of an ability.
//	/// </summary>
//	public void AcceptAbilityUse ( )
//	{
//		//Check for tutorial
//		if ( Tutorial.isPlaying )
//		{
//			//Check for current object
//			if ( tutor.CheckObject ( currentPlayer.acceptButton.gameObject ) )
//				tutor.NextMove ( );
//			else
//				return;
//		}
//
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Hide cancel buttons
//		currentPlayer.cancelButton.SetActive ( false );
//		
//		//Hide accept buttons
//		currentPlayer.acceptButton.gameObject.SetActive ( false );
//
//		//Hide prompt
//		currentPlayer.prompt.gameObject.SetActive ( false );
//
//		//Reset board
//		ResetBoardColor ( );
//
//		//Check ability
//		if ( abilityInUse.ID == Ability.AbilityList.Caboose.ID )
//		{
//			//Use Caboose ability
//			Ability.AbilityList.UseCaboose ( selectedCaboose, this );
//		}
//		else if ( abilityInUse.ID == Ability.AbilityList.MadHatter.ID )
//		{
//			//Use Mad Hatter ability
//			Ability.AbilityList.UseMadHatter ( abilityTileSelection [ 0 ], abilityTileSelection [ 1 ], this );
//		}
//		else if ( abilityInUse.ID == Ability.AbilityList.NonagressionPact.ID )
//		{
//			//Hide ability use button
//			currentAbilityButton.gameObject.SetActive ( false );
//
//			//Find ability
//			for ( int i = 0; i < currentPlayer.abilities.Length; i++ )
//			{
//				//Check for ability
//				if ( currentPlayer.abilities [ i ].ID != Ability.AbilityList.NonagressionPact.ID )
//					continue;
//				
//				//Make pact pieces visible
//				currentPlayer.p1NonagressionPieces [ i ].gameObject.SetActive ( true );
//				currentPlayer.p2NonagressionPieces [ i ].gameObject.SetActive ( true );
//				
//				//Display pact pieces
//				foreach ( Tile t in abilityTileSelection )
//				{
//					//Check color
//					switch ( t.currentPiece.color )
//					{
//						case PieceColor.Black:
//							//Check owner
//							if ( t.currentPiece.owner == player1 )
//								currentPlayer.p1NonagressionPieces [ i ].color = Piece.Display.Black;
//							else
//								currentPlayer.p2NonagressionPieces [ i ].color = Piece.Display.Black;
//							break;
//						case PieceColor.Blue:
//							//Check owner
//							if ( t.currentPiece.owner == player1 )
//								currentPlayer.p1NonagressionPieces [ i ].color = Piece.Display.Blue;
//							else
//								currentPlayer.p2NonagressionPieces [ i ].color = Piece.Display.Blue;
//							break;
//						case PieceColor.Green:
//							//Check owner
//							if ( t.currentPiece.owner == player1 )
//								currentPlayer.p1NonagressionPieces [ i ].color = Piece.Display.Green;
//							else
//								currentPlayer.p2NonagressionPieces [ i ].color = Piece.Display.Green;
//							break;
//						case PieceColor.Grey:
//							//Check owner
//							if ( t.currentPiece.owner == player1 )
//								currentPlayer.p1NonagressionPieces [ i ].color = Piece.Display.Grey;
//							else
//								currentPlayer.p2NonagressionPieces [ i ].color = Piece.Display.Grey;
//							break;
//						case PieceColor.Red:
//							//Check owner
//							if ( t.currentPiece.owner == player1 )
//								currentPlayer.p1NonagressionPieces [ i ].color = Piece.Display.Red;
//							else
//								currentPlayer.p2NonagressionPieces [ i ].color = Piece.Display.Red;
//							break;
//						case PieceColor.Yellow:
//							//Check owner
//							if ( t.currentPiece.owner == player1 )
//								currentPlayer.p1NonagressionPieces [ i ].color = Piece.Display.Yellow;
//							else
//								currentPlayer.p2NonagressionPieces [ i ].color = Piece.Display.Yellow;
//							break;
//					}
//				}
//				
//				//End search
//				break;
//			}
//
//			//Use Nonagression Pact ability
//			Ability.AbilityList.UseNonagressionPact ( abilityTileSelection [ 0 ], abilityTileSelection [ 1 ], this );
//		}
//	}
//
//	/// <summary>
//	/// Ends the player's turn voluntarily.
//	/// </summary>
//	public void OnEndTurnClick ( )
//	{
//		//Check for tutorial
//		if ( Tutorial.isPlaying )
//		{
//			//Check for current object
//			if ( !tutor.CheckObject ( currentPlayer.endButton ) )
//				return;
//		}
//
//		//Play SFX
//		if ( !( Tutorial.isPlaying && currentPlayer == player2 ) )
//			SFXManager.instance.Click ( );
//
//		//End the turn
//		EndTurn ( true );
//	}
//
//	/// <summary>
//	/// Ends the player's turn.
//	/// </summary>
//	public void EndTurn ( bool overrideSacrifice = false )
//	{
//		//Reset board
//		ResetBoardColor ( );
//
//		//Check for sacrifice
//		if ( !overrideSacrifice && Ability.AbilityList.HasSacrifice ( selectedPiece ) )
//		{
//			//Use sacrifice
//			Ability.AbilityList.UseSacrifice ( selectedPiece );
//
//			//Make the end turn button visible
//			currentPlayer.endButton.SetActive ( true );
//			
//			//Reselect piece
//			beginningOfTurn = true;
//			selectedPiece.currentTile.SelectPiece ( );
//
//			//Don't end the player's turn just yet
//			return;
//		}
//
//		//Clear selected piece
//		if ( selectedPiece != null )
//		{
//			selectedPiece.prevTile = null;
//			selectedPiece.sacrificeMoveRemaining = true;
//			selectedPiece = null;
//		}
//
//		//Check for jolt pieces
//		if ( joltList.Count > 0 )
//		{
//			//Hide end move button
//			currentPlayer.endButton.gameObject.SetActive ( true );
//
//			//Display prompt
//			currentPlayer.prompt.text = joltList [ 0 ].currentPiece.color.ToString ( ) + " has been given an additional move.";
//
//			//Highlight jolted piece
//			joltList [ 0 ].currentPiece.transform.DOKill ( );
//			joltList [ 0 ].currentPiece.Move ( joltList [ 0 ].transform.position );
//			joltList [ 0 ].HighlightSelectedPiece ( );
//
//			//Make it the beginning of the selected piece's turn
//			beginningOfTurn = true;
//
//			//Select the first piece on the jolt list
//			joltList [ 0 ].SelectPiece ( );
//
//			//Remove piece from the jolt list
//			joltList.RemoveAt ( 0 );
//
//			//Check if the jolt list is now empty
//			if ( joltList.Count == 0 )
//			{
//				//Change end turn button text
//				currentPlayer.endTurnText.text = "End Turn";
//			}
//
//			//Don't end the player's turn just yet
//			return;
//		}
//
//		//Check which player is ending their turn
//		if ( currentPlayer == player1 )
//		{
//			//Start player 2's turn
//			currentPlayer = player2;
//			opponent = player1;
//			turnText.text = "Player 2's\nTurn";
//		}
//		else
//		{
//			//Start player 1's turn
//			currentPlayer = player1;
//			opponent = player2;
//			turnText.text = "Player 1's\nTurn";
//		}
//
//		//Deactivate any preventive status from the player's piece
//		sacrificeArmorTile = null;
//		foreach ( Piece p in opponent.pieces )
//			p.isJolted = false;
//		
//		//Make end turn buttons invisible
//		opponent.endButton.SetActive ( false );
//		
//		//Hide the opponent's prompt
//		opponent.prompt.gameObject.SetActive ( false );
//		
//		//Start new turn
//		beginningOfTurn = true;
//
//		//Stop game clock
//		if ( !Tutorial.isPlaying && Settings.GameClock != 0 )
//			StopClock ( opponent );
//
//		//Check layout setting
//		if ( Settings.LayoutIsVertical )
//		{	
//			//Store rotation
//			Vector3 rot;
//
//			//Check current player
//			if ( currentPlayer == player1 )
//			{
//				//Rotate camera
//				rot = new Vector3 ( 0, 0, -90 );
//
//				//Display player 1 turn text
//				P1TurnText.gameObject.SetActive ( true );
//
//				//Hide player 2 turn text
//				p2TurnText.gameObject.SetActive ( false );
//			}
//			else
//			{
//				//Rotate camera
//				rot = new Vector3 ( 0, 0, 90 );
//
//				//Hide player 1 turn text
//				P1TurnText.gameObject.SetActive ( false );
//
//				//Display player 2 turn text
//				p2TurnText.gameObject.SetActive ( true );
//			}
//
//			//Rotate camera
//			cam.transform.DORotate ( rot, ANIMATE_TIME * 2 )
//				.SetRecyclable ( )
//				.OnComplete ( () =>
//				{
//					//Complete EndTurn ( )
//					EndTurnCallback ( );
//				} );
//
//			//Don't complete EndTurn ( ) just yet
//			return;
//		}
//
//		//Complete EndTurn ( )
//		EndTurnCallback ( );
//	}
//
//	/// <summary>
//	/// Completes EndTurn ( ) after any board rotating animation.
//	/// </summary>
//	private void EndTurnCallback ( )
//	{
//		//Move to the next turn in the tutorial
//		if ( Tutorial.isPlaying )
//			tutor.NextTurn ( );
//
//		//Start game clock
//		if ( !Tutorial.isPlaying && Settings.GameClock != 0 )
//			StartClock ( currentPlayer );
//
//		//Hide the opponent's ability buttons
//		for ( int i = 0; i < opponent.abilities.Length; i++ )
//		{
//			//Check for non-attached abilities
//			if ( !opponent.abilities [ i ].AttachesToPiece )
//				opponent.abilityButtons [ i ].gameObject.SetActive ( false );
//		}
//		
//		//Display the current player's ability buttons
//		for ( int i = 0; i < currentPlayer.abilities.Length; i++ )
//		{
//			//Check for non-attached abilities
//			if ( !currentPlayer.abilities [ i ].AttachesToPiece )
//			{
//				//Check for an inactive Nonagression Pact
//				if ( currentPlayer.abilities [ i ].ID == Ability.AbilityList.NonagressionPact.ID && !currentPlayer.abilities [ i ].IsActive )
//					continue;
//				
//				//Display use ability button
//				currentPlayer.abilityButtons [ i ].gameObject.SetActive ( true );
//			}
//		}
//		
//		//Enable the current player's ability buttons
//		EnableAbilityButtons ( );
//		
//		//Check if the current player has Grim Reaper
//		if ( Ability.AbilityList.HasGrimReaper ( currentPlayer ) )
//		{
//			//Check for a grim reaper tile
//			if ( grimReaperTile != null && grimReaperTile.currentPiece == null )
//			{
//				//Move the player's Grim Reaper piece
//				Ability.AbilityList.UseGrimReaper ( Ability.AbilityList.GetGrimReaper ( currentPlayer ), grimReaperTile, this );
//				
//				//Don't start the player's turn just yet
//				return;
//			}
//		}
//		
//		//Store temporary save data
//		TempSave ( );
//		
//		//Highlight pieces
//		HighlightCurrentPlayerPieces ( );
//	}
//
//	/// <summary>
//	/// Stores temporary save data from the beginning of a turn for in case the game gets saved that turn.
//	/// </summary>
//	public void TempSave ( )
//	{
//		//Save current player
//		if ( currentPlayer == player1 )
//			tempSaveIsP1Turn = true;
//		else
//			tempSaveIsP1Turn = false;
//		
//		//Save each of player 1's abilities
//		for ( int i = 0; i < player1.abilities.Length; i++ )
//		{
//			//Set ability ID
//			int id = player1.abilities [ i ].ID;
//			
//			//Set if the ability is active
//			int active = Convert.ToInt32 ( player1.abilities [ i ].IsActive );
//			
//			//Set which piece the ability is attached to
//			int piece = 0;
//			if ( player1.abilities [ i ].AttachesToPiece )
//				piece = (int)player1.abilities [ i ].AttachedPiece;
//			else if ( player1.abilities [ i ].ID == Ability.AbilityList.NonagressionPact.ID && !player1.abilities [ i ].IsActive )
//			{
//				if ( player1.p1NonagressionPieces [ i ].color == Piece.Display.Black )
//					piece = (int)PieceColor.Black * 10;
//				else if ( player1.p1NonagressionPieces [ i ].color == Piece.Display.Blue )
//					piece = (int)PieceColor.Blue * 10;
//				else if ( player1.p1NonagressionPieces [ i ].color == Piece.Display.Red )
//					piece = (int)PieceColor.Red * 10;
//				else if ( player1.p1NonagressionPieces [ i ].color == Piece.Display.Green )
//					piece = (int)PieceColor.Green * 10;
//				else if ( player1.p1NonagressionPieces [ i ].color == Piece.Display.Yellow )
//					piece = (int)PieceColor.Yellow * 10;
//				else if ( player1.p1NonagressionPieces [ i ].color == Piece.Display.Grey )
//					piece = (int)PieceColor.Grey * 10;
//				
//				if ( player1.p2NonagressionPieces [ i ].color == Piece.Display.Black )
//					piece += (int)PieceColor.Black;
//				else if ( player1.p2NonagressionPieces [ i ].color == Piece.Display.Blue )
//					piece += (int)PieceColor.Blue;
//				else if ( player1.p2NonagressionPieces [ i ].color == Piece.Display.Red )
//					piece += (int)PieceColor.Red;
//				else if ( player1.p2NonagressionPieces [ i ].color == Piece.Display.Green )
//					piece += (int)PieceColor.Green;
//				else if ( player1.p2NonagressionPieces [ i ].color == Piece.Display.Yellow )
//					piece += (int)PieceColor.Yellow;
//				else if ( player1.p2NonagressionPieces [ i ].color == Piece.Display.Grey )
//					piece += (int)PieceColor.Grey;
//			}
//			
//			//Save ability
//			tempSaveP1Abilities [ i ] = new Vector3 ( (float)id, (float)active, (float)piece );
//		}
//
//		//Save sacrifice pieces
//		if ( player1Sacrifice != null )
//			tempSaveP1Sacrifice = (int)player1Sacrifice;
//		else
//			tempSaveP1Sacrifice = 0;
//		
//		//Save each of player 2's abilities
//		for ( int i = 0; i < player2.abilities.Length; i++ )
//		{
//			//Set ability ID
//			int id = player2.abilities [ i ].ID;
//			
//			//Set if the ability is active
//			int active = Convert.ToInt32 ( player2.abilities [ i ].IsActive );
//			
//			//Set which piece the ability is attached to
//			int piece = 0;
//			if ( player2.abilities [ i ].AttachesToPiece )
//				piece = (int)player2.abilities [ i ].AttachedPiece;
//			else if ( player2.abilities [ i ].ID == Ability.AbilityList.NonagressionPact.ID && !player2.abilities [ i ].IsActive )
//			{
//				if ( player2.p1NonagressionPieces [ i ].color == Piece.Display.Black )
//					piece = (int)PieceColor.Black * 10;
//				else if ( player2.p1NonagressionPieces [ i ].color == Piece.Display.Blue )
//					piece = (int)PieceColor.Blue * 10;
//				else if ( player2.p1NonagressionPieces [ i ].color == Piece.Display.Red )
//					piece = (int)PieceColor.Red * 10;
//				else if ( player2.p1NonagressionPieces [ i ].color == Piece.Display.Green )
//					piece = (int)PieceColor.Green * 10;
//				else if ( player2.p1NonagressionPieces [ i ].color == Piece.Display.Yellow )
//					piece = (int)PieceColor.Yellow * 10;
//				else if ( player2.p1NonagressionPieces [ i ].color == Piece.Display.Grey )
//					piece = (int)PieceColor.Grey * 10;
//				
//				if ( player2.p2NonagressionPieces [ i ].color == Piece.Display.Black )
//					piece += (int)PieceColor.Black;
//				else if ( player2.p2NonagressionPieces [ i ].color == Piece.Display.Blue )
//					piece += (int)PieceColor.Blue;
//				else if ( player2.p2NonagressionPieces [ i ].color == Piece.Display.Red )
//					piece += (int)PieceColor.Red;
//				else if ( player2.p2NonagressionPieces [ i ].color == Piece.Display.Green )
//					piece += (int)PieceColor.Green;
//				else if ( player2.p2NonagressionPieces [ i ].color == Piece.Display.Yellow )
//					piece += (int)PieceColor.Yellow;
//				else if ( player2.p2NonagressionPieces [ i ].color == Piece.Display.Grey )
//					piece += (int)PieceColor.Grey;
//			}
//			
//			//Save ability
//			tempSaveP2Abilities [ i ] = new Vector3 ( (float)id, (float)active, (float)piece );
//		}
//
//		//Save sacrifice pieces
//		if ( player2Sacrifice != null )
//			tempSaveP2Sacrifice = (int)player2Sacrifice;
//		else
//			tempSaveP2Sacrifice = 0;
//		
//		//Save the position of each player's pieces
//		for ( int i = 0; i < player1.pieces.Count; i++ )
//		{
//			//Set piece id
//			int piece = (int)player1.pieces [ i ].color;
//			
//			//Set tile id
//			int tile = player1.pieces [ i ].currentTile.ID;
//			
//			//Save piece
//			tempSavePieces [ i ] = new Vector3 ( 1, (float)piece, (float)tile );
//		}
//		for ( int i = 0; i < player2.pieces.Count; i++ )
//		{
//			//Set piece id
//			int piece = (int)player2.pieces [ i ].color;
//			
//			//Set tile id
//			int tile = player2.pieces [ i ].currentTile.ID;
//			
//			//Save piece
//			tempSavePieces [ player1.pieces.Count + i ] = new Vector3 ( 2, (float)piece, (float)tile );
//		}
//		for ( int i = player1.pieces.Count + player2.pieces.Count; i < tempSavePieces.Length; i++ )
//			tempSavePieces [ i ] = Vector3.zero;
//
//		//Save each player's game clock
//		if ( !Tutorial.isPlaying && Settings.GameClock != 0 )
//		{
//			tempSaveP1GameClock = player1.gameClock;
//			tempSaveP2GameClock = player2.gameClock;
//		}
//		else
//		{
//			tempSaveP1GameClock = 0;
//			tempSaveP2GameClock = 0;
//		}
//	}
//
//	/// <summary>
//	/// Starts the player's game clock.
//	/// </summary>
//	private void StartClock ( Player p )
//	{
//		//Start clock
//		p.clockIsActive = true;
//		p.gameClockDisplay.color = new Color32 ( 150, 255, 255, 255 );
//	}
//
//	/// <summary>
//	/// Stops the player's game clock.
//	/// </summary>
//	private void StopClock ( Player p )
//	{
//		//Stop clock
//		p.clockIsActive = false;
//		p.gameClockDisplay.color = new Color32 ( 200, 200, 200, 255 );
//	}
//
//	/// <summary>
//	/// Brings the piece to the front of the render order.
//	/// </summary>
//	public void BringPieceToFront ( Piece piece )
//	{
//		//Set player 1's pieces to the default render order
//		foreach ( Piece p in player1.pieces )
//		{
//			//Bring selected piece to front
//			if ( p == piece )
//				p.sprite.sortingOrder = 2;
//			else
//				p.sprite.sortingOrder = 1;
//		}
//
//		//Set player 2's pieces to the default render order
//		foreach ( Piece p in player2.pieces )
//		{
//			//Bring selected piece to front
//			if ( p == piece )
//				p.sprite.sortingOrder = 2;
//			else
//				p.sprite.sortingOrder = 1;
//		}
//	}
//
//	/// <summary>
//	/// Wins the game for the current player and loads the win game prompt.
//	/// </summary>
//	public void WinGame ( )
//	{
//		//End the game
//		isGameOver = true;
//
//		//Change audio
//		MusicManager.instance.ChangeMusic ( AudioContext.Results );
//
//		//Stop clocks
//		if ( !Tutorial.isPlaying && Settings.GameClock != 0 )
//			StopClock ( currentPlayer );
//
//		//Display prompt
//		if ( currentPlayer == player1 )
//			turnText.text = "Player 1\nWins";
//		else
//			turnText.text = "Player 2\nWins";
//
//		//Initialize animation
//		Sequence s = DOTween.Sequence ( );
//		
//		//Make tiles ripple
//		for ( int i = 0; i < board.Count; i++ )
//			s.Insert ( WAIT_TIME + ( board [ i ].wave * DELAY_TIME * WAVE_TIME ), board [ i ].transform.DOPunchScale ( new Vector3 ( -1, -1, -1 ), WAVE_TIME, 1, 1 ) );
//
//		//Make win panel appear
//		s.AppendCallback ( () => 
//			{
//				//Open win game UI
//				popupPanel.gameObject.SetActive ( true );
//				winPanel.gameObject.SetActive ( true );
//				pausePanel.SetActive ( false );
//				settingsPanel.SetActive ( false );
//				
//				//Check winner
//				if ( currentPlayer == player1 )
//					winPrompt.text = "Player 1\nWins";
//				else
//					winPrompt.text = "Player 2\nWins";
//
//				//Make win panel transparent
//				winPanel.color       = new Color32 ( 200, 200, 200,   0 );
//				rematchButton.color  = new Color32 ( 255, 255, 255,   0 );
//				rematchText.color    = new Color32 (  50,  50,  50,   0 );
//				mainMenuButton.color = new Color32 ( 255, 255, 255,   0 );
//				mainMenuText.color   = new Color32 (  50,  50,  50,   0 );
//			} )
//			.Append ( popupPanel.DOFade ( 0, FADE_TIME ).From ( ) )
//			.Append ( winPanel.DOFade ( 1, SLIDE_TIME ) )
//			.Insert ( EXIT_TIME, winPanel.rectTransform.DOLocalMoveY ( winPanel.rectTransform.rect.height / 2, SLIDE_TIME ).From ( ) )
//			.Append ( rematchButton.DOFade ( 1, FADE_TIME ) )
//			.Insert ( EXIT_TIME + SLIDE_TIME, rematchText.DOFade ( 1, FADE_TIME ) )
//			.Insert ( EXIT_TIME + SLIDE_TIME, mainMenuButton.DOFade ( 1, FADE_TIME ) )
//			.Insert ( EXIT_TIME + SLIDE_TIME, mainMenuText.DOFade ( 1, FADE_TIME ) )
//			.Play ( );
//	}
//
//	/// <summary>
//	/// Loads a rematch of the game.
//	/// </summary>
//	public void Rematch ( )
//	{
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Restart the match
//		MusicManager.instance.ChangeMusic ( AudioContext.Gameplay );
//
//		//Check for tutorial
//		if ( Tutorial.isPlaying && Tutorial.isGame1 && isGameOver )
//			Tutorial.isGame1 = false;
//
//		//Fade out
//		exitFade.gameObject.SetActive ( true );
//		exitFade.DOFade ( 0, EXIT_TIME ).From ( )
//			.OnComplete ( () =>
//			{
//				//Load main menu
//				Application.LoadLevel ( "Game Board" );
//			} );
//	}
//
//	/// <summary>
//	/// Loads the main menu.
//	/// </summary>
//	public void MainMenu ( )
//	{
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Clear tutorial information
//		Tutorial.isPlaying = false;
//
//		//Wipe abilities
//		for ( int i = 0; i < player1AbilityList.Length; i++ )
//		{
//			player1AbilityList [ i ] = null;
//			player2AbilityList [ i ] = null;
//		}
//
//		//Clear any sacrifice pieces
//		player1Sacrifice = PieceColor.None;
//		player2Sacrifice = PieceColor.None;
//
//		//Change music
//		MusicManager.instance.ChangeMusic ( AudioContext.MainMenu );
//
//		//Fade out
//		exitFade.gameObject.SetActive ( true );
//		exitFade.DOFade ( 0, EXIT_TIME ).From ( )
//			.OnComplete ( () =>
//			{
//				//Load main menu
//				Application.LoadLevel ( "Main Menu" );
//			} );
//	}
//
//	/// <summary>
//	/// Opens the pause menu.
//	/// </summary>
//	private void PauseGame ( )
//	{
//		//Pause animations
//		DOTween.TogglePauseAll ( );
//
//		//Display pause menu
//		popupPanel.gameObject.SetActive ( true );
//		winPanel.gameObject.SetActive ( false );
//		pausePanel.SetActive ( true );
//		settingsPanel.SetActive ( false );
//		if ( !Tutorial.isPlaying )
//		{
//			saveButton.interactable = true;
//			savePrompt.text = "Save Game";
//		}
//
//		//Stop clock
//		if ( !Tutorial.isPlaying && Settings.GameClock != 0 )
//			StopClock ( currentPlayer );
//	}
//
//	/// <summary>
//	/// Closes the pause menu.
//	/// </summary>
//	public void ResumeGame ( ) 
//	{
//		//Play SFX
//		ResumeGame ( true );
//	}
//
//	/// <summary>
//	/// Closes the pause menu.
//	/// </summary>
//	public void ResumeGame ( bool playSFX ) 
//	{
//		//Play SFX
//		if ( playSFX )
//			SFXManager.instance.Click ( );
//
//		//Play animations
//		DOTween.TogglePauseAll ( );
//
//		//Close pause menu
//		pausePanel.SetActive ( false );
//		popupPanel.gameObject.SetActive ( false );
//		
//		//Start clock
//		if ( !Tutorial.isPlaying && Settings.GameClock != 0 )
//			StartClock ( currentPlayer );
//	}
//
//	/// <summary>
//	/// Saves the game.
//	/// </summary>
//	public void SaveGame ( )
//	{
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Save the player's turn
//		Settings.SaveDataIsP1Turn = tempSaveIsP1Turn;
//		PlayerPrefsX.SetBool ( "playerTurn", Settings.SaveDataIsP1Turn );
//
//		//Save player 1's abilities
//		Settings.SaveDataP1Abilities = tempSaveP1Abilities;
//		PlayerPrefsX.SetVector3Array ( "player1Abilities", Settings.SaveDataP1Abilities );
//
//		//Save player 2's abilities
//		Settings.SaveDataP2Abilities = tempSaveP2Abilities;
//		PlayerPrefsX.SetVector3Array ( "player2Abilities", Settings.SaveDataP2Abilities );
//
//		//Save the positions of the pieces
//		Settings.SaveDataPieces = tempSavePieces;
//		PlayerPrefsX.SetVector3Array ( "pieces", Settings.SaveDataPieces );
//
//		//Save sacrifice pieces
//		Settings.SaveDataP1Sacrifice = tempSaveP1Sacrifice;
//		PlayerPrefs.SetInt ( "player1Sacrifice", Settings.SaveDataP1Sacrifice );
//		Settings.SaveDataP2Sacrifice = tempSaveP2Sacrifice;
//		PlayerPrefs.SetInt ( "player2Sacrifice", Settings.SaveDataP2Sacrifice );
//
//		//Save the game clock
//		Settings.SaveDataGameClock = Settings.GameClock;
//		PlayerPrefs.SetInt ( "saveGameClock", Settings.SaveDataGameClock );
//		Settings.SaveDataP1GameClock = tempSaveP1GameClock;
//		PlayerPrefs.SetFloat ( "player1GameClock", Settings.SaveDataP1GameClock );
//		Settings.SaveDataP2GameClock = tempSaveP2GameClock;
//		PlayerPrefs.SetFloat ( "player2GameClock", Settings.SaveDataP2GameClock );
//
//		//Display successful save
//		saveButton.interactable = false;
//		savePrompt.text = "Game Saved";
//	}
//
//	/// <summary>
//	/// Opens the settings menu.
//	/// </summary>
//	public void SettingsMenu ( )
//	{
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Hide the pause menu
//		pausePanel.SetActive ( false );
//
//		//Display the settings menu
//		settingsPanel.SetActive ( true );
//	}
//
//	/// <summary>
//	/// Sets the board layout to horizontal.
//	/// </summary>
//	public void OnHorizontalSettingClick ( )
//	{
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Display center turn text
//		turnText.gameObject.SetActive ( true );
//
//		//Hide side turn text
//		P1TurnText.gameObject.SetActive ( false );
//		p2TurnText.gameObject.SetActive ( false );
//
//		//Set camera
//		cam.transform.position = new Vector3 ( 0, 1, cam.transform.position.z );
//		if ( Settings.LayoutIsVertical )
//		{
//			cam.GetComponent<CameraFit> ( ).adjustForVertical = false;
//			cam.orthographicSize -= 3;
//		}
//		//Rotate camera
//		cam.transform.eulerAngles = Vector3.zero;
//
//		//Set board layout as horizontal
//		Settings.LayoutIsVertical = false;
//		
//		//Save setting
//		PlayerPrefsX.SetBool ( "boardLayout", Settings.LayoutIsVertical );
//	}
//	
//	/// <summary>
//	/// Sets the board layout to vertical.
//	/// </summary>
//	public void OnVerticalSettingClick ( )
//	{
//		//Play SFX
//		if ( !isSetUp )
//			SFXManager.instance.Click ( );
//
//		//Hide center turn text
//		turnText.gameObject.SetActive ( false );
//
//		//Set camera
//		cam.transform.position = new Vector3 ( 0, 0, cam.transform.position.z );
//		if ( !Settings.LayoutIsVertical || isSetUp )
//		{
//			cam.GetComponent<CameraFit> ( ).adjustForVertical = true;
//			cam.orthographicSize += 3;
//		}
//		//Rotate camera
//		if ( currentPlayer == player1 )
//		{
//			//Rotate camera
//			cam.transform.eulerAngles = new Vector3 ( 0, 0, -90 );
//
//			//Display side turn text
//			P1TurnText.gameObject.SetActive ( true );
//		}
//		else
//		{
//			//Rotate camera
//			cam.transform.eulerAngles = new Vector3 ( 0, 0, 90 );
//
//			//Display side turn text
//			p2TurnText.gameObject.SetActive ( true );
//		}
//
//		//Set board layout as vertical
//		Settings.LayoutIsVertical = true;
//		
//		//Save setting
//		PlayerPrefsX.SetBool ( "boardLayout", Settings.LayoutIsVertical );
//	}
//	
//	/// <summary>
//	/// Sets the music volume.
//	/// </summary>
//	public void OnMusicSettingChange ( float value )
//	{
//		//Update slider
//		musicSlider.value = value;
//		
//		//Display music volume
//		musicValue.text = value + "%";
//		
//		//Set music volume
//		Settings.MusicVolume = value / 100;
//		MusicManager.instance.UpdateMusicVolume ( );
//		
//		//Save setting
//		PlayerPrefs.SetFloat ( "musicVolume", Settings.MusicVolume );
//	}
//	
//	/// <summary>
//	/// Sets the sound volume.
//	/// </summary>
//	public void OnSoundSettingChange ( float value )
//	{
//		//Update slider
//		soundSlider.value = value;
//		
//		//Display sound volume
//		soundValue.text = value + "%";
//		
//		//Set sound volume
//		Settings.SoundVolume = value / 100;
//		SFXManager.instance.UpdateSFXVolume ( );
//		
//		//Save setting
//		PlayerPrefs.SetFloat ( "soundVolume", Settings.SoundVolume );
//	}
//
//	/// <summary>
//	/// Closes the settings menu and returns the pause menu.
//	/// </summary>
//	public void OnBackButtonClick ( )
//	{
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Hide the settings menu
//		settingsPanel.SetActive ( false );
//
//		//Display the pause menu
//		pausePanel.SetActive ( true );
//	}
}
