/// 
namespace vrmeeting.protocol
{
    using ClientSignals = vrmeeting.protocol.signals.ClientSignals;

    /// <summary>
    /// This class aims to provide additional power to the <seealso cref="vrmeeting.protocol.Message"/> Class Allowing the Authentication Token encoded into the payload to be extracted
    /// @author Psymj1 (Marcus)
    /// 
    /// </summary>
    public class AuthTokenMessage : Message
    {

        public AuthTokenMessage(byte[] payload) : base(ClientSignals.TOKE.ToString(), payload)
        {
        }

        public virtual string AuthenticationToken
        {
            get
            {
                return System.Text.Encoding.Default.GetString(Payload);
            }
        }
    }
}
