using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TeamFormationMenu : Menu 
{
	// UI elements
	public Button randomButton;
	public Button unselectButton;
	public TextMeshProUGUI unitName;
	public GameObject [ ] teamPanels;
	public Image [ ] unitPanels;
	public Image [ ] unitIcons;
	public Button selectButton;
	public GameObject unitInfoPanel;
	public GameObject unitDisplayPanel;
	public GameObject confirmPanel;

	// Game objects
	public GameObject teamFormationObjs;
	public SpriteRenderer [ ] tiles;
	public SpriteRenderer [ ] tileOutlines;
	public SpriteRenderer [ ] tileIcons;

	// Player information
	private PlayerSettings player;
	private int specialIndex = 0;

	// Menu information
	public TeamSetup setup;
	private bool canSelect = true;
	private int tileIndex = 0;
	private int randomStartIndex;

	// HACK
	public Sprite [ ] icons;

	/// <summary>
	/// Opens the team formation menu.
	/// Use this for going down a layer (e.g. from a parent menu to a sub menu).
	/// </summary>
	public override void OpenMenu ( bool closeParent = true, params object [ ] values )
	{
		// Open the menu
		base.OpenMenu ( closeParent );
		unitInfoPanel.SetActive ( true );
		unitDisplayPanel.SetActive ( true );
		confirmPanel.SetActive ( false );
		teamFormationObjs.SetActive ( true );

		// Set the player
		player = values [ 0 ] as PlayerSettings;
		specialIndex = 0;
		tileIndex = 0;
		canSelect = true;

		// Disable selection buttons
		selectButton.interactable = false;
		unselectButton.interactable = false;

		// Enable random button
		randomButton.interactable = true;

		// Set tiles
		for ( int i = 0; i < tiles.Length; i++ )
		{
			// Set outline color
			tileOutlines [ i ].color = Util.TeamColor ( player.teamColor );

			// Set icon color
			tileIcons [ i ].color = Util.TeamColor ( player.teamColor );

			// Hide icon
			if ( i != 0 )
				tileIcons [ i ].gameObject.SetActive ( false );
		}

		// Set icons
		for ( int i = 0; i < teamPanels.Length; i++ )
		{
			if ( i < player.specialIDs.Count )
			{
				unitIcons [ i ].sprite = icons [ player.specialIDs [ i ] ];

				unitPanels [ i ].color = new Color32 ( 255, 255, 200, 255 );
				RectTransform rect = unitPanels [ i ].transform as RectTransform;
				rect.offsetMax = Vector2.zero;
				rect.offsetMin = Vector2.zero;
			}
			else
			{
				teamPanels [ i ].SetActive ( false );
			}
		}

		// Set unit for position selection
		SetCurrentUnit ( );

		// Display prompt
		setup.splash.Slide ( "<size=75%>" + player.name + "</size>\n<color=white>Team Formation", Util.TeamColor ( player.teamColor ), true );
	}

	/// <summary>
	/// Sets the current unit that is being positioned.
	/// </summary>
	private void SetCurrentUnit ( )
	{
		// Display name
		unitName.text = SpecialInfo.GetSpecialByID ( player.specialIDs [ specialIndex ] ).name;

		// Display current unit
		unitPanels [ specialIndex ].color = new Color32 ( 255, 210, 75, 255 );
		RectTransform rect = unitPanels [ specialIndex ].transform as RectTransform;
		rect.offsetMax = new Vector2 ( 2.5f, 2.5f );
		rect.offsetMin = new Vector2 ( -2.5f, -2.5f );
	}

	/// <summary>
	/// Highlights an available tile when the mouse starts hovering over the tile.
	/// </summary>
	public void OnTileEnter ( int index )
	{
		// Check if the position is available
		if ( canSelect && player.formation [ index ] == 0 && index != tileIndex )
		{
			// Display icon
			tileIcons [ index ].gameObject.SetActive ( true );
			tileIcons [ index ].sprite = icons [ player.specialIDs [ specialIndex ] ];
		}
	}

	/// <summary>
	/// Unhighlights an available tile when the mouse stops hovering over the tile.
	/// </summary>
	public void OnTileExit ( int index )
	{
		// Check if the position is available
		if ( canSelect && player.formation [ index ] == 0 && index != tileIndex )
		{
			// Display icon
			tileIcons [ index ].gameObject.SetActive ( false );
		}
	}

	/// <summary>
	/// Selects the starting position for the current unit.
	/// </summary>
	public void OnTileClick ( int index )
	{
		// Check if the position is available
		if ( canSelect && player.formation [ index ] == 0 && index != tileIndex )
		{
			// Clear previous tile
			if ( tileIndex != 0 )
			{
				tileIcons [ tileIndex ].gameObject.SetActive ( false );
				tiles [ tileIndex ].color = new Color32 ( 200, 200, 200, 255 );
			}

			// Display icon
			tileIcons [ index ].gameObject.SetActive ( true );
			tileIcons [ index ].sprite = icons [ player.specialIDs [ specialIndex ] ];

			// Highlight the tile
			tiles [ index ].color = new Color32 ( 255, 210, 75, 255 );

			// Store tile
			tileIndex = index;

			// Enable select button
			selectButton.interactable = true;
		}
	}

	/// <summary>
	/// Confirms the starting position for the current unit.
	/// </summary>
	public void SelectPosition ( bool playAnimation = true )
	{
		// Unhighlight tile
		tiles [ tileIndex ].color = new Color32 ( 200, 200, 200, 255 );

		// Highlight unit
		unitPanels [ specialIndex ].color = new Color32 ( 0, 165, 255, 255 );

		// Store position
		player.formation [ tileIndex ] = player.specialIDs [ specialIndex ];

		// Increment index
		specialIndex++;

		// Clear index
		tileIndex = 0;

		// Check if more units need to be positioned
		if ( specialIndex < player.specialIDs.Count )
		{
			// Set next unit
			SetCurrentUnit ( );

			// Enable unselect button
			unselectButton.interactable = true;

			// Disable select button
			selectButton.interactable = false;
		}
		else
		{
			// Disable selection
			canSelect = false;

			// Disable random button
			randomButton.interactable = false;

			// Hide unit controls
			unitInfoPanel.SetActive ( false );
			unitDisplayPanel.SetActive ( false );

			// Create animation
			if ( playAnimation )
			{
				Sequence s = DOTween.Sequence ( );
				for ( int i = 0; i < player.formation.Length; i++ )
				{
					// Display pawns in the formation
					if ( player.formation [ i ] == 0 )
					{
						tileIcons [ i ].gameObject.SetActive ( true );
						tileIcons [ i ].sprite = icons [ 0 ];
						s.AppendInterval ( 0.1f );
						s.Append ( tileIcons [ i ].DOFade ( 0, 0.25f ).From ( ) );
					}
				}
				s.AppendInterval ( 0.1f )
					.OnComplete ( () => 
					{
						// Display confirmation controls
						confirmPanel.SetActive ( true );
					} )
					.Play ( );
			}
			else
			{
				// Display pawns in the formation
				for ( int i = 0; i < player.formation.Length; i++ )
				{
					if ( player.formation [ i ] == 0 )
					{
						tileIcons [ i ].gameObject.SetActive ( true );
						tileIcons [ i ].sprite = icons [ 0 ];
					}
				}

				// Display confirmation controls
				confirmPanel.SetActive ( true );
			}
		}
	}

	/// <summary>
	/// Selects random positions for each unit left in the team.
	/// </summary>
	public void RandomFormation ( )
	{
		// Check index
		if ( specialIndex < player.specialIDs.Count )
		{
			// Store index
			randomStartIndex = specialIndex;
		}
		else
		{
			// Unselect randomly positioned units
			for ( int i = specialIndex; i > randomStartIndex; i-- )
				UnselectPosition ( );
		}

		// Clear index
		if ( tileIndex != 0 )
		{
			tileIcons [ tileIndex ].gameObject.SetActive ( false );
			tiles [ tileIndex ].color = new Color32 ( 200, 200, 200, 255 );
		}
		tileIndex = 0;

		// Randomly select positions for each remaining unit
		while ( specialIndex < player.specialIDs.Count )
		{
			// Get a random position
			int index;
			do 
			{
				index = Random.Range ( 1, player.formation.Length );
			} while ( player.formation [ index ] != 0 );
			OnTileClick ( index );
			SelectPosition ( false );
		}

		// Enable random button
		randomButton.interactable = true;
	}

	/// <summary>
	/// Unselects the position for the last unit that was positioned.
	/// </summary>
	public void UnselectPosition ( )
	{
		// Check if team formation is completed
		if ( specialIndex == player.specialIDs.Count )
		{
			// Display controls
			unitInfoPanel.SetActive ( true );
			unitDisplayPanel.SetActive ( true );
			confirmPanel.SetActive ( false );

			// Remove pawns
			for ( int i = 0; i < player.formation.Length; i++ )
				if ( player.formation [ i ] == 0 )
					tileIcons [ i ].gameObject.SetActive ( false );

			// Enable selection
			canSelect = true;
		}
		else
		{
			// Reset current unit
			unitPanels [ specialIndex ].color = new Color32 ( 255, 255, 210, 255 );
			RectTransform rect = unitPanels [ specialIndex ].transform as RectTransform;
			rect.offsetMax = Vector2.zero;
			rect.offsetMin = Vector2.zero;
		}

		// Decrement index
		specialIndex--;

		// Clear previous tile
		if ( tileIndex != 0 )
		{
			tileIcons [ tileIndex ].gameObject.SetActive ( false );
			tiles [ tileIndex ].color = new Color32 ( 200, 200, 200, 255 );
		}
		tileIndex = 0;

		// Display the unit to be positioned
		SetCurrentUnit ( );

		// Get tile index
		int index = 0;
		for ( int i = 0; i < player.formation.Length; i++ )
		{
			if ( player.formation [ i ] == player.specialIDs [ specialIndex ] )
			{
				index = i;
				break;
			}
		}

		// Remove unit from formation
		player.formation [ index ] = 0;

		// Display the unit as selected
		OnTileClick ( index );

		// Enable random button
		randomButton.interactable = true;

		// Disable select button
		selectButton.interactable = false;

		// Disable unselect button if at the start
		if ( specialIndex == 0 )
			unselectButton.interactable = false;
	}

	public void ConfirmFormation ( )
	{
		// Get next player
		if ( setup.SetNextPlayer ( ) )
		{
			// Close menu and begin the next player's team selection
			teamFormationObjs.SetActive ( false );
			base.CloseMenu ( true, setup.currentPlayer );
		}
		else
		{
			// Load match
			setup.BeginMatch ( );
		}
	}
}
