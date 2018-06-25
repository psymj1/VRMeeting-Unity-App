using System.Collections;
using System.Collections.Generic;
using System.Threading;
using vrmeeting.protocol;

/*
 * Acts as a synchronized buffer for messages. You can Output a message to the outlet and it will be stored in an internal buffer. Then you can call IsOutletEmpty() to check if a message is in the buffer and 
 * then use GetNextMessage to retrieve the next message from the buffer. It stores the messages in a queue so the first message in is the first message read out
 */
public class MessageOutlet{
	private Mutex m_bufferMutex;
	private Mutex m_filterMutex;

	private List<string> m_filter;
	private List<Message> m_internalBuffer;

	public MessageOutlet()
	{
		m_filter = new List<string> ();
		m_internalBuffer = new List<Message> ();
		m_bufferMutex = new Mutex ();
		m_filterMutex = new Mutex ();
	}

	/*
	 * Add a message to the buffer of this output so that it can be read by something else
	 */
	public void OutputMessage(Message message)
	{
		m_bufferMutex.WaitOne ();
		m_internalBuffer.Add (message);
		m_bufferMutex.ReleaseMutex ();
	}

	public void OutputMessages(Message[] messages)
	{
		m_bufferMutex.WaitOne ();
		m_internalBuffer.AddRange (messages);
		m_bufferMutex.ReleaseMutex ();
	}

	/*
	 * Returns true if there are currently no messages stored in the outlet's internal buffer
	 */
	public bool IsOutletEmpty()
	{
		bool temp = false;
		m_bufferMutex.WaitOne ();
		temp = m_internalBuffer.Count == 0;
		m_bufferMutex.ReleaseMutex ();
		return temp;
	}

	/*
	 * Get the next message in the internal buffer. Returns null if there are no messages to read
	 */
	public Message GetNextMessage()
	{
		if (IsOutletEmpty ()) {
			return null;
		} else {
			Message temp;
			m_bufferMutex.WaitOne ();
			temp = m_internalBuffer.GetRange (0, 1)[0];
			m_internalBuffer.RemoveRange (0, 1);
			m_bufferMutex.ReleaseMutex ();
			return temp;
		}
	}

	/**
	 * Add a new string to the filter.
	 */
	public void AddSignalToMessageFilter(string filter)
	{
		m_filterMutex.WaitOne ();
		m_filter.Add (filter);
		m_filterMutex.ReleaseMutex ();
	}

	/*
	 * Removes the signal from the filter if it exists in the filter
	 */
	public void RemoveSignalFromMessageFilter(string signal)
	{
		m_filterMutex.WaitOne ();
		m_filter.Remove (signal);
		m_filterMutex.ReleaseMutex ();
	}

	/**
	 * Checks if the string passed into the function is in the list of strings to allow (The filter)
	 * Returns true if the filter contains a string identical to the signal
	 */
	public bool IsSignalInFilter(string signal)
	{
		bool inFilter = false;
		m_filterMutex.WaitOne ();
		inFilter = m_filter.Contains (signal);
		m_filterMutex.ReleaseMutex ();
		return inFilter;
	}
}
