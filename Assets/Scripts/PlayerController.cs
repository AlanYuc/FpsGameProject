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

    //���Բ�д��transform��Ĭ��Ϊ�ýű����ص������transform�����
    //public Transform transform;

    public float moveSpeed;//�ƶ��ٶ�
    public float mouseSensitivity;//��������ȣ���ʱ��ô��⣩
    public float cameraMinVerticalAngle;//��С�Ƕ�
    public float cameraMaxVerticalAngle;//���Ƕ�
    private float xRotation; // �洢��ֱ��ת�Ƕȣ�X�ᣩ
    private float yRotation; // �洢ˮƽ��ת�Ƕȣ�Y�ᣩ
    public Animator animator;
    public Rigidbody playerRigidbody;
    private float jumpforce;
    public Transform muzzleTransform;//ǹ��λ��
    public GameObject bloodEffect;//ѪҺ��Ч
    public GameObject wallEffect;//����·��ǽ����Ч
    public GameObject grassEffect;//���вݡ�Ҷ�ӵ���Ч
    public GameObject treeEffect;//����������Ч
    public GameObject riverEffect;//����ˮ����Ч
    public float attackCD;//���㿪ǹ��CD
    public float attackTimer;//��ǹ��ʱ������ʱ��
    public GameObject fireEffect;//��ǹ�������Ч
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

        Cursor.lockState = CursorLockMode.Locked; // �������
        Cursor.visible = false; // �������
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
    /// ����ƶ�
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
    /// ����ӽ���ת
    /// </summary>
    private void PlayerView()
    {
        //Quaternion.AngleAxis�ʺ��Ƶ�һ�����ת
        //�˴��ǵ�һ�˳��ӽ��������ת�����Quaternion.Euler������

        /*
        float offsetX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;//��ȡ���ˮƽƫ����
        float offsetY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;//��ȡ��괹ֱƫ����
        Quaternion rotationX = Quaternion.AngleAxis(offsetX, Vector3.up);//Vector3.up������Y���ˮƽ��ת
        transform.rotation *= rotationX;
        */

        float offsetX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;//��ȡ���ˮƽƫ����
        float offsetY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;//��ȡ��괹ֱƫ����

        //����ˮƽ��ת(Y��)
        yRotation += offsetX;

        //���㴹ֱ��ת(X��)
        xRotation -= offsetY;
        xRotation = Mathf.Clamp(xRotation, cameraMinVerticalAngle, cameraMaxVerticalAngle);

        //ʹ����Ԫ������������ת
        Quaternion verticalRotation = Quaternion.Euler(xRotation, 0f, 0f);//������ת(X��)
        Quaternion horizontalRotation = Quaternion.Euler(0f, yRotation, 0f);//������ת(Y��)

        //Ӧ����ת(��ˮƽ��ֱ���������������)
        transform.rotation = horizontalRotation * verticalRotation;
    }

    /// <summary>
    /// ����
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
        //�����л�
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gunType = GUNTYPE.SINGLESHOTRIGLE;
            attackCD = 0.8f;
            Debug.Log("�л�Ϊ����");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gunType = GUNTYPE.AUTORIFLE;
            attackCD = 0.1f;
            Debug.Log("�л�Ϊ�Զ�");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gunType = GUNTYPE.SNIPERRIFLE;
            Debug.Log("�л�Ϊ�ѻ�");
        }

        //�����л�
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
        fire.transform.localEulerAngles = Vector3.zero;//ע�ⶼ��local

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
