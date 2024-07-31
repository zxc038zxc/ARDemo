using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARRuler : MonoBehaviour
{
	[SerializeField]
	private ARRaycastManager _arRaycastMgr;
	[SerializeField]
	private RulerDrawer _drawer;

	private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
	private bool _started = false;

	private void Update()
	{
		if(_started==false)
		{
			if(Input.GetMouseButtonDown(0))
			{
				_started = true;
				SetStartPoint();
			}
		}
		else
		{
			if(Input.GetMouseButtonDown(0))
			{
				SetEndPoint();
			}

			if(Input.GetMouseButtonUp(0))
			{
				_started=false;
			}
		}
	}

	private void SetStartPoint()
	{
		if(GetRaycastPoint(out Vector3 worldPos))
		{
			_drawer.SetStart(worldPos);
		}
	}

	private void SetEndPoint()
	{
		if (GetRaycastPoint(out Vector3 worldPos))
		{
			_drawer.SetEnd(worldPos);
		}
	}

	private bool GetRaycastPoint(out Vector3 worldPos)
	{
		Vector2 screenPos = Input.mousePosition;
		bool hasHit = _arRaycastMgr.Raycast(screenPos, _hits, TrackableType.PlaneWithinPolygon);

		if(!hasHit || _hits.Count==0)
		{
			worldPos = Vector3.zero;
			return false;
		}

		worldPos = _hits[0].pose.position;
		return true;
	}
}
