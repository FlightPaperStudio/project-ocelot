﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
	#region Tile Object Data

	public SpriteRenderer Icon;
	public HeroUnit Caster;
	public Hex CurrentHex;

	public bool CanBeOccupied;
	public bool CanAssist;
	public bool CanBeMoved;
	public bool CanBeAttacked;

	private int duration;

	public delegate void TileObjectDelegate ( );
	private TileObjectDelegate durationDelegate;

	#endregion // Tile Object Data

	#region Public Functions

	/// <summary>
	/// Sets the tile object instance information.
	/// </summary>
	public void SetTileObject ( HeroUnit caster, Hex hex, int _duration, TileObjectDelegate _delegate )
	{
		// Set owner
		Caster = caster;

		// Set tile
		CurrentHex = hex;

		// Set position
		transform.position = hex.transform.position;

		// Set duration
		duration = _duration;

		// Set delegate
		durationDelegate = _delegate;
	}

	/// <summary>
	/// Decrements the duration each turn.
	/// </summary>
	public void Duration ( )
	{
		// Decrement duration
		duration--;

		// Check if duration has expired
		if ( duration == 0 )
			OnDurationExpire ( );
	}

	/// <summary>
	/// Assists a unit.
	/// </summary>
	public void Assist ( )
	{
		// Add assist for the caster
		Caster.Assist ( );
	}

	/// <summary>
	/// Responds to getting attacked by an opponent.
	/// </summary>
	public void GetAttacked ( )
	{
		// End duration
		OnDurationExpire ( );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Executes the delegate on the duration of this tile object expiring.
	/// </summary>
	private void OnDurationExpire ( )
	{
		// Execute delegate
		durationDelegate ( );
	}

	#endregion // Private Functions
}
