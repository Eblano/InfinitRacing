using UnityEngine;
using System.Collections;

public class Selectcolor : MonoBehaviour {

public Material go;
	
public Material[] mats;
	
private int index = 0;

// Use this for initialization
void Start () {
    go = mats[index];
}

void OnGUI(){
    GUILayout.BeginArea(new Rect(Screen.width/2-100,Screen.height-60,200,50));
    GUI.Box (new Rect(10,10,190,40),"");

    GUI.Label(new Rect(62,20,100,20),"Wall Testing"+(index +1));

    if(GUI.Button (new Rect(15,15,30,30),"<<")){
        index--;
        if(index<0){
            index = mats.Length - 1;

        }
        go = mats[index];
    }

    if(GUI.Button (new Rect(165,15,30,30),">>")){
        index++;
        if(index > mats.Length -1){
            index = 0;

        }
        go = mats[index];
    }
    GUILayout.EndArea();
}
}
