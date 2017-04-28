using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpMenu : Menu 
{
	// UI elements
	public TextMeshProUGUI prompt;
	public GameObject acknowledgeControls;
	public GameObject confirmControls;

	// Menu information
	public delegate void PopUpDelegate ( );
	private PopUpDelegate acknowledgeDelegate;
	private PopUpDelegate confirmDelegate;
	private PopUpDelegate denyDelegate;

	/// <summary>
	/// Opens the pop up menu.
	/// The parameters passed to the opening the pop up menu should be OpenMenu ( false, bool isAcknowledge, string prompt, delegate acknowledge/confirm, delegate deny ).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true, params object [ ] values )
	{
		// Reset any previous delegates
		acknowledgeDelegate = null;
		confirmDelegate = null;
		denyDelegate = null;

		// Display prompt
		prompt.text = values [ 1 ] as string;

		// Check pop up type
		bool type = (bool)values [ 0 ];
		if ( type )
		{
			// Display controls
			acknowledgeControls.SetActive ( true );
			confirmControls.SetActive ( false );

			// Store any delegates
			if ( values [ 2 ] != null )
				acknowledgeDelegate += values [ 2 ] as PopUpDelegate;
		}
		else
		{
			// Display controls
			acknowledgeControls.SetActive ( false );
			confirmControls.SetActive ( true );

			// Store any delegates
			if ( values [ 2 ] != null )
				confirmDelegate += values [ 2 ] as PopUpDelegate;
			if ( values [ 3 ] != null )
				denyDelegate += values [ 3 ] as PopUpDelegate;
		}

		StartCoroutine ( UpdateFrame ( closeParent ) );
	}

	private IEnumerator UpdateFrame ( bool closeParent )
	{
		// Wait one frame
		yield return 0;

		// Open the menu
		base.OpenMenu ( closeParent );
	}

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
}
