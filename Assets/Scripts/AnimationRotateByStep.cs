
// Hannes Helmholz
//
// nothing to do here anymore

using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class AnimationRotateByStep : AnimationRotateAbstract 
{
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	[SerializeField]	protected float AngleStep;
	[SerializeField]	protected float TimeStep;

	protected float LastStep;
	// =============================================================================

	public AnimationRotateByStep()
	{
		AngleStep = 45;
		TimeStep = 0.25f;
		LastStep = 0;
	}
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------

	protected virtual void Start ()
	{
        base.Start();
		LastStep = Time.time;
	}

	// =============================================================================

	// =============================================================================
	// METHODS ---------------------------------------------------------------------


	protected override void DoFixedUpdate()
	{
		if (LastStep + TimeStep <= Time.time)
		{
			LastStep = Time.time;
			
			float x = (RotationAxis == Axis.X) ? 1 : 0;
			float y = (RotationAxis == Axis.Y) ? 1 : 0;
			float z = (RotationAxis == Axis.Z) ? 1 : 0;
			Trans.Rotate(x * AngleStep, y * AngleStep, z * AngleStep);
		}
	}
	// =============================================================================
}