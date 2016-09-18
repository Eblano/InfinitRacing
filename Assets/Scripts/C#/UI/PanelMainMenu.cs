using UnityEngine;
using System.Collections;

public class PanelMainMenu : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void OnBtnCreateRoomClicked()
    {
        UIManager.GetInst().SetPanelShow("hostgame");
    }

    public void OnBtnEnterRoomClicked()
    {
        UIManager.GetInst().SetPanelShow("findgame");	
    }

    public void OnBtnChangeNameClicked()
    {
        UIManager.GetInst().SetPanelShow("setname");	
    }

    public void OnBtnSelectCarClicked()
    {
        UIManager.GetInst().SetPanelShow("setavatarstyle");	
    }

    public void OnBtnExitClicked()
    {
        Application.Quit();		
    }
}
