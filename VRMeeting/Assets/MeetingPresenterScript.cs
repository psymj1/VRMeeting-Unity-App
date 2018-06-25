using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using Configuration;
using vrmeeting.protocol;
using vrmeeting.net;
using System;
using System.IO.Compression;
using System.IO;
using vrmeeting.protocol.signals;
using System.Threading;

public class MeetingPresenterScript : MonoBehaviour
{
    private AudioClip buffer;
    private ClientSocket socket = null;
	private bool initialised = false;
	private bool started = false;
    private AudioClip clip;
	private int payloadSize = Config.AUDIO_MESSAGE_PAYLOAD_SIZE;


    // Use this for initialization
    void Start()
    {
		clip = Microphone.Start(null, true, Config.CAPTURE_LENGTH, Config.MIC_SAMPLE_FREQUENCY);
        while (!(Microphone.GetPosition(null) > 0)) { }
        //Debug.Log("start playing... position is " + Microphone.GetPosition(null));
		GameObject camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
		TestSocket socketScript = camera.GetComponent(typeof(TestSocket)) as TestSocket;
		socket = socketScript.getSocket();
		Debug.Log("Obtained socket");
		InvokeRepeating("SendNextClip", Config.CAPTURE_LENGTH, Config.CAPTURE_LENGTH);
    }

    private void SendNextClip()
    {

//		clip = Microphone.Start (null, true, Config.CAPTURE_LENGTH, Config.MIC_SAMPLE_FREQUENCY);

		int numOfSamples = clip.samples;
		float[] samples = new float[numOfSamples];

		clip.GetData(samples, 0);
		byte[] bSamples = new byte[samples.Length * 4];
		Buffer.BlockCopy(samples, 0, bSamples, 0, bSamples.Length);

//		CompressClipAndSend (bSamples);

		Thread compressThread = new Thread (CompressClipAndSend);
		compressThread.Start (bSamples);
    }

	private void CompressClipAndSend(object toSend)
	{
		byte[] samples = (byte[])toSend;

		byte[] bCompressedSamples = Compress(samples);

		int numOfMessagesRequired = (bCompressedSamples.Length / payloadSize);
		if(bCompressedSamples.Length % payloadSize != 0)
		{
			numOfMessagesRequired++;
		}


		Message[] messages = new Message[numOfMessagesRequired + 1];

		for(int i = 0;i < numOfMessagesRequired;i++)
		{
			int offset = i * payloadSize;
			int payloadLength;

			if(i == numOfMessagesRequired - 1 && bCompressedSamples.Length % payloadSize != 0)
			{
				payloadLength = bCompressedSamples.Length % payloadSize;
			}else
			{
				payloadLength = payloadSize;
			}

			byte[] payload = new byte[payloadLength];

			Buffer.BlockCopy(bCompressedSamples, offset, payload,0, payloadLength);

			messages[i] = new Message("AUDI", payload);
		}
		int originalLength = samples.Length;
		ByteBuffer b = new ByteBuffer ();
		b.WriteInt (originalLength);
		messages [messages.Length - 1] = new Message (ClientSignals.EAUD.ToString (),b.ToBytes()); //Put into the EAUD message the original length

		socket.sendMessages (messages);
	}

	private static byte[] Compress(byte[] bArray)
	{
		Debug.Log ("Beginning compression");
		AndroidJNI.AttachCurrentThread ();
		byte[] compressedArray = ByteArrayCompression.ByteArrayCompressionClass.CallStatic<byte[]> ("compress", bArray);
		AndroidJNI.DetachCurrentThread ();
		Debug.Log ("Finished compressing. Result: " + bArray.Length + " bytes -> " + compressedArray.Length + " bytes" );
		return compressedArray;
	}



    // Update is called once per frame
    void Update()
    {
//		AndroidJavaClass bt = new AndroidJavaClass ("vrmeeting.hexcore.audiotransformer.AudioByteArrayTransformer");
//		Debug.Log("Output from the method is: " + bt.CallStatic<int>("getNumber")); //This code works
    }
}
