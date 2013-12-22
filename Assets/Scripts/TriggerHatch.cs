
// Hannes Helmholz
//
// class for animated objects with one open and one close animation
using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC

public class TriggerHatch : MonoBehaviour
{

    // =============================================================================
    #region MEMBERS
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
 
    public TriggerType Type;
    public TriggerObject Trigger;
    public int AnimationsOpenSize;
    public int AnimationsCloseSize;
    public AnimationClip[] AnimationsOpen;
    public AnimationClip[] AnimationsClose;
    public TriggerButtonSequence Script;
    private ConfigClass Config;
    private Animation Ani;
    private NetworkView NetView;
    private bool IsInit;
    #endregion
    // =============================================================================
 
    public TriggerHatch()
    {
        Type = new TriggerType();
        Trigger = new TriggerObject();
        AnimationsOpenSize = 1;
        AnimationsCloseSize = 1;
        AnimationsOpen = new AnimationClip[AnimationsOpenSize];
        AnimationsClose = new AnimationClip[AnimationsCloseSize];
        Script = new TriggerButtonSequence();
        Config = new ConfigClass();
        Ani = new Animation();
        NetView = new NetworkView();
        IsInit = false;
    }


 
    // =============================================================================
    #region METHODS UNITY ---------------------------------------------------------------
 
    void Start()
    {
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
            Script = transform.GetComponent<TriggerButtonSequence>();
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