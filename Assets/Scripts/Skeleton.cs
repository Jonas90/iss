//6.E
using UnityEngine;
using System.Collections;

using Cave;

public class Skeleton : MonoBehaviour, IButtonObserver<WiiMote.ButtonId>
{
    private NetworkObserver networkObserver;
    public GameObject SkeletonJointOriginal;
    private KinectController __kinectController;
    private GameObject[] __joints = new GameObject[20];
    private bool __enabled = false;
    private Cave.ButtonObserverConnector<Cave.WiiMote.ButtonId> __buttonConnector;


    void Start ()
    {
        this.networkObserver = (NetworkObserver) GameObject.FindObjectOfType ( typeof(NetworkObserver) );
        if ( Config.Instance.UseWii )
        {
            __buttonConnector = new ButtonObserverConnector<WiiMote.ButtonId> ( this );
            ( (WiiController) GameObject.FindObjectOfType ( typeof(WiiController) ) ).WiiMote.addButtonObserver ( __buttonConnector );
        }
        if ( Config.Instance.UseKinect )
        {
            __kinectController = (KinectController) GameObject.FindObjectOfType ( typeof(KinectController) );
        }

        for ( int i = 0; i < 20; i++ )
        {
            __joints[i] = Instantiate ( SkeletonJointOriginal, Vector3.zero, Quaternion.identity ) as GameObject;
            __joints[i].transform.parent = this.transform;
            __joints[i].name = "SkeletonJoint" + i;
            __joints[i].renderer.enabled = __enabled;
        }
    }


    void Update ()
    {
        if ( !GameController.Instance.IsServer || !__enabled )
        {
            return;
        }

        for ( int i = 0; i < 20; i++ )
        {
            __joints[i].transform.localPosition = __kinectController.getSensor ( (Kinect.TrackerId) i );
        }
    }


    public bool Enabled {
        get
        {
            return __enabled;
        }
        set
        {
            __enabled = value;
            for ( int i = 0; i < 20; i++ )
            {
                __joints[i].renderer.enabled = __enabled;
            }
            if ( GameController.Instance.IsServer )
            {
                networkObserver.SendSkeletonEnabled ( __enabled );
            }
        }
    }


    public void OnButtonDown ( WiiMote.ButtonId id )
    {
        if ( id == WiiMote.ButtonId.MINUS )
        {
            this.Enabled = !this.Enabled;
        }
    }


    public void OnButtonUp ( WiiMote.ButtonId id )
    {
    }
}
