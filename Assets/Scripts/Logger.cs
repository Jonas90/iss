//dan
//Logger Klasse aus dem 6. Etage Projekt. 

using UnityEngine;
using System;

public class Logger : MonoBehaviour
{    
    private static string __onScreenText = "";
    /// <summary>
    /// Beim Aufruf durch Teamspeak blockte Debug.isDebugBuild. Deswegen einfach hier merken...
    /// </summary>
    private static bool isDebugBuild = true;


    void Awake ()
    {
     
        isDebugBuild = Debug.isDebugBuild;
    }


    void OnGUI ()
    {        
        if ( isDebugBuild && !Application.isEditor )
        {
            GUILayout.BeginArea ( new Rect ( Screen.width*0.6f, 10, Screen.width*0.35f, Screen.height*0.9f ) );
            GUILayout.BeginScrollView ( new Vector2 ( 0, float.MaxValue ), GUILayout.Width ( Screen.width*0.35f ), GUILayout.Height ( Screen.height*0.9f ) );
            GUILayout.Label ( __onScreenText );
            GUILayout.EndScrollView ();
            GUILayout.EndArea ();
        }
    }


    public static void Log ( object message )
    {
        LogToOther ( message.ToString () );
        Debug.Log ( message );
    }


    public static void LogWarning ( object message )
    {
        string msg = "WARNUNG: " + message;
        LogToOther ( msg );
        Debug.LogWarning ( message );
    }


    public static void LogError ( object message )
    {
        string msg = "FEHLER: " + message;
        LogToOther ( msg );
        Debug.LogError ( message );
    }


    public static void LogException ( Exception ex )
    {
        string msg = "EXCEPTION: " + ex;
        LogToOther ( msg );
        Debug.LogException ( ex );
    }


    private static void LogToOther ( string message )
    {
        if ( isDebugBuild )
        {
            __onScreenText += message + "\n";
        }
    }
}
