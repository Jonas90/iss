//6.E
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Cave;

public class Server
{
    private NetworkObserver observer;
    /* dan private ShootController shootController; */
    private WiiController wiiController;
    private Game game;
    private bool isStandalone;
    private Transform cameraRotationTransform;
    private KinectController kinectController;
    private bool keyboardLastButtonState = false;
 
    /* dan
 private LightController lightController;
 private List<GameObject> balls; 
 private Enemy[] enemies;
 private DoorTrigger[] doors;
 private int numberOfBalls;
 */
  
  
    private GameObject playerEAvatar = null;
    //private GameObject player = null;
    //private GameObject playerAvatar = null;

    public Server ( Game game, bool isStandalone )
    {

        this.observer = (NetworkObserver) GameObject.FindObjectOfType ( typeof(NetworkObserver) );
        /* dan this.shootController = ((ShootController)GameObject.FindObjectOfType(typeof(ShootController))); */
        this.kinectController = (KinectController) GameObject.FindObjectOfType ( typeof(KinectController) );
        this.cameraRotationTransform = ( (KinectController) GameObject.FindObjectOfType ( typeof(KinectController) ) ).transform;
        this.wiiController = (WiiController) GameObject.FindObjectOfType ( typeof(WiiController) );
        /* dan this.lightController = ((LightController)GameObject.FindObjectOfType(typeof(LightController))); */
        this.game = game;
        this.isStandalone = isStandalone;
     
        /* dan
     this.enemies = (Enemy[])GameObject.FindObjectsOfType(typeof(Enemy));
     this.doors = (DoorTrigger[])GameObject.FindObjectsOfType(typeof(DoorTrigger));

     this.balls = new List<GameObject>();
     this.numberOfBalls = (Config.Instance.IsStandalone) ? shootController.maxNumberOfBallsStandalone : shootController.maxNumberOfBalls;
     
     */
        this.playerEAvatar = GameObject.Find ( "AvatarEnemyPlayer" );
    }


    public void Update ()
    {
        try
           {
            updateControls ();
        }
        catch ( Exception e )
        {
            Logger.LogException ( e );
        }
    }


    public void FixedUpdate ()
    {
        try
           {
            updateModels ();
        }
        catch ( Exception e )
        {
            Logger.LogException ( e );
        }
    }


    private void updateModels ()
    {
        if ( Network.peerType == NetworkPeerType.Disconnected )
        {
            return;
        }  //wenn nicht verbunden

        foreach ( PhysicModel model in game.PhysicModels.Values )
        {
            if ( model.ShouldBeUpdated () )
            {
                UpdateInformation update = model.GetUpdateInformations ();
                observer.SendPhysicData ( model.name, update.position, update.rotation );

            }
        }
    }
    /// <summary>
    /// Updates the rotation from the Enemy.
    /// </summary>
    /// <param name='modelRotation'>
    /// Model rotation.
    /// </param>
    public void updateRota ( Quaternion modelRotation )
    {
        playerEAvatar.transform.rotation = modelRotation;
    }

    /// <summary>
    /// Updates the position from the Enemy.
    /// </summary>
    /// <param name='modelPosition'>
    /// Model position.
    /// </param>
    public void updatePosition ( Vector3 modelPosition )
    {
        //PhysicModel model = (PhysicModel)game.PhysicModels["EnemyPlayer"];
        //model.transform.position = modelPosition;
        playerEAvatar.transform.position = modelPosition;
    }

