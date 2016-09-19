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
        // 当本客户端没有连接并且有主机地址存在的时候，拉取服务器列表
		if(UtilsC.CheckPeerType(NetworkPeerType.Disconnected) && UtilsC.IsHostsExists())
			hostData = MasterServer.PollHostList();
	}
	
    // 启动服务器
	public void StartServer(string levelName)
    {
		foreach (GameObject go in MonoBehaviour.FindObjectsOfType(typeof (GameObject)))
			go.SendMessage("OnTryingToStartServer",connectPort, SendMessageOptions.DontRequireReceiver);
		Network.InitializeSecurity();
		if(usePassword)
			Network.incomingPassword = password;
			
        // 初始化服务器
        bool useNat = !ForceUseNat ? !Network.HavePublicAddress() : true;
        Network.InitializeServer(maxConnections, connectPort, useNat);
        // 注册主机
		MasterServer.RegisterHost(gameType, serverName, serverDescription);
		
		Network.RemoveRPCsInGroup(0);
		Network.RemoveRPCsInGroup(1);
		networkView.RPC("LoadMap", RPCMode.AllBuffered, levelName, lastLevelPrefix + 1);
	}
	
    // 连接到主机
	public void Connect()
    {
		foreach (GameObject go in MonoBehaviour.FindObjectsOfType(typeof (GameObject)))
			go.SendMessage("OnTryingToConnect", SendMessageOptions.DontRequireReceiver);
		Network.Connect(connectToIP, connectPort, password);
	}
	
    // 刷新服务器列表
	public void RefreshServerList()
    {
		MasterServer.ClearHostList();
        MasterServer.RequestHostList(gameType);
	}

    // 踢掉玩家
	public void Kick(NetworkPlayer player, bool sendDisconnectionNotification)
    {
		Network.CloseConnection(player, sendDisconnectionNotification);
	}
	
    // 断掉连接
	public void Disconnect(int timeout)
    {
        bool isServer = UtilsC.CheckPeerType(NetworkPeerType.Server);
		Network.Disconnect(timeout);
        if (isServer)
			MasterServer.UnregisterHost();
	}
	
    // 添加玩家
	[RPC]
	void AddPlayerToList(NetworkPlayer player, string username)
    {
		PlayerInfo newPlayerInfo = new PlayerInfo();
		newPlayerInfo.player = player;
		newPlayerInfo.username = username;
		playerList.Add(newPlayerInfo);
		Debug.Log("Add Palyer: " + username);
	}
	
    // 删除玩家
	[RPC]
	void RemovePlayerFromList(NetworkPlayer player)
    {
		foreach (PlayerInfo playerInstance in playerList) 
        {
			if (player == playerInstance.player) 		
				playerList.Remove(playerInstance);
		}
	}
	
    // 连接到服务器的回调
	void OnConnectedToServer() 
    {
		tryingToConnect = false;

        UIManager.GetInst().HideMsgBox();
        //UIManager.GetInst().SetPanelShow("ingame");
	}

    // 断开连接的回调
	void OnDisconnectedFromServer(NetworkDisconnection info) 
    {
		Application.LoadLevel(sceneOnDisconnect);
	}
	
	void OnTryingToStartServer(int port)
    {
	
	}
	
    // 尝试连接
	void OnTryingToConnect()
    {
		tryingToConnect = true;

        UIManager.GetInst().ShowMsgBox("", @"Connecting server, please wait ...", PanelMessageBox.BtnStatus.BS_OK, null, null);
	}
	
    // 连接失败
	void OnFailedToConnect(NetworkConnectionError error)
    {
		tryingToConnect = false;

        UIManager.GetInst().ShowMsgBox("Warning", "Connect to server failed!", PanelMessageBox.BtnStatus.BS_OK, null, null);
	}
	
    // 有玩家连接进来
	void OnPlayerConnected(NetworkPlayer player) 
    {
		Debug.Log("Player connected from: " + player.ipAddress +":" + player.port);
	}
	
    // 服务器初始化完成
	void OnServerInitialized() 
    {
		Debug.Log("Server initialized and ready");
	}
	
    // 有玩家掉线
	void OnPlayerDisconnected(NetworkPlayer player) 
    {
		Debug.Log("Player disconnected from: " + player.ipAddress+":" + player.port);
	//	networkView.RPC("RemovePlayerFromList", RPCMode.All, player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
    // 赛道地图加载完成
	void OnNetworkLoadedLevel()
    {
		playerList  = new ArrayList();
        //playerName = PlayerPrefs.GetString("playerName");
		networkView.RPC("AddPlayerToList",RPCMode.AllBuffered, Network.player, playerName);
	}
	
    // 连接到主服务器失败
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) 
    {
        Debug.Log("Could not connect to master server: " + info);
    }
	
    // 加载赛道地图
	[RPC]
	IEnumerator LoadMap (string _levelName, int _levelPrefix)
    {
        UIManager.GetInst().ShowLoadingMap(true);
        UIManager.GetInst().RefreshLoadingProgress(0.0f);

		Debug.Log("Loading level " + _levelName + " with prefix " + _levelPrefix);
		lastLevelPrefix = _levelPrefix;
		
        // 暂停网络处理
		Network.SetSendingEnabled(0, false);	
		//UtilsC.SetReceivingEnabled(0, false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(_levelPrefix);
			
        // 异步加载场景
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
			 
        // 恢复网络处理
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
