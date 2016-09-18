using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;


[System.Serializable]
public class LoadLevelInformation
{
	public string mapName;
	public string sceneName;
	public Texture mapPreview;
}

[System.Serializable]
public class LoadAvatarInformation
{
	public Transform AvatarPreview;
	public string TagName;
}

public class UI_menu : MonoBehaviour 
{	
	NetworkConnection networkConnection;
	
	int check = 0;
	private GameObject Destr1;
	private GameObject Destr2;
	private GameObject Destr3;
	private GameObject text_;
	
	private GameObject Netw;
	public LoadLevelInformation[] loadLevelInfo;
	
	public LoadAvatarInformation[] loadAvatarInfo;
	public GUISkin guiskin;
	public GUISkin guiskin2;
	public Texture2D cursorImage;
	public Vector2 cursorOffset;
	
	int levelIndex = 0;
    int AvatarIndex = 0;
	
	string menuState;
	
	string errorText;
	string connectionStatusText;
	
	Vector2 scrollPosition;
	

	void Start () 
    {		
        //Screen.showCursor = false;
		if(GameObject.Find("Network"))
        {
			networkConnection = GameObject.Find("Network").GetComponent<NetworkConnection>();
			networkConnection.RefreshServerList();			
		}
		else
			Debug.Log("There are no object with name Network");
	}
	
	void OnGUI ()
    {
        if (UIManager.C_USE_NEW_UI == false)
        {
            GUI.skin = guiskin;
            GUILayout.Label("Connection status: " + Network.peerType.ToString());
            if (UtilsC.CheckPeerType(NetworkPeerType.Disconnected))
            {
                switch (menuState)
                {
                    // 选择赛车
                    case "setavatarstyle": SetAvatarStyle();
                        break;
                    // 显示主菜单
                    case "menublock": MenuBlock();
                        break;
                    // 建立比赛房间
                    case "hostgame": HostGame();
                        break;
                    // 进入比赛房间
                    case "findgame": FindGame();
                        break;
                    //				case "offlinegame":OfflineGameSettings();
                    //		       		break;
                    // 设置玩家名称
                    case "setname": SetPlayerName();
                        break;
                    // 消息出错
                    case "networkerror": MSG_Error();
                        break;
                    // 尝试重连
                    case "tryingtoconnect": MSG_TryingToConnect();
                        break;
                    // 设置玩家名称
                    default: SetPlayerName();
                        break;
                }
            }
            DrawCursor();	
        }
	}
	
	void DrawCursor()
    {
		GUI.skin = guiskin2;
		Vector3 mousePos = Input.mousePosition;
		GUI.Label(new Rect(mousePos.x + cursorOffset.x ,Screen.height - mousePos.y + cursorOffset.y, cursorImage.width,cursorImage.height),cursorImage);
	}
	
	/*
	void LoadingPopup(){
		if(UtilsC.IsWebPlayer(){
			GUI.Box(new Rect(Screen.width/4+0,Screen.height/2-30,450,50), "");
			if(Application.CanStreamedLevelBeLoaded (levelIndex)){
				GUI.Label(new Rect(Screen.width/4+10,Screen.height/2-25,285,150), "Starting the game!");
				//Application.LoadLevel((Application.loadedLevel+1));
			}else{
				GUI.Label(new Rect(Screen.width/4+10,Screen.height/2-25,285,150), "Loading the game: "+Mathf.Floor(Application.GetStreamProgressForLevel(levelIndex)*100)+" %");
			}
		}
	}
	*/
	
