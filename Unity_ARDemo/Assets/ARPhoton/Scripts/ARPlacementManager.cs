using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlacementManager : MonoBehaviour
{
	[Header("要放置的物件"), SerializeField]
	private GameObject _boardAnchorSample;
	[Header("放置預測模型"), SerializeField]
	private GameObject _placementIndicator;
	[SerializeField]
	private Transform _jengaRoot;

	[SerializeField]
	private float _rotateDelta = 0.2f;
	[SerializeField]
	private Vector2 _scaleRange;
	[SerializeField]
	private float _scaleDelta;

	private bool _isPlaceObject = false;
	private Pose _placementPose;
	private bool _canPlacementPose;
	[SerializeField]
	private bool _isFindPlane = false;
	private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
	private Vector2 _screenCenter;
	private GameObject _currentBoard;
	private ARAnchor _currentAnchor;

	private ARRaycastManager _arRaycastMgr;
	private InputController _inputController;

	private bool _isWorking = false;

	public void Initialize(ARRaycastManager arRaycastMgr, InputController inputController)
	{
		_screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

		_arRaycastMgr = arRaycastMgr;
		_inputController = inputController;
		_inputController.RotateByTouchHandler += OnRotateObj;
		_inputController.ScaleByTouchHandler += OnScaleObj;
		_isFindPlane = false;
	}

	public void Deinitialize()
	{
		_inputController.RotateByTouchHandler -= OnRotateObj;
		_inputController.ScaleByTouchHandler -= OnScaleObj;
	}

	private void OnEnable()
	{
		OnPlaceObj(false);
		_isWorking = true;
		_isFindPlane = false;
	}

	private void OnDisable()
	{
		_isWorking = false;
	}

	private void Update()
	{
		if (_isPlaceObject || _inputController.IsTouchUI || !_isWorking)
		{
			return;
		}

		UpdatePlacementPose();
		UpdatePlacementIndicator();
	}

	public bool OnPlaceObj(bool place)
	{
		if (place)
		{
			if (_isPlaceObject)
			{
				Debug.LogError("Something error. Is placing obj");
			}
			else if (_isFindPlane)
			{
				_currentBoard = Instantiate(_boardAnchorSample, _placementPose.position, _placementPose.rotation, _jengaRoot);
				_currentBoard.transform.localScale = Vector3.one;
				_currentBoard.transform.localEulerAngles = new Vector3(_currentBoard.transform.localEulerAngles.x, 0, _currentBoard.transform.localEulerAngles.z);

				_isPlaceObject = true;
				_placementIndicator.SetActive(false);
				return true;
			}
		}
		else
		{
			if (_currentBoard != null)
			{
				// ARAnchor會自動幫我們刪掉他
				_currentBoard.gameObject.SetActive(false);
			}

			_isPlaceObject = false;
			_placementIndicator.SetActive(true);
		}
		return false;
	}

	public GameObject GetBoardObj()
	{
		return _currentBoard;
	}

	private void UpdatePlacementPose()
	{
		_arRaycastMgr.Raycast(_screenCenter, _hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);

		_canPlacementPose = _hits.Count > 0;
		if (_canPlacementPose)
		{
			_isFindPlane = true;
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

	private void OnRotateObj(Vector2 deltaPos)
	{
		if (_currentBoard == null && !_isWorking)
		{
			return;
		}

		_currentBoard.transform.Rotate(new Vector3(0, -deltaPos.x * _rotateDelta, 0), Space.World);
	}

	private void OnScaleObj(float dist)
	{
		if (_currentBoard == null && !_isWorking)
		{
			return;
		}

		var delta = dist > 0 ? _scaleDelta : -_scaleDelta;
		var scale = _currentBoard.transform.localScale.x + 1 * delta;
		scale = Mathf.Clamp(scale, _scaleRange.x, _scaleRange.y);
		_currentBoard.transform.localScale = Vector3.one * scale;
	}
}
