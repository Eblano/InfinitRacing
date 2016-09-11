﻿using UnityEngine;
using System.Collections;

public class CarCameraController : MonoBehaviour 
{
    Camera seatCamera = null;
    Camera thirdCamera = null;
    public bool useThirdCamera = true;

    public Transform target = null;
    public float distance = 4.0f;
    public float height = 1.0f;
    public float smoothLag = 0.2f;
    public float maxSpeed = 10.0f;
    public float snapLag = 0.3f;
    public float clampHeadPositionScreenSpace = 0.75f;
    public LayerMask lineOfSightMask = 0;
    public Vector3 headOffset = Vector3.zero;
    public Vector3 centerOffset = Vector3.zero;

    private bool isSnapping = false;
    private Vector3 velocity = Vector3.zero;
    private float targetHeight = 100000.0f;

    
    public void ToggleCamera()
    {
        useThirdCamera = !useThirdCamera;
        ApplyCameraSetting();
    }

    public void ApplyCameraSetting()
    {
        if (seatCamera == null || thirdCamera == null)
            return;
        seatCamera.depth = useThirdCamera ? -1 : 0;
        thirdCamera.depth = useThirdCamera ? 0 : -1;
    }

    void Apply (Transform dummyTarget, Vector3 dummyCenter )
    {	
	    var targetCenter = target.position + centerOffset;
	    var targetHead = target.position + headOffset;
        /*	
	        // When jumping don't move camera upwards but only down!
	        if (controller.IsJumping ())
	        {
		        // We'd be moving the camera upwards, do that only if it's really high
		        var newTargetHeight = targetCenter.y + height;
		        if (newTargetHeight < targetHeight || newTargetHeight - targetHeight > 5)
			        targetHeight = targetCenter.y + height;
	        }
	        // When walking always update the target height
	        else
	        {
		        targetHeight = targetCenter.y + height;
	        }
        */
	    targetHeight = targetCenter.y + height;

	
	    // We start snapping when user pressed Fire2!
	    if (Input.GetButton("Fire2") && !isSnapping)
	    {
		    velocity = Vector3.zero;
		    isSnapping = true;
	    }

    //	if (isSnapping)
	    {
		    ApplySnapping (targetCenter);
	    }
    //	else
    //	{
    //		ApplyPositionDamping (Vector3(targetCenter.x, targetHeight, targetCenter.z));
    //	}
	
	    SetUpRotation(targetCenter, targetHead);
    }

    void LateUpdate ()
    {
	    if (target)
		    Apply (null, Vector3.zero);	
		
    }

    void ApplySnapping (Vector3 targetCenter )
    {
        Vector3 position = transform.position;
        Vector3 offset = position - targetCenter;
	    offset.y = 0;
	    float currentDistance = offset.magnitude;

        float targetAngle = target.eulerAngles.y;
        float currentAngle = transform.eulerAngles.y;

        currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref velocity.x, snapLag);
	    currentDistance = Mathf.SmoothDamp(currentDistance, distance, ref velocity.z, snapLag);

        float gap = 1.0f;
        if( currentDistance > distance + gap )
            currentDistance = distance + gap;
        else if( currentDistance < distance - gap )
            currentDistance = distance - gap;

	    Vector3 newPosition = targetCenter;
	    newPosition += Quaternion.Euler(0, currentAngle, 0) * Vector3.back * currentDistance;

        newPosition.y = Mathf.SmoothDamp(position.y, targetCenter.y + height, ref velocity.y, smoothLag, maxSpeed);

	    newPosition = AdjustLineOfSight(newPosition, targetCenter);
	
	    transform.position = newPosition;
	
