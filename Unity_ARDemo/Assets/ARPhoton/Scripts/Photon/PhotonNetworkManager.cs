using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RaiseEventCode
{
	PlayerReadyCode,
}

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
	private static PhotonNetworkManager _instance;
	public static PhotonNetworkManager Instance
	{
		get
		{
			return _instance;
		}
	}

	private bool _isConnecting = false;

	private void Awake()
	{
		_instance = this;
	}
	private void OnDestroy()
	{
		if (_instance != null)
		{
			_instance = null;
		}
	}
	private void Update()
	{
		if (_isConnecting)
		{
			//GameController.Instance.UpdateConnectionStatus("Connection Status: " + PhotonNetwork.NetworkClientState);
		}
	}

	public void ConnectToServer()
	{
		PhotonNetwork.ConnectUsingSettings();
		//GameController.Instance.OpenConnectionStatus(true);
		_isConnecting = true;
	}

	public void JoinLobby()
	{
		PhotonNetwork.JoinLobby();
	}

	public void JoinRoom(string roomName)
	{
		PhotonNetwork.JoinRoom(roomName);
	}

	public void JoinRandomRoom()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	public void CreateRoom()
	{
		string roomName = Random.Range(0, 1000).ToString();
		RoomOptions roomOptions = new RoomOptions()
		{
			MaxPlayers = 2,
		};
		PhotonNetwork.CreateRoom(roomName, roomOptions);
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	#region Photon Callbacks
	public override void OnConnected()
	{
		Debug.Log("Connected to Internet.");
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master.");
		//GameController.Instance.OpenConnectionStatus(false);
		_isConnecting = false;

		JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby");
		InfoTransceiver<ChangeGameState>.Broadcast(new ChangeGameState
		{
			State = GameState.Lobby,
		});
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom: " + PhotonNetwork.CurrentRoom.Name);
		InfoTransceiver<ChangeGameState>.Broadcast(new ChangeGameState
		{
			State = GameState.Room,
		});
		InfoTransceiver<JoinRoomMsg>.Broadcast(new JoinRoomMsg
		{
			RoomID = PhotonNetwork.CurrentRoom.Name,
		});
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log(message);
		CreateRoom();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log(message);
		CreateRoom();
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log(message);
		CreateRoom();
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.Log($"OnPlayerJoin: PlayerName{newPlayer.NickName}");
		//GameController.Instance.CheckPlayersReady();
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		Debug.Log($"OnPlayerLeftRoom: PlayerName{otherPlayer.NickName}");
		//GameController.Instance.CheckPlayersReady();

		if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
		{
			PhotonNetwork.RaiseEvent((byte)RaiseEventCode.PlayerReadyCode, null, new RaiseEventOptions { CachingOption = EventCaching.RemoveFromRoomCache }, new ExitGames.Client.Photon.SendOptions { Reliability = false });
		}

		InfoTransceiver<PlayerReadyMsg>.Broadcast(new PlayerReadyMsg
		{
			PlayerID = otherPlayer.ActorNumber,
			Ready = false,
		});
	}

	public override void OnLeftRoom()
	{
		InfoTransceiver<ChangeGameState>.Broadcast(new ChangeGameState
		{
			State = GameState.Lobby,
		});
	}
	#endregion
}
