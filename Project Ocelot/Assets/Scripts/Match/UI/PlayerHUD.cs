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
	public TextMeshProUGUI [ ] cooldownCounters;

	// HACK
	public Sprite [ ] icons;

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
				specialIcons [ i ].sprite = icons [ p.specialIDs [ i ] - 1 ];
				specialIcons [ i ].color = Util.TeamColor ( p.team );
				foreach ( Unit u in p.units )
				{
					if ( u is SpecialUnit )
					{
						SpecialUnit s = u as SpecialUnit;
						if ( s.specialID == p.specialIDs [ i ] && !indexDic.ContainsKey ( s.instanceID ) )
						{
							indexDic.Add ( s.instanceID, i );
							break;
						}
					}
				}

				// Hide cooldown counter
				cooldownCounters [ i ].gameObject.SetActive ( false );
			}
			else
			{
				// Hide extra icons
				specialIcons [ i ].gameObject.SetActive ( false );
			}
		}
	}

	/// <summary>
	/// Displays the cooldown counter for specified special ability by its ID.
	/// </summary>
	public void DisplayCooldown ( int id, int cooldown )
	{
		// Check cooldown
		if ( cooldown > 0 )
		{
			// Show that special ability is on cooldown
			specialIcons [ indexDic [ id ] ].color = new Color32 ( 200, 200, 200, 255 );

			// Display cooldown
			cooldownCounters [ indexDic [ id ] ].gameObject.SetActive ( true );
			cooldownCounters [ indexDic [ id ] ].text = cooldown.ToString ( );
		}
		else
		{
			// Show that special ability is no longer on cooldown
			specialIcons [ indexDic [ id ] ].color = Util.TeamColor ( player.team );

			// Hide cooldown
			cooldownCounters [ indexDic [ id ] ].gameObject.SetActive ( false );
		}
	}

	/// <summary>
	/// Displays the deactivation of a special ability by its ID.
	/// </summary>
	public void DisplayDeactivation ( int id )
	{
		// Display deactivation
		specialIcons [ indexDic [ id ] ].color = new Color32 ( 200, 200, 200, 255 );

		// Hide cooldown
		cooldownCounters [ indexDic [ id ] ].gameObject.SetActive ( false );
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
