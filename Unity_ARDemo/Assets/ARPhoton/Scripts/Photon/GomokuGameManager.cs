using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GomokuGameManager : InitializeMonoBehaviour
{
	[SerializeField]
	private PhotonUIManager _uiMgr;
	[SerializeField]
	private ARPlacementManager _placeMgr;
	[SerializeField]
	private ARChessPieceController _chessController;
	[SerializeField]
	private ARRaycastManager _arRaycastMgr;
	[SerializeField]
	private InputController _inputController;

	private Dictionary<int, bool> _playerReadyDict = new Dictionary<int, bool>();
	
	public override void Initialize()
	{
		InfoTransceiver<ChangeGameState>.AddListener(OnChangeUIState);
		InfoTransceiver<PlacePlaneMsg>.AddListener(OnPlaceObj);
		InfoTransceiver<PlayerReadyMsg>.AddListener(OnPlayerReady);
		InfoTransceiver<JoinRoomMsg>.AddListener(OnSetJoinRoomID);
		InfoTransceiver<LeaveRoomMsg>.AddListener(OnLeaveRoom);
		InfoTransceiver<PlayerWinMsg>.AddListener(OnPlayerWin);

		_placeMgr.Initialize(_arRaycastMgr, _inputController);
		OnChangeUIState(GameState.Login);
	}
	public override void Deinitialize()
	{
		InfoTransceiver<ChangeGameState>.RemoveListener(OnChangeUIState);
		InfoTransceiver<PlacePlaneMsg>.RemoveListener(OnPlaceObj);
		InfoTransceiver<PlayerReadyMsg>.RemoveListener(OnPlayerReady);
		InfoTransceiver<JoinRoomMsg>.RemoveListener(OnSetJoinRoomID);
		InfoTransceiver<LeaveRoomMsg>.RemoveListener(OnLeaveRoom);
		InfoTransceiver<PlayerWinMsg>.RemoveListener(OnPlayerWin);

		_placeMgr.Deinitialize();
	}

	private void OnChangeUIState(ChangeGameState msg)
	{
		OnChangeUIState(msg.State);

	}

	private void OnChangeUIState(GameState state)
	{
		_uiMgr.OnChangeState(state);
		_placeMgr.gameObject.SetActive(state == GameState.Room);
		_chessController.gameObject.SetActive(state == GameState.Game);

		if (state == GameState.Game)
		{
			_chessController.SetBoard(_placeMgr.GetBoardObj());
		}
	}

	private void OnPlaceObj(PlacePlaneMsg msg)
	{
		bool place = _placeMgr.OnPlaceObj(msg.IsPlace);

		InfoTransceiver<PlacePlaneCallback>.Broadcast(new PlacePlaneCallback
		{
			IsPlace = place,
		});
	}

	private void OnSetJoinRoomID(JoinRoomMsg msg)
	{
		_uiMgr.SetRoomID(msg.RoomID);
	}

	private void OnLeaveRoom(LeaveRoomMsg msg)
	{
		if (msg.PlayerID == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			PhotonNetworkManager.Instance.LeaveRoom();
			_placeMgr.OnPlaceObj(false);
			_chessController.Release();
			OnChangeUIState(GameState.Lobby);
		}
	}

	private void OnPlayerWin(PlayerWinMsg msg)
	{
	}

	private void OnPlayerReady(PlayerReadyMsg msg)
	{
		Debug.Log($"Player:{msg.PlayerID} is ready:{msg.Ready}");
		if (_playerReadyDict == null)
		{
			_playerReadyDict = new Dictionary<int, bool>();
		}

		_playerReadyDict[msg.PlayerID] = msg.Ready;

		if (_playerReadyDict.Count > 0)
		{
			int readyCount = 0;
			foreach (var tmp in _playerReadyDict)
			{
				readyCount = tmp.Value ? readyCount + 1 : readyCount - 1;
			}

			if (readyCount >= 2)
			{
				OnChangeUIState(GameState.Game);
				Debug.Log("StartGame");
			}
		}
	}

	[ContextMenu("FakePlayerReady")]
	private void Test()
	{
		OnPlayerReady(new PlayerReadyMsg
		{
			PlayerID = 999,
			Ready = true,
		});
	}
}
