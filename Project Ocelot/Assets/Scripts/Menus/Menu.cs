using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour 
{
	#region UI Elements

	[SerializeField]
	protected GameObject menuContainer;

	#endregion // UI Elements

	#region Menu Data

	public Menu ParentMenu;
	
	/// <summary>
	/// Tracks whether or not this menu is currently open.
	/// </summary>
	public bool IsOpen
	{
		get
		{
			return menuContainer.activeSelf;
		}
	}

	#endregion // Menu Data

	#region Public Functions

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
	/// Closes the menu.
	/// Use this as a button even wrapper for going up a layer (e.g. from a sub menu to a parent menu).
	/// Default parameters are passed.
	/// </summary>
	public void CloseMenu ( )
	{
		// Close the menu
		CloseMenu ( true );
	}

	#endregion // Public Functions

	#region Public Virtual Functions

	/// <summary>
	/// Opens the menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	/// <param name="closeParent"> Whether or not the parent menu should be closed upon this menu opening. </param>
	public virtual void OpenMenu ( bool closeParent = true )
	{
		// Close parent menu
		if ( ParentMenu != null && closeParent )
			ParentMenu.CloseMenu ( false );

		// Display menu
		menuContainer.SetActive ( true );
	}

	/// <summary>
	/// Closes the menu.
	/// Use this for going up a layer (e.g. from a sub menu to a parent menu).
	/// </summary>
	/// <param name="openParent"> Whether or not the parent menu should be opened upon this menu closing. </param>
	/// <param name="values"> Any parameters needed as additional information when opening the parent menu. </param>
	public virtual void CloseMenu ( bool openParent = true )
	{
		// Hide menu
		menuContainer.SetActive ( false );

		// Open parent menu
		if ( ParentMenu != null && openParent )
			ParentMenu.OpenMenu ( false );
	}

	#endregion // Public Virtual Functions
}
