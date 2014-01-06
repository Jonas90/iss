#pragma strict

// Hannes Helmholz
//
// 


private class NearPlane extends System.ValueType {
	public var left:float;
	public var right:float;
	public var top:float;
	public var bottom:float;
	public var near:float;
	public var far:float;
}


private static class ClientCameraVanishingPoint extends MonoBehaviour {
	
	// =============================================================================
	// METHODS STATIC --------------------------------------------------------------
	
	public function SetVanishingPoint(cam:Camera, offset:float, screen:ClientCameraScreen) {
	 	var t:Transform = cam.transform;
	 	var plane:NearPlane = NearPlane();
	 	
	 	var nearCenter:Vector3 = t.position + t.forward * cam.nearClipPlane;
	 	var nearPlane:Plane = Plane(-t.forward, nearCenter);
	 	var distance:float;
	 	var direction:Vector3;
	 	var ray:Ray;
	 	
	 	var screenTL:Vector3 = t.TransformPoint(Vector3((-screen.Width / 2.0) + offset,  screen.Height / 2.0, screen.Distance));
	 	direction = (screenTL - t.position).normalized;
	 	ray = Ray(t.position, direction);
	 	nearPlane.Raycast(ray, distance);
	 	var nearTL:Vector3 = -(t.InverseTransformPoint(nearCenter) - t.InverseTransformPoint((t.position + direction * distance)));
	 	
	 	var screenBR:Vector3 = t.TransformPoint(Vector3(( screen.Width / 2.0) + offset, -screen.Height / 2.0, screen.Distance));
	 	direction = (screenBR - t.position).normalized;
	 	ray = Ray(t.position, direction);
	 	nearPlane.Raycast(ray, distance);
	 	var nearBR:Vector3 = -(t.InverseTransformPoint(nearCenter) - t.InverseTransformPoint((t.position + direction * distance)));
	 	
	 	plane.left = nearTL.x;
		plane.top = nearTL.y;
	 	plane.right = nearBR.x;
		plane.bottom = nearBR.y;
	 	plane.near = cam.nearClipPlane;
		plane.far = cam.farClipPlane;
		cam.projectionMatrix = PerspectiveOffCenter(plane);
	}
	
	
	private function PerspectiveOffCenter(plane:NearPlane):Matrix4x4 {
		var x:float =  (2.0 * plane.near)			  / (plane.right - plane.left);
		var y:float =  (2.0 * plane.near)			  / (plane.top - plane.bottom);
		var a:float =  (plane.right + plane.left)	  / (plane.right - plane.left);
		var b:float =  (plane.top + plane.bottom)	  / (plane.top - plane.bottom);
		var c:float = -(plane.far + plane.near)		  / (plane.far - plane.near);
		var d:float = -(2.0 * plane.far * plane.near) / (plane.far - plane.near);
		var e:float = -1.0;
		var m:Matrix4x4 = Matrix4x4();
		
		m[0,0] =   x;  m[0,1] = 0.0;  m[0,2] = a;   m[0,3] = 0.0;
		m[1,0] = 0.0;  m[1,1] =   y;  m[1,2] = b;   m[1,3] = 0.0;
		m[2,0] = 0.0;  m[2,1] = 0.0;  m[2,2] = c;   m[2,3] =   d;
		m[3,0] = 0.0;  m[3,1] = 0.0;  m[3,2] = e;   m[3,3] = 0.0;
		
		return m;
	}
	// =============================================================================
}