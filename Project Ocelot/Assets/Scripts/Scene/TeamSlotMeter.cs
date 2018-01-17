using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TeamSlotMeter : MonoBehaviour
{
	#region UI Elements

	public Image [ ] slots;

	#endregion // UI Elements

	#region Meter Data

	/// <summary>
	/// The number of slots currently filled.
	/// </summary>
	public int FilledSlots
	{
		get;
		private set;
	}
	
	/// <summary>
	/// The number of slots currently being previewed.
	/// </summary>
	public int PreviewedSlots
	{
		get;
		private set;
	}

	/// <summary>
	/// The total number of slots available.
	/// </summary>
	public int TotalSlots
	{
		get
		{
			return slots.Length;
		}
	}

	private List<Tween> slotAnimations = new List<Tween> ( );
	private const float ANIMATION_TRANSPARENCY = 0.5f;
	private const float ANIMATION_DURATION = 0.5f;
	private readonly Color32 SLOT_COLOR = new Color32 ( 255, 210, 75, 255 );

	#endregion // Meter Data

	#region Public Functions

	/// <summary>
	/// Resets the slot meter to the start (1 by default for the Leader Unit).
	/// </summary>
	public void ResetMeter ( )
	{
		// Set meter to 1
		SetMeter ( 1 );
	}

	/// <summary>
	/// Sets the slot meter to a specified value.
	/// </summary>
	/// <param name="fillSlots"> The number of slots that are filled. </param>
	public void SetMeter ( int fillSlots )
	{
		// Remove any slot previews
		EndSlotPreview ( );

		// Check that the slot index is within range
		if ( fillSlots < 1 ) // The meter always has at least one slot filled for the Leader Unit
			fillSlots = 1;
		else if ( fillSlots > TotalSlots )
			fillSlots = TotalSlots;

		// Store the number of filled slots
		FilledSlots = fillSlots;

		// Display all filled slots
		for ( int i = 0; i < slots.Length; i++ )
			slots [ i ].gameObject.SetActive ( i < FilledSlots );
	}

	/// <summary>
	/// Animates a number of slots that could be filled from the current unit.
	/// </summary>
	/// <param name="numberOfSlots"> The number of slots for the current unit. </param>
	public void PreviewSlots ( int numberOfSlots )
	{
		// Check that there are enough slots to preview
		if ( numberOfSlots < 0 )
			numberOfSlots = 0;
		else if ( numberOfSlots > TotalSlots - FilledSlots )
			numberOfSlots = TotalSlots - FilledSlots;

		// Check if this is a different amout of slots currently being previewed
		if ( numberOfSlots != PreviewedSlots )
		{
			// Remove previous slot previews
			EndSlotPreview ( );

			// Store the number of slots being previewed
			PreviewedSlots = numberOfSlots;

			// Animate the previewed number of slots
			for ( int i = FilledSlots; i < FilledSlots + PreviewedSlots; i++ )
			{
				// Display marker
				slots [ i ].gameObject.SetActive ( true );

				// Reset marker color
				slots [ i ].color = SLOT_COLOR;

				// Create animation
				Tween t = slots [ i ].DOFade ( ANIMATION_TRANSPARENCY, ANIMATION_DURATION ).SetLoops ( -1, LoopType.Yoyo );

				// Add animation to list
				slotAnimations.Add ( t );
			}
		}
	}

	#endregion // Public Functions

	#region Private Functions

	/// <summary>
	/// Ends any animations for previewed slots.
	/// </summary>
	private void EndSlotPreview ( )
	{
		// End animations
		foreach ( Tween t in slotAnimations )
			t.Kill ( );

		// Reset each marker
		for ( int i = FilledSlots; i < FilledSlots + PreviewedSlots; i++ )
		{
			// Reset marker color
			slots [ i ].color = SLOT_COLOR;

			// Display marker
			slots [ i ].gameObject.SetActive ( false );
		}

		// Clear animation list
		slotAnimations.Clear ( );

		// Clear the number of slots being previewed
		PreviewedSlots = 0;
	}

	#endregion // Private Functions
}
