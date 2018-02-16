using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dropkick : HeroUnit
{
	/// <summary>
	///
	/// Hero Ability Information
	/// 
	/// Ability 1: Divebomb
	/// Type: Special Ability
	/// Default Cooldown: 3 Turns
	/// 
	/// Ability 2: Dropkick
	/// Type: Command Ability
	/// Default Cooldown: 4 Turns
	/// 
	/// </summary>

	#region Ability Data

	private Dictionary<Tile, int> dropkickTargetDirection = new Dictionary<Tile, int> ( );

	private const int MAX_DIVE_RANGE = 3;
	private const float DIVE_ANIMATION_TIME = 1.0f;
	private const float DIVE_ANIMATION_SCALE = 2.0f;
	private const string DROPKICK_STATUS_PROMPT = "Dropkick";

	#endregion // Ability Data

	#region Unit Override Functions

	/// <summary>
	/// Calculates all base moves available to a unit as well as any special ability moves.
	/// </summary>
	/// <param name="t"> The tile who's neighbor will be checked for moves. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach this tile. </param>
	/// <param name="returnOnlyJumps"> Whether or not only jump moves should be stored as available moves. </param>
	public override void FindMoves ( Tile t, MoveData prerequisite, bool returnOnlyJumps )
	{
		// Get base moves
		base.FindMoves ( t, prerequisite, returnOnlyJumps );

		// Get Divebomb moves
		if ( SpecialAvailabilityCheck ( CurrentAbility1, prerequisite ) )
			GetDivebomb ( t, MAX_DIVE_RANGE - 1 );

		// Get Dropkick availability
		CurrentAbility2.active = CommandAvailabilityCheck ( CurrentAbility2, prerequisite );
	}

	#endregion // Unit Override Functions

	#region HeroUnit Override Functions

	/// <summary>
	/// Checks if the hero is capable of using a special ability.
	/// Returns true if the special ability is available.
	/// </summary>
	/// /// <param name="current"> The current ability data for the special ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given special ability. </param>
	/// <returns> Whether or not the special ability can be used. </returns>
	protected override bool SpecialAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.SpecialAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check if any move has been made
		if ( prerequisite != null )
			return false;

		// Check for edge
		if ( !EdgeTileCheck ( currentTile ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Checks if this hero is capable of using a command ability.
	/// Returns true if the command ability is available.
	/// </summary>
	/// <param name="current"> The current ability data for the command ability being checked. </param>
	/// <param name="prerequisite"> The Move Data for any moves required for this hero unit to use the given command ability. </param>
	/// <returns> Whether or not the command ability can be used. </returns>
	protected override bool CommandAvailabilityCheck ( AbilitySettings current, MoveData prerequisite )
	{
		// Check base conditions
		if ( !base.CommandAvailabilityCheck ( current, prerequisite ) )
			return false;

		// Check if a target is available
		if ( !DropkickCheck ( ) )
			return false;

		// Return that the ability is available
		return true;
	}

	/// <summary>
	/// Uses the unit's special ability.
	/// Override this function to call specific special ability functions for a hero unit.
	/// </summary>
	/// <param name="data"> The move data required for this move. </param>
	protected override void UseSpecial ( MoveData data )
	{
		// Create animation for dive
		Tween t1 = transform.DOMove ( data.Tile.transform.position, DIVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Start teleport cooldown
				StartCooldown ( CurrentAbility1, Info.Ability1 );

				// Set unit and tile data
				SetUnitToTile ( data.Tile );
			} );
		Tween t2 = transform.DOScale ( DIVE_ANIMATION_SCALE, DIVE_ANIMATION_TIME / 2 )
			.SetLoops ( 2, LoopType.Yoyo );

		// Add animations to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t1, true ) );
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t2, false ) );

		int targetCount = 0;

		// Get knockback targets
		for ( int i = 0; i < data.Tile.neighbors.Length; i++ )
		{
			// Check for target
			if ( data.Tile.neighbors [ i ] != null && data.Tile.neighbors [ i ].currentUnit != null && data.Tile.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) && data.Tile.neighbors [ i ].currentUnit.status.CanBeMoved && KnockbackTileCheck ( data.Tile.neighbors [ i ].neighbors [ i ], i, false ) )
			{
				// Create animation for knockback
				targetCount++;
				DivebombKnockback ( data.Tile.neighbors [ i ].currentUnit, data.Tile.neighbors [ i ].neighbors [ i ], targetCount <= 1 );
			}
		}
	}

	/// <summary>
	/// Sets up the hero's command use.
	/// </summary>
	public override void StartCommand ( )
	{
		// Clear the board
		base.StartCommand ( );

		// Clear previous targets
		dropkickTargetDirection.Clear ( );

		// Get back direction
		IntPair back = GetBackDirection ( owner.direction );

		// Check each neighbor tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Check for back direction
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for target
			if ( currentTile.neighbors [ i ] != null && currentTile.neighbors [ i ].currentUnit != null && currentTile.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) && KnockbackTileCheck ( currentTile.neighbors [ i ], i, true ) )
			{
				// Mark target
				currentTile.neighbors [ i ].SetTileState ( TileState.AvailableCommand );

				// Store direction
				dropkickTargetDirection.Add ( currentTile.neighbors [ i ], i );
			}
		}
	}

	/// <summary>
	/// Selects the unit to dropkick.
	/// </summary>
	/// <param name="t"> The selected tile for the command. </param>
	public override void SelectCommandTile ( Tile t )
	{
		// Pause turn timer
		if ( MatchSettings.turnTimer )
			GM.UI.timer.PauseTimer ( );

		// Hide cancel button
		GM.UI.unitHUD.ability2.cancelButton.SetActive ( false );

		// Clear board
		GM.board.ResetTiles ( );

		// Create animation
		Sequence s = DOTween.Sequence ( );

		// Animate dropkick
		s.Append ( transform.DOMove ( currentTile.transform.position + ( ( t.transform.position - currentTile.transform.position ) * 0.5f ), MOVE_ANIMATION_TIME / 2 ) );
		s.Append ( transform.DOMove ( currentTile.transform.position, MOVE_ANIMATION_TIME / 2 ) );

		// Knockback targets
		DropkickKnockback ( t, dropkickTargetDirection [ t ], s );

		// End animation
		s.OnComplete ( ( ) =>
		{
			// Start cooldown
			StartCooldown ( CurrentAbility2, Info.Ability2 );

			// Apply status effect
			status.AddStatusEffect ( abilitySprite2, DROPKICK_STATUS_PROMPT, this, 1, StatusEffects.StatusType.CAN_MOVE );

			// Pause turn timer
			if ( MatchSettings.turnTimer )
				GM.UI.timer.ResumeTimer ( );

			// Get moves
			GM.GetTeamMoves ( );

			// Display team
			GM.DisplayAvailableUnits ( );
			GM.SelectUnit ( this );
		} );
	}

	#endregion // HeroUnit Override Functions

	#region Private Functions

	/// <summary>
	/// Checks to see if a tile is an edge tile.
	/// </summary>
	/// <param name="t"> The tile being check as an edge tile. </param>
	/// <returns> Whether or not the given tile is an edge tile. </returns>
	private bool EdgeTileCheck ( Tile t )
	{
		// Check each neighbor tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Check for edge
			if ( t.neighbors [ i ] == null )
				return true;
		}

		// Return that no edge was found
		return false;
	}

	/// <summary>
	/// Gets all tiles within range of the Divebomb ability.
	/// </summary>
	/// <param name="t"> The current tile who's neighbors are being checked. </param>
	/// <param name="count"> The range of tiles left to check. </param>
	private void GetDivebomb ( Tile t, int count )
	{
		// Check each neighbor tile
		for ( int i = 0; i < t.neighbors.Length; i++ )
		{
			// Check if the tile exists
			if ( t.neighbors [ i ] != null )
			{
				// Check if the tile is available to move to
				if ( OccupyTileCheck ( t.neighbors [ i ], null ) && MinimumDiveDistance ( t.neighbors [ i ] ) && !moveList.Exists ( match => match.Tile == t.neighbors [ i ] && match.Type == MoveData.MoveType.SPECIAL ) )
				{
					// Add as an available special move
					moveList.Add ( new MoveData ( t.neighbors [ i ], null, MoveData.MoveType.SPECIAL, i ) );	
				}

				// Check if the max range has been reached
				if ( count > 0 )
				{
					// Continue search
					GetDivebomb ( t.neighbors [ i ], count - 1 );
				}
			}
		}
	}

	/// <summary>
	/// Check is if a given tile is the minimum distance (2 tiles) away to dive to.
	/// </summary>
	/// <param name="t"> The tile being check for distance. </param>
	/// <returns> Whether or not the tile is the minimum distance away. </returns>
	private bool MinimumDiveDistance ( Tile t )
	{
		// Check for tile
		if ( t == null )
			return false;

		// Check current tile
		if ( t == currentTile )
			return false;

		// Check neighboring tiles
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
			if ( t == currentTile.neighbors [ i ] )
				return false;

		// Return that the tile is the minimum dive distance away
		return true;
	}

	/// <summary>
	/// Checks if a tile is available for knocking a unit back.
	/// </summary>
	/// <param name="t"> The tile being checked. </param>
	/// <param name="direction"> The direction the unit is being knocked back. </param>
	/// <param name="isRecursive"> Whether or not tiles should be continuously checked. </param>
	/// <returns> Whether or not a tile is available for knockback. </returns>
	private bool KnockbackTileCheck ( Tile t, int direction, bool isRecursive )
	{
		// Check for tile
		if ( t == null )
			return false;

		// Check for available tile
		if ( t.currentUnit == null )
		{
			// Return that there is room for knockback
			return true;
		}
		else
		{
			// Check if the unit can not be moved
			if ( !t.currentUnit.status.CanBeMoved )
				return false;

			// Check if tiles should continue to be checked
			if ( isRecursive )
			{
				// Check next tile in the direction
				return KnockbackTileCheck ( t.neighbors [ direction ], direction, isRecursive );
			}
			else
			{
				// Return that there is not room for knockback
				return false;
			}
		}
	}

	/// <summary>
	/// Knocks a unit back from the Divebomb ability
	/// </summary>
	/// <param name="target"> The unit affected by the Divebomb ability. </param>
	/// <param name="knockbackTile"> The tile the unit is being knocked back to. </param>
	private void DivebombKnockback ( Unit target, Tile knockbackTile, bool isAppend )
	{
		// Create animation
		Tween t = target.transform.DOMove ( knockbackTile.transform.position, MOVE_ANIMATION_TIME )
			.OnStart ( ( ) =>
			{
				// Interupt target
				target.InteruptUnit ( );
			} )
			.OnComplete ( ( ) =>
			{
				// Remove target from previous tile
				target.currentTile.currentUnit = null;

				// Set the target's new current tile
				target.currentTile = knockbackTile;
				knockbackTile.currentUnit = target;
			} );

		// Add animations to queue
		GM.animationQueue.Add ( new GameManager.TurnAnimation ( t, isAppend ) );
	}

	/// <summary>
	/// Checks if any targets are avialable for the Dropkick ability.
	/// </summary>
	/// <returns> Whether or not any targets are available. </returns>
	private bool DropkickCheck ( )
	{
		// Store which tiles are to be ignored
		IntPair back = GetBackDirection ( owner.direction );

		// Check each neighboring tile
		for ( int i = 0; i < currentTile.neighbors.Length; i++ )
		{
			// Ignore tiles that would allow for backward movement
			if ( i == back.FirstInt || i == back.SecondInt )
				continue;

			// Check for target
			if ( currentTile.neighbors [ i ] != null && currentTile.neighbors [ i ].currentUnit != null && currentTile.neighbors [ i ].currentUnit.UnitAttackCheck ( this ) && currentTile.neighbors [ i ].currentUnit.status.CanBeMoved && KnockbackTileCheck ( currentTile.neighbors [ i ].neighbors [ i ], i, true ) )
				return true;
		}

		// Return that no targets were found
		return false;
	}

	/// <summary>
	/// Knocks back a target from the Dropkick ability.
	/// </summary>
	/// <param name="t"> The current tile of the target. </param>
	/// <param name="direction"> The direction of the dropkick. </param>
	/// <param name="s"> The animation sequence for the dropkick. </param>
	private void DropkickKnockback ( Tile t, int direction, Sequence s )
	{
		// Check for additional targets
		if ( t.neighbors [ direction ] != null && t.neighbors [ direction ].currentUnit != null )
			DropkickKnockback ( t.neighbors [ direction ], direction, s );

		// Store target
		Unit target = t.currentUnit;

		// Clear target's current tile
		t.currentUnit = null;

		// Interupt target
		target.InteruptUnit ( );

		// Move target
		Tween animation = target.transform.DOMove ( t.neighbors [ direction ].transform.position, MOVE_ANIMATION_TIME )
			.OnComplete ( ( ) =>
			{
				// Set target to tile
				target.currentTile = t.neighbors [ direction ];
				t.neighbors [ direction ].currentUnit = target;
			} );

		// Add animation
		s.Join ( animation );
	}

	#endregion // Private Functions
}
