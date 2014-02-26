using UnityEngine;
using System;
using System.Collections;

using Cave;

/** Zustaendig fuer das Neustarten des Spiels.
 *  Dabei wird der Spieler an die Anfangsposition zurueckgesetzt.
 */
public class Restarter : MonoBehaviour
{
    private WiiController wiiController;
    private NetworkObserver observer;
    private int restartFrame = -10;


    void Start ()
    {
        observer = (NetworkObserver) GameObject.FindObjectOfType ( typeof(NetworkObserver) );
        wiiController = (WiiController) GameObject.FindObjectOfType ( typeof(WiiController) );
    }


    void LateUpdate ()
    {
        if ( !GameController.Instance.IsServer )
        {
            return;
        }

        if ( Input.GetKey ( Config.Instance.keyboardButtonReset ) || Config.Instance.UseWii && wiiController.WiiMote.getButtonState ( Config.Instance.wiiButtonReset ) == ButtonState.TOGGLE_DOWN )
        {
            restart ();

            if ( !Config.Instance.IsStandalone )
            {
                observer.SendRestartGame ();
            }

            restartFrame = Time.frameCount;
        }
     
        // Das muss wirklich zwei mal gemacht werden, sonst gibt's Chaos...
        // TODO: fix.
        if ( Time.frameCount == restartFrame + 2 )
        {
            restartFrame = 0;
            restart ();

            if ( !Config.Instance.IsStandalone )
            {
                observer.SendRestartGame ();
            }
        }

        /* Das Crasht zur Zeit. Animation nicht Vorhanden. Müll aus vergangenen Zeiten?
     if (Input.GetKeyDown(KeyCode.Z))
     {
         Logger.Log("!");
         GameObject.Find("Model").animation.Play("Take 001");
     }*/
    }


    public void restart ()
    {
        try
           {
            GameController.Instance.restart ();
        }
        catch ( Exception e )
        {
            Logger.LogException ( e );
        }
    }
}
