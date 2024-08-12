using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyManager : MonoBehaviour, IUIService
{
	[SerializeField]
	private GameObject _chooseRoomPanel;
	[SerializeField]
	private Button _joinRandomRoomBtn;
	[SerializeField]
	private Button _createRoomBtn;
	[SerializeField]
	private Button _joinSpecificRoomBtn;

	[SerializeField]
	private GameObject _joinSpecificRoomPanel;
	[SerializeField]
	private InputField _roomNameInputField;
	[SerializeField]
	private Button _joinRoomBtn;
	[SerializeField]
	private Button _caneclJoinRoomBtn;


	public void Init()
	{
		_joinRandomRoomBtn.onClick.AddListener(OnJoinRandomBtnClick);
		_createRoomBtn.onClick.AddListener(OnCreateRoomBtnClick);
		_joinSpecificRoomBtn.onClick.AddListener(OnJoinSpecificBtnClick);

		_joinRoomBtn.onClick.AddListener(OnJoinRoomBtnClick);
		_caneclJoinRoomBtn.onClick.AddListener(OnCaneclJoinRoomBtnClick);
	}

	public void Deinit()
	{
		_joinRandomRoomBtn.onClick.RemoveListener(OnJoinRandomBtnClick);
		_createRoomBtn.onClick.RemoveListener(OnCreateRoomBtnClick);
		_joinSpecificRoomBtn.onClick.RemoveListener(OnJoinSpecificBtnClick);

		_joinRoomBtn.onClick.RemoveListener(OnJoinRoomBtnClick);
		_caneclJoinRoomBtn.onClick.RemoveListener(OnCaneclJoinRoomBtnClick);
	}

	private void OnEnable()
	{
		_chooseRoomPanel.SetActive(true);
		_joinSpecificRoomPanel.SetActive(false);
	}

	private void OnJoinRandomBtnClick()
	{
		PhotonNetworkManager.Instance.JoinRandomRoom();
	}

	private void OnCreateRoomBtnClick()
	{
		PhotonNetworkManager.Instance.CreateRoom();
	}

	private void OnJoinSpecificBtnClick()
	{
		_joinSpecificRoomPanel.SetActive(true);
		_chooseRoomPanel.SetActive(false);
	}

	private void OnJoinRoomBtnClick()
	{
		string roomName = _roomNameInputField.text;
		if (!string.IsNullOrEmpty(roomName))
		{
			PhotonNetworkManager.Instance.JoinRoom(roomName);
			_joinSpecificRoomPanel.SetActive(false);
			_chooseRoomPanel.SetActive(false);
		}
	}

	private void OnCaneclJoinRoomBtnClick()
	{
		_joinSpecificRoomPanel.SetActive(false);
		_chooseRoomPanel.SetActive(true);
	}
}
