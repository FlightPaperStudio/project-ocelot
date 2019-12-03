using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Tools
{
	public class Util
	{
		/// <summary>
		/// Returns the appropriate color value for the given team color.
		/// </summary>
		public static Color32 TeamColor ( Match.Player.TeamColor team, int factor = 0 )
		{
			// Get team color
			Color32 col = Color.white;
			switch ( team )
			{
			case Match.Player.TeamColor.BLUE:
				col = new Color32 ( 0, 174, 239, 255 ); // Old color - 0, 0, 255
				break;
			case Match.Player.TeamColor.GREEN:
				col = new Color32 ( 78, 219, 49, 255 ); // Old color - 0, 150, 0
				break;
			case Match.Player.TeamColor.YELLOW:
				col = new Color32 ( 255, 222, 23, 255 ); // Old color - 255, 255, 0
				break;
			case Match.Player.TeamColor.ORANGE:
				col = new Color32 ( 247, 148, 29, 255 ); // Old color - 255, 125, 0
				break;
			case Match.Player.TeamColor.PINK:
				col = new Color32 ( 239, 58, 144, 255 ); // Old color - 255, 100, 150
				break;
			case Match.Player.TeamColor.PURPLE:
				col = new Color32 ( 182, 53, 196, 255 ); // Old color - 150, 0, 255
				break;
			}

			// Return color with darkness factor
			Color c = col;
			c = new Color ( c.r - ( c.r * ( 0.1f * factor ) ), c.g - ( c.g * ( 0.1f * factor ) ), c.b - ( c.b * ( 0.1f * factor ) ) );
			return c;
		}

		/// <summary>
		/// Returns the appropriate accent color value for the given team.
		/// </summary>
		/// <param name="team"> The team associated with the accent color. </param>
		/// <returns> The accent color for the team. </returns>
		public static Color32 AccentColor ( Match.Player.TeamColor team, byte alpha = 255 )
		{
			// Get team color
			Color32 col = Color.white;
			switch ( team )
			{
			case Match.Player.TeamColor.BLUE:
				col = new Color32 ( 0, 101, 131, alpha );
				break;
			case Match.Player.TeamColor.GREEN:
				col = new Color32 ( 0, 127, 63, alpha );
				break;
			case Match.Player.TeamColor.YELLOW:
				col = new Color32 ( 255, 244, 164, alpha );
				break;
			case Match.Player.TeamColor.ORANGE:
				col = new Color32 ( 239, 66, 54, alpha );
				break;
			case Match.Player.TeamColor.PINK:
				col = new Color32 ( 158, 32, 99, alpha );
				break;
			case Match.Player.TeamColor.PURPLE:
				col = new Color32 ( 102, 45, 145, alpha );
				break;
			}

			// Return the accent color
			return col;
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
		public static Vector3 GetTileTransformDistance ( int direction )
		{
			// Return distance
			return GetTileTransformDistance ( (MoveData.MoveDirection)direction );
		}

		/// <summary>
		/// Gets the tile distance of a neighbor tile's position.
		/// Returns a Vector3 that should be added to current tile's position to give you the neighbor's tile position.
		/// </summary>
		public static Vector3 GetTileTransformDistance ( MoveData.MoveDirection direction )
		{
			// Check direction
			switch ( direction )
			{
			// Above
			case MoveData.MoveDirection.NORTH:
				return new Vector3 ( 0.0f, 1.5f, 0f );

			// Right above
			case MoveData.MoveDirection.NORTHEAST:
				return new Vector3 ( 1.3f, 0.75f, 0f );

			// Right below
			case MoveData.MoveDirection.SOUTHEAST:
				return new Vector3 ( 1.3f, -0.75f, 0f );

			// Below
			case MoveData.MoveDirection.SOUTH:
				return new Vector3 ( 0.0f, -1.5f, 0f );

			// Left Below
			case MoveData.MoveDirection.SOUTHWEST:
				return new Vector3 ( -1.3f, -0.75f, 0f );

			// Left above
			case MoveData.MoveDirection.NORTHWEST:
				return new Vector3 ( -1.3f, 0.75f, 0f );
			}

			// Return error
			return Vector3.zero;
		}

		/// <summary>
		/// Orients a sprite to be inverted or not inverted to face the appropriate direction.
		/// </summary>
		public static void OrientSpriteToDirection ( SpriteRenderer sprite, Match.Player.Direction direction )
		{
			// Check the direction
			if ( direction == Match.Player.Direction.RIGHT_TO_LEFT || direction == Match.Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT || direction == Match.Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT )
				sprite.flipX = true;
			else
				sprite.flipX = false;
		}

		/// <summary>
		/// Converts the direction from MoveData to Hex.
		/// </summary>
		/// <param name="direction"> The MoveData direction. </param>
		/// <returns> The Hex Direction. </returns>
		public static Hex.Direction MoveDirectionToHexDirection ( MoveData.MoveDirection direction )
		{
			// Check direction
			switch ( direction )
			{
			case MoveData.MoveDirection.NORTH:
				return Hex.Direction.NORTH;
			case MoveData.MoveDirection.NORTHEAST:
				return Hex.Direction.NORTHEAST;
			case MoveData.MoveDirection.SOUTHEAST:
				return Hex.Direction.SOUTHEAST;
			case MoveData.MoveDirection.SOUTH:
				return Hex.Direction.SOUTH;
			case MoveData.MoveDirection.SOUTHWEST:
				return Hex.Direction.SOUTHWEST;
			case MoveData.MoveDirection.NORTHWEST:
				return Hex.Direction.NORTHWEST;
			}

			// Return north by default
			return Hex.Direction.NORTH;
		}

		/// <summary>
		/// Converts the direction from Hex to MoveData.
		/// </summary>
		/// <param name="direction"> The Hex direction. </param>
		/// <returns> The MoveData direction. </returns>
		public static MoveData.MoveDirection HexDirectionToMoveDirection ( Hex.Direction direction )
		{
			// Check direction
			switch ( direction )
			{
			case Hex.Direction.NORTH:
				return MoveData.MoveDirection.NORTH;
			case Hex.Direction.NORTHEAST:
				return MoveData.MoveDirection.NORTHEAST;
			case Hex.Direction.SOUTHEAST:
				return MoveData.MoveDirection.SOUTHEAST;
			case Hex.Direction.SOUTH:
				return MoveData.MoveDirection.SOUTH;
			case Hex.Direction.SOUTHWEST:
				return MoveData.MoveDirection.SOUTHWEST;
			case Hex.Direction.NORTHWEST:
				return MoveData.MoveDirection.NORTHWEST;
			}

			// Return none by default
			return MoveData.MoveDirection.NONE;
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
}