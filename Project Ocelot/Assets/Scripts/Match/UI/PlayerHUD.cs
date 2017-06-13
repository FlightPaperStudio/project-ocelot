using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour 
{
	// UI elements
	public TextMeshProUGUI playerName;
	public Image [ ] unitIcons;

	// HUD information
	private Player player;
	private Dictionary<int, int> indexDic = new Dictionary<int, int> ( );

	/// <summary>
	/// Initializes the HUD for a specified player.
	/// </summary>
	public void Initialize ( Player p )
	{
		// Store player
		player = p;

		// Set team name
		playerName.text = p.playerName;
		playerName.color = Util.TeamColor ( p.team );

		// Set special icons
		for ( int i = 0; i < unitIcons.Length; i++ )
		{
			// Set starter icons
			if ( i < p.units.Count )
			{
				// Set icon
				unitIcons [ i ].gameObject.SetActive ( true );
				unitIcons [ i ].sprite = p.units [ i ].displaySprite;
				unitIcons [ i ].color = Util.TeamColor ( p.team );

				// Add unit to the index
				indexDic.Add ( p.units [ i ].instanceID, i );
			}
			else
			{
				// Hide extra icons
				unitIcons [ i ].gameObject.SetActive ( false );
			}
		}
	}

	/// <summary>
	/// Displays the deactivation of a unit by its ID.
	/// </summary>
	public void DisplayDeactivation ( int id )
	{
		// Display deactivation
		unitIcons [ indexDic [ id ] ].color = new Color32 ( 200, 200, 200, 255 );
	}

	/// <summary>
	/// Displays the elimination a player from a match.
	/// </summary>
	public void DisplayElimination ( )
	{
		// Display player elimination
		playerName.color = new Color32 ( 200, 200, 200, 255 );

		// Display deactivation of each special ability
		//foreach ( int id in indexDic.Keys )
		//	DisplayDeactivation ( id );
	}

	/// <summary>
	/// Updates a unit's icon to a new sprite. 
	/// </summary>
	public void UpdateIcon ( int id, Sprite sprite )
	{
		// Change icon
		unitIcons [ indexDic [ id ] ].sprite = sprite;
	}
}
