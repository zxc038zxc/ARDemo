using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class RotationController : MonoBehaviour
{
	[SerializeField]
	private MakeAppearOnPlane _makeAppearOnPlane;

	[SerializeField, Tooltip("用於控制旋轉因子的滑塊")]
	private Slider _silder;
	[SerializeField, Tooltip("用於在屏幕上顯示當前旋轉因子的文本")]
	private Text _text;
	[SerializeField, Tooltip("最小旋轉因子")]
	private float _minRotation = 0;
	[SerializeField, Tooltip("最大旋轉因子")]
	private float _maxRotation = 360;
	[SerializeField]
	private XROrigin _xrOrigin;

	public void OnSilderValuedChanged()
	{
		if(_silder!=null)
		{
			Angle = _silder.value * (_maxRotation - _minRotation) + _minRotation;
		}
	}

	public float Angle
	{
		get
		{
			return _makeAppearOnPlane.Rotation.eulerAngles.y;
		}
		set
		{
			_makeAppearOnPlane.Rotation = Quaternion.AngleAxis(value, Vector3.up);
			UpdateText();
		}
	}

	private void OnEnable()
	{
		if(_silder!=null)
		{
			_silder.value = (Angle - _minRotation) * (_maxRotation - _minRotation);
		}
		UpdateText();
	}

	private void UpdateText()
	{
		if(_text!=null)
		{
			_text.text = "Rotation: " + Angle;
		}
	}
}
