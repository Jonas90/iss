#pragma strict
//#pragma implicit
//#pragma downcast

// Hannes Helmholz
//
// The blur iteration shader.
// Basically it just takes 4 texture samples and averages them.
// By applying it repeatedly and spreading out sample locations
// we get a Gaussian blur approximation.


@script ExecuteInEditMode
@script AddComponentMenu("Image Effects/Funky Glowing Things")
@script RequireComponent(Camera)


private class FunkyGlowingThingsEffect extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var iterations:int = 2; // Blur iterations - larger number means more blur.
	@SerializeField	private var blurSpread:float = 0.5; // Blur spread for each iteration. Lower values give better looking blur, but require more iterations to get large blurs. Value is usually between 0.5 and 1.0.
	@SerializeField	private var colorRamp:Texture;
	@SerializeField	private var blurRamp:Texture;
	@SerializeField	private var compositeShader:Shader;
	@SerializeField	private var renderThingsShader:Shader;
	
	private var m_Material:Material;
	private var m_CompositeMaterial:Material;
	private var renderTexture:RenderTexture;
	private var shaderCamera:GameObject;
	
	private static var blurMatString:String =
		"Shader \"BlurConeTap\" { SubShader { Pass { " +
			"ZTest Always Cull Off ZWrite Off Fog { Mode Off } " +
			"	SetTexture [__RenderTex] {constantColor (0,0,0,0.25) combine texture * constant alpha} " +
			"	SetTexture [__RenderTex] {constantColor (0,0,0,0.25) combine texture * constant + previous} " +
			"	SetTexture [__RenderTex] {constantColor (0,0,0,0.25) combine texture * constant + previous} " +
			"	SetTexture [__RenderTex] {constantColor (0,0,0,0.25) combine texture * constant + previous} " +
		"} } Fallback off }";
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			return;
		}
		// Disable if the shader can't run on the users graphics card
		if (!GetMaterial().shader.isSupported) {
			enabled = false;
			return;
		}
	}
	
	
	function OnDisable() {	
		if( m_Material ) {
			DestroyImmediate( m_Material.shader );
			DestroyImmediate( m_Material );
		}
		DestroyImmediate (m_CompositeMaterial);
		DestroyImmediate (shaderCamera);
		if (renderTexture != null) {
			RenderTexture.ReleaseTemporary (renderTexture);
			renderTexture = null;
		}
	}
	
	
	function OnPreRender() {
		if (!enabled || !gameObject.activeSelf)
			return;
			
		if (renderTexture != null) {
			RenderTexture.ReleaseTemporary (renderTexture);
			renderTexture = null;
		}
		renderTexture = RenderTexture.GetTemporary (camera.pixelWidth, camera.pixelHeight, 16);
		if (!shaderCamera) {
			shaderCamera = new GameObject("ShaderCamera", Camera);
			shaderCamera.camera.enabled = false;
			shaderCamera.hideFlags = HideFlags.HideAndDontSave;
		}
		
		var cam:Camera = shaderCamera.camera;
		cam.CopyFrom(camera);
		cam.projectionMatrix = camera.projectionMatrix; // added by Hannes Helmholz  :P
		cam.backgroundColor = Color(0,0,0,0);
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.targetTexture = renderTexture;
		cam.RenderWithShader (renderThingsShader, "RenderType");
	}
	
	
	// Called by the camera to apply the image effect
	function OnRenderImage(source:RenderTexture, destination:RenderTexture) {
		var sourceWidth:float = source.width;
		var sourceHeight:float = source.height;
		var buffer:RenderTexture = RenderTexture.GetTemporary(sourceWidth / 4, sourceHeight / 4, 0);
		var buffer2:RenderTexture = RenderTexture.GetTemporary(sourceWidth / 4, sourceHeight / 4, 0);
		
		// Copy things mask to the 4x4 smaller texture.
		DownSample4x (renderTexture, buffer);
		
		// Blur the small texture
		var oddEven:boolean = true;
		for(var i:int = 0; i < iterations; i++)
		{
			if( oddEven )
				FourTapCone (buffer, buffer2, i);
			else
				FourTapCone (buffer2, buffer, i);
			oddEven = !oddEven;
		}
		var compositeMat:Material = GetCompositeMaterial();
		compositeMat.SetTexture ("_BlurTex", oddEven ? buffer : buffer2);
		compositeMat.SetTexture ("_ColorRamp", colorRamp);
		compositeMat.SetTexture ("_BlurRamp", blurRamp);
		
		ImageEffects.BlitWithMaterial(compositeMat, source, destination);
		
		RenderTexture.ReleaseTemporary(buffer);
		RenderTexture.ReleaseTemporary(buffer2);
		
		if (renderTexture != null) {
			RenderTexture.ReleaseTemporary (renderTexture);
			renderTexture = null;
		}
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS STATIC --------------------------------------------------------------
	
	private static function Render4TapQuad( dest : RenderTexture, offsetX : float, offsetY : float ) {
		GL.Begin( GL.QUADS );
	
		// Direct3D needs interesting texel offsets!		
		var off:Vector2 = Vector2.zero;
		if( dest != null )
			off = dest.GetTexelOffset() * 0.75;
		
		Set4TexCoords( off.x, off.y, offsetX, offsetY );
		GL.Vertex3( 0,0, 0.1 );
		
		Set4TexCoords( 1.0 + off.x, off.y, offsetX, offsetY );
		GL.Vertex3( 1,0, 0.1 );
		
		Set4TexCoords( 1.0 + off.x, 1.0 + off.y, offsetX, offsetY );
		GL.Vertex3( 1,1, 0.1 );
		
		Set4TexCoords( off.x, 1.0 + off.y, offsetX, offsetY );
		GL.Vertex3( 0,1, 0.1 );
		
		GL.End();
	}
	
	
	private static function Set4TexCoords( x : float, y : float, offsetX : float, offsetY : float ) {
		GL.MultiTexCoord2( 0, x - offsetX, y - offsetY );
		GL.MultiTexCoord2( 1, x + offsetX, y - offsetY );
		GL.MultiTexCoord2( 2, x + offsetX, y + offsetY ); 
		GL.MultiTexCoord2( 3, x - offsetX, y + offsetY );
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	private function GetMaterial():Material {
		if (m_Material == null) {
			m_Material = new Material( blurMatString );
			m_Material.hideFlags = HideFlags.HideAndDontSave;
			m_Material.shader.hideFlags = HideFlags.HideAndDontSave;
		}
		return m_Material;
	} 
	
	
	private function GetCompositeMaterial():Material {
		if (m_CompositeMaterial == null) {
			m_CompositeMaterial = new Material( compositeShader );
			m_CompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		return m_CompositeMaterial;
	} 
	
	
	// Performs one blur iteration.
	private function FourTapCone (source : RenderTexture, dest : RenderTexture, iteration : int) {
		RenderTexture.active = dest;
		source.SetGlobalShaderProperty ("__RenderTex");
		
		var offsetX:float = (0.5+iteration*blurSpread) / source.width;
		var offsetY:float = (0.5+iteration*blurSpread) / source.height;
		GL.PushMatrix ();
		GL.LoadOrtho ();    
		
		var mat:Material = GetMaterial();
		for (var i:int = 0; i < mat.passCount; ++i) {
			mat.SetPass (i);
			Render4TapQuad( dest, offsetX, offsetY );
		}
		GL.PopMatrix ();
	}
	
	
	// Downsamples the texture to a quarter resolution.
	private function DownSample4x (source : RenderTexture, dest : RenderTexture) {
		RenderTexture.active = dest;
		source.SetGlobalShaderProperty ("__RenderTex");
		
		var offsetX:float = 1.0 / source.width;
		var offsetY:float = 1.0 / source.height;
		
		GL.PushMatrix ();
		GL.LoadOrtho ();
		var mat:Material = GetMaterial();
		for (var i:int = 0; i < mat.passCount; ++i)
		{
			mat.SetPass (i);
			Render4TapQuad( dest, offsetX, offsetY );
		}
		GL.PopMatrix ();
	}
	// =============================================================================
}