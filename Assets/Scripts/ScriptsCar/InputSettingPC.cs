using UnityEngine;
using System.Collections;

public class InputSettingPC : MonoBehaviour, InputDevice
{
    public KeyCode keySteeringLeft = KeyCode.A;
    public KeyCode keySteeringRight = KeyCode.D;
    public KeyCode keyThrottle = KeyCode.W;
    public KeyCode keyBreak = KeyCode.S;
    public KeyCode keyHandBreak = KeyCode.Space;
    public KeyCode keyNitro = KeyCode.N;
    public KeyCode keyShiftUp = KeyCode.UpArrow;
    public KeyCode KeyShiftDown = KeyCode.DownArrow;

    InputManager im;

	// Use this for initialization
	void Start () 
    {
	    im = GetComponent<InputManager>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    public void HandleInput()
    {
        im.triggerSteeringLeft = Input.GetKey(keySteeringLeft);
        im.triggerSteeringRight = Input.GetKey(keySteeringRight);
        im.triggerThrottle = Input.GetKey(keyThrottle);
        im.triggerBreak = Input.GetKey(keyBreak);
        im.triggerHandBreak = Input.GetKey(keyHandBreak);
        im.triggerNitro = Input.GetKey(keyNitro);
        im.triggerShiftUp = Input.GetKeyDown(keyShiftUp);
        im.triggerShiftDown = Input.GetKeyDown(KeyShiftDown);
    }
}
