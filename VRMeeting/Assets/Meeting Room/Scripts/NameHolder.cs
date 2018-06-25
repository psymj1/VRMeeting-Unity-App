using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameHolder : MonoBehaviour {

    public Text nameLabel = null;
    public string text;

    public NameHolder()
    {
       
    }

    // Use this for initialization
    void Start()
    {
        nameLabel.text = text;
    }

    void Update () {
        Vector3 namePos = Camera.main.WorldToScreenPoint(this.transform.position);
        nameLabel.transform.position = namePos;
	}
}
