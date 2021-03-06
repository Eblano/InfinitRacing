using UnityEngine;
using System.Collections;

// This class is repsonsible for controlling inputs to the car.
// Change this code to implement other input types, such as support for analogue input, or AI cars.
// 这个类主要负责车辆的输入控制
// 更改此代码来实现其他输入类型，如模拟输入或人工智能汽车的支持。
[RequireComponent (typeof (Drivetrain))]
public class CarController : MonoBehaviour 
{

	// Add all wheels of the car here, so brake and steering forces can be applied to them.
	public Wheel[] wheels;
	
	// A transform object which marks the car's center of gravity.
	// Cars with a higher CoG tend to tilt more in corners.
	// The further the CoG is towards the rear of the car, the more the car tends to oversteer. 
	// If this is not set, the center of mass is calculated from the colliders.
    // 指定汽车的重心
    // 如果没有指定的话,车辆重心将由碰撞体的质心来计算
	public Transform centerOfMass;

	// A factor applied to the car's inertia tensor. 
	// Unity calculates the inertia tensor based on the car's collider shape.
	// This factor lets you scale the tensor, in order to make the car more or less dynamic.
	// A higher inertia makes the car change direction slower, which can make it easier to respond to.
    // 适用于车辆惯性张量的因子
    // unity使用车辆碰撞体计算惯性张量
    // 这个因子用于缩放惯性张量,可以让车辆有更多/更少的动态性
    // 惯性张量越大,车辆转向越慢,反之则越快
	public float inertiaFactor = 1.5f;

	// current input state
	float brake;
	float throttle;
	float throttleInput;
	float steering;
	float lastShiftTime = -1;
	float handbrake;
		
	// cached Drivetrain reference
	Drivetrain drivetrain;
	
	// How long the car takes to shift gears
	public float shiftSpeed = 0.8f;
	

	// These values determine how fast throttle value is changed when the accelerate keys are pressed or released.
	// Getting these right is important to make the car controllable, as keyboard input does not allow analogue input.
	// There are different values for when the wheels have full traction and when there are spinning, to implement 
	// traction control schemes.
		
	// How long it takes to fully engage the throttle
	public float throttleTime = 1.0f;
	// How long it takes to fully engage the throttle 
	// when the wheels are spinning (and traction control is disabled)
	public float throttleTimeTraction = 10.0f;
    public float throttleTimeNoneTraction = 5.0f;
	// How long it takes to fully release the throttle
	public float throttleReleaseTime = 0.5f;
	// How long it takes to fully release the throttle 
	// when the wheels are spinning.
	public float throttleReleaseTimeTraction = 0.1f;

	// Turn traction control on or off
	public bool tractionControl = true;
	public GUISkin mySkin;
	
	// These values determine how fast steering value is changed when the steering keys are pressed or released.
	// Getting these right is important to make the car controllable, as keyboard input does not allow analogue input.
	
	// How long it takes to fully turn the steering wheel from center to full lock
	public float steerTime = 1.2f;
	// This is added to steerTime per m/s of velocity, so steering is slower when the car is moving faster.
	public float veloSteerTime = 0.1f;

	// How long it takes to fully turn the steering wheel from full lock to center
	public float steerReleaseTime = 0.6f;
	// This is added to steerReleaseTime per m/s of velocity, so steering is slower when the car is moving faster.
	public float veloSteerReleaseTime = 0f;
	// When detecting a situation where the player tries to counter steer to correct an oversteer situation,
	// steering speed will be multiplied by the difference between optimal and current steering times this 
	// factor, to make the correction easier.
	public float steerCorrectionFactor = 4.0f;

	// Used by SoundController to get average slip velo of all wheels for skid sounds.
	public float slipVelo 
    {
		get 
        {
			float val = 0.0f;
			foreach(Wheel w in wheels)
				val += w.slipVelo / wheels.Length;
			return val;
		}
	}

    CarCameraController carCamCtrl = null;

    public int CurGear
    {
        get { return drivetrain.gear; }
    }

	// Initialize
	void Start () 
	{
		if (centerOfMass != null)
			rigidbody.centerOfMass = centerOfMass.localPosition;
		rigidbody.inertiaTensor *= inertiaFactor;
		drivetrain = GetComponent (typeof (Drivetrain)) as Drivetrain;

        UIManager.GetInst().MainPlayerCarCtrl = this;
	}
	
