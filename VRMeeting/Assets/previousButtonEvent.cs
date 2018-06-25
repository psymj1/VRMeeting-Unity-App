using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vrmeeting.net;
using vrmeeting.protocol;

public class previousButtonEvent : MonoBehaviour {

    public float spinvalue = 90;
    GameObject pBoard;
    PresentationBoard script;

    // Use this for initialization
    void Start()
    {
        pBoard = GameObject.FindGameObjectsWithTag("pBoard")[0];
        script = pBoard.GetComponent<PresentationBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.up * spinvalue * Time.deltaTime);
    }

    public void flipspinWithDelay()
    {
		InvokeRepeating("changeSlide", Configuration.Config.BUTTON_TRIGGER_DELAY,Configuration.Config.BUTTON_POST_INITIAL_TRIGGER_DELAY);
    }

    public void filpspinCancel()
    {
		CancelInvoke("changeSlide");
    }

    public void changeSlide()
    {
		AnimateButton ();
		script.DecrementSlide ();
    }

	public void AnimateButton()
	{
		GrowButton ();
		Invoke ("ShrinkButton", 0.2f);
	}

	public void GrowButton()
	{
		gameObject.transform.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
	}

	public void ShrinkButton()
	{
		gameObject.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
	}
}
