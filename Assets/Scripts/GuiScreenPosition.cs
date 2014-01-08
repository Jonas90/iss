
// Hannes Helmholz
//
// 
using UnityEngine;

public class GuiScreenPosition : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    public enum Anchor
    {
        CenteredTop,
        CenteredBottom,
        Absolute
    }
 
    public Anchor PositionAnchor = Anchor.CenteredBottom;
    public float CenteredSpacing = 0.02f;
    public float CenteredWidth = 0.5f;
    public float AbsoluteSpacingLeft = 0.02f;
    public float AbsoluteSpacingBottom = 0.02f;
    public float AbsoluteWidth = 0.2f;
    public bool DifferentInStandalone = false;
    public Anchor PositionAnchorStandalone = Anchor.Absolute;
    public float CenteredSpacingStandalone = 0.02f;
    public float CenteredWidthStandalone = 0.5f;
    public float AbsoluteSpacingLeftStandalone = 0.02f;
    public float AbsoluteSpacingBottomStandalone = 0.02f;
    public float AbsoluteWidthStandalone = 0.25f;
    private Config Config;
    private float Aspect;
    private float CurrentX;
    private float CurrentY;
    private float CurrentWidth;
    private float CurrentHeight;
    // =============================================================================

 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------

    void Start ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
    }
 
 
    void Update ()
    {
        if ( !Config.OwnClientData.ShowGuiCamera )
        {
            return;
        }
     
        if ( Config.IsServer && DifferentInStandalone )
        {
            switch ( PositionAnchorStandalone )
            {
                case Anchor.CenteredTop:
                    CurrentX = ( 1.0f - CenteredWidthStandalone )/2.0f;
                    CurrentWidth = CenteredWidthStandalone;
                    CurrentHeight = CenteredWidthStandalone/Aspect;
                    CurrentY = 1 - CenteredSpacingStandalone - CurrentHeight;
                    break;
                 
                case Anchor.CenteredBottom:
                    CurrentX = ( 1.0f - CenteredWidthStandalone )/2.0f;
                    CurrentY = CenteredSpacingStandalone;
                    CurrentWidth = CenteredWidthStandalone;
                    CurrentHeight = CurrentWidth/Aspect;
                    break;
             
                case Anchor.Absolute:
                    CurrentX = AbsoluteSpacingLeftStandalone;
                    CurrentY = AbsoluteSpacingBottomStandalone;
                    CurrentWidth = AbsoluteWidthStandalone;
                    CurrentHeight = CurrentWidth/Aspect;
                    break;
                 
                default:
                    break;
            }
         
        }
        else
        {
            switch ( PositionAnchor )
            {
                case Anchor.CenteredTop:
                    CurrentX = ( 1.0f - CenteredWidth )/2.0f;
                    CurrentWidth = CenteredWidth;
                    CurrentHeight = CurrentWidth/Aspect;
                    CurrentY = 1 - CenteredSpacing - CurrentHeight;
                    break;
                 
                case Anchor.CenteredBottom:
                    CurrentX = ( 1.0f - CenteredWidth )/2.0f;
                    CurrentY = CenteredSpacing;
                    CurrentWidth = CenteredWidth;
                    CurrentHeight = CurrentWidth/Aspect;
                    break;
             
                case Anchor.Absolute:
                    CurrentX = AbsoluteSpacingLeft;
                    CurrentY = AbsoluteSpacingBottom;
                    CurrentWidth = AbsoluteWidth;
                    CurrentHeight = CurrentWidth/Aspect;
                    break;
                 
                default:
                    break;
            }
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public void SetCameraAspect ( float aspect )
    {
        Aspect = aspect;
    }

 
    public float X ()
    {
        return CurrentX;
    }


    public float Y ()
    {
        return CurrentY;
    }


    public float Width ()
    {
        return CurrentWidth;
    }


    public float Height ()
    {
        return CurrentHeight;
    }
    // =============================================================================
}