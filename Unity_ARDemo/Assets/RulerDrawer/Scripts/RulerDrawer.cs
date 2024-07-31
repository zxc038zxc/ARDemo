using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerDrawer : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _lineRenderer;
	[SerializeField]
	private TextMesh _infoText;

	[SerializeField]
	private bool _hasStartPoint = false;
	[SerializeField]
	private bool _hasEndPoint = false;
	[SerializeField]
	private Vector3 _startPoint;
	[SerializeField]
	private Vector3 _endPoint;

	private float _startY;

	public void SetStart(Vector3 start)
	{
		_startPoint = start;
		_hasEndPoint = false;
	}

	public void SetEnd(Vector3 end)
	{
		end.y = _startPoint.y;
		_endPoint = end;
		_hasEndPoint = true;
	}

	void UpdateMeasureInfo()
	{
		Vector3 midPoint = (_startPoint + _endPoint) / 2;
		_infoText.transform.position = midPoint;

		float distance = Vector3.Distance(_startPoint, _endPoint);
		_infoText.text = (distance * 100).ToString("0") + "cm";    // to CM Unit

		Vector3 dirVec = _endPoint - _startPoint;
		float angle = Vector3.SignedAngle(dirVec, new Vector3(1, 0, 0), Vector3.up);
		_infoText.transform.localEulerAngles = new Vector3(0, 0, angle);

	}

	// Start is called before the first frame update
	void Start()
	{

	}


	void Update()
	{
		if (_hasEndPoint == false)
		{
			_lineRenderer.enabled = false;
			_infoText.gameObject.SetActive(false);
			return;
		}

		// 2
		_lineRenderer.enabled = true;
		_lineRenderer.SetPosition(0, _startPoint);
		_lineRenderer.SetPosition(1, _endPoint);


		// 3
		_infoText.gameObject.SetActive(true);
		UpdateMeasureInfo();
	}
}
