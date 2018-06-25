using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class ReadFile : MonoBehaviour
{
	private string token;
	private string meetingCode;
	private string webServerIdentifier;
	private string hostServerIdentifier;
	private string fileServerIdentifier;
	private int webServerPort;
	private int hostServerPort;
	private int fileServerPort;
	private static int userID;

    // Use this for initialization
    void Start()
    {
		if (Application.isMobilePlatform) {
			SetPropertiesFromIntentExtras ();
		} else {
			SetPropertiesFromFile ();
		}
    }

	private void SetPropertiesFromIntentExtras()
	{
		AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
		AndroidJavaObject intent = activity.Call<AndroidJavaObject>("getIntent");
		AndroidJavaObject extras = intent.Call<AndroidJavaObject> ("getExtras");
		token = extras.Call<string> ("getString","AuthToken");
		meetingCode = extras.Call<string> ("getString", "MeetingCode");
		webServerIdentifier = extras.Call<string> ("getString", "WebServerIdentifier");
		hostServerIdentifier = extras.Call<string> ("getString", "HostServerIdentifier");
		fileServerIdentifier = extras.Call<string> ("getString","FileServerIdentifier");
		webServerPort = extras.Call<int> ("getInt", "WebServerPort");
		hostServerPort = extras.Call<int> ("getInt", "HostServerPort");
		fileServerPort = extras.Call<int> ("getInt", "FileServerPort");
		userID = extras.Call<int> ("getInt", "UserID");
//		Debug.Log ("Authentication Token:" + token);
//		Debug.Log ("Meeting Code:" + meetingCode);
//		Debug.Log ("Web Server URL:" + webServerIdentifier + ":" + webServerPort);
//		Debug.Log ("Host Server URL:" + hostServerIdentifier + ":" + hostServerPort);
//		Debug.Log ("File Server URL:" + fileServerIdentifier + ":" + fileServerPort);
		Debug.Log("User ID:" + userID);
	}

	private void SetPropertiesFromFile()
	{
		//File Format is:
		// AuthenticationToken
		// MeetingCode
		// UserID
		// WebServerIdentifier (IP or Domain name)
		// WebServerPort
		// HostServerIdentifier (IP or Domain Name)
		// HostServerPort
		// FileServerIdentifier (IP or Domain Name)
		// FileServerPort

		string path; 
		string fileName = "properties.txt"; 

		path = Path.Combine(Application.persistentDataPath, fileName); 

		try{
			StreamReader reader = new StreamReader(path);
			int numExpectedParams = 9;
			string [] param = File.ReadAllLines(path);

			if(param.Length < numExpectedParams)
			{
				throw new Exception("Missing properties from file " + path + " Expected properties file format: AuthenticationToken\n\t\t MeetingCode\n\t\t WebServerIdentifier (IP or Domain name)\n\t\t WebServerPort\n\t\t HostServerIdentifier (IP or Domain Name)\n\t\t HostServerPort\n\t\t FileServerIdentifier (IP or Domain Name)\n\t\t FileServerPort");
			}
				
			reader.Close(); 

			token = param [0];
			meetingCode = param [1];
			userID = Int32.Parse(param[2]);
			webServerIdentifier = param [3];
			webServerPort = Int32.Parse (param [4]);
			hostServerIdentifier = param [5];
			hostServerPort = Int32.Parse (param [6]);
			fileServerIdentifier = param [7];
			fileServerPort = Int32.Parse (param [8]);
		}catch(Exception e) {
			Debug.LogError("Failed to load properties from property file:" + e.Message);
			Application.Quit ();
		}
	}

	public static int getUserID()
	{
		return userID;
	}

	public string GetAuthToken()
	{
		return token;
	}

	public string getMeetingCode()
	{
		return meetingCode;
	}

	public int GetWebServerPort()
	{
		return webServerPort;
	}

	public int GetHostServerPort()
	{
		return hostServerPort;
	}

	public int GetFileServerPort()
	{
		return fileServerPort;
	}

	public string GetWebServerIdentifier()
	{
		return webServerIdentifier;
	}

	public string GetHostServerIdentifier()
	{
		return hostServerIdentifier;
	}

	public string GetFileServerIdentifier()
	{
		return fileServerIdentifier;
	}

    // Update is called once per frame
    void Update()
    {

    }
}
