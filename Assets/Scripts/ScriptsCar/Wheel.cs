using UnityEngine;
using System.Collections;

// This class simulates a single car's wheel with tire, brake and simple
// suspension (basically just a single, independant spring and damper).
public class Wheel : MonoBehaviour 
{

	// Wheel Specifications
	
	// Wheel radius in meters
    // 车轮半径
	public float radius = 0.34f;
	// Wheel suspension travel in meters
    // 悬挂压缩或者延展的距离(米)
	public float suspensionTravel = 0.2f;
	// Damper strength in kg/s
    // 阻尼力(千克/秒)
	public float damping = 5000;
	// Wheel angular inertia in kg * m^2
    // 车轮角转动惯量(千克 * 米^2)
	public float inertia = 2.2f;
	// Coeefficient of grip - this is simly multiplied to the resulting forces, 
	// so it is not quite realitic, but an easy way to quickly change handling characteritics
    // 轮胎抓地力?
	public float grip = 1.0f;
	// Maximal braking torque (in Nm)
    // 最大的脚刹制动力矩
	public float brakeFrictionTorque = 4000;
    // 最大的手刹制动力矩
	// Maximal handbrake torque (in Nm)
	public float handbrakeFrictionTorque = 0;
	// Base friction torque (in Nm)
    // 基础摩擦力矩
	public float frictionTorque = 10;
	// Maximal steering angle (in degrees)
    // 最大的转向角度
	public float maxSteeringAngle = 28f;
	// Graphical wheel representation (to be rotated accordingly)
	public GameObject model;
	// Fraction of the car's mass carried by this wheel
    // 轮子占整车质量的比重
	public float massFraction = 0.25f;
	// Pacejka coefficients
	public float[] a={1.0f,-60f,1688f,4140f,6.026f,0f,-0.3589f,1f,0f,-6.111f/1000f,-3.244f/100f,0f,0f,0f,0f};
	public float[] b={1.0f,-60f,1588f,0f,229f,0f,0f,0f,-10f,0f,0f};

	// inputs
	// engine torque applied to this wheel
    // 作用到这个车轮上的发动机扭矩
	public float driveTorque = 0;
	// engine braking and other drivetrain friction torques applied to this wheel
    // 作用到这个车轮上的刹车力矩以及传动摩擦力矩
	public float driveFrictionTorque = 0;
	// brake input
    // 脚刹的输入值
	public float brake = 0;
    // 手刹的输入值
	// handbrake input
	public float handbrake = 0;
	// steering input
    // 转向输入值
	public float steering = 0;
	// drivetrain inertia as currently connected to this wheel
    // 连接到这个车轮的传动惯量
	public float drivetrainInertia = 0;
	// suspension force externally applied (by anti-roll bars)
    // 外部施加的悬挂力(譬如防倾杆)
	public float suspensionForceInput = 0;
	
	// output
    // 角速度
	public float angularVelocity;
    // 滑移率
	public float slipRatio;
    // 滑移角
	public float slipVelo;
    // 压力
	public float compression;
	
	// state
	float fullCompressionSpringForce;
	Vector3 wheelVelo;
	Vector3 localVelo;
	Vector3 groundNormal;
	float rotation;
	float normalForce;
	Vector3 suspensionForce;
	Vector3 roadForce;
	Vector3 up, right, forward;
	Quaternion localRotation = Quaternion.identity;
	Quaternion inverseLocalRotation = Quaternion.identity;	
	float slipAngle;
	int lastSkid = -1;
	
	// cached values
	Rigidbody body;
	float maxSlip;
	float maxAngle;
	float oldAngle;	
	Skidmarks skid;
	
    // 计算纵向力
	float CalcLongitudinalForce(float Fz,float slip)
	{
		Fz*=0.001f;//convert to kN
		slip*=100f; //covert to %
		float uP=b[1]*Fz+b[2];
		float D=uP*Fz;	
		float B=((b[3]*Fz+b[4])*Mathf.Exp(-b[5]*Fz))/(b[0]*uP);
		float S=slip+b[9]*Fz+b[10];
		float E=b[6]*Fz*Fz+b[7]*Fz+b[8];
		float Fx=D*Mathf.Sin(b[0]*Mathf.Atan(S*B+E*(Mathf.Atan(S*B)-S*B)));
		return Fx;
	}
	
    // 计算侧向力
	float CalcLateralForce(float Fz,float slipAngle)
	{
		Fz*=0.001f;//convert to kN
		slipAngle*=(360f/(2*Mathf.PI)); //convert angle to deg
		float uP=a[1]*Fz+a[2];
		float D=uP*Fz;
		float B=(a[3]*Mathf.Sin(2*Mathf.Atan(Fz/a[4])))/(a[0]*uP*Fz);
		float S=slipAngle+a[9]*Fz+a[10];
		float E=a[6]*Fz+a[7];
		float Sv=a[12]*Fz+a[13];
		float Fy=D*Mathf.Sin(a[0]*Mathf.Atan(S*B+E*(Mathf.Atan(S*B)-S*B)))+Sv;
		return Fy;
	}
	