	void Update () 
	{
		
		// 解决车轮转向
		Vector3 carDir = transform.forward;
		float fVelo = rigidbody.velocity.magnitude;
		Vector3 veloDir = rigidbody.velocity * (1/fVelo);
		float angle = -Mathf.Asin(Mathf.Clamp( Vector3.Cross(veloDir, carDir).y, -1, 1));
		float optimalSteering = angle / (wheels[0].maxSteeringAngle * Mathf.Deg2Rad);
		if (fVelo < 1)
			optimalSteering = 0;	
		
		float steerInput = 0;
        if( InputManager.GetInst().triggerSteeringLeft )
			steerInput = -1;
        if (InputManager.GetInst().triggerSteeringRight)
			steerInput = 1;

        // 车轮左转
		if (steerInput < steering)
		{
			float steerSpeed = (steering>0)?(1/(steerReleaseTime+veloSteerReleaseTime*fVelo)) :(1/(steerTime+veloSteerTime*fVelo));
			if (steering > optimalSteering)
				steerSpeed *= 1 + (steering-optimalSteering) * steerCorrectionFactor;
			steering -= steerSpeed * Time.deltaTime;
			if (steerInput > steering)
				steering = steerInput;
		}
        // 车轮右转
		else if (steerInput > steering)
		{
			float steerSpeed = (steering<0)?(1/(steerReleaseTime+veloSteerReleaseTime*fVelo)) :(1/(steerTime+veloSteerTime*fVelo));
			if (steering < optimalSteering)
				steerSpeed *= 1 + (optimalSteering-steering) * steerCorrectionFactor;
			steering += steerSpeed * Time.deltaTime;
			if (steerInput < steering)
				steering = steerInput;
		}
		
		// Throttle/Brake

        bool accelKey = InputManager.GetInst().triggerThrottle;
        bool brakeKey = InputManager.GetInst().triggerBreak;

		
		if (drivetrain.automatic && drivetrain.gear == 0)
		{
            accelKey = InputManager.GetInst().triggerBreak;
            brakeKey = InputManager.GetInst().triggerThrottle;
		}
		
        //if (Input.GetKey (KeyCode.LeftShift))
        //{
        //    throttle += Time.deltaTime / throttleTime;
        //    throttleInput += Time.deltaTime / throttleTime;
        //}
        //else 
        if (accelKey)
		{
            if (drivetrain.slipRatio < 0.10f)
                throttle += Time.deltaTime / throttleTime;
            else if (!tractionControl)
                throttle += Time.deltaTime / throttleTimeTraction;
            else
                throttle -= Time.deltaTime / throttleReleaseTime;

			if (throttleInput < 0)
				throttleInput = 0;
			throttleInput += Time.deltaTime / throttleTime;
			brake = 0;
		}
		else 
		{
			if (drivetrain.slipRatio < 0.2f)
				throttle -= Time.deltaTime / throttleReleaseTime;
			else
				throttle -= Time.deltaTime / throttleReleaseTimeTraction;
		}
		throttle = Mathf.Clamp01 (throttle);

		if (brakeKey)
		{
			if (drivetrain.slipRatio < 0.2f)
				brake += Time.deltaTime / throttleTime;
			else
				brake += Time.deltaTime / throttleTimeTraction;
			throttle = 0;
			throttleInput -= Time.deltaTime / throttleTime;
		}
		else 
		{
			if (drivetrain.slipRatio < 0.2f)
				brake -= Time.deltaTime / throttleReleaseTime;
			else
				brake -= Time.deltaTime / throttleReleaseTimeTraction;
		}
		brake = Mathf.Clamp01 (brake);
		throttleInput = Mathf.Clamp (throttleInput, -1, 1);
				
		// Handbrake
        handbrake = Mathf.Clamp01(handbrake + (InputManager.GetInst().triggerHandBreak ? Time.deltaTime : -Time.deltaTime));
		
		// Gear shifting
		float shiftThrottleFactor = Mathf.Clamp01((Time.time - lastShiftTime)/shiftSpeed);
		drivetrain.throttle = throttle * shiftThrottleFactor;
		drivetrain.throttleInput = throttleInput;
		
        if( InputManager.GetInst().triggerShiftUp )
		{
			lastShiftTime = Time.time;
			drivetrain.ShiftUp ();
		}
        if (InputManager.GetInst().triggerShiftDown)
		{
			lastShiftTime = Time.time;
			drivetrain.ShiftDown ();
		}

		// Apply inputs
		foreach(Wheel w in wheels)
		{
			w.brake = brake;
			w.handbrake = handbrake;
			w.steering = steering;
		}

        if (InputManager.GetInst().triggerNitro)
            drivetrain.isUsingNitro = true;
        else
            drivetrain.isUsingNitro = false;
	}

	
	// Debug GUI. Disable when not needed.
	void OnGUI ()
    {
        if (UIManager.C_USE_NEW_UI == false)
        {
            Drivetrain gear;
            gear = gameObject.GetComponent("Drivetrain") as Drivetrain;

            GUI.skin = mySkin;
            var style1 = mySkin.customStyles[0];
            var speed = rigidbody.velocity.magnitude * 3.6f;
            GUI.Box(new Rect(0, Screen.height - 120, 140, 55), "", GUI.skin.FindStyle("Box"));
            GUI.Label(new Rect(100, Screen.height - 50, 100, 400), "" + speed.ToString("F0"), style1);
            GUI.Label(new Rect(40, Screen.height - 50, 100, 400), "KM/H : ");
            tractionControl = GUI.Toggle(new Rect(80, Screen.height - 70, 30, 20), tractionControl, "TC");
            gear.automatic = GUI.Toggle(new Rect(120, Screen.height - 70, 30, 20), gear.automatic, "T/A");

            if (carCamCtrl == null)
            {
                GameObject go = GameObject.Find("Car Camera");
                if (go != null)
                    carCamCtrl = go.GetComponent<CarCameraController>();
            }
            if (carCamCtrl != null)
            {
                bool val = GUI.Toggle(new Rect(160, Screen.height - 70, 30, 20), carCamCtrl.useThirdCamera, "Cam");
                if (val != carCamCtrl.useThirdCamera)
                {
                    carCamCtrl.useThirdCamera = val;
                    carCamCtrl.ApplyCameraSetting();
                }
            }
        }
	}
}