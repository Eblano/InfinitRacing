using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelMessageBox : MonoBehaviour 
{
    public enum BtnStatus
    {
        BS_None = 0,
        BS_OK = 1,
        BS_Cancel = 2,
        BS_OKCancel = 3,
    };
    public delegate void OnCancel();
    public delegate void OnOK();
    OnCancel cancelFunc;
    OnOK okFunc;

    public Text txtTitle;
    public Text txtMsg;
    public Button btnCancel;
    public Button btnOK;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void OnBtnCancelClicked()
    {
        this.gameObject.SetActive(false);
        if (cancelFunc != null)
            cancelFunc.Invoke();
        cancelFunc = null;
    }

    public void OnBtnOKClicked()
    {
        this.gameObject.SetActive(false);
        if (okFunc != null)
            okFunc.Invoke();
        okFunc = null;
    }

    public void OnShow(string title, string msg, BtnStatus bs, OnOK _okFunc, OnCancel _cancelFunc)
    {
        okFunc = _okFunc;
        cancelFunc = _cancelFunc;
        txtTitle.text = title;
        txtMsg.text = msg;
        btnCancel.gameObject.SetActive((bs & BtnStatus.BS_Cancel) > 0);
        btnOK.gameObject.SetActive((bs & BtnStatus.BS_OK) > 0);
        this.gameObject.SetActive(true);
    }
}
