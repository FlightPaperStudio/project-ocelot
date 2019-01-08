using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
	#region Tile Object Data

	public SpriteRenderer Icon;
	public HeroUnit Caster;
	public Hex CurrentHex;

	public bool CanBeOccupied;
	public bool CanAssist;
	public bool CanBeMoved;
	public bool CanBeAttacked;

	private int duration;

	public delegate void TileObjectDelegate ( );
	private TileObjectDelegate durationDelegate;
	private TileObjectDelegate attackDelegate;

	#endregion // Tile Object Data

	#region Public Functions

	/// <summary>
	/// Sets the tile object instance information.
	/// </summary>
	public void SetTileObject ( HeroUnit caster, Hex hex, int objDuration, TileObjectDelegate durationDel, TileObjectDelegate attackDel = null )
	{
		// Set owner
		Caster = caster;

		// Set tile
		CurrentHex = hex;

		// Set position
		transform.position = hex.transform.position;

		// Set duration
		duration = objDuration;

		// Set delegates
		durationDelegate = durationDel;
		attackDelegate = attackDel;
	}

	/// <summary>
	/// Decrements the duration each turn.
	/// </summary>
	public void Duration ( )
	{
		// Decrement duration
		duration--;

		// Check if duration has expired
		if ( duration <= 0 )
			OnDurationExpire ( );
	}

	/// <summary>
	/// Assists a unit.
	/// </summary>
	public void Assist ( )
	{
		// Add assist for the caster
		Caster.Assist ( );
	}

	/// <summary>
	/// Responds to getting attacked by an opponent.
	/// </summary>
	public void GetAttacked ( )
	{
		// Check delegates
		if ( attackDelegate != null )
			attackDelegate ( );
		else
			OnDurationExpire ( );
	}

	/// <summary>
	/// Determines if this tile object can be attacked by another unit.
	/// Call this function on the object being attacked with the unit that is attacking as the parameter.
	/// Returns true if this unit can be attacked.
	/// </summary>
	/// <param name="attacker"> The unit doing the attacking. </param>
	/// <param name="friendlyFire"> Whether or not the attack can affect ally units. </param>
	/// <returns> Whether or not this unit can be attacked by another unit. </returns>
	public bool UnitAttackCheck ( Unit attacker, bool friendlyFire = false )
	{
		// Check if this object can be attacked
		if ( !CanBeAttacked )
			return false;

		// Check if the object and attacker are on the same team
		if ( !friendlyFire && attacker.Owner == Caster.Owner )
			return false;

		// Check if the attacking unit can attack
		if ( !attacker.Status.CanAttack )
			return false;

		// Return that this object can be attacked by the attacking unit
		return true;
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Executes the delegate on the duration of this tile object expiring.
	/// </summary>
	private void OnDurationExpire ( )
	{
		// Execute delegate
		durationDelegate ( );
	}

	#endregion // Private Functions
}
