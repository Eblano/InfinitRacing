using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelCreateRoom : MonoBehaviour 
{
    NetworkConnection networkConnection;
    int levelIndex = 0;
    private GameObject text_;

    public LoadLevelInformation[] loadLevelInfo;
    public InputField inputPort;
    public InputField inputServerName;
    public InputField inputDesc;
    public InputField inputConntCount;
    public RawImage imgTrack;

	// Use this for initialization
	void Start () 
    {
        Screen.showCursor = false;
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
        inputPort.text = networkConnection.connectPort.ToString();
        inputServerName.text = networkConnection.serverName;
        inputDesc.text = networkConnection.serverDescription;
        inputConntCount.text = networkConnection.maxConnections.ToString();

        imgTrack.texture = loadLevelInfo[levelIndex].mapPreview;
	}

    public void OnShow()
    {

    }

    public void OnBtnStartServerClicked()
    {
        text_ = GameObject.Find("Loading text");
        text_.renderer.enabled = true;
        networkConnection.StartServer(loadLevelInfo[levelIndex].sceneName);
        UIManager.GetInst().SetPanelShow("ingame");
    }

    public void OnBtnBackToMenuClicked()
    {
        UIManager.GetInst().SetPanelShow("menublock");
    }

    public void OnBtnPrevClicked()
    {
        if (levelIndex == 0)
            levelIndex = loadLevelInfo.Length - 1;
        else
            levelIndex--;
    }

    public void OnBtnNextClicked()
    {
        if (levelIndex == loadLevelInfo.Length - 1)
            levelIndex = 0;
        else
            levelIndex++;
    }
}
