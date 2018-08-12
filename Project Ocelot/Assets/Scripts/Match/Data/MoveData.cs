using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveData
{
	#region Public Classes

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
		NORTH = 0,
		NORTHEAST = 1,
		SOUTHEAST = 2,
		SOUTH = 3,
		SOUTHWEST = 4,
		NORTHWEST = 5,
		NONE = 6,
		DIRECT = 7
	}

	#endregion // Public Classes

	#region Move Data

	/// <summary>
	/// The destination of this potential move.
	/// </summary>
	public Hex Destination
	{
		get;
		private set;
	}

	/// <summary>
	/// The move required to make this move.
	/// </summary>
	public MoveData PriorMove
	{
		get;
		private set;
	}

	/// <summary>
	/// The type of move.
	/// </summary>
	public MoveType Type
	{
		get;
		private set;
	}

	/// <summary>
	/// The direction of the move from the previous tile.
	/// </summary>
	public MoveDirection Direction
	{
		get;
		private set;
	}

	/// <summary>
	/// The tiles of units that would be attacked from this move.
	/// </summary>
	public Hex [ ] Attacks
	{
		get;
		private set;
	}

	public MoveData ( Hex hex, MoveData prior, MoveType type, MoveDirection direction, params Hex [ ] attacks )
	{
		Destination = hex;
		PriorMove = prior;
		Type = type;
		Direction = direction;
		Attacks = attacks;
		isConflicted = false;
		Value = 0;
	}

	public MoveData ( Hex hex, MoveData prior, MoveType type, int direction, params Hex [ ] attacks )
	{
		Destination = hex;
		PriorMove = prior;
		Type = type;
		Direction = (MoveDirection)direction;
		Attacks = attacks;
		isConflicted = false;
		Value = 0;
	}

	#endregion // Move Data







	public bool isConflicted;

	// Tracks the value of the move
	// This is used only for the AI's move determination
	public int Value
	{
		get;
		private set;
	}

	

	/// <summary>
	/// Determines the value of a move for the AI.
	/// </summary>
	public void DetermineValue ( )
	{
		Value = 0;
	}
}
