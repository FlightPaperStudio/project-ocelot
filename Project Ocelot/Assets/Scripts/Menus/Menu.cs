using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour 
{
	// Menu elements
	public Menu parentMenu;
	public GameObject menuContainer;

	/// <summary>
	/// Opens the menu.
	/// Use this as a button event wrapper for going down a layer (e.g. from a parent menu to a sub menu).
	/// Default parameters are passed.
	/// </summary>
	public void OpenMenu ( )
	{
		// Open the menu
		OpenMenu ( true );
	}

	/// <summary>
	/// Opens the menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public virtual void OpenMenu ( bool closeParent = true, params object [ ] values )
	{
		// Close parent menu
		if ( parentMenu != null && closeParent )
			parentMenu.CloseMenu ( false );

		// Display menu
		menuContainer.SetActive ( true );
	}

	/// <summary>
	/// Closes the menu.
	/// Use this as a button even wrapper for going up a layer (e.g. from a sub menu to a parent menu).
	/// Default parameters are passed.
	/// </summary>
	public void CloseMenu ( )
	{
		// Close the menu
		CloseMenu ( true );
	}

	/// <summary>
	/// Closes the menu.
	/// Use this for going up a layer (e.g. from a sub menu to a parent menu).
	/// </summary>
	public virtual void CloseMenu ( bool openParent = true, params object [ ] values )
	{
		// Hide menu
		menuContainer.SetActive ( false );

		// Open parent menu
		if ( parentMenu != null && openParent )
			parentMenu.OpenMenu ( false, values );
	}
}
