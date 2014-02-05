using UnityEngine;
using System.Collections;

using Cave;

public class MovementController : MonoBehaviour, IAvatarAdapter
{
    public float defaultWalkVelocityMeterPerSec = 1.4f; // durchschn. laut Wikipedia
    public float defaultTurnVelocityDegPerSec = 90f; // viertel Kreis pro Sekunde
	
    private float walkVelocityMeterPerSec = 0f;
    private float turnVelocityDegPerSec = 0f;
    private float upVelocityDegPerSec = 0f;
    private float rollVelocityDegPerSec = 0f;
    private WiiController wiiController;
    private KinectController kinectController;
    private GameObject player;
    private GameObject playerAvatar;
    //private SimpleWiiTurnNavigationMediator wiiTurnMediator; // Soll keine Standart konfiguration von der DHL benutzen.
    //private SimpleWiiWalkNavigationMediator wiiWalkMediator; // Soll keine Standart konfiguration von der DHL benutzen.
    private WalkingInPlaceNavigationMediator wipWalkMediator;
    private RedirectToFrontNavigationMediator redirectToFrontTurnMediator;
    private AvatarAdapterConnector avatarConnector;
    private NetworkObserver observer;
    /// <summary>
    /// Start this instance.
    /// alle Bewegungseingaben werden hier behandelt (WASD, oder WII)
    /// </summary>
    void Start ()
    {
        this.InitNavigation ();
        player = GameObject.Find ( "Player" );

        playerAvatar = GameObject.Find ( "AvatarMainPlayer" );
        this.observer = (NetworkObserver) GameObject.FindObjectOfType ( typeof(NetworkObserver) );
    }


    void OnDestroy ()
    {
        //wiiTurnMediator = null;
        //wiiWalkMediator = null;
        wipWalkMediator = null;
        redirectToFrontTurnMediator = null;
        avatarConnector = null;
    }


    private void InitNavigation ()
    {
        avatarConnector = new AvatarAdapterConnector ( this );

        if ( Config.Instance.UseWii )
        {
            wiiController = GetComponentInChildren<WiiController> ();

            //wiiTurnMediator = new SimpleWiiTurnNavigationMediator ( wiiController.WiiMote, avatarConnector );
            // RedirectToFrontTurnMediator benutzt Radianten. Da es den selben Callback setTurnVelocity() (siehe unten)
            // nutzt, muss das hier auch in Radiant sein.
            //wiiTurnMediator.setTargetTurnVelocity ( defaultTurnVelocityDegPerSec*Mathf.Deg2Rad );

            //wiiWalkMediator = new SimpleWiiWalkNavigationMediator ( wiiController.WiiMote, avatarConnector );
           // wiiWalkMediator.setTargetWalkVelocity ( defaultWalkVelocityMeterPerSec );
        }

        if ( Config.Instance.UseKinect )
        {
            kinectController = GetComponentInChildren<KinectController> ();

            wipWalkMediator = new WalkingInPlaceNavigationMediator ( kinectController.Kinect, avatarConnector );

            redirectToFrontTurnMediator = new RedirectToFrontNavigationMediator ( kinectController.Kinect, avatarConnector );
        }

        this.ToggleNav ();
    }


    private void ToggleNav ()
    {
        this.turnVelocityDegPerSec = 0f;
        this.walkVelocityMeterPerSec = 0f;
     
        if ( Config.Instance.UseWii && ( wipWalkMediator == null || wipWalkMediator.isEnabled () ) )
        {
            //wiiTurnMediator.setEnabled ( true );
            //wiiWalkMediator.setEnabled ( true );
            if ( Config.Instance.UseKinect )
            {
                wipWalkMediator.setEnabled ( false );
                redirectToFrontTurnMediator.setEnabled ( false );
            }
        }
        else if ( Config.Instance.UseKinect )//wiiTurnMediator == null || wiiTurnMediator.isEnabled () ) )
            {
                wipWalkMediator.setEnabled ( true );
                redirectToFrontTurnMediator.setEnabled ( true );

                if ( Config.Instance.UseWii )
                {
                    //wiiTurnMediator.setEnabled ( false );
                    //wiiWalkMediator.setEnabled ( false );
                }
            }
    }


    void OnGUI ()
    {
        if ( Config.Instance.UseWii)// && wiiTurnMediator.isEnabled () )
        {
            GUI.Box ( new Rect ( Screen.width - 155, 5, 150, 25 ), "Nav: Wii" );
        }
        else if ( Config.Instance.UseKinect && wipWalkMediator.isEnabled () )
            {
                GUI.Box ( new Rect ( Screen.width - 155, 5, 150, 25 ), "Nav: Körper" );
            }
            else
            {
                GUI.Box ( new Rect ( Screen.width - 155, 5, 150, 25 ), "Nav: Kein" );
            }
    }


