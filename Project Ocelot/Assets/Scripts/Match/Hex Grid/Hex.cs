using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Hex : MonoBehaviour
{
	#region Public Classes

	/// <summary>
	/// Hex grid coordinates to use for calculations.
	/// X + Y + Z = 0
	/// </summary>
	[System.Serializable]
	public struct CubeCoord
	{
		/// <summary>
		/// Axis runs West to East.
		/// + Northeast/Southeast.
		/// - Northwest/Southwest.
		/// </summary>
		public readonly int X;

		/// <summary>
		/// Axis runs Southeast to Northwest.
		/// + North/Northwest.
		/// - South/Southeast.
		/// </summary>
		public readonly int Y;

		/// <summary>
		/// Axis runs Northeast to Southwest.
		/// + South/Southwest.
		/// - North/Northeast.
		/// </summary>
		public readonly int Z;

		public CubeCoord ( int x, int y, int z )
		{
			X = x;
			Y = y;
			Z = z;

			if ( X + Y + Z != 0 )
				throw new System.Exception ( "X + Y + Z must be 0!" );
		}

		/// <summary>
		/// Gets the sum of this and another cube coordinate.
		/// </summary>
		/// <param name="cube"> The cube coordinate being added to this. </param>
		/// <returns> The sum of both cube coordinates. </returns>
		public CubeCoord Add ( CubeCoord cube )
		{
			// Add both coordinates
			return new CubeCoord ( X + cube.X, Y + cube.Y, Z + cube.Z );
		}

		/// <summary>
		/// Gets the difference of this and another cube coordinate.
		/// </summary>
		/// <param name="cube"> The cube coordinate being subtracted from this. </param>
		/// <returns> The difference of both cube coordinates. </returns>
		public CubeCoord Subtract ( CubeCoord cube )
		{
			// Subtract both coordinates
			return new CubeCoord ( X - cube.X, Y - cube.Y, Z - cube.Z );
		}

		/// <summary>
		/// Gets the scale of this and another cube coordinate.
		/// </summary>
		/// <param name="scale"> The cube coordinate being multiplied by this. </param>
		/// <returns> The scale of both cube coordinates. </returns>
		public CubeCoord Scale ( int scale )
		{
			// Multiply both coordinates
			return new CubeCoord ( X * scale, Y * scale, Z * scale );
		}

		/// <summary>
		/// Checks if two cube coordinates are equal to one another.
		/// </summary>
		/// <param name="cube"> The cube coordinates being compared to this. </param>
		/// <returns> Whether or not the cube coordinates are equal. </returns>
		public bool Equals ( CubeCoord cube )
		{
			// Check if the coordinates are equal to one another
			return X == cube.X && Y == cube.Y && Z == cube.Z;
		}

		/// <summary>
		/// Converts this cube coordinate to an axial coordinate.
		/// </summary>
		/// <returns> The axial coordinate equivalent to this. </returns>
		public AxialCoord ToAxial ( )
		{
			// Convert to axial coordinate
			return new AxialCoord ( X, Z );
		}
	}

	/// <summary>
	/// Hex grid coordinates to use for storage.
	/// </summary>
	[System.Serializable]
	public struct AxialCoord
	{
		/// <summary>
		/// Axis runs West to East.
		/// + Northeast/Southeast.
		/// - Northwest/Southwest.
		/// </summary>
		public readonly int Col;

		/// <summary>
		/// Axis runs Northeast to Southwest.
		/// + South/Southwest.
		/// - North/Northeast.
		/// </summary>
		public readonly int Row;

		public AxialCoord ( int col, int row )
		{
			Col = col;
			Row = row;
		}

		/// <summary>
		/// Checks if two axial coordinates are equal to one another.
		/// </summary>
		/// <param name="axial"> The axial coordinates being compared to this. </param>
		/// <returns> Whether or not the axial coordinates are equal. </returns>
		public bool Equals ( AxialCoord axial )
		{
			// Check if the coordinates are equal to one another
			return Col == axial.Col && Row == axial.Row;
		}

		/// <summary>
		/// Converts this axial coordinate to a cube coordinate.
		/// </summary>
		/// <returns> The cube coordinate equivalent to this. </returns>
		public CubeCoord ToCube ( )
		{
			// Convert to cube coordinate
			return new CubeCoord ( Col, -Col - Row, Row );
		}
	}

	public enum Direction
	{
		NORTH = 0,
		NORTHEAST = 1,
		SOUTHEAST = 2,
		SOUTH = 3,
		SOUTHWEST = 4,
		NORTHWEST = 5
	}

	#endregion // Public Classes

	#region Hex Data

	[SerializeField]
	private int hexCol;

	[SerializeField]
	private int hexRow;

	private AxialCoord coordinates;

	#if UNITY_EDITOR

	private AxialCoord currentCoordinates;

	private const float X_SHIFT = 1.3f;
	private const float Y_SHIFT = -1.5f;

	#endif

	public HexGrid Grid;
	public Tile Tile;

	private CubeCoord [ ] directions = new CubeCoord [ ]
	{
		new CubeCoord (  0,  1, -1 ), // North
		new CubeCoord (  1,  0, -1 ), // Northeast
		new CubeCoord (  1, -1,  0 ), // Southeast
		new CubeCoord (  0, -1,  1 ), // South
		new CubeCoord ( -1,  0,  1 ), // Southwest
		new CubeCoord ( -1,  1,  0 )  // Northwest
	};

	private CubeCoord [ ] diagonals = new CubeCoord [ ]
	{
		new CubeCoord (  1,  1, -2 ), // North by northeast
		new CubeCoord (  2, -1, -1 ), // East
		new CubeCoord (  1, -2,  1 ), // South by southeast
		new CubeCoord ( -1, -1,  2 ), // South by southwest
		new CubeCoord ( -2,  1,  1 ), // West
		new CubeCoord ( -1,  2, -1 )  // North by northwest
	};

	/// <summary>
	/// The axial coordinate of this hex.
	/// </summary>
	public AxialCoord Axial
	{
		get
		{
			// Return axial coordinate
			return coordinates;
		}
	}

	/// <summary>
	/// The cube coordinate of this hex.
	/// </summary>
	public CubeCoord Cube
	{
		get
		{
			// Return cube coordinate
			return coordinates.ToCube ( );
		}
	}

	/// <summary>
	/// All of the neighboring hexes to this hex.
	/// </summary>
	public Hex [ ] Neighbors
	{
		get
		{
			return new Hex [ ]
			{
				Neighbor ( Direction.NORTH ),
				Neighbor ( Direction.NORTHEAST ),
				Neighbor ( Direction.SOUTHEAST ),
				Neighbor ( Direction.SOUTH ),
				Neighbor ( Direction.SOUTHWEST ),
				Neighbor ( Direction.NORTHWEST )
			};
		}
	}

	/// <summary>
	/// All of the diagonal hexes to this hex.
	/// </summary>
	public Hex [ ] Diagonals
	{
		get
		{
			return new Hex [ ]
			{
				Diagonal ( Direction.NORTH, Direction.NORTHEAST ),
				Diagonal ( Direction.NORTHEAST, Direction.SOUTHEAST ),
				Diagonal ( Direction.SOUTHEAST, Direction.SOUTH),
				Diagonal ( Direction.SOUTH, Direction.SOUTHWEST ),
				Diagonal ( Direction.SOUTHWEST, Direction.NORTHWEST ),
				Diagonal ( Direction.NORTHWEST, Direction.NORTH )
			};
		}
	}

	#endregion // Hex Data

	#region MonoBehaviour Functions

	private void Start ( )
	{
		// Set coordinates
		coordinates = new AxialCoord ( hexCol, hexRow );

		#if UNITY_EDITOR

		// Set coordinates
		currentCoordinates = new AxialCoord ( hexCol, hexRow );

		#endif
	}

	private void Update ( )
	{
		#if UNITY_EDITOR

		// Check for update to coordinates
		if ( currentCoordinates.Col != hexCol || currentCoordinates.Row != hexRow )
		{
			// Store new coordinates
			currentCoordinates = new AxialCoord ( hexCol, hexRow );

			// Reposition hex
			transform.localPosition = new Vector3 ( currentCoordinates.Col * X_SHIFT, currentCoordinates.Row * Y_SHIFT + ( currentCoordinates.Col % 2 == 0 ? 0 : Y_SHIFT / 2 ), 0 );
		}

		#endif 
	}

	#endregion // MonoBehaviour Functions

	#region Public Functions

	/// <summary>
	/// Gets the neighboring hex in a specified direction.
	/// </summary>
	/// <param name="direction"> The direction from this hex to the neighbor. </param>
	/// <returns> Returns the neighboring hex. </returns>
	public Hex Neighbor ( Direction direction )
	{
		// Calculate and return the neighbor in the specified direction from this hex
		return Grid.GetHex ( Cube.Add ( directions [ (int)direction ] ).ToAxial ( ) );
	}

	/// <summary>
	/// Gets the neighboring hex in a specified direction and distance.
	/// </summary>
	/// <param name="direction"> The direction from this hex to the neighbor. </param>
	/// <returns> Returns the neighboring hex. </returns>
	public Hex Neighbor ( Direction direction, int distance )
	{
		// Calculate and return the neighbor in the specified direction and distance from this hex
		return Grid.GetHex ( Cube.Add ( directions [ (int)direction ].Scale ( distance ) ).ToAxial ( ) );
	}

	/// <summary>
	/// Gets the nearest diagonal hex in between two specifed directions.
	/// </summary>
	/// <param name="a"> One of the directions the diagonal is in between. </param>
	/// <param name="b"> The other direction the diagonal is in between. </param>
	/// <returns> Returns the diagonal hex. </returns>
	public Hex Diagonal ( Direction a, Direction b )
	{
		// Check for the north by northeast diagonal
		if ( ( a == Direction.NORTH && b == Direction.NORTHEAST ) || ( a == Direction.NORTHEAST && b == Direction.NORTH ) )
		{
			// Calculate and return the north by northeast diagonal
			return Grid.GetHex ( Cube.Add ( diagonals [ 0 ] ).ToAxial ( ) );
		}

		// Check for the east diagonal
		if ( ( a == Direction.NORTHEAST && b == Direction.SOUTHEAST ) || ( a == Direction.SOUTHEAST && b == Direction.NORTHEAST ) )
		{
			// Calculate and return the east diagonal
			return Grid.GetHex ( Cube.Add ( diagonals [ 1 ] ).ToAxial ( ) );
		}

		// Check for the south by southeast diagonal
		if ( ( a == Direction.SOUTHEAST && b == Direction.SOUTH ) || ( a == Direction.SOUTH && b == Direction.SOUTHEAST ) )
		{
			// Calculate and return the south by southeast diagonal
			return Grid.GetHex ( Cube.Add ( diagonals [ 2 ] ).ToAxial ( ) );
		}

		// Check for the south by southwest diagonal
		if ( ( a == Direction.SOUTH && b == Direction.SOUTHWEST ) || ( a == Direction.SOUTHWEST && b == Direction.SOUTH ) )
		{
			// Calculate and return the north by northeast diagonal
			return Grid.GetHex ( Cube.Add ( diagonals [ 3 ] ).ToAxial ( ) );
		}

		// Check for the west diagonal
		if ( ( a == Direction.SOUTHWEST && b == Direction.NORTHWEST ) || ( a == Direction.NORTHWEST && b == Direction.SOUTHWEST ) )
		{
			// Calculate and return the west diagonal
			return Grid.GetHex ( Cube.Add ( diagonals [ 4 ] ).ToAxial ( ) );
		}

		// Check for the north by northwest diagonal
		if ( ( a == Direction.NORTHWEST && b == Direction.NORTH ) || ( a == Direction.NORTH && b == Direction.NORTHWEST ) )
		{
			// Calculate and return the north by northwest diagonal
			return Grid.GetHex ( Cube.Add ( diagonals [ 5 ] ).ToAxial ( ) );
		}

		// Return that no diagonal was found
		return null;
	}

	/// <summary>
	/// Gets the distance between this and another hex.
	/// </summary>
	/// <param name="destination"> The destination hex being measured to. </param>
	/// <returns> The number of hexes between this and the destination. </returns>
	public int Distance ( Hex destination )
	{
		// Get the distance between this and the destination
		return Grid.GetDistance ( this, destination );
	}

	/// <summary>
	/// Gets the hexes within range of this hex.
	/// </summary>
	/// <param name="radius"> The radius of the range. </param>
	/// <returns> The list of hexes within range. </returns>
	public List<Hex> Range ( int radius )
	{
		// Get the hexes within range of this hex
		return Grid.GetRange ( this, radius );
	}

	/// <summary>
	/// Gets the hexes within range of this hex without any back hexes.
	/// </summary>
	/// <param name="radius"> The radius of the range. </param>
	/// <param name="direction"> The movement direction of the player. </param>
	/// <returns> The list of hexes within range without back hexes. </returns>
	public List<Hex> Range ( int radius, Player.Direction direction )
	{
		// Get the hexes within range of this hex
		return Grid.GetRange ( this, radius, direction );
	}

	#endregion // Public Functions

	#region Private Functions

	

	

	#endregion // Private Functions
}
