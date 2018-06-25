using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningText : MonoBehaviour {

	private float countdown;

	private TextMesh text;

	// Use this for initialization
	void Start () {
		
	}

	void Awake()
	{
		text = GetComponent<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnEnable()
	{
		countdown = Configuration.Config.DELAY_BEFORE_LEAVE; //Since it starts after 1 second
		InvokeRepeating("DisplayWarning",0,1);
	}

	void OnDisable()
	{
		CancelInvoke ("DisplayWarning");
	}

	public void DisplayWarning()
	{
		if (countdown >= 0) {
			text.text = "Leaving meeting in: " + countdown;
			countdown--;
		} else {
			CancelInvoke ("DisplayWarning");
		}
	}
}
