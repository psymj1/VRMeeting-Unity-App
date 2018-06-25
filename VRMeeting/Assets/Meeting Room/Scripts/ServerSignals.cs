namespace vrmeeting.protocol.signals
{ 
	/// <summary>
	/// Contains signals that will be in messages sent from the server to a client based on the VRMeeting Messaging Protocol
	/// @author Psymj1 (Marcus) </summary>
	/// <seealso cref= <a href="https://github.com/psymj1/VRMeeting-Documentation">VRMeeting Messaging Protocol</a> </seealso>
	public enum ServerSignals
	{
		AUTH,
		VAL,
		NVAL,
		CHNG,
		AUDI,
		END,
		MEET,
        UDM,
		EAUD,
		LEFT
	}
}
