/*
 * TeamSpeak 3 client minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using anyID = System.UInt16;
using uint64 = System.UInt64;

namespace ts3client_minimal_sample
{
	class ts3client
	{
		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_initClientLib")]
		public static extern uint initClientLib(ref client_callback_struct arg0, ref client_callbackrare_struct arg1, LogTypes arg2, string arg3, string arg4);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_getClientLibVersion")]
		public static extern uint getClientLibVersion(out IntPtr arg0);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_freeMemory")]
		public static extern uint freeMemory(IntPtr arg0);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_spawnNewServerConnectionHandler")]
		public static extern uint spawnNewServerConnectionHandler(int port, out uint64 arg0);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_openCaptureDevice")]
		public static extern uint openCaptureDevice(uint64 arg0, string arg1, string arg2);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_openPlaybackDevice")]
		public static extern uint openPlaybackDevice(uint64 arg0, string arg1, string arg2);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_createIdentity")]
		public static extern uint createIdentity(out IntPtr arg0);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_startConnection", CharSet = CharSet.Ansi)]
		public static extern uint startConnection(uint64 arg0, string identity, string ip, uint port, string nick, ref string defaultchannelarray, string defaultchannelpassword, string serverpassword);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_stopConnection")]
		public static extern uint stopConnection(uint64 arg0, string arg1);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_destroyServerConnectionHandler")]
		public static extern uint destroyServerConnectionHandler(uint64 arg0);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_destroyClientLib")]
		public static extern uint destroyClientLib();

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_getChannelVariableAsString")]
		public static extern uint getChannelVariableAsString(uint64 arg0, uint64 arg1, ChannelProperties arg2, out IntPtr arg3);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_getErrorMessage")]
		public static extern uint getErrorMessage(uint arg0, IntPtr arg1);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_getClientVariableAsString")]
		public static extern uint getClientVariableAsString(uint64 arg0, anyID arg1, ClientProperties arg2, out IntPtr arg3);

		[DllImport("ts3client_win32.dll", EntryPoint = "ts3client_requestMuteClients")]
		public static extern uint requestMuteClients(uint64 arg0, anyID[] arg1, out IntPtr arg3);
	}

	class TSClient
	{

		private string ip;
		private uint port;
		private string password;
		private uint64 scHandlerID = 0;
		private bool connected;

		string[] muted;

		public TSClient(string ip, int port, string password, string[] muted)
		{
			this.ip = ip;
			this.port = (uint)port;
			this.password = password;

			this.connected = false;

			this.muted = new string[muted.Length];
			Array.Copy(muted, this.muted, muted.Length);
		}

		public void start()
		{
			/* Create struct for callback function pointers */
			client_callback_struct cbs = new client_callback_struct();
			client_callbackrare_struct cbs_rare = new client_callbackrare_struct(); // dummy

			cbs.onConnectStatusChangeEvent_delegate = new onConnectStatusChangeEvent_type(callback.onConnectStatusChangeEvent);
			cbs.onServerProtocolVersionEvent_delegate = new onServerProtocolVersionEvent_type(callback.onServerProtocolVersionEvent);
			cbs.onNewChannelEvent_delegate = new onNewChannelEvent_type(callback.onNewChannelEvent);
			cbs.onNewChannelCreatedEvent_delegate = new onNewChannelCreatedEvent_type(callback.onNewChannelCreatedEvent);
			cbs.onDelChannelEvent_delegate = new onDelChannelEvent_type(callback.onDelChannelEvent);
			cbs.onClientMoveEvent_delegate = new onClientMoveEvent_type(callback.onClientMoveEvent);
			cbs.onClientMoveSubscriptionEvent_delegate = new onClientMoveSubscriptionEvent_type(callback.onClientMoveSubscriptionEvent);
			cbs.onClientMoveTimeoutEvent_delegate = new onClientMoveTimeoutEvent_type(callback.onClientMoveTimeoutEvent);
			cbs.onTalkStatusChangeEvent_delegate = new onTalkStatusChangeEvent_type(callback.onTalkStatusChangeEvent);
			cbs.onIgnoredWhisperEvent_delegate = new onIgnoredWhisperEvent_type(callback.onIgnoredWhisperEvent);
			cbs.onServerErrorEvent_delegate = new onServerErrorEvent_type(callback.onServerErrorEvent);
			cbs.onUserLoggingMessageEvent_delegate = new onUserLoggingMessageEvent_type(callback.onUserLoggingMessageEvent);

			callback.mutedClients = muted;

			/* Initialize client lib with callbacks */
			/* Resource path points to the SDK\bin directory to locate the soundbackends folder when running from Visual Studio. */
			/* If you want to run directly from the SDK\bin directory, use an empty string instead to locate the soundbackends folder in the current directory. */
			uint error = ts3client.initClientLib(ref cbs, ref cbs_rare, LogTypes.LogType_USERLOGGING, null, "");
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Failed to init clientlib: {0}.", error);
				return;
			}

			/* Spawn a new server connection handler using the default port and store the server ID */
			error = ts3client.spawnNewServerConnectionHandler(0, out scHandlerID);
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Error spawning server connection handler: {0}", error);
				return;
			}

			/* Open default capture device */
			/* Passing empty string for mode and null or empty string for device will open the default device */
			error = ts3client.openCaptureDevice(scHandlerID, "", null);
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Error opening capture device: {0}", error);
			}

			/* Open default playback device */
			/* Passing empty string for mode and NULL or empty string for device will open the default device */
			error = ts3client.openPlaybackDevice(scHandlerID, "", null);
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Error opening playback device: {0}", error);
			}

			/* Create a new client identity */
			/* In your real application you should do this only once, store the assigned identity locally and then reuse it. */
			IntPtr identityPtr = IntPtr.Zero;
			error = ts3client.createIdentity(out identityPtr);
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Error creating identity: {0}", error);
				return;
			}
			string identity = Marshal.PtrToStringAnsi(identityPtr);

			string nickname = System.Environment.MachineName; //"labor";

			string defaultarray = "";
			/* Connect to server on localhost:9987 with nickname "client", no default channel, no default channel password and server password "secret" */
			error = ts3client.startConnection(scHandlerID, identity, this.ip, this.port, nickname, ref defaultarray, "", this.password);
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Error connecting to server: 0x{0:X4}", error);
				Console.ReadLine();
				return;
			}
			ts3client.freeMemory(identityPtr);  /* Release dynamically allocated memory */

			this.connected = true;

			Console.WriteLine("Client lib initialized and running");

			/* Query and print client lib version */
			IntPtr versionPtr = IntPtr.Zero;
			error = ts3client.getClientLibVersion(out versionPtr);
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Failed to get clientlib version: {0}.", error);
				return;
			}
			string version = Marshal.PtrToStringAnsi(versionPtr);
			Console.WriteLine(version);
			ts3client.freeMemory(versionPtr); /* Release dynamically allocated memory */
		}

		public void stop()
		{

			if (!this.connected)
				return;

			/* Disconnect from server */
			uint error = ts3client.stopConnection(scHandlerID, "leaving");
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Error stopping connection: {0}", error);
				return;
			}

			Thread.Sleep(200);

			/* Destroy server connection handler */
			error = ts3client.destroyServerConnectionHandler(scHandlerID);
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Error destroying clientlib: {0}", error);
				return;
			}

			/* Shutdown client lib */
			error = ts3client.destroyClientLib();
			if (error != public_errors.ERROR_ok)
			{
				Console.WriteLine("Failed to destroy clientlib: {0}", error);
				return;
			}
		}

		public void muteClient(string client)
		{
			string name;
			IntPtr namePtr = IntPtr.Zero;

			anyID clientID = 0;

			/* Query client nickname from ID */
			uint error = ts3client.getClientVariableAsString(scHandlerID, clientID, ClientProperties.CLIENT_NICKNAME, out namePtr);
			if (error == public_errors.ERROR_ok)
			{
				name = Marshal.PtrToStringAnsi(namePtr);
				Console.WriteLine("New client: {0}", name);
				ts3client.freeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
			}
		}
	}
}
