using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerHUD : MonoBehaviour 
{
	#region Private Classes

	[System.Serializable]
	private class PlayerUnitHUD
	{
		[HideInInspector]
		public Unit Unit = null;

		public GameObject Container;
		public UnitPortrait Portrait;
		public Image [ ] StatusIcons;
	}

	#endregion // Private Classes

	#region UI Elements

	[SerializeField]
	private TextMeshProUGUI playerName;

	[SerializeField]
	private PlayerUnitHUD [ ] units;

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

	private Dictionary<int, PlayerUnitHUD> instanceHUDs = new Dictionary<int, PlayerUnitHUD> ( );

	private readonly Color32 ELIMINATION = Color.grey; //new Color32 ( 100, 100, 100, 255 );

	#endregion // HUD Data

	#region Public Functions

	/// <summary>
	/// Initializes the HUD for a specified player.
	/// </summary>
	/// <param name="player"> The player that this HUD will represent. </param>
	public void Initialize ( Player player )
	{
		// Store player
		Player = player;
		
		// Set team name
		playerName.text = Player.PlayerName;
		playerName.color = Util.TeamColor ( Player.Team );

		// Set units
		for ( int i = 0; i < units.Length; i++ )
		{
			// Check for unit
			if ( i < Player.Units.Count )
			{
				// Add unit to dictionary
				instanceHUDs.Add ( Player.UnitInstances [ i ].InstanceID, units [ i ] );

				// Set unit
				units [ i ].Unit = Player.UnitInstances [ i ];

				// Set unit portrait
				units [ i ].Portrait.SetPortrait ( Player.Units [ i ], Player.Team );

				// Clear status effects
				ClearStatusEffects ( units [ i ] );
			}
			else
			{
				// Hide unit portrait
				units [ i ].Container.SetActive ( false );
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
		instanceHUDs [ id ].Portrait.IsAvailable = false;

		// Hide status effects
		ClearStatusEffects ( instanceHUDs [ id ] );
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
	/// <param name="unit"> The unit being updated. </param>
	public void UpdatePortrait ( Unit unit )
	{
		// Update the portrait's icon or color
		instanceHUDs [ unit.InstanceID ].Portrait.SetPortrait ( unit.InstanceData, unit.Owner.Team );
	}

	/// <summary>
	/// Checks if this Player HUD displays a unit by its instance ID.
	/// Returns true if this HUD contains the unit.
	/// </summary>
	/// <param name="id"> The Instance ID being checked. </param>
	/// <returns> Whether or not this HUD contains the unit. </returns>
	public bool CheckForUnit ( int id )
	{
		// Check if the instance ID for a unit is included in this HUD
		return instanceHUDs.ContainsKey ( id );
	}

	/// <summary>
	/// Adds a portrait for a new unit to be displayed on the Player HUD.
	/// </summary>
	/// <param name="newUnit"> The unit whose portrait is being added. </param>
	/// <param name="adjacentID"> The instance ID the new portrait should appear next to. </param>
	public void AddPortrait ( Unit newUnit, int adjacentID )
	{
		// Store the first empty slot as the start index
		int startIndex = System.Array.IndexOf ( units, units.First ( x => !x.Container.activeSelf ) );

		// Store the index of the adjacent unit as the end index
		int endIndex = System.Array.IndexOf ( units, instanceHUDs [ adjacentID ] ) + 1;

		// Shift each portrait and unit back one space to make room for the new portrait
		for ( int i = startIndex; i < endIndex; i-- )
		{
			// Set portrait
			units [ i ].Container.SetActive ( true );
			units [ i ].Portrait.CopyPortrait ( units [ i - 1 ].Portrait );

			// Set status effect icons
			CopyStatusEffects ( units [ i - 1 ], units [ i ] );

			// Set unit to new portrait
			units [ i ].Unit = units [ i - 1 ].Unit;
			instanceHUDs [ units [ i ].Unit.InstanceID ] = units [ i ];
		}

		// Store the new unit's ID
		units [ endIndex ].Unit = newUnit;
		instanceHUDs.Add ( newUnit.InstanceID, units [ endIndex ] );

		// Display the new unit's portrait
		units [ endIndex ].Portrait.SetPortrait ( newUnit.InstanceData, Player.Team );

		// Display the status effects of the new unit
		UpdateStatusEffects ( newUnit.InstanceID, newUnit.Status );
	}

	/// <summary>
	/// Removes a portrait for a unit from the Player HUD.
	/// </summary>
	/// <param name="id"> The instance ID of the unit whose portrait is being removed. </param>
	public void RemovePortrait ( int id )
	{
		// Shift each portrait and unit forward one space to adjust for the removed portrait
		for ( int i = System.Array.IndexOf ( units, instanceHUDs [ id ] ); i < units.Length; i++ )
		{
			// Check for an existing unit
			if ( i + 1 < units.Length && units [ i + 1 ].Unit != null )
			{
				// Set portrait
				units [ i ].Portrait.CopyPortrait ( units [ i + 1 ].Portrait );

				// Set status effect icons
				CopyStatusEffects ( units [ i + 1 ], units [ i ] );

				// Set unit to new portrait
				units [ i ].Unit = units [ i + 1 ].Unit;
				instanceHUDs [ units [ i ].Unit.InstanceID ] = units [ i ];
			}
			else
			{
				// Set portrait has empty
				units [ i ].Unit = null;

				// Hide portrait
				units [ i ].Container.SetActive ( false );

				// Portrait removal is now complete
				break;
			}
		}

		// Remove unit from the dictionary
		instanceHUDs.Remove ( id );
	}

	/// <summary>
	/// Displays the icons of any status effects on a unit.
	/// </summary>
	/// <param name="id"> The instance ID of the unit whose status effects are being displayed. </param>
	/// <param name="status"> The unit's status effects. </param>
	public void UpdateStatusEffects ( int id, StatusEffects status )
	{
		// Update each status effect icon
		for ( int i = 0; i < instanceHUDs [ id ].StatusIcons.Length; i++ )
		{
			// Check for status effect
			if ( i < status.Effects.Count )
			{
				// Display status effect
				instanceHUDs [ id ].StatusIcons [ i ].gameObject.SetActive ( true );
				instanceHUDs [ id ].StatusIcons [ i ].sprite = status.Effects [ i ].Icon;
				instanceHUDs [ id ].StatusIcons [ i ].color = Util.TeamColor ( status.Effects [ i ].Caster.Owner.Team );
			}
			else
			{
				// Hide icon
				instanceHUDs [ id ].StatusIcons [ i ].gameObject.SetActive ( false );
			}
		}
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Hides all status effect icons.
	/// </summary>
	/// <param name="hud"> The status effects bar being cleared of icons. </param>
	private void ClearStatusEffects ( PlayerUnitHUD hud )
	{
		// Hide each status effect icon
		for ( int i = 0; i < hud.StatusIcons.Length; i++ )
			hud.StatusIcons [ i ].gameObject.SetActive ( false );
	}

	/// <summary>
	/// Copies the status effects icons from one portrait to another.
	/// </summary>
	/// <param name="from"> The status effect icons being copied from. </param>
	/// <param name="to"> The status effect icons being copied to. </param>
	private void CopyStatusEffects ( PlayerUnitHUD from, PlayerUnitHUD to )
	{
		// Copy each status effect
		for ( int i = 0; i < to.StatusIcons.Length; i++ )
		{
			// Copy status effect icon
			to.StatusIcons [ i ].gameObject.SetActive ( from.StatusIcons [ i ].gameObject.activeSelf );
			to.StatusIcons [ i ].sprite = from.StatusIcons [ i ].sprite;
			to.StatusIcons [ i ].color = from.StatusIcons [ i ].color;
		}
	}

	#endregion // Private Functions
}
