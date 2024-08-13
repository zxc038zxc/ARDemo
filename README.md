# AR + Photon + 五子棋遊戲

---

## AR Foundtaion應用
主要在實現Unity端，與ARFoundation + AR Core的應用
並建立一個簡單的遊戲，在手機端上操作

## Photon連線
### 1. 輸入名字後，進入房間選擇大廳

![Lobb](https://github.com/user-attachments/assets/3f56a909-e461-47fa-b845-254e9095e107)

### 2. 隨機進房、創建房間以及加入指定房間

![745251174 148763 (1)](https://github.com/user-attachments/assets/7fc19db4-7e4f-4a03-92f5-e1b416f5a7d3)

### 3. 進入房間後，雙方可以根據AR Plane Detect，檢測附近的地板，並放置五子棋台

![745251174 214914](https://github.com/user-attachments/assets/6259a1ce-4357-4963-8490-b6c25b6169e2)

### 4. 放置五子棋台後，可以進行「旋轉」、「放大」

![745251174 280327](https://github.com/user-attachments/assets/ea67f13d-91c3-48c1-8efe-75ff3afa6680)

### 5. 雙方放置五子棋台後，按下確認，開始輪流下棋

![745251174 343808](https://github.com/user-attachments/assets/b173bb67-48f2-4807-a4ea-aa9f1532e0b5)

### 6. 一方判斷贏了以後，便結束遊戲，返回大廳

![745251174 408022](https://github.com/user-attachments/assets/cc1dea29-efad-4efa-911f-d20053d238e6)
## 遇到問題點
### 1. 同步位置
玩家各自放置五子棋台後，由於雙方玩家所在位置不同，放置AR物件的世界座標也不相同。
若直接同步放置的座標，雙方會產生物件產生的落差。
同步上面想到兩種做法
- 計算相對位置
  ```csharp=
  private void ChessObj(Vector3 cellPos)
  {
    var player = PhotonNetwork.LocalPlayer;
	  var relativePos = _boardAnchor.transform.InverseTransformPoint(cellPos);
	  _pv.RPC("PlacePieceRPC", RpcTarget.All, player, relativePos);
  }

  [PunRPC]
	private void PlacePieceRPC(Player player, Vector3 pos)
	{
		_isSelfTurn = player != PhotonNetwork.LocalPlayer;

		// 生成相對位置的方法
		var worldPos = _boardAnchor.transform.TransformPoint(pos);
      //
        產生物件
      //
	}
  ```
  由於玩家都必須先在本地端放置一個基礎的五子棋盤位置，可透過本地的五子棋盤計算相對位置，讓其他玩家產生對應的位置
  
- 不傳送位置，而是傳送棋盤內的index
  ```csharp=
  private void ChessObj(int cellIndex)
  {
    var player = PhotonNetwork.LocalPlayer;
	  _pv.RPC("PlacePieceRPC", RpcTarget.All, player, cellIndex);
  }

  [PunRPC]
	private void PlacePieceRPC(Player player, int index)
	{
		_isSelfTurn = player != PhotonNetwork.LocalPlayer;
    var pos = _boardAnchor.GetCellIndex();
    // 產生相對位置
	}
  直接傳送index，讓其他玩家直接產生對應位置，這種方式比較穩定，避免了因為AR計算位置的誤差，導致後續棋子跟棋盤位置產生誤差
  ```
  
### 2. 放置棋盤後，相機看相其他地方，可能讓棋盤位置不固定
   解決辦法：   添加ARAnchor
   透過添加ARAnchor，棋盤的位置直接固定在物理空間的特定位置。
   ![image](https://github.com/user-attachments/assets/b8a4b776-44ba-442d-9440-3906dd95591e)

   注意：如果設定DestroyOnRemoval，當ARAnchor刪除時，物件也會被刪除，而ARAnchor刪除情況是當物件被Disable的時候

### 3. 玩家獨自在房間內按下準備完畢後，其他玩家進入沒有收到通知
  原先設定用PunRPC呼叫，但是沒有辦法完成
  後來改用RaiseEventOptions，設定AddToRoomCache就可以了
  ![image](https://github.com/user-attachments/assets/c0f7276e-04fb-4689-a025-515f2dcaa27a)

  注意：如果玩家全部離開房間後，需要手動呼叫清除RaiseEvent
  ![image](https://github.com/user-attachments/assets/a4f005b1-659d-4f22-809c-f5f7b5695627)


   



