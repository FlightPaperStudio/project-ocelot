using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitHUD : MonoBehaviour
{
	// UI elements
	public GameObject container;
	public Image portraitContainer;
	public Image unitIcon;
	public GameObject heroContainer;
	public TextMeshProUGUI heroName;
	public GameObject abilityContainer;
	public AbilityHUD ability1;
	public AbilityHUD ability2;
	public GameObject pawnContainer;
	public TextMeshProUGUI pawnName;
	public RectTransform heightReference;
	public GameObject [ ] statusPrompts;
	public Image [ ] statusIcons;
	public TextMeshProUGUI [ ] statusTexts;

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

		// Display team color
		portraitContainer.color = Util.TeamColor ( currentUnit.owner.Team );

		// Display unit icon
		unitIcon.sprite = currentUnit.displaySprite;
		unitIcon.color = Util.TeamColor ( currentUnit.owner.Team );

		// Display status prompts
		UpdateStatusEffects ( );

		// Check unit type
		if ( currentUnit is HeroUnit )
		{
			// Store unit as a hero
			HeroUnit h = currentUnit as HeroUnit;

			// Display hero information
			heroContainer.SetActive ( true );
			pawnContainer.SetActive ( false );

			// Display hero name
			heroName.text = currentUnit.characterName;

			// Display ability HUD
			abilityContainer.SetActive ( true );
			ability2.container.SetActive ( true );

			// Display ability 1
			SetupAbility ( h.CurrentAbility1, h.Info.Ability1, ability1, h.abilitySprite1 );

			// Display ability 2
			SetupAbility ( h.CurrentAbility2, h.Info.Ability2, ability2, h.abilitySprite2 );
		}
		else if ( currentUnit is Leader )
		{
			// Store unit as a leader
			Leader l = currentUnit as Leader;

			// Display leader information
			heroContainer.SetActive ( true );
			pawnContainer.SetActive ( false );

			// Display leader name
			heroName.text = currentUnit.characterName;

			// Display ability HUD
			abilityContainer.SetActive ( true );
			ability2.container.SetActive ( false );

			// Display ability icon
			ability1.icon.sprite = l.abilitySprite;

			// Display ability
			DisplayPassive ( l.currentAbility, l.ability, ability1 );
		}
		else if ( currentUnit is Pawn )
		{
			// Store unit as a pawn
			Pawn p = currentUnit as Pawn;

			// Display pawn information
			pawnContainer.SetActive ( true );
			heroContainer.SetActive ( false );
			abilityContainer.SetActive ( false );

			// Display unit name
			pawnName.text = p.characterName + "\n" + p.characterNickname;
		}
	}

	/// <summary>
	/// Displays all of the current status effects applied to the unit.
	/// </summary>
	public void UpdateStatusEffects ( )
	{
		// Check each status prompt
		for ( int i = 0; i < statusPrompts.Length; i++ )
		{
			// Check for status prompt
			if ( i < currentUnit.status.effects.Count )
			{
				// Display prompt
				statusPrompts [ i ].SetActive ( true );

				// Display icon
				statusIcons [ i ].sprite = currentUnit.status.effects [ i ].info.icon;

				// Display text
				statusTexts [ i ].text = currentUnit.status.effects [ i ].info.text;
			}
			else
			{
				// Hide prompt
				statusPrompts [ i ].SetActive ( false );
			}
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
			if ( current.type == Ability.AbilityType.COMMAND )
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
		if ( current == h.CurrentAbility1 )
		{
			// Store ability 1
			setting = h.Info.Ability1;
			hud = ability1;
		}
		else
		{
			// Store ability 2
			setting = h.Info.Ability2;
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
		case Ability.AbilityType.PASSIVE:

			// Display passive ability
			DisplayPassive ( current, setting, hud );

			break;

		// Special ability
		case Ability.AbilityType.SPECIAL:

			// Display special ability
			DisplaySpecial ( current, setting, hud );

			break;

		// Command ability
		case Ability.AbilityType.COMMAND:

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
			if ( current.duration == setting.Duration && !current.active )
			{
				// Display the ability normally
				hud.activityDisplay.gameObject.SetActive ( false );
			}
			else
			{
				// Display that that the ability is active
				hud.activityDisplay.gameObject.SetActive ( true );
				hud.activityDisplay.color = ACTIVE;

				// Calculate percentage
				hud.activityDisplay.rectTransform.offsetMax = CalculateCountSize ( (float)current.duration, (float)setting.Duration, heightReference.sizeDelta.y );
			}
		}
		else
		{
			// Check if the ability is still active once the duration has expired
			if ( current.active )
			{
				// Display the ability normally
				hud.activityDisplay.gameObject.SetActive ( false );
			}
			else
			{
				// Display that the ability is inactive
				hud.activityDisplay.gameObject.SetActive ( true );
				hud.activityDisplay.rectTransform.offsetMax = Vector2.zero;
				hud.activityDisplay.color = INACTIVE;
			}
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
			hud.activityDisplay.rectTransform.offsetMax = CalculateCountSize ( (float)current.cooldown, (float)setting.Cooldown, heightReference.sizeDelta.y );

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
			hud.activityDisplay.rectTransform.offsetMax = CalculateCountSize ( (float)current.cooldown, (float)setting.Cooldown, heightReference.sizeDelta.y );

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
			if ( h.CurrentAbility1.type == Ability.AbilityType.COMMAND )
				ability1.useButton.interactable = false;

			// Check if ability 2 is a command and disable the use button
			if ( h.CurrentAbility2.type == Ability.AbilityType.COMMAND )
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
