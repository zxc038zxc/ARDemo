using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnColorController : MonoBehaviour
{
	[SerializeField]
	private Button _btn;
	[SerializeField]
	private Color _changeColor;

	// For OnClick
	public void OnChangeColor()
	{
		MessageTransceiver<ChangeCarColorMsg>.Broadcast(new ChangeCarColorMsg
		{
			ChangeColor = _changeColor,
		});
	}
}
