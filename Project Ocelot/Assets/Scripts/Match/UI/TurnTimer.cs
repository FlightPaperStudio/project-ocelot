﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TurnTimer : MonoBehaviour 
{
	// UI elements
	public GameObject container;
	public TextMeshProUGUI display;

	// Timer information
	public GameManager GM;
	private bool isTimerActive = false;
	private float currentTime = 0f;
	public bool isOutOfTime
	{
		get;
		private set;
	}

	/// <summary>
	/// Initializes the turn timer.
	/// </summary>
	private void Start ( )
	{
		// Hide/Display timer
		container.SetActive ( MatchSettings.TurnTimer );

		// Set starting values
		isTimerActive = false;
		currentTime = 0f;
		isOutOfTime = false;
	}

	/// <summary>
	/// Updates the timer.
	/// </summary>
	private void Update ( ) 
	{
		// Check if the timer is active
		if ( MatchSettings.TurnTimer && isTimerActive )
		{
			// Check time remaining
			if ( currentTime > 0 )
			{
				// Decrease time
				currentTime -= Time.deltaTime;

				// Display time
				int roundedTime = Mathf.CeilToInt ( currentTime );
				int min = roundedTime / 60;
				int sec = roundedTime % 60;
				display.text = string.Format ( "{0:0}:{1:00}", min, sec );
			}
			else
			{
				// Stop the timer
				StopTimer ( );
			}
		}
	}

	/// <summary>
	/// Starts the timer.
	/// Use this at the start of a new turn.
	/// </summary>
	public void StartTimer ( )
	{
		// Set timer
		currentTime = MatchSettings.TimePerTurn;

		// Begin timer
		isTimerActive = true;
		isOutOfTime = false;
	}

	/// <summary>
	/// Pauses the timer.
	/// Use this during animations.
	/// </summary>
	public void PauseTimer ( )
	{
		// Pause the timer
		isTimerActive = false;
	}

	/// <summary>
	/// Resumes the timer.
	/// Use this when an animation has completed.
	/// </summary>
	public void ResumeTimer ( )
	{
		// Unpause the timer
		isTimerActive = true;
	}

	/// <summary>
	/// Stops the timer when there is no time remaining.
	/// This will either end the player's turn or force a random unit to move since inaction is not allowed for a turn.
	/// </summary>
	public void StopTimer ( )
	{
		// Pause the timer
		isTimerActive = false;
		isOutOfTime = true;

		// Check for start of turn
		if ( GM.IsStartOfTurn )
		{
			// Create list of possible units
			List<Unit> units = GM.CurrentPlayer.UnitInstances.FindAll ( x => x.MoveList.Count > 0 );

			// Select a random unit
			int unitIndex = Random.Range ( 0, units.Count );
			Unit unit = units [ unitIndex ];

			// Create list of possible moves
			List<MoveData> moves = unit.MoveList.FindAll ( x => x.Prerequisite == null );

			// Select a random move
			int moveIndex = Random.Range ( 0, moves.Count );
			MoveData move = unit.MoveList [ moveIndex ];

			// Clear board
			GM.Board.ResetTiles ( );

			// Force a panic move and end the player's turn
			StartCoroutine ( PanicMove ( unit, move.Tile ) );
		}
		else
		{
			// Skip all remaining units
			GM.SkipUnit ( true );
		}
	}

	/// <summary>
	/// Ends the current player's turn early by forcing a random unit to make a random move.
	/// </summary>
	private IEnumerator PanicMove ( Unit u, Tile t )
	{
		// Wait for slide animation
		yield return GM.UI.splash.Slide ( "Time's Up!\n<size=75%><color=white>Panic!", new Color32 ( 200, 50, 50, 255 ), true ).WaitForCompletion ( );

		// Select the unit
		GM.SelectUnit ( u );

		// Select move
		GM.SelectMove ( t );

		// Execute move
		GM.ExecuteMove ( true );
	}
}
