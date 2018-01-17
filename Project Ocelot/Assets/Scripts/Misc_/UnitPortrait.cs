using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour
{
	#region UI Elements

	public RectTransform container;
	public Image border;
	public Image icon;
	public Image [ ] slots;

	#endregion // UI Elements

	#region Portrait Data

	public bool IsEnabled
	{
		get;
		private set;
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
	private readonly Color32 DISABLED_COLOR = Color.grey; //new Color32 ( 100, 100, 100, 255 );
	private readonly Color32 SLOT_COLOR = new Color32 ( 255, 210, 75, 255 );

	#endregion // Portrait Data

	#region Public Functions

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
		icon.sprite = sprite;

		// Set slots
		if ( slots != null )
		{
			for ( int i = 0; i < slots.Length; i++ )
			{
				// Check for hero
				if ( unitID != MatchSettings.LEADER_UNIT && unitID != MatchSettings.PAWN_UNIT )
				{
					// Display the slot marker
					slots [ i ].gameObject.SetActive ( i < HeroInfo.GetHeroByID ( unitID ).Slots );

					// Set slot color
					slots [ i ].color = SLOT_COLOR;
				}
				else
				{
					// Hide slot markers
					slots [ i ].gameObject.SetActive ( false );
				}
			}
		}

		// Store team color
		teamColor = col;

		// Enable the portrait
		EnableToggle ( true );

		// Unselect the portrait
		SelectToggle ( false );
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

		// Check which color the button should be set
		if ( IsEnabled )
		{
			// Set border color
			border.color = teamColor;

			// Set icon color
			icon.color = teamColor;

			// Set slot color
			if ( slots != null && unitID != MatchSettings.LEADER_UNIT && unitID != MatchSettings.PAWN_UNIT )
				for ( int i = 0; i < slots.Length; i++ )
					slots [ i ].color = SLOT_COLOR;
		}
		else
		{
			// Set border color
			border.color = DISABLED_COLOR;

			// Set icon color
			icon.color = DISABLED_COLOR;

			// Set slot color
			if ( slots != null )
				for ( int i = 0; i < slots.Length; i++ )
					slots [ i ].color = DISABLED_COLOR;
		}
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
		if ( !IsEnabled )
			border.color = DISABLED_COLOR;
		else if ( isSelect )
			border.color = SLOT_COLOR;
		else
			border.color = teamColor;

		// Set size
		SetSize ( !IsSelected );
	}

	#region Mouse Input Functions

	/// <summary>
	/// Enlarge the portrait upon hovering over an enabled and unselected unit.
	/// </summary>
	public void MouseEnter ( )
	{
		// Check state
		if ( IsEnabled && !IsSelected )
		{
			// Set enlarged size
			SetSize ( false );
		}
	}

	/// <summary>
	/// Sets the portrait to its normal size upon the mouse no longer hover over an enable and unselected unit.
	/// </summary>
	public void MouseExit ( )
	{
		// Check state
		if ( IsEnabled && !IsSelected )
		{
			// Set default size
			SetSize ( true );
		}
	}

	/// <summary>
	/// Selects the unit upon clicking the button.
	/// </summary>
	public void MouseClick ( )
	{
		// Check state
		if ( IsEnabled && !IsSelected )
		{
			// Select the hero
			SelectToggle ( true );
		}
	}

	#endregion // Mouse Input Functions

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
