using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Tile : MonoBehaviour 
{
	// Tile information
	public int ID;
	public SpriteRenderer sprite;
	public GameManager GM;
	public Board board;

	// Neighboring tiles
	public Tile above;
	public Tile below;
	public Tile rightAbove;
	public Tile rightBelow;
	public Tile leftAbove;
	public Tile leftBelow;
	[ HideInInspector ]
	public Tile [ ] neighbors = new Tile [ 6 ];

	// Instance information
	public TileState state
	{
		get;
		private set;
	}
	public Unit currentUnit;

	/// <summary>
	/// Sets the neighbor tiles at the 
	/// </summary>
	private void Start ( )
	{
		// Set neighbors
		neighbors [ 0 ] = above;
		neighbors [ 1 ] = rightAbove;
		neighbors [ 2 ] = rightBelow;
		neighbors [ 3 ] = below;
		neighbors [ 4 ] = leftBelow;
		neighbors [ 5 ] = leftAbove;
	}

	/// <summary>
	/// Sets the state of the tile.
	/// </summary>
	public void SetTileState ( TileState s )
	{
		// Set tile state
		state = s;

		// Highlight tile
		HighlightTile ( s );
	}

	/// <summary>
	/// Highlights the tile according to its current state.
	/// </summary>
	public void HighlightTile ( TileState s )
	{
		// Set the tile's color to its corrisponding state
		sprite.color = board.tileColorDic [ s ];
	}

	/// <summary>
	/// Highlights the tile to display potential actions when the mouse starts hovering over the tile.
	/// </summary>
	public void MouseEnter ( )
	{
		// Check state
		switch ( state )
		{
		// Hover over a friendly unit
		case TileState.AvailableUnit:
			HighlightTile ( TileState.AvailableUnitHover );
			break;

		// Hover over an available move
		case TileState.AvailableMove:
			HighlightTile ( TileState.AvailableMoveHover );
			break;

		// Hover over an available move with a potential capture
		case TileState.AvailableMoveCapture:
			HighlightTile ( TileState.AvailableMoveCaptureHover );
			foreach ( Tile t in GM.selectedUnit.moveDic [ this ].capture )
				t.HighlightTile ( TileState.AvailableCaptureHover );
			break;

		// Hover over an available special ability move
		case TileState.AvailableSpecial:
			HighlightTile ( TileState.AvailableSpecialHover );
			break;

		// Hover over an available special ability move with a potential capture
		case TileState.AvailableSpecialCapture:
			HighlightTile ( TileState.AvailableSpecialCaptureHover );
			foreach ( Tile t in GM.selectedUnit.moveDic [ this ].capture )
				t.HighlightTile ( TileState.AvailableCaptureHover );
			break;

		// Hover over an available tile usable for a command
		case TileState.AvailableCommand:
			HighlightTile ( TileState.AvailableCommandHover );
			break;

		// Hover over a conflict tile
		case TileState.ConflictedTile:
			HighlightTile ( TileState.ConflictedTileHover );
			GM.UI.conflictPrompt.SetActive ( true );
			foreach ( MoveData m in GM.selectedUnit.moveList.FindAll ( item => item.tile == this ) )
				foreach ( Tile t in m.capture )
					t.HighlightTile ( TileState.AvailableCaptureHover );
			break;
		}
	}

	/// <summary>
	/// Highlights the tile to display potential actions when the mouse stops hovering over the tile.
	/// </summary>
	public void MouseExit ( )
	{
		// Return tile color to current state
		HighlightTile ( state );

		// Check if other tiles needs to return their tile color to their current state (in case of potential captures)
		if ( state == TileState.AvailableMoveCapture || state == TileState.AvailableSpecialCapture )
			foreach ( Tile t in GM.selectedUnit.moveDic [ this ].capture )
				t.HighlightTile ( TileState.AvailableCapture );

		if ( state == TileState.ConflictedTile )
		{
			GM.UI.conflictPrompt.SetActive ( false );
			foreach ( MoveData m in GM.selectedUnit.moveList.FindAll ( item => item.tile == this ) )
				foreach ( Tile t in m.capture )
					t.HighlightTile ( TileState.AvailableCapture );
		}
	}

	/// <summary>
	/// Performs the appropriate action upon a mouse click.
	/// </summary>
	public void MouseClick ( BaseEventData data )
	{
		// Check state
		switch ( state )
		{
		// Select unit
		case TileState.AvailableUnit:
			GM.SelectUnit ( currentUnit );
			break;

		// Select move
		case TileState.AvailableMove:
		case TileState.AvailableMoveCapture:
		case TileState.AvailableSpecial:
		case TileState.AvailableSpecialCapture:
			GM.SelectMove ( this );
			break;

		// Select a tile for a command
		case TileState.AvailableCommand:
			SetTileState ( TileState.AvailableCommandSelected );
			HeroUnit h = GM.selectedUnit as HeroUnit;
			h.SelectCommandTile ( this );
			break;

		// Select conflicted move
		case TileState.ConflictedTile:
			GM.UI.conflictPrompt.SetActive ( false );
			PointerEventData pointerEventData = data as PointerEventData;
			if ( pointerEventData.button == PointerEventData.InputButton.Left )
				GM.SelectMove ( this, true );
			else if ( pointerEventData.button == PointerEventData.InputButton.Right )
				GM.SelectMove ( this, false );
			break;
		}
	}


	// ---------------------------------------------------------------DELETE EVERYTHING BELOW THIS LINE------------------------------------------------------------------------

//	public int wave;
//
//	//Game information
//	private Info info;
//
//	//Tile color information
//	private Dictionary < TileStateColor, Color > tileColors = new Dictionary < TileStateColor, Color > ( ) {
//		{ TileStateColor.Normal,                new Color32 ( 200, 200, 200, 255 ) }, //Light grey
//		{ TileStateColor.FriendlyPiece,         new Color32 ( 255, 255, 200, 255 ) }, //Light yellow
//		{ TileStateColor.FriendlyPieceHover,    new Color32 ( 255, 210,  75, 255 ) }, //Gold
//		{ TileStateColor.SelectedPiece,         new Color32 ( 255, 210,  75, 255 ) }, //Gold
//		{ TileStateColor.AvailableMove,         new Color32 ( 150, 255, 255, 255 ) }, //Light cyan
//		{ TileStateColor.AvailableMoveHover,    new Color32 (   0, 165, 255, 255 ) }, //Dark cyan
//		{ TileStateColor.AvailableCapture,      new Color32 ( 255, 150, 150, 255 ) }, //Light red
//		{ TileStateColor.AvailableCaptureHover, new Color32 ( 200,  50,  50, 255 ) }, //Dark red
//		{ TileStateColor.AbilityMove,           new Color32 ( 255, 125, 255, 255 ) }, //Light purple
//		{ TileStateColor.AbilityMoveHover,      new Color32 ( 125,   0, 125, 255 ) }, //Purple
//		{ TileStateColor.AvailableWin,          new Color32 ( 150, 255, 150, 255 ) }, //Light green
//		{ TileStateColor.AvailableWinHover,     new Color32 (  75, 255,  75, 255 ) }  //Green
//	};
//
//	//Wrap around tiles for the Torus ability
//	public Tile torusAbove;
//	public Tile torusBelow;
//	public Tile torusRightAbove;
//	public Tile torusRightBelow;
//	public Tile torusLeftAbove;
//	public Tile torusLeftBelow;
//	[ HideInInspector ]
//	public Tile [ ] torusNeighbors = new Tile [ 6 ];
//	[ HideInInspector ]
//	public bool isTorusJump = false;
//
//	//Piece currently on this tile
//	public Piece currentPiece;
//
//	public bool isP1GoalArea = false;
//	public bool isP2GoalArea = false;
//
//	//Animation information
//	private const float ANIMATE_TIME = 0.5f;
//
//	private void Awake ( )
//	{
//		//Store informtion
//		info = this.GetComponentInParent < Info > ( );
//
//
//
//		//Store Torus neighbors
//		torusNeighbors [ 0 ] = torusLeftAbove;
//		torusNeighbors [ 1 ] = torusLeftBelow;
//		torusNeighbors [ 2 ] = torusAbove;
//		torusNeighbors [ 3 ] = torusBelow;
//		torusNeighbors [ 4 ] = torusRightAbove;
//		torusNeighbors [ 5 ] = torusRightBelow;
//	}
//
//	/// <summary>
//	/// Resets the tile color to normal.
//	/// </summary>
//	public void ResetColor ( )
//	{
//		//Change color
//		sprite.color = tileColors [ TileStateColor.Normal ];
//
//		//Change state
//		state = TileState.Normal;
//
//		//Mark that it's not a torus jump
//		isTorusJump = false;
//	}
//
//	/// <summary>
//	/// Highlights the player's pieces at the start of their turn.
//	/// </summary>
//	public void HighlightPlayerPiece ( )
//	{
//		//Change color
//		sprite.color = tileColors [ TileStateColor.FriendlyPiece ];
//
//		//Change state
//		state = TileState.FriendlyPiece;
//	}
//
//	/// <summary>
//	/// Highlights a player's piece on the mouse hovering over it.
//	/// </summary>
//	public void HighlightSelectedPiece ( )
//	{
//		//Change color
//		sprite.color = tileColors [ TileStateColor.FriendlyPieceHover ];
//	}
//
//	/// <summary>
//	/// Highlights pieces for selecting ability use.
//	/// </summary>
//	public void HighlightAbilityPieceSelection ( )
//	{
//		//Change color
//		sprite.color = tileColors [ TileStateColor.AbilityMove ];
//		
//		//Change state
//		state = TileState.AbilityPieceSelection;
//	}
//
//	/// <summary>
//	/// Highlights pieces for selecting ability use.
//	/// </summary>
//	public void HighlightSelectedAbilityPiece ( )
//	{
//		//Change color
//		sprite.color = tileColors [ TileStateColor.AbilityMoveHover ];
//	}
//
//	/// <summary>
//	/// Highlights a potential move for the player's selected piece.
//	/// </summary>
//	public void HighlightPotentialMove ( )
//	{
//		//Change color
//		if ( info.selectedPiece != null && ( info.selectedPiece.isJolted || ( info.selectedPiece.ability != null && info.selectedPiece.ability.ID == Ability.AbilityList.Sacrifice.ID && !info.selectedPiece.sacrificeMoveRemaining ) ) )
//			sprite.color = tileColors [ TileStateColor.AbilityMove ];
//		else
//			sprite.color = tileColors [ TileStateColor.AvailableMove ];
//
//		//Change state
//		state = TileState.PotentialMove;
//	}
//
//	/// <summary>
//	/// Highlights a potential jump for the player's selected piece.
//	/// </summary>
//	public void HighlightPotentialJump ( )
//	{
//		//Chang color
//		if ( info.selectedPiece != null && ( info.selectedPiece.isJolted || ( info.selectedPiece.ability != null && info.selectedPiece.ability.ID == Ability.AbilityList.Sacrifice.ID && !info.selectedPiece.sacrificeMoveRemaining ) ) )
//			sprite.color = tileColors [ TileStateColor.AbilityMove ];
//		else
//			sprite.color = tileColors [ TileStateColor.AvailableMove ];
//
//		//Change state
//		state = TileState.PotentialJump;
//	}
//
//	/// <summary>
//	/// Highlights a selected move on the mouse hovering over that tile.
//	/// </summary>
//	public void HighlightSelectedMove ( )
//	{
//		//Change color
//		if ( info.selectedPiece != null && ( info.selectedPiece.isJolted || ( info.selectedPiece.ability != null && info.selectedPiece.ability.ID == Ability.AbilityList.Sacrifice.ID && !info.selectedPiece.sacrificeMoveRemaining ) ) )
//			sprite.color = tileColors [ TileStateColor.AbilityMoveHover ];
//		else
//			sprite.color = tileColors [ TileStateColor.AvailableMoveHover ];
//
//		//Check for potential capture
//		if ( state == TileState.PotentialJump )
//			HighlightSelectedCapture ( );
//	}
//
//	/// <summary>
//	/// Highlights an opponent's piece that is available for capture.
//	/// </summary>
//	public void HighlightPotentialCapture ( )
//	{
//		//Change color
//		sprite.color = tileColors [ TileStateColor.AvailableCapture ];
//
//		//Change state
//		state = TileState.PotentialCapture;
//	}
//
//	/// <summary>
//	/// Highlights an opponent's piece that is available for capture for the selected piece.
//	/// </summary>
//	private void HighlightSelectedCapture ( )
//	{
//		//Check for piece captures
//		for ( int i = 0; i < neighbors.Length; i++ )
//		{
//			//Check if tile is a torus jump
//			if ( isTorusJump )
//			{
//				//Check for tile
//				if ( info.selectedPiece.currentTile.neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].torusNeighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].torusNeighbors [ i ] == this && info.selectedPiece.currentTile.neighbors [ i ].state == TileState.PotentialCapture )
//				{
//					//Change color
//					info.selectedPiece.currentTile.neighbors [ i ].sprite.color = tileColors [ TileStateColor.AvailableCaptureHover ];
//					
//					//Quit search
//					break;
//				}
//				else if ( info.selectedPiece.currentTile.torusNeighbors [ i ] != null && info.selectedPiece.currentTile.torusNeighbors [ i ].neighbors [ i ] != null && info.selectedPiece.currentTile.torusNeighbors [ i ].neighbors [ i ] == this && info.selectedPiece.currentTile.torusNeighbors [ i ].state == TileState.PotentialCapture )
//				{
//					//Change color
//					info.selectedPiece.currentTile.torusNeighbors [ i ].sprite.color = tileColors [ TileStateColor.AvailableCaptureHover ];
//					
//					//Quit search
//					break;
//				}
//			}
//			else
//			{
//				//Check for tile
//				if ( info.selectedPiece.currentTile.neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ] == this && info.selectedPiece.currentTile.neighbors [ i ].state == TileState.PotentialCapture )
//				{
//					//Change color
//					info.selectedPiece.currentTile.neighbors [ i ].sprite.color = tileColors [ TileStateColor.AvailableCaptureHover ];
//
//					//Quit search
//					break;
//				}
//			}
//		}
//	}
//
//	/// <summary>
//	/// Unhighlights an opponent's piece that is available for capture for the previously selected piece.
//	/// </summary>
//	private void UnhighlightSelectedCapture ( )
//	{
//		//Check for piece captures
//		for ( int i = 0; i < neighbors.Length; i++ )
//		{
//			//Check if tile is a torus jump
//			if ( isTorusJump )
//			{
//				//Check for tile
//				if ( info.selectedPiece.currentTile.neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].torusNeighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].torusNeighbors [ i ] == this && info.selectedPiece.currentTile.neighbors [ i ].state == TileState.PotentialCapture )
//				{
//					//Change color
//					info.selectedPiece.currentTile.neighbors [ i ].HighlightPotentialCapture ( );
//					
//					//Quit search
//					break;
//				}
//				else if ( info.selectedPiece.currentTile.torusNeighbors [ i ] != null && info.selectedPiece.currentTile.torusNeighbors [ i ].neighbors [ i ] != null && info.selectedPiece.currentTile.torusNeighbors [ i ].neighbors [ i ] == this && info.selectedPiece.currentTile.torusNeighbors [ i ].state == TileState.PotentialCapture )
//				{
//					//Change color
//					info.selectedPiece.currentTile.torusNeighbors [ i ].HighlightPotentialCapture ( );
//					
//					//Quit search
//					break;
//				}
//			}
//			else
//			{
//				//Check for tile
//				if ( info.selectedPiece.currentTile.neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ] == this && info.selectedPiece.currentTile.neighbors [ i ].state == TileState.PotentialCapture )
//				{
//					//Change color
//					info.selectedPiece.currentTile.neighbors [ i ].HighlightPotentialCapture ( );
//					
//					//Quit search
//					break;
//				}
//			}
//		}
//	}
//    
//	/// <summary>
//	/// Highlights a potential move that uses an ability.
//	/// </summary>
//    public void HighlightPotentialAbility ( )
//	{
//		//Check for jump
//		if ( state != TileState.PotentialJump )
//		{
//			//Change color
//			sprite.color = tileColors [ TileStateColor.AbilityMove ];
//			
//			//Change state
//			state = TileState.PotentialAbility;
//		}
//	}
//
//	/// <summary>
//	/// Highlights a selected move that uses an ability.
//	/// </summary>
//	public void HighlightSelectedAbility ( )
//	{
//		//Change color
//		sprite.color = tileColors [ TileStateColor.AbilityMoveHover ];
//
//		//Check for catapult tiles
//		if ( info.selectedPiece.ability != null && info.selectedPiece.ability.ID == Ability.AbilityList.Catapult.ID )
//		{
//			//Find middle tiles
//			for ( int i = 0; i < neighbors.Length; i++ )
//			{
//				//Check for catapult tile
//				if ( info.selectedPiece.currentTile.neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ].neighbors [ i ] == this )
//				{
//					//Check for capture on the first tile
//					if ( info.selectedPiece.currentTile.neighbors [ i ].state == TileState.PotentialCapture )
//						info.selectedPiece.currentTile.neighbors [ i ].sprite.color = tileColors [ TileStateColor.AvailableCaptureHover ];
//
//					//Check for capture on the second tile
//					if ( info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ].state == TileState.PotentialCapture )
//						info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ].sprite.color = tileColors [ TileStateColor.AvailableCaptureHover ];
//
//					//End search
//					break;
//				}
//			}
//		}
//
//		//Check for torus jump
//		if ( isTorusJump || ( info.sacrificeArmorTile != null && info.sacrificeArmorTile == this ) )
//			HighlightSelectedCapture ( );
//	}
//
//	/// <summary>
//	/// Show the player's currently selected piece's movement options.
//	/// </summary>
//	public void SelectPiece ( )
//	{
//		//Check if game is over
//		if ( !info.isGameOver )
//		{
//			//Store selected piece
//			info.selectedPiece = currentPiece;
//
//			//Reset board
//			info.ResetBoardColor ( );
//			if ( info.beginningOfTurn && !currentPiece.isJolted )
//				info.HighlightCurrentPlayerPieces ( );
//
//			//Highlight selected piece
//			HighlightSelectedPiece ( );
//
//			//Highlight tile
//			sprite.color = tileColors [ TileStateColor.SelectedPiece ];
//				
//			//Display goal area for white
//			if ( info.selectedPiece.color == PieceColor.White )
//				info.currentPlayer.goalArea.SetActive ( true );
//
//			//Store direction
//			int start, end;
//
//			//Check player
//			if ( currentPiece.owner == info.player1 ) 
//			{
//				//Section off tiles to move right
//				start = 2;
//				end = 6;
//			} 
//			else 
//			{
//				//Section off tiles to move left
//				start = 0;
//				end = 4;
//			}
//
//			//Track turn
//			bool additionalMove = false;
//
//			//Check neighboring tiles
//			for ( int i = start; i < end; i++ ) 
//			{
//				//Check for tile
//				if ( neighbors [ i ] != null )
//				{
//					//Check for piece 
//					if ( neighbors [ i ].currentPiece != null )
//					{
//						//Check for jump space
//						if ( neighbors [ i ].neighbors [ i ] != null && neighbors [ i ].neighbors [ i ].currentPiece == null && !currentPiece.IsPrevTile ( neighbors [ i ].neighbors [ i ] ) )
//						{
//							//Check for nonagression pacts
//							if ( neighbors [ i ].currentPiece.owner != info.currentPlayer && neighbors [ i ].currentPiece.nonagressionPartners.Contains ( currentPiece.color ) )
//								continue;
//
//							//Check if piece is friendly 
//							if ( neighbors [ i ].currentPiece.owner != info.currentPlayer && !Ability.AbilityList.HasPacifist ( info.selectedPiece, info ) && !Ability.AbilityList.HasPacifist ( neighbors [ i ].currentPiece, info ) )
//							{
//								//Highlight potential capture
//								neighbors [ i ].HighlightPotentialCapture ( );
//							}
//
//							//Allow turn to continue
//							if ( !info.beginningOfTurn )
//								additionalMove = true;
//
//							//Highlight potential jump
//							neighbors [ i ].neighbors [ i ].HighlightPotentialJump ( );
//						}
//					}
//					else
//					{
//						//Check if it is the beginning of the turn and for previous tile 
//						if ( info.beginningOfTurn && !currentPiece.IsPrevTile ( neighbors [ i ] ) )
//							neighbors [ i ].HighlightPotentialMove ( );
//					}
//				}
//			}
//
//			//Check for catapult ability
//			if ( info.beginningOfTurn && currentPiece.ability != null && currentPiece.ability.ID == Ability.AbilityList.Catapult.ID && !currentPiece.isJolted )
//			{
//				//Highlight catapult tiles
//				Ability.AbilityList.GetCatapult ( this, start, end, info );
//			}
//
//			//Check for sacrifice ability
//			if ( currentPiece.ability != null && currentPiece.ability.ID == Ability.AbilityList.Sacrifice.ID )
//			{
//				//Highlight any missed sacrifice tiles
//				Ability.AbilityList.GetSacrifice ( this, start, end, info );
//			}
//
//			//Check for teleport ability
//			if ( info.beginningOfTurn && currentPiece.ability != null && currentPiece.ability.ID == Ability.AbilityList.Teleport.ID && !currentPiece.isJolted )
//			{
//				//Highlight teleport tiles
//				Ability.AbilityList.GetTeleport ( this, start, end, 2 );
//			}
//
//			//Check for torus ability
//			if ( currentPiece.ability != null && currentPiece.ability.ID == Ability.AbilityList.Torus.ID && !currentPiece.isJolted )
//			{
//				//Get torus tiles
//				bool torusAdditionalMove = Ability.AbilityList.GetTorus ( this, start, end, info );
//
//				//Set any additional moves from torus
//				if ( !additionalMove )
//					additionalMove = torusAdditionalMove;
//			}
//
//			//Check if it is the beginning of the turn
//			if ( !info.beginningOfTurn )
//			{
//				//Check for addition moves
//				if ( additionalMove )
//				{
//					//Make the end turn button visible
//					info.currentPlayer.endButton.SetActive ( true );
//				}
//				else
//					info.EndTurn ( );
//			}
//		}	
//	}
//
//	/// <summary>
//	/// Moves the selected piece to the selected tile.
//	/// </summary>
//	private void MovePiece ( bool isJump )
//	{
//		//Store previous tile
//		info.selectedPiece.prevTile = info.selectedPiece.currentTile;
//
//		//Determine animation time
//		float moveTime = ANIMATE_TIME;
//		if ( isJump )
//			moveTime *= 2;
//
//		//Update tile and piece references
//		info.selectedPiece.currentTile.currentPiece = null;
//		info.selectedPiece.currentTile = this;
//		currentPiece = info.selectedPiece;
//
//		//Bring selected piece to the front
//		info.BringPieceToFront ( currentPiece );
//
//		//Clear board
//		info.ResetBoardColor ( );
//
//		//Mark that it is no longer the beginning of a turn
//		info.beginningOfTurn = false;
//		info.DisableAbilityButtons ( );
//
//		//Animate movement
//		currentPiece.transform.DOMove ( this.transform.position, moveTime )
//			.SetRecyclable ( )
//			.OnComplete ( () =>
//			{
//				//Check white reaching its goal
//				if ( currentPiece.color == PieceColor.White && ( ( info.currentPlayer == info.player1 && isP1GoalArea ) || ( info.currentPlayer == info.player2 && isP2GoalArea ) ) ) 
//				{
//					//End the game
//					info.WinGame ( );
//					return;
//				}
//
//				//Check follow up
//				if ( isJump )
//					SelectPiece ( );
//				else
//					info.EndTurn ( );
//			} );
//	}
//
//	/// <summary>
//	/// Move the selected piece by jumping another piece.
//	/// </summary>
//	private void JumpPiece ( )
//	{
//		//Find direction
//		int direction = 0;
//		for ( int i = 0; i < neighbors.Length; i++ )
//		{
//			//Check for selected tile
//			if ( info.selectedPiece.currentTile.neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ] == this )
//			{
//				//Store direction
//				direction = i;
//				break;
//			}
//		}
//
//		//Check piece being jumped
//		if ( info.selectedPiece.currentTile.neighbors [ direction ].state == TileState.PotentialCapture )
//		{
//			//Capture opponent
//			CapturePiece ( info.selectedPiece.currentTile.neighbors [ direction ] );
//		}
//		else
//		{
//			//Check for jolt ability
//			if ( info.selectedPiece.currentTile.neighbors [ direction ].currentPiece.owner == info.currentPlayer && info.selectedPiece.ability != null && info.selectedPiece.ability.ID == Ability.AbilityList.Jolt.ID && info.selectedPiece.currentTile.neighbors [ direction ].currentPiece.color != PieceColor.White )
//			{
//				//Jolt piece
//				Ability.AbilityList.UseJolt ( info.selectedPiece.currentTile.neighbors [ direction ], info );
//				
//				//Display prompt
//				info.currentPlayer.prompt.gameObject.SetActive ( true );
//				info.currentPlayer.prompt.text = info.selectedPiece.currentTile.neighbors [ direction ].currentPiece.color.ToString ( ) + " has been jolted!";
//				
//				//Change end turn button text
//				info.currentPlayer.endTurnText.text = "Next";
//			}
//		}
//
//		//Move piece
//		MovePiece ( true );
//	}
//
//	/// <summary>
//	/// Captures an opponent's piece.
//	/// </summary>
//	public void CapturePiece ( Tile t )
//	{
//		//Check for conversion ability
//		if ( info.selectedPiece.ability != null && info.selectedPiece.ability.ID == Ability.AbilityList.Conversion.ID && info.selectedPiece.ability.IsActive )
//		{
//			//Convert piece
//			Ability.AbilityList.UseConversion ( t.currentPiece, info );
//			
//			//Don't capture piece
//			return;
//		}
//		
//		//Check for armor ability
//		if ( t.currentPiece.ability != null && t.currentPiece.ability.ID == Ability.AbilityList.Armor.ID && t.currentPiece.ability.IsActive )
//		{
//			//Use armor
//			Ability.AbilityList.UseArmor ( t.currentPiece, info );
//			
//			//Don't capture piece
//			return;
//		}
//
//		//Play animation
//		Sequence s = DOTween.Sequence ( )
//			.AppendInterval ( ANIMATE_TIME )
//			.Append ( t.currentPiece.transform.DOScale ( new Vector3 ( 5, 5, 5 ), ANIMATE_TIME ) )
//			.Insert ( ANIMATE_TIME, t.currentPiece.sprite.DOFade ( 0, ANIMATE_TIME ) )
//			.SetRecyclable ( )
//			.OnComplete ( () =>
//			{
//				//Check for white capture
//				if ( t.currentPiece.color == PieceColor.White )
//				{
//					//End the game
//					info.WinGame ( );
//					return;
//				}
//				//Check if the captured piece has any abilities
//				if ( t.currentPiece.ability != null )
//				{
//					//Disable ability
//					info.DisableAbility ( t.currentPiece.ability.ID, t.currentPiece.owner );
//				}
//				
//				//Remove piece from opponent
//				t.currentPiece.owner.pieces.Remove ( t.currentPiece );
//				
//				//Delete piece
//				t.currentPiece.Capture ( );
//				
//				//Remove piece reference from the tile
//				t.currentPiece = null;
//				
//				//Check if the opponent has the Grim Reaper ability
//				if ( Ability.AbilityList.HasGrimReaper ( info.opponent ) )
//				{
//					//Add tile to the list of captured pieces
//					info.grimReaperTile = t;
//					
//					//Display Grim Reaper prompt
//					info.opponent.prompt.gameObject.SetActive ( true );
//					info.opponent.prompt.text = "Grim Reaper has been activated!";
//				}
//			} )
//			.Play ( );
//	}

	/// <summary>
	/// Highlight the tile for the appropriate action when the mouse hovers over it.
	/// </summary>
//	public void MouseEnter ( )
//	{ 
//		//Check state
//		switch ( state ) 
//		{
//			case TileState.FriendlyPiece:
//				//Check if it is the beginning of a turn
//				if ( info.beginningOfTurn )
//					HighlightSelectedPiece ( );	
//				break;
//			case TileState.PotentialMove:
//				//Highlight a move
//				HighlightSelectedMove ( );
//				break;
//			case TileState.PotentialJump:
//				//Highlight a jump
//				HighlightSelectedMove ( );
//				break;
//	        case TileState.PotentialAbility:
//				//Highlight an ability move
//				HighlightSelectedAbility ( );
//	            break;
//			case TileState.AbilityPieceSelection:
//				//Highlight a selection for an ability
//				HighlightSelectedAbilityPiece ( );
//
//				//Check for caboose
//				if ( info.abilityInUse.ID == Ability.AbilityList.Caboose.ID )
//				{
//					//Check for multiple cabooses on a tile
//					if ( info.multiCabooseTile )
//					{
//						//Reset board color
//						info.ResetBoardColor ( );
//
//						//Highlight each caboose
//						foreach ( CabooseList c in info.cabooseList )
//						{
//							//Highlight pieces
//							foreach ( Tile t in c.list )
//								t.HighlightAbilityPieceSelection ( );
//							
//							//Highlight the destination
//							c.destination.sprite.color = tileColors [ TileStateColor.AvailableMove ];
//						}
//						
//						//Highlight selected tile
//						HighlightSelectedAbilityPiece ( );
//
//						//Highlight caboose
//						foreach ( CabooseList c in info.cabooseList )
//							if ( this != info.abilityTileSelection [ 0 ] && c.list.Contains ( this ) )
//							    Ability.AbilityList.HighlightCaboose ( c );
//					}
//					else
//					{
//						//Reset the board
//						info.ResetBoardColor ( );
//						
//						//Get the list of tiles
//						Ability.AbilityList.GetCaboose ( info );
//						
//						//Highlight each caboose
//						foreach ( CabooseList c in info.cabooseList )
//						{
//							//Check tile
//							if ( c.list [ 0 ] == this )
//							{
//								//Highlight caboose
//								Ability.AbilityList.HighlightCaboose ( c );
//							}
//						}
//					}
//				}
//				break;
//		}
//	}

	/// <summary>
	/// Return the tile to its appropriate color when the mouse stops hovering over it.
	/// </summary>
//	public void MouseExit ( )
//	{
//		//Check state
//		switch ( state ) 
//		{
//			case TileState.FriendlyPiece:
//				//Check the beginning of the turn and for selected piece
//				if ( info.beginningOfTurn && currentPiece != info.selectedPiece )
//		 			HighlightPlayerPiece ( );
//				break;
//			case TileState.PotentialMove:
//				//Highlight a potential move
//				HighlightPotentialMove ( );
//				break;
//			case TileState.PotentialJump:
//				//Highlight a potential jump
//				HighlightPotentialJump ( );
//
//				//Highlight a potential capture
//				UnhighlightSelectedCapture ( );
//				break;
//			case TileState.PotentialAbility:
//				//Highlight a potential ability move
//				HighlightPotentialAbility ( );
//
//				//Check for catapult tiles
//				if ( info.selectedPiece.ability != null && info.selectedPiece.ability.ID == Ability.AbilityList.Catapult.ID )
//				{
//					//Find middle tiles
//					for ( int i = 0; i < neighbors.Length; i++ )
//					{
//						//Check for catapult tile
//						if ( info.selectedPiece.currentTile.neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ].neighbors [ i ] == this )
//						{
//							//Check for capture on the first tile
//							if ( info.selectedPiece.currentTile.neighbors [ i ].state == TileState.PotentialCapture )
//								info.selectedPiece.currentTile.neighbors [ i ].HighlightPotentialCapture ( );
//							
//							//Check for capture on the second tile
//							if ( info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ].state == TileState.PotentialCapture )
//								info.selectedPiece.currentTile.neighbors [ i ].neighbors [ i ].HighlightPotentialCapture ( );
//							
//							//End search
//							break;
//						}
//					}
//				}
//
//				//Unhighlight any captures
//				if ( isTorusJump || ( info.sacrificeArmorTile != null && info.sacrificeArmorTile == this ) )
//					UnhighlightSelectedCapture ( );
//				break;
//			case TileState.AbilityPieceSelection:
//				//Highlight a potential selection for an ability
//				HighlightAbilityPieceSelection ( );
//
//				//Check for caboose
//				if ( info.abilityInUse.ID == Ability.AbilityList.Caboose.ID )
//				{
//					//Reset board
//					info.ResetBoardColor ( );
//
//					//Check for multiple cabooses on a tile
//					if ( info.multiCabooseTile )
//					{
//						//Highlight each caboose
//						foreach ( CabooseList c in info.cabooseList )
//						{
//							//Highlight pieces
//							foreach ( Tile t in c.list )
//								t.HighlightAbilityPieceSelection ( );
//							
//							//Highlight the destination
//							c.destination.sprite.color = tileColors [ TileStateColor.AvailableMove ];
//						}
//						
//						//Highlight selected tile
//						info.abilityTileSelection [ 0 ].HighlightSelectedAbilityPiece ( );
//						info.abilityTileSelection [ 0 ].state = TileState.AbilitySelectedPiece;
//
//						//Check for selected caboose
//						if ( info.selectedCaboose != null )
//						{
//							//Select piece
//							foreach ( Tile t in info.selectedCaboose.list )
//								t.state = TileState.AbilitySelectedPiece;
//							
//							//Highlight caboose
//							Ability.AbilityList.HighlightCaboose ( info.selectedCaboose );
//						}
//
//					}
//					else
//					{
//						//Get the list of tiles
//						Ability.AbilityList.GetCaboose ( info );
//
//						//Check for selected caboose
//						if ( info.selectedCaboose != null )
//						{
//							//Select piece
//							info.abilityTileSelection [ 0 ].state = TileState.AbilitySelectedPiece;
//							
//							//Highlight caboose
//							Ability.AbilityList.HighlightCaboose ( info.selectedCaboose );
//						}
//					}
//				}
//				break;
//		}
//	}

	/// <summary>
	/// Perform the appropriate action when the mouse clicks the tile.
	/// </summary>
//	public void MouseClick ( )
//	{
//		//Check for tutorial
//		if ( Tutorial.isPlaying )
//		{
//			//Check for current object
//			if ( info.tutor.CheckObject ( this.gameObject ) )
//				info.tutor.NextMove ( );
//			else
//				return;
//		}
//
//		//Play SFX
//		if ( !( Tutorial.isPlaying && info.currentPlayer == info.player2 ) )
//			SFXManager.instance.Click ( );
//
//		//Check state
//		switch ( state ) 
//		{
//			case TileState.FriendlyPiece:
//				//Check if piece is selected
//				if ( currentPiece != info.selectedPiece )
//					SelectPiece ( );
//				else
//				{
//					//Check if the piece is jolted and has additional moves
//					if ( !info.selectedPiece.isJolted )
//					{
//						//Unselect piece
//						info.selectedPiece = null;
//						
//						//Reset board
//						info.ResetBoardColor ( );
//						
//						//Highlight pieces
//						info.HighlightCurrentPlayerPieces ( );
//						
//						//Highlight current tile
//						HighlightSelectedPiece ( );
//					}
//				}
//				break;
//			case TileState.PotentialMove:
//				//Move the piece
//				MovePiece ( false );
//				break;
//			case TileState.PotentialJump:
//				//Jump the piece
//				JumpPiece ( );
//				break;
//			case TileState.PotentialAbility:
//				//Check ability
//				if ( info.selectedPiece.ability.ID == Ability.AbilityList.Catapult.ID )
//				{
//					//Use catapult
//					Ability.AbilityList.UseCatapult ( info.selectedPiece, this, info );
//				}
//				else if ( info.selectedPiece.ability.ID == Ability.AbilityList.Sacrifice.ID )
//				{
//					//Use sacrifice
//					Ability.AbilityList.UseSacrifice ( info.selectedPiece );
//
//					//Check sacrifice armor tile
//					if ( info.sacrificeArmorTile != null && info.sacrificeArmorTile == this )
//						JumpPiece ( );
//					else
//						MovePiece ( false );
//				}
//				else if ( info.selectedPiece.ability.ID == Ability.AbilityList.Teleport.ID )
//				{
//					//Use teleport
//					Ability.AbilityList.UseTeleport ( info.selectedPiece, this, info );
//				}
//				else if ( info.selectedPiece.ability.ID == Ability.AbilityList.Torus.ID )
//				{
//					//Use torus
//					Ability.AbilityList.UseTorus ( info.selectedPiece, this, info );
//				}
//				break;
//			case TileState.AbilityPieceSelection:
//				//Select piece
//				state = TileState.AbilitySelectedPiece;
//
//				//Add tile to selection list
//				info.abilityTileSelection.Add ( this );
//
//				//Check ability
//				if ( info.abilityInUse.ID == Ability.AbilityList.Caboose.ID )
//				{
//					//Check for multiple cabooses at one tile
//					if ( info.multiCabooseTile )
//					{
//						//Check if a caboose is already selected
//						if ( info.abilityTileSelection.Count > 2 )
//						{
//							//Remove previous caboose
//							info.abilityTileSelection.RemoveAt ( 1 );
//
//							//Reset board color
//							info.ResetBoardColor ( );
//							
//							foreach ( CabooseList c in info.cabooseList )
//							{
//								//Highlight pieces
//								foreach ( Tile t in c.list )
//									t.HighlightAbilityPieceSelection ( );
//								
//								//Highlight the destination
//								c.destination.sprite.color = tileColors [ TileStateColor.AvailableMove ];
//							}
//							
//							//Highlight selected tile
//							HighlightSelectedAbilityPiece ( );
//							state = TileState.AbilitySelectedPiece;
//
//							//Highlight caboose
//							foreach ( CabooseList c in info.cabooseList )
//								if ( this != info.abilityTileSelection [ 0 ] && c.list.Contains ( this ) )
//									Ability.AbilityList.HighlightCaboose ( c );
//						}
//
//						//Store selected caboose
//						foreach ( CabooseList c in info.cabooseList )
//							if ( this != info.abilityTileSelection [ 0 ] && c.list.Contains ( this ) )
//								info.selectedCaboose = c;
//
//						//Enable accept button
//						info.currentPlayer.acceptButton.interactable = true;
//					}
//					else
//					{
//						//Check a caboose already selected
//						if ( info.abilityTileSelection.Count > 1 )
//						{
//							//Remove the first tile from the selection
//							info.abilityTileSelection.RemoveAt ( 0 );
//
//							//Reset the board
//							info.ResetBoardColor ( );
//						
//							//Get the list of tiles
//							Ability.AbilityList.GetCaboose ( info );
//						}
//
//						//Store the caboose
//						List < CabooseList > l = new List < CabooseList > ( );
//						
//						//Check caboose list
//						foreach ( CabooseList c in info.cabooseList )
//							if ( c.list [ 0 ] == this )
//								l.Add ( c );
//						
//						//Check for multiple cabooses
//						if ( l.Count > 1 )
//						{
//							//Store that a tile has multiple caboose
//							info.multiCabooseTile = true;
//							
//							//Remove other cabooses
//							List < CabooseList > temp = new List < CabooseList > ( );
//							foreach ( CabooseList c in info.cabooseList )
//								if ( !l.Contains ( c ) )
//									temp.Add ( c );
//							foreach ( CabooseList c in temp )
//									info.cabooseList.Remove ( c );
//							
//							//Reset board color
//							info.ResetBoardColor ( );
//
//							foreach ( CabooseList c in info.cabooseList )
//							{
//								//Highlight pieces
//								foreach ( Tile t in c.list )
//									t.HighlightAbilityPieceSelection ( );
//
//								//Highlight the destination
//								c.destination.sprite.color = tileColors [ TileStateColor.AvailableMove ];
//							}
//
//							//Highlight selected tile
//							HighlightSelectedAbilityPiece ( );
//							state = TileState.AbilitySelectedPiece;
//						}
//						else
//						{
//							//Highlight the selected caboose
//							foreach ( Tile t in l [ 0 ].list )
//								t.HighlightSelectedAbilityPiece ( );
//							
//							//Highlight the destination
//							l [ 0 ].destination.HighlightSelectedMove ( );
//							
//							//Store the caboose
//							info.selectedCaboose = l [ 0 ];
//							
//							//Enable accept button
//							info.currentPlayer.acceptButton.interactable = true;
//						}
//					}
//				}
//				else if ( info.abilityInUse.ID == Ability.AbilityList.MadHatter.ID )
//				{
//					//Check selection
//					if ( info.abilityTileSelection.Count > 2 )
//					{
//						//Remove the first tile from the selection
//						info.abilityTileSelection [ 0 ].HighlightAbilityPieceSelection ( );
//						info.abilityTileSelection.RemoveAt ( 0 );
//					}
//					else if ( info.abilityTileSelection.Count < 2 )
//					{
//						//Disable the accept button
//						info.currentPlayer.acceptButton.interactable = false;
//					}
//					else 
//					{
//						//Enable the accept button
//						info.currentPlayer.acceptButton.interactable = true;
//					}
//				}
//				else if ( info.abilityInUse.ID == Ability.AbilityList.NonagressionPact.ID )
//				{
//					//Check selection
//					if ( info.abilityTileSelection.Count > 1 )
//					{
//						//Check first piece if it is friendly
//						if ( info.abilityTileSelection [ 0 ].currentPiece.owner == currentPiece.owner )
//						{
//							//Remove the first tile from the selection
//							info.abilityTileSelection [ 0 ].HighlightAbilityPieceSelection ( );
//							info.abilityTileSelection.RemoveAt ( 0 );
//						}
//
//						//Check for second tile
//						if ( info.abilityTileSelection.Count > 2 )
//						{
//							//Remove the second tile from the selection
//							info.abilityTileSelection [ 1 ].HighlightAbilityPieceSelection ( );
//							info.abilityTileSelection.RemoveAt ( 1 );
//						}
//
//						//Check if two pieces are selected
//						if ( info.abilityTileSelection.Count == 2 )
//						{
//							//Enable the accept button
//							info.currentPlayer.acceptButton.interactable = true;
//						}
//						else
//						{
//							//Disable the accept button
//							info.currentPlayer.acceptButton.interactable = false;
//						}
//					}
//					else
//					{
//						//Disable the accept button
//						info.currentPlayer.acceptButton.interactable = false;
//					}
//				}
//
//				break;
//			case TileState.AbilitySelectedPiece:
//				//Unselect piece
//				state = TileState.AbilityPieceSelection;
//
//				//Remove tile from selection list
//				info.abilityTileSelection.Remove ( this );
//
//				//Check for caboose
//				if ( info.abilityInUse.ID == Ability.AbilityList.Caboose.ID )
//				{
//					//Unselect caboose
//					info.selectedCaboose = null;
//
//					//Check for multiple cabooses on one tile
//					if ( info.multiCabooseTile )
//					{
//						//Check if the multiple caboose tile is still selected
//						bool unselect = true;
//						foreach ( CabooseList c in info.cabooseList )
//							if ( !c.list.Contains ( this ) )
//								unselect = false;
//
//						//Check if unselecting multiple caboose tile
//						if ( unselect )
//						{
//							//Reset board
//							info.ResetBoardColor ( );
//
//							//Get a new list of tiles
//							Ability.AbilityList.GetCaboose ( info );
//
//							//Store that a multiple caboose tile is no longer selected
//							info.multiCabooseTile = false;
//						}
//					}
//				}
//
//				//Deactivate the accept button
//				info.currentPlayer.acceptButton.interactable = false;
//				break;
//		}
//	}
}

public enum TileState
{
	Default,
	AvailableUnit,
	AvailableUnitHover,
	SelectedUnit,
	AvailableMove,
	AvailableMoveHover,
	AvailableMoveCapture,
	AvailableMoveCaptureHover,
	AvailableCapture,
	AvailableCaptureHover,
	AvailableSpecial,
	AvailableSpecialHover,
	AvailableSpecialCapture,
	AvailableSpecialCaptureHover,
	AvailableCommand,
	AvailableCommandHover,
	AvailableCommandSelected,
	ConflictedTile,
	ConflictedTileHover,
	Error
}

public enum TileStateColor
{
	Normal,
	FriendlyPiece,
	FriendlyPieceHover,
	SelectedPiece,
	AvailableMove,
	AvailableMoveHover,
	AvailableCapture,
	AvailableCaptureHover,
	AbilityMove,
	AbilityMoveHover,
	AvailableWin,
	AvailableWinHover
}