using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMsgTransceiverMaanager: MonoBehaviour, ITransceiverManager
{
	private Dictionary<Type, ITransceiver> _transceiver = new Dictionary<Type, ITransceiver>();

	private void Awake()
	{
		MessageTransceiver.SetTransceiverManager(this);
	}

	private void OnDestroy()
	{
		MessageTransceiver.Release();
	}

	public void Release()
	{
		_transceiver.Clear();
	}

	public ITransceiver<TMsg> GetTransceiver<TMsg>() where TMsg : struct, IMsg
	{
		if (_transceiver.ContainsKey(typeof(TMsg)))
		{
			return (ITransceiver<TMsg>)_transceiver[typeof(TMsg)];
		}
		TransceiverAgent<TMsg> subTransceiver = new TransceiverAgent<TMsg>();
		_transceiver[typeof(TMsg)] = subTransceiver;
		return subTransceiver;
	}
}