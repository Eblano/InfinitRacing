using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NetworkView))]

public class NetworkConnection : MonoBehaviour 
{
    public bool ForceUseNat = true;
	public string connectToIP = "127.0.0.1";
	public int connectPort = 9000;
	
	public string masterServerIP = "127.0.0.1";
	public int masterServerPort = 9000;

	string gameType = "03h7fn49fmws02mf74mf66n5m88dmndaciorbyn";
	public string serverName = "Test server";
	public string playerName = "Player"; 
	public string ColorName = "White";
    public Color CarColor = Color.white;
	public string serverDescription = "Welcome Everybody!";
	public int maxConnections = 30;
	public bool usePassword;
	public string password;
	public string sceneOnDisconnect;
	
	int lastLevelPrefix = 0;
	
	public HostData[] hostData;
	public ArrayList playerList = new ArrayList();
	
	[HideInInspector]
	public bool tryingToConnect;
	
	void Start () 
    {	
		playerName = UtilsC.CreateRandomString(5);
		DontDestroyOnLoad(this);
		networkView.group = 1;
		Application.LoadLevel(sceneOnDisconnect);
	}
	

	void Update()
    {
        // �����ͻ���û�����Ӳ�����������ַ���ڵ�ʱ����ȡ�������б�
		if(UtilsC.CheckPeerType(NetworkPeerType.Disconnected) && UtilsC.IsHostsExists())
			hostData = MasterServer.PollHostList();
	}
	
    // ����������
	public void StartServer(string levelName)
    {
		foreach (GameObject go in MonoBehaviour.FindObjectsOfType(typeof (GameObject)))
			go.SendMessage("OnTryingToStartServer",connectPort, SendMessageOptions.DontRequireReceiver);
		Network.InitializeSecurity();
		if(usePassword)
			Network.incomingPassword = password;
			
        // ��ʼ��������
        bool useNat = !ForceUseNat ? !Network.HavePublicAddress() : true;
        Network.InitializeServer(maxConnections, connectPort, useNat);
        // ע������
		MasterServer.RegisterHost(gameType, serverName, serverDescription);
		
		Network.RemoveRPCsInGroup(0);
		Network.RemoveRPCsInGroup(1);
		networkView.RPC("LoadMap", RPCMode.AllBuffered, levelName, lastLevelPrefix + 1);
	}
	
    // ���ӵ�����
	public void Connect()
    {
		foreach (GameObject go in MonoBehaviour.FindObjectsOfType(typeof (GameObject)))
			go.SendMessage("OnTryingToConnect", SendMessageOptions.DontRequireReceiver);
		Network.Connect(connectToIP, connectPort, password);
	}
	
    // ˢ�·������б�
	public void RefreshServerList()
    {
		MasterServer.ClearHostList();
        MasterServer.RequestHostList(gameType);
	}

    // �ߵ����
	public void Kick(NetworkPlayer player, bool sendDisconnectionNotification)
    {
		Network.CloseConnection(player, sendDisconnectionNotification);
	}
	
    // �ϵ�����
	public void Disconnect(int timeout)
    {
        bool isServer = UtilsC.CheckPeerType(NetworkPeerType.Server);
		Network.Disconnect(timeout);
        if (isServer)
			MasterServer.UnregisterHost();
	}
	
    // ������
	[RPC]
	void AddPlayerToList(NetworkPlayer player, string username)
    {
		PlayerInfo newPlayerInfo = new PlayerInfo();
		newPlayerInfo.player = player;
		newPlayerInfo.username = username;
		playerList.Add(newPlayerInfo);
		Debug.Log("Add Palyer: " + username);
	}
	
    // ɾ�����
	[RPC]
	void RemovePlayerFromList(NetworkPlayer player)
    {
		foreach (PlayerInfo playerInstance in playerList) 
        {
			if (player == playerInstance.player) 		
				playerList.Remove(playerInstance);
		}
	}
	
    // ���ӵ��������Ļص�
	void OnConnectedToServer() 
    {
		tryingToConnect = false;

        UIManager.GetInst().HideMsgBox();
        //UIManager.GetInst().SetPanelShow("ingame");
	}

    // �Ͽ����ӵĻص�
	void OnDisconnectedFromServer(NetworkDisconnection info) 
    {
		Application.LoadLevel(sceneOnDisconnect);
	}
	
	void OnTryingToStartServer(int port)
    {
	
	}
	
    // ��������
	void OnTryingToConnect()
    {
		tryingToConnect = true;

        UIManager.GetInst().ShowMsgBox("", @"Connecting server, please wait ...", PanelMessageBox.BtnStatus.BS_OK, null, null);
	}
	
    // ����ʧ��
	void OnFailedToConnect(NetworkConnectionError error)
    {
		tryingToConnect = false;

        UIManager.GetInst().ShowMsgBox("Warning", "Connect to server failed!", PanelMessageBox.BtnStatus.BS_OK, null, null);
	}
	
    // ��������ӽ���
	void OnPlayerConnected(NetworkPlayer player) 
    {
		Debug.Log("Player connected from: " + player.ipAddress +":" + player.port);
	}
	
    // ��������ʼ�����
	void OnServerInitialized() 
    {
		Debug.Log("Server initialized and ready");
	}
	
    // ����ҵ���
	void OnPlayerDisconnected(NetworkPlayer player) 
    {
		Debug.Log("Player disconnected from: " + player.ipAddress+":" + player.port);
	//	networkView.RPC("RemovePlayerFromList", RPCMode.All, player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
    // ������ͼ�������
	void OnNetworkLoadedLevel()
    {
		playerList  = new ArrayList();
        //playerName = PlayerPrefs.GetString("playerName");
		networkView.RPC("AddPlayerToList",RPCMode.AllBuffered, Network.player, playerName);
	}
	
    // ���ӵ���������ʧ��
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) 
    {
        Debug.Log("Could not connect to master server: " + info);
    }
	
    // ����������ͼ
	[RPC]
	IEnumerator LoadMap (string _levelName, int _levelPrefix)
    {
        UIManager.GetInst().ShowLoadingMap(true);
        UIManager.GetInst().RefreshLoadingProgress(0.0f);

		Debug.Log("Loading level " + _levelName + " with prefix " + _levelPrefix);
		lastLevelPrefix = _levelPrefix;
		
        // ��ͣ���紦��
		Network.SetSendingEnabled(0, false);	
		//UtilsC.SetReceivingEnabled(0, false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(_levelPrefix);
			
        // �첽���س���
        AsyncOperation asyncOp = Application.LoadLevelAsync(_levelName);
        //AsyncOperation asyncOp = Application.LoadLevelAdditiveAsync(_levelName);
      		    
		while (!asyncOp.isDone)
        {
			Debug.Log("Loading: " + asyncOp.progress.ToString());
            UIManager.GetInst().RefreshLoadingProgress(asyncOp.progress);
            System.Threading.Thread.Sleep(1000);
			yield return null;
		}

        UIManager.GetInst().RefreshLoadingProgress(1.0f);
        System.Threading.Thread.Sleep(1000);
        yield return null;
			 
        // �ָ����紦��
		Debug.Log("Loading complete");
		//UtilsC.SetReceivingEnabled(0, true);
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);
		
		foreach (GameObject go in MonoBehaviour.FindObjectsOfType(typeof (GameObject)))
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);

        UIManager.GetInst().EnterGame();
	}
	
	[ContextMenu ("Set Game ID")]
	void SetGameID()
    {
		gameType = UtilsC.CreateRandomString(30);
	}
}