    // 计算当前的纵向力
	float CalcLongitudinalForceUnit(float Fz,float slip)
	{
		return CalcLongitudinalForce(Fz,slip*maxSlip);
	}
	
    // 计算当前的侧向力
	float CalcLateralForceUnit(float Fz,float slipAngle)
	{
		return CalcLongitudinalForce(Fz,slipAngle*maxAngle);
	}

    // 计算滑移的合力
	Vector3 CombinedForce(float Fz,float slip,float slipAngle)
	{
		float unitSlip = slip/maxSlip;
		float unitAngle = slipAngle/maxAngle;
		float p = Mathf.Sqrt(unitSlip*unitSlip + unitAngle*unitAngle);
        // 是否有滑移
		if(p > Mathf.Epsilon)
		{
			if (slip < -0.8f)
				return -localVelo.normalized * (Mathf.Abs(unitAngle/p * CalcLateralForceUnit(Fz,p)) + Mathf.Abs(unitSlip/p * CalcLongitudinalForceUnit(Fz,p)));
			else
			{
				Vector3 forward = new Vector3( 0, -groundNormal.z, groundNormal.y);
				return Vector3.right * unitAngle/p * CalcLateralForceUnit(Fz,p) + forward * unitSlip/p * CalcLongitudinalForceUnit(Fz,p);
			}
		}
		else
			return Vector3.zero;
	}

	void InitSlipMaxima()
	{
		const float stepSize = 0.001f;
		const float testNormalForce = 4000f;
		float force = 0;
        // 步进计算最大的滑移值
		for (float slip = stepSize;;slip += stepSize)
		{
            // 计算纵向力
			float newForce = CalcLongitudinalForce(testNormalForce,slip);
            if (force < newForce)
            {
                Debug.Log("[slip : " + slip + "] [force : " + newForce + "]");
                force = newForce;
            }
            else
            {
                maxSlip = slip - stepSize;
                break;
            }
		}
		force = 0;
        // 步进计算最大的滑移角
		for (float slipAngle = stepSize;;slipAngle += stepSize)
		{
            // 计算侧向力
			float newForce = CalcLateralForce(testNormalForce,slipAngle);
            if (force < newForce)
            {
                Debug.Log("[slipAngle : " + slipAngle + "] [force : " + newForce + "]");
                force = newForce;
            }
            else
            {
                maxAngle = slipAngle - stepSize;
                break;
            }
		}
	}
	
	void Start () 
    {
		Transform trs = transform;
		while (trs != null && trs.rigidbody == null)
			trs = trs.parent;
		if (trs != null)
			body = trs.rigidbody;

		InitSlipMaxima ();
		skid = FindObjectOfType(typeof(Skidmarks)) as Skidmarks;
        // 悬挂弹簧最大的压缩力
		fullCompressionSpringForce = body.mass * massFraction * 2.0f * -Physics.gravity.y;
	}
	
    // 计算悬挂力
	Vector3 SuspensionForce () 
    {
        // 计算当前的弹簧压力
		float springForce = compression * fullCompressionSpringForce;
		normalForce = springForce;
		// 计算阻尼力
		float damperForce = Vector3.Dot(localVelo, groundNormal) * damping;

		return (springForce - damperForce + suspensionForceInput) * up;
	}

    // 计算滑移率
	float SlipRatio ()
	{
		const float fullSlipVelo = 4.0f;

        // 计算车轮相对于路面的速率
		float wheelRoadVelo = Vector3.Dot (wheelVelo, forward);
		if (wheelRoadVelo == 0)
			return 0;
		
		float absRoadVelo = Mathf.Abs (wheelRoadVelo);
        // 计算滑移衰减
		float damping = Mathf.Clamp01( absRoadVelo / fullSlipVelo );
		
        // 计算轮胎的速率
		float wheelTireVelo = angularVelocity * radius;
        // 滑移率
		return (wheelTireVelo - wheelRoadVelo) / absRoadVelo * damping;
	}

    // 计算滑移角
	float SlipAngle ()
	{
		const float fullAngleVelo = 2.0f;
		
		Vector3 wheelMotionDirection = localVelo;
		wheelMotionDirection.y = 0;

		if (wheelMotionDirection.sqrMagnitude < Mathf.Epsilon)
			return 0;
				
		float sinSlipAngle = wheelMotionDirection.normalized.x;
		Mathf.Clamp(sinSlipAngle, -1, 1); // To avoid precision errors.

		float damping = Mathf.Clamp01( localVelo.magnitude / fullAngleVelo );
		
		return -Mathf.Asin(sinSlipAngle) * damping * damping;
	}
	
