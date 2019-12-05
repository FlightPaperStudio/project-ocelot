using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectOcelot.Menues
{
	public class PlayerMenu : Menu
	{
		#region Private Classes

		[System.Serializable]
		private class PlayerCard
		{
			public GameObject Container;
			public Image Background;
			public TextMeshProUGUI Icon;
			public UI.CarouselButton Carousel;

			private bool isActive = true;

			public bool IsActive
			{
				get
				{
					return isActive;
				}
				set
				{
					isActive = value;

					Container.SetActive ( isActive );
				}
			}

			public bool IsPlayer
			{
				get
				{
					return Carousel.IsOptionTrue;
				}
				set
				{
					// Set carousel
					Carousel.SetOption ( value, false );
				}
			}
		}

		#endregion // Private Classes

		#region UI Elements

		[SerializeField]
		private TextMeshProUGUI modeText;

		[SerializeField]
		private PlayerCard [ ] cards;

		[SerializeField]
		private LoadingScreen load;

		#endregion // UI Elements

		#region Menu Data

		[SerializeField]
		private string playerIcon;

		[SerializeField]
		private string cpuIcon;

		[SerializeField]
		private Color32 playerColor;

		[SerializeField]
		private Color32 cpuColor;

		#endregion // Menu Data

		#region Menu Override Functions

		public override void OpenMenu ( bool closeParent = true )
		{
			// Open the menu
			base.OpenMenu ( closeParent );

			// Set title
			switch ( Match.MatchSettings.Type )
			{
			case Match.MatchType.Classic:
			case Match.MatchType.CustomClassic:
				modeText.text = "Quickplay\n<size=60%>Classic Match";
				break;
			case Match.MatchType.Control:
			case Match.MatchType.CustomControl:
				modeText.text = "Quickplay\n<size=60%>Control Match";
				break;
			case Match.MatchType.Rumble:
			case Match.MatchType.CustomRumble:
				modeText.text = "Quickplay\n<size=60%>Rumble Match";
				break;
			case Match.MatchType.Inferno:
			case Match.MatchType.CustomInferno:
				modeText.text = "Quickplay\n<size=60%>Inferno Match";
				break;
			}

			// Set players
			for ( int i = 0; i < cards.Length; i++ )
			{
				// Display or hide card
				cards [ i ].IsActive = i < Match.MatchSettings.Players.Count;

				// Set as player or cpu
				if ( i < Match.MatchSettings.Players.Count )
				{
					// Set additional players as cpus by default
					cards [ i ].IsPlayer = i == 0;
					SetCardDisplay ( cards [ i ] );
				}
			}
		}

		#endregion // Menu Override Functions

		#region Public Functions

		/// <summary>
		/// Updates the display of the card. 
		/// </summary>
		/// <param name="index"> The index of the card. </param>
		public void OnOptionChange ( int index )
		{
			// Update display
			SetCardDisplay ( cards [ index ] );
		}

		/// <summary>
		/// Confirms the players.
		/// </summary>
		public void ConfirmPlayers ( )
		{
			// Begin loading
			load.BeginLoad ( );

			// Set each player
			for ( int i = 0; i < Match.MatchSettings.Players.Count; i++ )
				Match.MatchSettings.Players [ i ].Control = cards [ i ].IsPlayer ? Match.Player.PlayerControl.LOCAL_PLAYER : Match.Player.PlayerControl.LOCAL_BOT;

			// Load setup
			load.LoadScene ( Scenes.MATCH_SETUP );
		}

		#endregion // Public Functions

		#region Private Functions

		/// <summary>
		/// Sets the visuals of the card.
		/// </summary>
		/// <param name="card"> The card being updated. </param>
		private void SetCardDisplay ( PlayerCard card )
		{
			// Set color
			card.Background.color = card.IsPlayer ? playerColor : cpuColor;

			// Set icon
			card.Icon.text = card.IsPlayer ? playerIcon : cpuIcon;
		}

		#endregion // Private Functions
	}
}