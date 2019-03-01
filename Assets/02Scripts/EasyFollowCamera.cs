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

public class EasyFollowCamera : MonoBehaviour
{
    /// <summary>
    /// 目标物
    /// </summary>
    [SerializeField]
    private Transform targetTrans;
    [SerializeField]
    private float horizontalDis = 8;
    [SerializeField]
    private float verticalDis = 6;
    [SerializeField]
    private float smooth = 10;
    [SerializeField]
    private float springConstant = 10;//弹性常量

    private Vector3 targetPos = Vector3.zero;
    private Vector3 idealPos = Vector3.zero;
    private Quaternion targetRotation = Quaternion.identity;

    private float dampConstant = 0;//阻尼
    private Vector3 velocity = Vector3.zero;//速度

    private Vector3 offset = Vector3.zero;
    private Vector3 up = Vector3.up;

    private float yaw = 0;//偏航
    private float pitch = 0;//俯仰

    private Transform selfTrans;
	// Use this for initialization
	void Start ()
    {
        selfTrans = GetComponent<Transform>();

        InitCamera();
	}

    void UpdateRotation()
    {
        //创建一个世界向上的四元数
        Quaternion quatYaw = Quaternion.AngleAxis(yaw, Vector3.up);

        offset = quatYaw * offset;
        up = quatYaw * up;

        Vector3 forward = -offset;
        forward.Normalize();
        Vector3 left = Vector3.Cross(up, forward);
        left.Normalize();

        //创建关于摄像机左边旋转的四元数值
        Quaternion quatPitch = Quaternion.AngleAxis(pitch, left);

        offset = quatPitch * offset;
        up = quatPitch * up;

        targetPos = targetTrans.position + offset;
        targetRotation = Quaternion.LookRotation(targetTrans.position - targetPos, up);

        //插值
        transform.position = Vector3.Lerp(transform.position, targetPos, smooth * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smooth * Time.deltaTime);
    }
	
    void InitCamera()
    {
        dampConstant = Mathf.Sqrt(springConstant) * 2;

        offset = targetTrans.up * verticalDis - targetTrans.forward * horizontalDis;

        //相机位置
        targetPos = targetTrans.position + offset;

        //相机角度
        targetRotation = Quaternion.LookRotation(targetTrans.position - targetPos, targetTrans.up);

        transform.position = targetPos;
        transform.rotation = targetRotation;
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        //yaw = Input.GetAxis("Horizontal");
        //pitch = Input.GetAxis("Vertical");
        //UpdateRotation();

        //理想位置
        idealPos = targetTrans.position + targetTrans.up * verticalDis - targetTrans.forward * horizontalDis;

        //理想位置到真实位置的向量
        Vector3 displacement = targetPos - idealPos;
        //计算加速度
        Vector3 springAccel = -springConstant * displacement - dampConstant * velocity;

        velocity += springAccel * Time.deltaTime;

        targetPos += velocity * Time.deltaTime;

        //targetPos = targetTrans.position + targetTrans.up * verticalDis - targetTrans.forward * horizontalDis;

        //相机角度
        targetRotation = Quaternion.LookRotation(targetTrans.position - targetPos, targetTrans.up);

        //插值
        transform.position = Vector3.Lerp(transform.position, targetPos, smooth * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smooth * Time.deltaTime);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smooth * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smooth * Time.deltaTime);
    }
}