    // 设置玩家名称
	void SetPlayerName()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 85, Screen.height / 2 - 50, 200, 200));
        GUI.Box(new Rect(0, 40, 200, 80), "", GUI.skin.FindStyle("Box"));
        GUILayout.Label("Player Name", GUI.skin.FindStyle("Lable"));
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        networkConnection.playerName = GUILayout.TextField(networkConnection.playerName, 20, GUILayout.MinWidth(180), GUILayout.MaxWidth(250));
        GUILayout.Space(15);
        GUILayout.EndHorizontal();

        if (UtilsC.IsStringCorrect(networkConnection.playerName))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            if (GUILayout.Button("Exit", GUI.skin.FindStyle("Button"), GUILayout.MaxWidth(100)))
                Application.Quit();
            if (GUILayout.Button("OK", GUI.skin.FindStyle("Button"), GUILayout.MaxWidth(100)))
            {
                PlayerPrefs.SetString("playerName", networkConnection.playerName);
                menuState = "setavatarstyle";
            }
            GUILayout.Space(50);
            GUILayout.EndHorizontal();
        }
        else
            GUI.Label(new Rect(12, 90, 185, 30), "Enter your player name!");
        GUILayout.EndArea();
	}
	
    // 选择赛车
    void SetAvatarStyle()
    {
		if (check == 0)
        {
		    Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
			check = 1;
		}
		//Menu color selection
		
		GUI.Box(new Rect(10,80,100,250), "Color");
		
		if(GUI.Button(new Rect(20,120,80,20), "Black")) 
        {
			networkConnection.ColorName = "Black";
		}
		if(GUI.Button(new Rect(20,150,80,20), "White")) 
        {
			networkConnection.ColorName = "White";
		}
		if(GUI.Button(new Rect(20,180,80,20), "Red")) 
        {
			networkConnection.ColorName = "Red";
		}
		if(GUI.Button(new Rect(20,210,80,20), "Green")) 
        {
			networkConnection.ColorName = "Green";
		}
		if(GUI.Button(new Rect(20,240,80,20), "Blue")) 
        {
			networkConnection.ColorName = "Blue";
		}
		if(GUI.Button(new Rect(20,270,80,20), "Yellow")) 
        {
			networkConnection.ColorName = "Yellow";
		}
		
		GUILayout.BeginArea(new Rect(Screen.width - 300, Screen.height - 100, 200, 53));
				GUILayout.Label("SELECT CAR");
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
		if (GUILayout.Button("Prev",GUILayout.MinWidth(70)))
        {			
            Destr1= GameObject.FindWithTag("Player");
            Destroy (Destr1);
            if (AvatarIndex == 0)
                AvatarIndex = loadAvatarInfo.Length - 1;
            else
                AvatarIndex--;
			Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
		}
							
		if (GUILayout.Button("Next",GUILayout.MinWidth(70)))
        {			
			Destr2= GameObject.FindWithTag("Player");
			Destroy (Destr2);
			
			if(AvatarIndex == loadAvatarInfo.Length-1)
				AvatarIndex = 0;
			else
				AvatarIndex++;
			 Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
        }
				
		if (GUILayout.Button("OK", GUILayout.MaxWidth(55)))
        {
            PlayerPrefs.SetString("playerName", networkConnection.playerName);
			
			Destr2= GameObject.FindWithTag("Player");
	        Destroy (Destr2);
			
			Netw = GameObject.Find("Network");
			Netw.gameObject.tag = (loadAvatarInfo[AvatarIndex].TagName);
			
			menuState = "menublock";
		}
        GUILayout.EndHorizontal();
        if(loadLevelInfo[levelIndex].mapPreview)
	        GUILayout.Box(loadLevelInfo[levelIndex].mapPreview, GUI.skin.FindStyle("Box"));
        else
	        GUILayout.Box("No Image");
        GUILayout.EndVertical();
        GUILayout.EndArea();		
	}
	
	// 游戏主菜单
	void MenuBlock()
    {
		GUI.BeginGroup (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 250));
		GUILayout.Label("MAIN MENU", GUILayout.MaxWidth(200));
	
		if(GUILayout.Button ("Create Room",GUILayout.MaxWidth(200)))
			menuState = "hostgame";			
		if(GUILayout.Button ("Enter Room",GUILayout.MaxWidth(200)))
			menuState = "findgame";			
