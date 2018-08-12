using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
	#region Grid Data

	public Hex [ ] Grid;

	private Dictionary<Hex.AxialCoord, Hex> gridDictionary = new Dictionary<Hex.AxialCoord, Hex> ( );

	#endregion // Grid Data

	#region MonoBehaviour Functions

	private void Start ( )
	{
		// Store each hex in the grid for easy access
		for ( int i = 0; i < Grid.Length; i++ )
			gridDictionary.Add ( Grid [ i ].Axial, Grid [ i ] );
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
		return gridDictionary [ hex ];
	}

	/// <summary>
	/// Get a specified hex by its cube coordinate.
	/// </summary>
	/// <param name="hex"> The cube coordinate of the hex. </param>
	/// <returns> The specified hex. </returns>
	public Hex GetHex ( Hex.CubeCoord hex )
	{
		// Return the hex by its axial coordinate.
		return gridDictionary [ hex.ToAxial ( ) ];
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

		// Check each hex
		for ( int col = center.Axial.Col - radius; col <= center.Axial.Col + radius; col++ )
		{
			for ( int row = center.Axial.Row - radius; row <= center.Axial.Row + radius; row++ )
			{
				// Get coordinates
				Hex.AxialCoord coord = new Hex.AxialCoord ( col, row );

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

		// Check each hex
		for ( int col = center.Axial.Col - radius; col <= center.Axial.Col + radius; col++ )
		{
			for ( int row = center.Axial.Row - radius; row <= center.Axial.Row + radius; row++ )
			{
				// Get coordinates
				Hex.AxialCoord coord = new Hex.AxialCoord ( col, row );

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

		// Return all hexes within range
		return range;
	}

	/// <summary>
	/// Get the opposite direction of a specified direction.
	/// </summary>
	/// <param name="direction"> The direction to get the opposite from. </param>
	/// <returns> The opposite direction. </returns>
	public Hex.Direction GetOppositeDirection ( Hex.Direction direction )
	{
		// Return the opposite direction
		return (Hex.Direction)( ( (int)direction + 3 ) % 6 );
	}

	/// <summary>
	/// Returns the two directions that are considered backwards movement for a player's direction.
	/// </summary>
	/// <param name="direction"> The movement direction of a player. </param>
	/// <returns> The pair of integers that represent the direction of the two hexes to be considered behind the unit. </returns>
	public IntPair GetBackDirection ( Player.Direction direction )
	{
		// Store which tiles are to be ignored
		IntPair back = new IntPair ( 0, 1 );

		// Check the team's movement direction
		switch ( direction )
		{
		// Left to right movement
		case Player.Direction.LEFT_TO_RIGHT:
			back = new IntPair ( (int)Hex.Direction.SOUTHWEST, (int)Hex.Direction.NORTHWEST );
			break;

		// Right to left movement
		case Player.Direction.RIGHT_TO_LEFT:
			back = new IntPair ( (int)Hex.Direction.NORTHEAST, (int)Hex.Direction.SOUTHEAST );
			break;

		// Top left to bottom right movement
		case Player.Direction.TOP_LEFT_TO_BOTTOM_RIGHT:
			back = new IntPair ( (int)Hex.Direction.NORTH, (int)Hex.Direction.NORTHWEST );
			break;

		// Top right to bottom left movement
		case Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT:
			back = new IntPair ( (int)Hex.Direction.NORTH, (int)Hex.Direction.NORTHEAST );
			break;

		// Bottom left to top right movement
		case Player.Direction.BOTTOM_LEFT_TO_TOP_RIGHT:
			back = new IntPair ( (int)Hex.Direction.SOUTH, (int)Hex.Direction.SOUTHWEST );
			break;

		// Bottom right to top left movement
		case Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT:
			back = new IntPair ( (int)Hex.Direction.SOUTHEAST, (int)Hex.Direction.SOUTH );
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

	#endregion // Public Functions
}
