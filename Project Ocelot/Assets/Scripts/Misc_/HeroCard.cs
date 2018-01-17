using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroCard : MonoBehaviour
{
	#region UI Elements

	public RectTransform container;
	public Image border;
	public GameObject promptPanel;
	public TextMeshProUGUI promptHero;
	public TextMeshProUGUI promptNumber;
	public GameObject heroPanel;
	public TextMeshProUGUI heroName;
	public Image heroIcon;
	public GameObject [ ] heroSlots;
	public GameObject unselectButton;
	public GameObject randomButton;

	#endregion // UI Elements

	#region Card Data

	/// <summary>
	/// The ID of the hero displayed on the card.
	/// </summary>
	public int HeroID
	{
		get;
		private set;
	}

	/// <summary>
	/// Whether or not the prompt panel is included in this card.
	/// </summary>
	private bool IsPromptPresent
	{
		get
		{
			return promptPanel != null && promptHero != null && promptNumber != null;
		}
	}

	private Color32 teamColor;

	public enum CardControls
	{
		NONE,
		UNDO,
		RANDOM
	}

	#endregion // Card Data

	#region Public Functions

	/// <summary>
	/// Sets the current team color for the card.
	/// </summary>
	/// <param name="col"> The team color. </param>
	public void SetTeamColor ( Color32 col )
	{
		// Store the team color
		teamColor = col;

		// Set icon color
		heroIcon.color = teamColor;

		// Set border color
		border.color = teamColor;
	}

	/// <summary>
	/// Hides the card due to there not being enough heros to need the card.
	/// </summary>
	public void HideCard ( )
	{
		// Hide card
		gameObject.SetActive ( false );
	}

	/// <summary>
	/// Displays the card as available, but without a hero currently displayed.
	/// </summary>
	public void DisplayCardWithoutHero ( )
	{
		// Display card
		gameObject.SetActive ( true );

		// Set card to default size
		SetSize ( true );

		// Hide hero display
		heroPanel.SetActive ( false );

		// Set border color
		border.color = teamColor;

		// Check for hero prompt
		if ( IsPromptPresent )
		{
			// Display prompt
			promptPanel.SetActive ( true );

			// Set prompt color as enabled
			promptHero.color = Color.white;
			promptNumber.color = Color.white;
		}
	}

	/// <summary>
	/// Display an available card as disabled due to there not being enough slots remaining.
	/// </summary>
	public void DisableCard ( )
	{
		// Set card to smaller size
		SetSize ( false );

		// Set border color
		border.color = Color.grey;

		// Check for hero prompt
		if ( IsPromptPresent )
		{
			// Display prompt
			promptPanel.SetActive ( true );

			// Set prompt color as disabled
			promptHero.color = Color.grey;
			promptNumber.color = Color.grey;
		}
	}

	/// <summary>
	/// Displays the specified hero in the card.
	/// </summary>
	/// <param name="id"> The ID of the hero to be displayed. </param>
	/// <param name="icon"> The icon of the hero to be displayed. </param>
	public void SetHero ( int id, Sprite icon )
	{
		// Store the id
		HeroID = id;

		// Display hero
		heroPanel.SetActive ( true );
		if ( IsPromptPresent )
			promptPanel.SetActive ( false );

		// Display hero name
		heroName.text = HeroInfo.GetHeroByID ( HeroID ).CharacterName;

		// Display hero icon
		heroIcon.sprite = icon;
		
		// Display the number of slots the hero occupies
		for ( int i = 0; i < heroSlots.Length; i++ )
			heroSlots [ i ].SetActive ( i < HeroInfo.GetHeroByID ( HeroID ).Slots );
	}

	/// <summary>
	/// Sets which card control should be displayed.
	/// </summary>
	/// <param name="control"> The control to be displayed. </param>
	public void SetControls ( CardControls control )
	{
		// Check if the undo button is to be displayed
		if ( unselectButton != null )
			unselectButton.SetActive ( control == CardControls.UNDO );

		// Check if the random button is to be displayed
		if ( randomButton != null )
			randomButton.SetActive ( control == CardControls.RANDOM );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Sets the size of the card.
	/// </summary>
	/// <param name="isDefaultSize"> Whether or not the card should be its default size. </param>
	private void SetSize ( bool isDefaultSize )
	{
		// Check size
		if ( isDefaultSize )
		{
			// Set default size
			container.offsetMax = Vector2.zero;
			container.offsetMin = Vector2.zero;
		}
		else
		{
			// Set smaller size
			container.offsetMax = new Vector2 ( -10f, -10f );
			container.offsetMin = new Vector2 ( 10f, 10f );
		}
	}

	#endregion // Private Functions
}
