using UnityEngine;
using System.Collections;

public class IntVector2 : MonoBehaviour {

	public int x, y;
	
	public IntVector2() {
		x = y = 0;
	}
	
	public IntVector2(int x, int y) {
		this.x = x;
		this.y = y;
	}
	
}
