using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Configuration;
using vrmeeting.protocol;
using vrmeeting.net;
using System;

public class PresenterVoiceOutput : MonoBehaviour {

	private MessageOutlet outlet = new MessageOutlet ();
	private const int MAX_SAMPLE_MUTEX_WAIT_TIME = 1; //In ms, Used to avoid blocking on any public calls that will try to access the audio sample buffer, they will only block for this length of time
	private Mutex sampleListAccess = new Mutex (); //Used to avoid concurrent modification issues during read/write to samples stored
	private List<float> audioSamples = new List<float>(); //The audio samples that have been received from the server
	private float playNextSampleAt = 0; //The TIME to play the next sample at. This is set to the time the previous clip started + the length of the clip
	private long clipCount = 0; //Just used to make it easier to see new audio coming in and being played during debugging

	// Use this for initialization
	void Start () {
		//Here need to retrieve the ClientSocket somehow and add the outlet to the list of outlets
		outlet.AddSignalToMessageFilter("AUDI"); //Means only receive AUDI messages on this outlet
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time >= playNextSampleAt) {
			float[] receivedAudioSamples = GetStoredSamples ();
			if (receivedAudioSamples != null) {
				PlayAudioSamples (receivedAudioSamples);
			}
		}

		if (!outlet.IsOutletEmpty ()) { //Check for any new messages and extract the audio samples from them
			Message message = outlet.GetNextMessage ();
			if (message != null) {
				
				byte[] payload = message.Payload; //Get the payload containing the samples
				byte[] tempFloat = new byte[4]; //Create a temp byte array to store 1 sample
				List<float> tempSamples = new List<float> (); //Create a list to store each sample that's produced from the payload

				for (int i = 0; i < payload.Length; i++) { //Take every 4 bytes, convert it to a float and add it to the temp list
					if (i > 0 && i % 4 == 0) {
						tempSamples.Add (new ByteBuffer (tempFloat).ReadFloat());
					}
					tempFloat[i % 4] = payload[i];
				}

				AddSamplesToList (tempSamples.ToArray ());
			}
		}

	}

	private void PlayAudioSamples(float[] samples)
	{
		AudioClip newClip = AudioClip.Create ("Clip" + clipCount++, samples.Length, Config.NUM_OF_CHANNELS, Config.MIC_SAMPLE_FREQUENCY, false);
		newClip.SetData (samples,0);
		float length = newClip.length;
		playNextSampleAt = Time.time + length;
		AudioSource.PlayClipAtPoint (newClip, new Vector3 (0, 0, 0));
	}

	private void AddSamplesToList(float[] samples)
	{
		sampleListAccess.WaitOne ();
		audioSamples.AddRange (samples);
		sampleListAccess.ReleaseMutex ();
	}

	public float[] GetStoredSamples()
	{
		if (sampleListAccess.WaitOne (MAX_SAMPLE_MUTEX_WAIT_TIME)) {
			if (audioSamples.Count > 0) {
				float[] samples = audioSamples.ToArray ();
				audioSamples.Clear ();
				sampleListAccess.ReleaseMutex ();
				return samples;
			} else {
				sampleListAccess.ReleaseMutex ();
			}
		}

		return null;
	}
}
