using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectOcelot.Match.Arena
{
	public class HexGrid : MonoBehaviour
	{
		#region Grid Data

		[SerializeField]
		private GameManager GM;

		public Hex [ ] Grid;

		private Dictionary<Hex.AxialCoord, Hex> gridDictionary = new Dictionary<Hex.AxialCoord, Hex> ( );

		#endregion // Grid Data

		#region MonoBehaviour Functions

		private void Start ( )
		{
			// Get all hexes
			Grid = GetComponentsInChildren<Hex> ( );

			// Store each hex in the grid for easy access
			for ( int i = 0; i < Grid.Length; i++ )
			{
				// Hide border
				Grid [ i ].Tile.SetBorderActive ( false );

				// Add hex to grid
				gridDictionary.Add ( Grid [ i ].Axial, Grid [ i ] );
			}

		}

		#endregion // MonoBehaviour Functions

		#region Public Functions

		/// <summary>
		/// Get a specified hex by its axial coordinate.
		/// </summary>
		/// <param name="hex"> The axial coordinate of the hex. </param>
		/// <returns> The specified hex. </returns>
		public Hex GetHex ( Hex.AxialCoord hex )
		{
			// Return the hex by its axial coordinate.
			return gridDictionary.ContainsKey ( hex ) ? gridDictionary [ hex ] : null;
		}

		/// <summary>
		/// Get a specified hex by its cube coordinate.
		/// </summary>
		/// <param name="hex"> The cube coordinate of the hex. </param>
		/// <returns> The specified hex. </returns>
		public Hex GetHex ( Hex.CubeCoord hex )
		{
			// Return the hex by its axial coordinate.
			return gridDictionary.ContainsKey ( hex.ToAxial ( ) ) ? gridDictionary [ hex.ToAxial ( ) ] : null;
		}

		/// <summary>
		/// Gets the distance between two hexes.
		/// Adjacent hexes have a distance of 1.
		/// </summary>
		/// <param name="a"> The starting hex. </param>
		/// <param name="b"> The destination hex. </param>
		/// <returns> The number of hexes between the two hexes. </returns>
		public int GetDistance ( Hex a, Hex b )
		{
			// Calculate and return the distance
			return (int)( ( Mathf.Abs ( a.Cube.X - b.Cube.X ) + Mathf.Abs ( a.Cube.Y - b.Cube.Y ) + Mathf.Abs ( a.Cube.Z - b.Cube.Z ) ) / 2 );
		}

		/// <summary>
		/// Gets all hexes within a radius of a specified hex.
		/// </summary>
		/// <param name="center"> The center hex of the radius. </param>
		/// <param name="radius"> The distance of the radius. </param>
		/// <returns> The list of hexes within range. </returns>
		public List<Hex> GetRange ( Hex center, int radius )
		{
			// Store the hexes within range
			List<Hex> range = new List<Hex> ( );

			// Check each cube axis
			for ( int x = center.Cube.X - radius; x <= center.Cube.X + radius; x++ )
			{
				for ( int y = center.Cube.Y - radius; y <= center.Cube.Y + radius; y++ )
				{
					for ( int z = center.Cube.Z - radius; z <= center.Cube.Z + radius; z++ )
					{
						// Check for valid coordinateds
						if ( !Hex.CubeCoord.Validate ( x, y, z ) )
							continue;

						// Get axial coordinate
						Hex.AxialCoord coord = new Hex.CubeCoord ( x, y, z ).ToAxial ( );

						// Check for center
						if ( center.Axial.Equals ( coord ) )
							continue;

						// Check if the hex exists
						if ( gridDictionary.ContainsKey ( coord ) )
						{
							// Add hex to range
							range.Add ( gridDictionary [ coord ] );
						}
					}
				}
			}

			// Return all hexes within range
			return range;
		}

		/// <summary>
		/// Gets all hexes within a radius of a specified hex with the back direction hexes excluded.
		/// </summary>
		/// <param name="center"> The center hex of the radius. </param>
		/// <param name="radius"> The distance of the radius. </param>
		/// <param name="direction"> The movement direction of the player. </param>
		/// <returns> The list of hexes within range. </returns>
		public List<Hex> GetRange ( Hex center, int radius, Player.Direction direction )
		{
			// Store the hexes within range
			List<Hex> range = new List<Hex> ( );

			// Check each cube axis
			for ( int x = center.Cube.X - radius; x <= center.Cube.X + radius; x++ )
			{
				for ( int y = center.Cube.Y - radius; y <= center.Cube.Y + radius; y++ )
				{
					for ( int z = center.Cube.Z - radius; z <= center.Cube.Z + radius; z++ )
					{
						// Check for valid coordinateds
						if ( !Hex.CubeCoord.Validate ( x, y, z ) )
							continue;

						// Get axial coordinate
						Hex.AxialCoord coord = new Hex.CubeCoord ( x, y, z ).ToAxial ( );

						// Check for north back direction
						if ( IsBackDirection ( center.Cube, coord.ToCube ( ), direction ) )
							continue;

						// Check for center
						if ( center.Axial.Equals ( coord ) )
							continue;

						// Check if the hex exists
						if ( gridDictionary.ContainsKey ( coord ) )
						{
							// Add hex to range
							range.Add ( gridDictionary [ coord ] );
						}
					}
				}
			}

			// Return all hexes within range
			return range;
		}

		/// <summary>
		/// Returns the two directions that are considered backwards movement for a player's direction.
		/// </summary>
		/// <param name="direction"> The movement direction of a player. </param>
		/// <returns> The pair of integers that represent the direction of the two hexes to be considered behind the unit. </returns>
		public Tools.IntPair GetBackDirection ( Player.Direction direction )
		{
			// Store which tiles are to be ignored
			Tools.IntPair back = new Tools.IntPair ( 0, 1 );

			// Check the team's movement direction
			switch ( direction )
			{
			// Left to right movement
			case Player.Direction.LEFT_TO_RIGHT:
				back = new Tools.IntPair ( (int)Hex.Direction.SOUTHWEST, (int)Hex.Direction.NORTHWEST );
				break;

			// Right to left movement
			case Player.Direction.RIGHT_TO_LEFT:
				back = new Tools.IntPair ( (int)Hex.Direction.NORTHEAST, (int)Hex.Direction.SOUTHEAST );
				break;

			// Top left to bottom right movement
			case Player.Direction.TOP_LEFT_TO_BOTTOM_RIGHT:
				back = new Tools.IntPair ( (int)Hex.Direction.NORTH, (int)Hex.Direction.NORTHWEST );
				break;

			// Top right to bottom left movement
			case Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT:
				back = new Tools.IntPair ( (int)Hex.Direction.NORTH, (int)Hex.Direction.NORTHEAST );
				break;

			// Bottom left to top right movement
			case Player.Direction.BOTTOM_LEFT_TO_TOP_RIGHT:
				back = new Tools.IntPair ( (int)Hex.Direction.SOUTH, (int)Hex.Direction.SOUTHWEST );
				break;

			// Bottom right to top left movement
			case Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT:
				back = new Tools.IntPair ( (int)Hex.Direction.SOUTHEAST, (int)Hex.Direction.SOUTH );
				break;
			}

			// Return back tile elements
			return back;
		}

		/// <summary>
		/// Checks if a hex is in a backwards direction for a player.
		/// </summary>
		/// <param name="center"> The cube coordinates of the hex being compared to. </param>
		/// <param name="check"> The cube coordinates of the hex being checked. </param>
		/// <param name="direction"> The movement direction of the player. </param>
		/// <returns> Whether or not the hex is in a back direction. </returns>
		public bool IsBackDirection ( Hex.CubeCoord center, Hex.CubeCoord check, Player.Direction direction )
		{
			// Check left to right movement
			if ( direction == Player.Direction.LEFT_TO_RIGHT && check.X < center.X )
				return true;

			// Check right to left movement
			if ( direction == Player.Direction.RIGHT_TO_LEFT && check.X > center.X )
				return true;

			// Check bottom right to top left movement
			if ( direction == Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT && check.Y < center.Y )
				return true;

			// Check top left to bottom right movement
			if ( direction == Player.Direction.TOP_LEFT_TO_BOTTOM_RIGHT && check.Y > center.Y )
				return true;

			// Check top right to bottom left movement
			if ( direction == Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT && check.Z < center.Z )
				return true;

			// Check bottom left to top right movement
			if ( direction == Player.Direction.BOTTOM_LEFT_TO_TOP_RIGHT && check.Z > center.Z )
				return true;

			// Return that hex is not in a back direction
			return false;
		}

		/// <summary>
		/// Reset all tiles to their default state.
		/// </summary>
		/// <param name="exceptSelectedUnit"> Whether or not the tile of the selected unit should be excluded from the reset. </param>
		public void ResetTiles ( bool exceptSelectedUnit = false )
		{
			// Reset all tiles to their default state
			for ( int i = 0; i < Grid.Length; i++ )
			{
				// Check for selected unit
				if ( exceptSelectedUnit && GM.SelectedUnit != null && GM.SelectedUnit.CurrentHex == Grid [ i ] )
					continue;

				// Reset tile
				Grid [ i ].Tile.SetTileState ( TileState.Default );
			}
		}

		/// <summary>
		/// Reset all tiles except for a number of specified tiles to their default state.
		/// </summary>
		/// <param name="exceptionTiles"> The tiles being excluded from the reset. </param>
		/// <param name="exceptSelectedUnit"> Whether or not the tile of the selected unit should be excluded from the reset. </param>
		public void ResetTiles ( params Tile [ ] exceptionTiles )
		{
			// Reset all tiles to their default state except for some
			for ( int i = 0; i < Grid.Length; i++ )
			{
				// Check for selected unit
				if ( GM.SelectedUnit != null && GM.SelectedUnit.CurrentHex == Grid [ i ] )
					continue;

				// Check for exception tile
				if ( exceptionTiles.Contains ( Grid [ i ].Tile ) )
					continue;

				// Reset tile
				Grid [ i ].Tile.SetTileState ( TileState.Default );
			}
		}

		/// <summary>
		/// Reset all tiles except for those of a specified state to their default state.
		/// </summary>
		/// <param name="exceptionState"> The state being excluded from the reset. </param>
		/// <param name="exceptSelectedUnit"> Whether or not the tile of the selected unit should be excluded from the reset. </param>
		public void ResetTiles ( params TileState [ ] exceptionStates )
		{
			// Reset all tiles to their default state except those in a certain state
			for ( int i = 0; i < Grid.Length; i++ )
			{
				// Check for selected unit
				if ( GM.SelectedUnit != null && GM.SelectedUnit.CurrentHex == Grid [ i ] )
					continue;

				// Check for exception state
				if ( exceptionStates.Contains ( Grid [ i ].Tile.State ) )
					continue;

				// Reset tile
				Grid [ i ].Tile.SetTileState ( TileState.Default );
			}
		}

		#endregion // Public Functions
	}
}