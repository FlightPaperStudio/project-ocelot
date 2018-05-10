using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour 
{
	#region Unit Data

	public GameManager GM;
	public int unitID; // The ID of the unit type (i.e. Leader, Pawn, etc.)
	public int instanceID; // Every unit in a match has a unique instance ID 
	public string characterName;
	public Sprite displaySprite;

	#endregion // Unit Data

	#region Instance Data

	public Tile currentTile;
	public Player owner;
	public SpriteRenderer sprite;
	public delegate void KOdelegate ( Unit u );
	public KOdelegate koDelegate;

	#endregion // Instance Data

	#region Turn Data

	public List<MoveData> moveList = new List<MoveData> ( );
	protected const float MOVE_ANIMATION_TIME = 0.5f;
	protected const float KO_ANIMATION_TIME = 0.5f;

	#endregion // Turn Data

	#region Status Data

	public StatusEffects status = new StatusEffects ( );

	#endregion // Status Data

	#region Public Virtual Functions

	/// <summary>
	/// Calculates all base moves available to a unit.
	/// </summary>
	/// <param name="t"> The tile who's neighbor will be checked for moves. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach this tile. </param>
	/// /// <param name="returnOnlyJumps"> Whether or not only jump moves should be stored as available moves. </param>
	public virtual void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Clear previous move list
		if ( prerequisite == null )
			moveList.Clear ( );

		// Check status effects
		if ( status.CanMove )
		{
			// Store which tiles are to be ignored
			IntPair back = GetBackDirection ( owner.TeamDirection );

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
					moveList.Add ( new MoveData ( t.neighbors [ i ], prerequisite, MoveData.MoveType.MOVE, i ) );
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
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.ATTACK, i, t.neighbors [ i ] );
					}
					else
					{
						// Add as an available jump
						m = new MoveData ( t.neighbors [ i ].neighbors [ i ], prerequisite, MoveData.MoveType.JUMP, i );
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
	/// Determines if this unit can be attacked by another unit.
	/// Call this function on the unit being attacked with the unit that is attacking as the parameter.
	/// Returns true if this unit can be attacked.
	/// </summary>
	/// <param name="attacker"> The unit doing the attacking. </param>
	/// <returns> Whether or not this unit can be attacked by another unit. </returns>
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
	/// <param name="data"> The Move Data for the selected move. </param>
	public virtual void MoveUnit ( MoveData data )
	{
		// Check move data
		switch ( data.Type )
		{
		case MoveData.MoveType.MOVE:
			Move ( data );
			break;
		case MoveData.MoveType.JUMP:
			Jump ( data );
			break;
		case MoveData.MoveType.ATTACK:
			Jump ( data );
			AttackUnit ( data );
			break;
		}
	}

	/// <summary>
	/// Attack and KO this unit.
	/// Call this function on the unit being attacked.
	/// This function builds the animation queue from the Move Data.
	/// </summary>
	/// <param name="usePostAnimationQueue"> Whether or not the KO animation should play at the end of the turn animations. </param>
	public virtual void GetAttacked ( bool usePostAnimationQueue = false )
	{
		// Call KO delegate
		if ( koDelegate != null )
			koDelegate ( this );

		// Create animation
		Tween t1 = transform.DOScale ( new Vector3 ( 5, 5, 5 ), KO_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Display KO in HUD
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).DisplayKO ( instanceID );

				// Remove unit from the team
				owner.UnitInstances.Remove ( this );

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
			GM.PostAnimationQueue.Add ( new GameManager.PostTurnAnimation ( this, owner, new GameManager.TurnAnimation ( t1, false ), new GameManager.TurnAnimation ( t2, false ) ) );
		}
		else
		{
			GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
			GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );
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

	#endregion // Public Virtual Functions

	#region Public Functions

	/// <summary>
	/// Checks the entire list of potential moves for any conflicts from having the same tile being reached in multiple ways with the same prerequisite moves.
	/// </summary>
	public void MoveConflictCheck ( )
	{
		// Set the list to be accessible by the tile of each potential move
		foreach ( MoveData move in moveList )
		{
			// Check for conflicted tiles
			if ( moveList.Exists ( x => x.Tile == move.Tile && x.Prerequisite == move.Prerequisite && !x.isConflicted && x != move ) )
			{
				// Create list of conflicted moves 
				List<MoveData> conflicts = moveList.FindAll ( x => x.Tile == move.Tile && x.Prerequisite == move.Prerequisite && !x.isConflicted );

				// Mark moves as conflicted
				foreach ( MoveData m in conflicts )
					m.isConflicted = true;
			}
		}
	}

	/// <summary>
	/// Sets this unit's team color.
	/// </summary>
	/// <param name="color"> The team color being assigned to this unit. </param>
	public void SetTeamColor ( Player.TeamColor color )
	{
		sprite.color = Util.TeamColor ( color );
	}

	#endregion // Public Functions

	#region Protected Virtual Functions

	/// <summary>
	/// Determines if a tile can be moved to by this unit.
	/// Returns true if the tile can be moved to.
	/// </summary>
	/// <param name="t"> The tile being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach the given tile. </param>
	/// <returns> Whether or not this unit can move to the given tile. </returns>
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
	/// Determines if a tile can be jumped by this unit.
	/// Returns true if the tile can be jumped.
	/// </summary>
	/// <param name="t"> The tile being checked. </param>
	/// <returns> Whether or not this unit can jump over the given tile. </returns>
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
	/// Moves the unit to an adjecent tile.
	/// This function builds the animation queue from the Move Data.
	/// </summary>
	/// <param name="data"> The Move Data for the selected move. </param>
	protected virtual void Move ( MoveData data )
	{
		// Create animation
		Tween t = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Set unit and tile data
				SetUnitToTile ( data.Tile );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Have the unit jump an adjacent unit.
	/// This function builds the animation queue from the Move Data.
	/// </summary>
	/// <param name="data"> The Move Data for the selected move. </param>
	protected virtual void Jump ( MoveData data )
	{
		// Create animation
		Tween t = transform.DOMove ( data.Tile.transform.position, MOVE_ANIMATION_TIME * 2 )
			.OnComplete ( ( ) =>
			{
				// Set unit and tile data
				SetUnitToTile ( data.Tile );
			} );

		// Add animation to queue
		GM.AnimationQueue.Add ( new GameManager.TurnAnimation ( t, true ) );
	}

	/// <summary>
	/// Attacks the adjacent unit.
	/// Call this function on the attacking unit.
	/// This function builds the animation queue from the Move Data.
	/// </summary>
	/// <param name="data"> The Move Data for the selected move. </param>
	protected virtual void AttackUnit ( MoveData data )
	{
		// KO unit(s) being attacked
		foreach ( Tile t in data.Attacks )
		{
			// Interupt unit
			t.currentUnit.InteruptUnit ( );

			// Attack unit
			t.currentUnit.GetAttacked ( );
		}
	}

	#endregion // Protected Virtual Functions

	#region Protected Functions

	/// <summary>
	/// Returns the two directions that are considered backwards movement for the unit.
	/// </summary>
	/// <param name="direction"> The unit's movement direction. </param>
	/// <returns> The pair of integers that represent the direction of the two tiles to be considered behind the unit. </returns>
	protected IntPair GetBackDirection ( Player.Direction direction )
	{
		// Store which tiles are to be ignored
		IntPair back = new IntPair ( 0, 1 );

		// Check the team's movement direction
		switch ( direction )
		{
		// Left to right movement
		case Player.Direction.LEFT_TO_RIGHT:
			back = new IntPair ( (int)MoveData.MoveDirection.LEFT_BELOW, (int)MoveData.MoveDirection.LEFT_ABOVE );
			break;

		// Right to left movement
		case Player.Direction.RIGHT_TO_LEFT:
			back = new IntPair ( (int)MoveData.MoveDirection.RIGHT_ABOVE, (int)MoveData.MoveDirection.RIGHT_BELOW );
			break;

		// Top left to bottom right movement
		case Player.Direction.TOP_LEFT_TO_BOTTOM_RIGHT:
			back = new IntPair ( (int)MoveData.MoveDirection.ABOVE, (int)MoveData.MoveDirection.LEFT_ABOVE );
			break;

		// Top right to bottom left movement
		case Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT:
			back = new IntPair ( (int)MoveData.MoveDirection.ABOVE, (int)MoveData.MoveDirection.RIGHT_ABOVE );
			break;

		// Bottom left to top right movement
		case Player.Direction.BOTTOM_LEFT_TO_TOP_RIGHT:
			back = new IntPair ( (int)MoveData.MoveDirection.BELOW, (int)MoveData.MoveDirection.LEFT_BELOW );
			break;

		// Bottom right to top left movement
		case Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT:
			back = new IntPair ( (int)MoveData.MoveDirection.RIGHT_BELOW, (int)MoveData.MoveDirection.BELOW );
			break;
		}

		// Return back tile elements
		return back;
	}

	/// <summary>
	/// Determines if a tile matches any of the tiles in the prerequisite moves.
	/// Returns true if a match is found.
	/// </summary>
	/// <param name="t"> The tile being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach the given tile. </param>
	/// <returns> Whether or not this tile arleady exists in a path of moves. </returns>
	protected bool CheckPrequisiteTiles ( Tile t, MoveData m )
	{
		// Check for prerequisite move
		if ( m != null )
		{
			// Check if the tile matches
			if ( t == m.Tile )
			{
				// Return that a match has been found
				return true;
			}
			else
			{
				// Check prerequisite move's tile
				return CheckPrequisiteTiles ( t, m.Prerequisite );
			}
		}

		// Return that no matches were found
		return false;
	}

	/// <summary>
	/// Sets all of the unit and tile data for moving a unit to a tile.
	/// </summary>
	/// <param name="t"> The tile this unit is moving to. </param>
	protected void SetUnitToTile ( Tile t )
	{
		// Remove unit from previous tile
		currentTile.currentUnit = null;

		// Set the unit's new current tile
		currentTile = t;
		t.currentUnit = this;
	}

	#endregion // Protected Functions
}
