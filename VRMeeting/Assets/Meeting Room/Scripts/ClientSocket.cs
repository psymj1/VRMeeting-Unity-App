using UnityEngine;  
using System.Collections;  
using System.Net;  
using System.Net.Sockets;  
using System.IO;
using vrmeeting.protocol;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using vrmeeting.exceptions;

namespace vrmeeting.net  
{  
	public class ClientSocket  
	{
		List<MessageOutlet> messageOutlet = new List<MessageOutlet>();
        private static System.Timers.Timer aTimer;
        private const String PAYLOAD_UUID_ENDING = "d11ebd74585511e89c2dfa7ae01bbebc";
        private byte[] PAYLOAD_UUID_ENDING_BYTES;
		private bool encounteredError = false;
		private string error = "";

        //this is for sending messages to server
        private MessageOutlet messagesToServer = new MessageOutlet();

		private static byte[] result; 
		private static Socket clientSocket;

		private bool stop = false;
		// 
		public bool IsConnected = false;  

		public ClientSocket(){  
			clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ByteBuffer b = new ByteBuffer();
            b.WriteString(PAYLOAD_UUID_ENDING);
            PAYLOAD_UUID_ENDING_BYTES = b.ToBytes();
		}  

		public void Stop(){
			this.stop = true;
			clientSocket.Close ();
			try{
				clientSocket.Send(new byte[0]);
			}catch(Exception e) {

			}
		}

		public bool hasEncounteredError()
		{
			return encounteredError;
		}

		public string getError()
		{
			return error;
		}

		/// connected to server with specified IP and port 
		public void ConnectServer(string ip,int port)  
		{  
			try
			{

			IPAddress mIp = IPAddress.Parse(ip);  
			IPEndPoint ip_end_point = new IPEndPoint(mIp, port);  

			try {  
				clientSocket.Connect(ip_end_point);  
				IsConnected = true;  
				Debug.Log("connection success");  
			}  
			catch  
			{  
				IsConnected = false;
				encounteredError = true;
				error = "Failed to connect to " + ip + ":" + port;

				Debug.Log("connection failed to " + ip + ":" + port);  
				return;  
			}


			//retrieve meetingID and token
			GameObject pBoard = GameObject.FindGameObjectsWithTag("pBoard")[0];
			ReadFile readFile = pBoard.GetComponent(typeof(ReadFile)) as ReadFile;

			//receive once
			Message message = ReceiveMessage(result, clientSocket);

			//Debug.Log(message.Signal.Equals("AUTH"));
			//receive AUTH and send TOKEN
			if (message.Signal.Equals("AUTH"))
			{
				ByteBuffer bf2 = new ByteBuffer();
				bf2.WriteString("TOKE\n");
				bf2.WriteString(readFile.GetAuthToken() + PAYLOAD_UUID_ENDING);        // the token
				clientSocket.Send(bf2.ToBytes());          
			}

			message = ReceiveMessage(result, clientSocket);

			//receive NVAL or MEET
			if (message.Signal.Equals("MEET"))
			{
				ByteBuffer bf2 = new ByteBuffer();
				bf2.WriteString("MID\n");
				bf2.WriteString(readFile.getMeetingCode() + PAYLOAD_UUID_ENDING);        //meeting ID
				clientSocket.Send(bf2.ToBytes());
			}
			else if (message.Signal.Equals("NVAL"))
			{
                //end the meeting
				encounteredError = true;
				error = "Authentication Failed";
				Debug.Log("connection fail");
				return;
            }

			message = ReceiveMessage(result, clientSocket);

			//reveive NVAL or VAL
			if (message.Signal.Equals("VAL"))
			{
				//Debug.Log("connection success aaa");
                //connect success return clientSocket?
                Thread listeningThread = new Thread(ListeningMessages);
                listeningThread.Start();
                Thread sendingThread = new Thread(SendingMessages);
                sendingThread.Start();
			}
			else if (message.Signal.Equals("NVAL"))
			{
                //end the meeting
				encounteredError = true;
				error = "Authentication Failed";
				Debug.Log("connection fail");
				return;
            }
			}catch(ApplicationException e) {
				encounteredError = true;
				error = e.Message;
				return;
			}
		}

        public void sendMessage(Message message)
        {
            this.messagesToServer.OutputMessage(message);
        }

		public void sendMessages(Message[] messages)
		{
			messagesToServer.OutputMessages (messages);
		}

        //start to send messages to server
        private void SendingMessages()
        {
			Debug.Log ("Message Sending Thread Started");

            while (!this.stop) {
				Message message = this.messagesToServer.GetNextMessage ();
                if(message != null)
                {
                    ByteBuffer sending = new ByteBuffer();
                    sending.WriteString(message.Signal + "\n");
					if (message.Payload != null) {
						sending.WriteBytes(message.Payload);
					}                    
                    sending.WriteString(PAYLOAD_UUID_ENDING);
					if (clientSocket.Connected) {
						clientSocket.Send (sending.ToBytes ());
					} else {
						encounteredError = true;
						error = "Lost connection to server";
						Stop ();
					}
                }
			}
				
			Debug.Log ("Sending thread stopped");
        }

