
// Hannes Helmholz
//
// give level overview as a map
// 
// needs camera, i.e. ortographic
// camera aspect ratio can be set by normalized view port rect
// ortographic camera can be set on discrete height with near clipping plane at
// 0 to cut through roofs
//
// screen position in viewport coordinates [0..l] 

using UnityEngine;

[RequireComponent (typeof(GuiScreenPosition))]

public class Overview : MonoBehaviour
{
 
    // =========================================================================
    // MEMBERS -----------------------------------------------------------------
    [SerializeField]
    private Camera Cam;
    [SerializeField]
    private GUITexture Background; // optional

    private Config Config;
    private GuiScreenPosition ScreenPosition;
    private Transform BackgroundTrans;
    // =========================================================================



    // =========================================================================
    // METHODS UNITY -----------------------------------------------------------
 
    void Start ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
     
        ScreenPosition = gameObject.GetComponent<GuiScreenPosition> ();
     
        if ( Background )
        {
            BackgroundTrans = Background.transform;
        }
     
        if ( Cam.aspect >= 1 )
        {
            ScreenPosition.SetCameraAspect ( Cam.rect.width/Cam.rect.height );
        }
        else
        {
            ScreenPosition.SetCameraAspect ( Cam.rect.height/Cam.rect.width );
        }
    }
 
 
    void Update ()
    {
//        if ( !Config.OwnClientData.ShowGuiCamera || ( Config.IsStandalone () && !Screen.lockCursor ) )
//        {
//            Cam.enabled = false;
//            if ( Background )
//            {
//                Background.enabled = false;
//            }
//         
//            return;
//        }
//     
//        Cam.enabled = true;
//        Cam.rect = new Rect ( 0, 0, 1, 1 );
//     
//        Vector3 xy = Cam.ViewportToScreenPoint ( new Vector3 ( ScreenPosition.X (), ScreenPosition.Y (), 0 ) );
//        Vector3 wh = Cam.ViewportToScreenPoint ( new Vector3 ( ScreenPosition.Width (), ScreenPosition.Height (), 0 ) );
//        Rect screenPixels = new Rect ( xy.x, xy.y, wh.x, wh.y );
//        Cam.pixelRect = screenPixels;
// 
//        if ( Background )
//        {
//            Background.enabled = true;
//            BackgroundTrans.localScale = Vector3.zero; 
//            Background.pixelInset = screenPixels;
//        }
    }
    // =========================================================================
}