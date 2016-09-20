using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelSetPlayerName : MonoBehaviour 
{
    NetworkConnection networkConnection;

    public Text playerName;

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
	
	}

    public void OnBtnOKClicked()
    {
        networkConnection.playerName = playerName.text;
        if (UtilsC.IsStringCorrect(networkConnection.playerName))
        {
            PlayerPrefs.SetString("playerName", networkConnection.playerName);
            UIManager.GetInst().SetPanelShow("setavatarstyle");
        }
    }

    public void OnBtnQuitClicked()
    {
        Application.Quit();
    }
}
