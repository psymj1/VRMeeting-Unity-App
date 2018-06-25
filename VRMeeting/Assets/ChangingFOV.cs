using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingFOV : MonoBehaviour {

	bool zoomIn = false;
	float lastChange = 0;
	int currentFOV = baseFOV;
	bool needToChange = false;
	Camera camera;
	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (needToChange) {
			if (Time.time >= lastChange + 0.03) {
				lastChange = Time.time;
				if (zoomIn) {
					currentFOV = currentFOV <= zoomFOV ? zoomFOV : currentFOV - 1;
					if (currentFOV <= zoomFOV) {
						needToChange = false;
					}
				} else {
					currentFOV = currentFOV >= baseFOV ? baseFOV : currentFOV + 1;
					if (currentFOV >= baseFOV) {
						needToChange = false;
					}
				}

				camera.fieldOfView = currentFOV;
			}
		}
	}
	private const int baseFOV = 60;
	private const int zoomFOV = 30;

	public void ZoomIn()
	{
		zoomIn = true;
		needToChange = true;
	}

	public void ZoomOut()
	{
		zoomIn = false;
		needToChange = true;
	}
}
