using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveArea : MonoBehaviour
{
	#region Objective Data

	public Hex [ ] Hexes;

	#endregion // Objective Data

	#region Public Functions

	/// <summary>
	/// Checks whether or not a specific hex is contained within an objective area.
	/// </summary>
	/// <param name="hex"> The hex being checked. </param>
	/// <returns> Whether or not the hex is a part of the objective area. </returns>
	public bool Contains ( Hex hex )
	{
		// Check each hex
		for ( int i = 0; i < Hexes.Length; i++ )
		{
			// Check for hex
			if ( Hexes [ i ] == hex )
				return true;
		}

		// Return that the hex was not found
		return false;
	}

	/// <summary>
	/// Sets the border colors of each tile in the area.
	/// </summary>
	/// <param name="teamColor"> The color of the team entering in this area. </param>
	public void SetColor ( Player.TeamColor teamColor )
	{
		// Set each border color
		for ( int i = 0; i < Hexes.Length; i++ )
		{
			// Display border
			Hexes [ i ].Tile.SetBorderActive ( true );

			// Set border color
			Hexes [ i ].Tile.SetBorderColor ( Util.TeamColor ( teamColor ) );
		}
	}

	#endregion // Public Functions
}
