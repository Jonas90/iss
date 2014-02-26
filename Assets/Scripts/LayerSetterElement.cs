
// Hannes Helmholz
//
// 

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

//startet ebenen-element

public class LayerSetterElement : UniqueID
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    public int OldLayer;
    public int OtherLayer;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start ()
    {
        OldLayer = gameObject.layer;
    }
    // =============================================================================
}