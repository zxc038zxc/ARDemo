using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARChessPieceController : MonoBehaviour
{
	[SerializeField]
	private PhotonView _pv;
	[SerializeField]
	private Transform _cellRoot;
	[SerializeField]
	private PieceCellComponent _cellSample;

	private BoardManager _board;
	private ARAnchor _boardAnchor;
	private List<PieceCellComponent> _cacheCells = new List<PieceCellComponent>();

	private bool _isWorking = false;
	private bool _isSelfTurn = false;

	private void OnEnable()
	{
		_isWorking = true;
	}

	private void OnDisable()
	{
		_isWorking = false;
	}

	private void Update()
	{
		if (!_isSelfTurn)
		{
			return;
		}

		if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
		{
			if (Input.touchCount > 0)
			{
				var touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began)
				{
					if (_board.TryGetCellPos(out var cellPos, out var cellIndex))
					{
						var player = PhotonNetwork.LocalPlayer;
						var relativePos = _boardAnchor.transform.InverseTransformPoint(cellPos);
						_pv.RPC("PlacePieceRPC", RpcTarget.All,player, relativePos, cellIndex);
					}
				}

			}
			if (Input.GetMouseButtonDown(0))
			{
				if (_board.TryGetCellPos(out var cellPos, out var cellIndex))
				{
					var player = PhotonNetwork.LocalPlayer;
					var relativePos = _boardAnchor.transform.InverseTransformPoint(cellPos);
					_pv.RPC("PlacePieceRPC", RpcTarget.All,player, relativePos, cellIndex);
				}
			}
		}
	}

	public void Release()
	{
		if(_board != null)
			_board.ResetCell();

		foreach(var cell in _cacheCells)
		{
			UnitySpawnPool.Instance.Despawn(cell.gameObject);
		}
		_cacheCells.Clear();
	}

	public void SetBoard(GameObject obj)
	{
		_boardAnchor = obj.GetComponent<ARAnchor>();
		_board = obj.GetComponent<BoardManager>();
		_board.ResetCell();
		_isSelfTurn = PhotonNetwork.IsMasterClient;
		_board.StartHighlight(_isSelfTurn);
	}

	[PunRPC]
	private void PlacePieceRPC(Player player, Vector3 pos, Vector2 cellIndex)
	{
		_isSelfTurn = player != PhotonNetwork.LocalPlayer; 

		// 此為生成相對位置的方法
		//var worldPos = _boardAnchor.transform.TransformPoint(pos);

		_board.StartHighlight(_isSelfTurn);
		var cell = _board.GetCell((int)cellIndex.x, (int)cellIndex.y);

		var obj = UnitySpawnPool.Instance.SpawnGameObj(_cellSample);
		//obj.transform.SetParent(_cellRoot);
		//obj.transform.localPosition = worldPos;
		obj.transform.SetParent(_board.transform);
		obj.transform.position = cell.GetRenderBoundsTopPoint();
		obj.transform.localScale = _board.transform.localScale;
		obj.SetMat(player.IsMasterClient);

		_board.SetCellUsed(cellIndex, player.ActorNumber);
		_cacheCells.Add(obj);

		InfoTransceiver<ChangeTurnMsg>.Broadcast(new ChangeTurnMsg
		{
			IsSelfTurn = _isSelfTurn
		});
	}
}
