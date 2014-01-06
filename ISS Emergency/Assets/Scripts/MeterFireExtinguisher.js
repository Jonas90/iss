#pragma strict

// Hannes Helmholz
//
// 


private class MeterFireExtinguisher extends MeterAbstract {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var ExtinguishAnimationLength:float = 6;

	private var Nose:LayerSetterElement;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		super.Awake();
		
		Nose = gameObject.GetComponentInChildren.<LayerSetterElement>();
		
		Audio.clip = SoundSuccess;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------

	public function Extinguish(port:Transform) {
		if (!IsAvailable())
			return;
		
		var measureAnimationPartLength:float = ExtinguishAnimationLength / 4.0;
		
		SetPlayerMotion(false);
		
		var parms1:TweenParms = TweenParms();
		parms1.Prop("position", Plugins.Core.PlugVector3(port.position));
		parms1.Prop("rotation", Plugins.PlugQuaternion(port.rotation));
		//parms1.Prop("localScale", Plugins.Core.PlugVector3(port.localScale));
		parms1.Ease(EaseType.EaseInOutSine);
		var tweener1:Tweener = HOTween.To(Trans, measureAnimationPartLength, parms1);
		tweener1.autoKillOnComplete = false;
		yield WaitForSeconds(measureAnimationPartLength);
		
		LayerSetter.SetAllLayerRecursively(Nose.gameObject, Nose.OtherLayer);
		
		var parms2:TweenParms = TweenParms();
		parms2.Prop("position", Plugins.Core.PlugVector3(Trans.forward * 0.18, true));
		parms2.Ease(EaseType.EaseInOutSine);
		var tweener2:Tweener = HOTween.To(Trans, measureAnimationPartLength, parms2);
		tweener2.autoKillOnComplete = false;
		yield WaitForSeconds(measureAnimationPartLength);
		
		yield super.MeasureValues(3); // play sound
		yield WaitForSeconds(0.5);
		
		HOTween.PlayBackwards(tweener2);
		yield WaitForSeconds(measureAnimationPartLength);
		
		LayerSetter.SetAllLayerRecursively(Nose.gameObject, Nose.OldLayer);
		
		HOTween.PlayBackwards(tweener1);
		yield WaitForSeconds(measureAnimationPartLength);
		
		HOTween.Kill(tweener1);
		HOTween.Kill(tweener2);
		SetPlayerMotion(true);
	}
	// =============================================================================
}