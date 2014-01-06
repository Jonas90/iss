#pragma strict


// Hannes Helmholz
//
// 


@script RequireComponent(NetworkView) // for RPC
@script RequireComponent(UniqueIDManager)

private class LayerSetter extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	private static var NetView:NetworkView;
	
	private var IDManager:UniqueIDManager;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		NetView = networkView;
		
		IDManager = gameObject.GetComponent.<UniqueIDManager>();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS STATIC --------------------------------------------------------------
	
	public static function SetAllLayerRecursively(go:GameObject, newLayer:int) {
		SetLayer(go, newLayer);
		
		NetView.RPC("RPCSetAllLayerRecursively", RPCMode.OthersBuffered, go.GetComponent.<UniqueID>().ID, newLayer);
	}
	
	
	private static function SetLayer(go:GameObject, newLayer:int) {
	    if (!go)
	    	return;
	
	    go.layer = newLayer;
	    
	    for (var child:Transform in go.transform) {
	        if (!child)
	        	continue;
	        	
	        SetLayer(child.gameObject, newLayer);
	    }
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCSetAllLayerRecursively(id:int, newLayer:int) {
		SetLayer(IDManager.GetGameObjectByID(id), newLayer);
	}
	// =============================================================================
}