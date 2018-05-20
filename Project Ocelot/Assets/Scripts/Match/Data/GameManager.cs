using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{
	#region Private Classes

	[System.Serializable]
	private class UnitPrefab
	{
		public int ID;
		public Unit Unit;
	}

	#endregion // Private Classes

	#region UI Elements

	public UIManager UI;

	#endregion // UI Elements

	#region GameObjects

	public Board Board;

	#endregion // GameObjects

	#region Match Data

	[SerializeField]
	private Player [ ] players;

	[SerializeField]
	private UnitPrefab [ ] prefabs;

	private int playerIndex = 0;
	private Dictionary<int, Unit> unitPrefabDictionary = new Dictionary<int, Unit> ( );

	/// <summary>
	/// The player whose current turn it is.
	/// </summary>
	public Player CurrentPlayer
	{
		get
		{
			// Get the current player
			return players [ playerIndex ];
		}
	}


	#endregion // Match Data

	#region Turn Data

	/// <summary>
	/// A single animation to be played during a turn.
	/// </summary>
	public struct TurnAnimation
	{
		public Tween Animation;
		public bool IsAppend;

		public TurnAnimation ( Tween animation, bool isAppend )
		{
			Animation = animation;
			IsAppend = isAppend;
		}
	}

	/// <summary>
	/// A single or multiple animations to be played after a turn.
	/// </summary>
	public struct PostTurnAnimation
	{
		public Unit TargetUnit;
		public Player Owner;
		public TurnAnimation [ ] Animation;

		public PostTurnAnimation ( Unit unit, Player owner, params TurnAnimation [ ] animation )
		{
			TargetUnit = unit;
			Owner = owner;
			Animation = animation;
		}
	}

	[HideInInspector]
	public List<Unit> UnitQueue = new List<Unit> ( );

	[HideInInspector]
	public List<TurnAnimation> AnimationQueue = new List<TurnAnimation> ( );

	[HideInInspector]
	public List<PostTurnAnimation> PostAnimationQueue = new List<PostTurnAnimation> ( );

	private const float ANIMATION_BUFFER = 0.1f;

	/// <summary>
	/// Whether or not it is still the starting phase of a turn.
	/// </summary>
	public bool IsStartOfTurn
	{
		get;
		private set;
	}

	/// <summary>
	/// The currently selected unit for the current player.
	/// </summary>
	public Unit SelectedUnit
	{
		get;
		private set;
	}

	/// <summary>
	/// The currently selected set of moves for the current player.
	/// </summary>
	public MoveData SelectedMove
	{
		get;
		private set;
	}

	#endregion // Turn Data

	#region MonoBehaviour Functions

	private void Start ( )
	{
		// Set prefab dictionary
		for ( int i = 0; i < prefabs.Length; i++ )
			unitPrefabDictionary.Add ( prefabs [ i ].ID, prefabs [ i ].Unit );

		// Start the match
		StartMatch ( );
	}

	#endregion // MonoBehaviour Functions

	#region Start of Match - Private Functions

	/// <summary>
	/// Starts the match.
	/// </summary>
	private void StartMatch ( )
	{
		// Set players
		for ( int i = 0; i < players.Length; i++ )
		{
			// Set team name
			players [ i ].PlayerName = MatchSettings.Players [ i ].PlayerName;

			// Set team color
			players [ i ].Team = MatchSettings.Players [ i ].Team;

			// Set direction
			players [ i ].TeamDirection = MatchSettings.Players [ i ].TeamDirection;

			// Set goal area
			players [ i ].startArea.SetColor ( players [ i ].Team );

			// Set unit data
			players [ i ].Units = MatchSettings.Players [ i ].Units;

			// Set special IDs
			//players [ i ].specialIDs = MatchSettings.playerSettings [ i ].heroIDs.ToArray ( );

			// Spawn units
			for ( int j = 0; j < players [ i ].Units.Count; j++ )
			{
				// Create unit instance
				Unit u = Instantiate ( unitPrefabDictionary [ players [ i ].Units [ j ].ID ], players [ i ].transform );

				// Set the instance data
				u.InitializeInstance ( this, ( i * 10 ) + j, players [ i ].Units [ j ] );

				// Set unit's owner
				u.owner = players [ i ];

				// Set unit team color
				u.SetTeamColor ( players [ i ].Team );

				// Set unit direction
				Util.OrientSpriteToDirection ( u.sprite, players [ i ].TeamDirection );

				// Position unit to starting tile
				u.transform.position = players [ i ].startArea.tiles [ MatchSettings.Players [ i ].UnitFormation [ players [ i ].Units [ j ] ] ].transform.position;
				u.currentTile = players [ i ].startArea.tiles [ MatchSettings.Players [ i ].UnitFormation [ players [ i ].Units [ j ] ] ];
				players [ i ].startArea.tiles [ MatchSettings.Players [ i ].UnitFormation [ players [ i ].Units [ j ] ] ].currentUnit = u;

				// Add unit to team
				players [ i ].UnitInstances.Add ( u );
			}

			// Add standard KO delegate to each unit
			if ( players [ i ].standardKOdelegate != null )
				foreach ( Unit u in players [ i ].UnitInstances )
					u.koDelegate += players [ i ].standardKOdelegate;
		}

		// Sort players by turn order
		players = players.OrderBy ( x => x.TurnOrder ).ToArray ( );

		// Set up UI
		UI.Initialize ( players );

		// Begin match
		StartTurn ( );
	}

	#endregion // Start of Match - Private Functions

	#region Start of Turn - Public Functions

	/// <summary>
	/// Gets all of the available moves for each of the current player's units before any moves have been made.
	/// </summary>
	public void GetTeamMoves ( )
	{
		// Access each of the player's units
		foreach ( Unit u in CurrentPlayer.UnitInstances )
		{
			// Set move list
			u.FindMoves ( u.currentTile, null, false );
			u.MoveConflictCheck ( );
		}
	}

	/// <summary>
	/// Displays the available units for the current player.
	/// </summary>
	public void DisplayAvailableUnits ( )
	{
		// Highlight each unit
		foreach ( Unit u in CurrentPlayer.UnitInstances )
		{
			u.currentTile.SetTileState ( TileState.AvailableUnit );
		}
	}

	#endregion // Start of Turn - Public Functions

	#region Start of Turn - Private Functions

	/// <summary>
	/// Starts a player's turn.
	/// </summary>
	private void StartTurn ( )
	{
		// Clear previous animation queues
		AnimationQueue.Clear ( );
		PostAnimationQueue.Clear ( );
		
		// Begin turn
		IsStartOfTurn = true;

		// Start new turn animation
		StartCoroutine ( StartTurnCoroutine ( ) );
	}

	/// <summary>
	/// Starts a player's turn with an animation.
	/// </summary>
	private IEnumerator StartTurnCoroutine ( )
	{
		// Wait until animation is completed
		yield return UI.splash.Slide ( CurrentPlayer.PlayerName + "'s Turn", Util.TeamColor ( CurrentPlayer.Team ), true ).WaitForCompletion ( );

		// Set cooldowns
		UpdateUnitCountdowns ( );

		// Set durations
		UpdateTileObjectDurations ( );

		// Wait until the start of turn animations are complete
		yield return PlayAnimationQueue ( ).WaitForCompletion ( );
		
		// Wait until the post-start of turn animations are complete
		yield return PlayPostAnimationQueue ( ).WaitForCompletion ( );

		// Clear previous animation queues
		AnimationQueue.Clear ( );
		PostAnimationQueue.Clear ( );

		// Check for winner
		if ( WinnerCheck ( ) )
		{
			// Display the winner
			UI.WinPrompt ( CurrentPlayer );
		}

		// Get moves
		GetTeamMoves ( );

		// Display units for selection
		DisplayAvailableUnits ( );
		BringPlayerToTheFront ( CurrentPlayer );
		SelectedUnit = null;
		SelectedMove = null;

		// Start turn timer
		if ( MatchSettings.TurnTimer )
			UI.timer.StartTimer ( );
	}

	/// <summary>
	/// Updates all of the player's unit's status effects, ability cooldowns, and ability durations.
	/// </summary>
	private void UpdateUnitCountdowns ( )
	{
		// Access each of the player's units
		foreach ( Unit u in CurrentPlayer.UnitInstances )
		{
			// Set status effects
			u.Status.UpdateDurations ( );

			// Set cooldowns
			if ( u is HeroUnit )
			{
				HeroUnit h = u as HeroUnit;
				h.Cooldown ( );
			}

			// Update HUD
			UI.matchInfoMenu.GetPlayerHUD ( u ).UpdateStatusEffects ( u.InstanceID, u.Status );
		}
	}

	/// <summary>
	/// Updates all of the player's tile objects' durations.
	/// </summary>
	private void UpdateTileObjectDurations ( )
	{
		// Access each of the player's tile objects
		foreach ( TileObject o in CurrentPlayer.tileObjects )
			o.Duration ( );
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
		for ( int i = 0; i < AnimationQueue.Count; i++ )
		{
			// Check animation join type and add animation to the queue
			if ( AnimationQueue [ i ].IsAppend )
				s.Append ( AnimationQueue [ i ].Animation );
			else
				s.Join ( AnimationQueue [ i ].Animation );

			// Check for next animation and add a buffer between
			if ( i + 1 < AnimationQueue.Count && AnimationQueue [ i + 1 ].IsAppend )
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
		for ( int i = 0; i < PostAnimationQueue.Count; i++ )
		{
			// Check if animation is still valid after the animation queue
			if ( PostAnimationQueue [ i ].TargetUnit != null && PostAnimationQueue [ i ].Owner.UnitInstances.Contains ( PostAnimationQueue [ i ].TargetUnit ) && PostAnimationQueue.Find ( match => match.TargetUnit == PostAnimationQueue [ i ].TargetUnit ).Equals ( PostAnimationQueue [ i ] ) )
			{
				// Add each in animation in post-animation series
				foreach ( TurnAnimation a in PostAnimationQueue [ i ].Animation )
				{
					// Check animation join type and add animation to the queue
					if ( a.IsAppend )
						s.Append ( a.Animation );
					else
						s.Join ( a.Animation );
				}

				// Check for next animation and add a buffer between
				if ( i + 1 < PostAnimationQueue.Count && PostAnimationQueue [ i + 1 ].Animation [ 0 ].IsAppend )
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

	#endregion Start of Turn - Private Functions

	#region Mid-Turn - Public Functions

	/// <summary>
	/// Selects the current unit and displays any and all available moves for the unit.
	/// </summary>
	/// <param name="u"> The unit being selected. </param>
	public void SelectUnit ( Unit u )
	{
		// Check for previously selected unit
		if ( SelectedUnit != null && IsStartOfTurn && !UI.timer.isOutOfTime )
		{
			// Reset any previous selected units
			Board.ResetTiles ( );
			DisplayAvailableUnits ( );
		}

		// Set unit as the currently selected unit
		SelectedUnit = u;

		// Set that there are no currently selected moves
		SelectedMove = null;

		// Display unit HUD
		UI.unitHUD.DisplayUnit ( SelectedUnit );

		// Highlight the tile of the selected unit
		SelectedUnit.currentTile.SetTileState ( TileState.SelectedUnit );
		BringUnitToTheFront ( SelectedUnit );

		// Display the unit's available moves
		DisplayAvailableMoves ( null );
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
		Board.ResetTiles ( SelectedUnit.currentTile );

		// Get move data
		MoveData data;
		if ( isConflict )
		{
			if ( isLeftClick )
				data = SelectedUnit.MoveList.Find ( x => x.Tile == t && x.Prerequisite == SelectedMove && ( x.Type != MoveData.MoveType.SPECIAL && x.Type != MoveData.MoveType.SPECIAL_ATTACK ) );
			else
				data = SelectedUnit.MoveList.Find ( x => x.Tile == t && x.Prerequisite == SelectedMove && ( x.Type == MoveData.MoveType.SPECIAL || x.Type == MoveData.MoveType.SPECIAL_ATTACK ) );
		}
		else
		{
			data = SelectedUnit.MoveList.Find ( x => x.Tile == t && x.Prerequisite == SelectedMove );
		}

		// Store selected move
		SelectedMove = data;

		// Display any additional moves
		DisplayAvailableMoves ( SelectedMove );
	}

	/// <summary>
	/// Executes all of the current moves selected.
	/// </summary>
	public void ExecuteMove ( bool isPanicMove )
	{
		// Check for the start of turn
		if ( IsStartOfTurn )
		{
			// Add selected unit to unit queue
			UnitQueue.Add ( SelectedUnit );

			// Mark that it is no longer the beginning phase of a turn
			IsStartOfTurn = false;
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
		UI.ToggleMidTurnControls ( false, !IsStartOfTurn );

		// Remove selected move
		SelectedMove = null;

		// Reset board
		SelectUnit ( SelectedUnit );
	}

	/// <summary>
	/// Forfeits the unit's movement and moves to the next unit in the unit queue or ends the player's turn.
	/// </summary>
	public void SkipUnit ( bool absoluteEnd )
	{
		// Clear previous animation queues
		AnimationQueue.Clear ( );
		PostAnimationQueue.Clear ( );

		// Hide unit HUD
		UI.unitHUD.HideHUD ( );

		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false, false );

		// Reset tiles from previous turn
		Board.ResetTiles ( );

		// Remove selected unit from the unit queue
		UnitQueue.Remove ( SelectedUnit );

		// Check unit queue
		if ( UnitQueue.Count > 0 && !absoluteEnd )
		{
			// Continue the player's turn
			ContinueTurn ( );
		}
		else
		{
			// Clear unit queue
			if ( absoluteEnd )
				UnitQueue.Clear ( );

			// Start the next player's turn
			GetNextPlayer ( );
			StartTurn ( );
		}
	}

	/// <summary>
	/// Continues a player's turn when units still remain in the unit queue.
	/// </summary>
	public void ContinueTurn ( )
	{
		// Select the next unit in the unit queue
		SelectedUnit = UnitQueue [ 0 ];

		// Clear selected move
		SelectedMove = null;

		// Find unit moves
		SelectedUnit.FindMoves ( SelectedUnit.currentTile, null, false );
		SelectedUnit.MoveConflictCheck ( );

		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false, true );

		// Resume turn timer
		if ( MatchSettings.TurnTimer )
			UI.timer.ResumeTimer ( );

		// Continue the selected unit's turn
		SelectUnit ( SelectedUnit );
	}

	#endregion // Mid-Turn - Public Functions

	#region Mid-Turn - Private Functions

	/// <summary>
	/// Displays the available moves for the selected unit and any selected prerequisite moves.
	/// </summary>
	private void DisplayAvailableMoves ( MoveData prerequisite )
	{
		// Get only the moves for the prerequisite
		foreach ( MoveData m in SelectedUnit.MoveList.FindAll ( x => x.Prerequisite == prerequisite ) )
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

	#endregion // Mid-Turn - Private Functions

	#region End of Turn - Private Functions

	/// <summary>
	/// Plays the animations at the end of a unit's turn, and then either ends the player's turn or begins the next unit's turn.
	/// Absolute End will end the player's turn regardless of remaining unit turns.
	/// </summary>
	private void EndTurn ( bool absoluteEnd )
	{
		// Clear previous animation queues
		AnimationQueue.Clear ( );
		PostAnimationQueue.Clear ( );

		// Create moves list
		GetMoves ( SelectedMove );

		// Clear board
		Board.ResetTiles ( );

		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false, false );

		// Pause turn timer
		if ( MatchSettings.TurnTimer )
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
		AnimationQueue.Clear ( );
		PostAnimationQueue.Clear ( );

		// Hide unit HUD
		UI.unitHUD.HideHUD ( );

		// Reset tiles from previous turn
		Board.ResetTiles ( );

		// Check for winner
		if ( WinnerCheck ( ) )
		{
			// Display the winner
			UI.WinPrompt ( CurrentPlayer );
		}
		else
		{
			// Remove selected unit from the unit queue
			UnitQueue.Remove ( SelectedUnit );

			// Check unit queue
			if ( UnitQueue.Count > 0 && !absoluteEnd )
			{
				// Continue the player's turn
				ContinueTurn ( );
			}
			else
			{
				// Clear unit queue
				if ( absoluteEnd )
					UnitQueue.Clear ( );

				// Start the next player's turn
				GetNextPlayer ( );
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
		SelectedUnit.MoveUnit ( move );
	}

	/// <summary>
	/// Gets the next player the turn order.
	/// </summary>
	private void GetNextPlayer ( )
	{
		// Move to the next player in the turn order
		for ( int i = playerIndex + 1; i < players.Length; i++ )
		{
			// Check if the player is still in the game
			if ( !players [ i ].IsEliminated )
			{
				// Set current player
				playerIndex = i;
				return;
			}
		}

		// Continue search for the next player
		for ( int i = 0; i < playerIndex; i++ )
		{
			// Check if the player is still in the game
			if ( !players [ i ].IsEliminated )
			{
				// Set current player
				playerIndex = i;
				return;
			}
		}
	}

	#endregion // End of Turn - Private Functions

	#region End of Match - Public Functions

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
	/// Eliminates a player from the match.
	/// </summary>
	public void LoseMatch ( Player p )
	{
		// Display the player being eliminated
		UI.matchInfoMenu.GetPlayerHUD ( p ).DisplayElimination ( );

		// Remove player from match
		foreach ( Unit u in p.UnitInstances )
		{
			// Capture all other units
			u.GetAttacked ( true );
		}
	}

	#endregion // End of Match - Public Functions

	#region End of Match - Private Functions

	/// <summary>
	/// Checks to make sure that the player has at least one available move.
	/// Returns true if the player has no available moves and is forced to forfeit.
	/// </summary>
	private bool ForfeitCheck ( )
	{
		// Check each unit's move list until at least one move is found
		foreach ( Unit u in CurrentPlayer.UnitInstances )
			if ( u.MoveList.Count > 0 )
				return false;

		// Return that no moves were found
		return true;
	}

	private void ForfeitMatch ( )
	{
		LoseMatch ( CurrentPlayer );
		EndTurn ( true );
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
			Unit l = p.UnitInstances.Find ( x => x is Leader );
			if ( l != null && p.startArea.IsGoalTile ( l.currentTile ) )
				return true;
		}

		// Tracking info
		int playerCount = 0;

		// Check each player to see if they have units remaining
		foreach ( Player p in players )
			if ( p.UnitInstances.Count > 0 )
				playerCount++;

		// Return if there is a winner
		return playerCount == 1;
	}

	#endregion // End of Match - Private Functions

	#region Private Functions

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
				for ( int j = 0; j < players [ i ].UnitInstances.Count; j++ )
					players [ i ].UnitInstances [ j ].sprite.sortingOrder = 1;
			}
			else
			{
				// Bring each of the player's units to the default layer
				for ( int j = 0; j < players [ i ].UnitInstances.Count; j++ )
					players [ i ].UnitInstances [ j ].sprite.sortingOrder = 0;
			}
		}
	}

	/// <summary>
	/// Brings one of the current player's unit's sprite to the front layer.
	/// </summary>
	private void BringUnitToTheFront ( Unit u )
	{
		// Set each of the current player's units to the base layer
		for ( int i = 0; i < CurrentPlayer.UnitInstances.Count; i++ )
			CurrentPlayer.UnitInstances [ i ].sprite.sortingOrder = 1;

		// Bring the specified unit's sprite to the front layer
		u.sprite.sortingOrder = 2;
	}

	#endregion // Private Functions
}
