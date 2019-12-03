using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Units
{
	public class Unit : MonoBehaviour
	{
		#region Unit Data

		public Sprite displaySprite;

		/// <summary>
		/// The game manager for the match.
		/// </summary>
		protected Match.GameManager GM
		{
			get;
			private set;
		}

		/// <summary>
		/// The ID for this unit type.
		/// </summary>
		protected int ID
		{
			get;
			private set;
		}

		/// <summary>
		/// The unique ID for this particular unit instance in a match.
		/// </summary>
		public int InstanceID
		{
			get;
			private set;
		}

		/// <summary>
		/// The unit's instance data.
		/// </summary>
		public UnitInstanceData InstanceData
		{
			get;
			protected set;
		}

		#endregion // Unit Data

		#region Instance Data

		public Hex CurrentHex;
		public Match.Player Owner;
		public SpriteRenderer sprite;
		public delegate void KOdelegate ( Unit u );
		public KOdelegate koDelegate;

		#endregion // Instance Data

		#region Turn Data

		public List<MoveData> MoveList = new List<MoveData> ( );

		protected const float MOVE_ANIMATION_TIME = 0.5f;
		protected const float KO_ANIMATION_TIME = 0.5f;

		/// <summary>
		/// Get the best available move for this unit.
		/// </summary>
		public MoveData BestMove
		{
			get
			{
				// Store the best possible move
				MoveData best = null;
				int max = -10000;

				// Search for best move
				for ( int i = 0; i < MoveList.Count; i++ )
				{
					// Check for better score
					if ( MoveList [ i ].FinalValue > max )
					{
						// Store best move
						best = MoveList [ i ];
						max = MoveList [ i ].FinalValue;
					}
				}

				// Return best move
				return best;
			}
		}

		#endregion // Turn Data

		#region Status Data

		public StatusEffects Status = new StatusEffects ( );

		#endregion // Status Data

		#region Public Virtual Functions

		/// <summary>
		/// Sets the unit's instance data from the player's unit setting data.
		/// </summary>
		/// <param name="settingData"> The unit's setting data. </param>
		public virtual void InitializeInstance ( Match.GameManager gm, int instanceID, UnitSettingData settingData )
		{
			// Set manager
			GM = gm;

			// Set unit ID
			ID = settingData.ID;

			// Set instance's ID
			InstanceID = instanceID;

			// Set instance data from setting data
			InstanceData = new UnitInstanceData
			{
				ID = settingData.ID,
				UnitName = settingData.UnitName,
				UnitNickname = settingData.UnitNickname,
				UnitBio = settingData.UnitBio,
				FinishingMove = settingData.FinishingMove,
				Role = settingData.Role,
				Slots = settingData.Slots,
				Portrait = settingData.Portrait,
				IsEnabled = settingData.IsEnabled
			};

			// Set ability instance data from setting data
			List<AbilityInstanceData> abilities = new List<AbilityInstanceData> ( );

			// Set instance data for ability 1
			if ( settingData.Ability1 != null )
				abilities.Add ( new AbilityInstanceData
				{
					ID = settingData.Ability1.ID,
					AbilityName = settingData.Ability1.AbilityName,
					AbilityDescription = settingData.Ability1.AbilityDescription,
					Icon = settingData.Ability1.Icon,
					Type = settingData.Ability1.Type,
					IsEnabled = settingData.Ability1.IsEnabled,
					Duration = settingData.Ability1.Duration,
					Cooldown = settingData.Ability1.Cooldown,
					PerkName = settingData.Ability1.PerkName,
					PerkValue = settingData.Ability1.PerkValue,

					IsAvailable = true,
					CurrentDuration = 0,
					CurrentCooldown = 0
				} );

			// Set instance data for ability 2
			if ( settingData.Ability2 != null )
				abilities.Add ( new AbilityInstanceData
				{
					ID = settingData.Ability2.ID,
					AbilityName = settingData.Ability2.AbilityName,
					AbilityDescription = settingData.Ability2.AbilityDescription,
					Icon = settingData.Ability2.Icon,
					Type = settingData.Ability2.Type,
					IsEnabled = settingData.Ability2.IsEnabled,
					Duration = settingData.Ability2.Duration,
					Cooldown = settingData.Ability2.Cooldown,
					PerkName = settingData.Ability2.PerkName,
					PerkValue = settingData.Ability2.PerkValue,

					IsAvailable = true,
					CurrentDuration = 0,
					CurrentCooldown = 0
				} );

			// Set instance data for ability 3
			if ( settingData.Ability3 != null )
				abilities.Add ( new AbilityInstanceData
				{
					ID = settingData.Ability3.ID,
					AbilityName = settingData.Ability3.AbilityName,
					AbilityDescription = settingData.Ability3.AbilityDescription,
					Icon = settingData.Ability3.Icon,
					Type = settingData.Ability3.Type,
					IsEnabled = settingData.Ability3.IsEnabled,
					Duration = settingData.Ability3.Duration,
					Cooldown = settingData.Ability3.Cooldown,
					PerkName = settingData.Ability3.PerkName,
					PerkValue = settingData.Ability3.PerkValue,

					IsAvailable = true,
					CurrentDuration = 0,
					CurrentCooldown = 0
				} );

			// Set ability instances
			InstanceData.InitializeAbilities ( abilities.ToArray ( ) );
		}

		/// <summary>
		/// Calculates all base moves available to a unit.
		/// </summary>
		/// <param name="hex"> The tile who's neighbor will be checked for moves. </param>
		/// <param name="prior"> The Move Data for any moves required for the unit to reach this tile. </param>
		/// /// <param name="returnOnlyJumps"> Whether or not only jump moves should be stored as available moves. </param>
		public virtual void FindMoves ( Hex hex, MoveData prior, bool returnOnlyJumps )
		{
			// Clear previous move list
			if ( prior == null )
				MoveList.Clear ( );

			// Check status effects
			if ( Status.CanMove )
			{
				// Check each neighboring tile
				for ( int i = 0; i < hex.Neighbors.Length; i++ )
				{
					// Ignore tiles that would allow for backward movement
					//if ( hex.Grid.IsBackDirection ( CurrentTile.Cube, hex.Cube, owner.TeamDirection ) )
					//	continue;

					// Check if this unit can move to the neighboring tile
					if ( !returnOnlyJumps && OccupyTileCheck ( hex.Neighbors [ i ], prior ) )
					{
						// Add as an available move
						AddMove ( new MoveData ( hex.Neighbors [ i ], prior, MoveData.MoveType.MOVE, i ) );
					}
					// Check if this unit can jump the neighboring tile
					else if ( JumpTileCheck ( hex.Neighbors [ i ] ) && OccupyTileCheck ( hex.Neighbors [ i ].Neighbors [ i ], prior ) )
					{
						// Track move data
						MoveData move;

						// Check if the neighboring unit can be attacked
						if ( AttackTileCheck ( hex.Neighbors [ i ] ) )
						{
							// Add as an available attack
							move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prior, MoveData.MoveType.JUMP, i, null, hex.Neighbors [ i ] );
						}
						else
						{
							// Add as an available assist
							move = new MoveData ( hex.Neighbors [ i ].Neighbors [ i ], prior, MoveData.MoveType.JUMP, i, hex.Neighbors [ i ], null );
						}

						// Add move to the move list
						AddMove ( move );

						// Find additional jumps
						FindMoves ( hex.Neighbors [ i ].Neighbors [ i ], move, true );
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
		/// <param name="friendlyFire"> Whether or not the attack can affect ally units. </param>
		/// <returns> Whether or not this unit can be attacked by another unit. </returns>
		public virtual bool UnitAttackCheck ( Unit attacker, bool friendlyFire = false )
		{
			// Check if the units are on the same team
			if ( !friendlyFire && attacker.Owner == Owner )
				return false;

			// Check if the attacking unit can attack
			if ( !attacker.Status.CanAttack )
				return false;

			// Check if this unit can be attacked
			if ( !Status.CanBeAttacked )
				return false;

			// Return that this unit can be attacked by the attacking unit
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
				if ( data.IsAssist )
					GetAssisted ( data );
				else if ( data.IsAttack )
					AttackUnit ( data );
				break;
			}
		}

		public virtual void Assist ( )
		{

		}

		/// <summary>
		/// Attack and KO this unit.
		/// Call this function on the unit being attacked.
		/// This function builds the animation queue from the Move Data.
		/// </summary>
		/// <param name="usePostAnimationQueue"> Whether or not the KO animation should play at the end of the turn animations. </param>
		public virtual void GetAttacked ( bool usePostAnimationQueue = false )
		{
			// KO unit
			UnitKO ( usePostAnimationQueue );
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
			foreach ( MoveData move in MoveList )
			{
				// Check for conflicted tiles
				if ( MoveList.Exists ( x => x.Destination == move.Destination && x.PriorMove == move.PriorMove && !x.IsConflicted && x != move ) )
				{
					// Create list of conflicted moves 
					List<MoveData> conflicts = MoveList.FindAll ( x => x.Destination == move.Destination && x.PriorMove == move.PriorMove && !x.IsConflicted );

					// Mark moves as conflicted
					foreach ( MoveData m in conflicts )
						m.IsConflicted = true;
				}
			}
		}

		/// <summary>
		/// Sets this unit's team color.
		/// </summary>
		/// <param name="color"> The team color being assigned to this unit. </param>
		public void SetTeamColor ( Match.Player.TeamColor color )
		{
			sprite.color = Tools.Util.TeamColor ( color );
		}

		/// <summary>
		/// KO's this unit.
		/// </summary>
		/// <param name="usePostAnimationQueue"> Whether or not the KO animation should bein the Post Animation Queue. </param>
		public void UnitKO ( bool usePostAnimationQueue = false )
		{
			// Call KO delegate
			if ( koDelegate != null )
				koDelegate ( this );

			// Create animation
			Tween t1 = transform.DOScale ( new Vector3 ( 5, 5, 5 ), KO_ANIMATION_TIME )
				.OnComplete ( ( ) =>
				{
				// Display KO in HUD
				GM.UI.matchInfoMenu.GetPlayerHUD ( this ).DisplayKO ( InstanceID );

				// Remove unit from the team
				Owner.UnitInstances.Remove ( this );

				// Remove unit reference from the tile
				CurrentHex.Tile.CurrentUnit = null;

				// Delete the unit
				Destroy ( this.gameObject );
				} )
				.Pause ( );
			Tween t2 = sprite.DOFade ( 0, MOVE_ANIMATION_TIME )
				.Pause ( );

			// Add animations to queue
			if ( usePostAnimationQueue )
			{
				GM.PostAnimationQueue.Add ( new Match.GameManager.PostTurnAnimation ( this, Owner, new Match.GameManager.TurnAnimation ( t1, false ), new Match.GameManager.TurnAnimation ( t2, false ) ) );
			}
			else
			{
				GM.AnimationQueue.Add ( new Match.GameManager.TurnAnimation ( t1, true ) );
				GM.AnimationQueue.Add ( new Match.GameManager.TurnAnimation ( t2, false ) );
			}
		}

		#endregion // Public Functions

		#region Protected Virtual Functions

		/// <summary>
		/// Determines if a tile can be moved to by this unit.
		/// Returns true if the tile can be moved to.
		/// </summary>
		/// <param name="hex"> The tile being checked. </param>
		/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach the given tile. </param>
		/// <returns> Whether or not this unit can move to the given tile. </returns>
		protected virtual bool OccupyTileCheck ( Hex hex, MoveData prerequisite )
		{
			// Check if the tile exists
			if ( hex == null )
				return false;

			// Check if the tile is blocked by a previous move that turn
			if ( PriorMoveCheck ( hex, prerequisite ) )
				return false;

			// Check if the tile is currently occupied
			if ( hex.Tile.IsOccupied )
				return false;

			// Return that the tile can be occupied by this unit
			return true;
		}

		/// <summary>
		/// Determines if a tile can be jumped by this unit.
		/// Returns true if the tile can be jumped.
		/// </summary>
		/// <param name="hex"> The tile being checked. </param>
		/// <returns> Whether or not this unit can jump over the given tile. </returns>
		protected virtual bool JumpTileCheck ( Hex hex )
		{
			// Check if the tile exists
			if ( hex == null )
				return false;

			// For starting tile
			if ( hex == CurrentHex )
				return false;

			// Check if the tile is occupied
			if ( !hex.Tile.IsOccupied )
				return false;

			// Check if the unit on the tile can assist
			if ( hex.Tile.CurrentUnit != null && !hex.Tile.CurrentUnit.Status.CanAssist && !hex.Tile.CurrentUnit.Status.CanBeAttacked )
				return false;

			// Check if the tile has a tile object blocking it
			if ( hex.Tile.CurrentObject != null && !hex.Tile.CurrentObject.CanAssist && !hex.Tile.CurrentObject.CanBeAttacked )
				return false;

			// Return that the tile can be jumped by this unit
			return true;
		}

		/// <summary>
		/// Determines if a tile can assist this unit.
		/// Returns true if the tile can assist.
		/// </summary>
		/// <param name="hex"> The tile being checked. </param>
		/// <returns> Whether or not the unit or object on the tile can assist. </returns>
		protected virtual bool AssistTileCheck ( Hex hex )
		{
			// Check if the tile exists
			if ( hex == null )
				return false;

			// For starting tile
			if ( hex == CurrentHex )
				return false;

			// Check if the tile is occupied
			if ( !hex.Tile.IsOccupied )
				return false;

			// Check for unit
			if ( hex.Tile.CurrentUnit != null )
			{
				// Check if the unit can assist
				if ( !hex.Tile.CurrentUnit.Status.CanAssist )
					return false;
			}

			// Check for object
			if ( hex.Tile.CurrentObject != null )
			{
				// Check if the object can be attack
				if ( !hex.Tile.CurrentObject.CanAssist )
					return false;
			}

			// Return that the tile can be attacked by this unit
			return true;
		}

		/// <summary>
		/// Determines if a tile can be attacked by this unit.
		/// Returns true if the tile can be attacked.
		/// </summary>
		/// <param name="hex"> The tile being checked. </param>
		/// <returns> Whether or not the unit or object on the tile can be attacked. </returns>
		protected virtual bool AttackTileCheck ( Hex hex )
		{
			// Check if the tile exists
			if ( hex == null )
				return false;

			// For starting tile
			if ( hex == CurrentHex )
				return false;

			// Check if the tile is occupied
			if ( !hex.Tile.IsOccupied )
				return false;

			// Check for unit
			if ( hex.Tile.CurrentUnit != null )
			{
				// Check if this unit can attack the unit
				if ( !hex.Tile.CurrentUnit.UnitAttackCheck ( this ) )
					return false;
			}

			// Check for object
			if ( hex.Tile.CurrentObject != null )
			{
				// Check if the object can be attack
				if ( !hex.Tile.CurrentObject.UnitAttackCheck ( this ) )
					return false;
			}

			// Return that the tile can be attacked by this unit
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
			Tween t = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME )
				.OnComplete ( ( ) =>
				{
				// Set unit and tile data
				SetUnitToTile ( data.Destination );
				} );

			// Add animation to queue
			GM.AnimationQueue.Add ( new Match.GameManager.TurnAnimation ( t, true ) );
		}

		/// <summary>
		/// Have the unit jump an adjacent unit.
		/// This function builds the animation queue from the Move Data.
		/// </summary>
		/// <param name="data"> The Move Data for the selected move. </param>
		protected virtual void Jump ( MoveData data )
		{
			// Create animation
			Tween t = transform.DOMove ( data.Destination.transform.position, MOVE_ANIMATION_TIME * 2 )
				.OnComplete ( ( ) =>
				{
				// Set unit and tile data
				SetUnitToTile ( data.Destination );
				} );

			// Add animation to queue
			GM.AnimationQueue.Add ( new Match.GameManager.TurnAnimation ( t, true ) );
		}

		/// <summary>
		/// Gets assisted by the unit being jumped.
		/// Call this funciton on the moving unit.
		/// This function is used for stat tracking.
		/// </summary>
		/// <param name="data"></param>
		protected virtual void GetAssisted ( MoveData data )
		{
			// Add assists for each target
			foreach ( Hex hex in data.AssistTargets )
			{
				// Check for tile
				if ( hex == null )
					continue;

				// Check if tile is occupied
				if ( !hex.Tile.IsOccupied )
					continue;

				// Check for unit
				if ( hex.Tile.CurrentUnit != null )
				{
					// Track assist
					hex.Tile.CurrentUnit.Assist ( );
				}

				// Check for object
				if ( hex.Tile.CurrentObject != null && hex.Tile.CurrentObject.CanAssist )
				{
					// Track assist
					hex.Tile.CurrentObject.Assist ( );
				}
			}
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
			foreach ( Hex hex in data.AttackTargets )
			{
				// Check for tile
				if ( hex == null )
					continue;

				// Check if tile is occupied
				if ( !hex.Tile.IsOccupied )
					continue;

				// Check for unit
				if ( hex.Tile.CurrentUnit != null )
				{
					// Interupt unit
					hex.Tile.CurrentUnit.InteruptUnit ( );

					// Attack unit
					hex.Tile.CurrentUnit.GetAttacked ( );
				}

				// Check for object
				if ( hex.Tile.CurrentObject != null && hex.Tile.CurrentObject.CanBeAttacked )
				{
					// Attack the object
					hex.Tile.CurrentObject.GetAttacked ( );
				}
			}
		}

		#endregion // Protected Virtual Functions

		#region Protected Functions

		/// <summary>
		/// Updates the unit's data to a new set of unit data.
		/// Ability data is not affected.
		/// </summary>
		/// <param name="data"> The new unit data. </param>
		protected void UpdateUnitData ( IReadOnlyUnitData data )
		{
			// Update unit data
			InstanceData.ID = data.ID;
			InstanceData.UnitName = data.UnitName;
			InstanceData.UnitNickname = data.UnitNickname;
			InstanceData.UnitBio = data.UnitBio;
			InstanceData.FinishingMove = data.FinishingMove;
			InstanceData.Role = data.Role;
			InstanceData.Slots = data.Slots;
			InstanceData.Portrait = data.Portrait;
			InstanceData.IsEnabled = data.IsEnabled;

			// Update Portrait
			sprite.sprite = data.Portrait;
			displaySprite = data.Portrait;
			//GM.UI.matchInfoMenu.GetPlayerHUD ( this ).UpdatePortrait ( InstanceID, data.Portrait );
		}

		/// <summary>
		/// Adds a potential move to the unit's list of moves.
		/// </summary>
		/// <param name="move"> The new available move. </param>
		protected void AddMove ( MoveData move )
		{
			// Check for needed evaluation
			if ( Owner.IsBot )
				EvaluateMove ( move );

			// Add move to list
			MoveList.Add ( move );
		}


		/// <summary>
		/// Returns the two directions that are considered backwards movement for the unit.
		/// </summary>
		/// <param name="direction"> The unit's movement direction. </param>
		/// <returns> The pair of integers that represent the direction of the two tiles to be considered behind the unit. </returns>
		//protected IntPair GetBackDirection ( Player.Direction direction )
		//{
		//	// Store which tiles are to be ignored
		//	IntPair back = new IntPair ( 0, 1 );

		//	// Check the team's movement direction
		//	switch ( direction )
		//	{
		//	// Left to right movement
		//	case Player.Direction.LEFT_TO_RIGHT:
		//		back = new IntPair ( (int)MoveData.Direction.SOUTHWEST, (int)MoveData.Direction.NORTHWEST );
		//		break;

		//	// Right to left movement
		//	case Player.Direction.RIGHT_TO_LEFT:
		//		back = new IntPair ( (int)MoveData.Direction.NORTHEAST, (int)MoveData.Direction.SOUTHEAST );
		//		break;

		//	// Top left to bottom right movement
		//	case Player.Direction.TOP_LEFT_TO_BOTTOM_RIGHT:
		//		back = new IntPair ( (int)MoveData.Direction.NORTH, (int)MoveData.Direction.NORTHWEST );
		//		break;

		//	// Top right to bottom left movement
		//	case Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT:
		//		back = new IntPair ( (int)MoveData.Direction.NORTH, (int)MoveData.Direction.NORTHEAST );
		//		break;

		//	// Bottom left to top right movement
		//	case Player.Direction.BOTTOM_LEFT_TO_TOP_RIGHT:
		//		back = new IntPair ( (int)MoveData.Direction.SOUTH, (int)MoveData.Direction.SOUTHWEST );
		//		break;

		//	// Bottom right to top left movement
		//	case Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT:
		//		back = new IntPair ( (int)MoveData.Direction.SOUTHEAST, (int)MoveData.Direction.SOUTH );
		//		break;
		//	}

		//	// Return back tile elements
		//	return back;
		//}

		/// <summary>
		/// Determines if a tile matches any of the tiles in the prior moves.
		/// Returns true if a match is found.
		/// </summary>
		/// <param name="hex"> The tile being checked. </param>
		/// <param name="prerequisite"> The Move Data for any moves required for the unit to reach the given tile. </param>
		/// <returns> Whether or not this tile arleady exists in a path of moves. </returns>
		protected bool PriorMoveCheck ( Hex hex, MoveData move )
		{
			// Check for starting tile
			if ( hex == CurrentHex )
				return false;

			// Check for prerequisite move
			if ( move != null )
			{
				// Check if the tile matches
				if ( hex == move.Destination )
				{
					// Return that a match has been found
					return true;
				}
				else
				{
					// Check prerequisite move's tile
					return PriorMoveCheck ( hex, move.PriorMove );
				}
			}

			// Return that no matches were found
			return false;
		}

		/// <summary>
		/// Sets all of the unit and tile data for moving a unit to a tile.
		/// </summary>
		/// <param name="hex"> The tile this unit is moving to. </param>
		protected void SetUnitToTile ( Hex hex )
		{
			// Remove unit from previous tile
			CurrentHex.Tile.CurrentUnit = null;

			// Set the unit's new current tile
			CurrentHex = hex;
			hex.Tile.CurrentUnit = this;
		}

		#endregion // Protected Functions

		#region Public AI Functions

		/// <summary>
		/// Gets the value of attacking this unit.
		/// </summary>
		/// <param name="move"> The move data for this attack. </param>
		/// <returns> The value of the attack. </returns>
		public virtual int GetAttackValue ( MoveData move )
		{
			// Get attack value
			int value = MoveData.ATTACK_VALUE;

			// Get ko value
			if ( this is Pawn )
				value += MoveData.KO_PAWN_VALUE;
			else if ( this is HeroUnit )
				value += MoveData.KO_HERO_VALUE;
			else if ( this is Leader )
				value += MoveData.KO_LEADER_VALUE;

			// Return value
			return value;
		}

		/// <summary>
		/// Gets the additional value of being assisted by this unit.
		/// </summary>
		/// <param name="move"> The move data for the assist. </param>
		/// <returns> The value of the assist. </returns>
		public virtual int GetAssistValue ( MoveData move )
		{
			// Return no additional value
			return 0;
		}

		public virtual int GetPositionValue ( MoveData move )
		{
			return 0;
		}

		#endregion // Public AI Functions

		#region Protected AI Functions

		/// <summary>
		/// Evaluates the value of a move for the AI.
		/// </summary>
		/// <param name="move"> The move data being evaluated. </param>
		protected void EvaluateMove ( MoveData move )
		{
			// Get the base value
			move.BaseValue = move.PriorMove != null ? move.PriorMove.BaseValue : 0;

			// Get attack values
			move.BaseValue += GetAttackValues ( move );

			// Get assist values
			move.BaseValue += GetAssistValues ( move );

			// Get the final value
			move.FinalValue = move.BaseValue;

			// Get the position value
			move.FinalValue += GetPositionValues ( move );
		}

		/// <summary>
		/// Gets the values of the attacks in this move.
		/// </summary>
		/// <param name="move"> The move data for the attacks. </param>
		/// <returns> The value of the attacks. </returns>
		protected virtual int GetAttackValues ( MoveData move )
		{
			// Get attack value
			int value = 0;

			// Get value of each attack
			for ( int i = 0; i < move.AttackTargets.Length; i++ )
			{
				// Check for unit
				if ( move.AttackTargets [ i ].Tile.CurrentUnit != null )
					value += move.AttackTargets [ i ].Tile.CurrentUnit.GetAttackValue ( move );
				else if ( move.AttackTargets [ i ].Tile.CurrentObject != null )
					value += MoveData.ATTACK_VALUE;
			}

			// Return value
			return value;
		}

		/// <summary>
		/// Gets the values of the assists in this move.
		/// </summary>
		/// <param name="move"> The move data for the assists. </param>
		/// <returns> The value of the assists. </returns>
		protected virtual int GetAssistValues ( MoveData move )
		{
			// Get assist value
			int value = 0;

			// Get value of each assist
			for ( int i = 0; i < move.AssistTargets.Length; i++ )
			{
				// Add assist value
				value += MoveData.ASSIST_VALUE;

				// Check for unit
				if ( move.AssistTargets [ i ].Tile.CurrentUnit != null )
					value += move.AssistTargets [ i ].Tile.CurrentUnit.GetAssistValue ( move );
				//else if ( move.AssistTargets [ i ].Tile.CurrentObject != null )
				//	value += MoveData.ATTACK_VALUE;
			}

			// Return value
			return value;
		}

		/// <summary>
		/// Gets the values of surrounding positions in this move.
		/// </summary>
		/// <param name="move"> The move data for this move. </param>
		/// <returns> The value of the position. </returns>
		protected virtual int GetPositionValues ( MoveData move )
		{
			// Get position value
			int value = 0;

			// Check adjacent tiles
			for ( int i = 0; i < move.Destination.Neighbors.Length; i++ )
			{
				// Add position value for adjacent tiles
				value += GetPositionValue ( move, move.Destination.Neighbors [ i ], Hex.GetOppositeDirection ( (Hex.Direction)i ) );

				// Add position value for nearby tiles
				value += GetPositionValue ( move, move.Destination.Neighbor ( (Hex.Direction)i, 2 ), move.Destination.Neighbors [ i ], Hex.GetOppositeDirection ( (Hex.Direction)i ) );

				// Add position value for diagonal tiles
				value += GetPositionValue ( move, move.Destination.Diagonals [ i ], move.Destination.Neighbors [ i ], Hex.GetOppositeDirection ( (Hex.Direction)i ) );
				value += GetPositionValue ( move, move.Destination.Diagonals [ i ], move.Destination.Neighbors [ i + 1 < Hex.TOTAL_SIDES ? i + 1 : 0 ], Hex.GetOppositeDirection ( (Hex.Direction)( i + 1 < Hex.TOTAL_SIDES ? i + 1 : 0 ) ) );
			}

			// Return value
			return value;
		}

		/// <summary>
		/// Gets the value of a surrounding position in this move.
		/// </summary>
		/// <param name="move"> The move data for this move. </param>
		/// <param name="hex"> The tile being evaluated. </param>
		/// <param name="direction"> The direction from the evaluated tile to the destination tile. </param>
		/// <returns> The value of the surrounding position. </returns>
		protected virtual int GetPositionValue ( MoveData move, Hex hex, Hex.Direction direction )
		{
			// Check for tile
			if ( hex == null )
			{
				// Add protection bonus
				return MoveData.ADJACENT_VALUE;
			}

			// Check for ally
			if ( hex.Tile.CurrentUnit != null && hex.Tile.CurrentUnit.Owner == Owner )
			{
				// Add protection bonus
				return MoveData.ADJACENT_VALUE;
			}

			// Check for opponent
			if ( hex.Tile.CurrentUnit != null && hex.Tile.CurrentUnit.Owner != Owner && !IsAttacked ( move, hex ) )
			{
				// Add threat penalty and check for attack potential
				return -1 * ( MoveData.ADJACENT_VALUE + ( IsDirectionBlocked ( move.Destination, direction ) ? 0 : GetAttackValue ( move ) ) );
			}

			// Return no bonus or penalty
			return 0;
		}

		/// <summary>
		/// Gets the value of a surrounding position in this move.
		/// </summary>
		/// <param name="move"> The move data for this move. </param>
		/// <param name="hex"> The tile being evaluated. </param>
		/// <param name="midpoint"> The tile between the evaluated tile and the destination tile. </param>
		/// <param name="direction"> The direction from the evaluated tile to the destination tile. </param>
		/// <returns> The value of the surrounding position. </returns>
		protected virtual int GetPositionValue ( MoveData move, Hex hex, Hex midpoint, Hex.Direction direction )
		{
			// Check for tiles or if the path is blocked
			if ( hex == null || midpoint == null || midpoint.Tile.IsOccupied )
			{
				// Return no bonus or penalty
				return 0;
			}

			// Check for opponent
			if ( hex.Tile.CurrentUnit != null && hex.Tile.CurrentUnit.Owner != Owner && !IsAttacked ( move, hex ) )
			{
				// Add threat penalty and check for attack potential
				return -1 * ( MoveData.NEARBY_VALUE + ( IsDirectionBlocked ( move.Destination, direction ) ? 0 : GetAttackValue ( move ) ) );
			}

			// Return no bonus or penalty
			return 0;
		}

		/// <summary>
		/// Checks whether or not this tile was attacked at any point in this move.
		/// </summary>
		/// <param name="move"> The move data for this move. </param>
		/// <param name="hex"> The tile being checked. </param>
		/// <returns> Whether or not the tile was attacked. </returns>
		protected bool IsAttacked ( MoveData move, Hex hex )
		{
			// Check if this position was attacked
			for ( int i = 0; i < move.AttackTargets.Length; i++ )
				if ( move.AttackTargets [ i ] == hex )
					return true;

			// Check for previous moves
			if ( move.PriorMove != null )
				return IsAttacked ( move.PriorMove, hex );

			// Return that this position was not attacked
			return false;
		}

		/// <summary>
		/// Checks whether or not a tile is protected from an attack from a specific direction.
		/// </summary>
		/// <param name="hex"> The tile being checked. </param>
		/// <param name="direction"> The direction of a potential attack. </param>
		/// <returns> Whether or not a potential attack would be blocked. </returns>
		protected bool IsDirectionBlocked ( Hex hex, Hex.Direction direction )
		{
			// Return whether or not the direction is blocked
			return hex.Neighbor ( direction ) == null || hex.Neighbor ( direction ).Tile.IsOccupied;
		}

		#endregion // Protected AI Functions
	}
}