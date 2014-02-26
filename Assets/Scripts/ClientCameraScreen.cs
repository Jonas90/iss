//Diese Klasse wird durch den Merge des 6. Etage Projektes nicht mehr benötigt,
//da die Camera aus dem 6. Etage Projekt verwendet wird

// Hannes Helmholz
//
// represent cave screen in real world
// defindes field of view for camera
// used by ClientCamera


using UnityEngine;

public class ClientCameraScreen
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    public float Width;
    public float Height;
    public float Distance;
    // =============================================================================
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public ClientCameraScreen( float width, float height, float distance )
    {
        this.Width = width;
        this.Height = height;
        this.Distance = distance;
    }
 
 
    public override string ToString()
    {
        return Width + " x " + Height + " @ " + Distance;
    }
    // =============================================================================
}