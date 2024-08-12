using UnityEngine;
using UnityEngine.UI;

public class UILoginManager : MonoBehaviour, IUIService
{
	[SerializeField]
	private InputField _playerNameInput;
	[SerializeField]
	private Button _loginBtn;

	private ILoginService _loginService;

	public void Init()
	{
		_loginService = new PhotonLoginService();
		_loginBtn.onClick.AddListener(OnLoginBtnClicked);
	}

	public void Deinit()
	{
		_loginBtn.onClick.RemoveListener(OnLoginBtnClicked);
	}

	private void OnLoginBtnClicked()
	{
		string playerName = _playerNameInput.text;
		if (!string.IsNullOrEmpty(playerName))
		{
			_loginService.Login(playerName);
		}
	}
}
