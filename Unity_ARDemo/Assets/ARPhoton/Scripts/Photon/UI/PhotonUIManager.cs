using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
	Login,
	Lobby,
	Room,
	Game,
}
public class PhotonUIManager : InitializeMonoBehaviour
{
	[SerializeField]
	private UILoginManager _loginUI;
	[SerializeField]
	private UILobbyManager _lobbyUI;
	[SerializeField]
	private UIRoomManager _roomUI;
	[SerializeField]
	private UIGameManager _gameUI;
	[SerializeField]
	private GameObject _roomIDObj;
	[SerializeField]
	private Text _roomIDText;

	public override void Initialize()
	{
		_loginUI.Init();
		_lobbyUI.Init();
		_roomUI.Init();
		_gameUI.Init();
	}

	public override void Deinitialize()
	{
		_loginUI.Deinit();
		_lobbyUI.Deinit();
		_roomUI.Deinit();
		_gameUI.Deinit();
	}

	public void OnChangeState(GameState state)
	{
		_loginUI.gameObject.SetActive(state == GameState.Login);
		_lobbyUI.gameObject.SetActive(state == GameState.Lobby);
		_roomUI.gameObject.SetActive(state == GameState.Room);
		_gameUI.gameObject.SetActive(state == GameState.Game);

		_roomIDObj.SetActive(state >= GameState.Room);
	}

	public void SetRoomID(string roomID)
	{
		_roomIDText.text = roomID;
	}
}
