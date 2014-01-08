// Hannes Helmholz
//
// (abstract) base class for animated rotations

using Holoville.HOTween;
using UnityEngine;
using System;
using System.Collections;


//abst
public class AnimationRotateAbstract : MonoBehaviour
{
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]    protected bool OnlyByServer;
    [SerializeField]    protected Axis RotationAxis;
    protected Config Config;
    protected Transform Trans;
    // =============================================================================

    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------

    public AnimationRotateAbstract()
    {
        OnlyByServer = true;
        RotationAxis = Axis.Y;
        Config = new Config();
    }

    protected virtual void Start ()
    {
        if ( OnlyByServer )
        {
            try
            {
                Config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
            }
            catch ( Exception e )
            {
                OnlyByServer = false;
                //dan
				//Config.Log ( e.ToString (), true );
            }
        }

        Trans = transform;
    }

    void FixedUpdate ()
    {
        if ( OnlyByServer && !Config.IsServer )
        {
            return;
        }

        DoFixedUpdate ();
    }

    // =============================================================================

    // =============================================================================
    // METHODS  --------------------------------------------------------------------

    virtual protected void DoFixedUpdate (){}
    // =============================================================================
}