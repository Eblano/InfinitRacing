using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelEnterRoom : MonoBehaviour 
{
    NetworkConnection networkConnection;
    public GameObject SlotPrefab;
    public RectTransform serverListViewTrans;
    public InputField inputIP;
    public InputField inputPort;
    float iniWidth;
    float iniHeight;
    bool needUpdate = true;


	// Use this for initialization
	void Start () 
    {
        Init();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (needUpdate)
        {
            OnShow();
        }
	}

    void Init()
    {
        iniWidth = serverListViewTrans.rect.width;
        iniHeight = serverListViewTrans.rect.height;
        //Screen.showCursor = false;
        networkConnection = NetworkConnection.GetInst();
        if (networkConnection != null)
        {            
            networkConnection.RefreshServerList();
            inputIP.text = networkConnection.connectToIP;
            inputPort.text = networkConnection.connectPort.ToString();
        }
        else
            Debug.Log("There are no object with name Network");
    }

    public void OnShow()
    {
        Init();
        while (serverListViewTrans.childCount > 0)
        {
            RectTransform ct = serverListViewTrans.GetChild(0) as RectTransform;
            ct.SetParent(null);
            GameObject.DestroyImmediate(ct.gameObject);
        }
        if (networkConnection.hostData == null ||
            networkConnection.hostData.Length == 0)
            return;
        int i = 0;
        float maxHeight = 0.0f;
        foreach (HostData element in networkConnection.hostData)
        {
            GameObject slot = GameObject.Instantiate(SlotPrefab) as GameObject;
            slot.transform.SetParent(serverListViewTrans);
            slot.SetActive(true);
            RectTransform ct = slot.transform as RectTransform;
            ct.anchoredPosition = new Vector2(0.0f, i * ct.rect.height);
            ct.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ct.rect.width);
            ++i;
            maxHeight += ct.rect.height;
            SingleServerSlot comp = slot.GetComponent<SingleServerSlot>();
            comp.HostDataWrap = element;
        }
        if (maxHeight < iniHeight)
            serverListViewTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iniHeight);
        else
            serverListViewTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);
        needUpdate = false;
    }

    public void OnBtnRefreshServerListClicked()
    {
        networkConnection.RefreshServerList();
        needUpdate = true;
    }

    public void OnBtnBackToMainMenuClicked()
    {
        UIManager.GetInst().SetPanelShow("menublock");
    }

    public void OnBtnConnectClicked()
    {
        networkConnection.connectToIP = inputIP.text;
        networkConnection.connectPort = int.Parse(inputPort.text);
        networkConnection.Connect();
    }
}
