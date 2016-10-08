using UnityEngine;
using System.Collections;

public class PanelSettings : MonoBehaviour 
{
    public enum PageType
    {
        PT_KeyBoard,
    }

    public PanelKeyBoardInputSetting keyBoardSetting;
    public PageType curPage = PageType.PT_KeyBoard;
    public string lastPanelTag = "";

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void OnShow()
    {

        switch (curPage)
        {
            case PageType.PT_KeyBoard:
                {
                    keyBoardSetting.gameObject.SetActive(true);
                    keyBoardSetting.OnShow();
                    break;
                }
        }

    }

    public void OnBtnKeyBoardClicked()
    {
        curPage = PageType.PT_KeyBoard;
        OnShow();
    }

    public void OnBtnBackClicked()
    {
        UIManager.GetInst().SetPanelShow(lastPanelTag);
    }
}
