using System.Collections;
using vrmeeting.net;
using UnityEngine;
using System;
using vrmeeting.protocol.signals;
using vrmeeting.protocol;

public class TestSocket : MonoBehaviour {
	private ClientSocket mSocket;
	// Use this for initialization
	public SplashScreen splash;
	private int countdown;
	private string errorMessage;

	void Start () {

		GameObject pBoard = GameObject.FindGameObjectsWithTag("pBoard")[0];
		ReadFile readFile = pBoard.GetComponent(typeof(ReadFile)) as ReadFile;

		this.mSocket = new ClientSocket();  
		mSocket.ConnectServer(readFile.GetHostServerIdentifier(), readFile.GetHostServerPort());  
		InvokeRepeating ("SendHeartbeat", 0, Configuration.Config.HEARTBEAT_FREQUENCY);
	}

	private void SendHeartbeat()
	{
		mSocket.sendMessage (new Message (ClientSignals.HRTB.ToString (), null));
	}

	private bool shutdownTriggered = false;
	// Update is called once per frame
	void Update () {
		if (!shutdownTriggered) {
			if (mSocket.hasEncounteredError ()) {
				shutdownTriggered = true;
				errorMessage = mSocket.getError ();
				mSocket.Stop ();
				countdown = 10;
				InvokeRepeating ("StopSequence", 0, 1);
			}
		}

	}

	private void StopSequence()
	{
		if (countdown >= 0) {
			splash.DisplaySplashScreen (errorMessage + "\nReturning to Main Menu in:\n " + countdown);
			countdown--;
		} else {
			splash.HideSplashScreen ();
			Application.Quit ();
		}
	}

	void OnApplicationQuit(){
		mSocket.Stop ();
		CancelInvoke ("SendHeartbeat");
	}

    public ClientSocket getSocket()
    {
        return this.mSocket;
    }
}
