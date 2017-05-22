using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitHUD : MonoBehaviour
{
	// UI elements
	public GameObject container;
	public Image unitIcon;
	public TextMeshProUGUI unitName;
	public GameObject abilityContainer;
	public AbilityHUD ability1;
	public AbilityHUD ability2;
	public TextMeshProUGUI pawnName;
	public RectTransform heightReference;

	// HUD information
	private Unit currentUnit;
	private readonly Color32 INACTIVE = new Color32 ( 255, 150, 150, 255 );
	private readonly Color32 ACTIVE   = new Color32 ( 255, 210,  75, 255 );

	/// <summary>
	/// Displays the HUD information for the selected unit.
	/// </summary>
	public void DisplayUnit ( Unit u )
	{
		// Store current unit
		currentUnit = u;

		// Display HUD
		container.SetActive ( true );

		// Display unit icon
		unitIcon.sprite = currentUnit.displaySprite;
		unitIcon.color = Util.TeamColor ( currentUnit.team.team );

		// Check unit type
		if ( currentUnit is HeroUnit )
		{
			// Store unit as a hero
			HeroUnit h = currentUnit as HeroUnit;

			// Display unit name
			unitName.gameObject.SetActive ( true );
			unitName.text = currentUnit.characterName;

			// Display ability HUD
			abilityContainer.SetActive ( true );
			ability2.container.SetActive ( true );
			pawnName.gameObject.SetActive ( false );

			// Display ability 1
			SetupAbility ( h.currentAbility1, h.info.ability1, ability1, h.abilitySprite1 );

			// Display ability 2
			SetupAbility ( h.currentAbility2, h.info.ability2, ability2, h.abilitySprite2 );
		}
		else if ( currentUnit is Leader )
		{
			// Store unit as a leader
			Leader l = currentUnit as Leader;

			// Display unit name
			unitName.gameObject.SetActive ( true );
			unitName.text = currentUnit.characterName;

			// Display ability HUD
			abilityContainer.SetActive ( true );
			ability2.container.SetActive ( false );
			pawnName.gameObject.SetActive ( false );

			// Display ability icon
			ability1.icon.sprite = l.abilitySprite;

			// Display ability
			DisplayPassive ( l.currentAbility, l.ability, ability1 );
		}
		else if ( currentUnit is Pawn )
		{
			// Store unit as a pawn
			Pawn p = currentUnit as Pawn;

			// Display unit name
			pawnName.gameObject.SetActive ( true );
			pawnName.text = p.characterName + "\n" + p.characterNickname;

			// Hide ability HUD
			unitName.gameObject.SetActive ( false );
			abilityContainer.SetActive ( false );
		}
	}

	/// <summary>
	/// Fills the Unit HUD for an ability upon a unit being selected.
	/// </summary>
	private void SetupAbility ( AbilitySettings current, Ability setting, AbilityHUD hud, Sprite sprite )
	{
		// Set ability icon
		hud.icon.sprite = sprite;

		// Check if ability is enabled
		if ( current.enabled )
		{
			// Display that the ability is enabled
			hud.disabledPrompt.SetActive ( false );

			// Display ability
			DisplayAbility ( current, setting, hud );
		}
		else
		{
			// Display that the ability is not enabled
			hud.activityDisplay.gameObject.SetActive ( true );
			hud.activityDisplay.rectTransform.offsetMax = Vector2.zero;
			hud.activityDisplay.color = INACTIVE;
			hud.disabledPrompt.SetActive ( true );

			// Hide extra elements
			hud.cooldown.gameObject.SetActive ( false );

			// Check ability type
			if ( current.type == Ability.AbilityType.Command )
			{
				// Display command buttons
				hud.useButton.gameObject.SetActive ( true );
				hud.useButton.interactable = false;
				hud.cancelButton.SetActive ( false );
			}
			else
			{
				// Hide command buttons
				hud.useButton.gameObject.SetActive ( false );
				hud.cancelButton.SetActive ( false );
			}
		}
	}

	/// <summary>
	/// Displays the current state of an ability in the Unit HUD.
	/// Use this for updating an already filled Unit HUD.
	/// </summary>
	public void DisplayAbility ( AbilitySettings current )
	{
		// Get current unit information
		HeroUnit h = currentUnit as HeroUnit;
		Ability setting;
		AbilityHUD hud;
		if ( current == h.currentAbility1 )
		{
			// Store ability 1
			setting = h.info.ability1;
			hud = ability1;
		}
		else
		{
			// Store ability 2
			setting = h.info.ability2;
			hud = ability2;
		}

		// Display ability
		DisplayAbility ( current, setting, hud );
	}

	/// <summary>
	/// Displays the current state of an ability in the Unit HUD.
	/// </summary>
	private void DisplayAbility ( AbilitySettings current, Ability setting, AbilityHUD hud )
	{
		// Check ability type
		switch ( current.type )
		{
		// Passive ability
		case Ability.AbilityType.Passive:

			// Display passive ability
			DisplayPassive ( current, setting, hud );

			break;

		// Special ability
		case Ability.AbilityType.Special:

			// Display special ability
			DisplaySpecial ( current, setting, hud );

			break;

		// Command ability
		case Ability.AbilityType.Command:

			// Display command ability
			DisplayCommand ( current, setting, hud );

			break;
		}
	}

	/// <summary>
	/// Displays the appropriate information for the current state of a passive ability.
	/// </summary>
	private void DisplayPassive ( AbilitySettings current, Ability setting, AbilityHUD hud )
	{
		// Hide cooldown prompt
		hud.cooldown.gameObject.SetActive ( false );

		// Hide command buttons
		hud.useButton.gameObject.SetActive ( false );
		hud.cancelButton.SetActive ( false );

		// Check if the ability is still active
		if ( current.duration > 0 )
		{
			// Check if the ability's duration is full
			if ( current.duration != setting.duration )
			{
				// Display that that the ability's duration is not full
				hud.activityDisplay.gameObject.SetActive ( true );
				hud.activityDisplay.color = ACTIVE;

				// Calculate percentage
				hud.activityDisplay.rectTransform.offsetMax = CalculateCountSize ( (float)current.duration, (float)setting.duration, heightReference.sizeDelta.y );
			}
			else
			{
				// Display that the ability is active
				hud.activityDisplay.gameObject.SetActive ( false );
			}
		}
		else
		{
			// Display that the ability is inactive
			hud.activityDisplay.gameObject.SetActive ( true );
			hud.activityDisplay.rectTransform.offsetMax = Vector2.zero;
			hud.activityDisplay.color = INACTIVE;
		}
	}

	/// <summary>
	/// Displays the appropriate information for the current state of a special ability.
	/// </summary>
	private void DisplaySpecial ( AbilitySettings current, Ability setting, AbilityHUD hud )
	{
		// Hide command buttons
		hud.useButton.gameObject.SetActive ( false );
		hud.cancelButton.SetActive ( false );

		// Check if the ability is on cooldown
		if ( current.cooldown > 0 )
		{
			// Display that the ability is on cooldown
			hud.activityDisplay.gameObject.SetActive ( true );
			hud.activityDisplay.color = INACTIVE;

			// Calculate percentage
			hud.activityDisplay.rectTransform.offsetMax = CalculateCountSize ( (float)current.cooldown, (float)setting.cooldown, heightReference.sizeDelta.y );

			// Display cooldown
			hud.cooldown.gameObject.SetActive ( true );
			hud.cooldown.text = current.cooldown.ToString ( );
		}
		else
		{
			// Display that the ability is not on cooldown
			hud.activityDisplay.gameObject.SetActive ( false );
			hud.cooldown.gameObject.SetActive ( false );
		}
	}

	/// <summary>
	/// Displays the appropriate information for the current state of a command ability.
	/// </summary>
	private void DisplayCommand ( AbilitySettings current, Ability setting, AbilityHUD hud )
	{
		// Display command button
		hud.useButton.gameObject.SetActive ( true );
		hud.useButton.interactable = false;
		hud.cancelButton.SetActive ( false );

		// Check if the command is on cooldown
		if ( current.cooldown > 0 )
		{
			// Display that the ability is on cooldown
			hud.activityDisplay.gameObject.SetActive ( true );
			hud.activityDisplay.color = INACTIVE;

			// Calculate percentage
			hud.activityDisplay.rectTransform.offsetMax = CalculateCountSize ( (float)current.cooldown, (float)setting.cooldown, heightReference.sizeDelta.y );

			// Display cooldown
			hud.cooldown.gameObject.SetActive ( true );
			hud.cooldown.text = current.cooldown.ToString ( );
		}
		else
		{
			// Display that the ability is not on cooldown
			hud.activityDisplay.gameObject.SetActive ( false );
			hud.cooldown.gameObject.SetActive ( false );

			// Display if the command is ready for active use
			if ( current.active )
				hud.useButton.interactable = true;
		}
	}

	/// <summary>
	/// Calculates what percentate of the Ability HUD the Activity Display should fill for displaying durations and cooldowns.
	/// </summary>
	private Vector2 CalculateCountSize ( float current, float setting, float size )
	{
		// Calculate percentage
		float p = current / setting;

		// Calculate offset of UI panel from the top of the parent
		float offset = -1f * ( size - ( p * size ) );

		// Return UI panel offset
		return new Vector2 ( 0, offset );
	}

	/// <summary>
	/// Begins the selection phase of using a command.
	/// </summary>
	public void UseCommand ( bool isAbility1 )
	{
		// Check which ability is being used
		AbilityHUD hud;
		if ( isAbility1 )
			hud = ability1;
		else
			hud = ability2;

		// Hide the use button
		hud.useButton.gameObject.SetActive ( false );

		// Display the cancel button
		hud.cancelButton.SetActive ( true );

		// Display that the command is in use
		hud.activityDisplay.gameObject.SetActive ( true );
		hud.activityDisplay.color = ACTIVE;
		hud.activityDisplay.rectTransform.offsetMax = Vector2.zero;

		// Start the command setup
		HeroUnit h = currentUnit as HeroUnit;
		h.StartCommand ( );
	}

	/// <summary>
	/// Cancels the use of the hero's command.
	/// </summary>
	public void CancelCommand ( bool isAbility1 )
	{
		// Check which ability is being used
		AbilityHUD hud;
		if ( isAbility1 )
			hud = ability1;
		else
			hud = ability2;

		// Display the use button
		hud.useButton.gameObject.SetActive ( true );

		// Hide the cancel button
		hud.cancelButton.SetActive ( false );

		// Display that the command is not in use
		hud.activityDisplay.gameObject.SetActive ( false );

		// Cancel the command setup
		HeroUnit h = currentUnit as HeroUnit;
		h.EndCommand ( );
	}

	/// <summary>
	/// Sets any and all command buttons to be disabled.
	/// Call this function once moves have been selected and would no longer constitute the beginning of a turn.
	/// </summary>
	public void DisableCommandButtons ( )
	{
		// Check if the unit is a hero
		if ( currentUnit is HeroUnit )
		{
			// Store hero
			HeroUnit h = currentUnit as HeroUnit;

			// Check if ability 1 is a command and disable the use button
			if ( h.currentAbility1.type == Ability.AbilityType.Command )
				ability1.useButton.interactable = false;

			// Check if ability 2 is a command and disable the use button
			if ( h.currentAbility2.type == Ability.AbilityType.Command )
				ability2.useButton.interactable = false;
		}
	}

	/// <summary>
	/// Hides the Unit HUD.
	/// </summary>
	public void HideHUD ( )
	{
		// Hide HUD
		container.SetActive ( false );

		// Clear stored unit
		currentUnit = null;
	}
}
