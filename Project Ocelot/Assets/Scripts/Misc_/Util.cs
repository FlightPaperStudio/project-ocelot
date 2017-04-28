using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util 
{
	/// <summary>
	/// Returns the appropriate color value for the given team color.
	/// </summary>
	public static Color32 TeamColor ( Player.TeamColor team, int factor = 0 )
	{
		// Get team color
		Color32 col = Color.white;
		switch ( team )
		{
		case Player.TeamColor.Blue:
			col = new Color32 (   0,   0, 255, 255 );
			break;
		case Player.TeamColor.Green:
			col = new Color32 (   0, 150,   0, 255 );
			break;
		case Player.TeamColor.Yellow:
			col = new Color32 ( 255, 255,   0, 255 );
			break;
		case Player.TeamColor.Orange:
			col = new Color32 ( 255, 125,   0, 255 );
			break;
		case Player.TeamColor.Pink:
			col = new Color32 ( 255, 100, 150, 255 );
			break;
		case Player.TeamColor.Purple:
			col = new Color32 ( 150,   0, 255, 255 );
			break;
		}

		// Return color with darkness factor
		Color c = col;
		c = new Color ( c.r - ( c.r * ( 0.1f * factor ) ), c.g - ( c.g * ( 0.1f * factor ) ), c.b - ( c.b * ( 0.1f * factor ) ) );
		return c;
	}

	/// <summary>
	/// Returns the opposite direction for a tile neighbor.
	/// </summary>
	public static int GetOppositeDirection ( int direction )
	{
		// Increment index
		direction += 3;

		// Check for overlap
		if ( direction >= 6 )
			direction -= 6;

		// Return opposite direction
		return direction;
	}

	/// <summary>
	/// Returns the opposite direction for a tile neighbor.
	/// </summary>
	public static MoveData.MoveDirection GetOppositeDirection ( MoveData.MoveDirection direction )
	{
		// Return opposite direction
		return (MoveData.MoveDirection)GetOppositeDirection ( (int)direction );
	}

	/// <summary>
	/// Gets the tile distance of a neighbor tile's position.
	/// Returns a Vector3 that should be added to current tile's position to give you the neighbor's tile position.
	/// </summary>
	public static Vector3 GetTileDistance ( int direction )
	{
		// Return distance
		return GetTileDistance ( (MoveData.MoveDirection)direction );
	}

	/// <summary>
	/// Gets the tile distance of a neighbor tile's position.
	/// Returns a Vector3 that should be added to current tile's position to give you the neighbor's tile position.
	/// </summary>
	public static Vector3 GetTileDistance ( MoveData.MoveDirection direction )
	{
		// Check direction
		switch ( direction )
		{
		// Above
		case MoveData.MoveDirection.Above:
			return new Vector3 (  0.0f,  1.5f, 0f );

		// Right above
		case MoveData.MoveDirection.RightAbove:
			return new Vector3 (  1.3f,  0.75f, 0f );

		// Right below
		case MoveData.MoveDirection.RightBelow:
			return new Vector3 (  1.3f, -0.75f, 0f );

		// Below
		case MoveData.MoveDirection.Below:
			return new Vector3 (  0.0f, -1.5f, 0f );

		// Left Below
		case MoveData.MoveDirection.LeftBelow:
			return new Vector3 ( -1.3f, -0.75f, 0f );

		// Left above
		case MoveData.MoveDirection.LeftAbove:
			return new Vector3 ( -1.3f,  0.75f, 0f );
		}

		// Return error
		return Vector3.zero;
	}
}

public class IntPair
{
	public int FirstInt;
	public int SecondInt;

	public IntPair ( int first, int second )
	{
		FirstInt = first;
		SecondInt = second;
	}
}