
// Hannes Helmholz
//
// give indicator on level overview (i.e. for player position)
//
// good looking with sphere mesh filter and diffuse material with solid color

using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for Transform
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]

public class OverviewIndicator : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private Transform IndicatedObject;
    private Config Config;
    private Transform Trans;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
     
        Trans = transform;
    }
 
 
    void Update ()
    {
        if ( !Config.IsServer )
        {
            return;
        }

        // Trans.position.y = not changed !!!
        Vector3 temp = new Vector3(IndicatedObject.position.x, Trans.position.y, IndicatedObject.position.z);

        Trans.position = temp;
     
        Trans.rotation = IndicatedObject.rotation;
    }
    // =============================================================================
}