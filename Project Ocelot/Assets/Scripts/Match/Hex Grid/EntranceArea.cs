using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceArea : MonoBehaviour
{
	#region Entrance Data

	public Hex [ ] Hexes;

	#endregion // Entrance Data

	#region Public Functions

	/// <summary>
	/// Sets the border colors of each tile in the area.
	/// </summary>
	/// <param name="teamColor"> The color of the team entering in this area. </param>
	public void SetColor ( Player.TeamColor teamColor )
	{
		// Set each border color
		for ( int i = 0; i < Hexes.Length; i++ )
		{
			// Set border color
			Hexes [ i ].Tile.SetBorderColor ( Util.TeamColor ( teamColor ) );
		}
	}

	#endregion // Public Functions
}
