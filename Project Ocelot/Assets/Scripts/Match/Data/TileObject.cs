using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
	// Object information
	public SpriteRenderer sprite;
	public HeroUnit hero;
	public Tile tile;
	public bool canBeOccupied;
	public bool canBeJumped;

	// Instance information
	private int duration;
	public delegate void TileObjectDelegate ( );
	private TileObjectDelegate durationDelegate;

	/// <summary>
	/// Sets the tile object instance information.
	/// </summary>
	public void SetTileObject ( HeroUnit _hero, Tile _tile, int _duration, TileObjectDelegate _delegate )
	{
		// Set owner
		hero = _hero;

		// Set tile
		tile = _tile;

		// Set position
		transform.position = _tile.transform.position;

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
	/// Executes the delegate on the duration of this tile object expiring.
	/// </summary>
	private void OnDurationExpire ( )
	{
		// Execute delegate
		durationDelegate ( );
	}
}
