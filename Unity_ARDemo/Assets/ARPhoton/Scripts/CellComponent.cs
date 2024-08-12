using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellComponent : MonoBehaviour
{
	[SerializeField]
	private Renderer _cellRenderer;
	[SerializeField]
	private Material _highlighMat;
	[SerializeField]
	private Material _normallighMat;

	public bool IsUsed = false;
	public int PlayerID = -1;

	public Vector2 CellIndex
	{
		get
		{
			return _cellIndex;
		}
		set
		{
			_cellIndex = value;
		}
	}

	private Vector2 _cellIndex;

	public void Awake()
	{
		SetHighligh(false);
	}

	public void ResetInfo()
	{
		IsUsed = false;
		PlayerID = -1;
		SetHighligh(false);
	}

	public Vector3 GetRenderBoundsTopPoint()
	{
		var bounds = _cellRenderer.bounds;
		return bounds.center + Vector3.up * bounds.extents.y;
	}

	public void SetHighligh(bool highlight)
	{
		if (highlight)
		{
			_cellRenderer.material = _highlighMat;
		}
		else
		{
			_cellRenderer.material = _normallighMat;
		}
	}
}
