using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour 
{
	// UI elements
	public TextMeshProUGUI playerName;
	public Image [ ] specialIcons;

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
		playerName.text = p.name;
		playerName.color = Util.TeamColor ( p.team );

		// Set special icons
		for ( int i = 0; i < specialIcons.Length; i++ )
		{
			// Set starter icons
			if ( i < p.specialIDs.Length )
			{
				// Set icon
				specialIcons [ i ].gameObject.SetActive ( true );
				specialIcons [ i ].color = Util.TeamColor ( p.team );
				foreach ( Unit u in p.units )
				{
					if ( u is HeroUnit )
					{
						HeroUnit h = u as HeroUnit;
						if ( h.heroID == p.specialIDs [ i ] && !indexDic.ContainsKey ( h.instanceID ) )
						{
							specialIcons [ i ].sprite = h.displaySprite;
							indexDic.Add ( h.instanceID, i );
							break;
						}
					}
				}
			}
			else
			{
				// Hide extra icons
				specialIcons [ i ].gameObject.SetActive ( false );
			}
		}
	}

	/// <summary>
	/// Displays the deactivation of a special ability by its ID.
	/// </summary>
	public void DisplayDeactivation ( int id )
	{
		// Display deactivation
		specialIcons [ indexDic [ id ] ].color = new Color32 ( 200, 200, 200, 255 );
	}

	/// <summary>
	/// Displays the elimination a player from a match.
	/// </summary>
	public void DisplayElimination ( )
	{
		// Display player elimination
		playerName.color = new Color32 ( 200, 200, 200, 255 );

		// Display deactivation of each special ability
		foreach ( int id in indexDic.Keys )
			DisplayDeactivation ( id );
	}
}
