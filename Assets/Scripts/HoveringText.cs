
// Hannes Helmholz
//
// textoverlay on target object
// started by ViewOnTarget()
// stoped by Hide()
using UnityEngine;

[RequireComponent (typeof(GUIText))]

public class HoveringText : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    private enum FadeState
    {
        None,
        FadeIn,
        FadeOut
    }
 
    [SerializeField]    private Color TextColor;
    [SerializeField]    private Camera Cam;
    [SerializeField]    private Transform Player;
    [SerializeField]    private GUITexture Background; // optional
    [SerializeField]    private Vector2 BackgroundSpacing = new Vector2 ( 10, 10 ); // optional
    [SerializeField]    private float FadeDuration = 0.5f; // optional
 
    private Transform Trans;
    private Transform TransTarget;
    private Vector3 TransTargetOffset;
    private GUIText GuiText;
    //private float GuiTextAlphaOriginal;
    //private float FontBaseSize;
    private Transform BackgroundTransform;
    private float BackgroundAlphaOriginal;
    private float TimeStart;
    private float TimeLeft;
    private FadeState Status;
 
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start ()
    {
        Trans = transform;
     
        GuiText = guiText;
        GuiText.font.material.color = TextColor;
        //GuiTextAlphaOriginal = GuiText.font.material.color.a;
        //FontBaseSize = GuiText.fontSize;
     
        if ( Background )
        {
            BackgroundTransform = Background.transform;
            BackgroundAlphaOriginal = Background.color.a;
         
            BackgroundTransform.localScale = Vector3.zero; // should be done, dont know why
            Background.pixelInset = new Rect ( 0, 0, 0, 0 );
        }
    }
 
 
    void Update ()
    {
        if ( GuiText.text.Length == 0 )
        {
            return;
        }
     
        Vector3 ray = TransTarget.position - Player.position;
        ray += Quaternion.Euler ( Player.rotation.eulerAngles )*TransTargetOffset;
        Trans.position = Cam.WorldToViewportPoint ( ray + Player.position );
     
        // fontSize is just integer -> value is jumping :(
        //GuiText.fontSize = FontBaseSize / (TransTarget.position - Player.position).magnitude;
     
        if ( Background )
        {
            Rect rect = GuiText.GetScreenRect ( Cam );
            BackgroundTransform.localScale = Vector3.zero; // should be done, dont know why
         
            Background.pixelInset = new Rect ( ( -rect.width/2 ) - BackgroundSpacing.x,
                                         ( -rect.height/2 ) - BackgroundSpacing.y,
                                         rect.width + 2*BackgroundSpacing.x,
                                         rect.height + 2*BackgroundSpacing.y );
        }
                                     
        TimeLeft = TimeStart + FadeDuration - Time.time;
        if ( Status == FadeState.FadeIn )
        {
            Fade ( true );
        }
        else if ( Status == FadeState.FadeOut )
            {
                Fade ( false );
            }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS  --------------------------------------------------------------------
 
    public void ViewOnTarget ( Transform target, Vector3 offset, string text )
    {
        Status = FadeState.FadeIn;
        TransTarget = target;
        TransTargetOffset = offset;
        GuiText.text = text;
     
        TimeStart = Time.time;
    }
 
 
    public void Hide ()
    {
        Status = FadeState.FadeOut;
     
        TimeStart = Time.time;
    }
 
 
    private void Fade ( bool fadeIn )
    {
        float a = TimeLeft/FadeDuration;
        if ( a < 0 )
        {
            a = 0;
            Status = FadeState.None;
         
            if ( !fadeIn )
            {
                GuiText.text = "";
            }
        }
     
        if ( fadeIn )
        {
            a = 1 - a;
        }

        Color temp = GuiText.font.material.color;/* * GuiTextAlphaOriginal*/;
        temp.a = a;
        GuiText.font.material.color = temp;

        if ( Background )
        {
            temp = Background.color;
            temp.a = a*BackgroundAlphaOriginal;

            Background.color = temp;
        }
    }
    // =============================================================================
}