
// Hannes Helmholz
//
// nothing to do here anymore
using Holoville.HOTween;
using UnityEngine;

public class AnimationRotateByTime : AnimationRotateAbstract
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    protected float RotationTime = 10f;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    protected override void DoFixedUpdate ()
    {
        float x = ( RotationAxis == Axis.X ) ? 1 : 0;
        float y = ( RotationAxis == Axis.Y ) ? 1 : 0;
        float z = ( RotationAxis == Axis.Z ) ? 1 : 0;
        float step = Time.fixedDeltaTime*360/-RotationTime;
     
        Trans.Rotate ( x*step, y*step, z*step );
    }
    // =============================================================================
}