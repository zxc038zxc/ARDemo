using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARTapToPlaceObject : InitializeMonoBehaviour
{

	[SerializeField]
	private ARRaycastManager _arRaycastMgr;
	[SerializeField]
	private InputController _inputController;

	[Header("需要擺放的模型"), SerializeField]
	private CarComponent _objectToPlaceSample;
	[Header("準星模型"), SerializeField]
	private GameObject _placementIndicator;

	private bool _isPlaceObject = false;
	private Pose _placementPose;
	private bool _canPlacementPose;
	private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
	private Vector2 _screenCenter;
	private CarComponent _currentCarComp;

	public override void Initialize()
	{
		MessageTransceiver<OpenDoorMsg>.AddListener(OnOpenDoor);
		MessageTransceiver<ResetMsg>.AddListener(OnResetAll);
		MessageTransceiver<ChangeCarColorMsg>.AddListener(OnChangeColor); 
		
		_screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
	}
	public override void Deinitialize()
	{
		MessageTransceiver<OpenDoorMsg>.AddListener(OnOpenDoor);
		MessageTransceiver<ResetMsg>.RemoveListener(OnResetAll);
		MessageTransceiver<ChangeCarColorMsg>.RemoveListener(OnChangeColor);
	}

	private void Update()
	{
		if (_isPlaceObject || _inputController.IsTouchUI)
		{
			return;
		}

		UpdatePlacementPose();
		UpdatePlacementIndicator();

		if (_canPlacementPose && Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				PlaceObject();
			}
		}
	}
	private void UpdatePlacementPose()
	{
		_arRaycastMgr.Raycast(_screenCenter, _hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

		_canPlacementPose = _hits.Count > 0;
		if (_canPlacementPose)
		{
			_placementPose = _hits[0].pose;

			var cameraForward = Camera.main.transform.forward;
			var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
			_placementPose.rotation = Quaternion.LookRotation(cameraBearing);
		}
	}


	// 更新展示位置指示器
	private void UpdatePlacementIndicator()
	{
		if (_canPlacementPose)
		{
			_placementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
		}
	}

	[ContextMenu("Test")]
	private void PlaceObject()
	{
		_currentCarComp = UnitySpawnPool.Instance.SpawnGameObj(_objectToPlaceSample);
		_currentCarComp.transform.SetParent(transform);
		_currentCarComp.transform.localPosition = _placementPose.position;
		_currentCarComp.transform.localRotation = _placementPose.rotation;
		_currentCarComp.transform.localScale = Vector3.one;

		_isPlaceObject = true;
		_placementIndicator.SetActive(false);

		MessageTransceiver<PlaceObjectMsg>.Broadcast(new PlaceObjectMsg()
		{
			IsPlace = true,
		});
	}

	private void OnOpenDoor(OpenDoorMsg msg)
	{
		if (_currentCarComp != null)
		{
			_currentCarComp.OpenOrCloseDoor();
		}
	}

	private void OnResetAll(ResetMsg msg)
	{
		_isPlaceObject = false;
		_placementIndicator.SetActive(true);

		if (_currentCarComp != null)
		{
			UnitySpawnPool.Instance.Despawn(_currentCarComp.gameObject);
		}
	}

	private void OnChangeColor(ChangeCarColorMsg msg)
	{
		if (_currentCarComp != null)
		{
			_currentCarComp.ChangeColor(msg.ChangeColor);
		}
	}
}
