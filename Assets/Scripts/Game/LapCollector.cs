using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SinglePlayerLapInfo
{
    public bool mCompleted = false;
    //public CheckPoint mLastCheckPoint = null;
    public int mLastCheckPoint = -1;
    public float mLapTime = 0.0f;
    public float mStartTimeStamp;
    public float mCompletedTimeStamp;
    public int mLapIndex = -1;

    public SinglePlayerLapInfo()
    {
    }

    public void CopyFrom(SinglePlayerLapInfo other)
    {
        mCompleted = other.mCompleted;
        mLastCheckPoint = other.mLastCheckPoint;
        mLapTime = other.mLapTime;
        mStartTimeStamp = other.mStartTimeStamp;
        mCompletedTimeStamp = other.mCompletedTimeStamp;
        mLapIndex = other.mLapIndex;
    }

    //public static object[] Pack(SinglePlayerLapInfo info)
    //{
    //    List<object> res = new List<object>();
    //    res.Add(info.mCompleted);
    //    res.Add(info.mLastCheckPoint);
    //    res.Add(info.mLapTime);
    //    res.Add(info.mStartTimeStamp);
    //    res.Add(info.mCompletedTimeStamp);
    //    res.Add(info.mLapIndex);
    //    return res.ToArray();
    //}

    //public static SinglePlayerLapInfo UnPack(object[] data)
    //{
    //    SinglePlayerLapInfo info = new SinglePlayerLapInfo();
    //    info.mCompleted = (bool)(data[0]);
    //    info.mLastCheckPoint = (int)(data[1]);
    //    info.mLapTime = (float)(data[2]);
    //    info.mStartTimeStamp = (float)(data[3]);
    //    info.mCompletedTimeStamp = (float)(data[4]);
    //    info.mLapIndex = (int)(data[5]);
    //    return info;
    //}
}

public class PlayerLapInfo
{
    public Dictionary<NetworkPlayer, List<SinglePlayerLapInfo>> lapInfos = new Dictionary<NetworkPlayer,List<SinglePlayerLapInfo>>();

    public PlayerLapInfo()
    {

    }

    public void Clear()
    {
        foreach (NetworkPlayer player in lapInfos.Keys)
        {
            lapInfos[player].Clear();
        }
        lapInfos.Clear();
    }
}

public class LapCollector : MonoBehaviour 
{
    public int maxLap = -1;
    public CheckPoint[] checkPointList;

    PlayerLapInfo playerLapInfos;

