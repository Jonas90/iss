#pragma strict

// Hannes Helmholz
//
// base class for TriggerButton
// needs FunkyGlowingThingsEffect on a camera to get shown
// 
// player have to be in range to get shown
// mouse have to be over object to get shown


@script RequireComponent(NetworkView) // for RPC

private class FunkyGlowingThingsElement extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var MaterialOn:Material;
	@SerializeField private var ChangeChildMaterials:boolean = true;
	@SerializeField	private var PlayerMinDistance:float = 3;
	
	protected var Config:ConfigClass;
	protected var Trans:Transform;
	protected var Renderers:RendererData[];
	protected var NetView:NetworkView;
	protected var Player:Transform;
	protected var MouseInside:boolean = false;
	protected var Status:boolean = false;
	protected var StatusLast:boolean = false;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		Player = GameObject.FindWithTag("Player").transform;
		Trans = transform;
		NetView = networkView;
		
		GeneratRenderersData();
	}
	
	
	function Update() {
		if (!Config.IsServer)
			return;
		
		Status = MouseInside && CalculateDistance(Trans, Player) <= PlayerMinDistance;
		if (Status != StatusLast && NetView)
			NetView.RPC("RPCSetStatus", RPCMode.AllBuffered, Status);
			
		StatusLast = Status;
	}
	
	
	function OnMouseEnter() {
		MouseInside = true;
	}
	
	
	function OnMouseExit() {
		MouseInside = false;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
		
	public function GetIsTriggered():boolean {
		return Status;
	}
	
	
	public static function CalculateDistance(object1:Transform, object2:Transform):float {
		return (object1.position - object2.position).magnitude;
	}
	
	
	private function GeneratRenderersData() {
		var childs:Renderer[];
		if (ChangeChildMaterials)
			childs = gameObject.GetComponentsInChildren.<Renderer>() as Renderer[];
		else
			childs = gameObject.GetComponents.<Renderer>() as Renderer[];
		
		Renderers = new RendererData[childs.length];
		for (var i:int = 0; i < childs.length; i++)
			Renderers[i] = RendererData(childs[i], MaterialOn);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCSetEnabled(status:boolean) {
		enabled = status;
	}
	
	
	@RPC
	function RPCSetStatus(status:boolean) {
		Status = status;
		StatusLast = Status;
		
		for (var rend:RendererData in Renderers)
			rend.SetMaterials(status);
	}
	// =============================================================================
}