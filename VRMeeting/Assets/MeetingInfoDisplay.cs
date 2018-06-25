using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MeetingInfoDisplay : MonoBehaviour {

	private int numUsersInMeeting = 0;
	private string presenter = "";
	private int currentSlideNumber = -1;
	private int maxSlideNumber = -1;

	private Mutex meetingInfoLock = new Mutex();
	private GameObject meetingTextObject;
	private TextMesh textOut;
	private bool textChanged = false;

	// Use this for initialization
	void Start () {
		meetingTextObject = GameObject.FindGameObjectWithTag ("MeetingInfoText");
		textOut = meetingTextObject.GetComponent<TextMesh> ();
	}

	// Update is called once per frame
	void Update () {
		if (hasTextChanged ()) {
			int curSlide;
			int numUsers;
			string presenterName;

			meetingInfoLock.WaitOne ();
			curSlide = currentSlideNumber;
			numUsers = numUsersInMeeting;
			presenterName = presenter;
			textChanged = false;
			meetingInfoLock.ReleaseMutex ();

			string textToDisplay = "Current Slide Number: " + (curSlide == -1 ? "" : curSlide.ToString ()) + (maxSlideNumber == -1 ? "" : (Configuration.Config.isClientPresenter ? "/" + maxSlideNumber : ""));
			textToDisplay += "\nPresenter: " + presenterName;

			textOut.text = textToDisplay;
		}
	}

	public void SetMaxSlideNumber(int num)
	{
		meetingInfoLock.WaitOne ();
		maxSlideNumber = num;
		textChanged = true;
		meetingInfoLock.ReleaseMutex ();
	}

	private bool hasTextChanged()
	{
		bool temp = false;
		meetingInfoLock.WaitOne ();
		temp = textChanged;
		meetingInfoLock.ReleaseMutex ();
		return temp;
	}

	public void SetPresenterName(string name)
	{
		meetingInfoLock.WaitOne ();
		presenter = name;
		textChanged = true;
		meetingInfoLock.ReleaseMutex ();
	}

	public void SetNumUsersInMeeting(int count)
	{
		meetingInfoLock.WaitOne ();
		numUsersInMeeting = count;
		textChanged = true;
		meetingInfoLock.ReleaseMutex ();
	}

	public void SetCurrentSlideNumber(int num)
	{
		meetingInfoLock.WaitOne ();
		currentSlideNumber = num;
		textChanged = true;
		meetingInfoLock.ReleaseMutex ();
	}
}
