using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectOcelot.Match.Setup
{
	public class TeamSelectionMenu : Menues.Menu
	{
		#region Private Classes

		[System.Serializable]
		private class HeroPortraits
		{
			public UI.UnitPortrait Portrait;
			public int UnitID;
		}

		#endregion // Private Classes

		#region UI Elements

		[SerializeField]
		private HeroPortraits [ ] heroes;

		[SerializeField]
		private UI.UnitPortrait [ ] grunts;

		[SerializeField]
		private TextMeshProUGUI heroLimit;

		[SerializeField]
		private Button undoButton;

		[SerializeField]
		private Button randomButton;

		[SerializeField]
		private Button selectButton;

		#endregion // UI Elements

		#region Menu Data

		[SerializeField]
		private TeamSetup setupManager;

		[SerializeField]
		private Menues.Menu teamFormationMenu;

		[SerializeField]
		private Menues.PopUpMenu popup;

		private UnitSettingData [ ] gruntData;
		private UnitSettingData selectedUnit;
		private int lineupStart = 0;
		private int lineupCounter = 0;
		private int heroLimitCounter = 0;

		private const float PORTRAIT_OFFSET = 5f;

		#endregion // Menu Data

		#region Menu Override Functions

		/// <summary>
		/// Opens the team selection menu.
		/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
		/// </summary>
		public override void OpenMenu ( bool closeParent = true )
		{
			// Open the menu
			base.OpenMenu ( closeParent );

			// Set instructions
			setupManager.DisplayInstructions ( "Select your lineup" );

			// Display hero limit
			lineupStart = setupManager.CurrentPlayer.Units.Count;
			lineupCounter = lineupStart;
			heroLimitCounter = 0;
			DisplayHeroLimit ( heroLimitCounter );

			// Display heroes
			for ( int i = 0; i < heroes.Length; i++ )
			{
				// Display portrait
				heroes [ i ].Portrait.SetPortrait ( MatchSettings.GetHero ( heroes [ i ].UnitID ), setupManager.CurrentPlayer.Team );
				heroes [ i ].Portrait.IsEnabled = MatchSettings.GetHero ( heroes [ i ].UnitID ).IsEnabled;
			}

			// Initialize grunts
			gruntData = new UnitSettingData [ grunts.Length ];
			for ( int i = 0; i < grunts.Length; i++ )
			{
				// Generate grunt
				gruntData [ i ] = MatchSettings.GetPawn ( );

				// Display grunt
				grunts [ i ].SetPortrait ( gruntData [ i ], setupManager.CurrentPlayer.Team );
			}

			// Enable buttons
			randomButton.interactable = true;
			selectButton.interactable = true;

			// Start by selecting a random hero
			SelectRandomUnit ( );

			// Display prompt
			setupManager.Splash.Slide ( "<size=75%>" + setupManager.CurrentPlayer.PlayerName + "</size>\n<color=white>Team Selection", Tools.Util.TeamColor ( setupManager.CurrentPlayer.Team ), true );
		}

		#endregion // Menu Override Functions

		#region Public Functions

		/// <summary>
		/// Previews a hero on mouse enter of the hero's portrait.
		/// </summary>
		/// <param name="index"> The index of the hero. </param>
		public void MouseEnterHero ( int index )
		{
			// Check if hero is available
			if ( heroes [ index ].Portrait.IsEnabled && heroes [ index ].Portrait.IsAvailable )
			{
				// Display hero
				DisplayUnit ( MatchSettings.GetHero ( heroes [ index ].UnitID ) );
			}
		}

		/// <summary>
		/// Previews a grunt on mouse enter of the grunt's portrait.
		/// </summary>
		/// <param name="index"> The index of the grunt. </param>
		public void MouseEnterGrunt ( int index )
		{
			// Check if grunt is available
			if ( grunts [ index ].IsEnabled && grunts [ index ].IsAvailable )
			{
				// Display grunt
				DisplayUnit ( gruntData [ index ] );
			}
		}

		/// <summary>
		/// Ends preview of hero on mouse exit of the hero's portrait.
		/// </summary>
		/// <param name="index"> The index of the hero. </param>
		public void MouseExitHero ( int index )
		{
			// Check if hero is available
			if ( heroes [ index ].Portrait.IsEnabled && heroes [ index ].Portrait.IsAvailable && selectedUnit.ID != heroes [ index ].UnitID )
			{
				// Reset portrait
				heroes [ index ].Portrait.ResetSize ( );

				// Display selected unit
				DisplayUnit ( selectedUnit );
			}
		}

		/// <summary>
		/// Ends preview of grunt on mouse exit of the grunt's portrait.
		/// </summary>
		/// <param name="index"> The index of the grunt. </param>
		public void MouseExitGrunt ( int index )
		{
			// Check if grunt is available
			if ( grunts [ index ].IsEnabled && grunts [ index ].IsAvailable && selectedUnit != gruntData [ index ] )
			{
				// Reset portrait
				grunts [ index ].ResetSize ( );

				// Display selected unit
				DisplayUnit ( selectedUnit );
			}
		}

		/// <summary>
		/// Selects a hero to be potentially added to the team.
		/// </summary>
		/// <param name="index"> The index of the hero. </param>
		public void SelectHero ( int index )
		{
			// Check if hero is available
			if ( heroes [ index ].Portrait.IsEnabled && heroes [ index ].Portrait.IsAvailable && selectedUnit.ID != heroes [ index ].UnitID )
			{
				// Reset previous selection
				if ( selectedUnit != null )
				{
					// Check for grunt
					if ( selectedUnit.Role == UnitData.UnitRole.PAWN )
					{
						// Get portrait
						UI.UnitPortrait previousGrunt = GetGruntPortrait ( selectedUnit );

						// Reset portrait
						previousGrunt.ResetSize ( );
						previousGrunt.IsBorderHighlighted = false;
					}
					else
					{
						// Get portrait
						HeroPortraits previousHero = GetHeroPortrait ( selectedUnit );

						// Reset portrait
						previousHero.Portrait.ResetSize ( );
						previousHero.Portrait.IsBorderHighlighted = false;
					}
				}

				// Store unit
				selectedUnit = MatchSettings.GetHero ( heroes [ index ].UnitID );

				// Display selected unit
				DisplayUnit ( selectedUnit );

				// Highlight portrait
				heroes [ index ].Portrait.IsBorderHighlighted = true;
			}
		}

		/// <summary>
		/// Selects a grunt to be potentially added to the team.
		/// </summary>
		/// <param name="index"> The index of the grunt. </param>
		public void SelectGrunt ( int index )
		{
			// Check if grunt is available
			if ( grunts [ index ].IsEnabled && grunts [ index ].IsAvailable && selectedUnit != gruntData [ index ] )
			{
				// Reset previous selection
				if ( selectedUnit != null )
				{
					// Check for grunt
					if ( selectedUnit.Role == UnitData.UnitRole.PAWN )
					{
						// Get portrait
						UI.UnitPortrait previousGrunt = GetGruntPortrait ( selectedUnit );

						// Reset portrait
						previousGrunt.ResetSize ( );
						previousGrunt.IsBorderHighlighted = false;
					}
					else
					{
						// Get portrait
						HeroPortraits previousHero = GetHeroPortrait ( selectedUnit );

						// Reset portrait
						previousHero.Portrait.ResetSize ( );
						previousHero.Portrait.IsBorderHighlighted = false;
					}
				}

				// Store unit
				selectedUnit = gruntData [ index ];

				// Display selected unit
				DisplayUnit ( selectedUnit );

				// Highlight portrait
				grunts [ index ].IsBorderHighlighted = true;
			}
		}

		/// <summary>
		/// Undoes the last unit confirmation.
		/// </summary>
		public void Undo ( )
		{
			// Check if currently selected unit is a hero
			if ( selectedUnit != null && selectedUnit.Role != UnitData.UnitRole.PAWN )
			{
				// Reset portrait
				HeroPortraits currentHero = GetHeroPortrait ( selectedUnit );
				currentHero.Portrait.ResetSize ( );
				currentHero.Portrait.IsBorderHighlighted = false;
			}
			else
			{
				// Reset portrait
				UI.UnitPortrait currentGrunt = GetGruntPortrait ( selectedUnit );
				currentGrunt.ResetSize ( );
				currentGrunt.IsBorderHighlighted = false;
			}

			// Get last unit
			selectedUnit = setupManager.CurrentPlayer.Units [ setupManager.CurrentPlayer.Units.Count - 1 ];

			// Remove last unit from lineup
			setupManager.CurrentPlayer.Units.Remove ( selectedUnit );
			lineupCounter--;

			// Check if last unit is a hero
			if ( selectedUnit.Role != UnitData.UnitRole.PAWN )
			{
				// Update hero limit
				heroLimitCounter--;
				DisplayHeroLimit ( heroLimitCounter );
			}

			// Enable heroes
			for ( int i = 0; i < heroes.Length; i++ )
			{
				// Check if the hero was enabled for the match
				if ( !heroes [ i ].Portrait.IsEnabled )
					continue;

				// Check if the hero is on the team
				if ( MatchSettings.HeroLimit && setupManager.CurrentPlayer.Units.Contains ( MatchSettings.GetHero ( heroes [ i ].UnitID ) ) )
					continue;

				// Check if 2 slot heroes can be selected
				if ( lineupCounter == MatchSettings.TEAM_SIZE - 1 && MatchSettings.GetHero ( heroes [ i ].UnitID ).Slots > 1 )
					continue;

				// Enable hero
				heroes [ i ].Portrait.IsAvailable = true;
			}

			// Enable grunts
			for ( int i = 0; i < grunts.Length; i++ )
			{
				// Check if the hero is on the team
				if ( MatchSettings.HeroLimit && setupManager.CurrentPlayer.Units.Contains ( gruntData [ i ] ) )
					continue;

				// Enable grunt
				grunts [ i ].IsAvailable = true;
			}

			// Display last unit
			DisplayUnit ( selectedUnit );

			// Highlight portrait
			if ( selectedUnit.Role != UnitData.UnitRole.PAWN )
				GetHeroPortrait ( selectedUnit ).Portrait.IsBorderHighlighted = true;
			else
				GetGruntPortrait ( selectedUnit ).IsBorderHighlighted = true;

			// Enable selection buttons
			randomButton.interactable = true;
			selectButton.interactable = true;

			// Enable undo button if not back at start
			undoButton.interactable = lineupCounter > lineupStart;
		}

		/// <summary>
		/// Selects an available unit at random.
		/// </summary>
		public void SelectRandomUnit ( )
		{
			// Get available heroes
			List<HeroPortraits> availableHeroes = new List<HeroPortraits> ( );
			for ( int i = 0; i < heroes.Length; i++ )
				if ( heroes [ i ].Portrait.IsEnabled && heroes [ i ].Portrait.IsAvailable )
					availableHeroes.Add ( heroes [ i ] );

			// Get available grunts
			List<UI.UnitPortrait> availableGrunts = new List<UI.UnitPortrait> ( );
			for ( int i = 0; i < grunts.Length; i++ )
				if ( grunts [ i ].IsEnabled && grunts [ i ].IsAvailable )
					availableGrunts.Add ( grunts [ i ] );

			// Get a random selection
			int rng = Random.Range ( 0, availableHeroes.Count + availableGrunts.Count );

			// Check if a hero was selected
			if ( rng < availableHeroes.Count )
				SelectHero ( System.Array.IndexOf ( heroes, availableHeroes [ rng ] ) );
			else
				SelectGrunt ( System.Array.IndexOf ( grunts, availableGrunts [ rng - availableHeroes.Count ] ) );
		}

		/// <summary>
		/// Confirms the unit to be added to the team.
		/// </summary>
		public void ConfirmUnit ( )
		{
			// Add unit to lineup
			setupManager.CurrentPlayer.Units.Add ( selectedUnit );
			lineupCounter += selectedUnit.Slots;

			// Check if added unit is a hero
			if ( selectedUnit.Role != UnitData.UnitRole.PAWN )
			{
				// Reset portrait
				HeroPortraits previousHero = GetHeroPortrait ( selectedUnit );
				previousHero.Portrait.ResetSize ( );
				previousHero.Portrait.IsBorderHighlighted = false;

				// Make unit unavailable if cloning is not enabled
				previousHero.Portrait.IsAvailable = !MatchSettings.HeroLimit;

				// Increment hero limit
				heroLimitCounter++;
				DisplayHeroLimit ( heroLimitCounter );

				// Check if the hero limit has been reached
				if ( heroLimitCounter == MatchSettings.HeroesPerTeam )
					for ( int i = 0; i < heroes.Length; i++ )
						if ( heroes [ i ].Portrait.IsEnabled )
							heroes [ i ].Portrait.IsAvailable = false;
			}
			else
			{
				// Reset portrait
				UI.UnitPortrait previousGrunt = GetGruntPortrait ( selectedUnit );
				previousGrunt.ResetSize ( );
				previousGrunt.IsBorderHighlighted = false;

				// Make unit unavailable if cloning is not enabled
				previousGrunt.IsAvailable = !MatchSettings.HeroLimit;
			}

			// Clear selected unit
			selectedUnit = null;

			// Enable undo
			undoButton.interactable = true;

			// Check remaining slots
			if ( lineupCounter == MatchSettings.TEAM_SIZE )
			{
				// Disable buttons
				randomButton.interactable = false;
				selectButton.interactable = false;

				// Disable all units
				for ( int i = 0; i < heroes.Length; i++ )
					if ( heroes [ i ].Portrait.IsEnabled )
						heroes [ i ].Portrait.IsAvailable = false;
				for ( int i = 0; i < grunts.Length; i++ )
					if ( grunts [ i ].IsEnabled )
						grunts [ i ].IsAvailable = false;
			}
			else if ( lineupCounter == MatchSettings.TEAM_SIZE - 1 )
			{
				// Disable 2 slot heroes
				for ( int i = 0; i < heroes.Length; i++ )
					if ( heroes [ i ].Portrait.IsEnabled && MatchSettings.GetHero ( heroes [ i ].UnitID ).Slots > 1 )
						heroes [ i ].Portrait.IsAvailable = false;

				// Select another unit
				SelectRandomUnit ( );
			}
			else
			{
				// Select another unit
				SelectRandomUnit ( );
			}
		}

		/// <summary>
		/// Confirms the team lineup selected.
		/// </summary>
		public void ConfirmLineup ( )
		{
			// Check for full lineup
			if ( lineupCounter < MatchSettings.TEAM_SIZE )
			{
				// Open popup
				popup.SetConfirmationPopUp ( "Confirm lineup?\n<size=75%>(Your lineup is not full yet!)",
											 ( confirm ) =>
											 {
												 // Continue to team formation menu
												 if ( confirm )
													 teamFormationMenu.OpenMenu ( );
											 } );
			}
			else
			{
				// Open the team formation menu
				teamFormationMenu.OpenMenu ( );
			}
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Displays the hero limit prompt.
		/// </summary>
		/// <param name="currentHeroes"> The number of heroes current selected. </param>
		private void DisplayHeroLimit ( int currentHeroes )
		{
			// Display prompt
			heroLimit.text = "Hero Limit: ";

			// Change color for max
			if ( currentHeroes == MatchSettings.HeroesPerTeam )
				heroLimit.text += "<color=#FFD24B>";

			// Display limit
			heroLimit.text += currentHeroes + " / " + MatchSettings.HeroesPerTeam;
		}

		/// <summary>
		/// Gets the corrisponding portrait for a hero.
		/// </summary>
		/// <param name="unit"> The data for the hero. </param>
		/// <returns> The hero's portrait. </returns>
		private HeroPortraits GetHeroPortrait ( UnitSettingData unit )
		{
			return heroes.First ( x => x.UnitID == unit.ID );
		}

		/// <summary>
		/// Gets the corrisponding portrait for a grunt.
		/// </summary>
		/// <param name="unit"> The data for the grunt. </param>
		/// <returns> The grunt's portrait. </returns>
		private UI.UnitPortrait GetGruntPortrait ( UnitSettingData unit )
		{
			return grunts [ System.Array.IndexOf ( gruntData, unit ) ];
		}

		/// <summary>
		/// Previews a unit in the HUD.
		/// </summary>
		/// <param name="unit"> The data for the unit. </param>
		private void DisplayUnit ( UnitSettingData unit )
		{
			// Increase the size of the portrait
			if ( unit.Role == UnitData.UnitRole.PAWN )
				GetGruntPortrait ( unit ).ChangeSize ( PORTRAIT_OFFSET );
			else
				GetHeroPortrait ( unit ).Portrait.ChangeSize ( PORTRAIT_OFFSET );

			// Display unit
			setupManager.DisplayUnit ( unit, setupManager.CurrentPlayer.Team );

			// Update lineup
			setupManager.SetCardInLineup ( lineupCounter, unit, setupManager.CurrentPlayer.Team );
		}

		#endregion // Private Functions
	}
}