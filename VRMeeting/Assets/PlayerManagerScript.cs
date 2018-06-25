using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using vrmeeting.protocol;
using vrmeeting.net;
using System.Threading;
using vrmeeting.protocol.signals;

public class PlayerManagerScript : MonoBehaviour {

	private MessageOutlet outlet;
	private bool initialised = false;
	private ClientSocket socket;
	private Mutex playerListLock = new Mutex();
	private IDictionary<int,User> users = new Dictionary<int,User> ();
	private MeetingInfoDisplay meetingDisplay;

	public GameObject button1;
	public GameObject button2;
	public MeetingPresenterScript presenterScript;
	public MeetingParticipantScript participantScript;

	// Use this for initialization
	void Start () {
		outlet = new MessageOutlet();
		outlet.AddSignalToMessageFilter("UDM");
		outlet.AddSignalToMessageFilter ("LEFT");
		meetingDisplay = GameObject.FindGameObjectWithTag ("MeetingInfoText").GetComponent<MeetingInfoDisplay>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!initialised) {
			try {
				GameObject camera = GameObject.FindGameObjectsWithTag ("MainCamera") [0];
				TestSocket socketScript = camera.GetComponent (typeof(TestSocket)) as TestSocket;
				socket = socketScript.getSocket ();
				socket.registerObserver (outlet);

				initialised = true;
				Debug.Log("Successfully Initialised Player Manager");
				Message ma = new Message("GAP",new byte[0]);
				socket.sendMessage(ma);
			} catch (NullReferenceException ex) {

			}
		} else {
			

			if( !outlet.IsOutletEmpty())
			{
				Message m = outlet.GetNextMessage();

				if (m.Signal.Equals (ServerSignals.LEFT.ToString ())) {
					int userID = BitConverter.ToInt32(m.Payload,0);
					User user = GetUser (userID);

					if (user != null) {
						Debug.Log ("User " + user.userID + ":" + user.firstName + " has left");
						RemoveUser (userID);
					}
				} else if (m.Signal.Equals (ServerSignals.UDM.ToString ())) {
					string userInfo = System.Text.ASCIIEncoding.ASCII.GetString (m.Payload);
					User user = JsonUtility.FromJson<User> (userInfo);

					if (user.presenting) {
						meetingDisplay.SetPresenterName (user.firstName);
					}

					if (user.userID == ReadFile.getUserID ()) {
						Configuration.Config.isClientPresenter = user.presenting;
						button1.SetActive (user.presenting);
						button2.SetActive (user.presenting);
						presenterScript.enabled = user.presenting;
						participantScript.enabled = !user.presenting;
					}
						
					AddUser (user);
				}
			}

		}  
	}

	public void AddUser(User user)
	{
		playerListLock.WaitOne ();
		if (!users.ContainsKey (user.userID)) {
			Debug.Log ("User " + user.userID + ":" + user.firstName + " has joined");
			meetingDisplay.SetNumUsersInMeeting (users.Count+1);
		}
		users.Add (user.userID,user);
		playerListLock.ReleaseMutex ();
	}

	public void RemoveUser(int userID)
	{
		playerListLock.WaitOne ();
		int preCount = users.Count;
		users.Remove (userID);
		Debug.Log ("Was " + preCount + "users, removed a user, now there are only " + users.Count);
		meetingDisplay.SetNumUsersInMeeting (users.Count);
		playerListLock.ReleaseMutex ();
	}

	public User GetUser(int userID)
	{
		User user = null;
		playerListLock.WaitOne ();
		users.TryGetValue (userID, out user);
		playerListLock.ReleaseMutex ();
		return user;
	}

	public int GetNumberOfUsersInMeeting()
	{
		int temp = 0;
		playerListLock.WaitOne ();
		temp = users.Count;
		playerListLock.ReleaseMutex ();
		return temp;
	}
}
