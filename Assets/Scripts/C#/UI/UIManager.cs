using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour 
{
    public const bool C_USE_NEW_UI = true;

    CarController mainPlayerCarCtrl = null;
    public CarController MainPlayerCarCtrl
    {
        get { return mainPlayerCarCtrl; }
        set { mainPlayerCarCtrl = value; }
    }

    UI_ingame inGameUI;
    public UI_ingame InGameUI
    {
        get { return inGameUI; }
        set { inGameUI = value; }
    }

    public PanelCreateRoom mPanelCreateRoom;
    public PanelEnterRoom mPanelEnterRoom;
    public PanelInGame mPanelInGame;
    public PanelMainMenu mPanelMainMenu;
    public PanelSelectCar mPanelSelectCar;
    public PanelSetPlayerName mPanelSetPlayerName;
    public PanelMessageBox mPanelMessageBox;
    public PanelLoadingMap mPanelLoadingMap;
    public PanelSettings mPanelSettings;

    static UIManager sInst = null; 
    public static UIManager GetInst()
    {
        if (sInst != null)
            return sInst;
        GameObject go = GameObject.Find("UIManager");
        if (go != null)
            sInst = go.GetComponent<UIManager>();
        return sInst;
    }

	// Use this for initialization
	void Start () 
    {
        if (C_USE_NEW_UI)
        {
            SetPanelShow("setname");
        }
        else
        {
            SetPanelShow("");
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void ShowMsgBox(string title, string msg, PanelMessageBox.BtnStatus bs, PanelMessageBox.OnOK _okFunc, PanelMessageBox.OnCancel _cancelFunc)
    {
        if (C_USE_NEW_UI )
            mPanelMessageBox.OnShow(title, msg, bs, _okFunc, _cancelFunc);
    }

    public void HideMsgBox()
    {
        if (C_USE_NEW_UI)
            mPanelMessageBox.gameObject.SetActive(false);
    }

    public void ShowLoadingMap(bool show)
    {
        if (C_USE_NEW_UI)
            mPanelLoadingMap.gameObject.SetActive(show);
    }

    public void RefreshLoadingProgress(float progress)
    {
        mPanelLoadingMap.RefreshProgress(progress);
    }

    public void EnterGame()
    {
        mPanelLoadingMap.EnterGame();
    }

    public PanelSettings GetPanelSetting()
    {
        return mPanelSettings;
    }

    public void SetPanelShow(string menuState)
    {
        if (C_USE_NEW_UI == false)
            return;
        mPanelCreateRoom.gameObject.SetActive(false);
        mPanelEnterRoom.gameObject.SetActive(false);
        mPanelInGame.gameObject.SetActive(false);
        mPanelMainMenu.gameObject.SetActive(false);
        mPanelSelectCar.gameObject.SetActive(false);
        mPanelSetPlayerName.gameObject.SetActive(false);
        mPanelSettings.gameObject.SetActive(false);

        switch (menuState)
        {
            // 选择赛车
            case "setavatarstyle":
                {
                    mPanelSelectCar.gameObject.SetActive(true);
                    mPanelSelectCar.OnShow();
                    break;
                }
            // 显示主菜单
            case "menublock":
                {
                    mPanelMainMenu.gameObject.SetActive(true);
                    mPanelMainMenu.OnShow();
                    break;
                }
            // 设置界面
            case "setting":
                {
                    mPanelSettings.gameObject.SetActive(true);
                    mPanelSettings.OnShow();
                    break;
                }
            // 建立比赛房间
            case "hostgame":
                {
                    mPanelCreateRoom.gameObject.SetActive(true);
                    mPanelCreateRoom.OnShow();
                    break;
                }
            // 进入比赛房间
            case "findgame":
                {
                    mPanelEnterRoom.gameObject.SetActive(true);
                    mPanelEnterRoom.OnShow();
                    //FindGame();
                    break;
                }
            // 设置玩家名称
            case "setname":
                {
                    mPanelSetPlayerName.gameObject.SetActive(true);
                    break;
                }
            // 消息出错
            case "networkerror":
                {
                    //MSG_Error();
                    break;
                }
            // 尝试重连
            case "tryingtoconnect":
                {
                    //MSG_TryingToConnect();
                    break;
                }
            case "ingame":
                {
                    mPanelInGame.gameObject.SetActive(true);
                    break;
                }
            // 设置玩家名称
            default:
                //mPanelSetPlayerName.gameObject.SetActive(true);
                break;
        }	
    }
}
