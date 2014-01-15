
// Hannes Helmholz
//
// manage player movement
// at beginning player flight or jump at start position
// movement will be deactivated while flight
using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for Transform

public class PlayerMovement : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private float Speed = 5;
    private Config Config;
    private CharacterController CC;
    private Transform Trans;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake()
    {
        Config = GameObject.FindWithTag( "Config" ).GetComponent<Config>();
        Trans = transform;
     
        CC = gameObject.GetComponent<CharacterController>();
    }
 
 
    void FixedUpdate()
    {
        //TODO check: // bewegung in abhängigkeit der blickrichtung!?::
        /*
        if( !Input.GetAxis( "Horizontal" ).Equals( 0f ) )
        {
            CC.Move( Trans.right*Input.GetAxis( "Horizontal" )*Speed*Time.deltaTime );
        }

        if( !Input.GetAxis( "Vertical" ).Equals(0f) )
        {
            CC.Move( Trans.forward*Input.GetAxis( "Vertical" )*Speed*Time.deltaTime );
        }

        if( !Input.GetAxis( "UpDown" ).Equals(0f) )
        {
            CC.Move( Trans.up*Input.GetAxis( "UpDown" )*Speed*Time.deltaTime );
        }
        */
        
        CC.Move( Trans.right*Input.GetAxis( "Horizontal" )*Speed*Time.deltaTime );
        CC.Move( Trans.forward*Input.GetAxis( "Vertical" )*Speed*Time.deltaTime );
        CC.Move( Trans.up*Input.GetAxis( "UpDown" )*Speed*Time.deltaTime );

    }
    // =============================================================================
}