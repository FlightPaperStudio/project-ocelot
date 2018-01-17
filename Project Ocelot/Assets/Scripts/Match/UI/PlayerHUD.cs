using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour 
{
	#region UI Elements

	[System.Serializable]
	public struct StatusEffectsBar
	{
		public Image [ ] statusIcons;
	}

	public TextMeshProUGUI playerName;
	public UnitPortrait [ ] unitPortraits;
	public StatusEffectsBar [ ] statusEffects;

	#endregion // UI Elements

	#region HUD Data

	/// <summary>
	/// The player associated with this Player HUD
	/// </summary>
	public Player Player
	{
		get;
		private set;
	}

	private int [ ] unitIDs = { MatchSettings.NO_UNIT, MatchSettings.NO_UNIT, MatchSettings.NO_UNIT, MatchSettings.NO_UNIT, MatchSettings.NO_UNIT, MatchSettings.NO_UNIT };
	private int [ ] instanceIDs = { MatchSettings.NO_UNIT, MatchSettings.NO_UNIT, MatchSettings.NO_UNIT, MatchSettings.NO_UNIT, MatchSettings.NO_UNIT, MatchSettings.NO_UNIT };
	private Dictionary<int, int> unitIndexDic = new Dictionary<int, int> ( );
	private readonly Color32 ELIMINATION = Color.grey; //new Color32 ( 100, 100, 100, 255 );

	#endregion // HUD Data

	#region Public Functions

	/// <summary>
	/// Initializes the HUD for a specified player.
	/// </summary>
	/// <param name="p"> The player that this HUD will represent. </param>
	public void Initialize ( Player p )
	{
		// Set team name
		playerName.text = p.playerName;
		playerName.color = Util.TeamColor ( p.team );

		// Set units
		for ( int i = 0; i < instanceIDs.Length; i++ )
		{
			// Check for unit
			if ( i < p.units.Count )
			{
				// Store unit instance id
				unitIDs [ i ] = p.units [ i ].unitID;
				instanceIDs [ i ] = p.units [ i ].instanceID;
				unitIndexDic.Add ( p.units [ i ].instanceID, i );

				// Set unit portrait
				unitPortraits [ i ].SetUnit ( p.units [ i ].unitID, p.units [ i ].displaySprite, Util.TeamColor ( p.team ) );

				// Clear status effects
				ClearStatusEffects ( statusEffects [ i ] );
			}
			else
			{
				// Hide unit portrait
				unitPortraits [ i ].gameObject.SetActive ( false );
			}
		}
	}

	/// <summary>
	/// Displays that a unit has been KO'd.
	/// </summary>
	/// <param name="id"> The instance ID of the KO'd unit. </param>
	public void DisplayKO ( int id )
	{
		// Disable portrait
		unitPortraits [ unitIndexDic [ id ] ].EnableToggle ( false );

		// Hide status effects
		ClearStatusEffects ( statusEffects [ unitIndexDic [ id ] ] );
	}

	/// <summary>
	/// Displays the elimination of a player from a match.
	/// </summary>
	public void DisplayElimination ( )
	{
		// Display player elimination
		playerName.color = ELIMINATION;

		// Display deactivation of each special ability
		//foreach ( int id in indexDic.Keys )
		//	DisplayDeactivation ( id );
	}

	/// <summary>
	/// Updates a unit's portrait to a new sprite. 
	/// </summary>
	/// <param name="id"> The instance ID of the unit. </param>
	/// <param name="newSprite"> The new sprite to display in the portrait. </param>
	public void UpdatePortrait ( int id, Sprite newSprite )
	{
		// Change portrait icon
		unitPortraits [ unitIndexDic [ id ] ].SetUnit ( unitIDs [ unitIndexDic [ id ] ], newSprite, unitPortraits [ unitIndexDic [ id ] ].teamColor );
	}

	/// <summary>
	/// Updates a unit's portrait to a new team color.
	/// </summary>
	/// <param name="id"> The instance ID of the unit. </param>
	/// <param name="newColor"> The new team color to display in the portrait. </param>
	public void UpdatePortrait ( int id, Color32 newColor )
	{
		// Change portrait color
		unitPortraits [ unitIndexDic [ id ] ].SetUnit ( unitIDs [ unitIndexDic [ id ] ], unitPortraits [ unitIndexDic [ id ] ].icon.sprite, newColor );
	}

	/// <summary>
	/// Checks if this Player HUD displays a unit by its instance ID.
	/// Returns true if this HUD contains the unit.
	/// </summary>
	/// <returns> Whether or not this HUD contains the unit. </returns>
	public bool CheckForUnit ( int id )
	{
		// Check if the instance ID for a unit is included in this HUD
		return unitIndexDic.ContainsKey ( id );
	}

	/// <summary>
	/// Adds a portrait for a new unit to be displayed on the Player HUD.
	/// </summary>
	/// <param name="newUnit"> The unit whose portrait is being added. </param>
	/// <param name="adjacentID"> The instance ID the new portrait should appear next to. </param>
	public void AddPortrait ( Unit newUnit, int adjacentID )
	{
		// Get the first empty slot as the start index
		int startIndex = System.Array.IndexOf ( instanceIDs, MatchSettings.NO_UNIT );

		// Get the index of the adjacent unit as the end index
		int endIndex = unitIndexDic [ adjacentID ] + 1;

		// Shift each portrait and unit back one space to make room for the new portrait
		for ( int i = startIndex; i > endIndex; i-- )
		{
			// Update the stored id
			unitIDs [ i ] = unitIDs [ i - 1 ];
			instanceIDs [ i ] = instanceIDs [ i - 1 ];
			unitIndexDic [ instanceIDs [ i - 1 ] ]++;

			// Set portrait
			unitPortraits [ i ].gameObject.SetActive ( true );
			unitPortraits [ i ].SetUnit ( unitIDs [ i - 1 ], unitPortraits [ i - 1 ].icon.sprite, unitPortraits [ i - 1 ].teamColor );
			unitPortraits [ i ].EnableToggle ( unitPortraits [ i - 1 ].IsEnabled );

			// Set status effect icons
			CopyStatusEffects ( statusEffects [ i - 1 ], statusEffects [ i ] );
		}

		// Store the ids of the new unit
		unitIDs [ endIndex ] = newUnit.unitID;
		instanceIDs [ endIndex ] = newUnit.instanceID;
		unitIndexDic.Add ( newUnit.instanceID, endIndex );

		// Display portrait of the new unit
		unitPortraits [ endIndex ].SetUnit ( newUnit.unitID, newUnit.displaySprite, Util.TeamColor ( newUnit.owner.team ) );

		// Display status effects of the new unit
		UpdateStatusEffects ( newUnit.instanceID, newUnit.status );
	}

	/// <summary>
	/// Removes a portrait for a unit from the Player HUD.
	/// </summary>
	/// <param name="id"> The instance ID of the unit whose portrait is being removed. </param>
	public void RemovePortrait ( int id )
	{
		// Shift each portrait and unit forward one space to adjust for the removed portrait
		for ( int i = unitIndexDic [ id ]; i < unitPortraits.Length; i++ )
		{
			// Check for an exist unit
			if ( i + 1 < unitPortraits.Length && unitIDs [ i + 1 ] != MatchSettings.NO_UNIT )
			{
				// Update the stored id
				unitIDs [ i ] = unitIDs [ i + 1 ];
				instanceIDs [ i ] = instanceIDs [ i + 1 ];
				unitIndexDic [ instanceIDs [ i + 1 ] ]--;

				// Set portrait
				unitPortraits [ i ].SetUnit ( unitIDs [ i + 1 ], unitPortraits [ i + 1 ].icon.sprite, unitPortraits [ i + 1 ].teamColor );
				unitPortraits [ i ].EnableToggle ( unitPortraits [ i + 1 ].IsEnabled );

				// Set status effect icons
				CopyStatusEffects ( statusEffects [ i + 1 ], statusEffects [ i ] );
			}
			else
			{
				// Update the stored id to be nothing
				unitIDs [ i ] = MatchSettings.NO_UNIT;
				instanceIDs [ i ] = MatchSettings.NO_UNIT;

				// Hide the empty portrait
				unitPortraits [ i ].gameObject.SetActive ( false );

				// Hide the status effects
				ClearStatusEffects ( statusEffects [ i ] );

				// The portrait removal is now complete
				break;
			}
		}

		// Remove the instance id from the dictionary
		unitIndexDic.Remove ( id );
	}

	/// <summary>
	/// Displays the icons of any status effects on a unit.
	/// </summary>
	/// <param name="id"> The instance ID of the unit whose status effects are being displayed. </param>
	/// <param name="status"> The unit's status effects. </param>
	public void UpdateStatusEffects ( int id, StatusEffects status )
	{
		// Update each status effect icon
		for ( int i = 0; i < statusEffects [ unitIndexDic [ id ] ].statusIcons.Length; i++ )
		{
			// Check for status effect
			if ( i < status.effects.Count )
			{
				// Display status effect
				statusEffects [ unitIndexDic [ id ] ].statusIcons [ i ].gameObject.SetActive ( true );
				statusEffects [ unitIndexDic [ id ] ].statusIcons [ i ].sprite = status.effects [ i ].info.icon;
				statusEffects [ unitIndexDic [ id ] ].statusIcons [ i ].color = Util.TeamColor ( status.effects [ i ].info.caster.owner.team );
			}
			else
			{
				// Hide icon
				statusEffects [ unitIndexDic [ id ] ].statusIcons [ i ].gameObject.SetActive ( false );
			}
		}
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Hides all status effect icons.
	/// </summary>
	/// <param name="bar"> The status effects bar being cleared of icons. </param>
	private void ClearStatusEffects ( StatusEffectsBar bar )
	{
		// Hide each status effect icon
		for ( int i = 0; i < bar.statusIcons.Length; i++ )
			bar.statusIcons [ i ].gameObject.SetActive ( false );
	}

	/// <summary>
	/// Copies the status effects icons from one portrait to another.
	/// </summary>
	/// <param name="from"> The status effect icons being copied from. </param>
	/// <param name="to"> The status effect icons being copied to. </param>
	private void CopyStatusEffects ( StatusEffectsBar from, StatusEffectsBar to )
	{
		// Copy each status effect
		for ( int i = 0; i < to.statusIcons.Length; i++ )
		{
			// Copy status effect icon
			to.statusIcons [ i ].gameObject.SetActive ( from.statusIcons [ i ].gameObject.activeSelf );
			to.statusIcons [ i ].sprite = from.statusIcons [ i ].sprite;
			to.statusIcons [ i ].color = from.statusIcons [ i ].color;
		}
	}

	#endregion // Private Functions
}
