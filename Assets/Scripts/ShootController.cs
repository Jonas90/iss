using UnityEngine;
using System.Collections;

public class ShootController : MonoBehaviour
{
	public static float speed = 500;
	public GameObject munition;
	public int maxNumberOfBalls = 50;
	public int maxNumberOfBallsStandalone = 300;

	private Transform munitionParent;
	private static int id = 0;
	//private Transform cameraRotationTransform;

	public GameObject soundThrowBall;

	void Start()
	{
		GameObject go = new GameObject("Munition");
		munitionParent = go.transform;
		//cameraRotationTransform = transform.FindChild("CAVEStereoHeadNew/CameraTransform");

	}
	/// <summary>
	/// Spawns the munition.
	/// </summary>
	/// <returns>
	/// The munition.
	/// </returns>
	/// <param name='startPosition'>
	/// Start position.
	/// </param>
	/// <param name='Force'>
	/// Force / Wurfrichtung.
	/// </param>
	public GameObject spawnMunition(Vector3 startPosition, Vector3 Force)
	{

		GameObject munitionInstance = Instantiate(munition, startPosition, Quaternion.Euler(Vector3.zero)) as GameObject;
		munitionInstance.transform.parent = munitionParent;
		munitionInstance.rigidbody.AddForce(Force);
		munitionInstance.name = "Munition" + id;
		munitionInstance.GetComponentInChildren<Light>().enabled = !LightController.Instance.RoomLightIsOn && !LightController.Instance.GlobalLightIsOn;
		id++;

		//sound
		Instantiate(soundThrowBall, this.transform.position, this.transform.rotation);

		return munitionInstance;
	}
}
