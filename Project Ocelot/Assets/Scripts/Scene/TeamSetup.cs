using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectOcelot.Match.Setup
{
	public class TeamSetup : MonoBehaviour
	{
		#region Private Classes

		[System.Serializable]
		private class UnitAbilityTag
		{
			public GameObject Container;
			public Image Icon;
			public TextMeshProUGUI Name;
		}

		#endregion // Private Classes

		#region UI Elements

		[SerializeField]
		private TextMeshProUGUI matchTitle;

		[SerializeField]
		private TextMeshProUGUI matchTypeIcon;

		[SerializeField]
		private TextMeshProUGUI matchType;

		[SerializeField]
		private TextMeshProUGUI playerName;

		[SerializeField]
		private TextMeshProUGUI menuInstructions;

		[SerializeField]
		private Image unitPortrait;

		[SerializeField]
		private TextMeshProUGUI unitName;

		[SerializeField]
		private TextMeshProUGUI unitRoleIcon;

		[SerializeField]
		private TextMeshProUGUI unitRole;

		[SerializeField]
		private UnitAbilityTag [ ] unitAbilities;

		[SerializeField]
		private TextMeshProUGUI unitSlot;

		[SerializeField]
		private UI.UnitCard [ ] singleSlotCards;

		[SerializeField]
		private UI.UnitCard [ ] doubleSlotCards;

		#endregion // UI Elements

		#region Match Setup Data

		[SerializeField]
		private DebateMenu debateMenu;

		[SerializeField]
		private TeamFormationMenu formationMenu;

		[SerializeField]
		private Menues.Menu [ ] menus;

		public SplashPrompt Splash;
		public Menues.PopUpMenu PopUp;
		public LoadingScreen Load;

		private bool isPaused = false;
		private int playerIndex = 0;
		private UnitSettingData displayedUnit;

		private const string CLASSIC_ICON = "";
		private const string CONTROL_ICON = "";
		private const string RUMBLE_ICON = "";
		private const string INFERNO_ICON = "";
		private const string LEADER_ICON = "";
		private const string OFFENSE_ICON = "";
		private const string DEFENSE_ICON = "";
		private const string SUPPORT_ICON = "";
		private const string GRUNT_ICON = "";

		/// <summary>
		/// Whether or not the game is currently paused.
		/// </summary>
		public bool IsPaused
		{
			get
			{
				// Return value
				return isPaused;
			}
			set
			{
				// Store value
				isPaused = value;
			}
		}

		/// <summary>
		/// The current player up for team selection.
		/// </summary>
		public PlayerSettings CurrentPlayer
		{
			get
			{
				return playerIndex < MatchSettings.Players.Count ? MatchSettings.Players [ playerIndex ] : null;
			}
		}

		#endregion // Match Setup Data

		#region MonoBehaviour Functions

		/// <summary>
		/// Start the team setup menu.
		/// </summary>
		private void Start ( )
		{
			// Display match info
			matchTitle.text = MatchSettings.MatchDebate.EventName;
			switch ( MatchSettings.Type )
			{
			case MatchType.Classic:
			case MatchType.CustomClassic:
				matchTypeIcon.text = CLASSIC_ICON;
				matchType.text += "Classic Match";
				break;
			case MatchType.Control:
			case MatchType.CustomControl:
				matchTypeIcon.text = CONTROL_ICON;
				matchType.text += "Control Match";
				break;
			case MatchType.Rumble:
			case MatchType.CustomRumble:
				matchTypeIcon.text = RUMBLE_ICON;
				matchType.text += "Rumble Match";
				break;
			case MatchType.Inferno:
			case MatchType.CustomInferno:
				matchTypeIcon.text = INFERNO_ICON;
				matchType.text += "Inferno Match";
				break;
			}

			// Begin team selection
			debateMenu.OpenMenu ( );
		}

		/// <summary>
		/// Listens for the pause button being pressed.
		/// </summary>
		private void Update ( )
		{
			// Check for the escape button being pressed
			if ( Input.GetKeyDown ( KeyCode.Escape ) && !PopUp.IsOpen )
			{
				// Check if the game is paused
				if ( IsPaused )
				{
					// Find the current open menu and close it
					foreach ( Menues.Menu m in menus )
					{
						if ( m.IsOpen )
						{
							// Check if the current menu is the base pause menu
							if ( m is Menues.PauseMenu )
								IsPaused = false;

							// Close the menu
							m.CloseMenu ( );
							break;
						}
					}
				}
				else
				{
					// Mark that the game is paused
					IsPaused = true;

					// Open the pause menu
					menus [ 0 ].OpenMenu ( );
				}
			}
		}

		#endregion // MonoBehaviour Functions

		#region Public Functions

		/// <summary>
		/// Displays the instructions for menu in the HUD.
		/// </summary>
		/// <param name="instructions"> The instructions to be displayed. </param>
		public void DisplayInstructions ( string instructions )
		{
			// Display player
			playerName.text = CurrentPlayer.PlayerName;

			// Display instructions
			menuInstructions.text = instructions;
		}

		/// <summary>
		/// Displays a unit in the HUD.
		/// </summary>
		/// <param name="unit"> The unit to be displayed. </param>
		/// <param name="team"> The team color for the unit. </param>
		public void DisplayUnit ( UnitSettingData unit, Match.Player.TeamColor team )
		{
			// Story unit
			displayedUnit = unit;

			// Display unit
			unitPortrait.sprite = unit.Portrait;
			unitPortrait.color = Tools.Util.TeamColor ( team );

			// Display name
			unitName.text = unit.UnitName + "\n<size=60%>" + unit.UnitNickname;

			// Display role
			switch ( unit.Role )
			{
			case UnitData.UnitRole.LEADER:
				unitRoleIcon.text = LEADER_ICON;
				unitRole.text = "Leader";
				break;
			case UnitData.UnitRole.OFFENSE:
				unitRoleIcon.text = OFFENSE_ICON;
				unitRole.text = "Offensive Hero";
				break;
			case UnitData.UnitRole.DEFENSE:
				unitRoleIcon.text = DEFENSE_ICON;
				unitRole.text = "Defensive Hero";
				break;
			case UnitData.UnitRole.SUPPORT:
				unitRoleIcon.text = SUPPORT_ICON;
				unitRole.text = "Supportive Hero";
				break;
			case UnitData.UnitRole.PAWN:
				unitRoleIcon.text = GRUNT_ICON;
				unitRole.text = "Grunt";
				break;
			}

			// Display slots
			unitSlot.text = unit.Slots == 1 ? "1 Slot" : "2 Slots";

			// Display abilities
			DisplayUnitAbility ( 0, unit.Ability1 );
			DisplayUnitAbility ( 1, unit.Ability2 );
			DisplayUnitAbility ( 2, unit.Ability3 );
		}

		/// <summary>
		/// Sets the unit in the lineup.
		/// </summary>
		/// <param name="index"> The index of the card in the lineup. </param>
		/// <param name="unit"> The unit to be displayed. </param>
		/// <param name="team"> The team color for the unit. </param>
		public void SetCardInLineup ( int index, UnitSettingData unit, Player.TeamColor team )
		{
			// Check the slots of the unit
			if ( unit.Slots == 1 )
			{
				// Display unit as single slot
				doubleSlotCards [ index ].IsEnabled = false;
				singleSlotCards [ index ].IsEnabled = true;
				if ( index + 1 < singleSlotCards.Length )
					singleSlotCards [ index + 1 ].IsEnabled = true;

				// Display unit
				singleSlotCards [ index ].SetCardWithUnit ( unit, team );
			}
			else
			{
				// Display unit as double slot
				doubleSlotCards [ index ].IsEnabled = true;
				singleSlotCards [ index ].IsEnabled = false;
				if ( index + 1 < singleSlotCards.Length )
					singleSlotCards [ index + 1 ].IsEnabled = false;

				// Display unit
				doubleSlotCards [ index ].SetCardWithUnit ( unit, team );
			}
		}

		/// <summary>
		/// Highlights a card in the lineup.
		/// </summary>
		/// <param name="index"> The index of the card in the lineup. </param>
		/// <param name="isHighlight"> Whether or not the card should be highlighted. </param>
		/// <param name="team"> The team color for the unit. </param>
		public void HighlightCardInLineup ( int index, bool isHighlight, Player.TeamColor team )
		{
			// Highlight the card
			singleSlotCards [ index ].SetCardHighlight ( isHighlight, team );
			doubleSlotCards [ index ].SetCardHighlight ( isHighlight, team );
		}

		/// <summary>
		/// Resets a single unit card in the lineup.
		/// </summary>
		/// <param name="index"> The index of the card in the lineup. </param>
		public void ResetCardInLineup ( int index )
		{
			// Display unit as single slot
			doubleSlotCards [ index ].IsEnabled = false;
			singleSlotCards [ index ].IsEnabled = true;
			if ( index + 1 < singleSlotCards.Length )
				singleSlotCards [ index + 1 ].IsEnabled = true;

			// Reset card
			singleSlotCards [ index ].SetCardWithoutUnit ( Player.TeamColor.NO_TEAM );
		}

		/// <summary>
		/// Resets all of the unit cards in the lineup.
		/// </summary>
		public void ResetLineup ( )
		{
			// Reset each unit card
			for ( int i = 0; i < singleSlotCards.Length; i++ )
			{
				// Display single cards
				singleSlotCards [ i ].IsEnabled = true;
				doubleSlotCards [ i ].IsEnabled = false;

				// Display empty slot
				singleSlotCards [ i ].SetCardWithoutUnit ( Player.TeamColor.NO_TEAM );
			}
		}

		/// <summary>
		/// Sets the next player for team selection.
		/// Returns false if all players have selected their teams.
		/// </summary>
		public void SetNextPlayer ( )
		{
			// Increment index
			playerIndex++;

			// Check if players remain
			if ( playerIndex < MatchSettings.playerSettings.Count )
			{
				// Display name of current player
				playerName.text = CurrentPlayer.PlayerName;

				// Begin the setup process for player
				formationMenu.CloseMenu ( false );
				debateMenu.OpenMenu ( );
			}
			else
			{
				// Set the units and formation for a mirror match
				if ( MatchSettings.Type == MatchType.Inferno || MatchSettings.Type == MatchType.CustomInferno )
					MatchSettings.SetMirrorUnits ( );

				// Begin match
				BeginMatch ( );
			}
		}

		/// <summary>
		/// Begins the match.
		/// </summary>
		public void BeginMatch ( )
		{
			// Track scene
			Scenes scene = Scenes.CLASSIC;

			// Check match type
			switch ( MatchSettings.Type )
			{
			case MatchType.Classic:
			case MatchType.CustomClassic:
			case MatchType.Inferno:
			case MatchType.CustomInferno:
				scene = Scenes.CLASSIC;
				break;
			case MatchType.Rumble:
			case MatchType.CustomRumble:
				scene = Scenes.RUMBLE;
				break;
			}

			// Load match
			Load.LoadScene ( scene );
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Display an ability of a unit.
		/// </summary>
		/// <param name="index"> The index of the tag. </param>
		/// <param name="ability"> The ability data for the unit. </param>
		private void DisplayUnitAbility ( int index, AbilityData ability )
		{
			// Check for ability
			if ( ability != null )
			{
				// Display tag
				unitAbilities [ index ].Container.SetActive ( true );

				// Display icon
				unitAbilities [ index ].Icon.sprite = ability.Icon;
				unitAbilities [ index ].Icon.color = ability.IsEnabled ? Color.white : Color.grey;

				// Display name
				unitAbilities [ index ].Name.text = ability.AbilityName;
				unitAbilities [ index ].Name.color = ability.IsEnabled ? Color.white : Color.grey;
			}
			else
			{
				// Hide tag
				unitAbilities [ index ].Container.SetActive ( false );
			}
		}

		#endregion // Private Functions
	}
}