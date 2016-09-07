using UnityEngine;
using System.Collections;

public class PlyerInit : MonoBehaviour {


	void Start () {
		if(networkView.isMine)
			gameObject.name = "Local Player";
		else
			gameObject.name = "Remote Player";
	}

}
