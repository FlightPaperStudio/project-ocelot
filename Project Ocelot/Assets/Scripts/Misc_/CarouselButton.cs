using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CarouselButton : MonoBehaviour
{
	#region UI Elements

	[SerializeField]
	private Button previousButton;

	[SerializeField]
	private Button nextButton;

	[SerializeField]
	private TextMeshProUGUI optionText;

	#endregion // UI Elements

	#region Carousel Data

	[SerializeField]
	private string [ ] options;

	[SerializeField]
	private bool isWrapAround = true;

	private int optionIndex;

	private const float SLIDE_TIME = 0.1f;

	/// <summary>
	/// The current option index of the carousel.
	/// </summary>
	public int OptionIndex
	{
		get
		{
			// Return value
			return optionIndex;
		}
	}

	/// <summary>
	/// The current option for if the carousel is a boolean toggle.
	/// </summary>
	public bool IsOptionTrue
	{
		get
		{
			// Return if option index is 1
			return optionIndex == 1;
		}
	}

	#endregion // Carousel Data

	#region Public Functions

	/// <summary>
	/// Moves to the next option in the carousel.
	/// Use this function as a button click event wrapper.
	/// </summary>
	public void NextOption ( )
	{
		// Enable previous button if wrap around is disabled
		if ( !isWrapAround )
			previousButton.interactable = true;

		// Increment index
		optionIndex++;

		// Check for overflow
		if ( optionIndex >= options.Length )
		{
			// Check if wrap around is enabled
			if ( isWrapAround )
			{
				// Wrap around to the first option
				optionIndex = 0;
			}
			else
			{
				// Go back to the last option
				optionIndex = options.Length - 1;

				// Disable the next button
				nextButton.interactable = false;
			}
		}

		// Create slide animation
		Sequence s = DOTween.Sequence ( )
			.Append ( optionText.rectTransform.DOAnchorPos ( new Vector2 ( -1 * ( optionText.rectTransform.rect.width / 2 ), 0 ), SLIDE_TIME )
				.SetEase ( Ease.InExpo )
				.OnComplete ( ( ) =>
				{
					// Display new option
					optionText.text = options [ optionIndex ];

					// Position text on other side of carousel for the slide in animation.
					optionText.rectTransform.anchoredPosition = new Vector2 ( optionText.rectTransform.rect.width / 2, 0 );
				} ) )
			.Join ( optionText.DOFade ( 0, SLIDE_TIME ) )
			.Append ( optionText.rectTransform.DOAnchorPos ( Vector2.zero, SLIDE_TIME )
				.SetEase ( Ease.OutExpo ) )
			.Join ( optionText.DOFade ( 1, SLIDE_TIME ) );
	}

	/// <summary>
	/// Move to the previous option in the carousel.
	/// Use this function as a button click event wrapper.
	/// </summary>
	public void PreviousOption ( )
	{
		// Enable next button if wrap around is disabled
		if ( !isWrapAround )
			nextButton.interactable = true;

		// Decrement index
		optionIndex--;

		// Check for underflow
		if ( optionIndex < 0 )
		{
			// Check if wrap around is enabled
			if ( isWrapAround )
			{
				// Wrap around to the last option
				optionIndex = options.Length - 1;
			}
			else
			{
				// Go back to the first option
				optionIndex = 0;

				// Disable the previous button
				previousButton.interactable = false;
			}
		}

		// Create slide animation
		Sequence s = DOTween.Sequence ( )
			.Append ( optionText.rectTransform.DOAnchorPos ( new Vector2 ( optionText.rectTransform.rect.width / 2, 0 ), SLIDE_TIME )
				.SetEase ( Ease.InExpo )
				.OnComplete ( ( ) =>
				{
					// Display new option
					optionText.text = options [ optionIndex ];

					// Position text on other side of carousel for the slide in animation.
					optionText.rectTransform.anchoredPosition = new Vector2 ( -1 * ( optionText.rectTransform.rect.width / 2 ), 0 );
				} ) )
			.Join ( optionText.DOFade ( 0, SLIDE_TIME ) )
			.Append ( optionText.rectTransform.DOAnchorPos ( Vector2.zero, SLIDE_TIME )
				.SetEase ( Ease.OutExpo ) )
			.Join ( optionText.DOFade ( 1, SLIDE_TIME ) );
	}

	/// <summary>
	/// Moves to a specified option in the carousel.
	/// </summary>
	/// <param name="index"> The index of the option. </param>
	/// <param name="playAnimation"> Whether or not the slide animation should be played. </param>
	public void SetOption ( int index, bool playAnimation )
	{
		// Check for under or overflow
		if ( index < 0 )
			index = 0;
		else if ( index >= options.Length )
			index = options.Length - 1;

		// Set which direction to animate toward
		bool slideLeft = optionIndex < index;
		playAnimation = playAnimation && optionIndex != index;

		// Set index
		optionIndex = index;

		// Enable or disable buttons based on wrap around setting and current index
		previousButton.interactable = isWrapAround || optionIndex > 0;
		nextButton.interactable = isWrapAround || optionIndex < options.Length - 1;

		// Check if animation should be played
		if ( playAnimation )
		{
			// Create slide animation
			Sequence s = DOTween.Sequence ( )
				.Append ( optionText.rectTransform.DOAnchorPos ( new Vector2 ( slideLeft ? -1 * ( optionText.rectTransform.rect.width / 2 ) : optionText.rectTransform.rect.width / 2, 0 ), SLIDE_TIME )
					.SetEase ( Ease.InExpo )
					.OnComplete ( ( ) =>
					{
					// Display new option
					optionText.text = options [ optionIndex ];

					// Position text on other side of carousel for the slide in animation.
					optionText.rectTransform.anchoredPosition = new Vector2 ( slideLeft ? optionText.rectTransform.rect.width / 2 : -1 * ( optionText.rectTransform.rect.width / 2 ), 0 );
					} ) )
				.Join ( optionText.DOFade ( 0, SLIDE_TIME ) )
				.Append ( optionText.rectTransform.DOAnchorPos ( Vector2.zero, SLIDE_TIME )
					.SetEase ( Ease.OutExpo ) )
				.Join ( optionText.DOFade ( 1, SLIDE_TIME ) );
		}
		else
		{
			// Display option
			optionText.text = options [ optionIndex ];
		}
	}

	/// <summary>
	/// Moves to a specified option in the carousel.
	/// Call this function when the carousel is a boolean toggle.
	/// </summary>
	/// <param name="option"> Whether or not the option is true or false. </param>
	/// <param name="playAnimation"> Whether or not the slide animation should be played. </param>
	public void SetOption ( bool option, bool playAnimation )
	{
		// Set option
		SetOption ( option ? 1 : 0, playAnimation );
	}

	#endregion // Public Functions
}
