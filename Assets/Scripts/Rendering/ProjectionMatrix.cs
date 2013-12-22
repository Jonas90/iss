/*
Copyright © 2011, Stephan Krohn
All rights reserved.
krohn.stephan@googlemail.com
*/

using UnityEngine;

using System.IO;
using System.Collections;


/// <summary>
/// Set an off-center projection, where perspective's vanishing
/// point is not necessarily in the center of the screen.
///
/// left/right/top/bottom define near plane size, i.e.
/// how offset are corners of camera's near plane.
/// </summary>

//[ExecuteInEditMode]
public class ProjectionMatrix : MonoBehaviour
{
	#region properties

	/// <summary>
	/// The projection mode defines how the matrix is calculated.
	/// Standard - Standard projection matrix, no extra calculations.
	/// Oblique - Standard projection matrix + Homography
	/// Async - off-center projetcion matrix
	/// ObliqueAsync - off-center projection matrix + Homography
	/// </summary>
	public ProjectionMode ProjectionMode
	{
		get { return projectionMode; }
		set
		{
			Logger.Log("set projection mode to: " + value);
			if (value != projectionMode)
			{
				projectionMode = value;

				// Load matrix from file if we are using an oblique projection mode
				if (projectionMode == ProjectionMode.ObliqueAsync || projectionMode == ProjectionMode.Oblique)
					LoadHomographyMatrix();
				// remove mask for standard(not oblique) projection modes
				else
					PixelMask.Instance.gameObject.SetActive(false);
			}
		}
	}
	[HideInInspector]
	public ProjectionMode projectionMode = ProjectionMode.Standard;

	CamManager Manager
	{
		get
		{
			if (manager == null)
			{
				manager = FindObjectOfType(typeof(CamManager)) as CamManager;
			}
			return manager;
		}
	}
	CamManager manager; // keep reference to CamManager

	#endregion

	#region public

	public VirtualScreen screen;

	#endregion

	#region private

	// homography matrix
	Matrix4x4 bimberMatrix;

	// off-center matrix
	Matrix4x4 mp;
	Matrix4x4 origProjMatrix;

	Camera cam; // keep reference to camera component (faster)

	#endregion

	#region unity
	void Start()
	{
		// remember the original projectionMatrix
		origProjMatrix = camera.projectionMatrix;
		camera.ResetProjectionMatrix();

		// if we use oblique projection mode load the homography
		if (projectionMode == ProjectionMode.ObliqueAsync || projectionMode == ProjectionMode.Oblique)
		{
			LoadHomographyMatrix();
		}
	}

	void OnPreCull()
	{
		cam = camera;

		// update screen (if defined)
		// this must be called manually to ensure the correct order of execution
		if (screen != null)
		{
			screen.UpdateCorners();
		}
		else
		{
			cam.ResetProjectionMatrix();
		}

		// calculate off-center projection
		if (projectionMode == ProjectionMode.Async || ProjectionMode == ProjectionMode.ObliqueAsync)
		{
			NearPlane plane = CalculateNearPlane(screen, camera, transform);
			mp = PerspectiveOffCenter(plane);
		}

		// choose the correct projection matrix
		switch (projectionMode)
		{
			case ProjectionMode.Standard:
				cam.ResetProjectionMatrix();
				break;
			case ProjectionMode.Oblique:
				cam.projectionMatrix = bimberMatrix * origProjMatrix;
				break;
			case ProjectionMode.Async:
				cam.projectionMatrix = mp;
				break;
			case ProjectionMode.ObliqueAsync:
				cam.projectionMatrix = bimberMatrix * mp;
				break;
		}
	}

	#endregion

	#region public methods

	/// <summary>
	/// Loads a previously stored homography matrix from file.
	/// </summary>
	public void LoadHomographyMatrix()
	{
		string matrixFileName = Config.Instance.DeviceCoordinateFileName;
		Logger.Log("Lade " + System.IO.Path.GetFullPath(matrixFileName));

		// open file
		try
		{
			StreamReader reader = new StreamReader(new FileStream(matrixFileName, FileMode.Open));
			bimberMatrix = new Matrix4x4();

			for (int i = 0; i < 16; i++)
			{
				bimberMatrix[i] = float.Parse(reader.ReadLine());
			}

			// load ndc's
			Vector2[] points = new Vector2[4];

			for (int i = 0; i < 4; i++)
			{
				points[i].x = float.Parse(reader.ReadLine());
				points[i].y = float.Parse(reader.ReadLine());

				// remap to coordinates between 0 and 1
				points[i] += Vector2.one;
				points[i] /= 2;
			}
			reader.Close();

			// create pixel mask if neccessary
			PixelMask.Instance.CreateMask(new Quad(points));
		}
		catch (System.IO.IsolatedStorage.IsolatedStorageException e)
		{
			PixelMask.Instance.gameObject.SetActive(false);
			Logger.LogException(e);
		}

	}

