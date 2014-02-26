
// Hannes Helmholz
// Diese Klasse lösst den Sirenen Sound aus, der beim aktivieren eines Panels aktiviert wird. Desweiterin
// wird das blinken der Panels hier ausgelöst.
// 

using UnityEngine;

public class TriggerPanelButton : MonoBehaviour
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private Color ColorOn;
    private Material Mat;
    private AudioSource Audio;
    private bool State = false;
    private bool InitState = true;
    private Color ColorOff;
    // ============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    protected virtual void Awake ()
    {
        Mat = renderer.material;
        Audio = audio;
     
        ColorOff = Mat.color;
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public void SetState ( bool state )
    {
        if ( state != State )
        {
            Trigger ();
        }
    }
 
 
    public void Trigger ()
    {
        State = !State;
     
        if ( InitState )
        {
            //Mat.color = State ? ColorOn : ColorOff;
            Mat.color = ColorOn;
            InitState = false;
        }
     
        if ( Audio )
        {
            if ( State )
            {
                Audio.Play ();
            }
            else
            {
                Audio.Stop ();
            }
        }
    }
    // =============================================================================

}