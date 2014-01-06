#pragma strict

// Hannes Helmholz
//
// 


@script RequireComponent(NetworkView) // for RPC

private class MeterAbstract extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField protected var SoundSuccess:AudioClip;
	@SerializeField protected var SoundError:AudioClip;
	@SerializeField protected var DisplayLeft:TextMesh;
	@SerializeField protected var DisplayRight:TextMesh;
	@SerializeField protected var DisplayColor:Color;
	
	protected var Config:ConfigClass;
	protected var Trans:Transform;
	protected var Audio:AudioSource;
	protected var Equip:Equipment;
	protected var NetView:NetworkView;
	protected var PlayerMov:PlayerMovement;
	protected var PlayerRot:PlayerRotation;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		
		Trans = transform;
		Audio = audio;
		NetView = networkView;
		
		var player:GameObject = GameObject.FindWithTag("Player");
		PlayerMov = player.GetComponent.<PlayerMovement>();
		PlayerRot = player.GetComponent.<PlayerRotation>();
		Equip = gameObject.GetComponent.<Equipment>();
		
		if (DisplayLeft)
			DisplayLeft.renderer.material.color = DisplayColor;
		if (DisplayRight)
			DisplayRight.renderer.material.color = DisplayColor;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------

	public function IsAvailable():boolean {
		return Equip.IsAvailable();
	}
	
	
	public function IsEquipped():boolean {
		return Equip.IsEquipped();
	}
	
	
	protected function MeasureValues(time:float) {
		if (!IsAvailable())
			return;
		
		Audio.Play();
		yield WaitForSeconds(time);
		Audio.Stop();
	}
	
	
	protected function DisplayValues(values:MeasurementValues) {
		if (!values) {
			DisplayValues("ERR" + System.Environment.NewLine + "", "");
			return;
		}
			
		var textLeft:String = GetDotValue(values.O2) + System.Environment.NewLine + GetDotValue(values.HCN);
		var textRight:String = GetDotValue(values.CO) + System.Environment.NewLine + GetDotValue(values.HCL);
		DisplayValues(textLeft, textRight);
	}
	
	
	protected function DisplayValues(textLeft:String, textRight:String) {
		NetView.RPC("RPCDisplayValues", RPCMode.AllBuffered, textLeft, textRight);
	}
	
	
	private function GetDotValue(val:float):String {
		var text:String = val.ToString();
		
		if (val == 0)
			text = ".0"; // 0 -> .0
		else if (!text.Contains("."))
			text += ".0"; // 21 -> 21.0
		else if (val < 1)
			text = text.Substring(1); // 0.1 -> .1
		
		return text;
	} 
	
	
	protected function SetPlayerMotion(status:boolean) {
		PlayerMov.enabled = status;
		PlayerRot.TemporaryDisable = !status;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCDisplayValues(textLeft:String, textRight:String) {
		DisplayLeft.text = textLeft;
		DisplayRight.text = textRight;
	}
	// =============================================================================
}