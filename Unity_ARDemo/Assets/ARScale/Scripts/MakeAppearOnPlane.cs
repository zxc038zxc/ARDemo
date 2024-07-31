using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

public class MakeAppearOnPlane : MonoBehaviour
{
	[SerializeField, Tooltip("���ӥX�{�bĲ�N�I�B��Transform")]
	private Transform _content;
	[SerializeField, Tooltip("���e���ӧe�{�����ਤ��")]
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
