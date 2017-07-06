using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour 
{
	// Unit information
	public GameManager GM;
	public int instanceID; // Every unit in a match has a unique instance ID 
	public string characterName;
	public Sprite displaySprite;

	// Instance information
	public Tile currentTile;
	public Player owner;
	public SpriteRenderer sprite;
	public delegate void KOdelegate ( Unit u );
	public KOdelegate koDelegate;

	// Turn information
	public List<MoveData> moveList = new List<MoveData> ( );
	protected const float MOVE_ANIMATION_TIME = 0.5f;
	protected const float KO_ANIMATION_TIME = 0.5f;

	// Status information
	public StatusEffects status = new StatusEffects ( );

	/// <summary>
	/// Returns the two directions that are considered backwards movement for the unit.
	/// </summary>
	protected IntPair GetBackDirection ( Player.Direction direction )
	{
		// Store which tiles are to be ignored
		IntPair back = new IntPair ( 0, 1 );

		// Check the team's movement direction
		switch ( direction )
		{
		// Left to right movement
		case Player.Direction.LeftToRight:
			back = new IntPair ( (int)MoveData.MoveDirection.LeftBelow, (int)MoveData.MoveDirection.LeftAbove );
			break;

			// Right to left movement
		case Player.Direction.RightToLeft:
			back = new IntPair ( (int)MoveData.MoveDirection.RightAbove, (int)MoveData.MoveDirection.RightBelow );
			break;

			// Top left to bottom right movement
		case Player.Direction.TopLeftToBottomRight:
			back = new IntPair ( (int)MoveData.MoveDirection.Above, (int)MoveData.MoveDirection.LeftAbove );
			break;

			// Top right to bottom left movement
		case Player.Direction.TopRightToBottomLeft:
			back = new IntPair ( (int)MoveData.MoveDirection.Above, (int)MoveData.MoveDirection.RightAbove );
			break;

			// Bottom left to top right movement
		case Player.Direction.BottomLeftToTopRight:
			back = new IntPair ( (int)MoveData.MoveDirection.Below, (int)MoveData.MoveDirection.LeftBelow );
			break;

			// Bottom right to top left movement
		case Player.Direction.BottomRightToTopLeft:
			back = new IntPair ( (int)MoveData.MoveDirection.RightBelow, (int)MoveData.MoveDirection.Below );
			break;
		}

		// Return back tile elements
		return back;
	}

	/// <summary>
	/// Calculates all base moves available to a unit.
	/// </summary>
	public virtual void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Clear previous move list
		if ( prerequisite == null )
			moveList.Clear ( );

		// Check status effects
		if ( status.canMove )
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( owner.direction );

			// Check each neighboring tile
			for ( int i = 0; i < t.neighbors.Length; i++ )
			{
				// Ignore tiles that would allow for backward movement
				if ( i == back.FirstInt || i == back.SecondInt )
					continue;

				// Check if this unit can move to the neighboring tile
				if ( !returnOnlyJumps && OccupyTileCheck ( t.neighbors [ i ], prerequisite ) )
				{
					// Add as an available move
					moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.Move, i ) );
				}
				// Check if this unit can jump the neighboring tile
				else if ( JumpTileCheck ( t.neighbors [ i ] ) && OccupyTileCheck ( t.neighbors [ i ].neighbors [ i ], prerequisite ) )
				{
					// Track move data
					MoveData m;

					// Check if the neighboring unit can be attacked
					if ( t.neighbors [ i ].currentUnit != null && t.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) )
					{
						// Add as an available attack
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.Attack, i, t.neighbors [ i ] );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.Jump, i );
					}

					// Add move to the move list
					moveList.Add ( m );

					// Find additional jumps
					FindMoves ( t.neighbors [ i ].neighbors [ i ], m, true );
				}
			}
		}
	}

	/// <summary>
	/// Checks the entire list of potential moves for any conflicts by having the same tile being reached in multiple ways with the same prerequisite moves.
	/// </summary>
	public void MoveConflictCheck ( )
	{
		// Set the list to be accessible by the tile of each potential move
		foreach ( MoveData move in moveList )
		{
			// Check for conflicted tiles
			if ( moveList.Exists ( x => x.tile == move.tile && x.prerequisite == move.prerequisite && !x.isConflicted && x != move ) )
			{
				// Create list of conflicted moves 
				List<MoveData> conflicts = moveList.FindAll ( x => x.tile == move.tile && x.prerequisite == move.prerequisite && !x.isConflicted );

				// Mark moves as conflicted
				foreach ( MoveData m in conflicts )
					m.isConflicted = true;
			}
		}
	}

	/// <summary>
	/// Determines if a tile can be moved to by this unit.
	/// Returns true if the tile can be moved to.
	/// </summary>
	protected virtual bool OccupyTileCheck ( Tile t, MoveData prerequisite )
	{
		// Check if the tile exists
		if ( t == null )
			return false;

		// Check if the tile is blocked by a previous move that turn
		if ( CheckPrequisiteTiles ( t, prerequisite ) )
			return false;

		// Check if the tile currently occupied
		if ( t.currentUnit != null )
			return false;

		// Check if the tile has a tile object blocking it
		if ( t.currentObject != null && !t.currentObject.canBeOccupied )
			return false;

		// Return that the tile can be occupied by this unit
		return true;
	}

	/// <summary>
	/// Determines if a tile matches any of the tiles in the prerequisite moves.
	/// Returns true if a match is found.
	/// </summary>
	protected bool CheckPrequisiteTiles ( Tile t, MoveData m )
	{
		// Check for prerequisite move
		if ( m != null )
		{
			// Check if the tile matches
			if ( t == m.tile )
			{
				// Return that a match has been found
				return true;
			}
			else
			{
				// Check prerequisite move's tile
				return CheckPrequisiteTiles ( t, m.prerequisite );
			}
		}

		// Return that no matches were found
		return false;
	}

	/// <summary>
	/// Determines if a tile can be jumped by this unit.
	/// Returns true if the tile can be jumped.
	/// </summary>
	protected virtual bool JumpTileCheck ( Tile t )
	{
		// Check if the tile exists
		if ( t == null )
			return false;

		// Check if the tile is occupied
		if ( t.currentUnit == null && t.currentObject == null )
			return false;

		// Check if the tile has a tile object blocking it
		if ( t.currentObject != null && !t.currentObject.canBeJumped )
			return false;

		// Return that the tile can be jumped by this unit
		return true;
	}

	/// <summary>
	/// Determines if this unit can be attacked by another unit.
	/// Call this function on the unit being attacked with the unit that is attacking as the parameter.
	/// Returns true if this unit can be attacked.
	/// </summary>
	public virtual bool UnitAttackCheck ( Unit attacker )
	{
		// Check if the unit to be attacked is on the same team
		if ( attacker.owner == owner )
			return false;
		else
			return true;
	}

	/// <summary>
	/// Determines how the unit should move based on the Move Data given.
	/// </summary>
	public virtual void MoveUnit ( MoveData data )
	{
		// Check move data
		switch ( data.type )
		{
		case MoveData.MoveType.Move:
			Move ( data );
			break;
		case MoveData.MoveType.Jump:
			Jump ( data );
			break;
		case MoveData.MoveType.Attack:
			Jump ( data );
			AttackUnit ( data );
			break;
		}
	}

	/// <summary>
	/// Sets all of the unit and tile data for moving a unit to a tile.
	/// </summary>
	protected void SetUnitToTile ( Tile t )
	{
		// Remove unit from previous tile
		currentTile.currentUnit = null;

		// Set the unit's new current tile
		currentTile = t;
		t.currentUnit = this;
	}

	/// <summary>
	/// Moves the unit to an adjecent tile.
	/// This function builds the animation queue from the move data.
	/// </summary>
	protected virtual void Move ( MoveData data )
	{
		// Create animation
		Tween t = transform.DOMove ( data.tile.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( () =>
			{
				// Set unit and tile data
				SetUnitToTile ( data.tile );
			} );

		// Add animation to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Have the unit jump an adjacent unit.
	/// This function builds the animation queue from the move data.
	/// </summary>
	protected virtual void Jump ( MoveData data )
	{
		// Create animation
		Tween t = transform.DOMove ( data.tile.transform.position, MOVE_ANIMATION_TIME * 2 )
			.OnComplete ( ( ) =>
			{
				// Set unit and tile data
				SetUnitToTile ( data.tile );
			} );

		// Add animation to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Attacks the adjacent unit.
	/// Call this function on the attacking unit.
	/// This function builds the animation queue from the move data.
	/// </summary>
	protected virtual void AttackUnit ( MoveData data )
	{
		// KO unit(s) being attacked
		foreach ( Tile t in data.attacks )
		{
			// Interupt unit
			t.currentUnit.InteruptUnit ( );

			// Attack unit
			t.currentUnit.GetAttacked ( );
		}
	}

	/// <summary>
	/// Attack and KO this unit.
	/// Call this function on the unit being attacked.
	/// This function builds the animation queue from the move data.
	/// </summary>
	public virtual void GetAttacked ( bool usePostAnimationQueue = false )
	{
		// Call KO delegate
		if ( koDelegate != null )
			koDelegate ( this );

		// Create animation
		Tween t1 = transform.DOScale ( new Vector3 ( 5, 5, 5 ), KO_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Display deactivation
				GM.UI.GetPlayerHUD ( this ).DisplayDeactivation ( instanceID );

				// Remove unit from the team
				owner.units.Remove ( this );

				// Remove unit reference from the tile
				currentTile.currentUnit = null;

				// Delete the unit
				Destroy ( this.gameObject );
			} )
			.Pause ( );
		Tween t2 = sprite.DOFade ( 0, MOVE_ANIMATION_TIME )
			.Pause ( );

		// Add animations to queue
		if ( usePostAnimationQueue )
		{
			GM.postAnimationQueue.Add ( new GameManager.PostTurnAnimation ( this, owner, new GameManager.TurnAnimation ( t1, false ), new GameManager.TurnAnimation ( t2, false ) ) );
		}
		else
		{
			GM.animationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
			GM.animationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
		}
	}

	/// <summary>
	/// Interupts any actions that take more than one turn to complete that this unit is in the process of doing.
	/// Call this function when this unit is being attacked or being affected by some interupting ability.
	/// IMPORTANT: Be sure to call this function first before the interupting action since Interupts change the status effects of the action being interupted and the interupting action may apply new status effects.
	/// </summary>
	public virtual void InteruptUnit ( )
	{

	}

	public void SetTeamColor ( Player.TeamColor color )
	{
		sprite.color = Util.TeamColor ( color );
	}
}
