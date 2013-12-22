
// Hannes Helmholz
//
// 

using UnityEngine;
using System.Collections;

public class MeterFireAir : MeterAbstract
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private InteractionKey Button; // optional

    private MeasurementFireAir MeasurementScript;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake ()
    {
        base.Awake ();
     
        Button.Initialize ();
        MeasurementScript = FindObjectOfType ( typeof(MeasurementFireAir) ) as MeasurementFireAir;
    }
 
 
    void Update ()
    {
        if ( !Config.IsServer )
        {
            return;
        }
         
        if ( Button.GetButtonDown () && IsEquipped () )
        {
            StartCoroutine ( YieldMeasure () );
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    private IEnumerator YieldMeasure ()
    {
        //dan
        //TODO
        //yield StartCoroutine(MeasureValue());
        yield return StartCoroutine(MeasureValue());
    }
 
 
    protected IEnumerator MeasureValue ()
    {
        if ( !IsAvailable () )
        {
            return false;
        }

        //dan
        //TODO
        //yield WaitForSeconds(1);
        yield return new WaitForSeconds(1);
     
        string room = MeasurementScript.GetCurrentRoomName ( Trans.position );
        MeasurementValues values = MeasurementScript.GetValues ( room );
        string text = !values.isNull() ? values.ToString () : "ERR";
        ConfigClass.Log ( "AIR room: " + room + " values " + text );
        DisplayValues ( values );
     
        Audio.clip = room.Length == 0 ? SoundError : SoundSuccess;

        //dan
        //TODO
        //yield base.MeasureValues(2.5f); // play sound
        yield return base.MeasureValues(2.5f); // play sound
     
        DisplayValues ( "", "" );
    }
    // =============================================================================
}