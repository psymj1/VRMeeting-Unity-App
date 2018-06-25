using System;
using System.Text;
using UnityEngine;
using vrmeeting.net;

namespace vrmeeting.protocol
{

	using ProtocolConfig = vrmeeting.config.protocol.ProtocolConfig;
	using InvalidMessageException = vrmeeting.exceptions.InvalidMessageException;
	using ServerSignals = vrmeeting.protocol.signals.ServerSignals;

	/// <summary>
	/// A Utility Class used to parse incoming messages, checking that they adhere to the VRMeeting Messaging Protocol </summary>
	/// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a>
	/// @author 
	///  </seealso>
	public class MessageParser
	{

		private MessageParser()
		{
		}


		/// <summary>
		/// Parses the raw 'message' passed into the function according to the VRMeeting Messaging Protocol </summary>
		/// <param name="message"> The message to validate </param>
		/// <returns> Returns a <seealso cref="Message"/> object which represents the parsed raw message </returns>
		/// <exception cref="InvalidMessageException"> Thrown if the message is invalid </exception>
		/// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>

		public static Message parseMessage(byte[] message)
		{
			if (message == null)
			{
				throw new System.ArgumentException("Error parsing message: message cannot be null");
			}

			if (!(message.Length > 0))
			{
				throw new System.ArgumentException("Error parsing message: The byte array cannot be empty");
			}

			//if (message.Length > ProtocolConfig.MAX_POSSIBLE_SERIALIZED_MESSAGE_LENGTH)
			//{
			//	throw new InvalidMessageException("Error parsing message: The maximum possible message size defined by the VRMeeting Messaging Protocol is " + ProtocolConfig.MAX_POSSIBLE_SERIALIZED_MESSAGE_LENGTH + " but the message supplied is " + message.Length + " bytes long");
			//}

			ServerSignals signal = extractSignal(message);
			switch (signal)
			{
			case ServerSignals.AUDI:
				return parseAUDIMessage(message);
			case ServerSignals.CHNG:
				return parseCHNGMessage(message);
			case ServerSignals.END:
				return parseENDMessage();
				//case TOKE:
				//    return parseTOKEMessage(message);
			case ServerSignals.AUTH:
				return parseAUTHMessage();
			case ServerSignals.VAL:
				return parseVALMessage();
			case ServerSignals.NVAL:
				return parseNVALMessage();
			case ServerSignals.MEET:
				return parseMEETMessage();
            case ServerSignals.UDM:
                return parseUDMMessage(message);
			case ServerSignals.EAUD:
				return parseEAUDMessage (message);
			case ServerSignals.LEFT:
				return parseLEFTMessage (message);
			default:
				throw new InvalidMessageException("Warning: A " + signal.ToString() + " message should not be received by the server");
			}
		}

		private static Message parseLEFTMessage(byte[] message)
		{
			byte[] payload = extractPayload(message);
			return new Message(ServerSignals.LEFT.ToString(), payload);
		}

        private static Message parseUDMMessage(byte[] message)
        {
            byte[] payload = extractPayload(message);
            return new Message(ServerSignals.UDM.ToString(), payload);
        }

        private static Message parseMEETMessage()
		{
			return new Message(ServerSignals.MEET.ToString(), null);
		}

		private static Message parseNVALMessage()
		{
			return new Message(ServerSignals.NVAL.ToString(), null);
		}

		private static Message parseVALMessage()
		{
			return new Message(ServerSignals.VAL.ToString(), null);
		}

		private static Message parseAUTHMessage()
		{
			return new Message(ServerSignals.AUTH.ToString(), null);
		}

		private static Message parseAUDIMessage(byte[] message)
		{
			byte[] payload = extractPayload(message);
			validateAUDIPayload(payload);
			return new Message(ServerSignals.AUDI.ToString(), payload);
		}

		private static Message parseEAUDMessage(byte[] message)
		{
			byte[] payload = extractPayload (message);
			return new Message (ServerSignals.EAUD.ToString (), payload);
		}

		/// <summary>
		/// Checks the byte array passed in to see if it is a valid AUDI message payload according to the VRMeeting Messaging Protocol </summary>
		/// <param name="payload"> The payload to check </param>
		/// <exception cref="InvalidMessageException"> Thrown if the payload is invalid and contains the reason </exception>
		/// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>

		private static void validateAUDIPayload(byte[] payload)
		{
			if (!(payload.Length > 0))
			{
				throw new InvalidMessageException("Error Parsing AUDI Message: The payload of an AUDI message cannot be empty");
			}
//			if (!(payload.Length % ProtocolConfig.BYTES_PER_AUDIO_SAMPLE == 0))
//			{
//				throw new InvalidMessageException("Error Parsing AUDI Message: The payload length isn't a multiple of the number of bytes per audio sample. Payload length: " + payload.Length + ", bytes per audio samples: " + ProtocolConfig.BYTES_PER_AUDIO_SAMPLE);
//			}
		}


		private static Message parseCHNGMessage(byte[] message)
		{
			byte[] payload = extractPayload(message);
			validateCHNGPayload(payload);
			return new Message(ServerSignals.CHNG.ToString(), payload);
		}

		/// <summary>
		/// Checks the byte array passed in to see if it is a valid CHNG message payload according to the VRMeeting Messaging Protocol </summary>
		/// <param name="payload"> The payload to check </param>
		/// <exception cref="InvalidMessageException"> Thrown if the payload is invalid and contains the reason </exception>
		/// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>

