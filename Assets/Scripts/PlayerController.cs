using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    //可以不写，transform会默认为该脚本挂载的物体的transform组件。
    //public Transform transform;

    public float moveSpeed;
    public float mouseSensitivity;

    void Start()
    {
        transform.position = new Vector3(-0.2f, transform.position.y, transform.position.z);
        moveSpeed = 1.5f;
        mouseSensitivity = 800.0f;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        PlayerView();
    }

    /// <summary>
    /// 玩家移动
    /// </summary>
    private void PlayerMove()
    {
        float offsetV = Input.GetAxis("Vertical");
        float offsetH = Input.GetAxis("Horizontal");
        Vector3 moveV = transform.forward * offsetV * moveSpeed * Time.deltaTime;
        Vector3 moveH = transform.right * offsetH * moveSpeed * Time.deltaTime;
        transform.position += moveV;
        transform.position += moveH;
    }

    /// <summary>
    /// 玩家视角旋转
    /// </summary>
    private void PlayerView()
    {
        float offsetX = Input.GetAxis("Mouse X");//获取鼠标水平偏移量
        float offsetY = Input.GetAxis("Mouse Y");//获取鼠标垂直偏移量
        //四元数旋转
        Quaternion rotationX = Quaternion.AngleAxis(offsetX, Vector3.up);//Vector3.up就是绕Y轴的水平旋转
        transform.rotation *= rotationX;
    }
}
