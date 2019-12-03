using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectOcelot.Match.Arena;

namespace ProjectOcelot.Match.HUD
{
	public class HexCursor : MonoBehaviour
	{
		#region UI Elements

		[SerializeField]
		private SpriteRenderer border;

		#endregion // UI Elements

		#region Cursor Data

		[SerializeField]
		private GameManager gm;

		private List<Hex> hexes = new List<Hex> ( );

		#endregion // Cursor Data

		#region Public Functions

		/// <summary>
		/// Displays the cursor over a hex.
		/// </summary>
		/// <param name="hex"> The hex being clicked or hovered over. </param>
		public void OnHexEnter ( Hex hex )
		{
			// Display hex
			gameObject.SetActive ( true );

			// Set cursor to the current player's team color
			border.color = Tools.Util.TeamColor ( gm.CurrentPlayer.Team );

			// Add hex to list
			if ( !hexes.Contains ( hex ) )
				hexes.Add ( hex );

			// Position cursor
			transform.position = hex.transform.position;
		}

		/// <summary>
		/// Removes the cursor from a hex.
		/// </summary>
		/// <param name="hex"> The hex no longer being hovered over. </param>
		public void OnHexExit ( Hex hex )
		{
			// Remove hex from list
			if ( hexes.Contains ( hex ) )
				hexes.Remove ( hex );

			// Hide cursor
			if ( hexes.Count == 0 )
				gameObject.SetActive ( false );
		}

		/// <summary>
		/// Forces the cursor to hide.
		/// </summary>
		public void HideCursor ( )
		{
			// Clear list
			hexes.Clear ( );

			// Hide cursor
			gameObject.SetActive ( false );
		}

		#endregion // Public Functions
	}
}