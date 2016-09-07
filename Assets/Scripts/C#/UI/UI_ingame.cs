using UnityEngine;
using System.Collections;

public class UI_ingame : MonoBehaviour {
	NetworkConnection networkConnection;
	
	public GUISkin guiskin;
	public Texture2D cursorImage;
	public Vector2 cursorOffset;
	
	
	bool onPause = false;
	Vector2 scrollPosition;
	
	
	
	void Start () {
		//Screen.showCursor = false;
		if(GameObject.Find("Network"))
			networkConnection = GameObject.Find("Network").GetComponent<NetworkConnection>();
		else
			Debug.Log("There are no object with name Network");
	}
	

	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			onPause = !onPause;
			foreach (GameObject go in FindObjectsOfType(typeof (GameObject)))
				go.SendMessage("OnPauseStateChange", onPause, SendMessageOptions.DontRequireReceiver);	
		}
	}	
	
	void OnGUI(){
		GUI.skin = guiskin;
		GUILayout.Label("Connection status: " + Network.peerType.ToString());
		GUILayout.Label("Press ESC to "+ (onPause ? "hide" : "show").ToString() + " menu");
		
		if(Input.GetKey(KeyCode.Tab))
			UserBoard();
		if(onPause){
			InGameMenu();
			DrawCursor();
		}
	}
	
	void DrawCursor(){
		Vector3 mousePos = Input.mousePosition;
     	GUI.Label(new Rect(mousePos.x,Screen.height - mousePos.y,cursorImage.width,cursorImage.height),cursorImage);
	}
	
	void InGameMenu(){
		if (UtilsC.CheckPeerType(NetworkPeerType.Client)){
			if (GUILayout.Button ("Disconnect"))
				networkConnection.Disconnect(200);
			
			GUILayout.Label("Ping to server: "+Network.GetAveragePing(  Network.connections[0] ) );	
		}			
			
		if (UtilsC.CheckPeerType(NetworkPeerType.Server)){
			if (GUILayout.Button ("Stop Server", GUILayout.MaxWidth(115)))
				networkConnection.Disconnect(200);
			
			if(Network.connections.Length > 0){
				GUILayout.Label("Connections: "+Network.connections.Length);
				
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(170), GUILayout.Height(100));
				for (int i =0; i < Network.connections.Length; i++){
					GUILayout.Label("Ping to " + i.ToString() + " player: " + Network.GetAveragePing(Network.connections[i]));
					if (GUILayout.Button ("Kick " + i.ToString() + " player",GUILayout.MaxWidth(150)))
						networkConnection.Kick(Network.connections[i], true);
					
				}
				 GUILayout.EndScrollView();
			}else
				GUILayout.Label("No Player Connected");
			
		}
		
		GUILayout.Label("You Name: " + networkConnection.playerName);
		
		if(networkConnection.usePassword && UtilsC.IsStringCorrect(networkConnection.password))
			GUILayout.Label("Server Password: " + networkConnection.password);
		
	}
	
	void UserBoard(){	
		string scoreText = "Users: \n";
		
		foreach(PlayerInfo info in networkConnection.playerList)
			scoreText += info.username + "\n";
		
	
		GUILayout.BeginArea (new Rect((Screen.width-185), 20, 175, 30 + networkConnection.playerList.Count * 30));
		GUILayout.Box(scoreText);
		GUILayout.EndArea ();
	}
}
