using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour 
{
	//Piece information
	public PieceColor color;
	public Tile currentTile;
	public Tile prevTile;
	public Player owner;
	public Ability ability;
	public SpriteRenderer sprite;
	public static PieceColors Display = new PieceColors ( );

	//A check for preventing the unselecting of jolted pieces
	public bool isJolted = false;

	//A check to prevent two pieces in a nonagression pact from interacting with one another
	public List < PieceColor > nonagressionPartners = new List < PieceColor > ( );

	//A check to see how many moves the sacrifice piece has taken that turn
	public bool sacrificeMoveRemaining = true;

	/// <summary>
	/// Move a piece to a specified position.
	/// </summary>
	public void Move ( Vector3 pos )
	{
		//Move piece to new position
		this.transform.position = pos;
	}

	/// <summary>
	/// Removes this piece from the board.
	/// </summary>
	public void Capture ( )
	{
		Destroy ( this.gameObject );
	}

	/// <summary>
	/// Determines if a tile is the previous tile the piece occupied.
	/// </summary>
	public bool IsPrevTile ( Tile t )
	{
		//Check for the previous tile
		if ( prevTile != null && prevTile == t )
			return true;

		//Return the tile is not the previous tile
		return false;
	}
}

public enum PieceColor
{
	None = 0,
	White = 1,
	Red = 2,
	Blue = 3,
	Green = 4,
	Yellow = 5,
	Black = 6,
	Grey = 7
}

public class PieceColors
{
	private Color32 white  = new Color32 ( 255, 255, 255, 255 );
	private Color32 red    = new Color32 ( 255,   0,   0, 255 );
	private Color32 blue   = new Color32 (   0,   0, 255, 255 );
	private Color32 green  = new Color32 (   0, 200,   0, 255 );
	private Color32 yellow = new Color32 ( 255, 255,   0, 255 );
	private Color32 black  = new Color32 (   0,   0,   0, 255 );
	private Color32 grey   = new Color32 ( 150, 150, 150, 255 );

	public Color32 White
	{
		get { return white; }
	}

	public Color32 Red
	{
		get { return red; }
	}

	public Color32 Blue
	{
		get { return blue; }
	}

	public Color32 Green
	{
		get { return green; }
	}

	public Color32 Yellow
	{
		get { return yellow; }
	}

	public Color32 Black
	{
		get { return black; }
	}

	public Color32 Grey
	{
		get { return grey; }
	}
}