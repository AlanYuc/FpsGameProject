using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    //可以不写，transform会默认为该脚本挂载的物体的transform组件。
    //public Transform transform;

    public float moveSpeed;//移动速度
    public float mouseSensitivity;//鼠标灵敏度（暂时这么理解）
    public float cameraMinVerticalAngle;//最小角度
    public float cameraMaxVerticalAngle;//最大角度
    private float xRotation; // 存储垂直旋转角度（X轴）
    private float yRotation; // 存储水平旋转角度（Y轴）
    public Animator animator;
    public Rigidbody playerRigidbody;
    private float jumpforce;
    public Transform muzzleTransform;//枪口位置

    void Start()
    {
        transform.position = new Vector3(-0.2f, transform.position.y, transform.position.z);
        moveSpeed = 1.5f;
        mouseSensitivity = 800.0f;
        cameraMinVerticalAngle = -60f;
        cameraMaxVerticalAngle = 60f;
        xRotation = 0f;
        yRotation = 0f;
        jumpforce = 300;

        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false; // 隐藏鼠标
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        PlayerView();
        Attack();
        Jump();
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
        //Quaternion.AngleAxis适合绕单一轴的旋转
        //此处是第一人称视角相机的旋转，因此Quaternion.Euler更合适

        /*
        float offsetX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;//获取鼠标水平偏移量
        float offsetY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;//获取鼠标垂直偏移量
        Quaternion rotationX = Quaternion.AngleAxis(offsetX, Vector3.up);//Vector3.up就是绕Y轴的水平旋转
        transform.rotation *= rotationX;
        */

        float offsetX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;//获取鼠标水平偏移量
        float offsetY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;//获取鼠标垂直偏移量

        //计算水平旋转(Y轴)
        yRotation += offsetX;

        //计算垂直旋转(X轴)
        xRotation -= offsetY;
        xRotation = Mathf.Clamp(xRotation, cameraMinVerticalAngle, cameraMaxVerticalAngle);

        //使用四元数计算最终旋转
        Quaternion verticalRotation = Quaternion.Euler(xRotation, 0f, 0f);//上下旋转(X轴)
        Quaternion horizontalRotation = Quaternion.Euler(0f, yRotation, 0f);//左右旋转(Y轴)

        //应用旋转(先水平后垂直，避免万向节死锁)
        transform.rotation = horizontalRotation * verticalRotation;
    }

    /// <summary>
    /// 攻击
    /// </summary>
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");

            RaycastHit hit;
            if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, 20))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log(hit.collider.name);
                }
            }
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRigidbody.AddForce(jumpforce * Vector3.up);
        }
    }
}
