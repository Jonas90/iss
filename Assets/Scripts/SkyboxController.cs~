//dan 
//TODO fix
//
//// Hannes Helmholz
////
//// 
//
//using UnityEngine;
//
//[ExecuteInEditMode]
//
//public class SkyboxController : MonoBehaviour
//{
// 
//    // =============================================================================
//    // MEMBERS ---------------------------------------------------------------------
//    [SerializeField]
//    private bool FreezeHorizonHeight = true;
//    [SerializeField]
//    private float MainCamerasNearClipPlane = 0.35f;
//    [SerializeField]
//    private float MainCamerasFarClipPlane = 1000f;
//    [SerializeField]
//    private float MainCamerasFieldOfView = 60f;
//    private Transform Player;
//    private CharacterController PlayerCC;
//    private Transform Trans;
//    private Camera[] Cams;
//    private bool[] CamsIndividual;
//    // =============================================================================
// 
// 
// 
//    // =============================================================================
//    // METHODS UNITY ---------------------------------------------------------------
// 
//    void Start ()
//    {
//        GameObject obj = GameObject.FindWithTag ( "Player" );
//        if ( obj )
//        {
//            Player = obj.transform;
//            PlayerCC = obj.GetComponent<CharacterController> ();
//        }
//         
//        GameObject[] objects = GameObject.FindGameObjectsWithTag ( "MainCamera" );
//        if ( objects.Length > 0 )
//        { 
//            Cams = new Camera[objects.Length];
//            CamsIndividual = new bool[objects.Length];
//         
//            for ( int i = 0; i < objects.Length; i++ )
//            {
//                Cams[i] = objects[i].GetComponent<Camera> ();
//             
//                ClientCamera client = objects[i].GetComponent<ClientCamera> ();
//                CamsIndividual[i] = client ? client.IndividualPlanes : false;
//            }
//        }
// 
//        Trans = transform;
//    }
// 
// 
//    void Update ()
//    {
//        if ( Player )
//        {
//            float tempX =Player.position.x;
//            float tempY = ( !FreezeHorizonHeight ) ? Player.position.y : Trans.position.y;
//            float tempZ = Player.position.z;
//
//            Trans.position = new Vector3(tempX, tempY, tempZ);
//            Trans.localScale = new Vector3 ( MainCamerasFarClipPlane*2, MainCamerasFarClipPlane*2, MainCamerasFarClipPlane*2 );
//         
//            if ( PlayerCC )
//            { // deactivated in cave on clients
//                Resolution res = Screen.currentResolution;
//                float h = Mathf.Tan ( ( MainCamerasFieldOfView/2.0f )*Mathf.Deg2Rad )*MainCamerasNearClipPlane*2.0f;
//                float w = res.width*h/res.height;
//                PlayerCC.radius = Mathf.Sqrt ( Mathf.Pow ( w/2.0f, 2 ) + Mathf.Pow ( MainCamerasNearClipPlane, 2 ) );
//            }
//        }
//     
//        if ( Cams.Length > 0 )
//        {
//            for ( int i = 0; i < Cams.Length; i++ )
//            {
//                Cams[i].fieldOfView = MainCamerasFieldOfView;
//             
//                if ( !CamsIndividual[i] )
//                {
//                    Cams[i].nearClipPlane = MainCamerasNearClipPlane;
//                    Cams[i].farClipPlane = MainCamerasFarClipPlane;
//                }
//             
//            }
//        }
//    }
//    // =============================================================================
//}