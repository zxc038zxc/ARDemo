using UnityEngine;
using UnityEngine.UI;

public class UIManager : InitializeMonoBehaviour
{
	[SerializeField]
	private Button _openDoorBtn;
	[SerializeField]
	private Button _cancelBtn;
	[SerializeField]
	private GameObject _colorBtnPanel;

	public override void Initialize()
	{
		MessageTransceiver<PlaceObjectMsg>.AddListener(OnPlaceObjectMsg);
		_openDoorBtn.onClick.AddListener(OnOpenDoorBtnClick);
		_cancelBtn.onClick.AddListener(OnCancelBtnClick);
	}

	public override void Deinitialize()
	{
		MessageTransceiver<PlaceObjectMsg>.RemoveListener(OnPlaceObjectMsg);
		_openDoorBtn.onClick.RemoveListener(OnOpenDoorBtnClick);
		_cancelBtn.onClick.RemoveListener(OnCancelBtnClick);
	}

	private void OnOpenDoorBtnClick()
	{
		MessageTransceiver<OpenDoorMsg>.Broadcast();
	}

	private void OnCancelBtnClick()
	{
		MessageTransceiver<ResetMsg>.Broadcast();
		_cancelBtn.interactable = false;
		_colorBtnPanel.SetActive(false);
	}

	private void OnPlaceObjectMsg(PlaceObjectMsg obj)
	{
		_cancelBtn.interactable = true;
		_colorBtnPanel.SetActive(true);
	}
}
