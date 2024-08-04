using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class DetectionPlaneController: MonoBehaviour
{
	[SerializeField]
	private ARPlaneManager _arPlaneMgr;

	[SerializeField]
	private Toggle _toggle;
	[SerializeField]
	private Text _togglePlaneDetectionText;

	public void TogglePlaneDetection()
	{
		_arPlaneMgr.enabled = !_arPlaneMgr.enabled;
		
		string planeDetectionMsg = "";
		if(_arPlaneMgr.enabled)
		{
			planeDetectionMsg = "Disable PlaneDetection and Hide Existing";
			SetAllPlanesActive(true);
		}
		else
		{
			planeDetectionMsg = "Enable Plane Detection and Show Existing";
			SetAllPlanesActive(true);
		}

		if(_togglePlaneDetectionText!=null)
		{
			_togglePlaneDetectionText.text = planeDetectionMsg;
		}
	}

	private void SetAllPlanesActive(bool value)
	{
		foreach(var plane in _arPlaneMgr.trackables)
		{
			plane.gameObject.SetActive(value);
		}
	}

	public void SetDetectionHorizontal()
	{
		_arPlaneMgr.requestedDetectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Horizontal;
	}
	public void SetDetectionVertical()
	{
		_arPlaneMgr.requestedDetectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Vertical;
	}
	public void SetDetectionAll()
	{
		_arPlaneMgr.requestedDetectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Vertical | UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Horizontal;
	}
}
