
//#pragma implicit
//#pragma downcast

// Hannes Helmholz
//
// The blur iteration shader.
// Basically it just takes 4 texture samples and averages them.
// By applying it repeatedly and spreading out sample locations
// we get a Gaussian blur approximation.
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu ("Image Effects/Funky Glowing Things")]
[RequireComponent (typeof(Camera))]


public class FunkyGlowingThingsEffect : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private int iterations = 2; // Blur iterations - larger number means more blur.
    [SerializeField]
    private float blurSpread = 0.5f; // Blur spread for each iteration. Lower values give better looking blur, but require more iterations to get large blurs. Value is usually between 0.5 and 1.0.
    [SerializeField]
    private Texture colorRamp;
    [SerializeField]
    private Texture blurRamp;
    [SerializeField]
    private Shader compositeShader;
    [SerializeField]
    private Shader renderThingsShader;
    private Material m_Material;
    private Material m_CompositeMaterial;
    private RenderTexture renderTexture;
    private GameObject shaderCamera;
    private static string blurMatString =
     "Shader \"BlurConeTap\" { SubShader { Pass { " +
         "ZTest Always Cull Off ZWrite Off Fog { Mode Off } " +
         "   SetTexture [__RenderTex] {constantColor (0,0,0,0.25) combine texture * constant alpha} " +
         "   SetTexture [__RenderTex] {constantColor (0,0,0,0.25) combine texture * constant + previous} " +
         "   SetTexture [__RenderTex] {constantColor (0,0,0,0.25) combine texture * constant + previous} " +
         "   SetTexture [__RenderTex] {constantColor (0,0,0,0.25) combine texture * constant + previous} " +
     "} } Fallback off }";
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start()
    {
        // Disable if we don't support image effects
        if( !SystemInfo.supportsImageEffects )
        {
            enabled = false;
            return;
        }
        // Disable if the shader can't run on the users graphics card
        if( !GetMaterial().shader.isSupported )
        {
            enabled = false;
            return;
        }
    }
 
 
    void OnDisable()
    {  
        if( m_Material )
        {
            DestroyImmediate( m_Material.shader );
            DestroyImmediate( m_Material );
        }
        DestroyImmediate( m_CompositeMaterial );
        DestroyImmediate( shaderCamera );
        if( renderTexture != null )
        {
            RenderTexture.ReleaseTemporary( renderTexture );
            renderTexture = null;
        }
    }
 
 
    void OnPreRender()
    {
        if( !enabled || !gameObject.activeSelf )
        {
            return;
        }
         
        if( renderTexture != null )
        {
            RenderTexture.ReleaseTemporary( renderTexture );
            renderTexture = null;
        }
        renderTexture = RenderTexture.GetTemporary( (int) (0.5 + camera.pixelWidth), (int) (0.5 + camera.pixelHeight), 16 );
        if( !shaderCamera )
        {
            shaderCamera = new GameObject( "ShaderCamera");
            shaderCamera.AddComponent("Camera");
            shaderCamera.camera.enabled = false;
            shaderCamera.hideFlags = HideFlags.HideAndDontSave;
        }
     
        Camera cam = shaderCamera.camera;
        cam.CopyFrom( camera );
        cam.projectionMatrix = camera.projectionMatrix; // added by Hannes Helmholz  :P
        cam.backgroundColor = new Color( 0, 0, 0, 0 );
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.targetTexture = renderTexture;
        cam.RenderWithShader( renderThingsShader, "RenderType" );
    }
 
 
    // Called by the camera to apply the image effect
    void OnRenderImage( RenderTexture source, RenderTexture destination )
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        RenderTexture buffer = RenderTexture.GetTemporary( sourceWidth/4, sourceHeight/4, 0 );
        RenderTexture buffer2 = RenderTexture.GetTemporary( sourceWidth/4, sourceHeight/4, 0 );
     
        // Copy things mask to the 4x4 smaller texture.
        DownSample4x( renderTexture, buffer );
     
        // Blur the small texture
        bool oddEven = true;
        for( int i = 0; i < iterations; i++ )
        {
            if( oddEven )
            {
                FourTapCone( buffer, buffer2, i );
            }
            else
            {
                FourTapCone( buffer2, buffer, i );
            }
            oddEven = !oddEven;
        }
        Material compositeMat = GetCompositeMaterial();
        compositeMat.SetTexture( "_BlurTex", oddEven ? buffer : buffer2 );
        compositeMat.SetTexture( "_ColorRamp", colorRamp );
        compositeMat.SetTexture( "_BlurRamp", blurRamp );
     
        ImageEffects.BlitWithMaterial( compositeMat, source, destination );
     
        RenderTexture.ReleaseTemporary( buffer );
        RenderTexture.ReleaseTemporary( buffer2 );
     
        if( renderTexture != null )
        {
            RenderTexture.ReleaseTemporary( renderTexture );
            renderTexture = null;
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS STATIC --------------------------------------------------------------
 
    private static void Render4TapQuad( RenderTexture dest, float offsetX, float offsetY )
    {
        GL.Begin( GL.QUADS );
 
        // Direct3D needs interesting texel offsets!        
        Vector2 off = Vector2.zero;
        if( dest != null )
        {
            off = dest.GetTexelOffset()*0.75f;
        }
     
        Set4TexCoords( off.x, off.y, offsetX, offsetY );
        GL.Vertex3( 0f, 0f, 0.1f );
     
        Set4TexCoords( 1.0f + off.x, off.y, offsetX, offsetY );
        GL.Vertex3( 1f, 0f, 0.1f );

        Set4TexCoords( 1.0f + off.x, 1.0f + off.y, offsetX, offsetY );
        GL.Vertex3( 1f, 1f, 0.1f );

        Set4TexCoords( off.x, 1.0f + off.y, offsetX, offsetY );
        GL.Vertex3( 0f, 1f, 0.1f );
     
        GL.End();
    }
 
 
    private static void Set4TexCoords( float x, float y, float offsetX, float offsetY )
    {
        GL.MultiTexCoord2( 0, x - offsetX, y - offsetY );
        GL.MultiTexCoord2( 1, x + offsetX, y - offsetY );
        GL.MultiTexCoord2( 2, x + offsetX, y + offsetY ); 
        GL.MultiTexCoord2( 3, x - offsetX, y + offsetY );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS  --------------------------------------------------------------------
 
    private Material GetMaterial()
    {
        if( m_Material == null )
        {
            m_Material = new Material( blurMatString );
            m_Material.hideFlags = HideFlags.HideAndDontSave;
            m_Material.shader.hideFlags = HideFlags.HideAndDontSave;
        }
        return m_Material;
    }
 
 
    private Material GetCompositeMaterial()
    {
        if( m_CompositeMaterial == null )
        {
            m_CompositeMaterial = new Material( compositeShader );
            m_CompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        return m_CompositeMaterial;
    } 
 
 
    // Performs one blur iteration.
    private void FourTapCone( RenderTexture source, RenderTexture dest, int iteration )
    {
        RenderTexture.active = dest;
        source.SetGlobalShaderProperty( "__RenderTex" );
     
        float offsetX = ( 0.5f + iteration*blurSpread )/source.width;
        float offsetY = ( 0.5f + iteration*blurSpread )/source.height;
        GL.PushMatrix();
        GL.LoadOrtho();    
     
        Material mat = GetMaterial();
        for( int i = 0; i < mat.passCount; ++i )
        {
            mat.SetPass( i );
            Render4TapQuad( dest, offsetX, offsetY );
        }
        GL.PopMatrix();
    }
 
 
    // Downsamples the texture to a quarter resolution.
    private void DownSample4x( RenderTexture source, RenderTexture dest )
    {
        RenderTexture.active = dest;
        source.SetGlobalShaderProperty( "__RenderTex" );
     
        float offsetX = 1.0f/source.width;
        float offsetY = 1.0f/source.height;
     
        GL.PushMatrix();
        GL.LoadOrtho();
        Material mat = GetMaterial();
        for( int i = 0; i < mat.passCount; ++i )
        {
            mat.SetPass( i );
            Render4TapQuad( dest, offsetX, offsetY );
        }
        GL.PopMatrix();
    }
    // =============================================================================
}