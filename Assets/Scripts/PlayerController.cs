using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GUNTYPE
{
        SINGLESHOTRIGLE,
        AUTORIFLE,
        SNIPERRIFLE
}

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
    public GameObject bloodEffect;//血液特效
    public GameObject wallEffect;//击中路、墙的特效
    public GameObject grassEffect;//击中草、叶子的特效
    public GameObject treeEffect;//击中树的特效
    public GameObject riverEffect;//击中水的特效
    public float attackCD;//单点开枪的CD
    public float attackTimer;//开枪的时间间隔计时器
    public GameObject fireEffect;//开枪的焰火特效
    public GUNTYPE gunType;

    void Start()
    {
        transform.position = new Vector3(-0.2f, transform.position.y, transform.position.z);
        moveSpeed = 2.0f;
        mouseSensitivity = 800.0f;
        cameraMinVerticalAngle = -60f;
        cameraMaxVerticalAngle = 60f;
        xRotation = 0f;
        yRotation = 0f;
        jumpforce = 300;
        attackCD = 0.8f;
        attackTimer = 0;
        gunType = GUNTYPE.SINGLESHOTRIGLE;

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
        ChangeGunType();
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
        switch (gunType)
        {
            case GUNTYPE.SINGLESHOTRIGLE:
                SingleShotAttack();
                break;
            case GUNTYPE.AUTORIFLE:
                AutoShotAttack();
                break;
            case GUNTYPE.SNIPERRIFLE:
                break;
            default:
                break;
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRigidbody.AddForce(jumpforce * Vector3.up);
        }
    }

    private void ChangeGunType()
    {
        //按键切换
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gunType = GUNTYPE.SINGLESHOTRIGLE;
            attackCD = 0.8f;
            Debug.Log("切换为单点");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gunType = GUNTYPE.AUTORIFLE;
            attackCD = 0.1f;
            Debug.Log("切换为自动");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gunType = GUNTYPE.SNIPERRIFLE;
            Debug.Log("切换为狙击");
        }

        //滚轮切换
        //to do
    }

    private void SingleShotAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            GunAttack();
        }
    }

    private void AutoShotAttack()
    {
        if(Input.GetMouseButton(0) && Time.time - attackTimer >= attackCD)
        {
            GunAttack();
        }
    }

    private void GunAttack()
    {
        attackTimer = Time.time;

        animator.SetTrigger("Attack");

        GameObject fire = Instantiate(fireEffect, muzzleTransform);
        fire.transform.localPosition = Vector3.zero;
        fire.transform.localEulerAngles = Vector3.zero;//注意都是local

        RaycastHit hit;
        if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, 20))
        {
            //if (hit.collider.CompareTag("Enemy"))   
            //{
            //    Debug.Log(hit.collider.name);
            //    Instantiate(bloodEffect,hit.point, Quaternion.identity);
            //}

            switch (hit.collider.tag)
            {
                case "Enemy": Instantiate(bloodEffect, hit.point, Quaternion.identity); break;
                case "Wall": Instantiate(wallEffect, hit.point, Quaternion.identity); break;
                case "Grass": Instantiate(grassEffect, hit.point, Quaternion.identity); break;
                case "Tree": Instantiate(treeEffect, hit.point, Quaternion.identity); break;
                case "River": Instantiate(riverEffect, hit.point + Vector3.up * 0.2f, Quaternion.identity); break;
            }
        }
    }
}
