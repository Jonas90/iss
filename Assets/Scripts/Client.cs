//6. Etage

//dan
//Für die ISS 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cave;

public class Client
{

	private GameController game;
	private GameObject player;

	private GameObject playerAvatar = null;
	private GameObject playerEAvatar = null;
	
	/* dan
	private LightController lightController;
	private Enemy[] enemies;
	*/
	 
	private NetworkObserver observer;
	private Transform cameraRotationTransform;
	private KinectController kinectController;
	private bool keyboardLastButtonState = false;
	private WiiController wiiController;


	public Client (GameController game)
	{
		this.game = game;
		this.player = GameObject.Find ("Player");
		
		
		
		/* dan
		this.lightController = ((LightController)GameObject.FindObjectOfType(typeof(LightController)));
		this.enemies = (Enemy[])GameObject.FindObjectsOfType(typeof(Enemy));
		 */
		 
		this.playerAvatar = GameObject.Find("AvatarMainPlayer");
		this.playerEAvatar = GameObject.Find("AvatarEnemyPlayer");

		if (!Config.Instance.IsStandalone)
		{	//CaveClient
			disableControl();
			disableAllRigidBodys();
		}
		else
		{	//GameClient version
			this.observer = (NetworkObserver)GameObject.FindObjectOfType(typeof(NetworkObserver));
			this.wiiController = (WiiController)GameObject.FindObjectOfType(typeof(WiiController));
			this.kinectController = (KinectController)GameObject.FindObjectOfType(typeof(KinectController));
			this.cameraRotationTransform = ((KinectController)GameObject.FindObjectOfType(typeof(KinectController))).transform;
		}
	}

	/// <summary>
	/// Disables all rigid bodys.
	/// </summary>
	private void disableAllRigidBodys()
	{
		Rigidbody[] allRigids = (Rigidbody[])GameObject.FindObjectsOfType(typeof(Rigidbody));
		Collider[] allCollider = (Collider[])GameObject.FindObjectsOfType(typeof(Collider));

		Logger.Log("found " + allRigids.Length + " Rigidbodys and " + allCollider.Length + " Collider");

		foreach (Rigidbody body in allRigids)
			GameObject.Destroy(body);
		foreach (Collider col in allCollider)
			GameObject.Destroy(col);
	}

	/// <summary>
	/// Disables the control.
	/// </summary>
	private void disableControl()
	{
		player.GetComponent<MovementController>().enabled = false;
	}
	public void Update()
	{
		updateControls();
	}
	/// <summary>
	/// Updates the rotation from Enemy.
	/// </summary>
	/// <param name='modelRotation'>
	/// Model rotation.
	/// </param>
	public void updateRota(Quaternion modelRotation)
	{
		playerEAvatar.transform.rotation = modelRotation;
	}
	/// <summary>
	/// Updates the position from Enemy.
	/// </summary>
	/// <param name='modelPosition'>
	/// Model position.
	/// </param>
	public void updatePosition(Vector3 modelPosition)
	{
		PhysicModel model = (PhysicModel)game.PhysicModels["EnemyPlayer"];
		model.transform.position = modelPosition;
	}
	/// <summary>
	/// Updates the controls. (ausser Bewegung)
	/// </summary>
	private void updateControls ()
	{
		//bool keyboardToggleOn = (Input.GetKey (Config.Instance.keyboardButtonLight) && !keyboardLastButtonState);

		if (Config.Instance.UseWii && wiiController.WiiMote.getButtonState (Config.Instance.wiiButtonShoot) == ButtonState.TOGGLE_DOWN || Input.GetKey (KeyCode.Space) || Input.GetMouseButtonDown (0)) {
			Vector3 Force; //wurfkraft, da Kinect bei Client muss es nun durchgegeben werden.
			Force = playerAvatar.transform.forward;	//richtung
			Force.y = 0.3f;		//etwas nach oben

			if (Config.Instance.UseKinect)
				Force = cameraRotationTransform.TransformDirection (kinectController.HeadToHandDirection);
			
			
			/* dan
			//server mitteilen das wurftaste gedrückt wurde
			observer.SendWillShoot (cameraRotationTransform.TransformPoint (kinectController.HandPosition), Force);
			*/
		}
		
		/* dan
		if (Config.Instance.UseWii && wiiController.WiiMote.getButtonState (Config.Instance.wiiButtonLight) == ButtonState.TOGGLE_DOWN || keyboardToggleOn)
			observer.SendWillSwitchGlobalLight ();
		*/

		keyboardLastButtonState = Input.GetKey (Config.Instance.keyboardButtonLight);
	}

