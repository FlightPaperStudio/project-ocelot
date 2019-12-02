using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMenu : Menu
{
	#region Private Classes

	[System.Serializable]
	private class PlayerCard
	{
		public GameObject Container;
		public Image Background;
		public Image Icon;
		public CarouselButton Carousel;

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

	#endregion // UI Elements

	#region Menu Data

	[SerializeField]
	private Sprite playerIcon;

	[SerializeField]
	private Sprite cpuIcon;

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
		switch ( MatchSettings.Type )
		{
		case MatchType.Classic:
		case MatchType.CustomClassic:
			modeText.text = "Quick Play - Classic Match";
			break;
		case MatchType.Rumble:
		case MatchType.CustomRumble:
			modeText.text = "Quick Play - Rumble Match";
			break;
		}

		// Set players
		for ( int i = 0; i < cards.Length; i++ )
		{
			// Display or hide card
			cards [ i ].IsActive = i < MatchSettings.Players.Count;

			// Set as player or cpu
			if ( i < MatchSettings.Players.Count )
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
		// Set each player
		for ( int i = 0; i < MatchSettings.Players.Count; i++ )
			MatchSettings.Players [ i ].Control = cards [ i ].IsPlayer ? Player.PlayerControl.LOCAL_PLAYER : Player.PlayerControl.LOCAL_BOT;
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
		card.Icon.sprite = card.IsPlayer ? playerIcon : cpuIcon;
	}

	#endregion // Private Functions
}
