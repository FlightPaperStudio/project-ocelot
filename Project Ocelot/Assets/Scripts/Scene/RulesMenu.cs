using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class RulesMenu : MonoBehaviour 
{
//	//Menu panels
//	public RectTransform rulesPanel;
//	public RectTransform abilitiesPanel;
//	public CanvasGroup infoPanel;
//	public CanvasGroup settingsPanel;
//
//	//Menu game objects
//	public Button [ ] menuButtons;
//	public Text titleText;
//	public Text descText;
//	public GameObject abilityButton;
//	public GameObject movementTutorial;
//	public GameObject abilityTutorial;
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
//	//Ability menu game objects
//	public Button [ ] abilityButtons;
//	public Scrollbar scroll;
//
//	//Button colors
//	private ColorBlock selected;
//	private ColorBlock unselected;
//
//	//Menu text
//	private string [ ] titles = new string [ 6 ];
//	private string [ ] descs = new string [ 6 ];
//
//	//Ability information
//	private Dictionary < Button, Ability > selectedAbility = new Dictionary < Button, Ability > ( );
//
//	//Animation information
//	private const float SLIDE_TIME = 1f;
//	private const float FADE_TIME = 0.4f;
//	private bool allowInput = true;
//	private enum MenuDestinations
//	{
//		None,
//		RulesToAbilites,
//		AbilitiesToRules,
//		Tutorial
//	};
//
//	/// <summary>
//	/// Initializes the How to Play Menu.
//	/// </summary>
//	private void Start ( )
//	{
//		//Set selected/unselected colors
//		selected = menuButtons [ 0 ].colors;
//		selected.normalColor = new Color32 ( 255, 210, 75, 255 );
//		unselected = menuButtons [ 0 ].colors;
//		unselected.normalColor = new Color32 ( 255, 255, 200, 255 );
//
//		//Store abilities
//		selectedAbility.Add ( abilityButtons [ 0 ], Ability.AbilityList.Armor );
//		selectedAbility.Add ( abilityButtons [ 1 ], Ability.AbilityList.Caboose );
//		selectedAbility.Add ( abilityButtons [ 2 ], Ability.AbilityList.Catapult );
//		selectedAbility.Add ( abilityButtons [ 3 ], Ability.AbilityList.Conversion );
//		selectedAbility.Add ( abilityButtons [ 4 ], Ability.AbilityList.GrimReaper );
//		selectedAbility.Add ( abilityButtons [ 5 ], Ability.AbilityList.Jolt );
//		selectedAbility.Add ( abilityButtons [ 6 ], Ability.AbilityList.MadHatter );
//		selectedAbility.Add ( abilityButtons [ 7 ], Ability.AbilityList.NonagressionPact );
//		selectedAbility.Add ( abilityButtons [ 8 ], Ability.AbilityList.Pacifist );
//		selectedAbility.Add ( abilityButtons [ 9 ], Ability.AbilityList.Sacrifice );
//		selectedAbility.Add ( abilityButtons [ 10 ], Ability.AbilityList.Teleport );
//		selectedAbility.Add ( abilityButtons [ 11 ], Ability.AbilityList.Torus );
//
//		//Store prompts
//		InitText ( );
//
//		//Select Overview menu first
//		OnMenuClick ( menuButtons [ 0 ], false );
//
//		//Play intro
//		AnimateIntro ( rulesPanel, infoPanel );
//	}
//
//	/// <summary>
//	/// Loads the menu texts.
//	/// </summary>
//	private void InitText ( )
//	{
//		//Load overview
//		titles [ 0 ] = "Overview";
//		descs [ 0 ] = "Evasion is a Checkers variant designed to focus on fast-paced, asymmetric play. Each game has players selecting an assortment of abilities that will allow them to drastically modify the rules in their favor. As players move their pieces across the board, abilities can be used to teleport pieces, chain together multiple moves at once, or even steal an opponent's piece for their own use, just to name a few. With hundreds of different ability combinations, Evasion allows every game to be an interesting and unique experience and requires players to both think on their feet as well as develop their own distinct styles and strategies.";
//		
//		//Load objective
//		titles [ 1 ] = "Objective";
//		descs [ 1 ] = "A player can win in one of two ways. If a player captures the opponent's white piece or if a player's white piece reaches the last three rows of tiles on the opposite end of the board, the player will win the game.";
//		
//		//Load movement
//		titles [ 2 ] = "Movement";
//		descs [ 2 ] = "A player is allowed to move one piece per turn. A piece can move in a number of difference ways but must obey two fundemental rules. Pieces cannot move backwards, and pieces can only move to tiles that are unoccupied by any other piece. When a player selects a piece to move, any tiles that piece can move to will be highlighted blue.\nFirst, a piece can move to any adjacent, unoccupied tile. Once a piece has moved to an adjacent tile, the player's turn is over.\nSecond, if an adjacent tile is occupied by a piece and the next tile in that direction is unoccupied, a piece can jump the adjacent piece and land on the unoccupied tile (the same as in Checkers). After a jump, the piece is allowed to continue jumping pieces if an additional jump is available to the piece. A player's turn ends when the piece has no more jumps available or when the player chooses to stop moving.\nA piece is allowed to jump both the player's pieces and the opponent's pieces. If the player jumps an opponent's piece, the opponent's piece is captured and removed from the board. Tiles occupied by an opponent's piece that the player's selected piece can capture will be highlighted red to indicate a potential capture.";
//
//		//Load abilities
//		titles [ 3 ] = "Abilities";
//		descs [ 3 ] = "At the beginning of the game, each player chooses three abilities to use for that game. Each ability changes the rules of the game in ways that benefit the player.\nSome abilities are assigned to a singular piece. Pieces can only be granted one ability per game, however, the player's white piece is not allowed to have any abilities. When a piece is granted an ability, only that piece is allowed to use the ability. If that piece is captured, the ability is disabled, and the player can no longer use the ability. If the ability affects the piece's movement, any tiles the piece can move to using the ability will be highlighted purple when the piece is selected to move.\nOther abilities are not assigned to a singular piece. These global abilities can be used on multiple pieces. These abilities must be used at the beginning of a turn before any pieces have moved. Some of these abilities can only be used once per game, and once they are used, the ability is disabled.\nBoth of the players' chosen abilities are always displayed on screen. A player can hover the mouse over the name of an ability at any time to view the ability's description. When an ability is disabled, the name of the ability will turn grey to indicate that the player can no longer use the ability. If an ability is assigned to a specific piece, the color of the piece will be displayed next to the name of the ability. If an ability is a global ability, a Use Button will be displayed next to the name of the ability. The player can click the Use Button to activate the ability on that turn. A grey Use Button indicates that the ability is not available to use at that time.";
//
//		//Load credits
//		titles [ 4 ] = "Tutorial";
//		descs [ 4 ] = "";
//
//		//Load credits
//		titles [ 5 ] = "Credits";
//		descs [ 5 ] = "Producer\n<color=#c8c8c8>Ethan Caraway</color>\n\nDesigners\n<color=#c8c8c8>Jesse Clifton\nElijah Cohen\nJames Krause</color>\n\nProgrammer\n<color=#c8c8c8>Ethan Caraway</color>\n\nAudio\n<color=#c8c8c8>Ryan Travis</color>\n\nSpecial Thanks\n<color=#c8c8c8>Elizabeth Clausen\nDemigiant\nRay Larabie\nOffice57\nUnity</color>";
//	}
//
//	/// <summary>
//	/// Plays the intro animation for the menu.
//	/// </summary>
//	private void AnimateIntro ( RectTransform sliding, CanvasGroup fading )
//	{
//		//Disable input
//		allowInput = false;
//
//		//Set starting values
//		sliding.anchoredPosition = new Vector2 ( -sliding.rect.width, 0 );
//		fading.alpha = 0;
//
//		//Start animation
//		Sequence s = DOTween.Sequence ( )
//			.Append ( sliding.DOAnchorPos ( Vector2.zero, SLIDE_TIME ) )
//			.Append ( fading.DOFade ( 1, FADE_TIME ) )
//			.OnComplete ( () =>
//			{
//				//Enable input
//				allowInput = true;
//			} )
//			.SetRecyclable ( )
//			.Play ( );
//	}
//
//	/// <summary>
//	/// Plays the outro for the menu.
//	/// </summary>
//	private void AnimateOutro ( RectTransform sliding, CanvasGroup fading, MenuDestinations d )
//	{
//		//Disable input
//		allowInput = false;
//		
//		//Start animation
//		Sequence s = DOTween.Sequence ( )
//			.Append ( fading.DOFade ( 0, FADE_TIME ) )
//			.Append ( sliding.DOAnchorPos ( new Vector2 ( -sliding.rect.width, 0 ), SLIDE_TIME ) )
//			.OnComplete ( () =>
//			{
//				//Enable input
//				allowInput = true;
//
//				//Check transistion
//				switch ( d )
//				{
//					case MenuDestinations.RulesToAbilites:
//						//Hide button
//						abilityButton.SetActive ( false );
//						
//						//Hide menu panel
//						rulesPanel.gameObject.SetActive ( false );
//						
//						//Display ability panel
//						abilitiesPanel.gameObject.SetActive ( true );
//
//						//Set scroll panel to the top
//						scroll.value = 1;
//						
//						//Display first ability
//						OnAbilityClick ( abilityButtons [ 0 ], false );
//
//						//Play intro
//						AnimateIntro ( abilitiesPanel, fading );
//						break;
//					case MenuDestinations.AbilitiesToRules:
//						//Hide ability panel
//						abilitiesPanel.gameObject.SetActive ( false );
//						
//						//Display menu panel
//						rulesPanel.gameObject.SetActive ( true );
//						
//						//Display overview
//						OnMenuClick ( menuButtons [ 3 ], false );
//
//						//Play intro
//						AnimateIntro ( rulesPanel, fading );
//						break;
//					case MenuDestinations.Tutorial:
//						Application.LoadLevel ( "Game Board" );
//						break;
//					case MenuDestinations.None:
//						Application.LoadLevel ( "Main Menu" );
//						break;
//				}
//			} )
//			.SetRecyclable ( )
//			.Play ( );
//	}
//
//	/// <summary>
//	/// Opens a menu in the How To Play menu screen.
//	/// </summary>
//	public void OnMenuClick ( Button b )
//	{
//		//Check input
//		if ( allowInput )
//			OnMenuClick ( b, true );
//	}
//
//	/// <summary>
//	/// Opens a menu in the How To Play menu screen.
//	/// </summary>
//	private void OnMenuClick ( Button b, bool playSFX )
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
//		//Search for selected button
//		for ( int i = 0; i < menuButtons.Length; i++ )
//		{
//			//Check button
//			if ( menuButtons [ i ] != b )
//				continue;
//
//			//Check if the settings button is selected
//			if ( i == 6 )
//			{
//				//Hide the info panel
//				infoPanel.gameObject.SetActive ( false );
//
//				//Display the settings panel
//				settingsPanel.gameObject.SetActive ( true );
//
//				//Check layout settings
//				if ( Settings.LayoutIsVertical )
//				{
//					//Set toggle to vertical
//					ExecuteEvents.Execute ( verticalLayout.gameObject, new PointerEventData ( EventSystem.current ), ExecuteEvents.pointerClickHandler );
//				}
//				else
//				{
//					//Set toggle to horizontal
//					ExecuteEvents.Execute ( horizontalLayout.gameObject, new PointerEventData ( EventSystem.current ), ExecuteEvents.pointerClickHandler );
//				}
//
//				//Load music volume
//				OnMusicSettingChange ( Settings.MusicVolume * 100 );
//
//				//Load sound volume
//				OnSoundSettingChange ( Settings.SoundVolume * 100 );
//
//				//Load game clock start time
//				float startTime = 0;
//				if ( Settings.GameClock != 0 )
//					startTime = ( 35 - Settings.GameClock ) / 5;
//				OnTimerSettingChange ( startTime );
//			}
//			else
//			{
//				//Display the info panel
//				infoPanel.gameObject.SetActive ( true );
//
//				//Hide the settings panel
//				settingsPanel.gameObject.SetActive ( false );
//
//				//Load menu title
//				titleText.text = titles [ i ];
//
//				//Load menu description
//				descText.text = descs [ i ];
//
//				//Check if the ability button is selected
//				if ( i == 3 )
//				{
//					//Make the ability button visible
//					abilityButton.SetActive ( true );
//					movementTutorial.SetActive ( false );
//					abilityTutorial.SetActive ( false );
//				}
//				else if ( i == 4 )
//				{
//					//Make the tutorial buttons visible
//					abilityButton.SetActive ( false );
//					movementTutorial.SetActive ( true );
//					abilityTutorial.SetActive ( true );
//				}
//				else
//				{
//					//Make the buttons invisible
//					abilityButton.SetActive ( false );
//					movementTutorial.SetActive ( false );
//					abilityTutorial.SetActive ( false );
//				}
//
//				//Finish search
//				break;
//			}
//		}
//	}
//
//	/// <summary>
//	/// Exits the How to Play menu screen.
//	/// </summary>
//	public void OnExitClick ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.Click ( );
//
//			//Change music
//			MusicManager.instance.ChangeMusic ( AudioContext.MainMenu );
//
//			//Play outro
//			if ( infoPanel.gameObject.activeSelf )
//				AnimateOutro ( rulesPanel, infoPanel, MenuDestinations.None );
//			else
//				AnimateOutro ( rulesPanel, settingsPanel, MenuDestinations.None );
//		}
//	}
//
//	/// <summary>
//	/// Loads the list of abilities.
//	/// </summary>
//	public void OnViewAbilitiesClick ( )
//	{
//		//Play outro
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.Click ( );
//
//			//Transition to view abilities
//			AnimateOutro ( rulesPanel, infoPanel, MenuDestinations.RulesToAbilites );
//		}
//	}
//
//	/// <summary>
//	/// Displays the desciption for the selected ability.
//	/// </summary>
//	public void OnAbilityClick ( Button b )
//	{
//		//Check input
//		if ( allowInput )
//			OnAbilityClick ( b, true );	
//	}
//
//	/// <summary>
//	/// Displays the desciption for the selected ability.
//	/// </summary>
//	private void OnAbilityClick ( Button b, bool playSFX )
//	{
//		//Play SFX
//		if ( playSFX )
//			SFXManager.instance.Click ( );
//
//		//Unselect any previously selected button
//		foreach ( Button a in abilityButtons )
//			a.colors = unselected;
//		
//		//Select current button
//		b.colors = selected;
//
//		//Load ability name
//		titleText.text = selectedAbility [ b ].Name;
//
//		//Load ability description
//		descText.text = selectedAbility [ b ].Desc;
//	}
//
//	/// <summary>
//	/// Return to the How to Play menu.
//	/// </summary>
//	public void OnBackClick ( )
//	{
//		//Play outro
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.Click ( );
//
//			//Transition to how to play
//			AnimateOutro ( abilitiesPanel, infoPanel, MenuDestinations.AbilitiesToRules );
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
//			AnimateOutro ( rulesPanel, infoPanel, MenuDestinations.Tutorial );
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
//			AnimateOutro ( rulesPanel, infoPanel, MenuDestinations.Tutorial );
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
