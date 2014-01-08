//dan
//// Hannes Helmholz
////
//// give different possibilities to interact with scene
//// with different key for every input device
//// in combination with InputManager
//
//using UnityEngine;
//using System.Collections;
//
//public class InteractionKey : System.Object
//{    
//    // =============================================================================
//    // MEMBERS ---------------------------------------------------------------------
//    public enum MouseKey
//    {
//        Left = 0,
//        Right = 1,
//        Middle = 2,
//        WheelUp = 3,
//        WheelDown = 4,
//        None = 5
//    }
// 
//    [SerializeField]
//    private MouseKey ButtonMouse = MouseKey.None;
//    [SerializeField]
//    private string ButtonKeyboard = ""; // optional
//    [SerializeField]
//    private string ButtonXbox = ""; // optional
//    [SerializeField]
//    private string ButtonSpacepilot = ""; // optional
// 
//    private Config Config;
//    // =============================================================================
//
//
//
//    // =============================================================================
//    // METHODS  --------------------------------------------------------------------
// 
//    // must be used because Start() is not available (derives not from MonoBehaviour)
//    public void Initialize ()
//    {
//        Config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
//    }
// 
// 
//    public bool GetButtonDown ( Config.Device device )
//    {
//        if ( device == Config.Device.Keyboard && ButtonMouse != MouseKey.None )
//        {
//            bool mouse;
//            int buttonNumber = (int) ButtonMouse;
//         
//            if ( ButtonMouse == MouseKey.WheelUp )
//            {
//                mouse = Input.GetAxis ( "Mouse ScrollWheel" ) > 0;
//            }
//            else if ( ButtonMouse == MouseKey.WheelDown )
//            {
//                mouse = Input.GetAxis ( "Mouse ScrollWheel" ) < 0;
//            }
//            else
//            {
//                mouse = Input.GetMouseButtonDown ( buttonNumber );
//            }
//         
//            return Input.GetButtonDown ( ButtonKeyboard ) || mouse;
//        }
// 
//        string button = GetCurrentButton ( device );
//        if ( button.Length == 0 )
//        {
//            return false;
//        }
//
//        return Input.GetButtonDown ( button );
//    }
// 
// 
//    public bool GetButtonDown ()
//    {
//        return GetButtonDown ( Config.InputDevice );
//    }
// 
// 
//    public string GetCurrentButton ( Config.Device device )
//    {
//        return GetCurrentButton ( device, "  /  " );
//    }
// 
// 
//    public string GetCurrentButton ( Config.Device device, string separator )
//    {
//        string button = "";
//     
//        // in case of NullReferenceException you forgot to call Initialize() perhaps
//        switch ( device )
//        {
//            case Config.Device.Spacepilot:
//                button = ButtonSpacepilot;
//                break;
//             
//            case Config.Device.Xbox:
//                button = ButtonXbox;
//                break;
//             
//            case Config.Device.Keyboard:
//                button = ButtonKeyboard;
//                if ( ButtonMouse != MouseKey.None )
//                {
//                    button += separator + "Mouse" + ButtonMouse;
//                }
//                break;
//        }
//     
//        return button;
//    }
// 
// 
//    public string GetCurrentButton ()
//    {
//        return GetCurrentButton ( Config.InputDevice );
//    }
// 
// 
//    public string GetCurrentButton ( string separator )
//    {       
//        return GetCurrentButton ( Config.InputDevice, separator );
//    }
//    // =============================================================================
//}