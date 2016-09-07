using UnityEngine;
using System.Collections;

// This class simulates an anti-roll bar.
// Anti roll bars transfer suspension compressions forces from one wheel to another.
// This is used to minimize body roll in corners, and improve grip by balancing the wheel loads.
// Typical modern cars have one anti-roll bar per axle.
// 防侧倾杆
// 防侧倾杆把悬架压缩力从一个轮子传递到另一个轮子
// 这是用来减少车体在转弯时的倾侧度，并通过平衡车轮负载来提高抓地力
// 典型的现代汽车每个轮轴都有一个防滚条
public class AntiRollBar : MonoBehaviour 
{

	// The two wheels connected by the anti-roll bar. These should be on the same axle.
	public Wheel wheel1;
	public Wheel wheel2;
	
	// Coeefficient determining how much force is transfered by the bar.
	public float coefficient = 10000;
	
	void FixedUpdate () 
	{
		float force = (wheel1.compression - wheel2.compression) * coefficient;
		wheel1.suspensionForceInput =+ force;
		wheel2.suspensionForceInput =- force;
	}
}
