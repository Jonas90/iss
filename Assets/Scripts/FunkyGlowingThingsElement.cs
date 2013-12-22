
// Hannes Helmholz
//
// base class for TriggerButton
// needs FunkyGlowingThingsEffect on a camera to get shown
// 
// player have to be in range to get shown
// mouse have to be over object to get shown
using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC

public class FunkyGlowingThingsElement : MonoBehaviour
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]    private Material MaterialOn;
    [SerializeField]    private bool ChangeChildMaterials = true;
    [SerializeField]    private float PlayerMinDistance = 3;
    
    protected ConfigClass Config;
    protected Transform Trans;
    protected RendererData[] Renderers;
    protected NetworkView NetView;
    protected Transform Player;
    protected bool MouseInside = false;
 
    //true: mouse MouseInside && CalculateDistance(Trans, Player) <= PlayerMinDistance;
    protected bool Status = false;
    protected bool StatusLast = false;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    protected virtual void Awake ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<ConfigClass> ();
        Player = GameObject.FindWithTag ( "Player" ).transform;
        Trans = transform;
        NetView = networkView;
     
        GeneratRenderersData ();
    }
 
 
    protected virtual void Update ()
    {
        if ( !Config.IsServer )
        {
            return;
        }
     
        Status = MouseInside && CalculateDistance ( Trans, Player ) <= PlayerMinDistance;
        if ( Status != StatusLast && NetView )
        {
            NetView.RPC ( "RPCSetStatus", RPCMode.AllBuffered, Status );
        }
         
        StatusLast = Status;
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
        return Status;
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
        enabled = status;
    }
 
 
    [RPC]
    protected void RPCSetStatus ( bool status )
    {
        Status = status;
        StatusLast = Status;
     
        foreach ( RendererData rend in Renderers )
		{
            rend.SetMaterials ( status );
		}
    }
    // =============================================================================
}