#pragma strict

// Hannes Helmholz
//
// class for animated objects with one open and one close animation


@script RequireComponent(NetworkView) // for RPC

public class TriggerHatch extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public enum TriggerType {Trigger, InstantOpen, PermanentOpen, PermanentClosed}
	public enum TriggerObject {DistanceByCollider, Script}
	
	public var Type:TriggerType;
	public var Trigger:TriggerObject;
	public var AnimationsOpenSize:int = 1;
	public var AnimationsCloseSize:int = 1;
	public var AnimationsOpen:AnimationClip[] = new AnimationClip[AnimationsOpenSize];
	public var AnimationsClose:AnimationClip[] = new AnimationClip[AnimationsCloseSize];
	public var Script:TriggerButtonSequence;
	
	private var Config:ConfigClass;
	private var Ani:Animation;
	private var NetView:NetworkView;
	private var IsInit:boolean = false;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		Ani = animation;
		NetView = networkView;
		
		if (!Config.IsServer) {
			IsInit = true;
			return;
		}
		
		if (Type == TriggerType.Trigger)
			IsInit = true;
			
		if (Trigger == TriggerObject.Script)
			Script = transform.GetComponent.<TriggerButtonSequence>();
	}
	
	
	function Update() {	
		if (!Config.IsServer)
			return;
			
		if (!IsInit && Config.IsGameStarted)
			MakePermanent();
	}
	
	
	function OnTriggerEnter(other:Collider) {
		if (Trigger != TriggerObject.DistanceByCollider || !Config.IsServer || !other.gameObject.tag.Equals("Player"))
			return;	
				
		Open();
	}
	
	
	function OnTriggerExit(other:Collider) {
		if (Trigger != TriggerObject.DistanceByCollider || !Config.IsServer || !other.gameObject.tag.Equals("Player"))
			return;
			
		Close();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	private function MakePermanent() {
		IsInit = true;
		
		if (Type == TriggerType.PermanentOpen)
			Open();
		else if (Type == TriggerType.InstantOpen)
			NetView.RPC("RPCAnimateInstant", RPCMode.AllBuffered, true);
	}
	
	
	private function Open() {
		NetView.RPC("RPCAnimate", RPCMode.AllBuffered, true);
	}
	
	
	private function Close() {
		NetView.RPC("RPCAnimate", RPCMode.AllBuffered, false);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCAnimate(open:boolean) {
		if (open)
			for (var clip:AnimationClip in AnimationsOpen)
				Ani.PlayQueued(clip.name);
		else
			for (var clip:AnimationClip in AnimationsClose)
				Ani.PlayQueued(clip.name);
	}
	
	
	@RPC
	function RPCAnimateInstant(open:boolean) {
		if (open)
			Ani.Play(AnimationsOpen[AnimationsOpen.Length - 1].name);
		else
			Ani.Play(AnimationsClose[AnimationsClose.Length - 1].name);
	}
	// =============================================================================
}