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

	#region Public Functions

	/// <summary>
	/// Sets the unit to be displayed in the portrait.
	/// </summary>
	/// <param name="unit"> The unit being displayed. </param>
	/// <param name="team"> The team color of the unit. </param>
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

	#endregion // Public Funtions
}
