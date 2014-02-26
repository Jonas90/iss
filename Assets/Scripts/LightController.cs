using UnityEngine;
using System.Collections;


//organisiert komplette beleuchtung im projekt
public class LightController : MonoBehaviour
{

	private Light[] roomLights;
	private Light[] globalLights;

	public bool RoomLightIsOn { get; private set; }
	public bool GlobalLightIsOn { get; private set; }

	public static LightController Instance { get; private set; }

	void Awake()
	{
		Instance = this;
		RoomLightIsOn = true;
		GlobalLightIsOn = false;
	}

	void Start()
	{
		roomLights = GameObject.Find("RoomLight").GetComponentsInChildren<Light>();
		globalLights = GameObject.Find("GlobalLight").GetComponentsInChildren<Light>();
	}


	public void setGlobalLights(bool state)
	{
		this.GlobalLightIsOn = state;

		foreach (Light light in globalLights)
			light.enabled = state;

		if (this.GlobalLightIsOn)
			RenderSettings.ambientLight = new Color(1f, 1f, 1f);
		else
			RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f);

		GameObject[] balls = GameObject.FindGameObjectsWithTag("Projektil");
		foreach (GameObject ball in balls)
			ball.GetComponentInChildren<Light>().enabled = !this.GlobalLightIsOn && !this.RoomLightIsOn;
	}


	public void setRoomLights(bool state)
	{
		foreach (Light light in roomLights)
			light.enabled = state;

		this.RoomLightIsOn = state;

		GameObject[] balls = GameObject.FindGameObjectsWithTag("Projektil");
		foreach (GameObject ball in balls)
			ball.GetComponentInChildren<Light>().enabled = !this.GlobalLightIsOn && !this.RoomLightIsOn;
	}
}
