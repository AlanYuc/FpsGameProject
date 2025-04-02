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
    public GameObject fireEffect2;
    public GameObject fireEffect3;
    public GUNTYPE gunType;//枪械类型
    public Dictionary<GUNTYPE, int> magazineSize;//弹匣容量
    public Dictionary<GUNTYPE, int> reserveAmmo;//备用子弹
    public Dictionary<GUNTYPE, int> ammoInMag;//弹匣内剩余子弹
    public bool isReloading;//装填状态，装填中不能射击

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
        attackCD = 0.5f;
        attackTimer = 0;
        gunType = GUNTYPE.SINGLESHOTRIGLE;
        isReloading = false;
        InitAmmo();

        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false; // 隐藏鼠标
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        PlayerView();
        Attack();
        Reload();
        Jump();
        ChangeGunType();
    }

    private void InitAmmo()
    {
        magazineSize = new Dictionary<GUNTYPE, int>();
        reserveAmmo = new Dictionary<GUNTYPE, int>();
        ammoInMag = new Dictionary<GUNTYPE, int>();

        //设置弹匣容量
        magazineSize.Add(GUNTYPE.SINGLESHOTRIGLE, 20);
        magazineSize.Add(GUNTYPE.AUTORIFLE, 30);
        magazineSize.Add(GUNTYPE.SNIPERRIFLE, 5);

        //设置备弹量
        reserveAmmo.Add(GUNTYPE.SINGLESHOTRIGLE, 80);
        reserveAmmo.Add(GUNTYPE.AUTORIFLE, 120);
        reserveAmmo.Add(GUNTYPE.SNIPERRIFLE, 30);

        //游戏开始时向弹匣内添加满子弹
        ammoInMag.Add(GUNTYPE.SINGLESHOTRIGLE, magazineSize[GUNTYPE.SINGLESHOTRIGLE]);
        ammoInMag.Add(GUNTYPE.AUTORIFLE, magazineSize[GUNTYPE.AUTORIFLE]);
        ammoInMag.Add(GUNTYPE.SNIPERRIFLE, magazineSize[GUNTYPE.SNIPERRIFLE]);
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
        if (ammoInMag[gunType] > 0 && !isReloading)//弹匣内有子弹可以射击
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
                    SniperAttack();
                    break;
                default:
                    break;
            }
        }
        else//弹匣内没有子弹
        {
            //to do
        }
        
    }

    /// <summary>
    /// 重新装填
    /// </summary>
    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("Reloading");
            isReloading = true;
            Invoke("RecoverAttackState", 2.8f);//2.8是装填动画的时长

            if (reserveAmmo[gunType] == 0)
            {
                Debug.Log("备弹量为0，装填失败");
            }
            else if (reserveAmmo[gunType] + ammoInMag[gunType] <= magazineSize[gunType]) 
            {
                magazineSize[gunType] += reserveAmmo[gunType];
                reserveAmmo[gunType] = 0;
            }
            else
            {
                int reloadAmmo = magazineSize[gunType] - ammoInMag[gunType];
                ammoInMag[gunType] = magazineSize[gunType];
                reserveAmmo[gunType] -= reloadAmmo;
            }
        }
    }

    private void RecoverAttackState()
    {
        isReloading = false;
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRigidbody.AddForce(jumpforce * Vector3.up);
        }
    }

    /// <summary>
    /// 切换枪械类型
    /// </summary>
    private void ChangeGunType()
    {
        //按键切换
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gunType = GUNTYPE.SINGLESHOTRIGLE;
            attackCD = 0.4f;
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
            attackCD = 1f;
            Debug.Log("切换为狙击");
        }

        //滚轮切换
        //to do
    }

    /// <summary>
    /// 单点射击
    /// </summary>
    private void SingleShotAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            attackTimer = Time.time;
            ammoInMag[gunType]--;
            Debug.Log("剩余子弹数为" + ammoInMag[gunType]);

            GameObject fire = Instantiate(fireEffect, muzzleTransform);
            fire.transform.localPosition = Vector3.zero;
            fire.transform.localEulerAngles = Vector3.zero;//注意都是local

            animator.SetTrigger("SingleAttack");
            Invoke("GunAttack", 0.2f);//动画总时长0.5秒，后坐力产生大概是0.2秒左右
        }
    }
    /// <summary>
    /// 自动射击
    /// </summary>

    private void AutoShotAttack()
    {
        if(Input.GetMouseButton(0) && Time.time - attackTimer >= attackCD)
        {
            attackTimer = Time.time;
            ammoInMag[gunType]--;
            Debug.Log("剩余子弹数为" + ammoInMag[gunType]);

            GameObject fire = Instantiate(fireEffect2, muzzleTransform);
            fire.transform.localPosition = Vector3.zero;
            fire.transform.localEulerAngles = Vector3.zero;//注意都是local

            animator.SetBool("AutoAttack", true);
            GunAttack();

            //自动射击需要在子弹为0时直接停止射击动画，不然子弹为0时走不到下面的语句
            //只有自动射击的动画需要单独处理是因为，该动画切回idle状态是有条件的
            if(ammoInMag[gunType] == 0)
            {
                animator.SetBool("AutoAttack", false);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("AutoAttack", false);
        }
    }

    /// <summary>
    /// 狙击射击
    /// </summary>
    private void SniperAttack()
    {
        //未开镜
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            attackTimer = Time.time;
            ammoInMag[gunType]--;
            Debug.Log("剩余子弹数为" + ammoInMag[gunType]);

            GameObject fire = Instantiate(fireEffect3, muzzleTransform);
            fire.transform.localPosition = Vector3.zero;
            fire.transform.localEulerAngles = Vector3.zero;//注意都是local

            animator.SetTrigger("SniperAttack");
            Invoke("GunAttack", 0.2f);//动画总时长0.5秒，后坐力产生大概是0.2秒左右
        }

        //开镜部分
        //to do
    }

    /// <summary>
    /// 射击的射线检测
    /// </summary>
    private void GunAttack()
    {
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
