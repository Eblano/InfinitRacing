using UnityEngine;
using System.Collections;

public class CarNetworkInitWrapper : MonoBehaviour 
{
    public NetworkPlayer netPlayer;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnNetworkInstantiate(NetworkMessageInfo msg) 
    {
        //netPlayer = msg.sender;
        netPlayer = networkView.owner;
        NetworkConnection.GetInst().playerObjMap.Add(netPlayer, this);

	    // This is our own player
	    if (networkView.isMine)
	    {
		    Camera.main.SendMessage("SetTarget", transform);
            NetworkRigidbody comp = GetComponent("NetworkRigidbody") as NetworkRigidbody;
            comp.enabled = false;
	    }
	    // This is just some remote controlled player, don't execute direct
	    // user input on this
	    else
	    {
		    name += "Remote";
            CarController cc = GetComponent("CarController") as CarController;
            cc.enabled = false;
            NetworkRigidbody comp = GetComponent("NetworkRigidbody") as NetworkRigidbody;
            comp.enabled = true;
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cam.enabled = false;
            Camera.main.SendMessage("ApplyCameraSetting");
	    }
    }

    [RPC]
    public void ResetPlayerTransformS2C(NetworkPlayer player, Vector3 pos, Quaternion rot)
    {
        // 当玩家匹配的时候才执行transform重置
        if (player == Network.player && netPlayer == player)
        {
            transform.position = pos;
            transform.rotation = rot;
            transform.rigidbody.velocity = new Vector3(0, 0, 0);
            transform.rigidbody.angularVelocity = new Vector3(0, 0, 0);
        }
    }
}
