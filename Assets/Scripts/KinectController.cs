using UnityEngine;
using System.Collections;

using Cave;

//kalibriert steuerung per kinect
public class KinectController : MonoBehaviour
{
    private Vector3 curUserRotation;
    private Transform camHolder;
    private CapsuleCollider body;
    private GameObject leftArm;
    private GameObject rightArm;
    private Vector3 leftArmDirection;
    private Vector3 rightArmDirection;
    private Vector3 headToHandDirection;
    private Vector3 handPosition;
    private BodyArmature armaturePlayer1;


    public Kinect Kinect { get; private set; }


    void Start ()
    {
        camHolder = GameObject.Find ( "CamHolder" ).transform;
        body = GameObject.Find ( "Player" ).GetComponent<CapsuleCollider> ();
        leftArm = GameObject.Find ( "LeftArm" );
        rightArm = GameObject.Find ( "RightArm" );

        armaturePlayer1 = new BodyArmature ( "PlayerMainAvatar" );

        if ( Config.Instance.UseKinect )
        {
            Logger.Log ( "Verbinde zu Kinect: " + Config.Instance.KinectAddress );
            Kinect = new Kinect ( Config.Instance.KinectAddress );
        }
    }


    void Update ()
    {
        try
           {
            if ( !Config.Instance.UseKinect )
            {
                return;
            }

            Kinect.update ();

            setArmatureByKinect ( armaturePlayer1 );

            Vector3 leftHand = getSensor ( Kinect.TrackerId.HAND_LEFT );
            Vector3 rightHand = getSensor ( Kinect.TrackerId.HAND_RIGHT );
            Vector3 leftShoulder = getSensor ( Kinect.TrackerId.SHOULDER_LEFT );
            Vector3 rightShoulder = getSensor ( Kinect.TrackerId.SHOULDER_RIGHT );
            Vector3 head = getSensor ( Kinect.TrackerId.HEAD );


            Vector3 hip = getSensor ( Kinect.TrackerId.HIP_CENTER );

            setBodyByKinect ( head, leftShoulder, rightShoulder );
            leftArmDirection = setArmByKinect ( leftArm, leftShoulder, leftHand, -1f );
            rightArmDirection = setArmByKinect ( rightArm, rightShoulder, rightHand, 1f );

            calculateHandDirection ( head, rightHand, leftHand, hip, rightShoulder, leftShoulder );
            camHolder.localPosition = head;
        }
        catch ( System.Exception e )
        {
            Logger.LogException ( e );
        }
    }


    void OnDestroy ()
    {
        Kinect = null;
    }

    /// <summary>
    /// Gets the sensor.
    /// </summary>
    /// <returns>
    /// The sensor.
    /// </returns>
    /// <param name='sensor'>
    /// Sensor.
    /// </param>
    public Vector3 getSensor ( Kinect.TrackerId sensor )
    {
        if ( !Config.Instance.UseKinect )
        {
            return new Vector3 ();
        }

        Cave.Matrix44f m = Kinect.getTracker ( sensor );

        //if (Config.Instance.IsStandalone)
        //  vec.y += Config.Instance.bodyHeightLocalKinect;

        // Der Server verwendet ein rechtshändiges Koordinatensystem (Z zeigt nach hinten),
        // Unity ein linkshändiges (Z zeigt nach vorn), deswegen Z umdrehen.
        Vector3 vec = new Vector3 ( m [ 0, 3 ], m [ 1, 3 ], m [ 2, 3 ] );
        vec.z *= -1;
        return vec;
    }
    /// <summary>
    /// Gets the gesture.
    /// </summary>
    /// <returns>
    /// The gesture.
    /// </returns>
    /*public Gesture getGesture()
 {
     if (!Config.UseKinect)
         return Gesture.None;

     float qualityLeftHand  = VrpnKinect.getValue(VrpnKinect.Analog.LeftHandGestureTrackingState);
     float qualityRightHand = VrpnKinect.getValue(VrpnKinect.Analog.RightHandGestureTrackingState);
     float stateLeftHand = VrpnKinect.getValue(VrpnKinect.Analog.LeftHandGestureState);
     float stateRightHand   = VrpnKinect.getValue(VrpnKinect.Analog.RightHandGestureState);

     if (qualityLeftHand < 0.5 || qualityRightHand < 0.5)
         return Gesture.None;

     if (stateLeftHand > 0.5 && stateRightHand < 0.5)
         return Gesture.RotateLeft;
     if (stateLeftHand < 0.5 && stateRightHand > 0.5)
         return Gesture.RotateRight;
     if (stateLeftHand > 0.5 && stateRightHand > 0.5)
         return Gesture.Forward;
     else
         return Gesture.None; // Gesture.Backward;
 }*/

