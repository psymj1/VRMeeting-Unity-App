using System;

namespace Configuration
{
	public class Config
	{
		public const int MAX_PACKET_SIZE = 8192; //In Bytes
		public const int SIGNAL_SIZE = sizeof(char); //In Bytes
		public const int MEETING_ID_LENGTH = 16; //In bytes
		//public const int MAX_PAYLOAD_SIZE = MAX_PACKET_SIZE - SIGNAL_SIZE - MEETING_ID_LENGTH; //In bytes
		public const int MAX_PAYLOAD_SIZE = 1000; //In Bytes
		public const int AUDIO_MESSAGE_PAYLOAD_SIZE = 8000; //In Bytes
		public const int AUDIO_SAMPLE_SIZE = sizeof(float); //In bytes

		public const int MAX_NUMBER_OF_SAMPLES = MAX_PAYLOAD_SIZE/AUDIO_SAMPLE_SIZE;
		public const float DELAY_BEFORE_LEAVE = 5f; //In Seconds

		public const int CAPTURE_LENGTH = 3; //In seconds
		public const int DELAY_UNTIL_START_PLAYING = 15; //In seconds
		public const float AUDIO_PLAY_OFFSET = -0.1f; //In seconds 
		public const int SERVER_PORT = 25561;
		public const string SERVER_IP = "192.168.0.12";
		public const int MIC_SAMPLE_FREQUENCY = 16000; //In Hz
		public const int NUM_OF_CHANNELS = 1;
		public const string TEST_SERVER_ID = "testID1234567809";

		public const float HEARTBEAT_FREQUENCY = 5.0f;//In seconds
		public const float BUTTON_TRIGGER_DELAY = 0.5f; //In seconds, The time you have to look at a button for before it will perform an action
		public const float BUTTON_POST_INITIAL_TRIGGER_DELAY = 1.0f; //In Seconds, the time delay between each trigger of a button after it's initial trigger
		public static bool isClientPresenter = false;
		private Config ()
		{

		}
	}
}

