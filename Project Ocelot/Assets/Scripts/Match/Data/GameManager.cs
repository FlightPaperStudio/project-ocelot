using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{
	// UI information
	public UIManager UI;

	// Board information
	public Board board;

	// Player information
	public Player [ ] players;
	public Player currentPlayer
	{
		get;
		private set;
	}
	private int playerIndex = 0;

	// Turn information
	public bool isStartOfTurn
	{
		get;
		private set;
	}
	public struct TurnAnimation
	{
		public Tween tween;
		public bool isAppend;

		public TurnAnimation ( Tween _tween, bool _isAppend )
		{
			tween = _tween;
			isAppend = _isAppend;
		}
	}
	public List<TurnAnimation> startOfTurnAnimations = new List<TurnAnimation> ( );
	public List<TurnAnimation> endOfTurnAnimations = new List<TurnAnimation> ( );
	public List<TurnAnimation> postTurnAnimations = new List<TurnAnimation> ( );
	private const float ANIMATION_BUFFER = 0.1f;
	
	// Unit information
	public Unit [ ] unitPrefabs;
	public Unit selectedUnit
	{
		get;
		private set;
	}
	public MoveData selectedMove
	{
		get;
		private set;
	}

	#region Start of Match Setup

	/// <summary>
	/// Sets up the proper data structures before the match begins.
	/// </summary>
	private void Start ( )
	{
		// Start the match
		StartMatch ( );
	}

	/// <summary>
	/// Starts the match.
	/// </summary>
	private void StartMatch ( )
	{
		// Set players
		for ( int i = 0; i < players.Length; i++ )
		{
			// Set team name
			players [ i ].name = MatchSettings.playerSettings [ i ].name;

			// Set team color
			players [ i ].team = MatchSettings.playerSettings [ i ].teamColor;

			// Set goal area
			players [ i ].startArea.SetColor ( players [ i ].team );

			// Set special IDs
			players [ i ].specialIDs = MatchSettings.playerSettings [ i ].specialIDs.ToArray ( );

			// Spawn units
			for ( int j = 0; j < MatchSettings.playerSettings [ i ].formation.Length; j++ )
			{
				// Create unit
				Unit u = Instantiate ( unitPrefabs [ MatchSettings.playerSettings [ i ].formation [ j ] + 1 ], players [ i ].transform );

				// Set unit info
				u.GM = this;
				u.instanceID = ( i * 10 ) + j; // Create a unique instance ID for this unit
				u.team = players [ i ];

				// Set unit team color
				u.SetTeamColor ( players [ i ].team );

				// Set unit direction
				if ( players [ i ].direction == Player.Direction.RightToLeft || players [ i ].direction == Player.Direction.BottomRightToTopLeft || players [ i ].direction == Player.Direction.TopRightToBottomLeft )
					u.sprite.flipX = true;

				// Position unit to starting tile
				u.transform.position = players [ i ].startArea.tiles [ j ].transform.position;
				u.currentTile = players [ i ].startArea.tiles [ j ];
				players [ i ].startArea.tiles [ j ].currentUnit = u;

				// Add unit to team
				players [ i ].units.Add ( u );
			}

			// Check for blue team
			if ( players [ i ].team == Player.TeamColor.Blue )
			{
				// Set the blue team as the starting player
				currentPlayer = players [ i ];
				playerIndex = i;
			}
		}

		// Set up UI
		UI.Initialize ( players );

		// Begin match
		StartTurn ( );
	}

	#endregion

	#region Start of Turn Setup

	/// <summary>
	/// Starts a player's turn.
	/// </summary>
	private void StartTurn ( )
	{
		// Clear previous animation
		startOfTurnAnimations.Clear ( );

		// Start new turn animation
		StartCoroutine ( StartTurnCoroutine ( ) );
	}

	/// <summary>
	/// Starts a player's turn with an animation.
	/// </summary>
	private IEnumerator StartTurnCoroutine ( )
	{
		// Wait until animation is completed
		yield return UI.splash.Slide ( currentPlayer.name + "'s Turn", Util.TeamColor ( currentPlayer.team ), true ).WaitForCompletion ( );

		// Set cooldowns
		UpdateCooldowns ( );

		// Get moves
		GetTeamMoves ( );

		// Check to make sure the player has moves available
		if ( ForfeitCheck ( ) )
		{
			// Have the current player forfeit
			ForfeitMatch ( );
		}

		// Wait until the start of turn animations are complete
		yield return PlayStartOfTurnAnimations ( ).WaitForCompletion ( );

		// Display units for selection
		DisplayAvailableUnits ( );
		BringPlayerToTheFront ( currentPlayer );
		selectedUnit = null;

		// Start turn timer
		if ( MatchSettings.turnTimer )
			UI.timer.StartTimer ( );

		// Begin turn
		isStartOfTurn = true;
	}

	/// <summary>
	/// Updates all of the player's heroes' cooldowns and durations.
	/// </summary>
	private void UpdateCooldowns ( )
	{
		// Access each of the player's units
		foreach ( Unit u in currentPlayer.units )
		{
			// Set cooldowns
			if ( u is HeroUnit )
			{
				HeroUnit h = u as HeroUnit;
				h.Cooldown ( );
			}
		}
	}

	/// <summary>
	/// Gets all of the available moves for each of the current player's units before any moves have been made.
	/// </summary>
	public void GetTeamMoves ( )
	{
		// Access each of the player's units
		foreach ( Unit u in currentPlayer.units )
		{
			// Set move list
			u.FindMoves ( u.currentTile, null, false );
			u.MoveConflictCheck ( );
		}
	}

	/// <summary>
	/// Plays all of the start of turn animations before the player's turn begins.
	/// </summary>
	private Sequence PlayStartOfTurnAnimations ( )
	{
		// Create the animation
		Sequence s = DOTween.Sequence ( );

		// Add animations
		for ( int i = 0; i < startOfTurnAnimations.Count; i++ )
		{
			// Check animation join type and add animation to the queue
			if ( startOfTurnAnimations [ i ].isAppend )
				s.Append ( startOfTurnAnimations [ i ].tween );
			else
				s.Join ( startOfTurnAnimations [ i ].tween );

			// Check for next animation and add a buffer between
			if ( i + 1 < startOfTurnAnimations.Count && startOfTurnAnimations [ i + 1 ].isAppend )
				s.AppendInterval ( ANIMATION_BUFFER );
		}

		// Add delay at the end of the animation
		s.AppendInterval ( ANIMATION_BUFFER );

		// Return the sequence so that the code can wait for its completion
		return s;
	}

	/// <summary>
	/// Displays the available units for the current player.
	/// </summary>
	public void DisplayAvailableUnits ( )
	{
		// Highlight each unit
		foreach ( Unit u in currentPlayer.units )
		{
			u.currentTile.SetTileState ( TileState.AvailableUnit );
		}
	}

	#endregion

	#region Mid-Turn Controls

	/// <summary>
	/// Selects the current unit and displays any and all available moves for the unit.
	/// </summary>
	public void SelectUnit ( Unit u )
	{
		// Check for previously selected unit
		if ( selectedUnit != null && isStartOfTurn && !UI.timer.isOutOfTime )
		{
			// Reset any previous selected units
			board.ResetTiles ( );
			DisplayAvailableUnits ( );
		}

		// Set unit as the currently selected unit
		selectedUnit = u;

		// Set that there are no currently selected moves
		selectedMove = null;

		// Display unit HUD
		UI.unitHUD.DisplayUnit ( selectedUnit );

		// Highlight the tile of the selected unit
		selectedUnit.currentTile.SetTileState ( TileState.SelectedUnit );
		BringUnitToTheFront ( selectedUnit );

		// Display the unit's available moves
		DisplayAvailableMoves ( null );
	}

	/// <summary>
	/// Displays the available moves for the selected unit and any selected prerequisite moves.
	/// </summary>
	private void DisplayAvailableMoves ( MoveData prerequisite )
	{
		// Get only the moves for the prerequisite
		foreach ( MoveData m in selectedUnit.moveList.FindAll ( x => x.prerequisite == prerequisite ) )
		{
			// Check if conflicted
			if ( m.isConflicted )
			{
				// Set tile state and highlight tile
				m.tile.SetTileState ( TileState.ConflictedTile );
			}
			else
			{
				// Check move type
				switch ( m.type )
				{
				// Basic moves
				case MoveData.MoveType.Move:
				case MoveData.MoveType.MoveToWin:
					// Set tile state and highlight tile
					m.tile.SetTileState ( TileState.AvailableMove );
					break;

				// Basic jump
				case MoveData.MoveType.Jump:
				case MoveData.MoveType.JumpToWin:
					// Set tile state and highlight tile
					m.tile.SetTileState ( TileState.AvailableMove );
					break;

				// Jump and capture enemy
				case MoveData.MoveType.Attack:
				case MoveData.MoveType.AttackToWin:
					// Set tile state and highlight tile
					m.tile.SetTileState ( TileState.AvailableMoveAttack );

					// Set tile state and highlight tile for the unit available for attack
					foreach ( Tile t in m.attacks )
						t.SetTileState ( TileState.AvailableAttack );
					break;

				// Special ability move
				case MoveData.MoveType.Special:
					// Set tile state and highlight tile
					m.tile.SetTileState ( TileState.AvailableSpecial );
					break;

				// Special ability move and attack enemy
				case MoveData.MoveType.SpecialAttack:
					// Set tile state and highlight tile
					m.tile.SetTileState ( TileState.AvailableSpecialAttack );

					// Set tile state and highlight tile for the unit available for attack
					foreach ( Tile t in m.attacks )
						t.SetTileState ( TileState.AvailableAttack );
					break;
				}
			}
		}
	}

	/// <summary>
	/// Selects the move for the currently selected unit to make.
	/// </summary>
	public void SelectMove ( Tile t, bool isConflict = false, bool isLeftClick = true )
	{
		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( true );

		// Prevent any command usage
		UI.unitHUD.DisableCommandButtons ( );

		// Clear previous moves and units
		board.ResetTiles ( selectedUnit.currentTile );

		// Get move data
		MoveData data;
		if ( isConflict )
		{
			if ( isLeftClick )
				data = selectedUnit.moveList.Find ( x => x.tile == t && x.prerequisite == selectedMove && ( x.type != MoveData.MoveType.Special && x.type != MoveData.MoveType.SpecialAttack ) );
			else
				data = selectedUnit.moveList.Find ( x => x.tile == t && x.prerequisite == selectedMove && ( x.type == MoveData.MoveType.Special || x.type == MoveData.MoveType.SpecialAttack ) );
		}
		else
		{
			data = selectedUnit.moveList.Find ( x => x.tile == t && x.prerequisite == selectedMove );
		}

		// Store selected move
		selectedMove = data;

		// Display any additional moves
		DisplayAvailableMoves ( selectedMove );
	}

	/// <summary>
	/// Executes all of the current moves selected.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void ExecuteMove ( )
	{
		// End the player's turn
		EndTurn ( );
	}

	/// <summary>
	/// Removes all current moves selected.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void CancelMove ( )
	{
		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false );

		// Remove selected move
		selectedMove = null;

		// Reset board
		SelectUnit ( selectedUnit );
	}

	/// <summary>
	/// Continues a player's turn after a unit jumps.
	/// </summary>
	public void ContinueTurn ( )
	{
		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( true );

		// Resume turn timer
		if ( MatchSettings.turnTimer )
		{
			// Check if out of time
			if ( UI.timer.isOutOfTime )
				EndTurn ( );
			else
				UI.timer.ResumeTimer ( );
		}

		// Reset board to make only the selected unit interactable
		board.ResetTiles ( );

		// Continue the selected unit's turn
		SelectUnit ( selectedUnit );
	}

	#endregion

	#region End of Turn Setup

	/// <summary>
	/// Ends the current player's turn.
	/// </summary>
	private void EndTurn ( )
	{
		// Clear animations
		endOfTurnAnimations.Clear ( );
		postTurnAnimations.Clear ( );

		// Create moves list
		GetMoves ( selectedMove );

		// Clear board
		board.ResetTiles ( );

		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false );

		// Pause turn timer
		if ( MatchSettings.turnTimer )
			UI.timer.PauseTimer ( );

		// Play animations
		StartCoroutine ( EndTurnCoroutine ( ) );
	}

	/// <summary>
	/// Ends the current player's turn and begins the next player's turn with an animation.
	/// </summary>
	/// <returns></returns>
	private IEnumerator EndTurnCoroutine ( )
	{
		// Play move animations
		yield return PlayEndOfTurnAnimations ( ).WaitForCompletion ( );

		// Play post-turn animations
		yield return PlayPostTurnAnimations ( ).WaitForCompletion ( );

		// Hide unit HUD
		UI.unitHUD.HideHUD ( );

		// Reset tiles from previous turn
		board.ResetTiles ( );

		// Check for winner
		if ( WinnerCheck ( ) )
		{
			// Display the winner
			UI.WinPrompt ( currentPlayer );
		}
		else
		{
			// Start the next player's turn
			currentPlayer = GetNextPlayer ( );
			StartTurn ( );
		}
	}

	/// <summary>
	/// Creates the list of animations to play during a unit's move.
	/// </summary>
	private void GetMoves ( MoveData move )
	{
		// Check for previous moves
		if ( move.prerequisite != null )
			GetMoves ( move.prerequisite );

		// Get the move
		selectedUnit.MoveUnit ( move );
	}

	/// <summary>
	/// Plays all of the end of turn animations before the player's turn begins.
	/// </summary>
	private Sequence PlayEndOfTurnAnimations ( )
	{
		// Create the animation
		Sequence s = DOTween.Sequence ( );

		// Add delay at the start of the animation
		s.AppendInterval ( ANIMATION_BUFFER );

		// Add animations
		for ( int i = 0; i < endOfTurnAnimations.Count; i++ )
		{
			// Check animation join type and add animation to the queue
			if ( endOfTurnAnimations [ i ].isAppend )
				s.Append ( endOfTurnAnimations [ i ].tween );
			else
				s.Join ( endOfTurnAnimations [ i ].tween );

			// Check for next animation and add a buffer between
			if ( i + 1 < endOfTurnAnimations.Count && endOfTurnAnimations [ i + 1 ].isAppend )
				s.AppendInterval ( ANIMATION_BUFFER );
		}

		// Add delay at the end of the animation
		s.AppendInterval ( ANIMATION_BUFFER );

		// Return the sequence so that the code can wait for its completion
		return s;
	}

	/// <summary>
	/// Gets the next player the turn order.
	/// </summary>
	private Player GetNextPlayer ( )
	{
		// Move to the next player in the turn order
		for ( int i = playerIndex + 1; i < players.Length; i++ )
		{
			// Check if the player is still in the game
			if ( players [ i ].units.Count > 0 )
			{
				// Set current player
				playerIndex = i;
				return players [ i ];
			}
		}

		// Continue search for the next player
		for ( int i = 0; i < playerIndex; i++ )
		{
			// Check if the player is still in the game
			if ( players [ i ].units.Count > 0 )
			{
				// Set current player
				playerIndex = i;
				return players [ i ];
			}
		}

		// Return that the next player was not found 
		return null;
	}

	#endregion

	#region End of Match Setup

	/// <summary>
	/// Checks to make sure that the player has at least one available move.
	/// Returns true if the player has no available moves and is forced to forfeit.
	/// </summary>
	private bool ForfeitCheck ( )
	{
		// Check each unit's move list until at least one move is found
		foreach ( Unit u in currentPlayer.units )
			if ( u.moveList.Count > 0 )
				return false;

		// Return that no moves were found
		return true;
	}

	private void ForfeitMatch ( )
	{
		LoseMatch ( currentPlayer );
		EndTurn ( );
	}

	/// <summary>
	/// Eliminates a player from the match.
	/// </summary>
	public void LoseMatch ( Player p )
	{
		// Display the player being eliminated
		UI.hudDic [ p ].DisplayElimination ( );

		// Remove player from match
		foreach ( Unit u in p.units )
		{
			// Capture all other units
			u.GetAttacked ( true );
		}
	}

	/// <summary>
	/// Checks if there are multiple players still competing in the match.
	/// Returns false if more than one player is still competing.
	/// </summary>
	private bool WinnerCheck ( )
	{
		// Tracking info
		int playerCount = 0;

		// Check each player to see if they have units remaining
		foreach ( Player p in players )
			if ( p.units.Count > 0 )
				playerCount++;

		// Return if there is a winner
		return playerCount == 1;
	}

	/// <summary>
	/// Eliminates all other players then the provided winner.
	/// </summary>
	public void WinMatch ( Player winner )
	{
		// Eliminate each player
		foreach ( Player p in players )
			if ( p != winner )
				LoseMatch ( p );
	}

	/// <summary>
	/// Plays any and all of the after turn animations before the next player's turn begins.
	/// </summary>
	private Sequence PlayPostTurnAnimations ( )
	{
		// Create the animation
		Sequence s = DOTween.Sequence ( );

		// Add delay at the start of the animation
		s.AppendInterval ( ANIMATION_BUFFER );

		// Add additional "delay" for the tweens to join at
		s.AppendInterval ( 0f );

		// Add animations
		for ( int i = 0; i < postTurnAnimations.Count; i++ )
		{
			// Check animation join type and add animation to the queue
			if ( postTurnAnimations [ i ].isAppend )
				s.Append ( postTurnAnimations [ i ].tween );
			else
				s.Join ( postTurnAnimations [ i ].tween );

			// Check for next animation and add a buffer between
			if ( i + 1 < postTurnAnimations.Count && postTurnAnimations [ i + 1 ].isAppend )
				s.AppendInterval ( ANIMATION_BUFFER );
		}

		// Add delay at the end of the animation
		s.AppendInterval ( ANIMATION_BUFFER );

		// Return the sequence so that the code can wait for its completion
		return s;
	}

	#endregion

	/// <summary>
	/// Brings a player's unit sprites to the front layer.
	/// </summary>
	private void BringPlayerToTheFront ( Player p )
	{
		// Change the sprite layer for each player's units
		for ( int i = 0; i < players.Length; i++ )
		{
			// Check if the player is being brought to the front
			if ( players [ i ] == p )
			{
				// Bring each of the player's units to the front layer
				for ( int j = 0; j < players [ i ].units.Count; j++ )
					players [ i ].units [ j ].sprite.sortingOrder = 1;
			}
			else
			{
				// Bring each of the player's units to the default layer
				for ( int j = 0; j < players [ i ].units.Count; j++ )
					players [ i ].units [ j ].sprite.sortingOrder = 0;
			}
		}
	}

	/// <summary>
	/// Brings one of the current player's unit's sprite to the front layer.
	/// </summary>
	private void BringUnitToTheFront ( Unit u )
	{
		// Set each of the current player's units to the base layer
		for ( int i = 0; i < currentPlayer.units.Count; i++ )
			currentPlayer.units [ i ].sprite.sortingOrder = 1;

		// Bring the specified unit's sprite to the front layer
		u.sprite.sortingOrder = 2;
	}
}
