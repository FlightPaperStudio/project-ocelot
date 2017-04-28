using UnityEngine;
using System.Collections;

public class Turn : MonoBehaviour 
{
	/// <summary>
	/// The game objects that will clicked on this turn.
	/// </summary>
	public GameObject [ ] objs;

	/// <summary>
	/// The display arrows used on this turn.
	/// </summary>
	public GameObject [ ] arrows;

	/// <summary>
	/// A list of when a prompt should interrupt a turn.
	/// </summary>
	public int [ ] interrupts;

	/// <summary>
	/// The tutorial prompts displayed this turn.
	/// </summary>
	public string [ ] prompts;

	/// <summary>
	/// The amount of time needed to delay between moves for the computer.
	/// </summary>
	public float [ ] delays;

	/// <summary>
	/// A check for if this is an automated turn for the opponent.
	/// </summary>
	public bool isOpponent = false;
}
