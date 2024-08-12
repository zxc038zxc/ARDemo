using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	[SerializeField]
	private CellComponent _cellPrefab;
	[SerializeField]
	private int _boardSize = 15;
	[SerializeField]
	private CellComponent[,] _boardCells;
	[SerializeField]
	private LayerMask _cellLayerMask;

	private CellComponent _prevCellComp = null;
	private float _cellSize;
	private Vector3 _screenCenter;
	private RaycastHit _hit;

	[SerializeField]
	private bool _isStartGame = false;

	private void Start()
	{
		_cellSize = _cellPrefab.GetComponent<Renderer>().bounds.size.x;
		_screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
		_prevCellComp = null;
		_isStartGame = false;
		Initialize();
	}

	private void Update()
	{
		if (_isStartGame)
		{
			HighlighCellUnderCenter();
		}
	}

	public void StartHighlight(bool start)
	{
		_isStartGame = start;

		if (!start)
		{
			if (_prevCellComp != null)
			{
				_prevCellComp.SetHighligh(false);
				_prevCellComp = null;
			}
			_prevCellComp = null;
		}
	}

	public void ResetCell()
	{
		foreach (var cell in _boardCells)
		{
			cell.ResetInfo();
		}
	}

	public CellComponent GetCell(int x, int y)
	{
		return _boardCells[x, y];
	}
	private void HighlighCellUnderCenter()
	{
		Ray ray = Camera.main.ScreenPointToRay(_screenCenter);

		if (Physics.Raycast(ray, out _hit, Mathf.Infinity, _cellLayerMask))
		{
			var cell = _hit.collider.GetComponent<CellComponent>();

			// 如果 cell 為 null 或已經使用
			if (cell == null || cell.IsUsed)
			{
				UpdateHighlight(null);
			}
			else
			{
				UpdateHighlight(cell);
			}
		}
		else
		{
			// 如果 Raycast 沒有擊中任何對象，取消高亮顯示
			UpdateHighlight(null);
		}
	}

	private void UpdateHighlight(CellComponent newCell)
	{
		if (_prevCellComp != null && _prevCellComp != newCell)
		{
			_prevCellComp.SetHighligh(false);
		}

		if (newCell != null && newCell != _prevCellComp)
		{
			newCell.SetHighligh(true);
		}

		// 更新當前高亮的單元格
		_prevCellComp = newCell;
	}

	public void SetCellUsed(Vector2 cellIndex, int playerID)
	{
		int x = (int)cellIndex.x;
		int y = (int)cellIndex.y;
		_boardCells[x, y].IsUsed = true;
		_boardCells[x, y].PlayerID = playerID;

		var list = CheckWin(x, y, playerID);
		if (list != null)
		{
			foreach (var tmp in list)
			{
				_boardCells[tmp.x, tmp.y].SetHighligh(true);
			}

			_isStartGame = false;
			InfoTransceiver<EndGameMsg>.Broadcast(new EndGameMsg
			{
				PlayerID = playerID,
				IsWin = PhotonNetwork.LocalPlayer.ActorNumber == playerID
			});
		}
	}

	private List<Vector2Int> CheckWin(int x, int y, int playerID)
	{
		List<Vector2Int> list = new List<Vector2Int>();

		// 檢查水平
		list.Add(new Vector2Int(x, y));
		CountConnected(x, y, 1, 0, playerID, ref list);
		CountConnected(x, y, -1, 0, playerID, ref list);
		if (list.Count >= 5) return list;


		// 檢查垂直
		list.Clear();
		list.Add(new Vector2Int(x, y));
		CountConnected(x, y, 0, 1, playerID, ref list);
		CountConnected(x, y, 0, -1, playerID, ref list);
		if (list.Count >= 5) return list;

		// 檢查主對角線（從左上到右下）
		list.Clear();
		list.Add(new Vector2Int(x, y));
		CountConnected(x, y, 1, 1, playerID, ref list);
		CountConnected(x, y, -1, -1, playerID, ref list);
		if (list.Count >= 5) return list;

		// 檢查副對角線（從右上到左下）
		list.Clear();
		list.Add(new Vector2Int(x, y));
		CountConnected(x, y, 1, -1, playerID, ref list);
		CountConnected(x, y, -1, 1, playerID, ref list);
		if (list.Count >= 5) return list;

		return null;
	}

	private void CountConnected(int x, int y, int dx, int dy, int playerID, ref List<Vector2Int> list)
	{
		int count = 0;
		int nx = x + dx;
		int ny = y + dy;

		// 向一個方向檢查
		while (nx >= 0 && nx < _boardSize && ny >= 0 && ny < _boardSize && _boardCells[nx, ny].PlayerID == playerID)
		{
			count++;
			list.Add(new Vector2Int(nx, ny));
			nx += dx;
			ny += dy;
		}
	}

	private void Initialize()
	{
		_boardCells = new CellComponent[_boardSize, _boardSize];

		Vector3 startPos = transform.position - new Vector3((_boardSize - 1) * _cellSize / 2, 0, (_boardSize - 1) * _cellSize / 2);

		for (int x = 0; x < _boardSize; x++)
		{
			for (int z = 0; z < _boardSize; z++)
			{
				Vector3 cellPos = startPos + new Vector3(x * _cellSize, 0, z * _cellSize);
				_boardCells[x, z] = Instantiate(_cellPrefab, cellPos, Quaternion.identity, transform);
				_boardCells[x, z].CellIndex = new Vector2(x, z);
			}
		}
	}

	public bool TryGetCellIndex(Vector3 position, out Vector3 pos)
	{
		pos = default(Vector3);
		var cellIndex = new Vector2Int();
		cellIndex.x = Mathf.RoundToInt((position.x - transform.position.x) / _cellSize);
		cellIndex.y = Mathf.RoundToInt((position.z - transform.position.z) / _cellSize);

		// 確保放置在合法位置
		if (cellIndex.x >= 0 && cellIndex.x < _boardSize && cellIndex.y >= 0 && cellIndex.y < _boardSize)
		{
			pos = _boardCells[cellIndex.x, cellIndex.y].transform.position;
			return true;
		}

		return false;
	}

	public bool TryGetCellPos(out Vector3 pos, out Vector2 cellIndex)
	{
		pos = Vector3.zero;
		cellIndex = default(Vector2);

		if (_prevCellComp != null)
		{
			pos = _prevCellComp.GetRenderBoundsTopPoint();
			cellIndex = _prevCellComp.CellIndex;
			return true;
		}
		else
		{
			return false;
		}
	}
}
