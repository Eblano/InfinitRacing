using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelInGame : MonoBehaviour 
{
    public Text speedText;
    public Text gearText;

    public Text rankingText;
    public Text lapText;
    public Text bestLapTimeText;

    public RectTransform nitroTrans;

    CarCameraController carCamCtrl;
    public LapCollector lapCollector;

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

        float maxWidth = (nitroTrans.parent as RectTransform).rect.width;
        if (UIManager.GetInst().MainPlayerCarCtrl != null)
        {
            Drivetrain dt = UIManager.GetInst().MainPlayerCarCtrl.GetComponent<Drivetrain>();
            nitroTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dt.curNitroContainer / dt.NitroContainerMax * maxWidth);
        }
        else
            nitroTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);

        lapText.text = "";
        rankingText.text = "";
        bestLapTimeText.text = "";

        if (lapCollector != null)
        {
            float bestLapTime = -1.0f;
            SinglePlayerLapInfo pi = lapCollector.GetLocalPlayerLapInfo(ref bestLapTime);
            if (pi != null)
            {
                if (lapCollector.maxLap <= 0)
                    lapText.text = "Lap " + (pi.mLapIndex + 1).ToString();
                else
                    lapText.text = "Lap " + (pi.mLapIndex + 1).ToString() + " / " + lapCollector.maxLap.ToString();

                float lapTime = Time.realtimeSinceStartup - pi.mStartTimeStamp;
                if( lapTime < 60.0f )
                    rankingText.text = lapTime.ToString("#0.00");
                else if (lapTime < 3600.0f)
                {
                    int m = (int)lapTime / 60;
                    float s = lapTime - m * 60.0f;
                    rankingText.text = m.ToString() + ":" + s.ToString("#0.00");
                }

                if (bestLapTime > 0.0f)
                {
                    if (bestLapTime < 60.0f)
                        bestLapTimeText.text = bestLapTime.ToString("#0.00");
                    else if (bestLapTime < 3600.0f)
                    {
                        int m = (int)bestLapTime / 60;
                        float s = bestLapTime - m * 60.0f;
                        bestLapTimeText.text = m.ToString() + ":" + s.ToString("#0.00");
                    }
                }
            }
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
        //UIManager.GetInst().SetPanelShow("setname");
        //UIManager.GetInst().SetPanelShow("setavatarstyle");
    }

    public void OnBtnResetClicked()
    {
        if (UIManager.GetInst().MainPlayerCarCtrl != null)
        {
            CarNetworkInitWrapper car = UIManager.GetInst().MainPlayerCarCtrl.GetComponent<CarNetworkInitWrapper>();
            if (car != null)
            {
                Quaternion q = new Quaternion();
                q.SetLookRotation(car.transform.forward, Vector3.up);
                car.networkView.RPC("ResetPlayerTransformS2C", RPCMode.AllBuffered, car.netPlayer, car.transform.position, q);
            }
        }
    }
}
