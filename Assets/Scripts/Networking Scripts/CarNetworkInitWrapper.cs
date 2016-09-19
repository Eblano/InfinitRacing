using UnityEngine;
using System.Collections;

public class CarNetworkInitWrapper : MonoBehaviour 
{

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
}
