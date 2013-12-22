/*
Copyright © 2011, Stephan Krohn
All rights reserved.
krohn.stephan@googlemail.com
*/

using UnityEngine;
using System.Collections;

/// <summary>
/// This is used to create a pixel mask overlay to reduce the side effects of an oblique projection.
/// </summary>
[ExecuteInEditMode]
public class CamGroup : MonoBehaviour
{

	#region public
	/// <summary>
	/// Holds all the cameras in this group.
	/// </summary>
	public Camera[] cams;

	/// <summary>
	/// The eye separation (distance from eye to eye) used by this group.
	/// </summary>
	public float eyeSeparation;

	#endregion

	void Start()
	{

	}

	#region properties

	/// <summary>
	/// The virtual screen that the cams of this group should be looking through.
	/// </summary>
	public VirtualScreen Screen
	{
		get { return screen; }
		set
		{
			// shortcut to save performance
			if (screen == value)
				return;

			screen = value;

			// update all cams that belong to this group
			for (int i = 0; i < cams.Length; i++)
			{
				if (cams[i] == null)
				{
					Logger.LogError("You didn't assign all cameras in group: " + name);
					return;
				}

				cams[i].GetComponent<ProjectionMatrix>().screen = screen;
			}
		}
	}
	[SerializeField]
	[HideInInspector]
	VirtualScreen screen;

	/// <summary>
	/// The projection mode used by this group.
	/// </summary>
	public ProjectionMode ProjectionMode
	{
		get { return projectionMode; }
		set
		{
			if (projectionMode == value)
				return;

			Logger.Log("set group mode: " + value);
			projectionMode = value;

			foreach (Camera cam in cams)
			{
				if (cam != null && (cam.enabled || Application.isEditor))
				{
					cam.GetComponent<ProjectionMatrix>().ProjectionMode = projectionMode;
				}
			}
		}
	}
	[SerializeField]
	[HideInInspector]
	ProjectionMode projectionMode = ProjectionMode.Standard;

	#endregion


	void LateUpdate()
	{
		// set cams apart from each other using the eye separation
		if (cams.Length == 2)
		{
			cams[0].transform.position = transform.position - transform.right * (eyeSeparation / 2);
			cams[1].transform.position = transform.position + transform.right * (eyeSeparation / 2);
		}
	}
}
