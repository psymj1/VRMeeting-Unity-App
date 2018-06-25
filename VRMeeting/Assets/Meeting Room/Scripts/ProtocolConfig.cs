using System.Text;

namespace vrmeeting.config.protocol
{

    /// <summary>
    /// A class used to store the properties relating to the VRMeeting Messaging Protocol
    /// Final so that it cannot be extended
    /// @author Psymj1 (Marcus) </summary>
    /// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
    public sealed class ProtocolConfig
    {
        /// <summary>
        /// Private constructor so that it cannot be instantiated
        /// </summary>
        private ProtocolConfig()
        {
        }

        /// <summary>
        /// Represents the character(s) which will denote the end of the signal in a message
        /// </summary>
        public const string END_OF_SIGNAL_DELIMITER = "\n";

        /// <summary>
        /// The name of the character set used to encode the signal of a message as defined by the VRMeeting Messaging Protocol </summary>
        /// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
        //public const string SIGNAL_CHARACTER_SET_NAME = "UTF-8";

        /// <summary>
        /// The Java CharacterSet that is referenced by the <seealso cref="#SIGNAL_CHARACTER_SET_NAME"/>
        /// </summary>
        public static readonly Encoding SIGNAL_CHARACTER_SET = Encoding.UTF8;
        //Encoding utf8 = Encoding.UTF8;
        /// <summary>
        /// The name of the character set used to encode the authentication token of a user as defined by the VRMeeting Messaging Protocol </summary>
        /// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
        //public const string AUTHENTICATION_TOKEN_CHARACTER_SET_NAME = "UTF-8";

        /// <summary>
        /// The Java CharacterSet that is referenced by the <seealso cref="#AUTHENTICATION_TOKEN_CHARACTER_SET_NAME"/>
        /// </summary>
        public static readonly Encoding AUTHENTICATION_TOKEN_CHARACTER_SET = Encoding.UTF8;

        /// <summary>
        /// The number of bytes used to represent 1 character in the <seealso cref="#SIGNAL_CHARACTER_SET"/>
        /// </summary>
        public const int BYTES_PER_SIGNAL_CHARACTER = 4;

        /// <summary>
        /// The <seealso cref="#END_OF_SIGNAL_DELIMITER"/> Encoded using <seealso cref="#SIGNAL_CHARACTER_SET"/>
        /// </summary>
        public static readonly byte[] ENCODED_END_OF_SIGNAL_DELIMITER = SIGNAL_CHARACTER_SET.GetBytes(END_OF_SIGNAL_DELIMITER);

        /// <summary>
        /// The maximum number of characters that can be used to define a 'signal' as defined by the VRMeeting Messaging Protocol </summary>
        /// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
        public const int MAX_SIGNAL_LENGTH = 4; //In characters

        /// <summary>
        /// The maximum possible number of bytes that a signal could consist of assuming it is <seealso cref="#MAX_SIGNAL_LENGTH"/> characters long
        /// Defined as <seealso cref="#MAX_SIGNAL_LENGTH"/> * <seealso cref="#BYTES_PER_SIGNAL_CHARACTER"/>
        /// </summary>
        public static readonly int MAX_BYTES_PER_SIGNAL = MAX_SIGNAL_LENGTH * BYTES_PER_SIGNAL_CHARACTER; //In Bytes

        /// <summary>
        /// The maximum serialized signal length is defined as the <seealso cref="#MAX_BYTES_PER_SIGNAL"/> + The length of the <seealso cref="#ENCODED_END_OF_SIGNAL_DELIMITER"/>
        /// </summary>
        public static readonly int MAX_SERIALIZED_SIGNAL_LENGTH = MAX_BYTES_PER_SIGNAL + Encoding.UTF8.GetByteCount(END_OF_SIGNAL_DELIMITER); //In Bytes

        /// <summary>
        /// The maximum size of the payload that can be sent and received as denoted by the VRMeeting Messaging Protocol </summary>
        /// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
        public const int MAX_SERIALIZED_PAYLOAD_SIZE = 1000; //In Bytes

        /// <summary>
        /// The maximum possible serialized message length is defined as the sum of the <seealso cref="#MAX_SERIALIZED_SIGNAL_LENGTH"/> + the <seealso cref="#MAX_SERIALIZED_PAYLOAD_SIZE"/>
        /// </summary>
        public static readonly int MAX_POSSIBLE_SERIALIZED_MESSAGE_LENGTH = MAX_SERIALIZED_SIGNAL_LENGTH + MAX_SERIALIZED_PAYLOAD_SIZE; //In Bytes

        /// <summary>
        /// Represents the number of bytes per audio sample in an audio transmission message. Used to check the integrity of messages claiming to be transmitting audio
        /// </summary>
        public const int BYTES_PER_AUDIO_SAMPLE = 4; //In Bytes

        /// <summary>
        /// The number of bytes used to define the slide number in a CHNG message
        /// </summary>
        public static readonly int BYTES_PER_SLIDE_NUMBER = sizeof(int);

    }
}