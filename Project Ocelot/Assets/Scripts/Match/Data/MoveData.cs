using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveData
{
	public enum MoveType
	{
		MOVE,
		MOVE_TO_WIN,
		JUMP,
		JUMP_TO_WIN,
		ATTACK,
		ATTACK_TO_WIN,
		SPECIAL,
		SPECIAL_ATTACK
	}

	public enum MoveDirection
	{
		ABOVE = 0,
		RIGHT_ABOVE = 1,
		RIGHT_BELOW = 2,
		BELOW = 3,
		LEFT_BELOW = 4,
		LEFT_ABOVE = 5
	}

	// Tracks the tile a unit could potentially move to
	public Tile Tile
	{
		get;
		private set;
	}

	// Tracks the move required to make this move
	public MoveData Prerequisite
	{
		get;
		private set;
	}

	// Tracks what type of move is available
	public MoveType Type
	{
		get;
		private set;
	}

	// Tracks the direction the unit has to move
	// This is used for tracking tiles being jumped
	public MoveDirection Direction
	{
		get;
		private set;
	}

	// Tracks the tiles of units that could be attacked from this move.
	public Tile [ ] Attacks
	{
		get;
		private set;
	}

	public bool isConflicted;

	// Tracks the value of the move
	// This is used only for the AI's move determination
	public int Value
	{
		get;
		private set;
	}

	public MoveData ( Tile _tile, MoveData _prereq, MoveType _type, MoveDirection _dir, params Tile [ ] _capture )
	{
		Tile = _tile;
		Prerequisite = _prereq;
		Type = _type;
		Direction = _dir;
		Attacks = _capture;
		isConflicted = false;
		Value = 0;
	}

	public MoveData ( Tile _tile, MoveData _prereq, MoveType _type, int _dir, params Tile [ ] _capture )
	{
		Tile = _tile;
		Prerequisite = _prereq;
		Type = _type;
		Direction = ( MoveDirection )_dir;
		Attacks = _capture;
		isConflicted = false;
		Value = 0;
	}

	/// <summary>
	/// Determines the value of a move for the AI.
	/// </summary>
	public void DetermineValue ( )
	{
		Value = 0;
	}
}