        //start to receive messages with server
        private void ListeningMessages()
        {
			Debug.Log ("Message Receiving Thread Started");
			while (!this.stop) {
				//while true
				try { //Protects against the thread stopping due to an uncaught InvalidMessageException thrown by the MessageParser
					Message message = ReceiveMessage (result, clientSocket);
					
					if (message.Signal.Equals ("CHNG")) {
						//change slide?
						NotifyObserver ("CHNG", message);
					}
					
					if (message.Signal.Equals ("END")) {
						//
						NotifyObserver ("END", message);
					}
					
					if (message.Signal.Equals ("AUDI")) {
                        //Debug.Log("AUDIO MESSAGE SPAM");
                        //
                        //Debug.Log("AUDI SPAM");
						NotifyObserver ("AUDI", message);
					
					}

					if(message.Signal.Equals("EAUD")){
						//Debug.Log("END OF AUDIO SAMPLE REACHED");
						NotifyObserver(message.Signal,message);
					}
					
					if (message.Signal.Equals ("LEFT")) {
						NotifyObserver ("LEFT", message);
					}
					
					if (message.Signal.Equals ("UDM")) {
                        NotifyObserver ("UDM", message);
					}
				} catch (InvalidMessageException ex) {
					Debug.Log (ex);
				}catch (ApplicationException e) {
					Debug.Log (e);
					encounteredError = true;
					this.stop = true;
					error = e.Message;
				}
			}

			Debug.Log ("Receiving thread stopped");
        }

        private void NotifyObserver(String signal,Message message)
        {
            for(int i = 0; i < messageOutlet.Count; i++)
            {
                if (messageOutlet[i].IsSignalInFilter(signal))
                {
                    messageOutlet[i].OutputMessage(message);
                }
            }
        }

 
        //receive a message from server
        private Message ReceiveMessage(byte[] result, Socket clientSocket)
		{
            List<byte> signal = new List<byte>();
            List<byte> payload = new List<byte>();
            

            //First loop will gather the signal 
            while(true)
            {
                byte[] nextByte = new byte[1];
                int bytesReceived = clientSocket.Receive(nextByte,1,SocketFlags.None);
				if(bytesReceived == 0)
				{
					Debug.Log("Connection closed");
					throw new ApplicationException("Lost connection to server");
				}
                signal.Add(nextByte[0]);
                char byteAsCharacter = Convert.ToChar(nextByte[0]);
                if(byteAsCharacter == '\n')
                {
                    break;
                }
            }

            //Second loop will gather the payload
            List<char> uuidComparison = new List<char>();
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;

            while (true)
            {
                byte[] nextByte = new byte[1];
                int bytesReceived = clientSocket.Receive(nextByte, 1, SocketFlags.None);
                if(bytesReceived == 0)
                {
					Debug.LogError ("Connection closed"); 
					throw new ApplicationException("Connection lost to server");
                }
                else
                {
                    char byteAsCharacter = Convert.ToChar(nextByte[0]);
                    payload.Add(nextByte[0]);

                    if (payload.Count() >= PAYLOAD_UUID_ENDING_BYTES.Length)
                    {
                        int offsetFromStart = payload.Count - PAYLOAD_UUID_ENDING_BYTES.Length;
                        ByteBuffer bs = new ByteBuffer();
                        for(int i = 0;i < PAYLOAD_UUID_ENDING_BYTES.Length;i++)
                        {
                            bs.WriteByte(payload[offsetFromStart + i]);
                        }

                        String toCompare = System.Text.Encoding.Default.GetString(bs.ToBytes());
                        if (toCompare.Equals(PAYLOAD_UUID_ENDING))
                        {
                            break;
                        }
                    }

                    TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
                    int secondsSinceEpochs = (int)ts.TotalSeconds;
                    if (secondsSinceEpochs >= secondsSinceEpoch + 3)
                    {
						throw new ApplicationException("Failed to receive a full message after 3 seconds");
                    }
                }
                
            }

            //Debug.Log("Payload Length before remove: " + payload.Count);
            payload.RemoveRange(payload.Count - PAYLOAD_UUID_ENDING_BYTES.Length, PAYLOAD_UUID_ENDING_BYTES.Length);
            //Debug.Log("Payload length after remove:" + payload.Count);
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(signal.ToArray());
            buffer.WriteBytes(payload.ToArray());
            Message message = MessageParser.parseMessage(buffer.ToBytes());
			return message;
		}

        public void registerObserver(MessageOutlet o)
        {
            messageOutlet.Add(o);
        }
        public void removeObserver(MessageOutlet o)
        {
            if (!messageOutlet.Any())
            {
                messageOutlet.Remove(o);
            }
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

//[Serializable]
//public class InfosCollection
//{
//    public dataField[] infos;
//}