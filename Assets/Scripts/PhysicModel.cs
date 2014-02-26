using UnityEngine;
using System.Collections;

/** Variablenhalter fuer einen Vector3 sowie einem Quaternion.
 *  Dient zur Vereinfachung der Uebergabe dieser beiden Variablen bei einem Update.
 */
public class UpdateInformation
{
    public Vector3 position;
    public Quaternion rotation;
}

/**Verwaltet die Positionen von Models.
 * Beeinflusst die Positionen des Models im Raum.
 * Ermoeglicht das Speichern der aktuellen Position des Models.
 * Ermoeglicht das Zuruecksetzten von Models an die Startposition.
 */
public class PhysicModel : MonoBehaviour
{
    private UpdateInformation lastInformations = new UpdateInformation ();
    private Vector3 startPosition;
    private Quaternion startRotation;

    /// <summary>
    /// Start this instance. Wird an jedes Moddel gebunden
    /// </summary>
    void Start ()
    {
        GameController game = (GameController) GameObject.FindObjectOfType ( typeof(GameController) );
        game.AddToPhysicModels ( this );

        this.startPosition = transform.localPosition;
        this.startRotation = transform.localRotation;
    }

	/**Speichert die aktuelle Position des aktuellen Models in lastInformations.
	 */
    public UpdateInformation GetUpdateInformations ()
    {
        lastInformations.position = transform.position;
        lastInformations.rotation = transform.rotation;
        return lastInformations;
    }

	
	/**Ermoeglicht es das aktelle Model an die Position zu Transformieren, die es zu Beginn hatte.
	 */
    public void reset ()
    {
        this.transform.localPosition = startPosition;
        this.transform.localRotation = startRotation;
        lastInformations = new UpdateInformation ();
    }

    /// <summary>
    /// Information ob ein Model sich weit genug bewegt hat um diese information zu senden.
    /// </summary>
    public bool ShouldBeUpdated ()
    {
        //if (lastInformations.position == transform.position && lastInformations.rotation.eulerAngles == transform.rotation.eulerAngles)
        if ( Vector3.Distance ( lastInformations.position, transform.position ) < 0.005f && lastInformations.rotation.eulerAngles == transform.rotation.eulerAngles )
        {
            return false;
        }

        return true;
    }

	/**
	 * Entfernt dieses model von den Physics.
	 */
    void OnDestroy ()
    {
        GameController.Instance.removeFromPhysics ( this );
    }

	/**Hilfsfunktion um einen Vector3 auszugeben.
	 * @return X, Y und Z Koordinaten als String.
	 */
    public string encode ()
    {
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;

        return gameObject.name + "#" +
         pos.x + "#" + pos.y + "#" + pos.z + "#" +
         rot.x + "#" + rot.y + "#" + rot.z;
    }

	/**Hilfsfunktion um aus einer Reihe von Werten einen Vector3 zu machen.
	 */
    public void decode ( string[] values )
    {
        Vector3 pos = new Vector3 (
         float.Parse ( values[1] ),
         float.Parse ( values[2] ),
         float.Parse ( values[3] ) );

        Quaternion rot = Quaternion.Euler (
         float.Parse ( values[4] ),
         float.Parse ( values[5] ),
         float.Parse ( values[6] ) );

        this.transform.position = pos;
        this.transform.rotation = rot;

        //Config.Log("decoded: " + pos);// + " rot: " + rot.eulerAngles);
    }
}