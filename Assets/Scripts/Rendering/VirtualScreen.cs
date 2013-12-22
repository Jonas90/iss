/*
Copyright © 2011, Stephan Krohn
All rights reserved.
krohn.stephan@googlemail.com
*/

using UnityEngine;
using System.Collections;

/// <summary>
/// This component creates a virtual "screen" that can be placed anywhere in the scene
/// and can be used by the ProjectionMatrix as a window to create asymmetric frustums.
/// </summary>
[ExecuteInEditMode]
public class VirtualScreen : MonoBehaviour
{
	#region public

	/// <summary>
	/// The display type defines how to set the measurements of the screen.
	/// There are two possible types:
	/// Wall - specify width and height (in meters)
	/// Screen - specify aspect ratio and diagonal (in inches)
	/// </summary>
	public DisplayType displayType;

	// use this for Screen mode
	public Vector2 aspectRatio;
	public float screenSize;

	// use this for Wall mode
	public Vector2 size;

	// provide direct access to all points (better performance)
	public Vector3 tl;
	public Vector3 tr;
	public Vector3 br;
	public Vector3 bl;

	[HideInInspector]
	public Vector3[] origPoints = new Vector3[4];

	#endregion

	#region private

	Vector3 lastPosition;
	Quaternion lastRotation;

	#endregion

	#region unity

	/// <summary>
	/// Draw the in editor representation.
	/// </summary>
	void OnDrawGizmos()
	{
		Gizmos.DrawLine(tl, tr);
		Gizmos.DrawLine(tr, br);
		Gizmos.DrawLine(br, bl);
		Gizmos.DrawLine(bl, tl);
	}

	#endregion

	#region public methods

	/// <summary>
	/// Call this function to update the corners of the screen based on the current transform.
	/// This needs to be called everytime after the object was moved!
	/// </summary>
	public void UpdateCorners()
	{
		// we only need to update if position changed
		if (transform.position != lastPosition || lastRotation != transform.rotation)
		{
			tl = transform.TransformPoint(origPoints[0]);
			tr = transform.TransformPoint(origPoints[1]);
			br = transform.TransformPoint(origPoints[2]);
			bl = transform.TransformPoint(origPoints[3]);

			lastPosition = transform.position;
			lastRotation = transform.rotation;
		}
	}

	/// <summary>
	/// Adjust the size of the screen.
	/// </summary>
	/// <param name="width">
	/// A <see cref="System.Single"/>
	/// Width in meters.
	/// </param>
	/// <param name="height">
	/// A <see cref="System.Single"/>
	/// Height in meters.
	/// </param>
	public void SetSize(float width, float height)
	{
		origPoints[0] = new Vector3(-width / 2, height / 2, 0);
		origPoints[1] = new Vector3(width / 2, height / 2, 0);
		origPoints[2] = new Vector3(width / 2, -height / 2, 0);
		origPoints[3] = new Vector3(-width / 2, -height / 2, 0);

	}

	/// <summary>
	/// Update size of screen. This is for DisplayType Screen only.
	/// </summary>
	public void UpdateScreenSize()
	{
		if (displayType == DisplayType.Screen)
			size = CalculateScreenSize(screenSize, aspectRatio.x / aspectRatio.y);

		SetSize(size.x, size.y);
	}

	#endregion

	/// <summary>
	/// Calculate the real measurements in meters from aspect ratio and screen diagonal in inches.
	/// </summary>
	/// <param name="diagonal">
	/// A <see cref="System.Single"/>
	/// Screen diagonal in inches.
	/// </param>
	/// <param name="aspectRatio">
	/// A <see cref="System.Single"/>
	/// Aspect ratio in units. (e.g. 16:10 = 1.6f)
	/// </param>
	/// <returns>
	/// A <see cref="Vector2"/>
	/// The measurement of a screen in meters.
	/// </returns>
	Vector2 CalculateScreenSize(float diagonal, float aspectRatio)
	{
		Vector2 size = new Vector2();

		// convert to cm
		float d = diagonal * 2.54f;
		d /= 100;

		size.x = Mathf.Sin(Mathf.Atan(aspectRatio)) * d;
		size.y = Mathf.Cos(Mathf.Atan(aspectRatio)) * d;

		return size;
	}
}

public enum DisplayType
{
	Screen,
	Wall
}
