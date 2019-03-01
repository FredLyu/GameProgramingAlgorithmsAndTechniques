/******************************************
 *	Title:
 *	Description:
 *	Date:
 *	Version:
 *	Modify Recoder:
 *****************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstFollow : MonoBehaviour 
{
    public Transform playerTrans;
    public Vector3 verticalOffst;
    public Vector3 targetOffset;

    private float totalYaw = 0;
    private float totalPitch = 0;

    private Vector3 eye = Vector3.zero;
    private Vector3 target = Vector3.zero;

    private Quaternion targetQuat = Quaternion.identity;

    private Transform cameraTrans;

	void Start () 
	{
        cameraTrans = GetComponent<Transform>();

        eye = playerTrans.position + verticalOffst;
        target = eye + targetOffset;

        targetQuat = Quaternion.LookRotation(playerTrans.position-target+verticalOffst, playerTrans.up);

        cameraTrans.position = target;
        cameraTrans.rotation = targetQuat;

        Debug.Log("Name = " + cameraTrans.name + ", Rotation = " + targetQuat);
	}
	
	void LateUpdate () 
	{
        totalYaw += Input.GetAxis("Horizontal");
        totalPitch += Input.GetAxis("Vertical");
        //Debug.Log("Yaw = " + totalYaw + ", Pitch = " + totalPitch);
        totalPitch = Mathf.Clamp(totalPitch, -45f, 45f);

        Vector3 actualOffset = targetOffset;

        Quaternion quatYaw = Quaternion.AngleAxis(totalYaw, Vector3.up);
        actualOffset = quatYaw * actualOffset;

        Vector3 forward = actualOffset;
        forward.Normalize();
        Vector3 left = Vector3.Cross(Vector3.up, forward);
        left.Normalize();

        Quaternion quatPitch = Quaternion.AngleAxis(totalPitch, left);
        actualOffset = quatPitch * actualOffset;

        eye = playerTrans.position + verticalOffst;
        target = eye + actualOffset;

        targetQuat = Quaternion.LookRotation(playerTrans.position-target+verticalOffst, playerTrans.up);

        cameraTrans.position = target;
        cameraTrans.rotation = targetQuat;
	}
}
