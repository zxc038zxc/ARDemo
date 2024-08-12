using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomManager : MonoBehaviour, IUIService
{
	public enum RoomStatus
	{
		Prepare,
		Modify,
		Ready,
	}

	[SerializeField]
	private PhotonView _pv;
	[SerializeField]
	private Button _placeBtn;
	[SerializeField]
	private Button _caneclBtn;
	[SerializeField]
	private Button _confirmBtn;
	[SerializeField]
	private Button _leaveBtn;
	[SerializeField]
	private GameObject _infoPanel;
	[SerializeField]
	private Text _infoText;

	public void Init()
	{
		InfoTransceiver<PlacePlaneCallback>.AddListener(OnPlacePlaneCallback);

		_placeBtn.onClick.AddListener(OnPlaceBtnClick);
		_caneclBtn.onClick.AddListener(OnCancelBtnClick);
		_confirmBtn.onClick.AddListener(OnConfirmBtnClick);
		_leaveBtn.onClick.AddListener(OnLeaveBtnClick);

		_leaveBtn.gameObject.SetActive(true);

		PhotonNetwork.NetworkingClient.EventReceived += OnReceivedEvent;
	}

	public void Deinit()
	{
		InfoTransceiver<PlacePlaneCallback>.RemoveListener(OnPlacePlaneCallback);

		_placeBtn.onClick.RemoveListener(OnPlaceBtnClick);
		_caneclBtn.onClick.RemoveListener(OnCancelBtnClick);
		_confirmBtn.onClick.RemoveListener(OnConfirmBtnClick);
		_leaveBtn.onClick.RemoveListener(OnLeaveBtnClick);

		PhotonNetwork.NetworkingClient.EventReceived -= OnReceivedEvent;
	}

	private void OnEnable()
	{
		UpdateStatus(RoomStatus.Prepare);
	}

	private void OnReceivedEvent(EventData eventData)
	{
		if(eventData.Code==(byte)RaiseEventCode.PlayerReadyCode)
		{
			object[] data = (object[])eventData.CustomData;
			int playerActorNum = (int)data[0];
			bool ready = (bool)data[1];

			Debug.Log($"Player:{playerActorNum}, is Ready:{ready}");
			InfoTransceiver<PlayerReadyMsg>.Broadcast(new PlayerReadyMsg
			{
				PlayerID = playerActorNum,
				Ready = ready,
			});
		}
	}

	private void OnPlacePlaneCallback(PlacePlaneCallback msg)
	{
		UpdateStatus(msg.IsPlace ? RoomStatus.Modify : RoomStatus.Prepare);
	}

	private void UpdateStatus(RoomStatus status)
	{
		if (status == RoomStatus.Prepare)
		{
			_placeBtn.gameObject.SetActive(true);
			_caneclBtn.gameObject.SetActive(false);
			_confirmBtn.gameObject.SetActive(false);
			_infoText.text = "Find the AR place and place the board";
		}
		else if (status == RoomStatus.Modify)
		{
			_placeBtn.gameObject.SetActive(false);
			_caneclBtn.gameObject.SetActive(true);
			_confirmBtn.gameObject.SetActive(true);
			_infoText.text = "Now you can modify your board's scale or rotation by point the screen";
		}
		else
		{
			_placeBtn.gameObject.SetActive(false);
			_caneclBtn.gameObject.SetActive(false);
			_confirmBtn.gameObject.SetActive(false);
			_infoText.text = "Great! Wait for other player ready";
		}
	}

	private void OnPlaceBtnClick()
	{
		InfoTransceiver<PlacePlaneMsg>.Broadcast(new PlacePlaneMsg
		{
			IsPlace = true,
		});
	}

	private void OnCancelBtnClick()
	{
		InfoTransceiver<PlacePlaneMsg>.Broadcast(new PlacePlaneMsg
		{
			IsPlace = false,
		});
	}

	private void OnLeaveBtnClick()
	{
		InfoTransceiver<LeaveRoomMsg>.Broadcast(new LeaveRoomMsg
		{
			PlayerID = PhotonNetwork.LocalPlayer.ActorNumber
		});
	}

	private void OnConfirmBtnClick()
	{
		UpdateStatus(RoomStatus.Ready);

		// 由於如果玩家進房先完成，後來進房地會收不到，所以這邊不用PunRPC
		//_pv.RPC("PlayerIsReady", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, true);

		object[] content = new object[]
		{
			PhotonNetwork.LocalPlayer.ActorNumber,
			true,
		};

		RaiseEventOptions options = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.AddToRoomCache,
		};

		SendOptions sendOptions = new SendOptions
		{
			Reliability = true,
		};

		PhotonNetwork.RaiseEvent((byte)RaiseEventCode.PlayerReadyCode, content, options, sendOptions);
	}

	//[PunRPC]
	//private void PlayerIsReady(int playerID, bool ready)
	//{
	//	InfoTransceiver<PlayerReadyMsg>.Broadcast(new PlayerReadyMsg
	//	{
	//		PlayerID = playerID,
	//		Ready = ready,
	//	});
	//}
}

