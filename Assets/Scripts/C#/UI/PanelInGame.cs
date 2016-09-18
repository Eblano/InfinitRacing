using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelInGame : MonoBehaviour 
{
    public Text speedText;
    public Text gearText;

    CarCameraController carCamCtrl;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (carCamCtrl == null)
        {
            GameObject go = GameObject.Find("Car Camera");
            if (go != null)
                carCamCtrl = go.GetComponent<CarCameraController>();
        }

        if (UIManager.GetInst().MainPlayerCarCtrl != null)
        {
            int speed = (int)(UIManager.GetInst().MainPlayerCarCtrl.rigidbody.velocity.magnitude * 3.6f);
            speedText.text = speed.ToString() + " kmh";
            gearText.text = "[" + UIManager.GetInst().MainPlayerCarCtrl.CurGear.ToString() + "]";
        }
        else
        {
            speedText.text = "0 kmh";
            gearText.text = "[1]";
        }
	}

    public void OnBtnCamClicked()
    {
        if (carCamCtrl != null)
            carCamCtrl.ToggleCamera();
    }

    public void OnBtnPauseClicked()
    {
        UIManager.GetInst().InGameUI.TogglePauseChange();
    }

    public void OnBtnExitClicked()
    {
        UIManager.GetInst().InGameUI.OnQuit();
        UIManager.GetInst().SetPanelShow("setname");
    }
}
