
// Hannes Helmholz
//
// start script of application
// manage build all network connections
// after game started script only checks for application exit by server

using UnityEngine;

[RequireComponent (typeof (NetworkView))] // for RPC

public class GameController : MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public enum GameState {WaitForClientsSERVERONLY, WaitForFirstStart, WaitForStart, Started}
	
	[SerializeField]	private ApplicationStarter ApplicationStarter;
	[SerializeField]	private GameObject Player;
	[SerializeField]	private GameToManage[] Games;
	
	private ConfigClass Config;
	private NetworkView NetView;
	private PlayerMovement NavTrans;
	private PlayerRotation NavRot;
	
	private static GameState State;  
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	// start reading data from config file by GameStarter
	void Awake() {
		ConfigClass.Log("====================");
		ConfigClass.Log("awake");
		
		Config = GameObject.FindWithTag("Config").GetComponent<ConfigClass>();
		NetView = networkView;
		
		ConfigClass.Log("====================");
		ApplicationStarter.GenerateIpData();
		Config.OwnClientDataToString();
	}
	
	
	// start application as server or client
	// setup screen and camera settings
	void Start() 
	{
		NavTrans = Player.GetComponent<PlayerMovement>();
		NavRot = Player.GetComponent<PlayerRotation>();
		NavTrans.enabled = false;
		NavRot.enabled = false;
		
		ConfigClass.Log("====================");
		ApplicationStarter.SetQualityLevel();
		
		ConfigClass.Log("====================");
		StartCoroutine(ApplicationStarter.SetScreenResolution());
		
		StartApplication();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY NETWORK -------------------------------------------------------
	
	void OnServerInitialized(){
		if (!Config.IsStandalone())
			ConfigClass.Log("waiting for clients");
		State = GameState.WaitForClientsSERVERONLY;
		
		Config.ConnectedClientNumber++;
		CheckStartGame();
	}
	
	
	void OnConnectedToServer() {
		ConfigClass.Log("waiting for start");
		State = GameState.WaitForFirstStart;
		
		ConfigClass.Log("====================");
		ApplicationStarter.DestroyAllPhysics();
		ConfigClass.Log("====================");
		ApplicationStarter.DestroyAllAudio();
		ConfigClass.Log("====================");
	}
	
	
	// try reconnects or run as standalone
	void OnFailedToConnect(NetworkConnectionError error) {
		ConfigClass.Log("ConnectionError: " + error, true);
		
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
	
	
	void OnPlayerConnected(NetworkPlayer player){
		ConfigClass.Log("player connected: " + player.ToString());
	
		Config.ConnectedClientNumber++;
		CheckStartGame();
	}
	
	
	void OnPlayerDisconnected(NetworkPlayer player){
		Config.ConnectedClientNumber--;
	
		Network.RemoveRPCs(player);
	  	Network.DestroyPlayerObjects(player);
	}	
	// =============================================================================


	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	private void StartApplication() {
		ConfigClass.Log("====================");
		ClientCameraController.SetAllTransforms(Player);
		
		ConfigClass.Log("====================");
		Screen.lockCursor = false;
		
		if (Config.IsServer)
			Server();
		else
			Client();
	}
	
	
	private void Server() {
		ConfigClass.Log("server started");
		ConfigClass.Log("====================");
		
		Screen.showCursor = true;
		
		Network.InitializeServer(Config.ClientData.Count, Config.ServerPort, false);
	}
	
	
	private void Client() {
		ConfigClass.Log("waiting for server " + Config.ServerIp);
		ConfigClass.Log("====================");
		
		Screen.showCursor = false;
		
		Network.Connect(Config.ServerIp, Config.ServerPort);
	}
	
	
	private void CheckStartGame() {
		if (Config.ConnectedClientNumber == Config.ClientData.Count) {
			State = GameState.WaitForFirstStart;
		}
	}
	
	
	public GameState GetState() {
		return State;
	}
	
	
	public void RestartGame() {
		StopGame();
		StartGame();
		
		foreach (GameToManage game in Games)
			game.Reset();
	}
	
	
	public void StartGame() {
		NavTrans.enabled = true;
		NavRot.enabled = true;
			
		State = GameState.Started;
		foreach (GameToManage game in Games)
			game.Play();
			
		NetView.RPC("RPCGameStart", RPCMode.AllBuffered);
	}
	
	
	public void StopGame() {
		Input.ResetInputAxes();
		NavTrans.enabled = false;
		NavRot.enabled = false;
	
		State = GameState.WaitForStart;
		foreach (GameToManage game in Games)
			game.Stop();
		
		NetView.RPC("RPCGameStop", RPCMode.AllBuffered);
	}
	
	
	public void EndGame() {
		NetView.RPC("RPCEndGame", RPCMode.AllBuffered);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	[RPC]
	void RPCGameStart() {
		ConfigClass.Log("game started");
		
		Config.IsGameStarted = true;
		State = GameState.Started;
	}
	
	
	[RPC]
	void RPCGameStop() {
		ConfigClass.Log("game stoped");
		
		Config.IsGameStarted = false;
		State = GameState.WaitForStart;
	}
	
	
	[RPC]
	void RPCEndGame() {
		Application.Quit();
	}
	// =============================================================================
}