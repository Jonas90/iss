
// Hannes Helmholz
//
// 

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(NetworkView))] // for RPC

public class MeterAbstract : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]    protected AudioClip SoundSuccess;
    [SerializeField]    protected AudioClip SoundError;
    [SerializeField]    protected TextMesh DisplayLeft;
    [SerializeField]    protected TextMesh DisplayRight;
    [SerializeField]    protected Color DisplayColor;
	
    protected ConfigClass Config;
    protected Transform Trans;
    protected AudioSource Audio;
    protected Equipment Equip;
    protected NetworkView NetView;
    protected PlayerMovement PlayerMov;
    protected PlayerRotation PlayerRot;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    protected virtual void Awake ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<ConfigClass> ();
     
        Trans = transform;
        Audio = audio;
        NetView = networkView;
     
        GameObject player = GameObject.FindWithTag ( "Player" );
        PlayerMov = player.GetComponent<PlayerMovement> ();
        PlayerRot = player.GetComponent<PlayerRotation> ();
        Equip = gameObject.GetComponent<Equipment> ();

        if ( DisplayLeft )
        {
            DisplayLeft.renderer.material.color = DisplayColor;
        }
        if ( DisplayRight )
        {
            DisplayRight.renderer.material.color = DisplayColor;
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------

    public bool IsAvailable ()
    {
        return Equip.IsAvailable ();
    }
 
 
    public bool IsEquipped ()
    {
        return Equip.IsEquipped ();
    }
 
 
    protected IEnumerator MeasureValues ( float time )
    {
        if ( !IsAvailable () )
        {
            return false;
        }
     
        Audio.Play ();

        //dan
        //TODO Test
        //yield WaitForSeconds(time);
        yield return new WaitForSeconds(time);
        Audio.Stop ();
    }

 
    protected void DisplayValues ( MeasurementValues values )
    {
        if ( values.isNull() )
        {
            DisplayValues ( "ERR" + System.Environment.NewLine + "", "" );
            return;
        }
         
        string textLeft = GetDotValue ( values.O2 ) + System.Environment.NewLine + GetDotValue ( values.HCN );
        string textRight = GetDotValue ( values.CO ) + System.Environment.NewLine + GetDotValue ( values.HCL );
        DisplayValues ( textLeft, textRight );
    }
 
 
    protected void DisplayValues ( string textLeft, string textRight )
    {
        NetView.RPC ( "RPCDisplayValues", RPCMode.AllBuffered, textLeft, textRight );
    }
 
 
    private string GetDotValue ( float val )
    {
        string text = val.ToString ();
     
        if ( val == 0 )
        {
            text = ".0";
        } // 0 -> .0
        else if ( !text.Contains ( "." ) )
            {
                text += ".0";
            } // 21 -> 21.0
            else if ( val < 1 )
                {
                    text = text.Substring ( 1 );
                } // 0.1 -> .1

        return text;
    }

 
    protected void SetPlayerMotion ( bool status )
    {
        PlayerMov.enabled = status;
        PlayerRot.TemporaryDisable = !status;
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCDisplayValues ( string textLeft, string textRight )
    {
        DisplayLeft.text = textLeft;
        DisplayRight.text = textRight;
    }
    // =============================================================================
}