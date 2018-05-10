using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SlideMenu : MonoBehaviour
{
	#region UI Elements

	[SerializeField]
	private RectTransform container;

	[SerializeField]
	private SlideMenu parentMenu;

	[SerializeField]
	private Button parentButton;

	#endregion // UI Elements

	#region Menu Data

	[HideInInspector]
	public SlideMenu CurrentSubmenu;

	private const float SLIDE_TIME = 0.5f;

	/// <summary>
	/// Whether or not the menu is currently open.
	/// </summary>
	public bool IsOpen
	{
		get
		{
			// Return if the menu is open
			return container.gameObject.activeSelf;
		}
	}

	#endregion // Menu Data

	#region Public Functions

	/// <summary>
	/// Opens the slide menu.
	/// Use this function as a button click event wrapper.
	/// </summary>
	public void OpenMenu ( )
	{
		// Open the menu with animation
		OpenMenu ( true );
	}

	/// <summary>
	/// Closes the slide menu.
	/// Use this function as a button click event wrapper.
	/// </summary>
	public void CloseMenu ( )
	{
		// Close the menu with animation
		CloseMenu ( true );
	}

	#endregion // Public Functions

	#region Protected Virtual Functions

	/// <summary>
	/// Opens the slide menu.
	/// </summary>
	/// <param name="playAnimation"> Whether or not the slide animation should play when opening the menu. </param>
	protected virtual void OpenMenu ( bool playAnimation )
	{
		// Check if the menu is already open
		if ( parentMenu.CurrentSubmenu == this )
			return;

		// Check if the parent menu has a different submenu open
		if ( parentMenu != null && parentMenu.CurrentSubmenu != null && parentMenu.CurrentSubmenu.IsOpen )
		{
			// Close other submenu
			parentMenu.CurrentSubmenu.CloseMenu ( playAnimation );
		}

		// Set the parent menu's current submenu to this
		if ( parentMenu != null )
			parentMenu.CurrentSubmenu = this;

		// Disable the parent button now that the menu is open
		if ( parentMenu != null )
			parentButton.interactable = false;

		// Check if slide animation should play
		if ( playAnimation )
			SlideIn ( );
	}

	/// <summary>
	/// Closes the slide menu.
	/// </summary>
	/// <param name="playAnimation"> Whether or not the slide animation should play when closing the menu. </param>
	protected virtual void CloseMenu ( bool playAnimation )
	{
		// Remove this as the parent menu's current submenu
		if ( parentMenu != null )
			parentMenu.CurrentSubmenu = null;

		// Enable the parent button now that the menu is closed
		if ( parentMenu != null )
			parentButton.interactable = true;

		// Check if slide animation should play
		if ( playAnimation )
			SlideOut ( );
	}

	#endregion // Protected Virtual Functions

	#region Private Functions

	/// <summary>
	/// Creates the animation for the menu to slide inward.
	/// Call this function when opening the menu.
	/// </summary>
	private void SlideIn ( )
	{
		// Create slide in animation
		container.DOAnchorPos ( new Vector2 ( container.rect.width, 0 ), SLIDE_TIME ).From ( )
			.OnStart ( ( ) =>
			{
				// Make the menu visible
				container.gameObject.SetActive ( true );
			} );
	}

	/// <summary>
	/// Creates the animation for the menu to slide outward.
	/// Call this function when closing the menu.
	/// </summary>
	private void SlideOut ( )
	{
		// Create slide out animation
		container.DOAnchorPos ( new Vector2 ( container.rect.width, 0 ), SLIDE_TIME )
			.OnComplete ( ( ) =>
			{
				// Reset the menu's position
				container.anchoredPosition = Vector2.zero;

				// Hide the menu
				container.gameObject.SetActive ( false );
			} );
	}

	#endregion // Private Functions
}