	// Use this for initialization
	void Start () 
    {
        UIManager.GetInst().mPanelInGame.lapCollector = this;

        playerLapInfos = new PlayerLapInfo();
        for (int i = 0; i < checkPointList.Length; ++i)
        {
            checkPointList[i].index = i;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	}

    public SinglePlayerLapInfo GetLocalPlayerLapInfo(ref float bestLapTime)
    {
        bestLapTime = -1.0f;
        if (playerLapInfos.lapInfos.ContainsKey(Network.player))
        {
            List<SinglePlayerLapInfo> laps = playerLapInfos.lapInfos[Network.player];
            if (laps.Count > 0)
            {
                for (int i = 0; i < laps.Count; ++i)
                {
                    if (laps[i].mCompleted)
                    {
                        if (bestLapTime == -1.0f)
                            bestLapTime = laps[i].mLapTime;
                        else if( bestLapTime > laps[i].mLapTime )
                            bestLapTime = laps[i].mLapTime;
                    }
                }
                return laps[laps.Count - 1];
            }
        }
        return null;
    }

    public void OnGameStart()
    {
        playerLapInfos.Clear();
    }

    public void OnCarEnterCheckPoint( NetworkPlayer player, int cpIndex )
    {
        CarNetworkInitWrapper comp = NetworkConnection.GetInst().playerObjMap[player];
        CheckPoint cp = checkPointList[cpIndex];
        OnCarEnterCheckPoint(comp, cp);
    }

    public void OnCarEnterCheckPoint( CarNetworkInitWrapper car, CheckPoint cp )
    {
        List<SinglePlayerLapInfo> laps = null;
        if (playerLapInfos.lapInfos.ContainsKey(car.netPlayer))
            laps = playerLapInfos.lapInfos[car.netPlayer];
        else
        {
            laps = new List<SinglePlayerLapInfo>();
            playerLapInfos.lapInfos.Add(car.netPlayer, laps);
        }
        SinglePlayerLapInfo li = null;
        if (laps.Count == 0)
        {
            li = new SinglePlayerLapInfo();
            li.mStartTimeStamp = Time.realtimeSinceStartup;
            li.mCompleted = false;
            li.mLapTime = 0.0f;
            li.mLastCheckPoint = cp.index;
            li.mCompletedTimeStamp = 0.0f;
            li.mLapIndex = laps.Count;
            laps.Add(li);

            // 通知客户端更新游戏信息
            networkView.RPC("PlayerLapInfoChangedS2C", RPCMode.AllBuffered, car.netPlayer,
                li.mCompleted,
                li.mLastCheckPoint,
                li.mLapTime,
                li.mStartTimeStamp,
                li.mCompletedTimeStamp,
                li.mLapIndex,
                true);            
        }
        else
        {
            li = laps[laps.Count - 1];
            // next check point
            if (cp.index - li.mLastCheckPoint == 1 )
            {
                li.mLastCheckPoint = cp.index;

                // 通知客户端更新游戏信息
                networkView.RPC("PlayerLapInfoChangedS2C", RPCMode.AllBuffered, car.netPlayer,
                    li.mCompleted,
                    li.mLastCheckPoint,
                    li.mLapTime,
                    li.mStartTimeStamp,
                    li.mCompletedTimeStamp,
                    li.mLapIndex,
                    false);  
            }
            // from the last check point to the first check point, one lap completed
            else if(li.mLastCheckPoint == checkPointList.Length - 1 && cp.index == 0)
            {
                li.mCompleted = true;
                li.mCompletedTimeStamp = Time.realtimeSinceStartup;
                li.mLapTime = li.mCompletedTimeStamp - li.mStartTimeStamp;
                // 通知客户端更新游戏信息
                networkView.RPC("PlayerLapInfoChangedS2C", RPCMode.AllBuffered, car.netPlayer,
                    li.mCompleted,
                    li.mLastCheckPoint,
                    li.mLapTime,
                    li.mStartTimeStamp,
                    li.mCompletedTimeStamp,
                    li.mLapIndex,
                    false);  

                SinglePlayerLapInfo newLap = new SinglePlayerLapInfo();
                newLap.mStartTimeStamp = Time.realtimeSinceStartup;
                newLap.mCompleted = false;
                newLap.mLapTime = 0.0f;
                newLap.mLastCheckPoint = cp.index;
                newLap.mCompletedTimeStamp = 0.0f;
                newLap.mLapIndex = laps.Count;
                laps.Add(newLap);
                // 通知客户端更新游戏信息
                networkView.RPC("PlayerLapInfoChangedS2C", RPCMode.AllBuffered, car.netPlayer,
                    newLap.mCompleted,
                    newLap.mLastCheckPoint,
                    newLap.mLapTime,
                    newLap.mStartTimeStamp,
                    newLap.mCompletedTimeStamp,
                    newLap.mLapIndex,
                    true);   
            }
            // reverse lap 
            else if(cp.index - li.mLastCheckPoint == -1 ||
                li.mLastCheckPoint == 0 && cp.index == checkPointList.Length - 1)
            {
                // 通知客户端车辆位置重置
                car.networkView.RPC("ResetPlayerTransformS2C", RPCMode.AllBuffered, car.netPlayer, cp.transform.position, cp.transform.rotation);
            }
        }
    }

    [RPC]
    void PlayerLapInfoChangedS2C(NetworkPlayer player, 
        bool mCompleted, 
        int mLastCheckPoint, 
        float mLapTime, 
        float mStartTimeStamp, 
        float mCompletedTimeStamp, 
        int mLapIndex,
        bool isAdd)
    {
        if (Network.isClient)
        {
            List<SinglePlayerLapInfo> laps = null;
            if( playerLapInfos.lapInfos.ContainsKey(player) )
                laps = playerLapInfos.lapInfos[player];
            else
            {
                laps = new List<SinglePlayerLapInfo>();
                playerLapInfos.lapInfos.Add( player, laps );
            }
            SinglePlayerLapInfo lapi = null;
            if (isAdd)
            {
                lapi = new SinglePlayerLapInfo();
                laps.Add(lapi);
            }
            else
            {
                lapi = laps[laps.Count - 1];
            }
            lapi.mCompleted = mCompleted;
            lapi.mLastCheckPoint = mLastCheckPoint;
            lapi.mLapTime = mLapTime;
            lapi.mStartTimeStamp = mStartTimeStamp;
            lapi.mCompletedTimeStamp = mCompletedTimeStamp;
            lapi.mLapIndex = mLapIndex;
        }
    }

    [RPC]
    void LocalPlayerTriggerEnterC2S(NetworkPlayer player, int checkPointIndex)
    {
        // 在服务器端执行
        if (UtilsC.CheckPeerType(NetworkPeerType.Server))
        {
            OnCarEnterCheckPoint(player, checkPointIndex);
        }
    }
}
