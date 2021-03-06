﻿using UnityEngine;
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

    Object previewCar = null;


	// Use this for initialization
	void Start () 
    {
        //Screen.showCursor = false;
        networkConnection = NetworkConnection.GetInst();
        if (networkConnection != null)
        {  
            networkConnection.RefreshServerList();
        }
        else
            Debug.Log("There are no object with name Network");
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (previewCar == null)
        {
            previewCar = Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
        }

        previewColorImg.color = new Color(sliderR.value, sliderG.value, sliderB.value);
        networkConnection.CarColor = previewColorImg.color;
	}

    public void OnShow()
    {
        if (check == 0)
        {
            previewCar = Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
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
        previewCar = Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
    }

    public void OnNextBtnClicked()
    {
        Destr2 = GameObject.FindWithTag("Player");
        Destroy(Destr2);

        if (AvatarIndex == loadAvatarInfo.Length - 1)
            AvatarIndex = 0;
        else
            AvatarIndex++;
        previewCar = Instantiate(loadAvatarInfo[AvatarIndex].AvatarPreview);
    }

    public void OnOKBtnClicked()
    {
        PlayerPrefs.SetString("playerName", networkConnection.playerName);

        Destr2 = GameObject.FindWithTag("Player");
        Destroy(Destr2);

        Netw = GameObject.Find("Network");
        Netw.gameObject.tag = (loadAvatarInfo[AvatarIndex].TagName);

        UIManager.GetInst().SetPanelShow("menublock");
        check = 0;
        previewCar = null;
    }
}