	Vector3 RoadForce () 
    {
        //           slipRes
        //            |
        //            @ <----- 10
        //          @ | @
        //        @   |   @
        // @@@@@@     |     @@@@@@@
        //            |
        //------#-----------#--------------- angularVelocity
        //   -100     |     100

		int slipRes=(int)((100.0f-Mathf.Abs(angularVelocity))/(10.0f));
		if (slipRes < 1)
			slipRes = 1;
		float invSlipRes = (1.0f/(float)slipRes);
		
        // 计算总惯性
		float totalInertia = inertia + drivetrainInertia;
		float driveAngularDelta = driveTorque * Time.deltaTime * invSlipRes / totalInertia;
		float totalFrictionTorque = brakeFrictionTorque * brake + handbrakeFrictionTorque * handbrake + frictionTorque + driveFrictionTorque;
		float frictionAngularDelta = totalFrictionTorque * Time.deltaTime * invSlipRes / totalInertia;

		Vector3 totalForce = Vector3.zero;
		float newAngle = maxSteeringAngle * steering;
		for (int i=0; i<slipRes; i++)
		{
			float f = i * 1.0f/(float)slipRes;
			localRotation = Quaternion.Euler (0, oldAngle + (newAngle - oldAngle) * f, 0); 		
			inverseLocalRotation = Quaternion.Inverse(localRotation);
			forward = transform.TransformDirection (localRotation * Vector3.forward);
			right = transform.TransformDirection (localRotation * Vector3.right);
			
            // 计算当前步进转向角下的滑移率
			slipRatio = SlipRatio ();
            // 计算当前步进转向角下的滑移角
			slipAngle = SlipAngle ();
			Vector3 force = invSlipRes * grip * CombinedForce (normalForce, slipRatio, slipAngle);
			Vector3 worldForce = transform.TransformDirection (localRotation * force);
			angularVelocity -= (force.z * radius * Time.deltaTime) / totalInertia;
			angularVelocity += driveAngularDelta;
			if (Mathf.Abs(angularVelocity) > frictionAngularDelta)
				angularVelocity -= frictionAngularDelta * Mathf.Sign(angularVelocity);
			else
				angularVelocity = 0;
				
			wheelVelo += worldForce* (1/body.mass) * Time.deltaTime * invSlipRes;
			totalForce += worldForce;
		}

        // 计算纵向滑移速率
		float longitunalSlipVelo = Mathf.Abs(angularVelocity * radius - Vector3.Dot (wheelVelo, forward));	
        // 计算侧向滑移速率
		float lateralSlipVelo = Vector3.Dot (wheelVelo, right);
        // 合成滑移速率
		slipVelo = Mathf.Sqrt(longitunalSlipVelo * longitunalSlipVelo + lateralSlipVelo * lateralSlipVelo);
		
        // 记录轮胎的转向角
		oldAngle = newAngle;
		return totalForce;
	}
	
	void FixedUpdate () 
    {

		Vector3 pos = transform.position;
		up = transform.up;
		RaycastHit hit;
        // 检查车辆是否触地
		bool onGround = Physics.Raycast( pos, -up, out hit, suspensionTravel + radius);
		
        // 如果车子在一个触发器之上或者里面
		if (onGround && hit.collider.isTrigger)
		{
			onGround = false;
            float dist = suspensionTravel + radius;
			RaycastHit[] hits = Physics.RaycastAll( pos, -up, suspensionTravel + radius);
			foreach(RaycastHit test in hits)
			{
				if (!test.collider.isTrigger && test.distance <= dist)
				{
					hit = test;
					onGround = true;
					dist = test.distance;
				}
			}
		}

		if (onGround)
		{
			groundNormal = transform.InverseTransformDirection (inverseLocalRotation * hit.normal);
			compression = 1.0f - ((hit.distance - radius) / suspensionTravel);
			wheelVelo = body.GetPointVelocity (pos);
			localVelo = transform.InverseTransformDirection (inverseLocalRotation * wheelVelo);
			suspensionForce = SuspensionForce ();
			roadForce = RoadForce ();
			body.AddForceAtPosition (suspensionForce + roadForce, pos);
		}
		else
		{
			compression = 0.0f;
			suspensionForce = Vector3.zero;
			roadForce = Vector3.zero;
			float totalInertia = inertia + drivetrainInertia;
			float driveAngularDelta = driveTorque * Time.deltaTime / totalInertia;
			float totalFrictionTorque = brakeFrictionTorque * brake + handbrakeFrictionTorque * handbrake + frictionTorque + driveFrictionTorque;
			float frictionAngularDelta = totalFrictionTorque * Time.deltaTime / totalInertia;
			angularVelocity += driveAngularDelta;
			if (Mathf.Abs(angularVelocity) > frictionAngularDelta)
				angularVelocity -= frictionAngularDelta * Mathf.Sign(angularVelocity);
			else
				angularVelocity = 0;
			slipRatio = 0;
			slipVelo = 0;
		}
		
		if (skid != null && Mathf.Abs(slipRatio) > 0.2)
			lastSkid = skid.AddSkidMark(hit.point, hit.normal, Mathf.Abs(slipRatio) - 0.2f,lastSkid);
		else
			lastSkid = -1;
			
		compression = Mathf.Clamp01 (compression);
		rotation += angularVelocity * Time.deltaTime;
		if (model != null)
		{
			model.transform.localPosition = Vector3.up * (compression - 1.0f) * suspensionTravel;
			model.transform.localRotation = Quaternion.Euler (Mathf.Rad2Deg * rotation, maxSteeringAngle * steering, 0);
		}
	}
}
