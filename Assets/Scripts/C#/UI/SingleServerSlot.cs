using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SingleServerSlot : MonoBehaviour 
{
    public Text serverInfo;
    HostData hostData;
    NetworkConnection networkConnection;

    public HostData HostDataWrap
    {
        get { return hostData; }
        set { hostData = value; }
    }

	// Use this for initialization
	void Start () 
    {
        //Screen.showCursor = false;
        if (GameObject.Find("Network"))
        {
            networkConnection = GameObject.Find("Network").GetComponent<NetworkConnection>();
            networkConnection.RefreshServerList();
        }
        else
            Debug.Log("There are no object with name Network");
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (hostData != null && serverInfo!=null)
        {
            string name = " [" + hostData.gameName + " " + hostData.connectedPlayers + " / " + hostData.playerLimit + "] ";
            string hostInfo = "";
            string gameComment = " [" + hostData.comment + "] ";
            foreach (string host in hostData.ip)
                hostInfo = hostInfo + host + ":" + hostData.port + " ";
            hostInfo = " [" + hostInfo + "] ";
            serverInfo.text = name + gameComment + hostInfo;
        }
	}

    public void OnBtnConnectClicked()
    {
        if (hostData != null && networkConnection != null)
        {
            string TEMP_IP = "";
            foreach (string part in hostData.ip)
                TEMP_IP = TEMP_IP + part;
            networkConnection.connectToIP = TEMP_IP;
            networkConnection.connectPort = int.Parse(hostData.port.ToString());
            networkConnection.Connect();
        }

    }
}
