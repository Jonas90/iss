
// Hannes Helmholz
//
// 

using UnityEngine;

[ExecuteInEditMode]

/** Klasse, welche UniqueID's an GameObjekte
 *  vergibt und somit eindeutig macht. Außerdem
 *  können GameObjekte anhand ihrer UniqueID
 *  ermittelt werden.
 */
public class UniqueIDManager : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    public UniqueID[] UniqueIDs;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start ()
    {
        UniqueIDs = FindObjectsOfType ( typeof(UniqueID) ) as UniqueID[];
        for ( int i = 0; i < UniqueIDs.Length; i++ )
        {
            UniqueIDs[i].ID = i;
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public GameObject GetGameObjectByID ( int id )
    {
        return UniqueIDs[id].gameObject;
    }
    // =============================================================================
}