		private static void validateCHNGPayload(byte[] payload)
		{
			//Debug.Log(System.Text.Encoding.UTF8.GetString(payload));
			if (!(payload.Length > 0))
			{
				throw new InvalidMessageException("Error Parsing CHNG Message: The payload cannot be empty");
			}

			if (payload.Length != ProtocolConfig.BYTES_PER_SLIDE_NUMBER)
			{
				throw new InvalidMessageException("Error Parsing CHNG Message: The payload must be " + ProtocolConfig.BYTES_PER_SLIDE_NUMBER + " bytes long but was " + payload.Length + " bytes long");
			}

			//short slideNumber = Net.ByteBuffer.ReadShort(payload);
			ByteBuffer buffer = new ByteBuffer(payload);
			int slideNumber = buffer.ReadInt();

//			if (slideNumber == 0)
//			{
//				throw new InvalidMessageException("Error Parsing CHNG Message: The slide number cannot be 0");
//			}
			if (slideNumber < 0)
			{
				throw new InvalidMessageException("Error Parsing CHNG Message: The slide number cannot be negative");
			}
		}

		private static Message parseENDMessage()
		{
			return new Message(ServerSignals.END.ToString(), null);
		}


		/// <summary>
		/// Attempts to separate the payload from the signal in the raw message </summary>
		/// <param name="message"> The raw message to extract the payload from </param>
		/// <returns> The byte array that represents the payload of the message </returns>
		/// <exception cref="InvalidMessageException"> If the message does not contain a <seealso cref="vrmeeting.config.protocol.ProtocolConfig#END_OF_SIGNAL_DELIMITER"/> </exception>

		private static byte[] extractPayload(byte[] message)
		{


			//Debug.Log(System.Text.Encoding.Default.GetString(message));

			//ByteBuffer buffer = new ByteBuffer(message);
			//string messageAsString = buffer.ReadString();
			//ushort slideNumber = buffer.ReadShort();
			//Debug.Log(slideNumber);

			int delimiterIndex = findSignalDelimiterIndex(message);
			int payloadStart = delimiterIndex + Encoding.UTF8.GetByteCount(ProtocolConfig.END_OF_SIGNAL_DELIMITER);
			//Debug.Log(Encoding.UTF8.GetByteCount("ASDD"));
			//Debug.Log(System.Text.Encoding.Default.GetString(message));




			//Since 1 character = 1 byte when using ASCII
			byte[] payload = new byte[message.Length - payloadStart];
			Array.Copy(message, payloadStart, payload, 0, payload.Length);

			//ByteBuffer buffer = new ByteBuffer(payload);
			//ushort slideNumber = buffer.ReadShort();
			//Debug.Log(message.Length);
			//Debug.Log(payloadStart + "     " + payload.Length);

			return payload;
		}

		/// <summary>
		/// Attempts to decode the signal in the message and convert it to the appropriate <seealso cref="vrmeeting.messages.signals.Signals"/> </summary>
		/// <param name="message"> The raw message to extract the signal from </param>
		/// <returns> Returns the <seealso cref="vrmeeting.messages.signals.Signals"/> that was encoded into the message </returns>
		/// <exception cref="InvalidMessageException"> If there is an error with the format of the message such that the signal cannot be extracted </exception>

		private static ServerSignals extractSignal(byte[] message)
		{

			int delimiterIndex = findSignalDelimiterIndex(message);

			ByteBuffer buffer = new ByteBuffer(message);    
			string stringSignal = buffer.ReadString(delimiterIndex); 

			//string messageAsString = StringHelper.NewString(message);
			//string str = System.Text.Encoding.Default.GetString(message);
			//Debug.Log(str);
			//Get the string that represents the signal


			//string stringSignal = messageAsString.Substring(0, delimiterIndex);
			//Debug.Log(stringSignal);
			try
			{
				//ServerSignals signal = ServerSignals.valueOf(stringSignal);
				//ServerSignals signal;
				ServerSignals signal = (ServerSignals)Enum.Parse(typeof(ServerSignals), stringSignal, true);
				return signal;
			}
			catch (System.ArgumentException)
			{
				//If the string doesn't represent any of the valid signals
				throw new InvalidMessageException("Error parsing message: Unknown signal '" + stringSignal + "'");
			}
		}

		/// <summary>
		/// Attempts to find the index of the first character of the <seealso cref="vrmeeting.config.protocol.ProtocolConfig#END_OF_SIGNAL_DELIMITER"/> in a raw message </summary>
		/// <param name="message"> The message to search </param>
		/// <returns> The index of the first character of the <seealso cref="vrmeeting.config.protocol.ProtocolConfig#END_OF_SIGNAL_DELIMITER"/> in the raw message </returns>
		/// <exception cref="InvalidMessageException"> if the message does not contain a <seealso cref="vrmeeting.config.protocol.ProtocolConfig#END_OF_SIGNAL_DELIMITER"/> </exception>

		private static int findSignalDelimiterIndex(byte[] message)
		{
			//First convert the whole message to a string as the signal and signal delimiter are encoded in ASCII
			//string messageAsString = StringHelper.NewString(message);

			//ByteBuffer buffer = new ByteBuffer(message);
			//int len = buffer.ReadShort();  
			//string messageAsString = buffer.ReadString();
			string messageAsString = Encoding.UTF8.GetString(message);
			int delimiterIndex = messageAsString.IndexOf(ProtocolConfig.END_OF_SIGNAL_DELIMITER);
			if (delimiterIndex == -1)
			{
				throw new InvalidMessageException("Signal End Delimiter not found");
			}
			return delimiterIndex;
		}

	}

}