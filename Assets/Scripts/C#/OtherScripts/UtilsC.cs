
using UnityEngine;
using System.Collections;

public enum MotionStates{
	On_Pause,
	Idle,
	Run,
	Walk_Forward,
	Walk_Back,
	Walk_Left,
	Walk_Right,
	Jump,
}

public struct  PlayerInfo{
	public string username;
	public NetworkPlayer player;
}

public class UtilsC {
	#region bool
	public static bool IsStringCorrect(string str){
		for (int i = 0; i < str.Length; i++){
		  if (str[i] != ' ')
		  	 return true;
		}
		return false;
	}
	
	public static bool CheckPeerType(NetworkPeerType peerType){
		if(peerType == Network.peerType)
			return true;
		return false;
	}
	
	public static bool IsWebPlayer () {
		return (Application.platform == RuntimePlatform.WindowsWebPlayer ||Application.platform == RuntimePlatform.OSXWebPlayer);
	}
	
	public static bool IsLoading(string level){
		if(Application.GetStreamProgressForLevel(level) != 1) 
			return true;
		return false;
	}
	
	public static bool IsHostsExists(){
		if(MasterServer.PollHostList().Length !=0)
			return true;
		return false;
	}
	#endregion
	
	public static string CreateRandomString(int _length){
	  	string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
	  	char[] chars = new char[_length];
		for (int i = 0; i < _length; i++) 
		 	 chars[i] = allowedChars[Random.Range(0, allowedChars.Length)];
		return new string(chars);
	}
	
	public static void SetReceivingEnabled(int nw_group, bool state){
	    foreach (NetworkPlayer player in Network.connections) 
            Network.SetReceivingEnabled(player, nw_group, state);
	}
	
	public static GameObject FindClosestObjectWithTag(string tag, Vector3 toPosition){
	 	GameObject closestObject = null; 
	    float minDist = Mathf.Infinity; 
	    GameObject[] objects = GameObject.FindGameObjectsWithTag(tag); 
	  	
		foreach(GameObject obj in objects)  { 
	    	float currDist = Vector3.Distance(obj.transform.position, toPosition);
	        
	        if (currDist < minDist) { 
	           	 closestObject = obj; 
				 minDist = currDist; 
			} 
	    } 
	    return closestObject;
	}

}

