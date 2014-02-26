
// Hannes Helmholz
//
// 

using UnityEngine;
using System.Collections;


/** Zustaendig fuer das Rendern der verschiedenen Materialien.
 */
public class RendererData : System.Object
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    public Renderer Rend;
    public Material MaterialOn;
    public Material[] MaterialsOriginal;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public RendererData ( Renderer rend, Material materialOn )
    {
        this.Rend = rend;
        this.MaterialsOriginal = rend.materials;
        this.MaterialOn = materialOn;
    }
 
 
    public void SetMaterials ( bool status )
    {
        Material[] mats = new Material[MaterialsOriginal.Length];
        for ( int i = 0; i < MaterialsOriginal.Length; i++ )
        {
            mats[i] = ( status ) ? MaterialOn : MaterialsOriginal[i];
        }
     
        Rend.materials = mats;
    }
 
 
    public void SetEnabled ( bool status )
    {
        Rend.enabled = status;
    }
    // =============================================================================
}