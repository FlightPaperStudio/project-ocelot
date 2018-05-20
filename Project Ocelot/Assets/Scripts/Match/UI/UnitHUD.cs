using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitHUD : MonoBehaviour
{
	#region Private Classes

	[System.Serializable]
	private class AbilityHUD
	{
		public GameObject Container;
		public Image Background;
		public Image AbilityIcon;
		public Image DurationDisplay;
		public Image CooldownDisplay;
		public TextMeshProUGUI CooldownText;
		public Image ActiveDisplay;
		public GameObject DisabledDisplay;
		public Button UseButton;
		public Button CancelButton;
	}

	[System.Serializable]
	private class StatusEffectHUD
	{
		public GameObject Container;
		public Image StatusIcon;
		public TextMeshProUGUI StatusText;
	}

	#endregion // Private Classes

	#region UI Elements

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private UnitPortrait portrait;

	[SerializeField]
	private GameObject nameContainer;

	[SerializeField]
	private TextMeshProUGUI nameText;

	[SerializeField]
	private GameObject nicknameContainer;

	[SerializeField]
	private TextMeshProUGUI nicknameText;

	[SerializeField]
	private AbilityHUD [ ] abilities;

	[SerializeField]
	private StatusEffectHUD [ ] statuses;

	#endregion // UI Elements

	#region HUD Data

	private Unit currentUnit;

	private readonly Color32 UNAVAILABLE = new Color32 ( 255, 255, 255, 50);
	private readonly Color32 AVAILABLE = new Color32 ( 255, 255, 255, 125 );

	#endregion // HUD Data

	#region Public Functions

	/// <summary>
	/// Displays the HUD information for the selected unit.
	/// </summary>
	/// <param name="u"> The unit being displayed in the HUD. </param>
	public void DisplayUnit ( Unit u )
	{
		// Store current unit
		currentUnit = u;

		// Display HUD
		container.SetActive ( true );

		// Display unit in the portrait
		portrait.SetPortrait ( currentUnit.InstanceData, currentUnit.owner.Team );

		// Check if the unit has any abilities
		if ( currentUnit.InstanceData.Ability1 == null && currentUnit.InstanceData.Ability2 == null && currentUnit.InstanceData.Ability3 == null )
		{
			// Display the name and nickname rather than the name and abilities
			nameContainer.SetActive ( false );
			nicknameContainer.SetActive ( true );

			// Display nickname
			nicknameText.text = currentUnit.InstanceData.UnitName + "\n<size=60%>" + currentUnit.InstanceData.UnitNickname;

			// Hide ability HUD
			for ( int i = 0; i < abilities.Length; i++ )
				abilities [ i ].Container.SetActive ( false );
		}
		else
		{
			// Display unit name
			nameContainer.SetActive ( true );
			nicknameContainer.SetActive ( false );
			nameText.text = currentUnit.InstanceData.UnitName;

			// Display ability 1
			if ( currentUnit.InstanceData.Ability1 != null )
				UpdateAbilityHUD ( currentUnit.InstanceData.Ability1 );
			else
				abilities [ 0 ].Container.SetActive ( false );

			// Display ability 2
			if ( currentUnit.InstanceData.Ability2 != null )
				UpdateAbilityHUD ( currentUnit.InstanceData.Ability2 );
			else
				abilities [ 1 ].Container.SetActive ( false );

			// Display ability 3
			if ( currentUnit.InstanceData.Ability3 != null )
				UpdateAbilityHUD ( currentUnit.InstanceData.Ability3 );
			else
				abilities [ 2 ].Container.SetActive ( false );
		}

		// Display status effects
		UpdateStatusEffects ( );
	}

	/// <summary>
	/// Update the unit HUD for a specific ability.
	/// </summary>
	/// <param name="ability"> The ability being updated. </param>
	public void UpdateAbilityHUD ( AbilityInstanceData ability )
	{
		// Check for ability 1
		if ( ability == currentUnit.InstanceData.Ability1 )
			DisplayAbilityHUD ( ability, abilities [ 0 ] );

		// Check for ability 2
		if ( ability == currentUnit.InstanceData.Ability2 )
			DisplayAbilityHUD ( ability, abilities [ 1 ] );

		// Check for ability 3
		if ( ability == currentUnit.InstanceData.Ability3 )
			DisplayAbilityHUD ( ability, abilities [ 2 ] );
	}

	/// <summary>
	/// Displays all of the current status effects applied to the unit.
	/// </summary>
	public void UpdateStatusEffects ( )
	{
		// Display each status 
		for ( int i = 0; i < statuses.Length; i++ )
		{
			// Check for status
			if ( i < currentUnit.Status.effects.Count )
			{
				// Display container
				statuses [ i ].Container.SetActive ( true );

				// Display icon
				statuses [ i ].StatusIcon.sprite = currentUnit.Status.effects [ i ].info.icon;

				// Display text
				statuses [ i ].StatusText.text = currentUnit.Status.effects [ i ].info.text;
			}
			else
			{
				// Hide prompt
				statuses [ i ].Container.SetActive ( false );
			}
		}
	}

	/// <summary>
	/// Begins the selection phase of using a command.
	/// </summary>
	/// <param name="position"> The position of the ability in the order of abilities. </param>
	public void UseCommand ( int position )
	{
		// Get the ability and the ability HUD
		AbilityInstanceData ability;
		AbilityHUD hud;
		switch ( position )
		{
		case 1:
			ability = currentUnit.InstanceData.Ability1;
			hud = abilities [ 0 ];
			break;
		case 2:
		default:
			ability = currentUnit.InstanceData.Ability2;
			hud = abilities [ 1 ];
			break;
		case 3:
			ability = currentUnit.InstanceData.Ability3;
			hud = abilities [ 2 ];
			break;
		}

		// Start the command
		( currentUnit as HeroUnit ).StartCommand ( ability );

		// Display cancel button
		hud.UseButton.gameObject.SetActive ( false );
		hud.CancelButton.gameObject.SetActive ( true );

		// Update ability
		DisplayAbilityHUD ( ability, hud );
	}

	/// <summary>
	/// Cancels the use of the hero's command.
	/// </summary>
	/// <param name="position"> The position of the ability in the order of abilities. </param>
	public void CancelCommand ( int position )
	{
		// Get the ability and the ability HUD
		AbilityInstanceData ability;
		AbilityHUD hud;
		switch ( position )
		{
		case 1:
			ability = currentUnit.InstanceData.Ability1;
			hud = abilities [ 0 ];
			break;
		case 2:
		default:
			ability = currentUnit.InstanceData.Ability2;
			hud = abilities [ 1 ];
			break;
		case 3:
			ability = currentUnit.InstanceData.Ability3;
			hud = abilities [ 2 ];
			break;
		}

		// End the command
		( currentUnit as HeroUnit ).EndCommand ( );

		// Update ability
		DisplayAbilityHUD ( ability, hud );
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
			// Check if ability 1 is a command and disable the use button
			if ( currentUnit.InstanceData.Ability1 != null && ( currentUnit.InstanceData.Ability1.Type == AbilityData.AbilityType.COMMAND || currentUnit.InstanceData.Ability1.Type == AbilityData.AbilityType.TOGGLE_COMMAND ) )
				abilities [ 0 ].UseButton.interactable = false;

			// Check if ability 2 is a command and disable the use button
			if ( currentUnit.InstanceData.Ability2 != null && ( currentUnit.InstanceData.Ability2.Type == AbilityData.AbilityType.COMMAND || currentUnit.InstanceData.Ability2.Type == AbilityData.AbilityType.TOGGLE_COMMAND ) )
				abilities [ 1 ].UseButton.interactable = false;

			// Check if ability 3 is a command and disable the use button
			if ( currentUnit.InstanceData.Ability3 != null && ( currentUnit.InstanceData.Ability3.Type == AbilityData.AbilityType.COMMAND || currentUnit.InstanceData.Ability3.Type == AbilityData.AbilityType.TOGGLE_COMMAND ) )
				abilities [ 2 ].UseButton.interactable = false;
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

	/// <summary>
	/// Hides a Cancel Button for an ability.
	/// </summary>
	/// <param name="ability"></param>
	public void HideCancelButton ( AbilityInstanceData ability )
	{
		if ( ability == currentUnit.InstanceData.Ability1 )
			abilities [ 0 ].CancelButton.gameObject.SetActive ( false );
		else if ( ability == currentUnit.InstanceData.Ability2 )
			abilities [ 1 ].CancelButton.gameObject.SetActive ( false );
		else if ( ability == currentUnit.InstanceData.Ability3 )
			abilities [ 2 ].CancelButton.gameObject.SetActive ( false );
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Sets the ability HUD to its default state to be further updated by the ability type.
	/// </summary>
	/// <param name="ability"> The ability the HUD is representing. </param>
	/// <param name="hud"> The HUD for the ability. </param>
	private void DisplayAbilityHUD ( AbilityInstanceData ability, AbilityHUD hud )
	{
		// Display hud
		hud.Container.SetActive ( true );

		// Display ability icon
		hud.AbilityIcon.sprite = ability.Icon;

		// Set team colors
		hud.CooldownDisplay.color = Util.TeamColor ( currentUnit.owner.Team );
		hud.ActiveDisplay.color = Util.TeamColor ( currentUnit.owner.Team );

		// Hide duration by default
		hud.DurationDisplay.gameObject.SetActive ( false );

		// Hide cooldown by default
		hud.CooldownDisplay.gameObject.SetActive ( false );
		hud.CooldownText.gameObject.SetActive ( false );

		// Hide active by default
		hud.ActiveDisplay.gameObject.SetActive ( false );

		// Hide controls by default
		hud.UseButton.gameObject.SetActive ( false );
		hud.CancelButton.gameObject.SetActive ( false );

		// Hide disable by default
		hud.DisabledDisplay.SetActive ( false );

		// Display ability
		switch ( ability.Type )
		{
		case AbilityData.AbilityType.PASSIVE:
			DisplayPassive ( ability, hud );
			break;
		case AbilityData.AbilityType.SPECIAL:
			DisplaySpecial ( ability, hud );
			break;
		case AbilityData.AbilityType.COMMAND:
			DisplayCommand ( ability, hud );
			break;
		case AbilityData.AbilityType.TOGGLE_COMMAND:
			DisplayToggleCommand ( ability, hud );
			break;
		}
	}

	/// <summary>
	/// Displays the appropriate information for the current state of a passive ability.
	/// </summary>
	/// <param name="ability"> The ability the HUD is representing. </param>
	/// <param name="hud"> The HUD for the ability. </param>
	private void DisplayPassive ( AbilityInstanceData ability, AbilityHUD hud )
	{
		// Set background color
		hud.Background.color = !ability.IsAvailable || !ability.IsEnabled ? UNAVAILABLE : AVAILABLE;

		// Check if the ability enabled
		if ( ability.IsEnabled )
		{
			// Display activity
			hud.ActiveDisplay.gameObject.SetActive ( ability.IsActive );

			// Check for duration
			if ( ability.Duration != 0 && ability.CurrentDuration < ability.Duration )
			{
				// Display duration
				hud.Background.color = UNAVAILABLE;
				hud.DurationDisplay.gameObject.SetActive ( true );
				hud.DurationDisplay.rectTransform.offsetMax = CalculateCountdownSize ( ability.CurrentDuration, ability.Duration, hud.Background.rectTransform.sizeDelta.y );
			}
		}
		else
		{
			// Display that the ability is disabled
			hud.DisabledDisplay.SetActive ( true );
		}
	}

	/// <summary>
	/// Displays the appropriate information for the current state of a special ability.
	/// </summary>
	/// <param name="ability"> The ability the HUD is representing. </param>
	/// <param name="hud"> The HUD for the ability. </param>
	private void DisplaySpecial ( AbilityInstanceData ability, AbilityHUD hud )
	{
		// Set background color
		hud.Background.color = !ability.IsAvailable || !ability.IsEnabled ? UNAVAILABLE : AVAILABLE;

		// Check if the ability enabled
		if ( ability.IsEnabled )
		{
			// Display activity
			hud.ActiveDisplay.gameObject.SetActive ( ability.IsActive );

			// Check for cooldown
			if ( ability.CurrentCooldown > 0 )
			{
				// Display cooldown
				hud.CooldownText.gameObject.SetActive ( true );
				hud.CooldownText.text = ability.CurrentCooldown.ToString ( );
				hud.CooldownDisplay.gameObject.SetActive ( true );
				hud.CooldownDisplay.rectTransform.offsetMax = CalculateCountdownSize ( ability.CurrentCooldown, ability.Cooldown, hud.Background.rectTransform.sizeDelta.y );
			}
		}
		else
		{
			// Display that the ability is disabled
			hud.DisabledDisplay.SetActive ( true );
		}
	}

	/// <summary>
	/// Displays the appropriate information for the current state of a command ability.
	/// </summary>
	/// <param name="ability"> The ability the HUD is representing. </param>
	/// <param name="hud"> The HUD for the ability. </param>
	private void DisplayCommand ( AbilityInstanceData ability, AbilityHUD hud )
	{
		// Set background color
		hud.Background.color = !ability.IsAvailable || !ability.IsEnabled ? UNAVAILABLE : AVAILABLE;

		// Check if the ability enabled
		if ( ability.IsEnabled )
		{
			// Set use button
			if ( ability.IsActive && ability.IsAvailable )
			{
				hud.UseButton.gameObject.SetActive ( false );
				hud.CancelButton.gameObject.SetActive ( true );
			}
			else
			{
				hud.UseButton.gameObject.SetActive ( true );
				hud.CancelButton.gameObject.SetActive ( false );
				hud.UseButton.interactable = ability.IsAvailable;
			}

			// Display activity
			hud.ActiveDisplay.gameObject.SetActive ( ability.IsActive || ability.CurrentDuration > 0 );

			// Check for cooldown
			if ( ability.CurrentCooldown > 0 )
			{
				// Display cooldown
				hud.CooldownText.gameObject.SetActive ( true );
				hud.CooldownText.text = ability.CurrentCooldown.ToString ( );
				hud.CooldownDisplay.gameObject.SetActive ( true );
				hud.CooldownDisplay.rectTransform.offsetMax = CalculateCountdownSize ( ability.CurrentCooldown, ability.Cooldown, hud.Background.rectTransform.sizeDelta.y );
			}
		}
		else
		{
			// Display that the ability is disabled
			hud.DisabledDisplay.SetActive ( true );
		}
	}

	/// <summary>
	/// Displays the appropriate information for the current state of a toggle command ability.
	/// </summary>
	/// <param name="ability"> The ability the HUD is representing. </param>
	/// <param name="hud"> The HUD for the ability. </param>
	private void DisplayToggleCommand ( AbilityInstanceData ability, AbilityHUD hud )
	{
		// Set background color
		hud.Background.color = !ability.IsAvailable || !ability.IsEnabled ? UNAVAILABLE : AVAILABLE;

		// Check if the ability enabled
		if ( ability.IsEnabled )
		{
			// Display activity
			hud.ActiveDisplay.gameObject.SetActive ( ability.IsActive );

			// Set use button
			if ( ability.IsActive && ability.IsAvailable )
			{
				hud.UseButton.gameObject.SetActive ( false );
				hud.CancelButton.gameObject.SetActive ( true );
			}
			else
			{
				hud.UseButton.gameObject.SetActive ( true );
				hud.CancelButton.gameObject.SetActive ( false );
				hud.UseButton.interactable = ability.IsAvailable;
			}

			// Check for duration
			if ( ability.Duration != 0 && ability.CurrentDuration < ability.Duration )
			{
				// Display duration
				hud.DurationDisplay.rectTransform.offsetMax = CalculateCountdownSize ( ability.CurrentDuration, ability.Duration, hud.Background.rectTransform.sizeDelta.y );
			}

			// Check for cooldown
			if ( ability.CurrentCooldown > 0 )
			{
				// Display cooldown
				hud.CooldownText.gameObject.SetActive ( true );
				hud.CooldownText.text = ability.CurrentCooldown.ToString ( );
				hud.CooldownDisplay.gameObject.SetActive ( true );
				hud.CooldownDisplay.rectTransform.offsetMax = CalculateCountdownSize ( ability.CurrentCooldown, ability.Cooldown, hud.Background.rectTransform.sizeDelta.y );
			}
		}
		else
		{
			// Display that the ability is disabled
			hud.DisabledDisplay.SetActive ( true );
		}
	}

	/// <summary>
	/// Calculates what percentate of the Ability HUD the Activity Display should fill for displaying durations and cooldowns.
	/// </summary>
	/// <param name="current"> The current cooldown or duration of an ability. </param>
	/// <param name="setting"> The setting for the cooldown or duration of an ability. </param>
	/// <param name="size"> The vertical size of the HUD. </param>
	private Vector2 CalculateCountdownSize ( int current, int setting, float size )
	{
		// Calculate percentage
		float percentage = (float)current / (float)setting;
		
		// Calculate offset of UI panel from the top of the parent
		float offset = -1f * ( size - ( percentage * size ) );

		// Return UI panel offset
		return new Vector2 ( 0, offset );
	}

	#endregion // Private Functions
}
