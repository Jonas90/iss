/*
Copyright © 2011, Stephan Krohn
All rights reserved.
krohn.stephan@googlemail.com
*/

using UnityEngine;
using System.IO;
using System.Collections;

/// <summary>
/// Manages the different clients, groups and cams that are part of the cluster-rendering.
/// It assigns each client a specific camera to render. The order of the assignment is determined by
/// a ipConfig.Instance file. The Config.Instance file should provide one IP-Address per line. Each cam automatically
/// gets a camID.
/// The camID is determined by the following equation: groupID * groupLength + groupIndex. Where groupID is
/// index of the CamGroup in the camGroups array, groupIndex is the index of the camera in CamGroup.cams and
/// groupLength is the number of cams per group. If in doubt you can see the automatically assigned camID in
/// each cams Inspector.
/// </summary>
public class CamManager : MonoBehaviour
{

	#region public

	// array of CamGroups. Normally there should be 2 cams (right/left) per group
	public CamGroup[] camGroups;
	public readonly int camsPerGroup = 2;

	#endregion

	#region properties
	/// <summary>
	/// Return the currently active camera on this machine.
	/// </summary>
	public int ActiveCam
	{
		get { return activeCam; }
	}
	int activeCam;
	NetworkObserver observer;

	/// <summary>
	/// The curren ProjectionMode. This is applied to ALL cameras. If you want to
	/// change some of the cameras use the CamGroup or ProjectionMatrix property.
	/// </summary>
	public ProjectionMode ProjectionMode
	{
		get { return projectionMode; }
		set
		{
			if (projectionMode == value)
				return;

			// if we are connected to the network use RPC to switch cams on clients
			if (Network.connections.Length > 0)
				//networkView.RPC("SwitchProjectionMode", RPCMode.All, (int)value);
				observer.SendSwitchProjectionMode((int)value);
			else
				// if not do a local switch
				SwitchProjectionMode((int)value);
		}
	}
	[HideInInspector]
	public ProjectionMode projectionMode;

	/// <summary>
	/// Adjust eye separation for ALL cams.
	/// </summary>
	public float EyeSeparation
	{
		get
		{
			return eyeSeparation;
		}
		set
		{
			if (eyeSeparation == value)
				return;

			if (Network.isServer)
				//networkView.RPC("SetEyeSeparation", RPCMode.All, value);
				observer.SendEyeSeperation(value);
			else
				SetEyeSeparation(value);
		}
	}
	[HideInInspector]
	public float eyeSeparation;

	public PixelMask Mask
	{
		get { return mask; }
		set { mask = value; }
	}
	PixelMask mask;

	#endregion

	#region unity
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		observer = (NetworkObserver)GameObject.FindObjectOfType(typeof(NetworkObserver));
		
		if (Config.Instance.Mode == Config.AppMode.CAVE_RENDERER)
		{
			SwitchToCam(Config.Instance.CaveNode -1); // Die Cams laufen in [0..7], CaveNode [1..8]
		}
	}

	void OnGUI()
	{
		if (Network.isServer)
		{
			foreach (NetworkPlayer player in Network.connections)
			{
				GUILayout.Label(player + ": " + player.ipAddress + ":" + player.port);
			}
		}
		else if (Debug.isDebugBuild)
		{
			//anzeige ob Disconnect oder nicht..
			GUILayout.Box("Verbindungsstatus: " + Network.peerType);
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Logger.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	/// <summary>
	/// If connection failed tell us why, but don't crash...
	/// </summary>
	/// <param name="error">
	/// A <see cref="NetworkConnectionError"/>
	/// </param>
	void OnFailedToConnect(NetworkConnectionError error)
	{
		Logger.LogError("OnFailedToConnect: " + error.ToString());
	}

	/// <summary>
	/// If we are server we will render the first Camera. (normally CenterLeft)
	/// </summary>
	void OnServerInitialized()
	{
		if (!Config.Instance.IsStandalone)
		{
			Logger.Log("Wir sind Server. Wechseln zu Cam0.");
			SwitchToCam(0);
		}
		else
		{
			//für Single Spiele ohne Cave aber server.
			Logger.Log("Standalone daher kein Cam wechsel.");
		}
	}

	#endregion

	#region private methods

	/// <summary>
	/// Returns the currently active camera of this client.
	/// </summary>
	/// <returns>
	/// A <see cref="Camera"/>
	/// </returns>
	Camera GetActiveCamera()
	{
		return camGroups[activeCam / camsPerGroup].cams[activeCam % camsPerGroup] as Camera;
	}

	/// <summary>
	/// Read the ipConfig.Instance file. This should be done before any client connects to the server.
	/// </summary>
	/*
	void LoadHosts()
	{
		Logger.Log("Load Hosts");
		StreamReader reader = new StreamReader(Config.Instance.ipConfigFile);

		ArrayList ips = new ArrayList();
		string line = reader.ReadLine();

		while (line != null)
		{
			ips.Add(line);
			Logger.Log("add: " + line);
			line = reader.ReadLine();
		}
		// hier eventuell andere Clients noch eintragen die mitspielen drüfen..
		if (ips.Count > 0)
			Config.Instance.ipAddresses = ips.ToArray(typeof(string)) as string[];
	}*/

	#endregion

	#region RPC

	/// <summary>
	/// Set the eye separation for ALL CamGroups. Measured in meters.
	/// </summary>
	/// <param name="separation">
	/// A <see cref="System.Single"/>
	/// </param>
	public void SetEyeSeparation(float separation)
	{
		eyeSeparation = separation;

		CamGroup[] groups = FindObjectsOfType(typeof(CamGroup)) as CamGroup[];
		foreach (CamGroup group in groups)
		{
			group.eyeSeparation = eyeSeparation;
		}
	}

	/// <summary>
	/// Switch on a single camera.
	/// Switch all other cams off.
	/// </summary>
	/// <param name='camID'>
	/// The id of the camera to switch on.
	/// </param>
	public void SwitchToCam(int camID)
	{
		// Activate the cam whose id is camID.
		// Deactivate all others.
		int numCams = camGroups.Length * camsPerGroup;
		for (int i = 0; i < numCams; i++)
		{
			if (i == camID)
			{
				camGroups[i / camsPerGroup].cams[i % camsPerGroup].enabled = true;
				camGroups[i / camsPerGroup].cams[i % camsPerGroup].GetComponent<ProjectionMatrix>().enabled = true;
			}
			else
			{
				camGroups[i / camsPerGroup].cams[i % camsPerGroup].enabled = false;
				camGroups[i / camsPerGroup].cams[i % camsPerGroup].GetComponent<ProjectionMatrix>().enabled = false;
			}
		}
		activeCam = camID;
		Logger.Log("switch to cam " + camID);
	}

	/// <summary>
	/// Set ProjectionMode for ALL CamGroups.
	/// </summary>
	/// <param name="mode">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void SwitchProjectionMode(int mode)
	{
		projectionMode = (ProjectionMode)mode;
		CamGroup[] groups = GetComponentsInChildren<CamGroup>();
		foreach (CamGroup group in groups)
		{
			group.ProjectionMode = projectionMode;
		}
	}

	#endregion
}
