using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vrmeeting.net;
using System;
using UnityEngine.UI;
using vrmeeting.protocol;
using vrmeeting.protocol.signals;

public class LeaveScript : MonoBehaviour {

	private bool initialised = false;
	private ClientSocket socket;

	private GameObject warningText;

	// Use this for initialization
	void Start () {

	}

	void Awake(){
		warningText = GameObject.FindGameObjectWithTag ("WarningText");
		warningText.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (!initialised) {
			try {
				GameObject camera = GameObject.FindGameObjectsWithTag ("MainCamera") [0];
				TestSocket socketScript = camera.GetComponent (typeof(TestSocket)) as TestSocket;
				socket = socketScript.getSocket ();
				initialised = true;
				Debug.Log("Successfully Initialised LeaveButton");
			} catch (NullReferenceException ex) {

			}
		}
	}

	public void BeginLeave()
	{
		if (initialised) {
			Invoke ("LeaveMeeting", Configuration.Config.DELAY_BEFORE_LEAVE);
			warningText.SetActive (true);
		}
	}

	public void CancelLeave()
	{
		CancelInvoke ("LeaveMeeting");
		warningText.SetActive (false);
	}

	public void LeaveMeeting()
	{
		Debug.Log ("Left meeting");
		socket.sendMessage (new Message (ClientSignals.GONE.ToString(), null));
		Application.Quit ();
	}


}
