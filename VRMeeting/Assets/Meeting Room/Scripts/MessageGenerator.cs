namespace vrmeeting.protocol
{
    using ServerSignals = vrmeeting.protocol.signals.ServerSignals;

    /// <summary>
    /// A Utility Class used to generate <seealso cref="Message"/> objects that are sent from the client to a server
    /// @author Psymj1 (Marcus) </summary>
    /// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
    public sealed class MessageGenerator
    {
        /// <summary>
        /// Private Constructor so that the class cannot be initialised
        /// </summary>
        private MessageGenerator()
        {
        }

        /// <returns> Returns a <seealso cref="Message"/> whose signal is AUTH and payload is empty </returns>
        public static Message generateAuthenticationRequestMessage()
        {
            return new Message(ServerSignals.AUTH.ToString(), null);
        }

        /// <returns> Returns a <seealso cref="Message"/> whose signal is VAL and payload is empty </returns>
        public static Message generateValidatedMessage()
        {
            return new Message(ServerSignals.VAL.ToString(), null);
        }

        /// <returns> Returns a <seealso cref="Message"/> whose signal is NVAL and payload is empty </returns>
        public static Message generateNotValidatedMessage()
        {
            return new Message(ServerSignals.NVAL.ToString(), null);
        }
    }
}

