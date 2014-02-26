using UnityEngine;
using System.Collections;


//hilfsklasse fuer berechnungen mit vektoren
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
