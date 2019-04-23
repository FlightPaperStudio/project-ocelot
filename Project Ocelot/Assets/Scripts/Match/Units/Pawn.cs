using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Unit
{
	// Hero information
	public string characterNickname;

	/// <summary>
	/// Initializes this Pawn unit instance.
	/// </summary>
	private void Start ( )
	{
		// Generate random name
		//characterName = NameGenerator.CreateName ( );

		// Generate nickname
		//characterNickname = NameGenerator.CreateNickname ( );
	}
}