    void FixedUpdate ()
    {
        if ( Config.Instance.IsStandalone )
        {
            this.updateKeyboard ();
        }

        if ( Config.Instance.UseKinect )
        {
            redirectToFrontTurnMediator.update ( Time.deltaTime );
        }

        player.rigidbody.velocity = new Vector3 (); // Gegen Sliden, wenn irgendwo gegengelaufen wird

        if ( turnVelocityDegPerSec != 0.0f )
        {
            // Wenn Kinect angeschlossen, können wir um den User rotieren
            if ( Config.Instance.UseKinect )
            {
                Vector3 pos = kinectController.getSensor ( Kinect.TrackerId.HIP_CENTER );
             
                // rotieren um den Benutzer: Zur Position hin...
                player.transform.Translate ( pos );
                // um diese drehen...
                player.transform.Rotate ( Vector3.up, turnVelocityDegPerSec*Time.deltaTime );
                // und wieder zurück.
                player.transform.Translate ( -pos );
            }
            else // Ansonsten um den Mittelpunkt, wie früher.
            {
                player.transform.Rotate ( Vector3.up, turnVelocityDegPerSec*Time.deltaTime );
            }

            playerAvatar.transform.Rotate ( Vector3.up, turnVelocityDegPerSec*Time.deltaTime );

            if ( Network.peerType != NetworkPeerType.Disconnected )
            {
                //eigene rotation Senden
                observer.SendRotate ( playerAvatar.transform.rotation, GameController.Instance.IsServer );
            }
        }

        if (upVelocityDegPerSec != 0.0f)
        {
            player.transform.Rotate(Vector3.right, upVelocityDegPerSec * Time.deltaTime);
        }

        if (rollVelocityDegPerSec != 0.0f)
        {
            player.transform.Rotate(Vector3.forward, rollVelocityDegPerSec * Time.deltaTime);
        }

        if ( walkVelocityMeterPerSec != 0 )
        {
            if ( Config.Instance.UseKinect )
            {
                player.transform.position = ( player.transform.position + player.transform.TransformDirection ( kinectController.CurUserRotation )*Time.deltaTime*walkVelocityMeterPerSec );
            }
            else
            {
                player.transform.position = ( player.transform.position + player.transform.forward*Time.deltaTime*walkVelocityMeterPerSec );
            }

            if ( Network.peerType != NetworkPeerType.Disconnected )
            {
                //eigene Position Senden
                observer.SendPosition ( player.transform.position, GameController.Instance.IsServer );
            }
        }
    }


    private void updateKeyboard ()
    {//
        if ( Input.GetKeyDown ( Config.Instance.keyboardButtonForward ) )
        {
            this.walkVelocityMeterPerSec = this.defaultWalkVelocityMeterPerSec;
        }
        if ( Input.GetKeyDown ( Config.Instance.keyboardButtonBackward ) )
        {
            this.walkVelocityMeterPerSec = -this.defaultWalkVelocityMeterPerSec;
        }
        if ( Input.GetKeyDown ( Config.Instance.keyboardButtonLeft ) )
        {
            this.turnVelocityDegPerSec = -this.defaultTurnVelocityDegPerSec;
        }
        if ( Input.GetKeyDown ( Config.Instance.keyboardButtonRight ) )
        {
            this.turnVelocityDegPerSec = this.defaultTurnVelocityDegPerSec;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            this.upVelocityDegPerSec = -this.defaultTurnVelocityDegPerSec;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            this.upVelocityDegPerSec = this.defaultTurnVelocityDegPerSec;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            this.rollVelocityDegPerSec = -this.defaultTurnVelocityDegPerSec;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.rollVelocityDegPerSec = this.defaultTurnVelocityDegPerSec;
        }

        if ( Input.GetKeyUp ( Config.Instance.keyboardButtonForward ) || Input.GetKeyUp ( Config.Instance.keyboardButtonBackward ) )
        {
            this.walkVelocityMeterPerSec = 0f;
        }
        if ( Input.GetKeyUp ( Config.Instance.keyboardButtonLeft ) || Input.GetKeyUp ( Config.Instance.keyboardButtonRight ) )
        {
            this.turnVelocityDegPerSec = 0f;
        }

        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
        {
            this.upVelocityDegPerSec = 0f;
        }

        if (Input.GetKeyUp(KeyCode.Y) || Input.GetKeyUp(KeyCode.C))
        {
            this.rollVelocityDegPerSec = 0f;
        }
    }

