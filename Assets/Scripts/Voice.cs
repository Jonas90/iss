//6.E
using UnityEngine;
using System;

using ts3client_minimal_sample;

/** Klasse, welche eine TeamSpeak-Verbindung
 *  in die Cave aufbaut. Es gibt einen Client 
 *  in der Cave für das Empfanden der Sprachenachricht
 *  und einen Client für das Mikrofon außerhalb der Cave
 *  zum Einsprechen der Nachricht.
 */
public class Voice// : MonoBehaviour
{
    private TSClient tsc;
    private string ip;
    private int port;
    private string clientSound;
    private string clientMic;
    private bool started;


    public Voice ( string ip, int port, string clientCaveSound, string clientCaveMic )
    {
        this.ip = ip;
        this.port = port;
        this.clientSound = clientCaveSound;
        this.clientMic = clientCaveMic;

        this.started = false;
    }


    public void start ()
    {
        if ( started )
        {
            return;
        }

        bool isCaveSound = System.Environment.MachineName.Equals ( clientSound, StringComparison.OrdinalIgnoreCase );
        started = ( Config.Instance.IsStandalone || isCaveSound );
        if ( started )
        {
            string[] muted;
            if ( isCaveSound )
            {
                muted = new string[] { clientMic };
            }
            else
            {
                muted = new string[] { };
            }

            tsc = new TSClient ( ip, port, "", muted );
            tsc.start ();

            Logger.Log ( "Voice started" );
        }
    }


    public void stop ()
    {
        if ( started )
        {
            tsc.stop ();
            started = false;
        }
    }

    /*
 public bool recording()
 {
     return true;
 }

 public bool talking()
 {
     return true;
 }
  * */
}
