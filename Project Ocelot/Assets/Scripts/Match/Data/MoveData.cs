using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveData
{
	#region Public Classes

	public enum MoveType
	{
		MOVE,
		JUMP,
		SPECIAL
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

	public Hex [ ] AssistTargets
	{
		get;
		private set;
	}

	/// <summary>
	/// The tiles of units that would be attacked from this move.
	/// </summary>
	public Hex [ ] AttackTargets
	{
		get;
		private set;
	}

	/// <summary>
	/// Whether or not multiple moves are associated with the same destination.
	/// </summary>
	public bool IsConflicted
	{
		get;
		set;
	}

	/// <summary>
	/// Whether or not this move has an assist.
	/// </summary>
	public bool IsAssist
	{
		get
		{
			// Check for assist targets
			return AssistTargets != null && AssistTargets.Length > 0;
		}
	}

	/// <summary>
	/// Whether or not this move has an attack.
	/// </summary>
	public bool IsAttack
	{
		get
		{
			// Check for attack targets
			return AttackTargets != null && AttackTargets.Length > 0;
		}
	}

	/// <summary>
	/// Whether or not the move will result in a victory.
	/// </summary>
	public bool IsVictoryMove
	{
		get;
		set;
	}

	public MoveData ( Hex hex, MoveData prior, MoveType type, MoveDirection direction, Hex assistTarget, Hex attackTarget )
	{
		Destination = hex;
		PriorMove = prior;
		Type = type;
		Direction = direction;
		AssistTargets = assistTarget != null ? new Hex [ ] { assistTarget } : null;
		AttackTargets = attackTarget != null ? new Hex [ ] { attackTarget } : null;
		IsConflicted = false;
		IsVictoryMove = false;
		Value = 0;
	}

	public MoveData ( Hex hex, MoveData prior, MoveType type, int direction, Hex assistTarget, Hex attackTarget )
	{
		Destination = hex;
		PriorMove = prior;
		Type = type;
		Direction = (MoveDirection)direction;
		AssistTargets = assistTarget != null ? new Hex [ ] { assistTarget } : null;
		AttackTargets = attackTarget != null ? new Hex [ ] { attackTarget } : null;
		IsConflicted = false;
		IsVictoryMove = false;
		Value = 0;
	}

	public MoveData ( Hex hex, MoveData prior, MoveType type, MoveDirection direction, Hex [ ] assistTargets, Hex [ ] attackTargets )
	{
		Destination = hex;
		PriorMove = prior;
		Type = type;
		Direction = direction;
		AssistTargets = assistTargets;
		AttackTargets = attackTargets;
		IsConflicted = false;
		IsVictoryMove = false;
		Value = 0;
	}

	public MoveData ( Hex hex, MoveData prior, MoveType type, int direction, Hex [ ] assistTargets, Hex [ ] attackTargets )
	{
		Destination = hex;
		PriorMove = prior;
		Type = type;
		Direction = (MoveDirection)direction;
		AssistTargets = assistTargets;
		AttackTargets = attackTargets;
		IsConflicted = false;
		IsVictoryMove = false;
		Value = 0;
	}

	public MoveData ( Hex hex, MoveData prior, MoveType type, MoveDirection direction )
	{
		Destination = hex;
		PriorMove = prior;
		Type = type;
		Direction = direction;
		AssistTargets = null;
		AttackTargets = null;
		IsConflicted = false;
		IsVictoryMove = false;
		Value = 0;
	}

	public MoveData ( Hex hex, MoveData prior, MoveType type, int direction )
	{
		Destination = hex;
		PriorMove = prior;
		Type = type;
		Direction = (MoveDirection)direction;
		AssistTargets = null;
		AttackTargets = null;
		IsConflicted = false;
		IsVictoryMove = false;
		Value = 0;
	}

	#endregion // Move Data









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
