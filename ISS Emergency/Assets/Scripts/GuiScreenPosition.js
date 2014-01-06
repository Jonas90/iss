#pragma strict

// Hannes Helmholz
//
// 


public class GuiScreenPosition extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public enum Anchor {CenteredTop, CenteredBottom, Absolute}
	
	public var PositionAnchor:Anchor = Anchor.CenteredBottom;
	public var CenteredSpacing:float = 0.02;
	public var CenteredWidth:float = 0.5;
	public var AbsoluteSpacingLeft:float = 0.02;
	public var AbsoluteSpacingBottom:float = 0.02;
	public var AbsoluteWidth:float = 0.2;
	
	public var DifferentInStandalone:boolean = false;
	
	public var PositionAnchorStandalone:Anchor = Anchor.Absolute;
	public var CenteredSpacingStandalone:float = 0.02;
	public var CenteredWidthStandalone:float = 0.5;
	public var AbsoluteSpacingLeftStandalone:float = 0.02;
	public var AbsoluteSpacingBottomStandalone:float = 0.02;
	public var AbsoluteWidthStandalone:float = 0.25;
	
	private var Config:ConfigClass;
	private var Aspect:float;
	private var CurrentX:float;
	private var CurrentY:float;
	private var CurrentWidth:float;
	private var CurrentHeight:float;
	// =============================================================================

	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------

	function Start() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
	}
	
	
	function Update() {
		if (!Config.OwnClientData.ShowGuiCamera)
			return;
		
		if (Config.IsServer && DifferentInStandalone) {
			switch (PositionAnchorStandalone) {
				case Anchor.CenteredTop:
					CurrentX = (1.0 - CenteredWidthStandalone) / 2.0;
					CurrentWidth = CenteredWidthStandalone;
					CurrentHeight = CenteredWidthStandalone / Aspect;
					CurrentY = 1 - CenteredSpacingStandalone - CurrentHeight;
					break;
					
				case Anchor.CenteredBottom:
					CurrentX = (1.0 - CenteredWidthStandalone) / 2.0;
					CurrentY = CenteredSpacingStandalone;
					CurrentWidth = CenteredWidthStandalone;
					CurrentHeight = CurrentWidth / Aspect;
					break;
				
				case Anchor.Absolute:
					CurrentX = AbsoluteSpacingLeftStandalone;
					CurrentY = AbsoluteSpacingBottomStandalone;
					CurrentWidth = AbsoluteWidthStandalone;
					CurrentHeight = CurrentWidth / Aspect;
					break;
					
				default:
					break;
			}
			
		} else {
			switch (PositionAnchor) {
				case Anchor.CenteredTop:
					CurrentX = (1.0 - CenteredWidth) / 2.0;
					CurrentWidth = CenteredWidth;
					CurrentHeight = CurrentWidth / Aspect;
					CurrentY = 1 - CenteredSpacing - CurrentHeight;
					break;
					
				case Anchor.CenteredBottom:
					CurrentX = (1.0 - CenteredWidth) / 2.0;
					CurrentY = CenteredSpacing;
					CurrentWidth = CenteredWidth;
					CurrentHeight = CurrentWidth / Aspect;
					break;
				
				case Anchor.Absolute:
					CurrentX = AbsoluteSpacingLeft;
					CurrentY = AbsoluteSpacingBottom;
					CurrentWidth = AbsoluteWidth;
					CurrentHeight = CurrentWidth / Aspect;
					break;
					
				default:
					break;
			}
		}
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function SetCameraAspect(aspect:float) {
		Aspect = aspect;
	}
	
	public function X():float { return CurrentX; }
	public function Y():float { return CurrentY; }
	public function Width():float { return CurrentWidth; }
	public function Height():float { return CurrentHeight; }
	// =============================================================================
}