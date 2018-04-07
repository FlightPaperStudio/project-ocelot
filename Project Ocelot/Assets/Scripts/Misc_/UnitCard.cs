using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCard : MonoBehaviour
{
	#region UI Elements

	[SerializeField]
	private RectTransform container;

	[SerializeField]
	private Image border;

	[SerializeField]
	private TextMeshProUGUI unitName;

	[SerializeField]
	private Image unitPortrait;

	[SerializeField]
	private Image [ ] unitSlots;

	#endregion // UI Elements

	#region Card Data

	private UnitDefaultData unitData;
	private Player.TeamColor teamColor;
	private bool isEnabled;
	private bool isAvailable;

	private readonly Color32 UNAVAILABLE_COLOR = Color.grey;
	private readonly Color32 SLOT_COLOR = new Color32 ( 255, 210, 75, 255 ); 

	/// <summary>
	/// Whether or not the card is to be displayed.
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

			// Display or hide card
			container.gameObject.SetActive ( isEnabled );
		}
	}

	/// <summary>
	/// Whether or not the card is selectable.
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

			// Set card color
			border.color = isAvailable ? Util.TeamColor ( teamColor ) : UNAVAILABLE_COLOR;
			unitPortrait.color = isAvailable ? Util.TeamColor ( teamColor ) : UNAVAILABLE_COLOR;
			for ( int i = 0; i < unitSlots.Length; i++ )
				unitSlots [ i ].color = isAvailable ? SLOT_COLOR : UNAVAILABLE_COLOR;
		}
	}

	#endregion // Card Data

	#region Public Functions

	/// <summary>
	/// Initializes the card to display a unit for a team.
	/// </summary>
	/// <param name="unit"> The unit to be displayed in the card. </param>
	/// <param name="team"> The unit's team. </param>
	public void SetCard ( UnitDefaultData unit, Player.TeamColor team )
	{
		// Store unit and team
		unitData = unit;
		teamColor = team;

		// Display card
		IsEnabled = true;
		IsAvailable = true;

		// Display unit data
		unitName.text = unitData.UnitName;
		unitPortrait.sprite = unitData.Portrait;
		for ( int i = 0; i < unitSlots.Length; i++ )
			unitSlots [ i ].gameObject.SetActive ( i < unitData.Slots );
	}

	/// <summary>
	/// Reset the card to its default size.
	/// </summary>
	public void ResetSize ( )
	{
		// Set default size
		container.offsetMax = Vector2.zero;
		container.offsetMin = Vector2.zero;
	}

	/// <summary>
	/// Increases or decreases the size of the card based on its current size.
	/// </summary>
	/// <param name="offset"> How much the card's size should uniformly change. Positive values make it bigger and negative values make it smaller. </param>
	public void ChangeSize ( float offset )
	{
		// Set card size
		container.offsetMax = new Vector2 ( offset, offset );
		container.offsetMin = new Vector2 ( -1 * offset, -1 * offset );
	}

	#endregion // Public Functions
}
