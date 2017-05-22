using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveData
{
	public enum MoveType
	{
		Move,
		MoveToWin,
		Jump,
		JumpToWin,
		Attack,
		AttackToWin,
		Special,
		SpecialAttack
	}

	public enum MoveDirection
	{
		Above = 0,
		RightAbove = 1,
		RightBelow = 2,
		Below = 3,
		LeftBelow = 4,
		LeftAbove = 5
	}

	// Tracks the tile a unit could potentially move to
	public Tile tile
	{
		get;
		private set;
	}

	// Tracks the move required to make this move
	public MoveData prerequisite
	{
		get;
		private set;
	}

	// Tracks what type of move is available
	public MoveType type
	{
		get;
		private set;
	}

	// Tracks the direction the unit has to move
	// This is used for tracking tiles being jumped
	public MoveDirection direction
	{
		get;
		private set;
	}

	// Tracks the tiles of units that could be attacked from this move.
	public Tile [ ] attacks
	{
		get;
		private set;
	}

	public bool isConflicted;

	// Tracks the value of the move
	// This is used only for the AI's move determination
	public int value
	{
		get;
		private set;
	}

	public MoveData ( Tile _tile, MoveData _prereq, MoveType _type, MoveDirection _dir, params Tile [ ] _capture )
	{
		tile = _tile;
		prerequisite = _prereq;
		type = _type;
		direction = _dir;
		attacks = _capture;
		isConflicted = false;
		value = 0;
	}

	public MoveData ( Tile _tile, MoveData _prereq, MoveType _type, int _dir, params Tile [ ] _capture )
	{
		tile = _tile;
		prerequisite = _prereq;
		type = _type;
		direction = ( MoveDirection )_dir;
		attacks = _capture;
		isConflicted = false;
		value = 0;
	}

	/// <summary>
	/// Determines the value of a move for the AI.
	/// </summary>
	public void DetermineValue ( )
	{
		value = 0;
	}
}
