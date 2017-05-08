using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TeamSelectionMenu : Menu 
{
	// UI elements
	public TextMeshProUGUI teamName;
	public Image [ ] teamIcons;
	public Button randomButton;
	public Button unselectButton;
	public TextMeshProUGUI unitName;
	public Button [ ] unitButtons;
	public GameObject unitInfoPanel;
	public GameObject unitSelectionPanel;
	public GameObject confirmPanel;

	// Game objects
	public GameObject teamSelectionObjs;
	public SpriteRenderer currentUnitSprite;
	public SpriteRenderer [ ] teamDisplaySprites;

	// Player information
	private PlayerSettings player;
	private int specialIndex = 0;

	// Menu information
	public TeamSetup setup;
	public Menu teamFormation;
	private int selectedSpecialID;
	private int randomStartIndex;

	// UI information
	private ColorBlock selected = new ColorBlock ( );
	private ColorBlock unselected = new ColorBlock ( );
	private ColorBlock inactive = new ColorBlock ( );

	// HACK
	public Sprite [ ] icons;

	/// <summary>
	/// Opens the team selection menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true, params object [ ] values )
	{
		// Open the menu
		base.OpenMenu ( closeParent );
		unitInfoPanel.SetActive ( true );
		unitSelectionPanel.SetActive ( true );
		confirmPanel.SetActive ( false );
		teamSelectionObjs.SetActive ( true );
		foreach ( SpriteRenderer s in teamDisplaySprites )
			if ( s != currentUnitSprite )
				s.gameObject.SetActive ( false );

		// Set the player
		player = values [ 0 ] as PlayerSettings;
		specialIndex = 0;

		// Display team name
		teamName.text = player.name;
		teamName.color = Util.TeamColor ( player.teamColor );

		// Hide unit icons
		for ( int i = 0; i < teamIcons.Length; i++ )
		{
			if ( i < player.specialIDs.Count )
				teamIcons [ i ].color = new Color32 ( 255, 255, 255, 0 );
			else
				teamIcons [ i ].gameObject.SetActive ( false );
		}

		// Set colors
		selected.normalColor =        new Color32 ( 255, 210,  75, 255 );
		selected.highlightedColor =   new Color32 ( 255, 210,  75, 255 );
		selected.pressedColor =       new Color32 ( 255, 210,  75, 255 );
		selected.disabledColor =      new Color32 (   0, 165, 255, 255 );
		selected.colorMultiplier = 1;
		unselected.normalColor =      new Color32 ( 255, 255, 200, 255 );
		unselected.highlightedColor = new Color32 ( 255, 210,  75, 255 );
		unselected.pressedColor =     new Color32 ( 255, 210,  75, 255 );
		unselected.disabledColor =    new Color32 (   0, 165, 255, 255 );
		unselected.colorMultiplier = 1;
		inactive.normalColor =        new Color32 (   0,   0,   0,   0 );
		inactive.highlightedColor =   new Color32 (   0,   0,   0,   0 );
		inactive.pressedColor =       new Color32 (   0,   0,   0,   0 );
		inactive.disabledColor =      new Color32 (   0,   0,   0,   0 );
		inactive.colorMultiplier = 1;

		// Start unit selection from the beginning
		randomButton.interactable = true;
		unselectButton.interactable = false;
		for ( int i = 0; i < unitButtons.Length; i++ )
		{
			if ( MatchSettings.heroSettings [ i ].selection )
			{
				unitButtons [ i ].interactable = true;
				unitButtons [ i ].colors = unselected;
			}
			else
			{
				unitButtons [ i ].interactable = false;
				unitButtons [ i ].colors = inactive;
			}
			RectTransform rect = unitButtons [ i ].transform as RectTransform;
			rect.offsetMax = Vector2.zero;
			rect.offsetMin = Vector2.zero;
		}
		SelectRandomUnit ( );

		// Display prompt
		setup.splash.Slide ( "<size=75%>" + player.name + "</size>\n<color=white>Team Selection", Util.TeamColor ( player.teamColor ), true );
	}

	/// <summary>
	/// Selects the unit for display and potential addition to the team.
	/// </summary>
	public void SelectUnit ( int index )
	{
		// Display the button as selected
		for ( int i = 0; i < unitButtons.Length; i++ )
		{
			if ( i == index )
			{
				unitButtons [ i ].colors = selected;
				RectTransform rect = unitButtons [ i ].transform as RectTransform;
				rect.offsetMax = new Vector2 ( 2.5f, 2.5f );
				rect.offsetMin = new Vector2 ( -2.5f, -2.5f );
			}
			else
			{
				// Set button as unselected
				if ( unitButtons [ i ].interactable )
				{
					unitButtons [ i ].colors = unselected;
					RectTransform rect = unitButtons [ i ].transform as RectTransform;
					rect.offsetMax = Vector2.zero;
					rect.offsetMin = Vector2.zero;
				}
			}
		}

		// Store special ID
		selectedSpecialID = index + 1;

		// Display unit name
		unitName.text = HeroInfo.GetHeroByID ( selectedSpecialID ).characterName;

		// Display unit
		currentUnitSprite.sprite = icons [ index ];
		currentUnitSprite.color = Util.TeamColor ( player.teamColor );
	}

	/// <summary>
	/// Confirms the unit to be added to the team.
	/// </summary>
	public void ConfirmUnit ( )
	{
		// Add unit to the team
		player.specialIDs [ specialIndex ] = selectedSpecialID;

		// Disable button
		if ( !MatchSettings.stacking )
			unitButtons [ selectedSpecialID - 1 ].interactable = false;

		// Display the added unit
		teamIcons [ specialIndex ].sprite = icons [ selectedSpecialID - 1 ];
		teamIcons [ specialIndex ].color = Util.TeamColor ( player.teamColor );

		// Activate undo button
		unselectButton.interactable = true;

		// Increment index
		specialIndex++;

		// Check if team selection is complete
		if ( specialIndex < player.specialIDs.Count )
		{
			// Continue team selection
			SelectRandomUnit ( );
		}
		else
		{
			// Display confirmation
			unitInfoPanel.SetActive ( false );
			unitSelectionPanel.SetActive ( false );
			confirmPanel.SetActive ( true );

			// Disable random team button
			randomButton.interactable = false;

			// Display team
			for ( int i = 0; i < player.specialIDs.Count; i++ )
			{
				teamDisplaySprites [ i ].gameObject.SetActive ( true );
				teamDisplaySprites [ i ].sprite = icons [ player.specialIDs [ i ] - 1 ];
				teamDisplaySprites [ i ].color = Util.TeamColor ( player.teamColor );
			}
		}
	}

	/// <summary>
	/// Selects random units to fill out the rest of the team.
	/// </summary>
	public void RandomTeam ( )
	{
		// Check index
		if ( specialIndex < player.specialIDs.Count )
		{
			// Store index
			randomStartIndex = specialIndex;
		}
		else
		{
			// Unselect the randomly selected units
			for ( int i = specialIndex; i > randomStartIndex; i-- )
				UnselectUnit ( );
		}

		// Randomly select the first unit
		SelectRandomUnit ( );
		ConfirmUnit ( );

		// Randomly select any remaining units
		while ( specialIndex < player.specialIDs.Count )
			ConfirmUnit ( );

		// Enable random team button for random reassignment
		randomButton.interactable = true;
	}

	/// <summary>
	/// Removes the last unit added to the team.
	/// </summary>
	public void UnselectUnit ( )
	{
		// Check if team is full
		if ( specialIndex == player.specialIDs.Count )
		{
			// Display selection controls
			unitInfoPanel.SetActive ( true );
			unitSelectionPanel.SetActive ( true );

			// Hide confirmation controls
			confirmPanel.SetActive ( false );
			foreach ( SpriteRenderer s in teamDisplaySprites )
				if ( s != currentUnitSprite )
					s.gameObject.SetActive ( false );
		}

		// Decrement index
		specialIndex--;

		// Remove icon from team
		teamIcons [ specialIndex ].color = new Color32 ( 255, 255, 255, 0 );

		// Enable button
		unitButtons [ player.specialIDs [ specialIndex ] - 1 ].interactable = true;

		// Set removed unit to be currently selected
		SelectUnit ( player.specialIDs [ specialIndex ] - 1 );

		// Disable undo button if team is empty
		if ( specialIndex == 0 )
			unselectButton.interactable = false;

		// Enable random team button
		randomButton.interactable = true;
	}

	/// <summary>
	/// Confirms the team selection.
	/// </summary>
	public void ConfirmTeam ( )
	{
		// Hide game objects
		teamSelectionObjs.SetActive ( false );

		// Open the team formation menu
		teamFormation.OpenMenu ( true, player );
	}

	/// <summary>
	/// Selects an active unit at random.
	/// </summary>
	private void SelectRandomUnit ( )
	{
		// Create list of available units
		List<int> list = new List<int> ( );
		for ( int i = 0; i < unitButtons.Length; i++ )
			if ( unitButtons [ i ].interactable )
				list.Add ( i );

		// Select random index
		int index = Random.Range ( 0, list.Count );

		// Select unit
		SelectUnit ( list [ index ] );
	}
}