	// ToggleNav (); :=
	// Resetbutton (aus der Config) ist belegt durch Restaeter.cs
    void Update ()
    {
        if ( Config.Instance.UseWii)
		{
			if  (wiiController.WiiMote.getButtonState ( WiiMote.ButtonId.TWO ) == ButtonState.TOGGLE_DOWN )
			{
				// ToggleNav ();
			}

			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.A) == ButtonState.TOGGLE_DOWN)
			{
				this.walkVelocityMeterPerSec = this.defaultWalkVelocityMeterPerSec;
			}
			/*Rückwärtslaufen
			 * if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.STICK_DIGITAL_DOWN) == ButtonState.TOGGLE_DOWN)
			{
				this.walkVelocityMeterPerSec = -this.defaultWalkVelocityMeterPerSec;
			}*/
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.LEFT) == ButtonState.TOGGLE_DOWN)
			{
				this.turnVelocityDegPerSec = -this.defaultTurnVelocityDegPerSec;
				//this.rollVelocityDegPerSec = this.defaultTurnVelocityDegPerSec;
			}
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.RIGHT) == ButtonState.TOGGLE_DOWN)
			{
				this.turnVelocityDegPerSec = this.defaultTurnVelocityDegPerSec;
				//this.rollVelocityDegPerSec = -this.defaultTurnVelocityDegPerSec;
			}

			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.DOWN) == ButtonState.TOGGLE_DOWN)
			{
				this.upVelocityDegPerSec = -this.defaultTurnVelocityDegPerSec;
			}
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.UP) == ButtonState.TOGGLE_DOWN)
			{
				this.upVelocityDegPerSec = this.defaultTurnVelocityDegPerSec;
			}

			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.MINUS) == ButtonState.TOGGLE_DOWN)
			{
				this.rollVelocityDegPerSec = -this.defaultTurnVelocityDegPerSec;
			}
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.PLUS) == ButtonState.TOGGLE_DOWN)
			{
				this.rollVelocityDegPerSec = this.defaultTurnVelocityDegPerSec;
			}
			
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.A) == ButtonState.TOGGLE_UP)
			{
				this.walkVelocityMeterPerSec = 0f;
			}
			/*if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.STICK_DIGITAL_DOWN) == ButtonState.TOGGLE_UP)
			{
				this.walkVelocityMeterPerSec = 0f;
			}*/
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.STICK_DIGITAL_RIGHT) == ButtonState.TOGGLE_UP)
			{
				this.turnVelocityDegPerSec = 0f;
			}
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.STICK_DIGITAL_LEFT) == ButtonState.TOGGLE_UP)
			{
				this.turnVelocityDegPerSec = 0f;
			}

			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.DOWN) == ButtonState.TOGGLE_UP)
			{
				this.upVelocityDegPerSec = 0f;
			}
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.UP) == ButtonState.TOGGLE_UP)
			{
				this.upVelocityDegPerSec = 0f;
			}

			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.MINUS) == ButtonState.TOGGLE_UP)
			{
				this.rollVelocityDegPerSec = 0f;
			}
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.PLUS) == ButtonState.TOGGLE_UP)
			{
				this.rollVelocityDegPerSec = 0f;
			}
			
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.LEFT) == ButtonState.TOGGLE_UP)
			{
				this.turnVelocityDegPerSec = 0f;
				//this.rollVelocityDegPerSec = 0f;
			}
			if  (wiiController.WiiMote.getButtonState(WiiMote.ButtonId.RIGHT) == ButtonState.TOGGLE_UP)
			{
				this.turnVelocityDegPerSec = 0f;
				//this.rollVelocityDegPerSec = 0f;
			}
		}
		
		
    }


    public void setTargetVelocity ( Vec3f vel )
    {
        this.walkVelocityMeterPerSec = vel.__data[2];
    }


    public void setTargetTurnVelocity ( Vec3f vel )
    {
        // RedirectToFrontNavigationMediator in Radiant, WiiTurnNavigationMediator auch
        this.turnVelocityDegPerSec = vel[1]*Mathf.Rad2Deg;
    }

    public void setTargetUpVelocity(Vec3f vel)
    {
        // RedirectToFrontNavigationMediator in Radiant, WiiTurnNavigationMediator auch
        this.upVelocityDegPerSec = vel[1] * Mathf.Rad2Deg;
    }

    public void setTargetRollVelocity(Vec3f vel)
    {
        // RedirectToFrontNavigationMediator in Radiant, WiiTurnNavigationMediator auch
        this.rollVelocityDegPerSec = vel[1] * Mathf.Rad2Deg;
    }


    public Vec3f getVelocity ()
    {
        return new Vec3f ( 0f, 0f, this.walkVelocityMeterPerSec );
    }


    public Vec3f getTurnVelocity ()
    {
        return new Vec3f ( 0f, this.turnVelocityDegPerSec*Mathf.Deg2Rad, 0f );
    }

    public Vec3f getUpVelocity()
    {
        return new Vec3f(0f, this.upVelocityDegPerSec * Mathf.Deg2Rad, 0f);
    }

    public Vec3f getRollVelocity()
    {
        return new Vec3f(0f, this.rollVelocityDegPerSec * Mathf.Deg2Rad, 0f);
    }
}