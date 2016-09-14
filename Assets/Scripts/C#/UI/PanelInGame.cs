using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelInGame : MonoBehaviour 
{
    public Text speedText;

    CarCameraController carCamCtrl;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (UIManager.GetInst().MainPlayerCarCtrl != null)
            speedText.text = (UIManager.GetInst().MainPlayerCarCtrl.rigidbody.velocity.magnitude * 3.6f).ToString();
        else
            speedText.text = "0";
	}

    public void OnBtnCamClicked()
    {
        if (carCamCtrl == null)
        {
            GameObject go = GameObject.Find("Car Camera");
            if (go != null)
                carCamCtrl = go.GetComponent<CarCameraController>();
        }
        if (carCamCtrl != null)
            carCamCtrl.ToggleCamera();
    }

    public void OnBtnPauseClicked()
    {
        UIManager.GetInst().InGameUI.TogglePauseChange();
    }

    public void OnBtnStopServerClicked()
    {
        UIManager.GetInst().InGameUI.OnQuit();
        UIManager.GetInst().SetPanelShow("setname");
    }
}
