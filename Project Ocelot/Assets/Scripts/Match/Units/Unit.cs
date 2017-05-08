using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour 
{
	// Unit info
	public GameManager GM;
	public int instanceID; // Every unit in a match has a unique instance ID 
	public string characterName;
	public Sprite displaySprite;

	// Instance info
	public Tile currentTile;
	public Player team;
	public SpriteRenderer sprite;

	// Turn info
	public List<MoveData> moveList = new List<MoveData> ( );
	public Dictionary<Tile, MoveData> moveDic = new Dictionary<Tile, MoveData> ( );
	public struct BlockedTile
	{
		public Tile blockedTile;
		public bool clearOnUpdate;

		public BlockedTile ( Tile _blockedTile, bool _clearOnUpdate )
		{
			blockedTile = _blockedTile;
			clearOnUpdate = _clearOnUpdate;
		}
	}
	public List<BlockedTile> blockedTiles = new List<BlockedTile> ( );

	/// <summary>
	/// Determines if a tile is the previous tile the unit occupied.
	/// This is used to prevent a unit from endlessly jumping the same tile on one turn.
	/// </summary>
	protected bool IsBlockedTile ( Tile t )
	{
		// Return if the tile is blocked
		return blockedTiles.Exists ( item => item.blockedTile == t );
	}

	/// <summary>
	/// Adds a tile to the list of blocked tiles for this unit's turn.
	/// clearOnUpdate determines if the blocked tiles should be removed once the board has been updated (i.e. a unit has been captured).
	/// </summary>
	public void AddBlockedTile ( Tile t, bool clearOnUpdate )
	{
		// Add new blocked tile
		blockedTiles.Add ( new BlockedTile ( t, clearOnUpdate ) );
	}

	/// <summary>
	/// Clears the designated blocked tiles on a board update.
	/// </summary>
	public void ClearBlockedTiles ( )
	{
		// Remove all blocked tiles on a board update
		blockedTiles.RemoveAll ( item => item.clearOnUpdate == true );
	}

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
	public virtual void FindMoves ( bool returnOnlyJumps = false )
	{
		// Cleare previous move list
		moveList.Clear ( );

		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( team.direction );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for existing tile
			if ( currentTile.neighbors [ i ] != null && !IsBlockedTile ( currentTile.neighbors [ i ] ) )
			{
				// Check if tile is occupied by a unit
				if ( currentTile.neighbors [ i ].currentUnit != null )
				{
					// Check if tile is available for a jump
					if ( currentTile.neighbors [ i ].neighbors [ i ] != null && currentTile.neighbors [ i ].neighbors [ i ].currentUnit == null && !IsBlockedTile ( currentTile.neighbors [ i ].neighbors [ i ] ) )
					{
						// Check if the unit being jumped is capable of being captured
						if ( currentTile.neighbors [ i ].currentUnit.UnitCaptureCheck ( this ) )
						{
							// Add as an available capture
							moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.JumpCapture, i, currentTile.neighbors [ i ] ) );
						}
						else
						{
							// Add as an available jump
							moveList.Add ( new MoveData ( currentTile.neighbors [ i ].neighbors [ i ], MoveData.MoveType.Jump, i ) );
						}
					}
				}
				else
				{
					// Check if moves are being returned
					if ( !returnOnlyJumps )
					{
						// Add as an available move
						moveList.Add ( new MoveData ( currentTile.neighbors [ i ], MoveData.MoveType.Move, i ) );
					}
				}
			}
		}
	}

	/// <summary>
	/// Sets the list of available move for this unit for the current stage of the turn.
	/// </summary>
	public void SetMoveList ( )
	{
		// Clear previous list
		moveDic.Clear ( );

		// Set the list to be accessible by the tile of each potential move
		foreach ( MoveData move in moveList )
		{
			// Check for conflicted tiles
			if ( moveDic.ContainsKey ( move.tile ) )
			{
				// Mark tile as conflicted
				move.isConflicted = true;
				moveDic [ move.tile ].isConflicted = true;
			}
			else
			{
				// Add move to list
				moveDic.Add ( move.tile, move );
			}
		}
	}

	/// <summary>
	/// Determines if this unit can be captured by another unit jumping it.
	/// Returns true if this unit can be captured.
	/// </summary>
	public virtual bool UnitCaptureCheck ( Unit jumpingUnit )
	{
		// Check if the jumping unit is on the same team
		if ( jumpingUnit.team == team )
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
		case MoveData.MoveType.JumpCapture:
			CaptureUnit ( data );
			Jump ( data );
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
	/// </summary>
	protected virtual void Move ( MoveData data )
	{
		// Set unit and tile data
		SetUnitToTile ( data.tile );

		// Animate the unit's move
		transform.DOMove ( data.tile.transform.position, 0.5f )
			.SetRecyclable ( )
			.OnComplete ( () =>
			{
				// End the player's turn after the unit's move
				GM.EndTurn ( );
			} );
	}

	/// <summary>
	/// Has the unit jump an adjacent unit.
	/// </summary>
	protected virtual void Jump ( MoveData data )
	{
		// Set unit and tile data
		SetUnitToTile ( data.tile );

		// Animate the unit's jump
		transform.DOMove ( data.tile.transform.position, 1.0f )
			.SetRecyclable ( )
			.OnComplete ( () =>
			{
				// Check for any additional jumps
				FindMoves ( true );
				SetMoveList ( );

				// End the player's turn if there are no jumps available
				if ( moveList.Count > 0 )
				{
					// Continue the player's turn
					GM.ContinueTurn ( );
				}
				else
				{
					// End the player's turn after the unit's jump
					GM.EndTurn ( );
				}				
			} );
	}

	/// <summary>
	/// Captures an adjacent unit from a jump.
	/// Call this function on the unit that is jumping.
	/// </summary>
	protected virtual void CaptureUnit ( MoveData data )
	{
		// Mark that the board has been updated
		ClearBlockedTiles ( );

		// Capture the adjacent unit(s) being jumped
		foreach ( Tile t in data.capture )
			t.currentUnit.GetCaptured ( );
	}

	/// <summary>
	/// Captures this unit.
	/// Call this function on the unit being captured.
	/// </summary>
	public virtual void GetCaptured ( bool lostMatch = false )
	{
		// Remove unit from the team
		team.units.Remove ( this );

		// Remove unit reference from the tile
		currentTile.currentUnit = null;

		// Animate this unit being captured
		Sequence s = DOTween.Sequence ( )
			.AppendInterval ( 0.5f )
			.Append ( transform.DOScale ( new Vector3 ( 5, 5, 5 ), 0.5f ) )
			.Insert ( 0.5f, sprite.DOFade ( 0, 0.5f ) )
			.SetRecyclable ( )
			.OnComplete ( () =>
			{
				// Delete the unit
				Destroy ( this.gameObject );
			} )
			.Play ( );
	}

	public void SetTeamColor ( Player.TeamColor color )
	{
		sprite.color = Util.TeamColor ( color );
	}
}
