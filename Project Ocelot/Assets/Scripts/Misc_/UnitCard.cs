using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectOcelot.UI
{
	public class UnitCard : MonoBehaviour
	{
		#region UI Elements

		[SerializeField]
		private GameObject unselectedContainer;

		[SerializeField]
		private GameObject selectedContainer;

		[SerializeField]
		private Image border;

		[SerializeField]
		private TextMeshProUGUI unitName;

		[SerializeField]
		private Image unitPortrait;

		[SerializeField]
		private TextMeshProUGUI unitRole;

		#endregion // UI Elements

		#region Card Data

		private bool isEnabled;
		private bool isCardOccupied;

		private const string LEADER_ICON = "";
		private const string OFFENSE_ICON = "";
		private const string DEFENSE_ICON = "";
		private const string SUPPORT_ICON = "";
		private const string GRUNT_ICON = "";

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
				gameObject.SetActive ( isEnabled );
			}
		}

		/// <summary>
		/// Whether or not the card is displaying a unit.
		/// </summary>
		public bool IsCardOccupied
		{
			get
			{
				// Return value
				return isCardOccupied;
			}
		}

		#endregion // Card Data

		#region Public Functions

		/// <summary>
		/// Initializes the card to display a unit for a team.
		/// </summary>
		/// <param name="unit"> The unit to be displayed in the card. </param>
		/// <param name="team"> The unit's team. </param>
		public void SetCardWithUnit ( UnitSettingData unit, Match.Player.TeamColor team )
		{
			// Display card
			IsEnabled = true;
			isCardOccupied = true;
			unselectedContainer.SetActive ( false );
			selectedContainer.SetActive ( true );

			// Set team color
			border.color = Tools.Util.TeamColor ( team );
			unitPortrait.color = Tools.Util.TeamColor ( team );

			// Display unit data
			unitName.text = unit.UnitName;
			unitPortrait.sprite = unit.Portrait;
			switch ( unit.Role )
			{
			case UnitData.UnitRole.LEADER:
				unitRole.text = LEADER_ICON;
				break;
			case UnitData.UnitRole.OFFENSE:
				unitRole.text = OFFENSE_ICON;
				break;
			case UnitData.UnitRole.DEFENSE:
				unitRole.text = DEFENSE_ICON;
				break;
			case UnitData.UnitRole.SUPPORT:
				unitRole.text = SUPPORT_ICON;
				break;
			case UnitData.UnitRole.PAWN:
				unitRole.text = GRUNT_ICON;
				break;
			}
		}

		/// <summary>
		/// Initializes the card to display no unit.
		/// </summary>
		/// <param name="team"> The team color. </param>
		public void SetCardWithoutUnit ( Match.Player.TeamColor team )
		{
			// Display card
			IsEnabled = true;
			isCardOccupied = false;
			unselectedContainer.SetActive ( true );
			selectedContainer.SetActive ( false );

			// Set team color
			border.color = Tools.Util.TeamColor ( team );
		}

		/// <summary>
		/// Sets the border of the card to be highlighted or a team color.
		/// </summary>
		/// <param name="isHighlight"> Whether or not the border should be highlighted. </param>
		/// <param name="team"> The team color. </param>
		public void SetCardHighlight ( bool isHighlight, Match.Player.TeamColor team )
		{
			// Set border color
			border.color = isHighlight ? Tools.Util.AccentColor ( team ) : Tools.Util.TeamColor ( team );
		}

		#endregion // Public Functions
	}
}