using UnityEngine;
using System.Collections;

public interface InputDevice
{
    void HandleInput();
}

public class InputManager : MonoBehaviour 
{
    public enum InputType
    {
        IT_PC,
        IT_Mobile,
    }

    

    InputDevice inputDevice = null;
    InputType inputType = InputType.IT_PC;
    public InputType CurInputType
    {
        get { return inputType; }
        set
        {
            inputDevice = null;
            if (inputType == InputType.IT_PC)
            {
                inputDevice = GetComponent<InputSettingPC>();
                if( inputDevice == null )
                    inputDevice = this.gameObject.AddComponent<InputSettingPC>();
            }
            else if (inputType == InputType.IT_Mobile)
            {
                inputDevice = GetComponent<InputSettingMobile>();
                if (inputDevice == null)
                    inputDevice = this.gameObject.AddComponent<InputSettingMobile>();
            }
        }
    }

    public bool triggerSteeringLeft;
    public bool triggerSteeringRight;
    public bool triggerThrottle;
    public bool triggerBreak;
    public bool triggerHandBreak;
    public bool triggerNitro;
    public bool triggerShiftUp;
    public bool triggerShiftDown;

    static InputManager sInst;
    public static InputManager GetInst()
    {
        return sInst;
    }

	// Use this for initialization
	void Start () 
    {
        sInst = this;
        CurInputType = inputType;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (inputDevice != null)
            inputDevice.HandleInput();
	}
}