    private void setArmatureByKinect ( BodyArmature armature )
    {
        /*
     Vector3 leftHand = getSensor(VrpnKinect.Sensor.LeftHand);
     Vector3 leftElbow = getSensor(VrpnKinect.Sensor.LeftElbow);
     Vector3 leftShoulder = getSensor(VrpnKinect.Sensor.LeftShoulder);

     leftHand.z *= -1;
     leftElbow.z *= -1;
     leftShoulder.z *= -1;

     //armature.setArmLeftDirection(leftElbow - leftShoulder);
     armature.setArmLowerLeftDirection(leftHand - leftElbow);

     Vector3 head = getSensor(VrpnKinect.Sensor.Head);
     Vector3 waist = getSensor(VrpnKinect.Sensor.Waist);

     //armature.setSpineDirection(head - waist);

     // get angle from Kinect: head - shoulderCenter
     // set neck angle

     // get angle from Kinect: waiste - shoulderCenter
     // set spine angle

     // get angle from Kinect: shoulder - elbow
     // set armupper angle

     // get angle from Kinect: elbow - wrist
     // set armlower angle

     // ---- ?????

     // get angle from Kinect: hip - knee
     // set legupper angle

     // get angle from Kinect: knee - ankle
     // set leglower angle
     */
    }
    /// <summary>
    /// Sets the body by kinect.
    /// </summary>
    /// <param name='head'>
    /// Head.
    /// </param>
    /// <param name='leftShoulder'>
    /// Left shoulder.
    /// </param>
    /// <param name='rightShoulder'>
    /// Right shoulder.
    /// </param>
    private void setBodyByKinect ( Vector3 head, Vector3 leftShoulder, Vector3 rightShoulder )
    {
        body.radius = Vector3.Distance ( leftShoulder, rightShoulder )/2.0f;
        body.height = head.y;

        // Alt: Blickrichtung selber berechnen
        /*Quaternion rot = new Quaternion();
     rot.SetLookRotation(leftShoulder - rightShoulder);
     Quaternion rotation = rot * Quaternion.Euler(0, 90, 0);
     curUserRotation = rotation * Vector3.forward;*/


        // Neu: Blickrichtung vom Server. Sieht erstmal komplizierter aus, aber wenn der das schon berechnet,
        // nehmen wir das für die Wartbarkeit.
        Matrix44f m = Kinect.getTracker ( Kinect.TrackerId.HEAD );
        Matrix4x4 mat = Matrix4x4.identity;
        for ( int i = 0; i < 3; i++ )
        {
            for ( int j = 0; j < 3; j++ )
            {
                // Ja, transponiert. Wo kommt das her? Unbekannt. Vielleicht, weil der Server schon transponiert.
                // Da steht genauso ein "transponieren. Warum?". Möglicherweise wegen VR-Juggler.
                mat [ i, j ] = m [ j, i ];
            }
        }

        curUserRotation = mat.MultiplyPoint3x4 ( Vector3.forward );
        curUserRotation [ 1 ] = 0f;
    }
    /// <summary>
    /// Sets the arm by kinect.
    /// </summary>
    /// <returns>
    /// The arm by kinect.
    /// </returns>
    /// <param name='arm'>
    /// Arm.
    /// </param>
    /// <param name='shoulder'>
    /// Shoulder.
    /// </param>
    /// <param name='hand'>
    /// Hand.
    /// </param>
    /// <param name='xOffset'>
    /// X offset.
    /// </param>
    private Vector3 setArmByKinect ( GameObject arm, Vector3 shoulder, Vector3 hand, float xOffset )
    {
        hand.x += 100;
        hand.y += 100;
        hand.z += 100;
        shoulder.x += 100;
        shoulder.y += 100;
        shoulder.z += 100;

        Rigidbody body = arm.GetComponent<Rigidbody> ();
        Quaternion rotation = new Quaternion ();
        Vector3 lookDir = ( hand - shoulder );
        if ( lookDir == Vector3.zero )
        {
            return new Vector3 ( 0.0f, 0.0f, 1.0f );
        }

        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero;

        //CapsuleCollider armCollider = arm.GetComponentInChildren<CapsuleCollider>();
        //armCollider.height = lookDir.magnitude;
        //armCollider.center = new Vector3(0, 0, lookDir.magnitude / 2.0f);

        lookDir.Normalize ();

        lookDir = transform.TransformDirection ( lookDir );

        rotation.SetLookRotation ( lookDir );
        rotation = rotation*Quaternion.Euler ( -90, 0, 0 );

        body.MoveRotation ( rotation );

        shoulder.x -= 100;
        shoulder.y -= 100;
        shoulder.z -= 100;
        body.MovePosition ( transform.TransformPoint ( shoulder ) );

        return lookDir;
    }
    /// <summary>
    /// Calculates the hand direction.
    /// </summary>
    /// <param name='head'>
    /// Head.
    /// </param>
    /// <param name='rightHand'>
    /// Right hand.
    /// </param>
    /// <param name='leftHand'>
    /// Left hand.
    /// </param>
    /// <param name='hip'>
    /// Hip.
    /// </param>
    /// <param name='rightShoulder'>
    /// Right shoulder.
    /// </param>
    /// <param name='leftShoulder'>
    /// Left shoulder.
    /// </param>
    private void calculateHandDirection ( Vector3 head, Vector3 rightHand, Vector3 leftHand, Vector3 hip, Vector3 rightShoulder, Vector3 leftShoulder )
    {
        float leftDist = Vector3.Distance ( leftHand, hip );
        float rightDist = Vector3.Distance ( rightHand, hip );

        if ( leftDist > rightDist )
        {
            headToHandDirection = leftHand - head;
            headToHandDirection.y = leftHand.y - leftShoulder.y;
            handPosition = leftHand;
        }
        else
        {
            headToHandDirection = rightHand - head;
            headToHandDirection.y = rightHand.y - rightShoulder.y;
            handPosition = rightHand;
        }
    }


    public Vector3 LeftArmDirection {
        get { return leftArmDirection; }
    }


    public Vector3 RightArmDirection {
        get { return rightArmDirection; }
    }


    public Vector3 HeadToHandDirection {
        get { return headToHandDirection; }
    }


    public Vector3 HandPosition {
        get { return handPosition; }
    }


    public Vector3 CurUserRotation {
        get { return curUserRotation; }
    }
}
