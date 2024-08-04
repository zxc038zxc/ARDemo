using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeMBManager : InitializeMonoBehaviour
{
	private enum InitializeCode
	{
		Awake,
		OnEnable,
		Start,
		OnCall
	}

	[SerializeField]
	private InitializeCode _initializeCode;
	[SerializeField]
	private InitializeMonoBehaviour[] _initializableArray= null;

	public override void Initialize()
	{
		Initialize(InitializeCode.OnCall);
	}

	public override void Deinitialize()
	{
		Deinitialize(InitializeCode.OnCall);
	}

	private void Awake()
	{
		Initialize(InitializeCode.Awake);
	}
	private void OnEnable()
	{
		Initialize(InitializeCode.OnEnable);
	}
	private void Start()
	{
		Initialize(InitializeCode.Start);
	}

	private void OnDisable()
	{
		Deinitialize(InitializeCode.OnEnable);
	}

	private void OnDestroy()
	{
		Deinitialize(InitializeCode.Awake);
		Deinitialize(InitializeCode.Start);
	}
	private void Initialize(InitializeCode code)
	{
		if (_initializeCode == code)
		{
			int count = _initializableArray.Length;
			for (int i = 0; i < count; i++)
			{
				_initializableArray[i].Initialize();
			}
		}
	}

	private void Deinitialize(InitializeCode code)
	{
		if (_initializeCode == code)
		{
			int count = _initializableArray.Length;
			for (int i = 0; i < count; i++)
			{
				_initializableArray[i].Deinitialize();
			}
		}
	}
}
