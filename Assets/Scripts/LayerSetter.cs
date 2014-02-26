
// Hannes Helmholz
//
// 

using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC
[RequireComponent (typeof(UniqueIDManager))]


//organisiert game-objekten und zugehoerige ebene
public class LayerSetter : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    private static NetworkView NetView;
    private UniqueIDManager IDManager;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake ()
    {
        NetView = networkView;
     
        IDManager = gameObject.GetComponent<UniqueIDManager> ();
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS STATIC --------------------------------------------------------------
 
    public static void SetAllLayerRecursively ( GameObject go, int newLayer )
    {
        SetLayer ( go, newLayer );
     
        NetView.RPC ( "RPCSetAllLayerRecursively", RPCMode.OthersBuffered, go.GetComponent<UniqueID> ().ID, newLayer );
    }
 
 
    private static void SetLayer ( GameObject go, int newLayer )
    {
        if ( !go )
        {
            return;
        }
 
        go.layer = newLayer;
     
        foreach ( Transform child in go.transform )
        {
            if ( !child )
            {
                continue;
            }
             
            SetLayer ( child.gameObject, newLayer );
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCSetAllLayerRecursively ( int id, int newLayer )
    {
        SetLayer ( IDManager.GetGameObjectByID ( id ), newLayer );
    }
    // =============================================================================
}