	#endregion

	#region private methods

	/// <summary>
	/// Calculate a projection matrix from a given near plane.
	/// </summary>
	/// <param name="np">
	/// A <see cref="NearPlane"/>
	/// The NearPlane that defines the projection.
	/// </param>
	/// <returns>
	/// A <see cref="Matrix4x4"/>
	/// The projection matrix.
	/// </returns>
	Matrix4x4 PerspectiveOffCenter(NearPlane np)
	{
		float x = (2.0f * np.near) / (np.right - np.left);
		float y = (2.0f * np.near) / (np.top - np.bottom);
		float a = (np.right + np.left) / (np.right - np.left);
		float b = (np.top + np.bottom) / (np.top - np.bottom);
		float c = -(np.far + np.near) / (np.far - np.near);
		float d = -(2.0f * np.far * np.near) / (np.far - np.near);
		float e = -1.0f;

		Matrix4x4 m = new Matrix4x4();

		m[0, 0] = x;
		m[0, 1] = 0;
		m[0, 2] = a;
		m[0, 3] = 0;
		m[1, 0] = 0;
		m[1, 1] = y;
		m[1, 2] = b;
		m[1, 3] = 0;
		m[2, 0] = 0;
		m[2, 1] = 0;
		m[2, 2] = c;
		m[2, 3] = d;
		m[3, 0] = 0;
		m[3, 1] = 0;
		m[3, 2] = e;
		m[3, 3] = 0;

		return m;
	}

	/// <summary>
	/// Calculate a near plane that defines a projection looking through a virtual window/screen.
	/// </summary>
	/// <param name="window">
	/// A <see cref="VirtualScreen"/>
	/// The virtual window the frustum should be locked to.
	/// </param>
	/// <param name="cam">
	/// A <see cref="Camera"/>
	/// The camera for which the projection is calculated.
	/// </param>
	/// <param name="t">
	/// A <see cref="Transform"/>
	///
	/// </param>
	/// <returns>
	/// A <see cref="NearPlane"/>
	/// </returns>
	NearPlane CalculateNearPlane(VirtualScreen window, Camera cam, Transform t)
	{
		Profiler.BeginSample("CalcNearPlane");

		// align camera with window
		t.rotation = window.transform.rotation;

		// get top left and bottom right coordinates
		Vector3 wtl = window.tl;
		Vector3 wbr = window.br;

		NearPlane plane = new NearPlane();

		// construct nearPlane
		Vector3 nearCenter = t.position + t.forward * cam.nearClipPlane;
		Plane nearPlane = new Plane(-t.forward, nearCenter);

		// calculate top left for near plane
		float dist = 0;
		Vector3 direction = (wtl - t.position).normalized;
		Ray ray = new Ray(t.position, direction);
		nearPlane.Raycast(ray, out dist);
		Vector3 ntl = -(t.InverseTransformPoint(nearCenter) - t.InverseTransformPoint((t.position + direction * dist)));
		plane.left = ntl.x;
		plane.top = ntl.y;


		// calculate bottom right for near plane
		direction = (wbr - t.position).normalized;
		ray = new Ray(t.position, direction);
		nearPlane.Raycast(ray, out dist);
		Vector3 nbr = -(t.InverseTransformPoint(nearCenter) - t.InverseTransformPoint((t.position + direction * dist)));
		plane.right = nbr.x;
		plane.bottom = nbr.y;
		plane.near = cam.nearClipPlane;
		plane.far = cam.farClipPlane;

		Profiler.EndSample();

		return plane;
	}

	#endregion
}

#region structs

/// <summary>
/// Helper struct that holds the parameters of a near plane
/// needed for calculation of a projection matrix.
/// </summary>
public struct NearPlane
{
	public float left;
	public float right;
	public float top;
	public float bottom;
	public float near;
	public float far;
}

#endregion

#region enums

public enum ProjectionMode
{
	Standard,
	Oblique,
	Async,
	ObliqueAsync
}

#endregion
