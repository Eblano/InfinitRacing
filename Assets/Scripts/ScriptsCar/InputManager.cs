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


    InputSettingMobile inputMobile = null;
    InputSettingPC inputPC = null;
    InputDevice curInputDevice = null;
    InputType inputType = InputType.IT_PC;
    public InputType CurInputType
    {
        get { return inputType; }
        set
        {
            if (inputPC == null)
            {
                inputPC = GetComponent<InputSettingPC>();
                if (inputPC == null)
                    inputPC = this.gameObject.AddComponent<InputSettingPC>();
            }
            if (inputMobile == null)
            {
                inputMobile = GetComponent<InputSettingMobile>();
                if (inputMobile == null)
                    inputMobile = this.gameObject.AddComponent<InputSettingMobile>();
            }

            if (inputType == InputType.IT_Mobile)
            {
                curInputDevice = inputMobile;
                inputMobile.enabled = true;
                inputPC.enabled = false;
            }
            else if (inputType == InputType.IT_PC)
            {
                curInputDevice = inputPC;
                inputMobile.enabled = false;
                inputPC.enabled = true;
            }
        }
    }
    public InputSettingPC GetInputPC()
    {
        return inputPC;
    }
    public InputSettingMobile GetInputMobile()
    {
        return inputMobile;
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
        if (curInputDevice != null)
            curInputDevice.HandleInput();
	}
}
