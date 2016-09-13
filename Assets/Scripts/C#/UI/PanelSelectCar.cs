using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelSelectCar : MonoBehaviour 
{
    public LoadAvatarInformation[] loadAvatarInfo;

    public Image previewColorImg;
    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;

    int check = 0;
    int levelIndex = 0;
    int AvatarIndex = 0;

    private GameObject Destr1;
    private GameObject Destr2;
    private GameObject Destr3;
    private GameObject text_;
    private GameObject Netw;

    NetworkConnection networkConnection;


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
        previewColorImg.color = new Color(sliderR.value, sliderG.value, sliderB.value);
        networkConnection.CarColor = previewColorImg.color;
	}

    public void OnShow()
    {
        if (check == 0)
        {
            Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
            check = 1;
        }
    }

    public void OnPrevBtnClicked()
    {
        Destr1 = GameObject.FindWithTag("Player");
        Destroy(Destr1);
        if (AvatarIndex == 0)
            AvatarIndex = loadAvatarInfo.Length - 1;
        else
            AvatarIndex--;
        Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
    }

    public void OnNextBtnClicked()
    {
        Destr2 = GameObject.FindWithTag("Player");
        Destroy(Destr2);

        if (AvatarIndex == loadAvatarInfo.Length - 1)
            AvatarIndex = 0;
        else
            AvatarIndex++;
        Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
    }

    public void OnOKBtnClicked()
    {
        PlayerPrefs.SetString("playerName", networkConnection.playerName);

        Destr2 = GameObject.FindWithTag("Player");
        Destroy(Destr2);

        Netw = GameObject.Find("Network");
        Netw.gameObject.tag = (loadAvatarInfo[AvatarIndex].TagName);

        UIManager.GetInst().SetPanelShow("menublock");
    }
}
