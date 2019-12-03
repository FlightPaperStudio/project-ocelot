using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ProjectOcelot.UI
{
	public class CardButton : MonoBehaviour
	{
		#region UI Elements

		[SerializeField]
		private RectTransform rect;

		[SerializeField]
		private GameObject outline;

		#endregion // UI Elements

		#region Menu Data

		[SerializeField]
		private Vector2 offset = new Vector2 ( 5f, 5f );

		[SerializeField]
		private UnityEvent onClick;

		private bool isEnlarged = false;
		private bool isForced = false;

		/// <summary>
		/// Whether or not this card is currently enlarged.
		/// </summary>
		public bool IsEnlarged
		{
			get
			{
				return isEnlarged;
			}
		}

		#endregion // Menu Data

		#region Public Functions

		/// <summary>
		/// Highlight card on mouse enter.
		/// </summary>
		public void MouseEnter ( )
		{
			// Check if size is forced
			if ( !isForced )
				Enlarge ( true );
		}

		/// <summary>
		/// Unhighlight card on mouse exit.
		/// </summary>
		public void MouseExit ( )
		{
			// Check if size is forced
			if ( !isForced )
				Enlarge ( false );
		}

		/// <summary>
		/// Callback for on mouse click.
		/// </summary>
		public void MouseClick ( )
		{
			// Call on click event
			onClick.Invoke ( );
		}

		/// <summary>
		/// Forces this card to a particular state.
		/// </summary>
		/// <param name="forced"> Whether or not the card should ignore mouse movement. </param>
		/// <param name="enlarged"> Whether or not the card should be enlarged. </param>
		public void ForceEnlarge ( bool forced, bool enlarged )
		{
			// Save state
			isForced = forced;

			// Set size
			Enlarge ( enlarged );
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Increases or decreases the size of the card.
		/// </summary>
		/// <param name="enlarged"> Whether or not the card should be enlarged. </param>
		private void Enlarge ( bool enlarged )
		{
			// Save state
			isEnlarged = enlarged;

			// Set size of card
			rect.offsetMax = isEnlarged ? offset : Vector2.zero;
			rect.offsetMin = isEnlarged ? offset * -1f : Vector2.zero;

			// Display outline
			if ( outline != null )
				outline.SetActive ( isEnlarged );
		}

		#endregion // Private Functions
	}
}