//			if(GUILayout.Button ("Play Offline",GUILayout.MaxWidth(200)))
//				menuState = "offlinegame";			
		if(GUILayout.Button ("Change Name",GUILayout.MaxWidth(200)))
			menuState = "setname";			
		if(!UtilsC.IsWebPlayer())
        {
			if(GUILayout.Button ("Exit",GUILayout.MaxWidth(200)))
				Application.Quit();				
		}
		GUI.EndGroup ();
	}

    // 进入比赛房间
	void FindGame()
    {
		GUILayout.BeginArea(new Rect (Screen.width / 2 - 300, Screen.height / 2 - 200, 600, 400)); 
		GUI.Box(new Rect(0, 20, 600, 400),"");
		GUILayout.Label("Find Game Servers");
		
		GUILayout.BeginArea(new Rect(0, 30, 595,200));
			GUI.Box(new Rect(5,0,590,200), "", GUI.skin.FindStyle("Box"));
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(575), GUILayout.Height(200));
			if(UtilsC.IsHostsExists())
            {
				foreach (HostData element in networkConnection.hostData)
                {
					string name = " [" +element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit + "] ";
					string hostInfo = "";
					string gameComment =" [" +element.comment+"] ";
					
					foreach (string host in element.ip)
						hostInfo = hostInfo + host + ":" + element.port + " ";
					hostInfo = " [" + hostInfo + "] ";
					
					GUILayout.BeginHorizontal();
					GUILayout.Label(name + gameComment + hostInfo +  (element.passwordProtected ? "Private " : "Public ").ToString(), GUI.skin.FindStyle("Lable1"));	
					GUILayout.Space(100);
					if (GUILayout.Button("Connect", GUI.skin.FindStyle("Button"), GUILayout.MaxWidth(55))){
						string TEMP_IP = "";
						foreach (string part in element.ip)
							TEMP_IP = TEMP_IP + part;
						networkConnection.connectToIP = TEMP_IP;
						networkConnection.connectPort = int.Parse(element.port.ToString());
						networkConnection.Connect();	
					}
					GUILayout.Space(10);
					GUILayout.EndHorizontal();	
				}
			}
			else
            {
				GUILayout.Label(" There are no hosts registered");
			}
			GUILayout.EndScrollView();
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(0, 233, 480, 127));
		GUI.Box(new Rect(5,0,450,127), "", GUI.skin.FindStyle("Box"));
			GUILayout.BeginArea(new Rect(15, 5, 350,250));
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
					GUILayout.Label("IP: ");
					GUILayout.Label("Port: ");
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
					networkConnection.connectToIP = GUILayout.TextField(networkConnection.connectToIP, GUILayout.MaxWidth(200));
					networkConnection.connectPort = int.Parse(GUILayout.TextField(networkConnection.connectPort.ToString(), GUILayout.MaxWidth(200)));
					GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
				networkConnection.usePassword = GUILayout.Toggle(networkConnection.usePassword,"Use Password");
				if(networkConnection.usePassword)
						networkConnection.password = GUILayout.PasswordField(networkConnection.password, "*"[0], 15,GUILayout.Width(100));
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(15, 340, 250,30));
			if (GUILayout.Button ("Connect", GUI.skin.FindStyle("Button"), GUILayout.MaxWidth(130)))
				networkConnection.Connect();
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(5, 365, 500, 30));
			GUILayout.BeginHorizontal();
				if (GUILayout.Button("Refresh List", GUILayout.MaxWidth(130)))
					networkConnection.RefreshServerList();
				if (GUILayout.Button ("Back To Menu", GUILayout.MaxWidth(130)))
					menuState = "menublock";
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		GUILayout.EndArea();
	}
	
    // 建立比赛房间
	void HostGame()
    {
		GUILayout.BeginArea(new Rect (Screen.width / 2 - 325, Screen.height / 2 - 180, 700, 400));
		
		GUI.Box(new Rect(0, 20, 650, 300),"");
		
		GUILayout.Label("CREATE ROOM");
			GUILayout.BeginArea(new Rect(5, 100, 400,180));
				GUILayout.Label("MAIN");
				GUILayout.BeginHorizontal();
					GUILayout.BeginVertical();
						GUILayout.Label("Server Port: ");
						GUILayout.Label("Server Name: ");
						GUILayout.Label("Server Info: ");
						GUILayout.Label("Max connections: ");
						GUILayout.EndVertical();
					GUILayout.BeginVertical();
						networkConnection.connectPort = int.Parse(GUILayout.TextField(networkConnection.connectPort.ToString()));
						networkConnection.serverName = GUILayout.TextField(networkConnection.serverName, GUILayout.MinWidth(140));
						networkConnection.serverDescription = GUILayout.TextField(networkConnection.serverDescription, GUILayout.MinWidth(140));
						networkConnection.maxConnections = int.Parse(GUILayout.TextField(networkConnection.maxConnections.ToString(), GUILayout.MinWidth(140)));
					GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
					
			GUILayout.BeginArea(new Rect(10, 240, 190,250));
				networkConnection.usePassword = GUILayout.Toggle(networkConnection.usePassword,"Use Password");
				if(networkConnection.usePassword)
                {
					networkConnection.password = GUILayout.PasswordField(networkConnection.password, "*"[0], 15,GUILayout.Width(150));
					GUILayout.BeginHorizontal();
					if(GUILayout.Button("Generate", GUI.skin.FindStyle("Button"), GUILayout.MaxWidth(70)))
						networkConnection.password = UtilsC.CreateRandomString(15);
					if(GUILayout.Button("Clear", GUI.skin.FindStyle("Button"),GUILayout.MaxWidth(70)))
						networkConnection.password = "";
					GUILayout.EndHorizontal();
				}
				
			GUILayout.EndArea();
		
			GUILayout.BeginArea(new Rect(180, 350, 500, 30));
				GUILayout.BeginHorizontal();
				if (GUILayout.Button ("Start Server", GUILayout.MaxWidth(130)))
                {
                    text_ = GameObject.Find("Loading text");
                    text_.renderer.enabled = true;
                    networkConnection.StartServer(loadLevelInfo[levelIndex].sceneName);	
                }
				if (GUILayout.Button ("Back To Menu", GUILayout.MaxWidth(130)))
					menuState = "menublock";
				
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			GUILayout.BeginArea(new Rect(450, 50, 190,250));
				GUILayout.Label("ACCESS POINT");
				GUILayout.BeginVertical();
				GUILayout.Label("Point: " + loadLevelInfo[levelIndex].mapName);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Prev",GUILayout.MinWidth(70))){
					if(levelIndex == 0)
						levelIndex = loadLevelInfo.Length-1;
					else
						levelIndex--;
				}
							
				if (GUILayout.Button("Next",GUILayout.MinWidth(70))){
					if(levelIndex == loadLevelInfo.Length-1)
						levelIndex = 0;
					else
						levelIndex++;
				}
				
				GUILayout.EndHorizontal();
				if(loadLevelInfo[levelIndex].mapPreview)
					GUILayout.Box(loadLevelInfo[levelIndex].mapPreview, GUI.skin.FindStyle("Box"));
				else
					GUILayout.Box("No Image");
				GUILayout.EndVertical();
			GUILayout.EndArea();
		GUILayout.EndArea(); 
	}	
	
	
