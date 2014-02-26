//6. Etage

//dan
//Erstellt das Menü im Standalone Modus mitdem man sich zur CAVE verbinden kann,
//bzw einen lokalen Server startet

using UnityEngine;
using System.Collections;

public class Client_Menu : MonoBehaviour
{
	void OnGUI()
	{
		// Wenn alleine Menu anzeigen
		if (Config.Instance.IsStandalone)
		{
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				if (GUI.Button(new Rect(100, 100, 150, 25), "Verbinde in die CAVE"))
				{
					Logger.Log("Client von Hand gestartet.");
					GameController.Instance.startClient(Config.Instance.caveGameServerAddress, Config.Instance.caveGameServerPort); //als Client weiter
					GameController.Instance.startTeamspeak(System.Environment.MachineName);
				}

				if (GUI.Button(new Rect(100, 125, 150, 25), "Verbinde auf Localhost"))
				{
					Logger.Log("Client von Hand gestartet.");
					GameController.Instance.startClient("127.0.0.1", Config.Instance.caveGameServerPort); //als Client weiter
					GameController.Instance.startTeamspeak(System.Environment.MachineName);
				}

				if (GUI.Button(new Rect(100, 150, 150, 25), "Lokalen Server starten"))
				{
					Logger.Log("Server von Hand gestartet.");
					GameController.Instance.startServer();
				}
			}
			else
			{
				if (Network.peerType == NetworkPeerType.Client)
				{

					GUI.Label(new Rect(100, 100, 100, 25), "Client");
					if (GUI.Button(new Rect(100, 125, 100, 25), "log out"))
					{
						Network.Disconnect(250);

					}
				}
				if (Network.peerType == NetworkPeerType.Server)
				{

					GUI.Label(new Rect(100, 100, 100, 25), "Server");
					GUI.Label(new Rect(100, 125, 100, 25), "Verbindugen: " + Network.connections.Length);
					if (GUI.Button(new Rect(100, 150, 100, 25), "log out"))
					{
						Network.Disconnect(250);
					}
				}
			}
		}
	}
}
