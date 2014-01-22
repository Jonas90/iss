
// Hannes Helmholz
//
// base class for TriggerButton
// needs FunkyGlowingThingsEffect on a camera to get shown
// 
// player have to be in range to get shown
// mouse have to be over object to get shown
using UnityEngine;
using System;


[RequireComponent (typeof(NetworkView))] // for RPC

public class FunkyGlowingThingsElement : MonoBehaviour
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]    private Material MaterialOn;
    [SerializeField]    private bool ChangeChildMaterials = true;
    [SerializeField]    private float PlayerMinDistance = 3;
    
    protected Config config;
    protected Transform transf;
    protected RendererData[] Renderers;
    protected NetworkView NetView;
    protected Transform Player;
    protected bool MouseInside;
 
    //true: mouse MouseInside && CalculateDistance(Trans, Player) <= PlayerMinDistance;
    protected bool status;
    protected bool statusLast;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    protected virtual void Start ()
    {
        config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
		
        Player = GameObject.FindWithTag ( "Player" ).transform;
        transf = transform;
        NetView = networkView;
		
		MouseInside = false;
		
		status = false;
    	statusLast = false;
     
        GeneratRenderersData ();
    }
 
 
    protected virtual void Update ()
    {
		//Debug.Log("config.IsServer: "+config.IsServer);
		
		if(config == null)
		{
			//Debug.Log("NO CONFIG");
			config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
			//if(config == null) throw new Exception("NO CONFIG GO");
		} //keine Ahnung wieso das nicht ofort in der Awake () methode gefunden wird
		
		if ( !config.IsServer ) return;
		
        this.status = MouseInside && CalculateDistance ( transf, Player ) <= PlayerMinDistance;
        if ( status != statusLast && NetView )
        {
            NetView.RPC ( "RPCSetthis.Status", RPCMode.AllBuffered, this.status );
        }
         
        statusLast = this.status;
    }
 
 
    protected virtual void OnMouseEnter ()
    {
        MouseInside = true;
    }
 
 
    protected virtual void OnMouseExit ()
    {
        MouseInside = false;
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
     
    public bool GetIsTriggered ()
    {
        return status;
    }
 
 
    public static float CalculateDistance ( Transform object1, Transform object2 )
    {
        return ( object1.position - object2.position ).magnitude;
    }
 
 
    private void GeneratRenderersData ()
    {
        Renderer[] childs;
        if ( ChangeChildMaterials )
        {
            childs = gameObject.GetComponentsInChildren<Renderer> () as Renderer[];
        }
        else
        {
            childs = gameObject.GetComponents<Renderer> () as Renderer[];
        }
		
        Renderers = new RendererData[childs.Length];
        for ( int i = 0; i < childs.Length; i++ )
        {
            Renderers[i] = new RendererData ( childs[i], MaterialOn );
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    protected void RPCSetEnabled ( bool status )
    {
        //unityengine.behavior.enabled
		base.enabled = status;
    }
 
 
    [RPC]
    protected void RPCSetStatus ( bool newStatus )
    {
        status = newStatus;
        statusLast = status;
		
//-		Debug.Log("Renderers: "+Renderers+" len: "+Renderers.Length);
		
        foreach ( RendererData rend in Renderers )
		{
            rend.SetMaterials ( status );
		}
    }
    // =============================================================================
}