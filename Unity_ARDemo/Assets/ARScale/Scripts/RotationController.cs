using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class RotationController : MonoBehaviour
{
	[SerializeField]
	private MakeAppearOnPlane _makeAppearOnPlane;

	[SerializeField, Tooltip("�Ω󱱨����]�l���ƶ�")]
	private Slider _silder;
	[SerializeField, Tooltip("�Ω�b�̹��W��ܷ�e����]�l���奻")]
	private Text _text;
	[SerializeField, Tooltip("�̤p����]�l")]
	private float _minRotation = 0;
	[SerializeField, Tooltip("�̤j����]�l")]
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
