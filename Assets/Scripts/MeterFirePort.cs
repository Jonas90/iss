
// Hannes Helmholz
//
// 
using UnityEngine;
using System.Collections;

using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;


public class MeterFirePort : MeterAbstract
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private float MeasureAnimationLength = 6;
    private MeasurementFirePort MeasurementScript;
    private LayerSetterElement Nose;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake()
    {
        base.Awake();
        MeasurementScript = GameObject.FindObjectOfType( typeof(MeasurementFirePort) ) as MeasurementFirePort;

        Nose = gameObject.GetComponentInChildren<LayerSetterElement>();
     
        Audio.clip = SoundSuccess;
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------

    public IEnumerator MeasureValues( string name, Transform port )
    {
        if( !IsAvailable() )
        {
            return false;
        }
     
        float measureAnimationPartLength = MeasureAnimationLength/4.0f;
     
        SetPlayerMotion( false );
     
        TweenParms parms1 = new TweenParms();
        parms1.Prop( "position", new PlugVector3( port.position ) );
        parms1.Prop( "rotation", new PlugQuaternion( port.rotation ) );
        //parms1.Prop("localScale", Plugins.Core.PlugVector3(port.localScale));
        parms1.Ease( EaseType.EaseInOutSine );
        Tweener tweener1 = HOTween.To( Trans, measureAnimationPartLength, parms1 );
        tweener1.autoKillOnComplete = false;

        //dan
        //TODO
        //yield WaitForSeconds(measureAnimationPartLength);
        yield return new WaitForSeconds(measureAnimationPartLength);
     
        LayerSetter.SetAllLayerRecursively( Nose.gameObject, Nose.OtherLayer );

        TweenParms parms2 = new TweenParms();
        parms2.Prop( "position", new PlugVector3( Trans.forward*0.18f, true ) );
        parms2.Ease( EaseType.EaseInOutSine );
        Tweener tweener2 = HOTween.To( Trans, measureAnimationPartLength, parms2 );
        tweener2.autoKillOnComplete = false;


        //dan
        //TODO
        //yield WaitForSeconds(measureAnimationPartLength);
        yield return new WaitForSeconds(measureAnimationPartLength);

        //dan
        //TODO
        //yield WaitForSeconds(1);
        yield return new WaitForSeconds(1);

        MeasurementValues values = MeasurementScript.GetValues( name );
        string text = values.isNull() ? "ERR" : values.ToString();
		//dan
		//Config.Log( "PORT port: " + name + " values " + text );
        DisplayValues( values );

        //dan
        //TODO
        //yield base.MeasureValues(0.5f); // play sound
        yield return base.MeasureValues(0.5f); // play sound
     
        HOTween.PlayBackwards( tweener2 );

        //dan
        //TODO
        //yield WaitForSeconds(measureAnimationPartLength);
        yield return new WaitForSeconds(measureAnimationPartLength);
     
        LayerSetter.SetAllLayerRecursively( Nose.gameObject, Nose.OldLayer );
     
        HOTween.PlayBackwards( tweener1 );

        //dan
        //TODO
        //yield WaitForSeconds(measureAnimationPartLength);
        yield return new WaitForSeconds(measureAnimationPartLength);
     
        HOTween.Kill( tweener1 );
        HOTween.Kill( tweener2 );
        SetPlayerMotion( true );

        //dan
        //TODO
        //yield WaitForSeconds(2.5f);
        yield return new WaitForSeconds(2.5f);
     
        DisplayValues( "", "" );
    }
    // =============================================================================
}