/*	void OfflineGameSettings(){
		GUILayout.BeginArea(new Rect(10, 50, 500, 300));
		GUI.Box(new Rect(0, 0, 410, 300),"");
		GUILayout.Label("Offline Game Settings");
		GUILayout.BeginArea(new Rect(5, 265, 500, 30));
		if (GUILayout.Button ("Back To Menu", GUILayout.MaxWidth(130)))
			menuState = "menublock";
		
		GUILayout.EndArea();
		GUILayout.EndArea();
	}
*/	

    // 连接失败的处理
	void OnFailedToConnect(NetworkConnectionError error)
    {
		errorText = error.ToString();
		menuState = "networkerror";
	}
	
    // 尝试连接
	void OnTryingToConnect()
    {
		connectionStatusText = "Trying to connect " +networkConnection.connectToIP.ToString()+":"+networkConnection.connectPort.ToString();
		menuState = "tryingtoconnect";
	}
	
    // 尝试连接的UI显示
	void MSG_TryingToConnect()
    {
		GUILayout.BeginArea(new Rect(Screen.width/2-150,Screen.height/2-100,300,100));
		GUI.Box(new Rect(0,0,300,100), "", GUI.skin.FindStyle("Box"));
		GUILayout.Label("Connecting...", GUI.skin.FindStyle("Lable1"));
		GUILayout.Label(connectionStatusText, GUI.skin.FindStyle("Lable1"));
		GUILayout.EndArea(); 
	}
	
    // 连接出错的UI显示
	void MSG_Error()
    {
		GUILayout.BeginArea(new Rect(Screen.width/2-150,Screen.height/2-100,300,100));
		GUI.Box(new Rect(0,0,300,100), "", GUI.skin.FindStyle("Box"));
		GUILayout.Label("Error", GUI.skin.FindStyle("Lable1"));
		GUILayout.Label(errorText, GUI.skin.FindStyle("Lable1"));
		GUILayout.BeginArea(new Rect(115,65,70,30));
			if(GUILayout.Button("OK",GUI.skin.FindStyle("Button"),GUILayout.MaxWidth(70)))
            {
				errorText="";
				menuState = "findgame";
			}
			GUILayout.EndArea(); 
		GUILayout.EndArea(); 
	}
}
