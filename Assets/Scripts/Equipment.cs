
// Hannes Helmholz
//
//

using Holoville.HOTween;
using Holoville.HOTween.Core;

using UnityEngine;

[RequireComponent (typeof(IHOTweenComponent))]


public class Equipment : MonoBehaviour
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    protected bool AvailableAtStart;
    protected ConfigClass Config;
    protected bool TriggerManual;
    protected bool Available;
    protected System.Object[] TweenTargets;
    private Transform Trans;
    private bool Stored;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
	public Equipment()
	{
		AvailableAtStart = false;
		TriggerManual = false;
		Stored = true;
	}
 
    protected virtual void Start ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<ConfigClass> ();
        Trans = transform;
     
        Available = AvailableAtStart;
     
        // there is some realy black magic going on in here
        // can't explain that to anyone
        // just to find out the tween target of this GameObject
        TweenTargets = new System.Object[0];
        foreach ( TweenInfo tweenInfo in HOTween.GetTweenInfos() )
        {
            System.Object obj = tweenInfo.targets[0];
         
            foreach ( Component component in gameObject.GetComponents<Component>() )
            {
                if ( component.Equals ( obj ) )
                {
                    int oldSize = TweenTargets.Length;
                    System.Array.Resize<System.Object> ( ref TweenTargets, oldSize + 1 );
                    TweenTargets[oldSize] = obj; // all for this stupid line of code
                }
            }
        }
     
        foreach ( Object tweenTarget in TweenTargets )
            HOTween.Complete ( tweenTarget ); // set to stored position
    }
 
 
    void Update ()
    {
        if ( !Config.IsServer )
        {
            return;
        }
         
        if ( Available && TriggerManual )
        {
            Trigger ();
            TriggerManual = false;
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    protected void Trigger ()
    {
        if ( Stored )
        {
            foreach ( Object tweenTarget in TweenTargets )
                HOTween.PlayBackwards ( tweenTarget );
        }
        else
        {
            foreach ( Object tweenTarget in TweenTargets )
                HOTween.PlayForward ( tweenTarget );
        }   
     
        Stored = !Stored;
    }
 
 
    public void SetAvailable ( bool available )
    {
        Available = available;
    }
 
 
    public void TriggerStored ()
    {
        if ( !Available )
        {
            return;
        }
     
        TriggerManual = true;
    }
 
 
    public bool IsAvailable ()
    {
        return Available;
    }
 
 
    public bool IsEquipped ()
    {  
        return Available && !Stored;
    }
    // =============================================================================
}