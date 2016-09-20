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
    }
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
    public CheckPoint[] checkPointList;

    PlayerLapInfo playerLapInfos;

	// Use this for initialization
	void Start () 
    {
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
            laps.Add(li);

            // 通知客户端更新游戏信息
            NetworkConnection.GetInst().networkView.RPC("PlayerLapInfoChangedS2C", RPCMode.AllBuffered, car.netPlayer, li, true);            
        }
        else
        {
            li = laps[laps.Count - 1];
            // next check point
            if (cp.index - li.mLastCheckPoint == 1 )
            {
                li.mLastCheckPoint = cp.index;

                // 通知客户端更新游戏信息
                NetworkConnection.GetInst().networkView.RPC("PlayerLapInfoChangedS2C", RPCMode.AllBuffered, car.netPlayer, li, false);  
            }
            // from the last check point to the first check point, one lap completed
            else if(li.mLastCheckPoint == checkPointList.Length - 1 && cp.index == 0)
            {
                li.mCompleted = true;
                li.mCompletedTimeStamp = Time.realtimeSinceStartup;
                li.mLapTime = li.mCompletedTimeStamp - li.mStartTimeStamp;
                // 通知客户端更新游戏信息
                NetworkConnection.GetInst().networkView.RPC("PlayerLapInfoChangedS2C", RPCMode.AllBuffered, car.netPlayer, li, false);  

                SinglePlayerLapInfo newLap = new SinglePlayerLapInfo();
                newLap.mStartTimeStamp = Time.realtimeSinceStartup;
                newLap.mCompleted = false;
                newLap.mLapTime = 0.0f;
                newLap.mLastCheckPoint = cp.index;
                newLap.mCompletedTimeStamp = 0.0f;
                laps.Add(newLap);
                // 通知客户端更新游戏信息
                NetworkConnection.GetInst().networkView.RPC("PlayerLapInfoChangedS2C", RPCMode.AllBuffered, car.netPlayer, newLap, true);   
            }
            // reverse lap 
            else if(cp.index - li.mLastCheckPoint == -1 ||
                li.mLastCheckPoint == 0 && cp.index == checkPointList.Length - 1)
            {
                // 通知客户端车辆位置重置
                NetworkConnection.GetInst().networkView.RPC("ResetPlayerTransformS2C", RPCMode.AllBuffered, car.netPlayer, cp.transform);
            }
        }
    }

    [RPC]
    void PlayerLapInfoChangedS2C( NetworkPlayer player, SinglePlayerLapInfo info, bool isAdd )
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
            if (isAdd)
            {
                SinglePlayerLapInfo newLap = new SinglePlayerLapInfo();
                newLap.CopyFrom(info);
                laps.Add(newLap);
            }
            else
            {
                SinglePlayerLapInfo dstLap = laps[laps.Count - 1];
                dstLap.CopyFrom(info);
            }
        }
    }
}
