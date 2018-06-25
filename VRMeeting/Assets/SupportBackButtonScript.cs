using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportBackButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	void FixedUpdate(){
		if (Application.platform == RuntimePlatform.Android)
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
			}
		}
	}
}
