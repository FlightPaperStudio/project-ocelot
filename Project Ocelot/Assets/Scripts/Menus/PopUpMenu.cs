using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectOcelot.Menues
{
	public class PopUpMenu : Menu
	{
		#region UI Elements

		[SerializeField]
		private TextMeshProUGUI promptText;

		[SerializeField]
		private GameObject acknowledgeControls;

		[SerializeField]
		private GameObject confirmControls;

		#endregion // UI Elements

		#region Pop Up Menu Data

		public delegate void PopUpDelegate ( );
		private PopUpDelegate acknowledgeDelegate;
		private PopUpDelegate confirmDelegate;
		private PopUpDelegate denyDelegate;

		private bool isAcknowledgePopUp;

		#endregion // Pop Up Menu Data

		#region Menu Override Functions

		/// <summary>
		/// Opens the pop up menu.
		/// The parameters passed to the opening the pop up menu should be OpenMenu ( false, bool isAcknowledge, string prompt, delegate acknowledge/confirm, delegate deny ).
		/// </summary>
		public override void OpenMenu ( bool closeParent = true )
		{
			//// Reset any previous delegates
			//acknowledgeDelegate = null;
			//confirmDelegate = null;
			//denyDelegate = null;

			//// Display prompt
			//promptText.text = values [ 1 ] as string;

			//// Check pop up type
			//bool type = (bool)values [ 0 ];
			//if ( type )
			//{
			//	// Display controls
			//	acknowledgeControls.SetActive ( true );
			//	confirmControls.SetActive ( false );

			//	// Store any delegates
			//	if ( values [ 2 ] != null )
			//		acknowledgeDelegate += values [ 2 ] as PopUpDelegate;
			//}
			//else
			//{
			//	// Display controls
			//	acknowledgeControls.SetActive ( false );
			//	confirmControls.SetActive ( true );

			//	// Store any delegates
			//	if ( values [ 2 ] != null )
			//		confirmDelegate += values [ 2 ] as PopUpDelegate;
			//	if ( values [ 3 ] != null )
			//		denyDelegate += values [ 3 ] as PopUpDelegate;
			//}

			StartCoroutine ( UpdateFrame ( closeParent ) );
		}

		#endregion // Menu Override Functions

		#region Event Trigger Functions

		/// <summary>
		/// Acknowledges the pop up prompt.
		/// Use this as a button click event wrapper for the acknowledge controls.
		/// Will play the acknowledge delegate if it exists. Otherwise, it will just close the pop up.
		/// </summary>
		public void OnAcknowledge ( )
		{
			// Check for delegate
			if ( acknowledgeDelegate != null )
				acknowledgeDelegate ( );
			else
				base.CloseMenu ( );
		}

		/// <summary>
		/// Confirms the pop up prompt.
		/// Use this as a button click event wrapper for the confirm controls.
		/// Will play the confirm delegate if it exists. Otherwise, it will just close the pop up.
		/// </summary>
		public void OnConfirm ( )
		{
			// Check for delegate
			if ( confirmDelegate != null )
				confirmDelegate ( );
			else
				base.CloseMenu ( );
		}

		/// <summary>
		/// Denies the pop up prompt.
		/// Use this as a button click event wrapper for the confirm controls.
		/// Will play the deny delegate if it exists. Otherwise, it will just close the pop up.
		/// </summary>
		public void OnDeny ( )
		{
			// Check for delegate
			if ( denyDelegate != null )
				denyDelegate ( );
			else
				base.CloseMenu ( );
		}

		#endregion // Event Trigger Functions

		#region Public Functions

		/// <summary>
		/// Set up the popup menu to display an acknowledgement prompt.
		/// Be sure to call this function BEFORE calling OpenMenu().
		/// </summary>
		/// <param name="prompt"> The text to display in the popup menu. </param>
		/// <param name="acknowledge"> What should happen when the player clicks the acknowledge button. </param>
		public void SetAcknowledgementPopUp ( string prompt, PopUpDelegate acknowledge )
		{
			// Display prompt
			promptText.text = prompt;

			// Display acknowledge controls
			acknowledgeControls.SetActive ( true );
			confirmControls.SetActive ( false );

			// Clear previous delegates
			acknowledgeDelegate = null;
			confirmDelegate = null;
			denyDelegate = null;

			// Set acknowledge delegate
			acknowledgeDelegate += acknowledge;
		}

		/// <summary>
		/// Set up the popup menu to display a confirmation prompt.
		/// Be sure to call this function BEFORE calling OpenMenu().
		/// </summary>
		/// <param name="prompt"> The text to display in the popup menu. </param>
		/// <param name="confirm"> What should happen when the player clicks the confirm button. </param>
		/// <param name="deny"> What should happen when the player clicks the deny button. </param>
		public void SetConfirmationPopUp ( string prompt, PopUpDelegate confirm, PopUpDelegate deny )
		{
			// Display prompt
			promptText.text = prompt;

			// Display confirm controls
			acknowledgeControls.SetActive ( false );
			confirmControls.SetActive ( true );

			// Clear previous delegates
			acknowledgeDelegate = null;
			confirmDelegate = null;
			denyDelegate = null;

			// Set acknowledge delegate
			confirmDelegate += confirm;
			denyDelegate += deny;
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Waits until the end of the frame to open the popup menu so that the UI canvas can reshape itself for the controls.
		/// </summary>
		/// <param name="closeParent"> Whether or not the parent menu should close when openning the popup menu. </param>
		/// <returns></returns>
		private IEnumerator UpdateFrame ( bool closeParent )
		{
			// Wait until the end of frame for the UI canvas to reshape itself
			yield return new WaitForEndOfFrame ( );

			// Open the menu
			base.OpenMenu ( closeParent );
		}

		#endregion // Private Functions
	}
}