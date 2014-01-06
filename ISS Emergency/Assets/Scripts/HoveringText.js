#pragma strict

// Hannes Helmholz
//
// textoverlay on target object
// started by ViewOnTarget()
// stoped by Hide()


@script RequireComponent(GUIText)

private class HoveringText extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	private enum FadeState {None, FadeIn, FadeOut}
	
	@SerializeField private var TextColor:Color;
	@SerializeField private var Cam:Camera;
	@SerializeField private var Player:Transform;
	@SerializeField private var Background:GUITexture; // optional
	@SerializeField private var BackgroundSpacing:Vector2 = Vector2(10, 10); // optional
	@SerializeField private var FadeDuration:float = 0.5; // optional
	
	private var Trans:Transform;
	private var TransTarget:Transform;
	private var TransTargetOffset:Vector3;
	private var GuiText:GUIText;
	//private var GuiTextAlphaOriginal:float;
	//private var FontBaseSize:float;
	private var BackgroundTransform:Transform;
	private var BackgroundAlphaOriginal:float;
	private var TimeStart:float;
	private var TimeLeft:float;
	private var Status:FadeState;
	
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Trans = transform;
		
		GuiText = guiText;
		GuiText.font.material.color = TextColor;
		//GuiTextAlphaOriginal = GuiText.font.material.color.a;
		//FontBaseSize = GuiText.fontSize;
		
		if (Background) {
			BackgroundTransform = Background.transform;
			BackgroundAlphaOriginal = Background.color.a;
			
			BackgroundTransform.localScale = Vector3.zero; // should be done, dont know why
			Background.pixelInset = Rect(0, 0, 0, 0);
		}
	}
	
	
	function Update() {
		if (GuiText.text.Length == 0)
			return;
		
		var ray:Vector3 = TransTarget.position - Player.position;
		ray += Quaternion.Euler(Player.rotation.eulerAngles) * TransTargetOffset;
		Trans.position = Cam.WorldToViewportPoint(ray + Player.position);
		
		// fontSize is just integer -> value is jumping :(
		//GuiText.fontSize = FontBaseSize / (TransTarget.position - Player.position).magnitude;
		
		if (Background) {
			var rect:Rect = GuiText.GetScreenRect(Cam);
			BackgroundTransform.localScale = Vector3.zero; // should be done, dont know why
			
			Background.pixelInset = Rect(	(-rect.width  / 2) - BackgroundSpacing.x,
											(-rect.height / 2) - BackgroundSpacing.y,
											rect.width  + 2 * BackgroundSpacing.x,
											rect.height + 2 * BackgroundSpacing.y);
		}
										
		TimeLeft = TimeStart + FadeDuration - Time.time;
		if (Status == FadeState.FadeIn)
			Fade(true);
		else if (Status == FadeState.FadeOut)
			Fade(false);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	public function ViewOnTarget(target:Transform, offset:Vector3, text:String) {
		Status = FadeState.FadeIn;
		TransTarget = target;
		TransTargetOffset = offset;
		GuiText.text = text;
		
		TimeStart = Time.time;
	}
	
	
	public function Hide() {
		Status = FadeState.FadeOut;
		
		TimeStart = Time.time;
	}
	
	
	private function Fade(fadeIn:boolean) {
		var a:float = TimeLeft / FadeDuration;
		if (a < 0) {
	    	a = 0;
	    	Status = FadeState.None;
	    	
	    	if (!fadeIn)
	    		GuiText.text = "";
	    }
	    
	    if (fadeIn)
	    	a = 1 - a;
	    
	    GuiText.font.material.color.a = a/* * GuiTextAlphaOriginal*/;
	    if (Background)
	    	Background.color.a = a * BackgroundAlphaOriginal;
	}
	// =============================================================================
}