    //	// We are close to the target, so we can stop snapping now!
    //	if (AngleDistance (currentAngle, targetAngle) < 3.0)
    //	{
    //		isSnapping = false;
    //		velocity = Vector3.zero;
    //	}
    }

    Vector3 AdjustLineOfSight (Vector3 newPosition, Vector3 target )
    {
	    RaycastHit hit;
	    if (Physics.Linecast (target, newPosition, out hit, lineOfSightMask.value))
	    {
		    velocity = Vector3.zero;
		    return hit.point;
	    }
	    return newPosition;
    }

    void ApplyPositionDamping (Vector3 targetCenter)
    {
	    // We try to maintain a constant distance on the x-z plane with a spring.
	    // Y position is handled with a seperate spring
        Vector3 position = transform.position;
        Vector3 offset = position - targetCenter;
	    offset.y = 0;
        Vector3 newTargetPos = offset.normalized * distance + targetCenter;
	
	    Vector3 newPosition;
	    newPosition.x = Mathf.SmoothDamp (position.x, newTargetPos.x, ref velocity.x, smoothLag, maxSpeed);
	    newPosition.z = Mathf.SmoothDamp (position.z, newTargetPos.z, ref velocity.z, smoothLag, maxSpeed);
	    newPosition.y = Mathf.SmoothDamp (position.y, targetCenter.y, ref velocity.y, smoothLag, maxSpeed);
	
	    newPosition = AdjustLineOfSight(newPosition, targetCenter);
	
	    transform.position = newPosition;
    }

    void SetUpRotation (Vector3 centerPos, Vector3 headPos)
    {
	    // Now it's getting hairy. The devil is in the details here, the big issue is jumping of course.
	    // * When jumping up and down don't center the guy in screen space. This is important to give a feel for how high you jump.
	    //   When keeping him centered, it is hard to see the jump.
	    // * At the same time we dont want him to ever go out of screen and we want all rotations to be totally smooth
	    //
	    // So here is what we will do:
	    //
	    // 1. We first find the rotation around the y axis. Thus he is always centered on the y-axis
	    // 2. When grounded we make him be cented
	    // 3. When jumping we keep the camera rotation but rotate the camera to get him back into view if his head is above some threshold
	    // 4. When landing we must smoothly interpolate towards centering him on screen
	    var cameraPos = transform.position;
	    var offsetToCenter = centerPos - cameraPos;
	
	    // Generate base rotation only around y-axis
	    var yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));

	    var relativeOffset = Vector3.forward * distance + Vector3.down * height;
	    transform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);

	    // Calculate the projected center position and top position in world space
        var centerRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
        var topRay = camera.ViewportPointToRay(new Vector3(0.5f, clampHeadPositionScreenSpace, 1.0f));

	    var centerRayPos = centerRay.GetPoint(distance);
	    var topRayPos = topRay.GetPoint(distance);
	
	    var centerToTopAngle = Vector3.Angle(centerRay.direction, topRay.direction);
	
	    var heightToAngle = centerToTopAngle / (centerRayPos.y - topRayPos.y);

	    var extraLookAngle = heightToAngle * (centerRayPos.y - centerPos.y);
	    if (extraLookAngle < centerToTopAngle)
	    {
		    extraLookAngle = 0;
	    }
	    else
	    {
		    extraLookAngle = extraLookAngle - centerToTopAngle;
		    transform.rotation *= Quaternion.Euler(-extraLookAngle, 0, 0);
	    }
    }

    float AngleDistance (float a, float b)
    {
	    a = Mathf.Repeat(a, 360);
	    b = Mathf.Repeat(b, 360);
	
	    return Mathf.Abs(b - a);
    }

    Vector3 GetCenterOffset ()
    {
	    return centerOffset;
    }

    void SetTarget(Transform t)
    {
        target = t;
    }


	// Use this for initialization
	void Start () 
    {
        if (target != null)
        {
            Camera[] cams = target.gameObject.GetComponentsInChildren<Camera>();
            if (cams.Length == 1)
                seatCamera = cams[0];
        }
        thirdCamera = this.gameObject.camera;
        ApplyCameraSetting();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (seatCamera == null && target != null)
        {
            Camera[] cams = target.gameObject.GetComponentsInChildren<Camera>();
            if (cams.Length == 1)
                seatCamera = cams[0];
        }
	}
}
