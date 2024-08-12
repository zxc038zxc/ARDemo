using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCellComponent : MonoBehaviour
{
	[SerializeField]
	private Renderer _renderer;
	[SerializeField]
	private Material _blackMat;
	[SerializeField]
	private Material _whiteMat;

	public void SetMat(bool isMaster)
	{
		_renderer.material = isMaster? _whiteMat: _blackMat;
	}
}
