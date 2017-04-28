using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SelectMenu : MonoBehaviour 
{
//	//UI game object
//	public GameObject canvasUI;
//	public GameObject canvasLoading;
//
//	//Side bar game objects
//	public RectTransform sideBar;
//	public Text sideBarPrompt;
//	public Button [ ] sideBarButtons;
//	public Scrollbar scroll;
//	public Button autoAssignButton;
//	public Button undoButton;
//
//	//Player abilities game objects
//	public CanvasGroup playerPanel;
//	public Text [ ] player1Abilities;
//	public Text [ ] player2Abilities;
//	public Image [ ] player1Pieces;
//	public Image [ ] player2Pieces;
//	public Image [ ] player1Highlights;
//	public Image [ ] player2Highlights;
//
//	//Ability description game objects
//	public CanvasGroup abilityPanel;
//	public Text abilityName;
//	public Text abilityDesc;
//
//	//Piece selection game objects
//	public CanvasGroup piecePanel;
//	public Text piecePrompt;
//	public Button [ ] pieceButtons;
//
//	//Ability confirmation game objects
//	public CanvasGroup confirmPanel;
//	public GameObject reassignButton;
//
//	//Ability information
//	private Dictionary < Button, Ability > abilitySelection = new Dictionary < Button, Ability > ( );
//	private Ability selectedAbility;
//	private PieceColor selectedPiece;
//	private bool isPlayer1 = true;
//	private int currentSlot = 0;
//	private Dictionary < Button, PieceColor > buttonColors = new Dictionary < Button, PieceColor > ( );
//	private Button selectedButton;
//	private int autoAssignAmount;
//
//	//Button colors
//	private ColorBlock selected;
//	private ColorBlock unselected;
//
//	//Animaiton information
//	private const float SLIDE_TIME = 1f;
//	private const float FADE_TIME = 0.4f;
//	private bool allowInput = true;
//	private enum MenuDestination
//	{
//		GameBoard,
//		MainMenu
//	};
//
//	/// <summary>
//	/// Load the Select Ability Menu.
//	/// </summary>
//	private void Start ( ) 
//	{
//		//Pair the ability select buttons with abilities
//		abilitySelection.Add ( sideBarButtons [ 0 ], Ability.AbilityList.Armor );
//		abilitySelection.Add ( sideBarButtons [ 1 ], Ability.AbilityList.Caboose );
//		abilitySelection.Add ( sideBarButtons [ 2 ], Ability.AbilityList.Catapult );
//		abilitySelection.Add ( sideBarButtons [ 3 ], Ability.AbilityList.Conversion );
//		abilitySelection.Add ( sideBarButtons [ 4 ], Ability.AbilityList.GrimReaper );
//		abilitySelection.Add ( sideBarButtons [ 5 ], Ability.AbilityList.Jolt );
//		abilitySelection.Add ( sideBarButtons [ 6 ], Ability.AbilityList.MadHatter );
//		abilitySelection.Add ( sideBarButtons [ 7 ], Ability.AbilityList.NonagressionPact );
//		abilitySelection.Add ( sideBarButtons [ 8 ], Ability.AbilityList.Pacifist );
//		abilitySelection.Add ( sideBarButtons [ 9 ], Ability.AbilityList.Sacrifice );
//		abilitySelection.Add ( sideBarButtons [ 10 ], Ability.AbilityList.Teleport );
//		abilitySelection.Add ( sideBarButtons [ 11 ], Ability.AbilityList.Torus );
//
//		//Pair the piece select buttons with a piece color
//		buttonColors.Add ( pieceButtons [ 0 ], PieceColor.Green );
//		buttonColors.Add ( pieceButtons [ 1 ], PieceColor.Yellow );
//		buttonColors.Add ( pieceButtons [ 2 ], PieceColor.Black );
//		buttonColors.Add ( pieceButtons [ 3 ], PieceColor.Red );
//		buttonColors.Add ( pieceButtons [ 4 ], PieceColor.Blue );
//
//		//Clear ability list
//		for ( int i = 0; i < Info.player1AbilityList.Length; i++ )
//		{
//			Info.player1AbilityList [ i ] = null;
//			Info.player2AbilityList [ i ] = null;
//		}
//
//		//Clear sacrifice pieces
//		Info.player1Sacrifice = PieceColor.None;
//		Info.player2Sacrifice = PieceColor.None;
//
//		//Store the button colors
//		selected = sideBarButtons [ 0 ].colors;
//		selected.normalColor = new Color32 ( 255, 210, 75, 255 );
//		unselected = sideBarButtons [ 0 ].colors;
//		unselected.normalColor = new Color32 ( 255, 255, 200, 255 );
//
//		//Make the ability description invisible
//		abilityPanel.gameObject.SetActive ( false );
//
//		//Make player abilities invisible
//		for ( int i = 0; i < player1Abilities.Length; i++ )
//		{
//			player1Abilities [ i ].text = "";
//			player2Abilities [ i ].text = "";
//			player1Pieces [ i ].enabled = false;
//			player2Pieces [ i ].enabled = false;
//			player1Highlights [ i ].enabled = false;
//			player2Highlights [ i ].enabled = false;
//		}
//
//		//Highlight first ability
//		player1Highlights [ 0 ].enabled = true;
//
//		//Set scroll bar
//		scroll.value = 1;
//
//		//Deactivate undo button
//		undoButton.interactable = false;
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
//			.Append ( playerPanel.DOFade ( 0, FADE_TIME ).From ( ) )
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
//	/// Plays the outro animation.
//	/// </summary>
//	private void AnimateOutro ( MenuDestination d )
//	{
//		//Disable input
//		allowInput = false;
//		
//		//Play animation
//		Sequence s = DOTween.Sequence ( )
//			.Append ( playerPanel.DOFade ( 0, FADE_TIME ) );
//		if ( abilityPanel.gameObject.activeSelf )
//			s.Insert ( 0, abilityPanel.DOFade ( 0, FADE_TIME ) );
//		else if ( piecePanel.gameObject.activeSelf )
//			s.Insert ( 0, piecePanel.DOFade ( 0, FADE_TIME ) );
//		else if ( confirmPanel.gameObject.activeSelf )
//			s.Insert ( 0, confirmPanel.DOFade ( 0, FADE_TIME ) );
//		s.Append ( sideBar.DOAnchorPos ( new Vector2 ( -sideBar.rect.width, 0 ), SLIDE_TIME ) )
//			.OnComplete ( () =>
//			{
//				//Check destination
//				switch ( d )
//				{
//					case MenuDestination.GameBoard:
//						//Hide UI and display a loading screen to visually minimize loading delay
//						canvasLoading.SetActive ( true );
//						canvasUI.SetActive ( false );
//
//						//Start game
//						Application.LoadLevel ( "Game Board" );
//						break;
//					case MenuDestination.MainMenu:
//						Application.LoadLevel ( "Main Menu" );
//						break;
//				}
//			} )
//			.SetRecyclable ( )
//			.Play ( );
//	}
//
//	/// <summary>
//	/// Selects an ability for viewing.
//	/// </summary>
//	public void SelectAbility ( Button b )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			SelectAbility ( b, true );
//		}
//	}
//
//	/// <summary>
//	/// Selects an ability for viewing.
//	/// </summary>
//	private void SelectAbility ( Button b, bool playSFX )
//	{
//		//Play SFX
//		if ( playSFX )
//			SFXManager.instance.Click ( );
//
//		//Check if ability list is full
//		if ( !confirmPanel.gameObject.activeSelf )
//		{
//			//Unselect previous button
//			foreach ( Button a in sideBarButtons )
//				a.colors = unselected;
//			
//			//Change color
//			b.colors = selected;
//			
//			//Store selected ability
//			selectedAbility = new Ability ( abilitySelection [ b ] );
//			
//			//Make the ability description visible
//			abilityPanel.gameObject.SetActive ( true );
//			piecePanel.gameObject.SetActive ( false );
//			
//			//Load ability information
//			abilityName.text = selectedAbility.Name;
//			abilityDesc.text = selectedAbility.Desc;
//			
//			//Store selected button
//			selectedButton = b;
//		}
//	}
//
//	/// <summary>
//	/// Randomly assigns the player's remaining abilities.
//	/// </summary>
//	public void AutoAssignAbilities ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			AutoAssignAbilities ( true );
//		}
//	}
//
//	/// <summary>
//	/// Randomly assigns the player's remaining abilities.
//	/// </summary>
//	private void AutoAssignAbilities ( bool playSFX )
//	{
//		//Play SFX
//		if ( playSFX )
//			SFXManager.instance.SelectAbility ( );
//
//		//Store the amount of abilities being auto assigned
//		autoAssignAmount = Info.player1AbilityList.Length - currentSlot;
//		
//		//Randomly assign each of the remaining ability slots open
//		for ( int i = 0; i < autoAssignAmount; i++ )
//		{
//			//Store the random number
//			int rand;
//			
//			do
//			{
//				//Assign random number
//				rand = Random.Range ( 0, sideBarButtons.Length  );
//			} while ( !sideBarButtons [ rand ].IsInteractable ( ) );
//			
//			//Select the random ability
//			SelectAbility ( sideBarButtons [ rand ], false );
//			AcceptAbility ( false );
//			
//			//Check if a piece needs to be assigned
//			if ( selectedAbility != null && selectedAbility.AttachesToPiece )
//			{
//				do
//				{
//					//Assign random number
//					rand = Random.Range ( 0, pieceButtons.Length );
//				} while ( !pieceButtons [ rand ].IsInteractable ( ) );
//				
//				//Attach ability to piece
//				AttachAbility ( pieceButtons [ rand ], false );
//
//				//Check for sacrifice
//				if ( selectedAbility != null && selectedAbility.ID == Ability.AbilityList.Sacrifice.ID )
//				{
//					do
//					{
//						//Assign random number
//						rand = Random.Range ( 0, pieceButtons.Length );
//					} while ( !pieceButtons [ rand ].IsInteractable ( ) );
//					
//					//Sacrifice the first piece
//					AttachAbility ( pieceButtons [ rand ], false );
//				}
//			}
//		}
//		
//		//Make the reassign button visible
//		reassignButton.SetActive ( true );
//	}
//
//	/// <summary>
//	/// Removes the last ability from the player's ability list.
//	/// </summary>
//	public void RemoveAbility ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			RemoveAbility ( true );
//		}
//	}
//
//	/// <summary>
//	/// Removes the last ability from the player's ability list.
//	/// </summary>
//	private void RemoveAbility ( bool playSFX )
//	{
//		//Play SFX
//		if ( playSFX )
//			SFXManager.instance.Click ( );
//
//		//Move to previous slot
//		currentSlot--;
//
//		//Close any open selection
//		if ( piecePanel.gameObject.activeSelf )
//			CancelAttachment ( false );
//		
//		//Check current player
//		if ( isPlayer1 )
//		{
//			//Make highlight visible
//			player1Highlights [ currentSlot ].enabled = true;
//			
//			//Make next highlight invisible
//			if ( currentSlot + 1 < player1Highlights.Length )
//				player1Highlights [ currentSlot + 1 ].enabled = false;
//			
//			//Hide ability name
//			player1Abilities [ currentSlot ].text = "";
//			
//			//Hide ability piece
//			player1Pieces [ currentSlot ].enabled = false;
//			
//			//Reenable ability button
//			foreach ( Button b in sideBarButtons )
//			{
//				if ( Info.player1AbilityList [ currentSlot ].ID == abilitySelection [ b ].ID )
//				{
//					b.interactable = true;
//					break;
//				}
//			}
//			
//			//Reenable piece button
//			if ( Info.player1AbilityList [ currentSlot ].AttachesToPiece )
//			{
//				//Reenable select pieces
//				foreach ( Button b in pieceButtons )
//					if ( buttonColors [ b ] == Info.player1AbilityList [ currentSlot ].AttachedPiece )
//						b.interactable = true;
//
//				//Check for sacrificed pieces
//				if ( Info.player1AbilityList [ currentSlot ].ID == Ability.AbilityList.Sacrifice.ID )
//				{
//					//Reenable select piece
//					foreach ( Button b in pieceButtons )
//						if ( Info.player1Sacrifice == buttonColors [ b ] )
//							b.interactable = true;
//
//					//Clear sacrifice list
//					Info.player1Sacrifice = PieceColor.None;
//				}
//			}
//			
//			//Remove ability
//			Info.player1AbilityList [ currentSlot ] = null;
//		}
//		else
//		{
//			//Make highlight visible
//			player2Highlights [ currentSlot ].enabled = true;
//			
//			//Make next highlight invisible
//			if ( currentSlot + 1 < player2Highlights.Length )
//				player2Highlights [ currentSlot + 1 ].enabled = false;
//			
//			//Hide ability name
//			player2Abilities [ currentSlot ].text = "";
//			
//			//Hide ability piece
//			player2Pieces [ currentSlot ].enabled = false;
//			
//			//Reenable ability button
//			foreach ( Button b in sideBarButtons )
//			{
//				if ( Info.player2AbilityList [ currentSlot ].ID == abilitySelection [ b ].ID )
//				{
//					b.interactable = true;
//					break;
//				}
//			}
//			
//			//Reenable piece button
//			if ( Info.player2AbilityList [ currentSlot ].AttachesToPiece )
//			{
//				//Reenable selected pieces
//				foreach ( Button b in pieceButtons )
//					if ( buttonColors [ b ] == Info.player2AbilityList [ currentSlot ].AttachedPiece )
//						b.interactable = true;
//
//				//Check for sacrificed pieces
//				if ( Info.player2AbilityList [ currentSlot ].ID == Ability.AbilityList.Sacrifice.ID )
//				{
//					//Reenable select piece
//					foreach ( Button b in pieceButtons )
//						if ( Info.player2Sacrifice == buttonColors [ b ] )
//							b.interactable = true;
//					
//					//Clear sacrifice list
//					Info.player2Sacrifice = PieceColor.None;
//				}
//			}
//			
//			//Remove ability
//			Info.player2AbilityList [ currentSlot ] = null;
//		}
//		
//		//Hide all panels
//		abilityPanel.gameObject.SetActive ( false );
//		piecePanel.gameObject.SetActive ( false );
//		confirmPanel.gameObject.SetActive ( false );
//		
//		//Activate auto assign button
//		autoAssignButton.interactable = true;
//		
//		//Unselect button
//		selectedButton = null;
//		foreach ( Button b in sideBarButtons )
//			b.colors = unselected;
//		
//		//Check for first slot
//		if ( currentSlot == 0 )
//		{
//			//Deactivate undo button
//			undoButton.interactable = false;
//		}
//	}
//
//	/// <summary>
//	/// Exits the Ability Selection Menu to the Main Menu.
//	/// </summary>
//	public void ExitToMainMenu ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.Click ( );
//
//			//Clear ability list
//			for ( int i = 0; i < Info.player1AbilityList.Length; i++ )
//			{
//				Info.player1AbilityList [ i ] = null;
//				Info.player2AbilityList [ i ] = null;
//			}
//
//			//Clear sacrifice pieces
//			Info.player1Sacrifice = PieceColor.None;
//			Info.player2Sacrifice = PieceColor.None;
//			
//			//Load main menu
//			MusicManager.instance.ChangeMusic ( AudioContext.MainMenu );
//
//			//Play outro
//			AnimateOutro ( MenuDestination.MainMenu );
//		}
//	}
//
//	/// <summary>
//	/// Accepts an ability for the player.
//	/// </summary>
//	public void AcceptAbility ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			AcceptAbility ( true );
//		}
//	}
//
//	/// <summary>
//	/// Accepts an ability for the player.
//	/// </summary>
//	private void AcceptAbility ( bool playSFX )
//	{
//		//Check if the ability attaches to a piece
//		if ( selectedAbility.AttachesToPiece )
//		{
//			//Play SFX
//			if ( playSFX )
//				SFXManager.instance.Click ( );
//
//			//Make piece selection visible
//			piecePanel.gameObject.SetActive ( true );
//			abilityPanel.gameObject.SetActive ( false );
//			
//			//Load piece selection prompt
//			piecePrompt.text = "Assign the " + selectedAbility.Name + " ability to a piece.";
//		}
//		else
//		{
//			//Play SFX
//			if ( playSFX )
//				SFXManager.instance.SelectAbility ( );
//
//			//Add ability to the player's ability list
//			AddAbility ( );
//		}
//	}
//
//	/// <summary>
//	/// Confirm the player's abilities.
//	/// </summary>
//	public void ConfirmAbilities ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.AcceptAbilities ( );
//
//			//Reset current slot
//			currentSlot = 0;
//			
//			//Deactive the undo button
//			undoButton.interactable = false;
//			
//			//Hide confirmation panel
//			confirmPanel.gameObject.SetActive ( false );
//			
//			//Check current player
//			if ( isPlayer1 )
//			{
//				//End player 1's turn
//				isPlayer1 = false;
//				
//				//Reactivate buttons
//				EnableAbilityButtons ( );
//				EnableAttachButtons ( );
//				autoAssignButton.interactable = true;
//				scroll.value = 1;
//				
//				//Prompt player 2
//				sideBarPrompt.text = "Player 2\nSelect Your Abilities";
//				player2Highlights [ currentSlot ].enabled = true;
//			}
//			else
//			{
//				//Start game
//				MusicManager.instance.ChangeMusic ( AudioContext.Gameplay );
//				AnimateOutro ( MenuDestination.GameBoard );
//			}
//		}
//	}
//
//	/// <summary>
//	/// Reassigns the player's abilities.
//	/// </summary>
//	public void ReassignAbilities ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			SFXManager.instance.SelectAbility ( );
//
//			//Remove the randomly assigned abilities
//			for ( int i = 0; i < autoAssignAmount; i++ )
//				RemoveAbility ( false );
//			
//			//Reassign abilities
//			AutoAssignAbilities ( false );
//		}
//	}
//
//	/// <summary>
//	/// Adds an ability to the player's ability list.
//	/// </summary>
//	private void AddAbility ( )
//	{
//		//Unselect button
//		foreach ( Button b in sideBarButtons )
//			b.colors = unselected;
//
//		//Check current player
//		if ( isPlayer1 )
//		{
//			//Make highlight invisible
//			player1Highlights [ currentSlot ].enabled = false;
//
//			//Store name
//			player1Abilities [ currentSlot ].text = selectedAbility.Name;
//
//			//Add ability
//			Info.player1AbilityList [ currentSlot ] = selectedAbility;
//
//			//Check if the ability is attachable
//			if ( selectedAbility.AttachesToPiece )
//			{
//				//Display attached piece
//				player1Pieces [ currentSlot ].enabled = true;
//				switch ( selectedPiece )
//				{
//					case PieceColor.Black:
//						player1Pieces [ currentSlot ].color = Piece.Display.Black;
//						Info.player1AbilityList [ currentSlot ].AttachedPiece = PieceColor.Black;
//						break;
//					case PieceColor.Blue:
//						player1Pieces [ currentSlot ].color = Piece.Display.Blue;
//						Info.player1AbilityList [ currentSlot ].AttachedPiece = PieceColor.Blue;
//						break;
//					case PieceColor.Green:
//						player1Pieces [ currentSlot ].color = Piece.Display.Green;
//						Info.player1AbilityList [ currentSlot ].AttachedPiece = PieceColor.Green;
//						break;
//					case PieceColor.Red:
//						player1Pieces [ currentSlot ].color = Piece.Display.Red;
//						Info.player1AbilityList [ currentSlot ].AttachedPiece = PieceColor.Red;
//						break;
//					case PieceColor.Yellow:
//						player1Pieces [ currentSlot ].color = Piece.Display.Yellow;
//						Info.player1AbilityList [ currentSlot ].AttachedPiece = PieceColor.Yellow;
//						break;
//				}
//			}
//
//			//Move to next slot
//			currentSlot++;
//			DisableAbilityButton ( );
//			
//			//Check if player is finished
//			if ( currentSlot >= player1Abilities.Length )
//			{
//				//Display confirm panel
//				confirmPanel.gameObject.SetActive ( true );
//
//				//Deactive the auto assign button
//				autoAssignButton.interactable = false;
//				reassignButton.SetActive ( false );
//			}
//			else
//			{
//				//Highlight next ability
//				player1Highlights [ currentSlot ].enabled = true;
//			}
//		}
//		else
//		{
//			//Make highlight invisible
//			player2Highlights [ currentSlot ].enabled = false;
//
//			//Store name
//			player2Abilities [ currentSlot ].text = selectedAbility.Name;
//
//			//Add ability
//			Info.player2AbilityList [ currentSlot ] = selectedAbility;
//			
//			//Check if the ability is attachable
//			if ( selectedAbility.AttachesToPiece )
//			{
//				//Display attached piece
//				player2Pieces [ currentSlot ].enabled = true;
//				switch ( selectedPiece )
//				{
//					case PieceColor.Black:
//						player2Pieces [ currentSlot ].color = Piece.Display.Black;
//						Info.player2AbilityList [ currentSlot ].AttachedPiece = PieceColor.Black;
//						break;
//					case PieceColor.Blue:
//						player2Pieces [ currentSlot ].color = Piece.Display.Blue;
//						Info.player2AbilityList [ currentSlot ].AttachedPiece = PieceColor.Blue;
//						break;
//					case PieceColor.Green:
//						player2Pieces [ currentSlot ].color = Piece.Display.Green;
//						Info.player2AbilityList [ currentSlot ].AttachedPiece = PieceColor.Green;
//						break;
//					case PieceColor.Red:
//						player2Pieces [ currentSlot ].color = Piece.Display.Red;
//						Info.player2AbilityList [ currentSlot ].AttachedPiece = PieceColor.Red;
//						break;
//					case PieceColor.Yellow:
//						player2Pieces [ currentSlot ].color = Piece.Display.Yellow;
//						Info.player2AbilityList [ currentSlot ].AttachedPiece = PieceColor.Yellow;
//						break;
//				}
//			}
//			
//			//Move to next slot
//			currentSlot++;
//			DisableAbilityButton ( );
//			
//			//Check if player is finished
//			if ( currentSlot >= player2Abilities.Length )
//			{
//				//Display confirm panel
//				confirmPanel.gameObject.SetActive ( true );
//
//				//Deactive the auto assign button
//				autoAssignButton.interactable = false;
//				reassignButton.SetActive ( false );
//			}
//			else
//			{
//				//Highlight next ability
//				player2Highlights [ currentSlot ].enabled = true;
//			}
//		}
//
//		//Make the ability description invisible
//		abilityPanel.gameObject.SetActive ( false );
//		piecePanel.gameObject.SetActive ( false );
//		selectedButton = null;
//		undoButton.interactable = true;
//
//		//Reset ability
//		selectedAbility = null;
//		selectedPiece = PieceColor.None;
//	}
//
//	/// <summary>
//	/// Deactivates the selected ability button.
//	/// </summary>
//	private void DisableAbilityButton ( )
//	{
//		//Deactive the selected button
//		selectedButton.interactable = false;
//	}
//
//	/// <summary>
//	/// Reenables all ability buttons.
//	/// </summary>
//	private void EnableAbilityButtons ( )
//	{
//		//Reenable the ability buttons
//		foreach ( Button b in sideBarButtons )
//			b.interactable = true;
//	}
//
//	/// <summary>
//	/// Back out of attaching an ability to a piece.
//	/// </summary>
//	public void CancelAttachment ( )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			CancelAttachment ( true );
//		}
//	}
//
//	/// <summary>
//	/// Back out of attaching an ability to a piece.
//	/// </summary>
//	private void CancelAttachment ( bool playSFX )
//	{
//		//Play SFX
//		if ( playSFX )
//			SFXManager.instance.Click ( );
//
//		//Make ability description visible
//		abilityPanel.gameObject.SetActive ( true );
//		piecePanel.gameObject.SetActive ( false );
//
//		//Check for sacrifice
//		if ( selectedAbility.ID == Ability.AbilityList.Sacrifice.ID )
//		{
//			//Reenable select pieces
//			foreach ( Button b in pieceButtons )
//				if ( selectedPiece == buttonColors [ b ] )
//					b.interactable = true;
//
//			//Clear the sacrifice list
//			selectedPiece = PieceColor.None;
//		}
//	}
//
//	/// <summary>
//	/// Attachs the selected ability to a piece.
//	/// </summary>
//	public void AttachAbility ( Button b )
//	{
//		//Check input
//		if ( allowInput )
//		{
//			//Play SFX
//			AttachAbility ( b, true );
//		}
//	}
//
//	/// <summary>
//	/// Attachs the selected ability to a piece.
//	/// </summary>
//	private void AttachAbility ( Button b, bool playSFX )
//	{
//		//Prevent ability stacking
//		b.interactable = false;
//
//		//Check for sacrifice
//		if ( selectedAbility.ID == Ability.AbilityList.Sacrifice.ID )
//		{
//			//Check if a piece has been selected
//			if ( selectedPiece != PieceColor.None )
//			{
//				//Play SFX
//				SFXManager.instance.SelectAbility ( );
//
//				//Store the sacrificed pieces
//				if ( isPlayer1 )
//					Info.player1Sacrifice = buttonColors [ b ];
//				else
//					Info.player2Sacrifice = buttonColors [ b ];
//
//				//Add ability
//				AddAbility ( );
//			}
//			else
//			{
//				//Play SFX
//				SFXManager.instance.Click ( );
//
//				//Store piece
//				selectedPiece = buttonColors [ b ];
//
//				//Load piece selection prompt
//				piecePrompt.text = "Select a piece to start without";
//			}
//		}
//		else
//		{
//			//Play SFX
//			SFXManager.instance.SelectAbility ( );
//
//			//Add ability
//			selectedPiece = buttonColors [ b ];
//			AddAbility ( );
//		}
//	}
//
//	/// <summary>
//	/// Enables each attach button.
//	/// </summary>
//	private void EnableAttachButtons ( )
//	{
//		//Enable each button
//		foreach ( Button b in pieceButtons )
//			b.interactable = true;
//	}
}
