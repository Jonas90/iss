
// Hannes Helmholz
//
// extension of TriggerAbstract as a button to trigger actions
//
// shows description as HoveringText (name, key and description onyl when set to a value)

using UnityEngine;
using System.Collections;

public class TriggerButton : TriggerAbstract
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]    private bool HideIfInactive ;
    [SerializeField]    private AnimationClip[] AnimationClips;
    private Animation Ani;
    // ============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
	public TriggerButton()
	{
		HideIfInactive = false;
	}
 
    void OnEnable ()
    {
        if ( HideIfInactive )
        {
            Coll.enabled = true;
         
            foreach ( RendererData rend in Renderers )
                rend.SetEnabled ( true );
        }
    }
 
 
    void OnDisable ()
    {
        if ( HideIfInactive )
        {
            Coll.enabled = false;
         
            foreach ( RendererData rend in Renderers )
                rend.SetEnabled ( false );
        }
     
        base.OnDisable ();
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
     
    // have to be called on init
    public void SetAnimation ( Animation animation )
    {
        Ani = animation;
    }
 
 
    public void SetAsStartingPoint ()
    {
        //TODO ka
        /*
        if ( !AnimationClip || !Ani. )
        {
            return;
        }*/
     
        Ani.Play ( AnimationClips[0].name );
        Ani.Sample ();
        Ani.Stop ();
    }
 
 
    protected new void DoTrigger ()
    {
        //TODO ka
        /*
        if ( !AnimationClip || !Ani. )
        {
            return;
        }*/
         
        base.DoTrigger ();
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCDoTrigger ()
    {
        foreach ( AnimationClip clip in AnimationClips )
            Ani.PlayQueued ( clip.name );
    }
    // =============================================================================
}