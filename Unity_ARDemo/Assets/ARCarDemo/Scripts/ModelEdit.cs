using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ModelEdit : MonoBehaviour
{
	[SerializeField]
	private ARRaycastManager _arRaycastMgr;
	[SerializeField]
	private InputController _inputController;
	[SerializeField]
	private Camera _camera;
	[SerializeField]
	private LayerMask _hitLayer;

	[SerializeField]
	private float _rotateDelta = 0.2f;
	[SerializeField]
	private float _scaleDelta;

	private GameObject _currentSelectModel;

	private void Awake()
	{
		_inputController.RotateByTouchHandler += OnRotateObj;
		_inputController.ScaleByTouchHandler += OnScaleObj;
	}

	private void OnDestroy()
	{
		_inputController.RotateByTouchHandler -= OnRotateObj;
		_inputController.ScaleByTouchHandler -= OnScaleObj;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			IsClickingModel(Input.mousePosition);
		}
	}

	private void OnRotateObj(Vector2 deltaPos)
	{
		if (_currentSelectModel == null)
		{
			return;
		}

		_currentSelectModel.transform.Rotate(new Vector3(0, -deltaPos.x * _rotateDelta, 0), Space.World);
	}

	private void OnScaleObj(float dist)
	{
		if (_currentSelectModel == null)
		{
			return;
		}

		var delta = dist > 0 ? _scaleDelta : -_scaleDelta;
		_currentSelectModel.transform.localScale = _currentSelectModel.transform.localScale + Vector3.one * delta;
	}

	private bool IsClickingModel(Vector2 vector2)
	{
		Ray ray = Camera.main.ScreenPointToRay(vector2);
		RaycastHit hitInfo;

		bool isCollider = Physics.Raycast(ray, out hitInfo, 1000, _hitLayer);
		if (isCollider)
		{
			GameObject selected = hitInfo.transform.gameObject;
			_currentSelectModel = selected;
		}

		return isCollider;
	}
}
