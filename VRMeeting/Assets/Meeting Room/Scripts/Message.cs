using System;

namespace vrmeeting.protocol
{
    using ProtocolConfig = vrmeeting.config.protocol.ProtocolConfig;

    /// <summary>
    /// This class models a 'message' sent during the VRMeeting Messaging Protocol.
    /// It contains a 'signal' which indicates the purpose of the message and a 'payload' which is the content of the message itself.
    /// The message 'signal' denotes the format of the message 'payload'
    /// @author Psymj1 (Marcus) </summary>
    /// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
    public class Message
    {
        private string signal;
        private byte[] payload;

        /// <summary>
        /// Creates a new Message
        /// Adheres to the standard defined by the VRMeeting Messaging Protocol </summary>
        /// <param name="signal"> Must be less than {@value vrmeeting.config.protocol.ProtocolConfig#MAX_SIGNAL_LENGTH} characters Long. Denotes the purpose of the message and the format of the 'payload' </param>
        /// <param name="payload"> Must be less than {@value vrmeeting.config.protocol.ProtocolConfig#MAX_SERIALIZED_PAYLOAD_SIZE} bytes long. The data to be sent in the message. </param>
        /// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
        public Message(string signal, byte[] payload)
        {
            if (string.ReferenceEquals(signal, null))
            {
                throw new System.ArgumentException("Signal cannot be null");
            }
            if (signal.Equals(""))
            {
                throw new System.ArgumentException("Signal cannot be empty");
            }

//            if (payload != null && payload.Length > ProtocolConfig.MAX_SERIALIZED_PAYLOAD_SIZE)
//            {
//                throw new System.ArgumentException("Payload is too long. Expected Payload Length < " + ProtocolConfig.MAX_SERIALIZED_PAYLOAD_SIZE + ". Actual length was (" + payload.Length + ")");
//            }

            if (signal.Length > ProtocolConfig.MAX_SIGNAL_LENGTH)
            {
                throw new System.ArgumentException("Signal is too long. Expected Signal Length < " + ProtocolConfig.MAX_SIGNAL_LENGTH + ". Actual length was (" + signal.Length + ")");
            }

            this.signal = signal;
            this.payload = (payload == null ? null : (payload.Length > 0 ? payload : null));
        }

        private int PayloadLength
        {
            get
            {
                return payload == null ? 0 : payload.Length;
            }
        }

        /// <summary>
        /// Converts the signal and payload stored in the message to a valid form as defined by the VRMeeting Messaging Protocol </summary>
        /// <returns> Returns a byte array which represents the data the signal and payload stored in the object </returns>
        /// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
        public virtual byte[] TransmittableMessage
        {
            get
            {
                int lengthOfPayload = PayloadLength;
                byte[] signalAbyteArray = System.Text.Encoding.UTF8.GetBytes(signal);

                byte[] message = new byte[signalAbyteArray.Length + ProtocolConfig.ENCODED_END_OF_SIGNAL_DELIMITER.Length + lengthOfPayload];
                int offset = 0;

                Array.Copy(signalAbyteArray, 0, message, offset, signalAbyteArray.Length);
                offset += signalAbyteArray.Length;
                Array.Copy(ProtocolConfig.ENCODED_END_OF_SIGNAL_DELIMITER, 0, message, offset, ProtocolConfig.ENCODED_END_OF_SIGNAL_DELIMITER.Length);
                offset += ProtocolConfig.ENCODED_END_OF_SIGNAL_DELIMITER.Length;

                if (payload != null)
                {
                    Array.Copy(payload, 0, message, offset, payload.Length);
                }

                return message;
            }
        }

        public virtual string Signal
        {
            get
            {
                return signal;
            }
        }

        public virtual byte[] Payload
        {
            get
            {
                return payload;
            }
        }
    }
}