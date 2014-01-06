#pragma strict

// Hannes Helmholz
//
// 

import Holoville.HOTween;

@script RequireComponent(IHOTweenComponent)


private class Equipment extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	protected var AvailableAtStart:boolean = false;
	
	protected var Config:ConfigClass;
	protected var TriggerManual:boolean = false;
	protected var Available:boolean;
	protected var TweenTargets:Object[];

	private var Trans:Transform;
	private var Stored:boolean = true;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		Trans = transform;
		
		Available = AvailableAtStart;
		
		// there is some realy black magic going on in here
		// can't explain that to anyone
		// just to find out the tween target of this GameObject
		TweenTargets = new Object[0];
		for (var tweenInfo:Core.TweenInfo in HOTween.GetTweenInfos()) {
			var object:Object = tweenInfo.targets[0] as Object;
			for (var component:Component in gameObject.GetComponents.<Component>())
				if (component.Equals(object)) {
					var oldSize:int = TweenTargets.length;
					System.Array.Resize.<Object>(TweenTargets, oldSize + 1);
					TweenTargets[oldSize] = object; // all for this stupid line of code
				}
		}
		
		for (var tweenTarget:Object in TweenTargets)
			HOTween.Complete(tweenTarget); // set to stored position
	}
	
	
	function Update() {
		if (!Config.IsServer)
			return;
			
		if (Available && TriggerManual) {
			Trigger();
			TriggerManual = false;
		}
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	protected function Trigger() {
		if (Stored)
			for (var tweenTarget:Object in TweenTargets)
				HOTween.PlayBackwards(tweenTarget);
		else
			for (var tweenTarget:Object in TweenTargets)
				HOTween.PlayForward(tweenTarget);	
		
		Stored = !Stored;
	}
	
	
	public function SetAvailable(available:boolean) {
		Available = available;
	}
	
	
	public function TriggerStored() {
		if (!Available)
			return;
		
		TriggerManual = true;
	}
	
	
	public function IsAvailable():boolean {
		return Available;
	}
	
	
	public function IsEquipped():boolean {	
		return Available && !Stored;
	}
	// =============================================================================
}