#pragma strict

// Hannes Helmholz
//
// extension of TriggerAbstract as a button to trigger actions
//
// shows description as HoveringText (name, key and description onyl when set to a value)


private class TriggerButton extends TriggerAbstract {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var HideIfInactive:boolean = false;
	@SerializeField	private var AnimationClips:AnimationClip[];
	
	private var Ani:Animation;
	// ============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function OnEnable() {
		if (HideIfInactive) {
			Coll.enabled = true;
			
			for (var rend:RendererData in Renderers)
				rend.SetEnabled(true);
		}
	}
	
	
	function OnDisable() {
		if (HideIfInactive) {
			Coll.enabled = false;
			
			for (var rend:RendererData in Renderers)
				rend.SetEnabled(false);
		}
		
		super.OnDisable();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
		
	// have to be called on init
	public function SetAnimation(animation:Animation) {
		Ani = animation;
	}
	
	
	public function SetAsStartingPoint() {
		if (!AnimationClip || !Ani)
			return;
		
		Ani.Play(AnimationClips[0].name);
		Ani.Sample();
		Ani.Stop();
	}
	
	
	protected function DoTrigger() {
		if (!AnimationClips || !Ani)
			return;
			
		super.DoTrigger();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCDoTrigger() {
		for (var clip:AnimationClip in AnimationClips)
			Ani.PlayQueued(clip.name);
	}
	// =============================================================================
}