#pragma strict

// Hannes Helmholz
//
// abstract class for all games managed by GameController


@script RequireComponent(NetworkView) // for RPC

private /*abstract*/ class GameToManage extends MonoBehaviour {
	
	public function Play() {}
	public function Stop() {}
	public function Reset() {}
}