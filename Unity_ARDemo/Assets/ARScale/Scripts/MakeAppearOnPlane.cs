using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

public class MakeAppearOnPlane : MonoBehaviour
{
	[SerializeField, Tooltip("應該出現在觸摸點處的Transform")]
	private Transform _content;
	[SerializeField, Tooltip("內容應該呈現的旋轉角度")]
	private Quaternion _rotation;

	[SerializeField]
	private XROrigin _xrOrigin;
	[SerializeField]
	private ARRaycastManager _raycastMgr;

	private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

	public Quaternion Rotation
	{
		get { return _rotation; }
		set
		{
			_rotation = value;
			if(_xrOrigin!=null)
			{
				_xrOrigin.MakeContentAppearAt(_content, _content.transform.position, _rotation);
			}
		}
	}

	private void Update()
	{
		if(Input.touchCount ==0 || _content==null)
		{
			return;
		}

		var touch = Input.GetTouch(0);
		if (_raycastMgr.Raycast(touch.position, _hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
		{
			var hitPost = _hits[0].pose;
			_xrOrigin.MakeContentAppearAt(_content,hitPost.position, _rotation);
		}
	}
}
