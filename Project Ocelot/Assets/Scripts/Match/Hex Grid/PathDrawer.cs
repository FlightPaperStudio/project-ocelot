using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Match.HUD
{
	public class PathDrawer : MonoBehaviour
	{
		#region UI Elements

		[SerializeField]
		private LineRenderer line;

		#endregion // UI Elements

		#region Public Functions

		/// <summary>
		/// Displays the selected movement path.
		/// </summary>
		/// <param name="unit"> The current unit selected. </param>
		/// <param name="move"> The current move selected. </param>
		public void DisplayPath ( Units.Unit unit, MoveData move )
		{
			// Display path
			gameObject.SetActive ( true );

			// Set line color
			SetColor ( unit.Owner.Team );

			// Set the number of points
			line.positionCount = GetMoveTotal ( 0, move ) + 1;

			// Set points
			line.SetPosition ( 0, unit.CurrentHex.transform.position );
			SetMovePath ( line.positionCount - 1, move );
		}

		/// <summary>
		/// Hide the movement path.
		/// </summary>
		public void HidePath ( )
		{
			// Hide path
			gameObject.SetActive ( false );
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Sets the color of the path.
		/// </summary>
		/// <param name="team"> The team of the current unit. </param>
		private void SetColor ( Player.TeamColor team )
		{
			line.startColor = Tools.Util.AccentColor ( team, 50 );
			line.endColor = Tools.Util.AccentColor ( team, 200 );
		}

		/// <summary>
		/// Gets the total number of moves selected.
		/// </summary>
		/// <param name="counter"> The number of moves counted so far. </param>
		/// <param name="move"> The current move data. </param>
		/// <returns> The total number of moves selected. </returns>
		private int GetMoveTotal ( int counter, MoveData move )
		{
			// Check for previous move
			if ( move.PriorMove != null )
				return GetMoveTotal ( counter + 1, move.PriorMove );
			else
				return counter + 1;
		}

		/// <summary>
		/// Sets the position of every move for the path.
		/// </summary>
		/// <param name="index"> The index of the move. </param>
		/// <param name="move"> The current move data. </param>
		private void SetMovePath ( int index, MoveData move )
		{
			// Check for previous move
			if ( move.PriorMove != null )
				SetMovePath ( index - 1, move.PriorMove );

			// Set point
			line.SetPosition ( index, move.Destination.transform.position );
		}

		#endregion // Private Functions
	}
}