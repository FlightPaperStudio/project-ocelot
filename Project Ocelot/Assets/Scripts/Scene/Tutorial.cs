using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

public class Tutorial : MonoBehaviour 
{
//	//Tutorial game objects
//	public GameObject popup;
//	public GameObject pausePanel;
//	public GameObject promptPanel;
//	public Text prompt;
//	public GameObject rematchButton;
//	public Text rematchText;
//
//	//Tutorial information
//	public static bool isPlaying = false;
//	public static bool isGame1 = true;
//	public GameObject movementTutorial;
//	public GameObject abilityTutorial;
//	public Turn [ ] game1Sequence;
//	public Turn [ ] game2Sequence;
//
//	//Turn information
//	private Turn currentTurn;
//	private GameObject currentObj;
//	private int turnCounter = 0;
//	private int currentMove = -1;
//	private int currentPrompt = 0;
//
//	//Animation information
//	private Vector3 arrowPos;
//	private Vector2 arrowPosUI;
//	private const float ANIMATE_TIME = 0.5f;
//	private const float MOVE_DISTANCE = 0.4f;
//
//	/// <summary>
//	/// Initializes the player's abilities for the tutorial.
//	/// </summary>
//	private void Awake ( )
//	{
//		//Check if the tutorial is playing
//		if ( isPlaying )
//		{
//			//Check tutorial
//			if ( isGame1 )
//			{
//				//Start player 1 with Teleport, Armor, and Mad Hatter
//				Info.player1AbilityList [ 0 ] = new Ability ( Ability.AbilityList.Teleport );
//				Info.player1AbilityList [ 0 ].AttachedPiece = PieceColor.Red;
//				Info.player1AbilityList [ 1 ] = new Ability ( Ability.AbilityList.Armor );
//				Info.player1AbilityList [ 1 ].AttachedPiece = PieceColor.Yellow;
//				Info.player1AbilityList [ 2 ] = new Ability ( Ability.AbilityList.MadHatter );
//
//				//Start player 2 with Caboose, Conversion, and Torus
//				Info.player2AbilityList [ 0 ] = new Ability ( Ability.AbilityList.Caboose );
//				Info.player2AbilityList [ 1 ] = new Ability ( Ability.AbilityList.Conversion );
//				Info.player2AbilityList [ 1 ].AttachedPiece = PieceColor.Green;
//				Info.player2AbilityList [ 2 ] = new Ability ( Ability.AbilityList.Torus );
//				Info.player2AbilityList [ 2 ].AttachedPiece = PieceColor.Yellow;
//
//				//Enable movement tutorial
//				movementTutorial.SetActive ( true );
//
//				//Edit the rematch button
//				rematchText.text = "Continue";
//
//				//Store starting turn
//				currentTurn = game1Sequence [ turnCounter ];
//			}
//			else
//			{
//				//Start player 1 with Teleport, Caboose, and Mad Hatter
//				Info.player1AbilityList [ 0 ] = new Ability ( Ability.AbilityList.Teleport );
//				Info.player1AbilityList [ 0 ].AttachedPiece = PieceColor.Black;
//				Info.player1AbilityList [ 1 ] = new Ability ( Ability.AbilityList.Caboose );
//				Info.player1AbilityList [ 2 ] = new Ability ( Ability.AbilityList.MadHatter );
//				
//				//Start player 2 with Armor, Conversion, and Torus
//				Info.player2AbilityList [ 0 ] = new Ability ( Ability.AbilityList.Armor );
//				Info.player2AbilityList [ 0 ].AttachedPiece = PieceColor.Red;
//				Info.player2AbilityList [ 1 ] = new Ability ( Ability.AbilityList.Conversion );
//				Info.player2AbilityList [ 1 ].AttachedPiece = PieceColor.Yellow;
//				Info.player2AbilityList [ 2 ] = new Ability ( Ability.AbilityList.Torus );
//				Info.player2AbilityList [ 2 ].AttachedPiece = PieceColor.Blue;
//
//				//Enable ability tutorial
//				abilityTutorial.SetActive ( true );
//
//				//Disable the rematch button
//				rematchButton.SetActive ( false );
//
//				//Store starting turn
//				currentTurn = game2Sequence [ turnCounter ];
//			}
//		}
//	}
//
//	/// <summary>
//	/// Checks if the object clicked is the current object to be clicked in the tutorial.
//	/// </summary>
//	public bool CheckObject ( GameObject obj )
//	{
//		//Check object
//		if ( obj == currentObj )
//		{
//			//Disable current arrow
//			if ( !currentTurn.isOpponent )
//			{
//				//Check arrow type
//				if ( currentTurn.arrows [ currentMove ].transform is RectTransform )
//				{
//					//Reset arrow
//					RectTransform a = currentTurn.arrows [ currentMove ].transform as RectTransform;
//					a.anchoredPosition = arrowPosUI;
//				}
//				else
//				{
//					//Reset arrow
//					currentTurn.arrows [ currentMove ].transform.position = arrowPos;
//				}
//
//				//Hide arrow
//				currentTurn.arrows [ currentMove ].transform.DOKill ( );
//				currentTurn.arrows [ currentMove ].SetActive ( false );
//			}
//
//			//Allow the click event
//			return true;
//		}
//		else
//		{
//			//Prevent the click event
//			return false;
//		}
//	}
//
//	/// <summary>
//	/// Starts the next move in the current turn.
//	/// </summary>
//	public void NextMove ( )
//	{
//		//Store delay
//		float delay = 0;
//		if ( currentMove != -1 )
//			delay = currentTurn.delays [ currentMove ];
//
//		//Automatically make the next move
//		this.transform.DOMove ( this.transform.position, 0 )
//			.SetDelay ( delay )
//			.OnComplete ( () =>
//			{
//				//Increment move
//				currentMove++;
//				
//				//Check for move
//				if ( currentMove < currentTurn.objs.Length )
//				{
//					//Store move
//					currentObj = currentTurn.objs [ currentMove ];
//					
//					//Check prompt
//					if ( currentPrompt < currentTurn.interrupts.Length && currentTurn.interrupts [ currentPrompt ] == currentMove )
//						OpenPrompt ( );
//					
//					//Check for opponent
//					if ( currentTurn.isOpponent )
//					{
//						//Click the current object after the delay
//						ExecuteEvents.Execute ( currentObj, new PointerEventData ( EventSystem.current ), ExecuteEvents.pointerClickHandler );
//					}
//					else
//					{
//						//Display arrow
//						currentTurn.arrows [ currentMove ].SetActive ( true );
//						
//						//Check arrow type
//						if ( currentTurn.arrows [ currentMove ].transform is RectTransform )
//						{
//							//Store position
//							RectTransform a = currentTurn.arrows [ currentMove ].transform as RectTransform;
//							arrowPosUI = a.anchoredPosition;
//							
//							//Animate arrow
//							a.DOAnchorPos ( new Vector2 ( arrowPosUI.x, arrowPosUI.y - 10 ), ANIMATE_TIME ).SetLoops ( -1, LoopType.Yoyo ).SetEase ( Ease.InOutSine );
//						}
//						else
//						{
//							//Store position
//							arrowPos = currentTurn.arrows [ currentMove ].transform.position;
//							
//							//Animate arrow
//							if ( currentTurn.arrows [ currentMove ].transform.rotation.z == 0 )
//								currentTurn.arrows [ currentMove ].transform.DOMoveY ( arrowPos.y + MOVE_DISTANCE, ANIMATE_TIME ).SetLoops ( -1, LoopType.Yoyo ).SetEase ( Ease.InOutSine );
//							else
//								currentTurn.arrows [ currentMove ].transform.DOMoveY ( arrowPos.y - MOVE_DISTANCE, ANIMATE_TIME ).SetLoops ( -1, LoopType.Yoyo ).SetEase ( Ease.InOutSine );
//						}
//					}
//				}
//			} );
//	}
//
//	/// <summary>
//	/// Starts the next turn in the tutorial.
//	/// </summary>
//	public void NextTurn ( )
//	{
//		//Reset counters
//		currentMove = -1;
//		currentPrompt = 0;
//
//		//Set next turn
//		turnCounter++;
//		if ( isGame1 )
//			currentTurn = game1Sequence [ turnCounter ];
//		else
//			currentTurn = game2Sequence [ turnCounter ];
//
//		//Start new turn
//		NextMove ( );
//	}
//
//	/// <summary>
//	/// Opens the current tutorial prompt.
//	/// </summary>
//	private void OpenPrompt ( )
//	{
//		//Display prompt
//		popup.SetActive ( true );
//		pausePanel.SetActive ( false );
//		promptPanel.SetActive ( true );
//		
//		//Display prompt
//		prompt.text = currentTurn.prompts [ currentPrompt ];
//	}
//
//	/// <summary>
//	/// Checks for the next tutorial prompt and either displays the next tutorial prompt or closes the prompt.
//	/// </summary>
//	public void ClosePrompt ( )
//	{
//		//Play SFX
//		SFXManager.instance.Click ( );
//
//		//Increment prompt counter
//		currentPrompt++;
//
//		//Check for next prompt
//		if ( currentPrompt < currentTurn.interrupts.Length && currentTurn.interrupts [ currentPrompt ] == currentMove )
//		{
//			//Display the next prompt
//			OpenPrompt ( );
//		}
//		else
//		{
//			//Close prompt
//			promptPanel.SetActive ( false );
//			popup.SetActive ( false );
//		}
//	}
}
