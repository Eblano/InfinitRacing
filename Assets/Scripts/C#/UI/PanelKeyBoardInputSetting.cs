using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PanelKeyBoardInputSetting : MonoBehaviour 
{
    public GameObject itemPrefab;
    public RectTransform transKeyBoardSettingPanel;
    public Scrollbar scrollBar;
    bool hasInit = false;
    float iniWidth;
    float iniHeight;
    float itemHeight;
    List<SingleKeyBoardItem> items = new List<SingleKeyBoardItem>();

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
        if (hasInit == false)
        {
            iniWidth = transKeyBoardSettingPanel.rect.width;
            iniHeight = transKeyBoardSettingPanel.rect.height;

            while (transKeyBoardSettingPanel.childCount > 0)
            {
                Transform ct = transKeyBoardSettingPanel.GetChild(0);
                ct.SetParent(null);
                GameObject.DestroyImmediate(ct.gameObject);
            }
            items.Clear();

            transKeyBoardSettingPanel.anchoredPosition = new Vector2(0, 0);
            InputSettingPC input = InputManager.GetInst().GetInputPC();
            CreateItem("左转", "keySteeringLeft", input.keySteeringLeft);
            CreateItem("右转", "keySteeringRight", input.keySteeringRight);
            CreateItem("油门", "keyThrottle", input.keyThrottle);
            CreateItem("刹车", "keyBreak", input.keyBreak);
            CreateItem("手刹", "keyHandBreak", input.keyHandBreak);
            CreateItem("氮气", "keyNitro", input.keyNitro);
            CreateItem("提档", "keyShiftUp", input.keyShiftUp);
            CreateItem("降档", "KeyShiftDown", input.KeyShiftDown);

            float maxHeight = transKeyBoardSettingPanel.childCount * itemHeight;
            if (maxHeight < iniHeight)
                transKeyBoardSettingPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iniHeight);
            else
                transKeyBoardSettingPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);

            scrollBar.value = 1;
            hasInit = true;
        }
    }

    void CreateItem( string keyDesc, string keyName, KeyCode key )
    {   
        GameObject go = GameObject.Instantiate(itemPrefab) as GameObject;
        go.transform.SetParent(transKeyBoardSettingPanel);
        RectTransform childTrans = go.transform as RectTransform;
        itemHeight = childTrans.rect.height;
        float y = itemHeight * items.Count;
        childTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transKeyBoardSettingPanel.rect.width);
        childTrans.anchoredPosition = new Vector2(0, -y);
        SingleKeyBoardItem item = go.GetComponent<SingleKeyBoardItem>();
        item.Init(keyDesc, key, keyName);
        item.itemIndex = items.Count;

        items.Add(item);
    }

    public void OnClickItemBtn( int index )
    {
        for (int i = 0; i < items.Count; ++i)
        {
            if (i != index)
            {
                items[i].isModifying = false;
                items[i].ApplySetting();
            }
        }
    }
}
