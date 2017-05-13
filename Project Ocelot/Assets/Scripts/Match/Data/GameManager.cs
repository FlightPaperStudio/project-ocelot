using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{
	// Player information
	public Player [ ] players;
	public Player currentPlayer
	{
		get;
		private set;
	}
	private int playerIndex = 0;

	// Board information
	public Board board;

	// UI information
	public UIManager UI;

	// Unit information
	public Unit [ ] unitPrefabs;
	public Unit selectedUnit
	{
		get;
		private set;
	}
	public bool isStartOfTurn
	{
		get;
		private set;
	}
	public bool isMatchComplete
	{
		get;
		private set;
	}

	/// <summary>
	/// Sets up the proper data structures before the match begins.
	/// </summary>
	private void Start ( )
	{
		// Initialize the match settings

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
		isMatchComplete = false;
		StartTurn ( );
	}

	/// <summary>
	/// Starts a player's turn.
	/// </summary>
	private void StartTurn ( )
	{
		// Start new turn animation
		if ( !isMatchComplete )
			StartCoroutine ( StartTurnCoroutine ( ) );
	}

	/// <summary>
	/// Starts a player's turn with an animation.
	/// </summary>
	private IEnumerator StartTurnCoroutine ( )
	{
		// Wait until animation is completed
		yield return UI.splash.Slide ( currentPlayer.name + "'s Turn", Util.TeamColor ( currentPlayer.team ), true ).WaitForCompletion ( );

		// Get moves
		GetTeamMoves ( true );

		// Check to make sure the player has moves available
		if ( ForfeitCheck ( ) )
		{
			// Have the current player forfeit
			ForfeitMatch ( );
		}

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

	/// <summary>
	/// Ends the current player's turn and begins the next player's turn.
	/// </summary>
	public void EndTurn ( )
	{
		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false );
		UI.unitHUD.HideHUD ( );

		// Pause turn timer
		if ( MatchSettings.turnTimer )
			UI.timer.PauseTimer ( );

		// Reset tiles from previous turn
		board.ResetTiles ( );

		// Start the next player's turn
		currentPlayer = GetNextPlayer ( );
		StartTurn ( );
	}

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

	public void LoseMatch ( Player p )
	{
		// Display the player being eliminated
		UI.hudDic [ p ].DisplayElimination ( );

		// Remove player from match
		for ( int i = p.units.Count - 1; i >= 0; i-- )
		{
			// Ignore the leader unit
			//if ( u is Leader )
			//	continue;

			// Capture all other units
			p.units [ i ].GetCaptured ( true );
		}

		// Check if match has been won
		Player winner = WinnerCheck ( );
		if ( winner != null )
			WinMatch ( winner );
	}

	/// <summary>
	/// Checks if there are multiple players still competing in the match.
	/// Returns null if more than one player is still competing. Returns the winning player if only one player remains.
	/// </summary>
	private Player WinnerCheck ( )
	{
		// Tracking info
		int playerCount = 0;
		Player winner = null;

		// Check each player to see if they have units remaining
		foreach ( Player p in players )
		{
			if ( p.units.Count > 0 )
			{
				// Track remaining players
				winner = p;
				playerCount++;
			}
		}

		// Check if there is a winner
		if ( playerCount == 1 )
		{
			// Return the winning player
			return winner;
		}
		else
		{
			// Return that there is no winning player yet
			return null;
		}
	}

	public void WinMatch ( Player p )
	{
		isMatchComplete = true;
		if ( MatchSettings.turnTimer )
			UI.timer.PauseTimer ( );
		board.ResetTiles ( );
		UI.WinPrompt ( p );
	}

	/// <summary>
	/// Gets all of the available moves for each of the current player's units.
	/// Set the parameter to true if the units' ability cooldowns and durations should be decremented.
	/// </summary>
	public void GetTeamMoves ( bool doCooldowns )
	{
		// Access each of the player's units
		foreach ( Unit u in currentPlayer.units )
		{
			// Clear previous blocked tiles
			u.ClearBlockedTiles ( );

			// Add starting tile as a blocked tile
			u.AddBlockedTile ( u.currentTile, true );

			// Set cooldowns
			if ( doCooldowns && u is HeroUnit )
			{
				HeroUnit h = u as HeroUnit;
				h.Cooldown ( );
			}

			// Set move list
			u.FindMoves ( );
			u.SetMoveList ( );
		}
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
		
		// Display unit HUD
		UI.unitHUD.DisplayUnit ( selectedUnit );

		// Highlight the tile of the selected unit
		selectedUnit.currentTile.SetTileState ( TileState.SelectedUnit );
		BringUnitToTheFront ( selectedUnit );

		// Display each available move in the selected unit's move list
		foreach ( MoveData data in selectedUnit.moveList )
		{
			// Check if conflicted
			if ( data.isConflicted )
			{
				// Set tile state and highlight tile
				data.tile.SetTileState ( TileState.ConflictedTile );
			}
			else
			{
				// Check move type
				switch ( data.type )
				{
				// Basic moves
				case MoveData.MoveType.Move:
				case MoveData.MoveType.MoveToWin:
					// Set tile state and highlight tile
					data.tile.SetTileState ( TileState.AvailableMove );
					break;

					// Basic jump
				case MoveData.MoveType.Jump:
				case MoveData.MoveType.JumpToWin:
					// Set tile state and highlight tile
					data.tile.SetTileState ( TileState.AvailableMove );
					break;

					// Jump and capture enemy
				case MoveData.MoveType.JumpCapture:
				case MoveData.MoveType.JumpCaptureToWin:
					// Set tile state and highlight tile
					data.tile.SetTileState ( TileState.AvailableMoveCapture );

					// Set tile state and highlight tile for the unit available for capture
					foreach ( Tile t in data.capture )
						t.SetTileState ( TileState.AvailableCapture );
					break;

					// Special ability move
				case MoveData.MoveType.Special:
					// Set tile state and highlight tile
					data.tile.SetTileState ( TileState.AvailableSpecial );
					break;

					// Special ability move and capture enemy
				case MoveData.MoveType.SpecialCapture:
					// Set tile state and highlight tile
					data.tile.SetTileState ( TileState.AvailableSpecialCapture );

					// Set tile state and highlight tile for the unit available for capture
					foreach ( Tile t in data.capture )
						t.SetTileState ( TileState.AvailableCapture );
					break;
				}
			}
		}
	}

	/// <summary>
	/// Selects the move for the currently selected unit to make.
	/// </summary>
	public void SelectMove ( Tile t, bool isLeftClick = true )
	{
		// Mark that the turn has progressed
		isStartOfTurn = false;

		// Hide mid-turn controls
		UI.ToggleMidTurnControls ( false );

		// Pause timer
		if ( MatchSettings.turnTimer )
			UI.timer.PauseTimer ( );

		// Get move data
		MoveData data;
		if ( isLeftClick )
			data = selectedUnit.moveDic [ t ];
		else
			data = selectedUnit.moveList.Find ( item => item.tile == t && item != selectedUnit.moveDic [ t ] );
		
		// Reset tiles
		board.ResetTiles ( selectedUnit.currentTile, t );

		// Move the unit to this tile
		selectedUnit.MoveUnit ( data );
	}
}
