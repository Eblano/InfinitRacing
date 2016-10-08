using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SingleKeyBoardItem : MonoBehaviour 
{
    public Text txtDesc;
    public Text txtKey;
    public Text txtBtn;
    public bool isModifying = false;
    public string[] btnTxt;
    string keyName;
    KeyCode curKey;
    public int itemIndex = -1;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        txtBtn.text = isModifying ? btnTxt[1] : btnTxt[0];
        txtBtn.color = isModifying ? Color.blue : Color.black;
	}

    void OnGUI()
    {
        if (isModifying && Input.anyKeyDown)
        {
            Event e = Event.current;
            if (e != null && e.isKey)
            {
                curKey = e.keyCode;
                txtKey.text = curKey.ToString();
            }
        }
    }

    public void OnBtnClicked()
    {
        if (isModifying)
        {
            ApplySetting();
        }
        isModifying = !isModifying;
        if (isModifying)
            UIManager.GetInst().GetPanelSetting().keyBoardSetting.OnClickItemBtn(itemIndex);
    }

    public void Init(string _desc, KeyCode _key, string _keyName)
    {
        txtDesc.text = _desc;
        txtKey.text = _key.ToString();
        keyName = _keyName;
        curKey = _key;
    }

    public void ApplySetting()
    {
        ReflectionHelper.SetProperty(InputManager.GetInst().GetInputPC(), keyName, curKey);
    }
}
