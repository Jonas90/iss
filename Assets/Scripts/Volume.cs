
// Hannes Helmholz
//
// 

using UnityEngine;

[RequireComponent (typeof(Collider))]

public class Volume : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private string Name;
    private Collider Coll;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake ()
    {
        Coll = collider;
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public VolumeData GetVolumeData ( Vector3 point )
    {
        float distance = Coll.bounds.SqrDistance ( point );
        return new VolumeData ( Name, distance );
    }
    // =============================================================================
}

public class VolumeData
{
    public string Name;
    public float Distance;

 
    public VolumeData ( string name, float distance )
    {
        this.Name = name;
        this.Distance = distance;
    }
}