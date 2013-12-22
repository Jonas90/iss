
// Hannes Helmholz
//
// 

using UnityEngine;
using System.Collections;

public class MeasurementFireAir : MeasurementAbstract
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private float DistanceTolerance = 0.5f;
    private Volume[] Rooms;
    private VolumeData[] CurrentData;
    // =============================================================================
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start ()
    {
        Rooms = GameObject.FindObjectsOfType ( typeof (Volume) ) as Volume[];
     
        CurrentData = new VolumeData[Rooms.Length];
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS STATIC --------------------------------------------------------------
 
    public string GetCurrentRoomName ( Vector3 position )
    {
        for ( int i = 0; i < Rooms.Length; i++ )
        {
            CurrentData[i] = Rooms[i].GetVolumeData ( position );
         
            if ( CurrentData[i].Distance == 0 )
            {
                return CurrentData[i].Name;
            }
        }
     
        // if not in room ... get the nearest one
        int indexLowest = 0;
        float valueLowest = CurrentData[0].Distance;
        for ( int i = 1; i < CurrentData.Length; i++ )
        {
            if ( CurrentData[i].Distance < valueLowest )
            {
                valueLowest = CurrentData[i].Distance;
                indexLowest = i;
            }
        }
     
        if ( CurrentData[indexLowest].Distance <= DistanceTolerance )
        {
            return CurrentData[indexLowest].Name;
        }
     
        return "";
    }
    // =============================================================================
}