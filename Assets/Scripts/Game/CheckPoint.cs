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
        CarNetworkInitWrapper netwrap = col.GetComponent<CarNetworkInitWrapper>();
        if (netwrap != null)
        {
            // 检测到本地玩家进入计时触发器
            if (networkView.isMine && netwrap.netPlayer == Network.player)
            {
                // 通知服务器本地玩家控制的车辆进入某个触发器 
                networkView.RPC("LocalPlayerTriggerEnterC2S", RPCMode.AllBuffered, Network.player, index);
            }
        }
    }

    [RPC]
    void LocalPlayerTriggerEnterC2S( NetworkPlayer player, int checkPointIndex )
    {
        // 在服务器端执行
        if (UtilsC.CheckPeerType(NetworkPeerType.Server))
        {
            lapCollector.OnCarEnterCheckPoint(player, checkPointIndex);
        }
    }
}
