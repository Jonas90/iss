using UnityEngine;
using System.Collections;

public class DestroyGameObjectsAfterPlay : MonoBehaviour
{
    private float totalTimeBeforeDestroy;

    /// <summary>
    /// for Soundinstane
    /// </summary>
    void Start()
    {
        var sound = this.GetComponent<AudioSource>();
        totalTimeBeforeDestroy = sound.clip.length;
    }


    void Update()
    {
        totalTimeBeforeDestroy -= Time.deltaTime;

        if( totalTimeBeforeDestroy <= 0f )
        {
            Destroy( this.gameObject );
        }
    }
}