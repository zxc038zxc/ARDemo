using System;

public class MessageTransceiver
{
	protected static ITransceiverManager _mgr;
	public static void SetTransceiverManager(ITransceiverManager mgr)
	{
		if (_mgr != null)
		{
			Release();
		}
		_mgr = mgr;
	}

	public static void Release()
	{
		if (_mgr != null)
		{
			_mgr.Release();
			_mgr = null;
		}
	}
}

public class MessageTransceiver<T> : MessageTransceiver where T : struct, IMsg
{
	public static void AddListener(Action<T> handler)
	{
		var transcevier = _mgr.GetTransceiver<T>();
		transcevier.AddListener(handler);
	}
	public static void RemoveListener(Action<T> handler)
	{
		if (_mgr == null) return;

		var transcevier = _mgr.GetTransceiver<T>();
		transcevier.RemoveListener(handler);
	}
	public static void Broadcast()
	{
		var transcevier = _mgr.GetTransceiver<T>();
		transcevier.Broadcast();
	}
	public static void Broadcast(T msg)
	{
		var transcevier = _mgr.GetTransceiver<T>();
		transcevier.Broadcast(msg);
	}
}