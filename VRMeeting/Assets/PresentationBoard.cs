using System.Collections;
using vrmeeting.protocol;
using vrmeeting.net;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PresentationBoard : MonoBehaviour {

    private int currentSlideNumber = -1;
    private bool slideChanged;
	private int maxSlideNumber = -1; //-1 means no max slide number

    private MessageOutlet outlet;
	private bool initialised = false;
	private ClientSocket socket;
	private MeetingInfoDisplay meetingDisplay;
	GameObject pBoard;
	ReadFile readFile;

	// Use this for initialization
	void Start () {
        slideChanged = false;
        outlet = new MessageOutlet();
        outlet.AddSignalToMessageFilter("CHNG");
		meetingDisplay = GameObject.FindGameObjectWithTag ("MeetingInfoText").GetComponent<MeetingInfoDisplay>();
    }

	void Awake(){
		pBoard = GameObject.FindGameObjectWithTag ("pBoard");
		readFile = pBoard.GetComponent(typeof(ReadFile)) as ReadFile;
	}

	/*
	 * Sends a signal to the server to change slide to the specified number
	 * Will only send the message if the number is greater than 0
	 */
	public void TriggerSlideChangeTo(int number)
	{
		if (number >= 0) {
			byte[] intBytes = BitConverter.GetBytes (number);
				
			socket.sendMessage(new Message("CHNG",intBytes));
		}
	}

	private void FixSlideNumber()
	{
		if (currentSlideNumber < 0) {
			currentSlideNumber = 0;
		}

		if (maxSlideNumber >= 0 && Configuration.Config.isClientPresenter) { //If the client is just watching it doesn't matter what the last slide of the power point is
			if (currentSlideNumber >= maxSlideNumber) {
				currentSlideNumber = maxSlideNumber;
			}
		}
	}

	/*
	 * Increases the current slide number by 1
	 */ 
	public void IncrementSlide()
	{
		TriggerSlideChangeTo (currentSlideNumber + 1);
	}

	public void DecrementSlide()
	{
		TriggerSlideChangeTo (currentSlideNumber - 1);
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
				Debug.Log("Successfully Initialised Presentation Board");
			} catch (NullReferenceException ex) {
				
			}
		} else {
			
			if( !outlet.IsOutletEmpty())
			{
				Message m = outlet.GetNextMessage();
				if (m.Signal == "CHNG")
				{
					ByteBuffer buffer = new ByteBuffer(m.Payload);
					currentSlideNumber = buffer.ReadInt(); //The only way the current slide number changes is if a CHNG message is received
					FixSlideNumber ();
					meetingDisplay.SetCurrentSlideNumber (currentSlideNumber);
					StartCoroutine(ChangeImage()); //The only way the image on the board will change is if a CHNG message is received
				}
			}
		}  
    }

	private void SetMaxSlideNumber(int num)
	{
		maxSlideNumber = num-1; //-1 because slide numbers index from 0
		meetingDisplay.SetMaxSlideNumber (maxSlideNumber);
	}

    IEnumerator ChangeImage()
    {
        
		string mCode = readFile.getMeetingCode();

		string url = "http://" + readFile.GetFileServerIdentifier() + ":" + readFile.GetFileServerPort() + "/" + mCode + "/" + currentSlideNumber + ".jpg";

        using (WWW www = new WWW(url))
        {
            yield return www;

			if (!string.IsNullOrEmpty(www.error)){
				//End of the slides must've been reached
				if (www.error.Contains ("404")) {
					SetMaxSlideNumber(currentSlideNumber);
					FixSlideNumber ();
				}
			} else {
				pBoard.GetComponent<Renderer>().material.mainTexture = www.texture;
			}
            
        }
    }
}
