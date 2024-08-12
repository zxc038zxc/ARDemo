using Photon.Pun;

public class PhotonLoginService : ILoginService
{
	public void Login(string userName)
	{
		PhotonNetwork.NickName = userName;
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetworkManager.Instance.ConnectToServer(); 
		}
		else
		{
			PhotonNetworkManager.Instance.JoinLobby();
		}
	}
}
