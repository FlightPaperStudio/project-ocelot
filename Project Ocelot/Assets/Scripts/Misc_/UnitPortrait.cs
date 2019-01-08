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
	private Player.TeamColor teamColor;
	private bool isEnabled;
	private bool isAvailable;
	private bool isBorderHighlighted;

	private readonly Color32 UNAVAILABLE_COLOR = Color.grey;

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
			border.color = isAvailable ? Util.TeamColor ( teamColor ) : UNAVAILABLE_COLOR;
			unitPortrait.color = isAvailable ? Util.TeamColor ( teamColor ) : UNAVAILABLE_COLOR;
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
				border.color = Util.AccentColor ( teamColor );
			}
			else
			{
				// Set border color to its normal color
				border.color = isAvailable ? Util.TeamColor ( teamColor ) : UNAVAILABLE_COLOR;
			}
		}
	}

	public bool IsSelected
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
		teamColor = team;

		// Display portrait
		IsEnabled = true;
		IsAvailable = true;
		IsBorderHighlighted = false;
		unitPortrait.sprite = unitData.Portrait;
	}

	/// <summary>
	/// Copies the settings from another portrait.
	/// </summary>
	/// <param name="from"> The portrait being copied from. </param>
	public void CopyPortrait ( UnitPortrait from )
	{
		// Store unit
		unitData = from.unitData;

		// Store team
		teamColor = from.teamColor;

		// Display portrait
		IsEnabled = from.IsEnabled;
		IsAvailable = from.IsAvailable;
		IsBorderHighlighted = from.IsBorderHighlighted;
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

	#endregion // Public Funtions
}
