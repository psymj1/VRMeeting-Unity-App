using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteArrayCompression : MonoBehaviour {

	public static AndroidJavaClass ByteArrayCompressionClass;

	// Use this for initialization
	void Start () {
		ByteArrayCompressionClass = new AndroidJavaClass ("vrmeeting.hexcore.audiotransformer.AudioByteArrayTransformer");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
