using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour
{
	#region UI Elements

	[SerializeField]
	private RectTransform container;

	[SerializeField]
	private Image border;

	[SerializeField]
	private Image unitPortrait;

	#endregion // UI Elements

	#region Portrait Data

	private UnitData unitData;
	private Player.TeamColor _teamColor;
	private bool isEnabled;
	private bool isAvailable;
	private bool isBorderHighlighted;

	private readonly Color32 UNAVAILABLE_COLOR = Color.grey;
	private readonly Color32 HIGHLIGHT_COLOR = new Color32 ( 255, 210, 75, 255 );

	/// <summary>
	/// Whether or not the portrait is to be displayed.
	/// </summary>
	public bool IsEnabled
	{
		get
		{
			// Return value
			return isEnabled;
		}
		set
		{
			// Store value
			isEnabled = value;

			// Display or hide portrait
			container.gameObject.SetActive ( isEnabled );
		}
	}

	/// <summary>
	/// Whether or not the portrait is selectable or interactable.
	/// </summary>
	public bool IsAvailable
	{
		get
		{
			// Return value
			return isAvailable;
		}
		set
		{
			// Store value
			isAvailable = value;

			// Set portrait color
			border.color = isAvailable ? Util.TeamColor ( _teamColor ) : UNAVAILABLE_COLOR;
			unitPortrait.color = isAvailable ? Util.TeamColor ( _teamColor ) : UNAVAILABLE_COLOR;
		}
	}

	/// <summary>
	/// Whether or not the border of the portrait should be highlighted to indicate selection.
	/// </summary>
	public bool IsBorderHighlighted
	{
		get
		{
			// Return value
			return isBorderHighlighted;
		}
		set
		{
			// Store value
			isBorderHighlighted = value;

			// Check value
			if ( isBorderHighlighted )
			{
				// Set border color to highlighted
				border.color = HIGHLIGHT_COLOR;
			}
			else
			{
				// Set border color to its normal color
				border.color = isAvailable ? Util.TeamColor ( _teamColor ) : UNAVAILABLE_COLOR;
			}
		}
	}

	public bool IsSelected
	{
		get;
		private set;
	}
	private int unitID;
	public Color32 teamColor
	{
		get;
		private set;
	}


	#endregion // Portrait Data

	//#region Mouse Input Functions

	///// <summary>
	///// Enlarge the portrait upon hovering over an enabled and unselected unit.
	///// </summary>
	//public void MouseEnter ( )
	//{
	//	// Check state
	//	if ( IsEnabled && !IsSelected )
	//	{
	//		// Set enlarged size
	//		SetSize ( false );
	//	}
	//}

	///// <summary>
	///// Sets the portrait to its normal size upon the mouse no longer hover over an enable and unselected unit.
	///// </summary>
	//public void MouseExit ( )
	//{
	//	// Check state
	//	if ( IsEnabled && !IsSelected )
	//	{
	//		// Set default size
	//		SetSize ( true );
	//	}
	//}

	///// <summary>
	///// Selects the unit upon clicking the button.
	///// </summary>
	//public void MouseClick ( )
	//{
	//	// Check state
	//	if ( IsEnabled && !IsSelected )
	//	{
	//		// Select the hero
	//		SelectToggle ( true );
	//	}
	//}

	//#endregion // Mouse Input Functions

	#region Public Functions

	public void SetPortrait ( UnitData unit, Player.TeamColor team )
	{
		// Store unit
		unitData = unit;

		// Store team
		_teamColor = team;

		// Display portrait
		IsEnabled = true;
		IsAvailable = true;
		IsBorderHighlighted = false;
		unitPortrait.sprite = unitData.Portrait;
	}


	/// <summary>
	/// Displays a given unit in the portait.
	/// </summary>
	/// <param name="id"> The ID of the unit to be displayed. </param>
	/// <param name="sprite"> The icon of the unit to be displayed. </param>
	/// <param name="col"> The team color. </param>
	public void SetUnit ( int id, Sprite sprite, Color32 col )
	{
		// Store the hero id
		unitID = id;

		// Display icon
		unitPortrait.sprite = sprite;

		// Store team color
		teamColor = col;

		// Enable the portrait
		EnableToggle ( true );

		// Unselect the portrait
		SelectToggle ( false );
	}

	/// <summary>
	/// Reset the portrait to its default size.
	/// </summary>
	public void ResetSize ( )
	{
		// Set default size
		container.offsetMax = Vector2.zero;
		container.offsetMin = Vector2.zero;
	}

	/// <summary>
	/// Increases or decreases the size of the portrait from its default size.
	/// </summary>
	/// <param name="offset"> How much the portrait's size should uniformly change. Positive values make it bigger and negative values make it smaller. </param>
	public void ChangeSize ( float offset )
	{
		// Set card size
		container.offsetMax = new Vector2 ( offset, offset );
		container.offsetMin = new Vector2 ( -1 * offset, -1 * offset );
	}


	/// <summary>
	/// Sets whether or not the unit is enabled.
	/// This sets the color of the portrait (either the team color or grey).
	/// </summary>
	/// <param name="isEnable"> Whether or not the portrait is enabled. </param>
	public void EnableToggle ( bool isEnable )
	{
		// Set enabled
		IsEnabled = isEnable;

		// Set border color
		border.color = isEnable ? teamColor : UNAVAILABLE_COLOR;

		// Set icon color
		unitPortrait.color = isEnable ? teamColor : UNAVAILABLE_COLOR;
	}

	/// <summary>
	/// Sets whether or not the hero is selected during team selection.
	/// This sets the size of the button.
	/// </summary>
	/// <param name="isSelect"> Whether or not the portrait is currently selected. </param>
	public void SelectToggle ( bool isSelect )
	{
		// Set select
		IsSelected = isSelect;

		// Set border color
		//if ( !IsEnabled )
		//	border.color = UNAVAILABLE_COLOR;
		//else if ( isSelect )
		//	border.color = SLOT_COLOR;
		//else
		//	border.color = teamColor;

		// Set size
		SetSize ( !IsSelected );
	}

	#endregion // Public Funtions

	#region Private Functions

	/// <summary>
	/// Sets the size of the button.
	/// The button is enlarged when selected or hovered over.
	/// </summary>
	/// <param name="isDefaultSize"> Whether or not the portrait should be its default size. </param>
	private void SetSize ( bool isDefaultSize )
	{
		// Check if the button should be set to its enlarged size
		if ( isDefaultSize )
		{
			// Set default size
			container.offsetMax = Vector2.zero;
			container.offsetMin = Vector2.zero;
		}
		else
		{
			// Set enlarged size
			container.offsetMax = new Vector2 ( 5f, 5f );
			container.offsetMin = new Vector2 ( -5f, -5f );
		}
	}

	#endregion // Private Functions
}
