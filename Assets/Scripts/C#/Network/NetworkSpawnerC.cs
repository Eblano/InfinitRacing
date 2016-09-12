using UnityEngine;
using System.Collections;

[System.Serializable]
public class  SpawnInfo
{
	public string name;
	public Transform actor;
	public Transform[] spawns;
}

public class NetworkSpawnerC : MonoBehaviour 
{	
	public SpawnInfo[] spawnInfo;
	private GameObject CarSel;
	
	void Start ()
    {
	    Screen.showCursor = true;	
	}

	void Update () 
    {
/*		
		if (Network.isServer){
			if(Input.GetKeyDown(KeyCode.F))
				SpawnNetworkActor(spawnInfo[2].actor, spawnInfo[2].spawns);
		}
*/		
	}

	void OnNetworkLoadedLevel()
    {
        CarSel = GameObject.Find("Network");		
		if (CarSel.gameObject.tag == "Player1")
        {
		    SpawnNetworkActor(spawnInfo[0].actor, spawnInfo[0].spawns);
	    }
		else if (CarSel.gameObject.tag == "Player2")
        {
		    SpawnNetworkActor(spawnInfo[1].actor, spawnInfo[1].spawns);
	    }
		else if (CarSel.gameObject.tag == "Player3")
        {
		    SpawnNetworkActor(spawnInfo[2].actor, spawnInfo[2].spawns);
        }
		else if (CarSel.gameObject.tag == "Player4")
        {
		    SpawnNetworkActor(spawnInfo[3].actor, spawnInfo[3].spawns);
        }
		else if (CarSel.gameObject.tag == "Player5")
        {
		    SpawnNetworkActor(spawnInfo[4].actor, spawnInfo[4].spawns);
        }
    }

	void SpawnNetworkActor(Transform networkActor,Transform[] spawnPoints)
    {
		int spIndex= Random.Range(0, spawnPoints.Length);
		Network.Instantiate(networkActor, spawnPoints[spIndex].position, spawnPoints[spIndex].rotation, 0);
	}

}
	
/*	
	void OnGUI(){
		if (Network.isServer){
			GUILayout.Space(330);
			GUILayout.Label("Press F button to spawn AI");
		}
	}*/
