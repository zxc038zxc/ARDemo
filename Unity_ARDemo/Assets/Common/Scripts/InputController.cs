using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
	public Action<Vector2> RotateByTouchHandler;
	public Action<float> ScaleByTouchHandler;

	[SerializeField]
	private float _ignoreScaleDist;

	public bool IsTouchUI
	{
		get { return _isTouchUI; }
	}

	private bool _isTouchUI;
	private float _twoPointStartDist;

	private void Update()
	{
		_isTouchUI = IsTouchingUI();

		if (_isTouchUI)
		{
			return;
		}

		if (Input.touchCount == 1)
		{
			var touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Moved)
			{
				RotateByTouchHandler?.Invoke(touch.deltaPosition);
			}
		}
		else if (Input.touchCount == 2)
		{
			var touch1 = Input.GetTouch(0);
			var touch2 = Input.GetTouch(1);
			if (touch2.phase == TouchPhase.Began)
			{
				_twoPointStartDist = Vector2.Distance(touch1.position, touch2.position);
			}
			else
			{
				var twoPointNewDist = Vector2.Distance(touch1.position, touch2.position);
				var dist = twoPointNewDist - _twoPointStartDist;

				if (Math.Abs(dist) > _ignoreScaleDist)
				{
					ScaleByTouchHandler?.Invoke(twoPointNewDist - _twoPointStartDist);
					_twoPointStartDist = twoPointNewDist;
				}
			}
		}
	}


	private bool IsTouchingUI()
	{
		if (UnityEngine.EventSystems.EventSystem.current == null) return false;

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.touchCount < 1) return false;
			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
		}
		else
		{
			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return true;
		}

		return false;
	}
}
