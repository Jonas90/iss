
// Hannes Helmholz
//
// class for animated objects with one open and one close animation

using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC

public class TriggerHatch : MonoBehaviour
{

    // =============================================================================
    #region MEMBERS
    private Config Config;
    private NetworkView NetView;
	
	public TriggerButtonSequence Script;
	
	public enum TriggerType
    {
        Trigger,
        InstantOpen,
        PermanentOpen,
        PermanentClosed
    }
	public TriggerType Type;

    public enum TriggerObject
    {
        DistanceByCollider,
        Script
    }
	public TriggerObject Trigger;
 
	private bool IsInit;
   
    private Animation Ani;
	public int AnimationsOpenSize;
    public int AnimationsCloseSize;
    public AnimationClip[] AnimationsOpen;
    public AnimationClip[] AnimationsClose;
	
	
	private bool IsClose = true;
	

    
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
 
	/**
	 * Diese Methode wird vor der Startmethode aufgerufen und setzt wichtige Parameter.
	 */
	void Awake()
	{		
		Type = TriggerType.PermanentOpen;
		Trigger = TriggerObject.Script;
		Config = GameObject.FindWithTag( "Config" ).GetComponent<Config>();
        Ani = animation;
        NetView = networkView;
	}
	/**
	 * Diese Methode wird einmal nach Awake aufgerufen.
	 */
	void Start()
	{
		if( !Config.IsServer && !Config.IsStandalone)
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
		//OnTriggerEnter(null);
		
	}
 
	/**
	 * Diese Methode wird jeden Update aufgerufen
	 */
    void Update()
    { 
        if( !Config.IsServer )
			return;
			
        if( !IsInit && Config.gameStarted )
        {
            MakePermanent();
        }
    }
 
 
	/**
	 * Diese Methode wird ausgelöst wenn ein Objekt den Trigger betritt.
	 */
    void OnTriggerEnter( Collider other )
    {
		if (IsClose)
		{
			// Handelt es sich dabei um den Spieler wird der Trigger ausgelöst.
			if (Config.IsStandalone && other.tag.Equals("Player")) {
				RPCAnimate(true);
			}
			else if (Config.IsServer){
				Open ();
			}
			IsClose = false;
		}
    }
 
 
	/**
	 * Diese Methode wird ausgelöst wenn ein Objekt den Trigger verlässt.
	 */
    void OnTriggerExit( Collider other )
    {
		if (!IsClose)
		{
			// Ist es ein Spieler soll die Animation gespielt werden.
			if (Config.IsStandalone && other.tag.Equals("Player")) {
				RPCAnimate(false);
			}
			else if (Config.IsServer){
				Close ();
			}
			IsClose = true;
		}
    }

    #endregion
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS  --------------------------------------------------------------------
 
    private void MakePermanent()
    {
        IsInit = true;
     
        if( Type == TriggerType.PermanentOpen ) Open();
        else if( Type == TriggerType.InstantOpen )
        {
            NetView.RPC( "RPCAnimateInstant", RPCMode.All, true );
        }
    }
 
 	/**
 	 * Diese Methode wird aufgerufen, wenn es sich um eine Cave handelt und 
 	 * der aktuelle Rechner der Server ist. Hier wird der RPC [Remote Procedure Call]
 	 * aufgerufen. Die dann die Methode RPCAnimate auf allen Rechnern mit den Parameter
 	 * true aufruft.
 	 */
    private void Open()
    {
        networkView.RPC( "RPCAnimate", RPCMode.All, true );
    }
 
 	/**
 	 * Diese Methode wird aufgerufen, wenn es sich um eine Cave handelt und 
 	 * der aktuelle Rechner der Server ist. Hier wird der RPC [Remote Procedure Call]
 	 * aufgerufen. Die dann die Methode RPCAnimate auf allen Rechnern mit den Parameter
 	 * false aufruft.
 	 */
    private void Close()
    {
        NetView.RPC( "RPCAnimate", RPCMode.All, false );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
	/**
	 * Diese Methode wird auf allen Rechnern ausgeführt und wird gestartet als RPC vom
	 * Server aus.
	 * 
	 * @param open True, wenn die Tür geöffnet werden soll. Andernfalls wird sie geschlossen. 
	 */
    [RPC]
    void RPCAnimate( bool open )
    {
        if( open )
        {
            foreach( AnimationClip clip in AnimationsOpen ) {
                Ani.PlayQueued( clip.name );
			}
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
            Ani.Play( AnimationsOpen[0].name );
        }
        else
        {
            Ani.Play( AnimationsClose[0].name );
        }
    }
    // =============================================================================
}