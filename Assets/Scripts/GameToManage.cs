
// Hannes Helmholz
//
// abstract class for all games managed by GameController

using UnityEngine;

[RequireComponent (typeof (NetworkView))] // for RPC

public /*abstract*/ class GameToManage : MonoBehaviour {
	
	public void Play() {}
	public void Stop() {}
	public void Reset() {}
}