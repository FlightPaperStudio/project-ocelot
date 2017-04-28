using UnityEngine;
using System.Collections;

public class CameraFit : MonoBehaviour
{
	//Screen information
	private bool canChange = false;
	private float width = 0;
	public float cameraDividend = 10f;
	public Camera cam;
	public bool adjustForVertical = false;

	/// <summary>
	/// Checks for if the window is resized, and updates the camera to fit the aspect ratio.
	/// </summary>
	private void Update ( )
	{
		//Adjust camera on window resize
		if ( canChange )
			AdjustCamera ( );
		
		//Check for window resize
		if ( width != Screen.width )
			canChange = true;
	}
	
	/// <summary>
	/// Updates the camera to fit the aspect ratio.
	/// </summary>
	private void AdjustCamera ( )
	{
		//Store screen width
		canChange = false;
		width = Screen.width;
		
		//Adjust camera
		cam.orthographicSize = cameraDividend / cam.aspect;

		//Check for vertical adjustment
		if ( adjustForVertical )
			cam.orthographicSize += 3;
	}
}
