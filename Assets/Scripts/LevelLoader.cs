
// Hannes Helmholz
//
// loader script for application
// to show image while big scene with iss is loading

using UnityEngine;
using System;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private Camera Cam;
    [SerializeField]
    private float FadeSpeed = 1.0f;
    private GameObject This;
    private AsyncOperation Async;
    private bool LoadedFirst = false;
    private bool LoadedSecond = false;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start ()
    {
        if ( !Application.isEditor )
        {
            Screen.lockCursor = true;
        }
     
        This = gameObject;
     
        Async = Application.LoadLevelAdditiveAsync ( 1 ); // start loading
        Async.allowSceneActivation = false;
        //yield Async;
    }
 
 
    void Update ()
    {
        if ( Async.progress >= 0.9f && !LoadedFirst )
        {
         // value progress will stop at 0.9 -> scene is ready
            AllowStart ();
        }
         
        if ( Async.isDone && !LoadedSecond )
        {
         // value progress is 1.0 -> scene is full loaded
            FadeImage ();
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS  --------------------------------------------------------------------
 
    private void AllowStart ()
    {
        LoadedFirst = true;
        Async.allowSceneActivation = true;
    }
 
 
    // do fade with camere field of view
    // and destroy this gameObject
    private void FadeImage ()
    {
        Cam.fieldOfView -= FadeSpeed;
         
        if ( Cam.fieldOfView <= 1 )
        {
            LoadedSecond = true;
            Destroy ( This, 1 );
        }
    }
    // =============================================================================
}