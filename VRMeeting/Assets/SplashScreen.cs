using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {

	public GameObject splashTextObj;
	private TextMesh splashText;

	// Use this for initialization
	void Start () {
		gameObject.SetActive (false);
	}

	void Awake(){
		splashText = splashTextObj.GetComponent<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DisplaySplashScreen(string message)
	{
		splashText.text = message;
		gameObject.SetActive (true);
	}

	public void HideSplashScreen()
	{
		gameObject.SetActive (false);
	}
}
