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
	public List<TurnAnimation> animationQueue = new List<TurnAnimation> ( );
	public struct PostTurnAnimation
	{
		public Unit unit;
		public Player owner;
		public TurnAnimation [ ] animation;

		public PostTurnAnimation ( Unit _unit, Player _owner, params TurnAnimation [ ] _animation )
		{
			unit = _unit;
			owner = _owner;
			animation = _animation;
		}
	}
	public List<PostTurnAnimation> postAnimationQueue = new List<PostTurnAnimation> ( );
	public List<Unit> unitQueue = new List<Unit> ( );
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
			players [ i ].PlayerName = MatchSettings.playerSettings [ i ].PlayerName;

			// Set team color
			players [ i ].Team = MatchSettings.playerSettings [ i ].Team;

			// Set goal area
			players [ i ].startArea.SetColor ( players [ i ].Team );

			// Set special IDs
			players [ i ].specialIDs = MatchSettings.playerSettings [ i ].heroIDs.ToArray ( );

			// Spawn units
			for ( int j = 0; j < MatchSettings.playerSettings [ i ].Formation.Length; j++ )
			{
				// Check for unit
				if ( MatchSettings.playerSettings [ i ].Formation [ j ] > MatchSettings.NO_UNIT )
				{
					// Create unit
					Unit u = Instantiate ( unitPrefabs [ MatchSettings.playerSettings [ i ].Formation [ j ] + 1 ], players [ i ].transform );

					// Set unit info
					u.GM = this;
					u.instanceID = ( i * 10 ) + j; // Create a unique instance ID for this unit
					u.owner = players [ i ];

					// Set unit team color
					u.SetTeamColor ( players [ i ].Team );

					// Set unit direction
					Util.OrientSpriteToDirection ( u.sprite, players [ i ].TeamDirection );

					// Position unit to starting tile
					u.transform.position = players [ i ].startArea.tiles [ j ].transform.position;
					u.currentTile = players [ i ].startArea.tiles [ j ];
					players [ i ].startArea.tiles [ j ].currentUnit = u;

					// Add unit to team
					players [ i ].units.Add ( u );
				}
			}

			// Add standard KO delegate to each unit
			if ( players [ i ].standardKOdelegate != null )
				foreach ( Unit u in players [ i ].units )
					u.koDelegate += players [ i ].standardKOdelegate;

			// Check for blue team
			if ( players [ i ].Team == Player.TeamColor.BLUE )
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
		// Clear previous animation queues
		animationQueue.Clear ( );
		postAnimationQueue.Clear ( );

		// Begin turn
		isStartOfTurn = true;

		// Start new turn animation
		StartCoroutine ( StartTurnCoroutine ( ) );
	}

	/// <summary>
	/// Starts a player's turn with an animation.
	/// </summary>
	private IEnumerator StartTurnCoroutine ( )
	{
		// Wait until animation is completed
		yield return UI.splash.Slide ( currentPlayer.PlayerName + "'s Turn", Util.TeamColor ( currentPlayer.Team ), true ).WaitForCompletion ( );

		// Set cooldowns
		UpdateUnitCountdowns ( );

		// Set durations
		UpdateTileObjectDurations ( );

		// Wait until the start of turn animations are complete
		yield return PlayAnimationQueue ( ).WaitForCompletion ( );
		
		// Wait until the post-start of turn animations are complete
		yield return PlayPostAnimationQueue ( ).WaitForCompletion ( );

		// Clear previous animation queues
		animationQueue.Clear ( );
		postAnimationQueue.Clear ( );

		// Check for winner
		if ( WinnerCheck ( ) )
		{
			// Display the winner
			UI.WinPrompt ( currentPlayer );
		}

		// Get moves
		GetTeamMoves ( );

		// Display units for selection
		DisplayAvailableUnits ( );
		BringPlayerToTheFront ( currentPlayer );
		selectedUnit = null;
		selectedMove = null;

		// Start turn timer
		if ( MatchSettings.turnTimer )
			UI.timer.StartTimer ( );
	}

	/// <summary>
	/// Updates all of the player's unit's status effects, ability cooldowns, and ability durations.
	/// </summary>
	private void UpdateUnitCountdowns ( )
	{
		// Access each of the player's units
		foreach ( Unit u in currentPlayer.units )
		{
			// Set status effects
			u.status.UpdateDurations ( );

			// Set cooldowns
			if ( u is HeroUnit )
			{
				HeroUnit h = u as HeroUnit;
				h.Cooldown ( );
			}

			// Update HUD
			UI.matchInfoMenu.GetPlayerHUD ( u ).UpdateStatusEffects ( u.instanceID, u.status );
		}
	}

	/// <summary>
	/// Updates all of the player's tile objects' durations.
	/// </summary>
	private void UpdateTileObjectDurations ( )
	{
		// Access each of the player's tile objects
		foreach ( TileObject o in currentPlayer.tileObjects )
			o.Duration ( );
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
	/// Plays any and all animations for a player's turn.
	/// The Animation Queue is used at the start of a player's turn for any ability duration related animations and again at the end of a player's turn for any unit action related animations.
	/// </summary>
	private Sequence PlayAnimationQueue ( )
	{
		// Create the animation
		Sequence s = DOTween.Sequence ( );

		// Add animations
		for ( int i = 0; i < animationQueue.Count; i++ )
		{
			// Check animation join type and add animation to the queue
			if ( animationQueue [ i ].isAppend )
				s.Append ( animationQueue [ i ].tween );
			else
				s.Join ( animationQueue [ i ].tween );

			// Check for next animation and add a buffer between
			if ( i + 1 < animationQueue.Count && animationQueue [ i + 1 ].isAppend )
				s.AppendInterval ( ANIMATION_BUFFER );
		}

		// Add delay at the end of the animation
		s.AppendInterval ( ANIMATION_BUFFER );

		// Play animation queue
		s.Play ( );

		// Return the sequence so that the code can wait for its completion
		return s;
	}

	/// <summary>
	/// Plays any and all of the animations that need to be played after the Animation Queue.
	/// The Post Animation Queue is not created or evaluated until after the completion of the Animation Queue.
	/// </summary>
	private Sequence PlayPostAnimationQueue ( )
	{
		// Create the animation
		Sequence s = DOTween.Sequence ( );

		// Add delay at the start of the animation
		s.AppendInterval ( ANIMATION_BUFFER );

		// Add additional "delay" for the tweens to join at
		s.AppendInterval ( 0f );

		// Add animations
		for ( int i = 0; i < postAnimationQueue.Count; i++ )
		{
			// Check if animation is still valid after the animation queue
			if ( postAnimationQueue [ i ].unit != null && postAnimationQueue [ i ].owner.units.Contains ( postAnimationQueue [ i ].unit ) && postAnimationQueue.Find ( match => match.unit == postAnimationQueue [ i ].unit ).Equals ( postAnimationQueue [ i ] ) )
			{
				// Add each in animation in post-animation series
				foreach ( TurnAnimation a in postAnimationQueue [ i ].animation )
				{
					// Check animation join type and add animation to the queue
					if ( a.isAppend )
						s.Append ( a.tween );
					else
						s.Join ( a.tween );
				}

				// Check for next animation and add a buffer between
				if ( i + 1 < postAnimationQueue.Count && postAnimationQueue [ i + 1 ].animation [ 0 ].isAppend )
					s.AppendInterval ( ANIMATION_BUFFER );
			}
		}

		// Add delay at the end of the animation
		s.AppendInterval ( ANIMATION_BUFFER );

		// Play animation queue
		s.Play ( );

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
		foreach ( MoveData m in selectedUnit.moveList.FindAll ( x => x.Prerequisite == prerequisite ) )
		{
			// Check if conflicted
			if ( m.isConflicted )
			{
				// Set tile state and highlight tile
				m.Tile.SetTileState ( TileState.ConflictedTile );
			}
			else
			{
				// Check move type
				switch ( m.Type )
				{
				// Basic moves
				case MoveData.MoveType.MOVE:
				case MoveData.MoveType.MOVE_TO_WIN:
					// Set tile state and highlight tile
					m.Tile.SetTileState ( TileState.AvailableMove );
					break;

				// Basic jump
				case MoveData.MoveType.JUMP:
				case MoveData.MoveType.JUMP_TO_WIN:
					// Set tile state and highlight tile
					m.Tile.SetTileState ( TileState.AvailableMove );
					break;

				// Jump and capture enemy
				case MoveData.MoveType.ATTACK:
				case MoveData.MoveType.ATTACK_TO_WIN:
					// Set tile state and highlight tile
					m.Tile.SetTileState ( TileState.AvailableMoveAttack );

					// Set tile state and highlight tile for the unit available for attack
					foreach ( Tile t in m.Attacks )
						t.SetTileState ( TileState.AvailableAttack );
					break;

				// Special ability move
				case MoveData.MoveType.SPECIAL:
					// Set tile state and highlight tile
					m.Tile.SetTileState ( TileState.AvailableSpecial );
					break;

				// Special ability move and attack enemy
				case MoveData.MoveType.SPECIAL_ATTACK:
					// Set tile state and highlight tile
					m.Tile.SetTileState ( TileState.AvailableSpecialAttack );

					// Set tile state and highlight tile for the unit available for attack
					foreach ( Tile t in m.Attacks )
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
		// Display mid-turn controls
		UI.ToggleMidTurnControls ( true, false );

		// Prevent any command usage
		UI.unitHUD.DisableCommandButtons ( );

		// Clear previous moves and units
		board.ResetTiles ( selectedUnit.currentTile );

		// Get move data
		MoveData data;
		if ( isConflict )
		{
			if ( isLeftClick )
				data = selectedUnit.moveList.Find ( x => x.Tile == t && x.Prerequisite == selectedMove && ( x.Type != MoveData.MoveType.SPECIAL && x.Type != MoveData.MoveType.SPECIAL_ATTACK ) );
			else
				data = selectedUnit.moveList.Find ( x => x.Tile == t && x.Prerequisite == selectedMove && ( x.Type == MoveData.MoveType.SPECIAL || x.Type == MoveData.MoveType.SPECIAL_ATTACK ) );
		}
		else
		{
			data = selectedUnit.moveList.Find ( x => x.Tile == t && x.Prerequisite == selectedMove );
		}

		// Store selected move
		selectedMove = data;

		// Display any additional moves
		DisplayAvailableMoves ( selectedMove );
	}

	/// <summary>
	/// Executes all of the current moves selected.
	/// </summary>
	public void ExecuteMove ( bool isPanicMove )
	{
		// Check for the start of turn
		if ( isStartOfTurn )
		{
			// Add selected unit to unit queue
			unitQueue.Add ( selectedUnit );

			// Mark that it is no longer the beginning phase of a turn
			isStartOfTurn = false;
		}

		// End the unit's turn
		EndTurn ( isPanicMove );
	}

	/// <summary>
	/// Removes all current moves selected.
	/// Use this as a button click event wrapper.
	/// </summary>
	public void CancelMove ( )
	{
		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false, !isStartOfTurn );

		// Remove selected move
		selectedMove = null;

		// Reset board
		SelectUnit ( selectedUnit );
	}

	/// <summary>
	/// Forfeits the unit's movement and moves to the next unit in the unit queue or ends the player's turn.
	/// </summary>
	public void SkipUnit ( bool absoluteEnd )
	{
		// Clear previous animation queues
		animationQueue.Clear ( );
		postAnimationQueue.Clear ( );

		// Hide unit HUD
		UI.unitHUD.HideHUD ( );

		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false, false );

		// Reset tiles from previous turn
		board.ResetTiles ( );

		// Remove selected unit from the unit queue
		unitQueue.Remove ( selectedUnit );

		// Check unit queue
		if ( unitQueue.Count > 0 && !absoluteEnd )
		{
			// Continue the player's turn
			ContinueTurn ( );
		}
		else
		{
			// Clear unit queue
			if ( absoluteEnd )
				unitQueue.Clear ( );

			// Start the next player's turn
			currentPlayer = GetNextPlayer ( );
			StartTurn ( );
		}
	}

	/// <summary>
	/// Continues a player's turn when units still remain in the unit queue.
	/// </summary>
	public void ContinueTurn ( )
	{
		// Select the next unit in the unit queue
		selectedUnit = unitQueue [ 0 ];

		// Clear selected move
		selectedMove = null;

		// Find unit moves
		selectedUnit.FindMoves ( selectedUnit.currentTile, null, false );
		selectedUnit.MoveConflictCheck ( );

		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false, true );

		// Resume turn timer
		if ( MatchSettings.turnTimer )
			UI.timer.ResumeTimer ( );

		// Continue the selected unit's turn
		SelectUnit ( selectedUnit );
	}

	#endregion

	#region End of Turn Setup

	/// <summary>
	/// Plays the animations at the end of a unit's turn, and then either ends the player's turn or begins the next unit's turn.
	/// Absolute End will end the player's turn regardless of remaining unit turns.
	/// </summary>
	private void EndTurn ( bool absoluteEnd )
	{
		// Clear previous animation queues
		animationQueue.Clear ( );
		postAnimationQueue.Clear ( );

		// Create moves list
		GetMoves ( selectedMove );

		// Clear board
		board.ResetTiles ( );

		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false, false );

		// Pause turn timer
		if ( MatchSettings.turnTimer )
			UI.timer.PauseTimer ( );

		// Play animations
		StartCoroutine ( EndTurnCoroutine ( absoluteEnd ) );
	}

	/// <summary>
	/// Ends the current player's turn and begins the next player's turn with an animation.
	/// </summary>
	private IEnumerator EndTurnCoroutine ( bool absoluteEnd )
	{
		// Play move animations
		yield return PlayAnimationQueue ( ).WaitForCompletion ( );

		// Play post-turn animations
		yield return PlayPostAnimationQueue ( ).WaitForCompletion ( );

		// Clear previous animation queues
		animationQueue.Clear ( );
		postAnimationQueue.Clear ( );

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
			// Remove selected unit from the unit queue
			unitQueue.Remove ( selectedUnit );

			// Check unit queue
			if ( unitQueue.Count > 0 && !absoluteEnd )
			{
				// Continue the player's turn
				ContinueTurn ( );
			}
			else
			{
				// Clear unit queue
				if ( absoluteEnd )
					unitQueue.Clear ( );

				// Start the next player's turn
				currentPlayer = GetNextPlayer ( );
				StartTurn ( );
			}
		}
	}

	/// <summary>
	/// Creates the list of animations to play during a unit's move.
	/// </summary>
	private void GetMoves ( MoveData move )
	{
		// Check for previous moves
		if ( move.Prerequisite != null )
			GetMoves ( move.Prerequisite );

		// Get the move
		selectedUnit.MoveUnit ( move );
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
		EndTurn ( true );
	}

	/// <summary>
	/// Eliminates a player from the match.
	/// </summary>
	public void LoseMatch ( Player p )
	{
		// Display the player being eliminated
		UI.matchInfoMenu.GetPlayerHUD ( p ).DisplayElimination ( );

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
		// Check each player to see if their Leader has reached the goal
		foreach ( Player p in players )
		{
			Unit l = p.units.Find ( x => x is Leader );
			if ( l != null && p.startArea.IsGoalTile ( l.currentTile ) )
				return true;
		}

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
