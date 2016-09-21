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

    bool needTriggerConnect = false;

    void Init()
    {
        networkConnection = NetworkConnection.GetInst();
        //Screen.showCursor = false;
        if (networkConnection)
            networkConnection.RefreshServerList();
        else
            Debug.Log("There are no object with name Network");
    }

	// Use this for initialization
	void Start () 
    {
        Init();
	}
	
	// Update is called once per frame
	void Update () 
    {
        imgTrack.texture = loadLevelInfo[levelIndex].mapPreview;

        if (needTriggerConnect)
        {
            this.gameObject.SetActive(false);
            UIManager.GetInst().ShowLoadingMap(true);
            UIManager.GetInst().RefreshLoadingProgress(0.0f);

            //text_ = GameObject.Find("Loading text");
            //text_.renderer.enabled = true;
            networkConnection.StartServer(loadLevelInfo[levelIndex].sceneName);
            //UIManager.GetInst().SetPanelShow("ingame");
            needTriggerConnect = false;
        }
	}

    public void OnShow()
    {
        Init();
        inputPort.text = networkConnection.connectPort.ToString();
        inputServerName.text = networkConnection.serverName;
        inputDesc.text = networkConnection.serverDescription;
        inputConntCount.text = networkConnection.maxConnections.ToString();
    }

    public void OnBtnStartServerClicked()
    {
        needTriggerConnect = true;
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
