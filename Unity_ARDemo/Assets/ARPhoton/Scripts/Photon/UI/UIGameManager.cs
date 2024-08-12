using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGameManager : MonoBehaviour, IUIService
{
	[SerializeField]
	private GameObject _infoPanel;
	[SerializeField]
	private GameObject _settleUpPanel;
	[SerializeField]
	private Button _leaveRoomBtn;
	[SerializeField]
	private Text _settleUpText;


	public void Init()
	{
		InfoTransceiver<ChangeTurnMsg>.AddListener(OnChangeTurn);
		InfoTransceiver<EndGameMsg>.AddListener(OnEndGame);
		_leaveRoomBtn.onClick.AddListener(OnLeaveBtnClick);
	}

	public void Deinit()
	{
		InfoTransceiver<ChangeTurnMsg>.RemoveListener(OnChangeTurn);
		InfoTransceiver<EndGameMsg>.RemoveListener(OnEndGame);
		_leaveRoomBtn.onClick.RemoveListener(OnLeaveBtnClick);
	}

	private void OnEnable()
	{
		_settleUpPanel.SetActive(false);
		_infoPanel.SetActive(false);
	}

	private void OnChangeTurn(ChangeTurnMsg msg)
	{
		_infoPanel.SetActive(!msg.IsSelfTurn);
	}
	private void OnLeaveBtnClick()
	{
		InfoTransceiver<LeaveRoomMsg>.Broadcast(new LeaveRoomMsg
		{
			PlayerID = PhotonNetwork.LocalPlayer.ActorNumber
		});
	}

	private void OnEndGame(EndGameMsg msg)
	{
		_infoPanel.SetActive(false);
		_settleUpPanel.SetActive(true);
		string text;
		if (msg.PlayerID == PhotonNetwork.LocalPlayer.ActorNumber && msg.IsWin)
		{
			text = "You Win";
		}
		else
		{
			text = "Lose";
		}
		_settleUpText.text = text;
	}
}