	/// <summary>
	/// Receives the physic data for all Models.
	/// </summary>
	/// <param name='modelName'>
	/// Model name.
	/// </param>
	/// <param name='modelPosition'>
	/// Model position.
	/// </param>
	/// <param name='modelRotation'>
	/// Model rotation.
	/// </param>
	public void receivePhysicData (string modelName, Vector3 modelPosition, Quaternion modelRotation)
	{
		if (game.PhysicModels.ContainsKey (modelName)) {
			//Game Client
			if (Config.Instance.IsStandalone && modelName == "Player")
				modelName = "EnemyPlayer";
			//Game Client (alle Kamera bewegungen rausfiltern)
			if (Config.Instance.IsStandalone && modelName != "AvatarMainPlayer" && modelName != "CamHolder" && modelName != "CAVEStereoHeadNew" && modelName != "CameraTransform" && modelName != "EnemyPlayer") {
				PhysicModel model = (PhysicModel)game.PhysicModels [modelName];
				model.transform.position = modelPosition;
				model.transform.rotation = modelRotation;
			}
			if (!Config.Instance.IsStandalone) { //CaveClient
				PhysicModel model = (PhysicModel)game.PhysicModels [modelName];
				model.transform.position = modelPosition;
				model.transform.rotation = modelRotation;
			}
		}
	}
	
	/* dan
	/// <summary>
	/// Spawns the shot.
	/// </summary>
	/// <param name='spawnPosition'>
	/// Spawn position.
	/// </param>
	/// <param name='Force'>
	/// Force.
	/// </param>
	public void spawnShot (Vector3 spawnPosition, Vector3 Force)
	{
		GameObject shot = player.GetComponent<ShootController> ().spawnMunition (spawnPosition, Force);
		GameObject.DestroyImmediate (shot.rigidbody);
		GameObject.DestroyImmediate (shot.collider);
	}
	
	public void actLightState(bool roomLight, bool globalLight)
	{
		lightController.setRoomLights(roomLight);
		lightController.setGlobalLights(globalLight);
	}

	/// <summary>
	/// Kills the shots. (bei zuvielen bällen)
	/// </summary>
	/// <param name='name'>
	/// Name.
	/// </param>
	public void killShots(string name)
	{
		if (!game.PhysicModels.ContainsKey(name))
			Logger.Log("couldnt delete " + name + " (not found)");

		else
		{
			PhysicModel model = (PhysicModel)game.PhysicModels[name];
			GameObject.Destroy(model.gameObject);
		}
	}

	#region enemy hit handling
	/// <summary>
	/// Hits the enemy.
	/// </summary>
	/// <param name='name'>
	/// Name.
	/// </param>
	public void hitEnemy(string name)
	{
		foreach (Enemy enemy in enemies)
			if (enemy.Name == name)
			{
				enemy.hit();
				break;
			}
	}

	/// <summary>
	/// 	Plays the sound on hitting an enemy.
	/// </summary>
	public void hitEnemySound()
	{
		EnemyHitTrigger objEnemyHitTrigger = (EnemyHitTrigger)GameObject.FindObjectOfType(typeof(EnemyHitTrigger));

		objEnemyHitTrigger.playHitEnemySound();
	}
	#endregion

	#region Open door sound handling
	/// <summary>
	/// 	Plays the sound on opening the door.
	/// </summary>
	public void openDoor()
	{
		DoorTrigger objDoorTrigger = (DoorTrigger)GameObject.FindObjectOfType(typeof(DoorTrigger));

		objDoorTrigger.playOpenDoorSound();
	}
	#endregion

	#region Close door sound handling
	/// <summary>
	/// 	Plays the sound on closing the door.
	/// </summary>
	public void closeDoor()
	{
		DoorTrigger objDoorTrigger = (DoorTrigger)GameObject.FindObjectOfType(typeof(DoorTrigger));

		objDoorTrigger.playCloseDoorSound();
	}
	#endregion

	#region Wall sound handling
	/// <summary>
	/// 	Plays the sound for the wall.
	/// </summary>
	public void wallSound()
	{
		ButtonWallTrigger objButtonWallTrigger = (ButtonWallTrigger)GameObject.FindObjectOfType(typeof(ButtonWallTrigger));

		objButtonWallTrigger.playWallSound();
	}
	#endregion
*/

	public void reset ()
	{
		/* dan
		foreach (Enemy enemy in enemies)
			enemy.reset( );
		*/
	}
}
