using System;
using System.Collections.Generic;

public interface ITransceiver
{
	void Broadcast();
	void Release();
}

public interface ITransceiver<T> : ITransceiver where T : struct, IMsg
{
	void AddListener(Action<T> handler);
	void RemoveListener(Action<T> handler);
	void Broadcast(T msg);
}

public interface ITransceiverManager
{
	ITransceiver<T> GetTransceiver<T>() where T : struct, IMsg;
	void Release();
}

public class TransceiverAgent<T>: ITransceiver<T> where T : struct, IMsg
{
	private List<Action<T>> _action = new List<Action<T>>();
	public void AddListener(Action<T> handler)
	{
		_action.Add(handler);
	}
	public void RemoveListener(Action<T> handler)
	{
		_action.Remove(handler);
	}
	public void Broadcast(T msg)
	{
		foreach (var action in _action)
		{
			action(msg);
		}
	}
	public void Broadcast()
	{
		foreach(var action in _action)
		{
			action.Invoke(default(T));
		}
	}
	public void Release()
	{
		_action.Clear();
	}
}

