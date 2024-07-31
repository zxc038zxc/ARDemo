using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;

public class ScaleController : MonoBehaviour
{
	[SerializeField, Tooltip("�Ω󱱨��Y��]�l���ƶ�")]
	private Slider _silder;
	[SerializeField, Tooltip("�Ω�b�̹��W��ܷ�e�Y��]�l���奻")]
	private Text _text;
	[SerializeField, Tooltip("�̤p�Y��]�l")]
	private float _minScale = .1f;
	[SerializeField, Tooltip("�̤j�Y��]�l")]
	private float _maxScale = 10f;
	[SerializeField]
	private XROrigin _xrOrigin;

	public Slider Slider
	{
		get { return _silder; }
		set { _silder = value; }
	}
	public Text Text
	{
		get { return _text; }
		set { _text = value; }
	}
	public float MinScale
	{
		get { return _minScale; }
		set { _minScale = value; }
	}
	public float MaxScale
	{
		get { return _maxScale; }
		set { _maxScale = value; }
	}

	public void OnSilderValuedChanged()
	{
		if(_silder!=null)
		{
			Scale = _silder.value * (MaxScale - MinScale) + MinScale;
		}
	}

	public float Scale
	{
		get
		{
			return _xrOrigin.transform.localScale.x;
		}
		set
		{
			_xrOrigin.transform.localScale = value * Vector3.one;
			UpdateText();
		}
	}

	private void OnEnable()
	{
		if(_silder!=null)
		{
			_silder.value = (Scale - MinScale) * (MaxScale - MinScale);
		}
		UpdateText();
	}

	private void UpdateText()
	{
		if(_text!=null)
		{
			_text.text = "Scale: " + Scale;
		}
	}
}
