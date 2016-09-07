using UnityEngine;
using System.Collections;



// This class simulates a car's engine and drivetrain, generating
// torque, and applying the torque to the wheels.
// 这个类主要是模拟车辆发动机以及传动装置,生成扭矩并作用到车轮上
public class Drivetrain : MonoBehaviour {
	
	// All the wheels the drivetrain should power
	public Wheel[] poweredWheels;
	
	// The gear ratios, including neutral (0) and reverse (negative) gears
    // 齿轮比，包括中性（0）和反向（负）齿轮
	public float[] gearRatios;
	
	// The final drive ratio, which is multiplied to each gear ratio
    // 最后的传动比，这是乘以每个齿轮比
	public float finalDriveRatio = 3.23f;
	
	// The engine's torque curve characteristics. Since actual curves are often hard to come by,
	// we approximate the torque curve from these values instead.

	// powerband RPM range
	public float minRPM = 800;
	public float maxRPM = 6400;

	// engine's maximal torque (in Nm) and RPM.
    // 发动机扭矩最大值
	public float maxTorque = 664;
    // 发动机达到扭矩最大值时的转速
	public float maxTorqueRPM = 4000;

	// engine's maximal power (in Watts) and RPM.
    // 发动机的功率最大值
	public float maxPower = 317000;
    // 发动机功率达到最大值时的转速
	public float maxPowerRPM = 5000;

	// engine inertia (how fast the engine spins up), in kg * m^2
    // 发动机转动惯量
	public float engineInertia = 0.3f;
	
	// engine's friction coefficients - these cause the engine to slow down, and cause engine braking.

	// constant friction coefficient
    // 发动机常摩擦系数
	public float engineBaseFriction = 25f;
	// linear friction coefficient (higher friction when engine spins higher)
    // 发动机线性摩擦系数
	public float engineRPMFriction = 0.02f;

	// Engine orientation (typically either Vector3.forward or Vector3.right). 
	// This determines how the car body moves as the engine revs up.	
	public Vector3 engineOrientation = Vector3.forward;
	
	// Coefficient determining how much torque is transfered between the wheels when they move at 
	// different speeds, to simulate differential locking.
    // 差速锁系数
	public float differentialLockCoefficient = 0;
	
	// inputs
	// engine throttle
    // 发动机油门
	public float throttle = 0;
	// engine throttle without traction control (used for automatic gear shifting)
    // 无牵引力控制的发动机油门（用于自动换档）
	public float throttleInput = 0;
	
	// shift gears automatically?
	public bool automatic = true;

	// state
    // 当前的变速档
	public int gear = 5;
    // 当前的发动机转速
	public float rpm;
    // 滑移率
	public float slipRatio = 0.0f;
    // 发动机转动角速度
	float engineAngularVelo;
	
	
	float Sqr (float x) { return x*x; }
	
	// Calculate engine torque for current rpm and throttle values.
    // 根据当前的转速以及油门值计算引擎的扭矩
	float CalcEngineTorque () 
	{ 
        // | torque
        // |
        // |         T1
        // |        @     T2
        // |      @          @
        // |    @              @
        // |@                   T3
        // ----------p1---p2----p3----------> rpm        
        // p1: maxTorqueRPM
        // p2: maxPowerRPM
        // p3: maxRPM
        // T1: maxTorque
        // T2: maxPowerTorque
        // T3: maxRPMTorque

		float result;
		if(rpm < maxTorqueRPM)
			result = maxTorque*(-Sqr(rpm / maxTorqueRPM - 1) + 1);
		else 
        {
			float maxPowerTorque = maxPower/(maxPowerRPM*2*Mathf.PI/60);
			float aproxFactor = (maxTorque-maxPowerTorque)/(2*maxTorqueRPM*maxPowerRPM-Sqr(maxPowerRPM)-Sqr(maxTorqueRPM));
			float torque = aproxFactor * Sqr(rpm-maxTorqueRPM)+maxTorque;
			result=torque>0?torque:0;
		} 
		if(rpm > maxRPM)
		{
			result *= 1-((rpm-maxRPM) * 0.006f);
			if(result<0)
				result=0;
		}
		if(rpm<0)
			result=0;
		return result;
	}
	
	
	void FixedUpdate () 
	{
		float ratio = gearRatios[gear] * finalDriveRatio;
		float inertia = engineInertia * Sqr(ratio);
		float engineFrictionTorque = engineBaseFriction + rpm * engineRPMFriction;
		float engineTorque = (CalcEngineTorque() + Mathf.Abs(engineFrictionTorque)) * throttle;
		slipRatio = 0.0f;		
		
		if (ratio == 0)
		{
			// Neutral gear - just rev up engine
            // 中性齿轮,刚刚发动发动机
			float engineAngularAcceleration = (engineTorque-engineFrictionTorque) / engineInertia;
			engineAngularVelo += engineAngularAcceleration * Time.deltaTime;
			
			// Apply torque to car body
			rigidbody.AddTorque(-engineOrientation * engineTorque);
		}
		else
		{
			float drivetrainFraction = 1.0f/poweredWheels.Length;
			float averageAngularVelo = 0;	
			foreach(Wheel w in poweredWheels)
				averageAngularVelo += w.angularVelocity * drivetrainFraction;

			// Apply torque to wheels
			foreach(Wheel w in poweredWheels)
			{
				float lockingTorque = (averageAngularVelo - w.angularVelocity) * differentialLockCoefficient;
				w.drivetrainInertia = inertia * drivetrainFraction;
				w.driveFrictionTorque = engineFrictionTorque * Mathf.Abs(ratio) * drivetrainFraction;
				w.driveTorque = engineTorque * ratio * drivetrainFraction + lockingTorque;

				slipRatio += w.slipRatio * drivetrainFraction;
			}
			
			// update engine angular velo
			engineAngularVelo = averageAngularVelo * ratio;
		}
		
		// update state
		slipRatio *= Mathf.Sign ( ratio );
		rpm = engineAngularVelo * (60.0f/(2*Mathf.PI));
		
		// very simple simulation of clutch - just pretend we are at a higher rpm.
		float minClutchRPM = minRPM;
		if (gear == 2)
			minClutchRPM += throttle * 3000;
		if (rpm < minClutchRPM)
			rpm = minClutchRPM;
			
		// Automatic gear shifting. Bases shift points on throttle input and rpm.
		if (automatic)
		{
			if (rpm >= maxRPM * (0.5f + 0.5f * throttleInput))
				ShiftUp ();
			else if (rpm <= maxRPM * (0.25f + 0.4f * throttleInput) && gear > 2)
				ShiftDown ();
			if (throttleInput < 0 && rpm <= minRPM)
				gear = (gear == 0?2:0);
		}
	}
		
	public void ShiftUp () {
		if (gear < gearRatios.Length - 1)
			gear ++;
	}

	public void ShiftDown () {
		if (gear > 0)
			gear --;
	}
	
	// Debug GUI. Disable when not needed.
	//void OnGUI () {
		

	//Spenti momentaneamente perchè se attivati si vedono su tutti i client
		
		//GUI.Label(new Rect(300,400,200,200),"RPM: "+rpm);
		//GUI.Label (new Rect(300,400,200,200),"Gear: "+(gear-1));

		

		
	//}
}
