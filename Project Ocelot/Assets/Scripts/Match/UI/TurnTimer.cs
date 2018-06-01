using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TurnTimer : MonoBehaviour 
{
	#region UI Elements

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private TextMeshProUGUI display;

	#endregion // UI Elements

	#region Timer Data

	[SerializeField]
	private GameManager GM;

	[SerializeField]
	private AudioSource audio;

	[SerializeField]
	private AudioClip outOfTimeSFX;

	private float currentTime = 0f;
	private bool isTimerActive = false;
	private bool isTimerWarning = false;

	private const float TIMER_WARNING = 15f;

	/// <summary>
	/// Whether or not the turn timer has run out of time.
	/// </summary>
	public bool IsOutOfTime
	{
		get;
		private set;
	}

	#endregion // Timer Data

	#region MonoBehaviour Functions

	private void Start ( )
	{
		// Hide/Display timer
		container.SetActive ( MatchSettings.TurnTimer );

		// Set starting values
		isTimerActive = false;
		currentTime = 0f;
		IsOutOfTime = false;

		// Add sfx player to sfx manager
		SFXManager.Instance.AddScenePlayer ( audio );
	}

	private void Update ( )
	{
		// Check if the timer is active
		if ( MatchSettings.TurnTimer && isTimerActive )
		{
			// Check time remaining
			if ( currentTime > 0 )
			{
				// Change color of the timer
				display.color = isTimerWarning ? new Color32 ( 200, 50, 50, 255 ) : (Color32)Color.white;

				// Decrease time
				currentTime -= Time.deltaTime;

				// Display time
				int roundedTime = Mathf.CeilToInt ( currentTime );
				int min = roundedTime / 60;
				int sec = roundedTime % 60;
				display.text = string.Format ( "{0:0}:{1:00}", min, sec );

				// Check for warning
				if ( !isTimerWarning && currentTime <= TIMER_WARNING )
				{
					// Mark that the timer is now on warning
					isTimerWarning = true;

					// Play sfx
					audio.Play ( );
				}
			}
			else
			{
				// Stop the timer
				StopTimer ( );
			}
		}
	}

	#endregion // MonoBehaviour Functions

	#region Public Functions

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
		isTimerWarning = false;
		IsOutOfTime = false;
	}

	/// <summary>
	/// Pauses the timer.
	/// Use this during animations.
	/// </summary>
	public void PauseTimer ( )
	{
		// Pause the timer
		isTimerActive = false;

		// Pause the timer warning
		if ( isTimerWarning )
			audio.Pause ( );
	}

	/// <summary>
	/// Resumes the timer.
	/// Use this when an animation has completed.
	/// </summary>
	public void ResumeTimer ( )
	{
		// Unpause the timer
		isTimerActive = true;

		// Resume the timer warning if it was paused
		if ( isTimerWarning )
			audio.UnPause ( );
	}

	/// <summary>
	/// Stops the timer when there is no time remaining.
	/// This will either end the player's turn or force a random unit to move since inaction is not allowed for a turn.
	/// </summary>
	public void StopTimer ( )
	{
		// Pause the timer
		isTimerActive = false;
		isTimerWarning = false;
		IsOutOfTime = true;

		// Play sfx
		audio.Stop ( );
		audio.PlayOneShot ( outOfTimeSFX );

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

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Ends the current player's turn early by forcing a random unit to make a random move.
	/// </summary>
	/// <param name="u"> The random unit selected for a panic move. </param>
	/// <param name="t"> The tile the unit will move to. </param>
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

	#endregion // Private Functions
}
