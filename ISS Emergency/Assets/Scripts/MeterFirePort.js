#pragma strict

// Hannes Helmholz
//
// 


private class MeterFirePort extends MeterAbstract {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var MeasureAnimationLength:float = 6;
	
	private var MeasurementScript:MeasurementFirePort;
	private var Nose:LayerSetterElement;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		super.Awake();
		MeasurementScript = GameObject.FindObjectOfType(MeasurementFirePort);
		
		Nose = gameObject.GetComponentInChildren.<LayerSetterElement>();
		
		Audio.clip = SoundSuccess;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------

	public function MeasureValues(name:String, port:Transform) {
		if (!IsAvailable())
			return;
		
		var measureAnimationPartLength:float = MeasureAnimationLength / 4.0;
		
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
		
		yield WaitForSeconds(1);
		var values:MeasurementValues = MeasurementScript.GetValues(name);
		var text:String = values ? values.ToString() : "ERR";
		ConfigClass.Log("PORT port: " + name + " values " + text);
		DisplayValues(values);
		
		yield super.MeasureValues(0.5); // play sound
		
		HOTween.PlayBackwards(tweener2);
		yield WaitForSeconds(measureAnimationPartLength);
		
		LayerSetter.SetAllLayerRecursively(Nose.gameObject, Nose.OldLayer);
		
		HOTween.PlayBackwards(tweener1);
		yield WaitForSeconds(measureAnimationPartLength);
		
		HOTween.Kill(tweener1);
		HOTween.Kill(tweener2);
		SetPlayerMotion(true);
		yield WaitForSeconds(2.5);
		
		DisplayValues("", "");
	}
	// =============================================================================
}