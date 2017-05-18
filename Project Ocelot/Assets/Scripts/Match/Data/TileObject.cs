using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
	// Object information
	public SpriteRenderer sprite;
	public HeroUnit owner;
	public Tile tile;
	public bool canBeOccupied;
	public bool canBeJumped;
}
