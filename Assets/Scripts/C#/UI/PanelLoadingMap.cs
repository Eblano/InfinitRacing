using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelLoadingMap : MonoBehaviour 
{
    public Text txtLoading;
    public RectTransform progressTrans;
    float maxWidth;
    float curProgress;
    bool needEnterGame = false;

	// Use this for initialization
	void Start () 
    {
        maxWidth = (progressTrans.parent as RectTransform).rect.width;
	}
	
	// Update is called once per frame
	void Update () 
    {
        txtLoading.text = "Loading " + (int)(curProgress * 100.0f) + "%";
        progressTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, curProgress * maxWidth);
	    if( needEnterGame )
        {
            needEnterGame = false;
            UIManager.GetInst().ShowLoadingMap(false);
            UIManager.GetInst().SetPanelShow("ingame");
        }
    }

    public void EnterGame()
    {
        needEnterGame = true;
        System.Threading.Thread.Sleep(1000);
    }

    public void RefreshProgress(float progress)
    {
        curProgress = progress;
        Update();
    }
}
