using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour 
{
    public LapCollector lapCollector;
    public int index = -1;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter(Collider col)
    {
        Transform topParentTrans = col.transform;
        while (topParentTrans.parent != null)
        {
            topParentTrans = topParentTrans.parent;
        }
        CarNetworkInitWrapper netwrap = topParentTrans.GetComponent<CarNetworkInitWrapper>();
        if (netwrap != null)
        {
            // 检测到本地玩家进入计时触发器
            if (netwrap.netPlayer == Network.player)
            {
                // 通知服务器本地玩家控制的车辆进入某个触发器 
                lapCollector.networkView.RPC("LocalPlayerTriggerEnterC2S", RPCMode.AllBuffered, Network.player, index);
            }
        }
    }


}
