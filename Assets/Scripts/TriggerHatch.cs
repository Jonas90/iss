
// Hannes Helmholz
//
// class for animated objects with one open and one close animation
using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC

public class TriggerHatch : MonoBehaviour
{

    // =============================================================================
    #region MEMBERS
    private ConfigClass Config;
    private NetworkView NetView;
	
	public TriggerButtonSequence Script;
	
	public enum TriggerType
    {
        Trigger,
        InstantOpen,
        PermanentOpen,
        PermanentClosed
    }

    public enum TriggerObject
    {
        DistanceByCollider,
        Script
    }
 
	private bool IsInit;
    public TriggerType Type;
    public TriggerObject Trigger;
	
    private Animation Ani;
	public int AnimationsOpenSize;
    public int AnimationsCloseSize;
    public AnimationClip[] AnimationsOpen;
    public AnimationClip[] AnimationsClose;
	

    
    #endregion
    // =============================================================================
 
//dan Konstructoren sind in unity ein noGo bei klassen die von MonoBehaviour erben !!!!
//http://answers.unity3d.com/questions/32413/using-constructors-in-unity-c.html	
//deshalb: constructor --> awake / start
//http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.Start.html
//http://docs.unity3d.com/Documentation/ScriptReference/MonoBehaviour.Awake.html
//awake wird immer asugeführt, start nur wen das gameobject aktiviert ist!
 
    // =============================================================================
    #region METHODS UNITY ---------------------------------------------------------------
 
	void Awake()
	{
		Type = TriggerType.PermanentClosed;
		Trigger = TriggerObject.Script;
		
		AnimationsOpenSize = 1;
        AnimationsCloseSize = 1;
		AnimationsOpen = new AnimationClip[AnimationsOpenSize];
        AnimationsClose = new AnimationClip[AnimationsCloseSize];
		
		Config = GameObject.FindWithTag( "Config" ).GetComponent<ConfigClass>();
        Ani = animation;
        NetView = networkView;
     
		
		
        if( !Config.IsServer )
        {
            IsInit = true;
            return;
        }
     
        if( Type == TriggerType.Trigger )
        {
            IsInit = true;
        }

        if( Trigger == TriggerObject.Script )
        {
			Script = GetComponent<TriggerButtonSequence>();
        }
		
	} 
 
    void Update()
    { 
        if( !Config.IsServer )
        {
            return;
        }
         
        if( !IsInit && Config.IsGameStarted )
        {
            MakePermanent();
        }
    }
 
 
    void OnTriggerEnter( Collider other )
    {
        if( Trigger != TriggerObject.DistanceByCollider || !Config.IsServer || !other.gameObject.tag.Equals( "Player" ) )
        {
            return;
        } 
             
        Open();
    }
 
 
    void OnTriggerExit( Collider other )
    {
        if( Trigger != TriggerObject.DistanceByCollider || !Config.IsServer || !other.gameObject.tag.Equals( "Player" ) )
        {
            return;
        }
         
        Close();
    }

    #endregion
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS  --------------------------------------------------------------------
 
    private void MakePermanent()
    {
        IsInit = true;
     
        if( Type == TriggerType.PermanentOpen )
        {
            Open();
        }
        else if( Type == TriggerType.InstantOpen )
        {
            NetView.RPC( "RPCAnimateInstant", RPCMode.AllBuffered, true );
        }
    }
 
 
    private void Open()
    {
        NetView.RPC( "RPCAnimate", RPCMode.AllBuffered, true );
    }
 
 
    private void Close()
    {
        NetView.RPC( "RPCAnimate", RPCMode.AllBuffered, false );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCAnimate( bool open )
    {
        if( open )
        {
            foreach( AnimationClip clip in AnimationsOpen )
                Ani.PlayQueued( clip.name );
        }
        else
        {
            foreach( AnimationClip clip in AnimationsClose )
                Ani.PlayQueued( clip.name );
        }
    }
 
 
    [RPC]
    void RPCAnimateInstant( bool open )
    {
        if( open )
        {
            Ani.Play( AnimationsOpen[AnimationsOpen.Length - 1].name );
        }
        else
        {
            Ani.Play( AnimationsClose[AnimationsClose.Length - 1].name );
        }
    }
    // =============================================================================
}