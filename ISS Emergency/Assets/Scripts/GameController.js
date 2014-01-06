#pragma strict

// Hannes Helmholz
//
// start script of application
// manage build all network connections
// after game started script only checks for application exit by server


@script RequireComponent(NetworkView) // for RPC

private class GameController extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public enum GameState {WaitForClientsSERVERONLY, WaitForFirstStart, WaitForStart, Started}
	
	@SerializeField	private var ApplicationStarter:ApplicationStarter;
	@SerializeField	private var Player:GameObject;
	@SerializeField	private var Games:GameToManage[];
	
	private var Config:ConfigClass;
	private var NetView:NetworkView;
	private var NavTrans:PlayerMovement;
	private var NavRot:PlayerRotation;
	
	private static var State:GameState;  
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	// start reading data from config file by GameStarter
	function Awake() {
		Config.Log("====================");
		Config.Log("awake");
		
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		NetView = networkView;
		
		Config.Log("====================");
		ApplicationStarter.GenerateIpData();
		Config.OwnClientDataToString();
	}
	
	
	// start application as server or client
	// setup screen and camera settings
	function Start() {
		NavTrans = Player.GetComponent.<PlayerMovement>();
		NavRot = Player.GetComponent.<PlayerRotation>();
		NavTrans.enabled = false;
		NavRot.enabled = false;
		
		Config.Log("====================");
		ApplicationStarter.SetQualityLevel();
		
		Config.Log("====================");
		StartCoroutine(ApplicationStarter.SetScreenResolution());
		
		StartApplication();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY NETWORK -------------------------------------------------------
	
	function OnServerInitialized(){
		if (!Config.IsStandalone())
			Config.Log("waiting for clients");
		State = GameState.WaitForClientsSERVERONLY;
		
		Config.ConnectedClientNumber++;
		CheckStartGame();
	}
	
	
	function OnConnectedToServer() {
		Config.Log("waiting for start");
		State = GameState.WaitForFirstStart;
		
		ConfigClass.Log("====================");
		ApplicationStarter.DestroyAllPhysics();
		ConfigClass.Log("====================");
		ApplicationStarter.DestroyAllAudio();
		ConfigClass.Log("====================");
	}
	
	
	// try reconnects or run as standalone
	function OnFailedToConnect(error:NetworkConnectionError) {
		Config.Log("ConnectionError: " + error, true);
		
		Config.ConnectionRetries--;
		if (Config.ConnectionRetries >= 0)
			Network.Connect(Config.ServerIp, Config.ServerPort);
		else {
			// run application as standalone, so as server without clients
			ApplicationStarter.ForceStandaloneServer();
			Config.OwnClientDataToString();
			StartApplication();
		}
	}
	
	
	function OnPlayerConnected(player:NetworkPlayer){
		Config.Log("player connected: " + player.ToString());
	
		Config.ConnectedClientNumber++;
		CheckStartGame();
	}
	
	
	function OnPlayerDisconnected(player:NetworkPlayer){
		Config.ConnectedClientNumber--;
	
		Network.RemoveRPCs(player);
	  	Network.DestroyPlayerObjects(player);
	}	
	// =============================================================================


	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	private function StartApplication() {
		Config.Log("====================");
		ClientCameraController.SetAllTransforms(Player);
		
		Config.Log("====================");
		Screen.lockCursor = false;
		
		if (Config.IsServer)
			Server();
		else
			Client();
	}
	
	
	private function Server() {
		Config.Log("server started");
		Config.Log("====================");
		
		Screen.showCursor = true;
		
		Network.InitializeServer(Config.ClientData.Count, Config.ServerPort, false);
	}
	
	
	private function Client() {
		Config.Log("waiting for server " + Config.ServerIp);
		Config.Log("====================");
		
		Screen.showCursor = false;
		
		Network.Connect(Config.ServerIp, Config.ServerPort);
	}
	
	
	private function CheckStartGame() {
		if (Config.ConnectedClientNumber == Config.ClientData.Count) {
			State = GameState.WaitForFirstStart;
		}
	}
	
	
	public function GetState():GameState {
		return State;
	}
	
	
	public function RestartGame() {
		StopGame();
		StartGame();
		
		for (var game:GameToManage in Games)
			game.Reset();
	}
	
	
	public function StartGame() {
		NavTrans.enabled = true;
		NavRot.enabled = true;
			
		State = GameState.Started;
		for (var game:GameToManage in Games)
			game.Play();
			
		NetView.RPC("RPCGameStart", RPCMode.AllBuffered);
	}
	
	
	public function StopGame() {
		Input.ResetInputAxes();
		NavTrans.enabled = false;
		NavRot.enabled = false;
	
		State = GameState.WaitForStart;
		for (var game:GameToManage in Games)
			game.Stop();
		
		NetView.RPC("RPCGameStop", RPCMode.AllBuffered);
	}
	
	
	public function EndGame() {
		NetView.RPC("RPCEndGame", RPCMode.AllBuffered);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCGameStart() {
		Config.Log("game started");
		
		Config.IsGameStarted = true;
		State = GameState.Started;
	}
	
	
	@RPC
	function RPCGameStop() {
		Config.Log("game stoped");
		
		Config.IsGameStarted = false;
		State = GameState.WaitForStart;
	}
	
	
	@RPC
	function RPCEndGame() {
		Application.Quit();
	}
	// =============================================================================
}