using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityInfoTransceiverMaanager: InitializeMonoBehaviour, ITransceiverManager
{
	private Dictionary<Type, ITransceiver> _transceiver = new Dictionary<Type, ITransceiver>();

	public override void Initialize()
	{
		InfoTransceiver.SetTransceiverManager(this);
	}

	public override void Deinitialize()
	{
		InfoTransceiver.Release();
	}

	public void Release()
	{
		_transceiver.Clear();
	}

	public ITransceiver<T> GetTransceiver<T>() where T : struct, IInfo
	{
		if (_transceiver.ContainsKey(typeof(T)))
		{
			return (ITransceiver<T>)_transceiver[typeof(T)];
		}
		TransceiverAgent<T> subTransceiver = new TransceiverAgent<T>();
		_transceiver[typeof(T)] = subTransceiver;
		return subTransceiver;
	}
}