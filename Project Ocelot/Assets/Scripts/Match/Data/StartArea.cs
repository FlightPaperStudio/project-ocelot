using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartArea : MonoBehaviour 
{
	public Tile [ ] tiles;
	public SpriteRenderer [ ] outlines;
	public StartArea goal;

	/// <summary>
	/// Sets the team color for the goal area.
	/// </summary>
	public void SetColor ( Player.TeamColor c )
	{
		// Set the outline color for the goal area
		for ( int i = 0; i < outlines.Length; i++ )
			outlines [ i ].color = Util.TeamColor ( c );
	}

	/// <summary>
	/// Checks if a given tile is in this goal area.
	/// </summary>
	public bool IsGoalTile ( Tile t )
	{
		// Return true if this tile is in this goal area
		return goal.tiles.Contains ( t );
	}
}
