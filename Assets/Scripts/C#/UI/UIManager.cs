using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour 
{
    public const bool C_USE_NEW_UI = false;

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
    public PanelInGame mPanelInGame;
    public PanelMainMenu mPanelMainMenu;
    public PanelSelectCar mPanelSelectCar;
    public PanelSetPlayerName mPanelSetPlayerName;

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

    public void SetPanelShow(string menuState)
    {
        mPanelCreateRoom.gameObject.SetActive(false);
        mPanelInGame.gameObject.SetActive(false);
        mPanelMainMenu.gameObject.SetActive(false);
        mPanelSelectCar.gameObject.SetActive(false);
        mPanelSetPlayerName.gameObject.SetActive(false);

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
                mPanelMainMenu.gameObject.SetActive(true);
                break;
            // 建立比赛房间
            case "hostgame":
                mPanelCreateRoom.gameObject.SetActive(true);
                break;
            // 进入比赛房间
            case "findgame": 
                //FindGame();
                break;
            // 设置玩家名称
            case "setname":
                mPanelSetPlayerName.gameObject.SetActive(true);
                break;
            // 消息出错
            case "networkerror": 
                //MSG_Error();
                break;
            // 尝试重连
            case "tryingtoconnect": 
                //MSG_TryingToConnect();
                break;
            case "ingame":
                mPanelInGame.gameObject.SetActive(true);
                break;
            // 设置玩家名称
            default:
                //mPanelSetPlayerName.gameObject.SetActive(true);
                break;
        }	
    }
}
