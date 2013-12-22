
// Hannes Helmholz
//
// 
using UnityEngine;

/*
public struct NearPlane
{
    public float left;
    public float right;
    public float top;
    public float bottom;
    public float near;
    public float far;
}
*/
public static class ClientCameraVanishingPoint
{

 
    // =============================================================================
    // METHODS STATIC --------------------------------------------------------------
 
    public static void SetVanishingPoint ( Camera cam, float offset, ClientCameraScreen screen )
    {
        Transform t = cam.transform;
        NearPlane plane = new NearPlane();

        Vector3 nearCenter = t.position + t.forward*cam.nearClipPlane;
        Plane nearPlane = new Plane ( -t.forward, nearCenter );
        float distance = 0f;
        Vector3 direction;
        Ray ray;

        Vector3 screenTL = t.TransformPoint ( new Vector3 ( ( -screen.Width/2.0f ) + offset, screen.Height/2.0f, screen.Distance ) );
        direction = ( screenTL - t.position ).normalized;
        ray = new Ray ( t.position, direction );
        nearPlane.Raycast ( ray, out distance );
        Vector3 nearTL = -( t.InverseTransformPoint ( nearCenter ) - t.InverseTransformPoint ( ( t.position + direction*distance ) ) );

        Vector3 screenBR = t.TransformPoint ( new Vector3 ( ( screen.Width/2.0f ) + offset, -screen.Height/2.0f, screen.Distance ) );
        direction = ( screenBR - t.position ).normalized;
        ray = new Ray ( t.position, direction );
        nearPlane.Raycast ( ray, out distance );
        Vector3 nearBR = -( t.InverseTransformPoint ( nearCenter ) - t.InverseTransformPoint ( ( t.position + direction*distance ) ) );

        plane.left = nearTL.x;
        plane.top = nearTL.y;
        plane.right = nearBR.x;
        plane.bottom = nearBR.y;
        plane.near = cam.nearClipPlane;
        plane.far = cam.farClipPlane;
        cam.projectionMatrix = PerspectiveOffCenter ( plane );
    }


    private static Matrix4x4 PerspectiveOffCenter ( NearPlane plane )
    {
        float x = ( 2.0f*plane.near )/( plane.right - plane.left );
        float y = ( 2.0f*plane.near )/( plane.top - plane.bottom );
        float a = ( plane.right + plane.left )/( plane.right - plane.left );
        float b = ( plane.top + plane.bottom )/( plane.top - plane.bottom );
        float c = -( plane.far + plane.near )/( plane.far - plane.near );
        float d = -( 2.0f*plane.far*plane.near )/( plane.far - plane.near );
        float e = -1.0f;
        Matrix4x4 m = new Matrix4x4 ();
     
        m[0, 0] = x;
        m[0, 1] = 0.0f;
        m[0, 2] = a;
        m[0, 3] = 0.0f;
        m[1, 0] = 0.0f;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0.0f;
        m[2, 0] = 0.0f;
        m[2, 1] = 0.0f;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0.0f;
        m[3, 1] = 0.0f;
        m[3, 2] = e;
        m[3, 3] = 0.0f;
     
        return m;
    }
    // =============================================================================
}