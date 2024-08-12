using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ARGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Button _searchForGameBtn;
	[SerializeField]
	private GameObject _uiInfromPanelObj;
	[SerializeField]
	private TextMeshProUGUI _uiInformText;
	[SerializeField]
	private float _delayCloseInformPanelTime = 2f;

    void Awake()
    {
        _searchForGameBtn.onClick.AddListener(JoinRandomRoom);        
    }

	private void OnDestroy()
	{
		_searchForGameBtn.onClick.RemoveListener(JoinRandomRoom);
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public void OpenRoot(bool open)
	{
		_uiInfromPanelObj.SetActive(open);
		_uiInformText.text = "Search for games to play!";
	}

	#region UI Callback Methods
    private void JoinRandomRoom()
	{
		_uiInformText.text = "Search for available rooms...";
		PhotonNetwork.JoinRandomRoom();
		_searchForGameBtn.gameObject.SetActive(false);
	}

	#endregion

	#region PHOTON Callback Methods
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		_uiInformText.text = message;
		Debug.Log(message);
		CreateAndJoinRoom();
	}
	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
		{
			_uiInformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name + ", Waiting for other players...";
		}
		else
		{
			_uiInformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name; 
			StartCoroutine(DeactiveAfterSeconds(_uiInfromPanelObj, _delayCloseInformPanelTime));
		}

		Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
	}
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		_uiInformText.text = PhotonNetwork.NickName + "joined to " + PhotonNetwork.CurrentRoom.Name + ", Player count: " + PhotonNetwork.CurrentRoom.PlayerCount;

		Debug.Log(PhotonNetwork.NickName +  "joined to " + PhotonNetwork.CurrentRoom.Name + ", Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);

		StartCoroutine(DeactiveAfterSeconds(_uiInfromPanelObj, _delayCloseInformPanelTime));
	}

	#endregion

	private void CreateAndJoinRoom()
	{
		string randomRoomName = "Room" + Random.Range(0, 10);
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 2;

		PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
	}

	IEnumerator DeactiveAfterSeconds(GameObject obj, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		obj.SetActive(false);
	}
}
