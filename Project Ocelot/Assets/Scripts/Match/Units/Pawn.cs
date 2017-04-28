using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Unit
{
	/// <summary>
	/// Initializes this Pawn unit instance.
	/// </summary>
	private void Start ( )
	{
		// Generate random name
		name = NameGenerator.CreateName ( );
	}
}