    /// <summary>
    /// Updates aller Tasteneingaben (bewegung nicht)
    /// </summary>
    private void updateControls ()
    {
       // bool keyboardToggleOn = ( Input.GetKey ( Config.Instance.keyboardButtonLight ) && !keyboardLastButtonState );

        //ballwurf
        if ( Config.Instance.UseWii && wiiController.WiiMote.getButtonState ( Config.Instance.wiiButtonShoot ) == ButtonState.TOGGLE_DOWN || Input.GetKey ( KeyCode.Space ) || Input.GetMouseButtonDown ( 0 ) )
        {
         
            /* dan
         Vector3 Force;  //wurfkraft, da Kinect bei Client muss es nun durchgegeben werden.
         Force = cameraRotationTransform.forward;    //richtung
         Force.y = 0.3f;     //etwas nach oben
         shoot (cameraRotationTransform.TransformPoint (kinectController.HandPosition), Config.Instance.IsStandalone, Force);
         */
        }
     
        /* dan
     //Lichtwechsel
     if (Config.Instance.UseWii && wiiController.WiiMote.getButtonState (Config.Instance.wiiButtonLight) == ButtonState.TOGGLE_DOWN || keyboardToggleOn)
         switchGlobalLight ();
      */
     
        keyboardLastButtonState = Input.GetKey ( Config.Instance.keyboardButtonLight );
    }
    /* dan
 /// <summary>
 /// Shoot the specified startPosition, isShootStandalone and Force.
 /// </summary>
 /// <param name='startPos'>
 /// Start position.
 /// </param>
 /// <param name='isShootStandalone'>
 /// Is shoot standalone.
 /// </param>
 /// <param name='Force'>
 /// Force inc direction.
 /// </param>
 public void shoot(Vector3 startPos, bool isShootStandalone, Vector3 Force)
 {
     if (!isShootStandalone)
     {   //wenn Cave
         Force = cameraRotationTransform.TransformDirection(kinectController.HeadToHandDirection * ShootController.speed) * 2.0f;
     }
     else
     {
         if (!Config.Instance.UseKinect)
             startPos.y += 0.7f; //bei keiner kinect, da es nicht von den füssen kommen soll
         Force = Force * ShootController.speed * 2f;

     }
     //Ball-limmit kontrollieren
     if (balls.Count == numberOfBalls)
     {
         if (!isStandalone)
             observer.SendKillShot(balls[0].name);
         GameObject.Destroy(balls[0]);
         balls.RemoveAt(0);
     }
     //neuer ball
     balls.Add(shootController.spawnMunition(startPos, Force));
     //wenn nicht alleine Wurf mitteilen
     if (Network.peerType != NetworkPeerType.Disconnected)
         observer.SendShot(startPos, Force);


 }

 /// <summary>
 /// Switchs the global light.
 /// </summary>
 public void switchGlobalLight()
 {
     lightController.setGlobalLights(!LightController.Instance.GlobalLightIsOn);

     if (LightController.Instance.GlobalLightIsOn && LightController.Instance.RoomLightIsOn)
         lightController.setRoomLights(false);
     else if (!LightController.Instance.GlobalLightIsOn && !LightController.Instance.RoomLightIsOn)
         lightController.setRoomLights(true);

     if (isStandalone && Network.peerType == NetworkPeerType.Disconnected)
         return;

     observer.SendLightToggle(LightController.Instance.RoomLightIsOn, LightController.Instance.GlobalLightIsOn);
 }

 /// <summary>
 /// Switchs the room light.
 /// </summary>
 public void switchRoomLight()
 {
     lightController.setRoomLights(!LightController.Instance.RoomLightIsOn);

     if (isStandalone && Network.peerType == NetworkPeerType.Disconnected)
         return;

     observer.SendLightToggle(LightController.Instance.RoomLightIsOn, LightController.Instance.GlobalLightIsOn);
 }
 */
  
    /// <summary>
    /// Reset the Game.
    /// </summary>
    public void reset ()
    {
        resetModelPositions ();
        /* dan killAllBalls (); */
        repositionPlayer ();
        /* dan
     resetAllEnemies ();
     resetAllDoors ();
     */
    }

    /* dan
 private void resetAllDoors()
 {
     foreach (DoorTrigger door in doors)
         door.reset();
 }
 */

    private void resetModelPositions ()
    {
        ICollection models = game.PhysicModels.Values;

        foreach ( PhysicModel model in models )
            model.reset ();
    }

    /* dan
 private void killAllBalls()
 {
     for (int i = 0; i < balls.Count; i++)
     {
         if (!Config.Instance.IsStandalone)
             observer.SendKillShot(balls[i].name);
         GameObject.Destroy(balls[i]);
     }
     balls.Clear();
 }
  */

    private void repositionPlayer ()
    {
        MovementController moveCon = ( (MovementController) GameObject.FindObjectOfType ( typeof(MovementController) ) );
        moveCon.rigidbody.MovePosition ( moveCon.rigidbody.position + Vector3.forward*0.01f );
    }

    /* dan
 private void resetAllEnemies()
 {
     foreach (Enemy enemy in enemies)
         enemy.reset();
 }
 */
}
