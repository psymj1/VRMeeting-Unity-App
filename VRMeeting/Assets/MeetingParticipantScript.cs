using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using vrmeeting.net;
using System.Threading;
using vrmeeting.protocol;
using Configuration;
using System.IO.Compression;
using vrmeeting.protocol.signals;

/**
 * Plays the audio received from the presenter aloud
 */
public class MeetingParticipantScript : MonoBehaviour {

	private bool initialised = false;
	private ClientSocket socket;

	private List<float> audioSampleBuffer = new List<float>();
    private Mutex audioSampleBufferMutex = new Mutex();
    private Boolean stop = false;
	private MessageOutlet presenterAudioOutlet = new MessageOutlet();

	private long clipCount = 0; //Just makes it easier to debug in the unity client to see if a new clip is playing as it will have a slightly different name
	private float playNextSampleAt = 0; //The next time to play a sample at
    private float lastSampleStartedAt = 0;
    private int startTime;
    private bool run = false;
    private bool canOutput = false;

	private long audioClipsGenerated = 0;
	private List<byte[]> payloadBuffer = new List<byte[]>(); //Used to store the audio data for the current sample from separate messages until an EAUD message is received
	private List<float[]> clipQueue = new List<float[]>(); //Used to store the next clips to be played incase they can't yet be played, stored in float form
	private Mutex clipQueueLock = new Mutex();

	// Use this for initialization
	void Start () {
        presenterAudioOutlet.AddSignalToMessageFilter("AUDI");
		presenterAudioOutlet.AddSignalToMessageFilter ("EAUD");
		Invoke("CanOutput", Config.DELAY_UNTIL_START_PLAYING);
    }

    private void CanOutput()
    {
        canOutput = true;
    }

	private bool IsClipQueueEmpty()
	{
		bool temp = false;
		clipQueueLock.WaitOne ();
		temp = clipQueue.Count == 0;
		clipQueueLock.ReleaseMutex ();
		return temp;
	}

	private void AddClipToQueue(float[] clip)
	{
		clipQueueLock.WaitOne ();
		clipQueue.Add (clip);
		clipQueueLock.ReleaseMutex ();
	}

	private AudioClip GetNextClipInQueue()
	{
		if (IsClipQueueEmpty ()) {
			return null;
		} else {
			float[] tempFloatArray;

			clipQueueLock.WaitOne ();
			tempFloatArray = clipQueue [0];
			clipQueue.RemoveAt (0);
			clipQueueLock.ReleaseMutex ();

			AudioClip newClip = AudioClip.Create("New clip" + audioClipsGenerated++, tempFloatArray.Length, 1, Config.MIC_SAMPLE_FREQUENCY, false);
			newClip.SetData(tempFloatArray, 0);
			//Debug.Log ("Returning next audio clip " + newClip.name);
			return newClip;
		}
	}

	private class CompressedData{
		private int originalLength;
		private byte[] compressedData;
		public CompressedData(byte[] compressedData,int originalLength)
		{
			this.originalLength = originalLength;
			this.compressedData = compressedData;
		}

		public int getOriginalLength()
		{
			return originalLength;
		}

		public byte[] getCompressedData()
		{
			return compressedData;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!initialised) {
			try {
				GameObject camera = GameObject.FindGameObjectsWithTag ("MainCamera") [0];
				TestSocket socketScript = camera.GetComponent (typeof(TestSocket)) as TestSocket;
				socket = socketScript.getSocket ();
                socket.registerObserver(presenterAudioOutlet);
				initialised = true;
                //Debug.Log("Successfully Initialised Meeting Participant Script");
            } catch (NullReferenceException ex) {

			}
        }
        else
        {
			
			if (!presenterAudioOutlet.IsOutletEmpty ()) { //Whenever you receive a message
				Message next = presenterAudioOutlet.GetNextMessage(); //Get that message

				if (next.Signal.Equals (ServerSignals.AUDI.ToString ())) { //Add the payload to the buffer of data for the next audio sample
					payloadBuffer.Add(next.Payload);
				} else if (next.Signal.Equals (ServerSignals.EAUD.ToString ())) { //Take all of the payloads obtained until this point and append them to make 1 byte[] then decompress the array and convert it to an audio clip
					ByteBuffer combinedPayload = new ByteBuffer();
					//Debug.Log ("Received full audio clip. It took " + payloadBuffer.Count + " messages to receive the audio");

					foreach (byte[] currentPayload in payloadBuffer) {
						combinedPayload.WriteBytes (currentPayload);
					}
					payloadBuffer.Clear ();


					int originalDataLength = BitConverter.ToInt32 (next.Payload, 0);

//					DecompressAndStore (new CompressedData (combinedPayload.ToBytes (), originalDataLength));
					Thread decompressThread = new Thread (DecompressAndStore);
					Debug.Log ("Decompressing " + combinedPayload.ToBytes ().Length + " bytes");
					decompressThread.Start (new CompressedData(combinedPayload.ToBytes (), originalDataLength));
				}
			}

			if(canOutput)//If the initial 15 seconds has passed
            { 
                if (Time.time >= playNextSampleAt) // If it's time to play the next sample i.e the time since the last sample has started + it's length has elapsed
                {
					if (!IsClipQueueEmpty()) //If there's a full clip to play
                    {
                        //Debug.Log("Playing audio");
						AudioClip nextClip = GetNextClipInQueue ();
						if (nextClip != null) {
							//Debug.Log("Playing audio clip that is " + nextClip.length + " seconds long");
							lastSampleStartedAt = Time.time;
							playNextSampleAt = Time.time + nextClip.length;
							AudioSource.PlayClipAtPoint(nextClip, new Vector3(0, 0, 0));
						}
                        

                    }
                }
            }
            
        }
	}

	private static byte[] DeCompress(byte[] bArray,int originalSize)
	{
		Debug.Log ("Beginning decompression");
		AndroidJNI.AttachCurrentThread ();
		Debug.Log ("a");
		byte[] decompressedArray = ByteArrayCompression.ByteArrayCompressionClass.CallStatic<byte[]> ("decompress", bArray);
		Debug.Log ("b");
		AndroidJNI.DetachCurrentThread ();
		if (decompressedArray != null) {
			Debug.Log ("Finished Decompressing. Result: " + bArray.Length + " bytes -> " + decompressedArray.Length + " bytes");
			return decompressedArray;
		} else {
			return new byte[0];
		}
	}

	void DecompressAndStore(object data)
	{
		Debug.Log ("1");
		CompressedData compressedData = (CompressedData)data;
		Debug.Log ("2");
		byte[] decompressedPayload = DeCompress (compressedData.getCompressedData(),compressedData.getOriginalLength());
		Debug.Log ("3");
		if (decompressedPayload.Length > 0) {
			float[] samples = new float[decompressedPayload.Length / 4];
			Buffer.BlockCopy (decompressedPayload, 0, samples, 0, decompressedPayload.Length);
			Debug.Log ("4");
			AddClipToQueue (samples);
			Debug.Log ("5");
		} else {
			Debug.Log ("Attempted to decompress invalid data, Audio Chunk Lost");
		}
	}

    void OnApplicationQuit()
    {
        stop = true;
    }
}
