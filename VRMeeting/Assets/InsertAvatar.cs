using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vrmeeting.protocol;
using vrmeeting.net;
using System;

public class InsertAvatar : MonoBehaviour {

    private MessageOutlet messageOutlet = null;
    private bool initialised = false;
    private List<Vector3> listOfPositions = new List<Vector3>();
    private bool[] exists = new bool[10];

    // Use this for initialization
    void Start () {
        listOfPositions[0] = new Vector3(2.06f, 0.49f, 2.60f);
        listOfPositions[1] = new Vector3(1.379f, 0.49f, 2.60f);
        listOfPositions[2] = new Vector3(0.868f, 0.49f, 2.60f);
        listOfPositions[3] = new Vector3(0.335f, 0.49f, 2.60f);
        listOfPositions[4] = new Vector3(-0.198f, 0.49f, 2.60f);
        listOfPositions[5] = new Vector3(-0.32f, 0.49f, 4.93f);
        listOfPositions[6] = new Vector3(0.335f, 0.49f, 4.93f);
        listOfPositions[7] = new Vector3(0.868f, 0.49f, 4.93f);
        listOfPositions[8] = new Vector3(1.379f, 0.49f, 4.93f);
        listOfPositions[9] = new Vector3(2.06f, 0.49f, 4.93f);
        for(int i = 0; i < listOfPositions.Count; i++)
        {
            exists[0] = true;
        }
        messageOutlet = new MessageOutlet();
        messageOutlet.AddSignalToMessageFilter("UDM");
    }

    // Update is called once per frame
    void Update () {
        if (!initialised)
        {
            try
            {
                GameObject camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
                TestSocket socketScript = camera.GetComponent(typeof(TestSocket)) as TestSocket;
                ClientSocket socket = socketScript.getSocket();
                socket.registerObserver(messageOutlet);
                initialised = true;
                Debug.Log("Successfully Initialised New Avatar");
            }
            catch (NullReferenceException ex)
            {

            }
        }
        else
        {
            //Debug.Log("In else statement");
            if (messageOutlet != null)
            {
                if (!messageOutlet.IsOutletEmpty())
                {
                    Message m = messageOutlet.GetNextMessage();
                    Debug.Log("signal:" + m.Signal);
                    if (m.Signal == "UDM")
                    {
                        ByteBuffer bf1 = new ByteBuffer(m.Payload);
                        String userData = bf1.ReadString(m.Payload.Length);

                        dataField userInfo = JsonUtility.FromJson<dataField>(userData);
                        String userID = userInfo.userID;
                        String firstName = userInfo.firstName;
                        String surName = userInfo.surName;
                        String Company = userInfo.Company;
                        String jobTitle = userInfo.jobTitle;
                        String workEmail = userInfo.workEmail;
                        String phoneNumber = userInfo.phoneNumber;
                        String avatarID = userInfo.avatarID;

                        int id;
                        int.TryParse(avatarID, out id);

                        Debug.Log("Avatar ID:" + id);
                        DisplayAvatar(id);
                    }
                }
            }
        }
    }

    void DisplayAvatar(int avatarid)
    {
        string path;
        if (avatarid >= 0 && avatarid <= 4)
        {
            path = "Male" + (avatarid + 1).ToString() + "/maleChar" + (avatarid + 1).ToString();
        }
        else
        {
            path = "Female" + (avatarid + 1).ToString() + "/femaleChar" + (avatarid + 1).ToString() + "blend";
        }
        GameObject avatar = (GameObject)GameObject.Instantiate(Resources.Load(path));
        GameObject[] chairs = GameObject.FindGameObjectsWithTag("chair");
        foreach (GameObject target in chairs)
        {
            GameObject.Destroy(target);
        }
        for (int i = 0; i<listOfPositions.Count; i++)
        {
            if (exists[i])
            {
                avatar.transform.position = listOfPositions[i];
                avatar.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                exists[i] = false;
                break;
            }
        }
    }

    [Serializable]
    public class dataField
    {
        public String userID;
        public String firstName;
        public String surName;
        public String Company;
        public String jobTitle;
        public String workEmail;
        public String phoneNumber;
        public String avatarID;
    }
}
