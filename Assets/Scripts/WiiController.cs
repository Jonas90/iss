using UnityEngine;

using Cave;

public class WiiController : MonoBehaviour
{
    public WiiMote WiiMote { get; private set; }


    void Start ()
    {
        try
              {
            if ( Config.Instance.UseWii )
            {
                Logger.Log ( "Verbinde zu WiiMote: " + Config.Instance.WiiAddress );
                WiiMote = new WiiMote ( Config.Instance.WiiAddress );
            }
        }
        catch ( System.Exception e )
        {
            Logger.LogException ( e );
        }
    }

    /// <summary>
    /// Jedes Grafik-Frame updaten. Bitte den Unterschied zwischen Update und FixedUpdate ansehen:
    /// http://answers.unity3d.com/questions/10993/whats-the-difference-between-update-and-fixedupdat.html
    ///
    /// Bedeutet: Die ToggleUp/Down-Status sind beim Drücken/Loslassen jeweils ein ganzes Frame vorhanden.
    /// Deswegen sollten alle Interaktionen mit der Wii, die auf Toggle-States basieren, in MonoBehaviour.Update()
    /// geschehen.
    /// </summary>
    void Update ()
    {
        try
              {
            if ( Config.Instance.UseWii )
            {
                WiiMote.update ();
            }
        }
        catch ( System.Exception e )
        {
            Logger.LogException ( e );
        }
    }


    void OnDestroy ()
    {
        WiiMote = null;
    }
}
