using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject fireEffect2;
    public GameObject fireEffect3;
    public GUNTYPE gunType;//ǹе����
    public Dictionary<GUNTYPE, int> magazineSize;//��ϻ����
    public Dictionary<GUNTYPE, int> reserveAmmo;//�����ӵ�
    public Dictionary<GUNTYPE, int> ammoInMag;//��ϻ��ʣ���ӵ�
    public Dictionary<GUNTYPE, int> weaponDamage;//��ͬ�������˺�
    public bool isReloading;//װ��״̬��װ���в������
    public GameObject[] gun;//���治ͬ��ǹ
    public GameObject scope;//�ѻ���
    public bool isScopeOpen;
    public int HP;
    public AudioSource audioSource;
    public AudioClip singleShotRifleAudioClip;
    public AudioClip autoShotRifleAudioClip;
    public AudioClip sniperRifleAudioClip;
    public AudioClip reloadingAudioClip;
    public AudioClip hitGroundAudioClip;//���е������Ч
    public AudioClip jumpAudioClip;
    public AudioSource walkAudioSource;

    public Text playerHP;
    public Text ammoInMagText;
    public Text reserveAmmoText;
    public Text moneyText;
    public GameObject SingleShotRifleUI;
    public GameObject AutoShotRifleUI;
    public GameObject SniperRifleUI;
    public GameObject BloodUI;
    public Dictionary<GUNTYPE, GameObject> gunUI;


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
        isScopeOpen = false;
        HP = 100;
        InitAmmo();
        InitUI();

        Cursor.lockState = CursorLockMode.Locked; // �������
        Cursor.visible = false; // �������
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
        ScopeControl();
        UpdateUI();
    }

    private void InitAmmo()
    {
        magazineSize = new Dictionary<GUNTYPE, int>();
        reserveAmmo = new Dictionary<GUNTYPE, int>();
        ammoInMag = new Dictionary<GUNTYPE, int>();
        weaponDamage = new Dictionary<GUNTYPE, int>();

        //���õ�ϻ����
        magazineSize.Add(GUNTYPE.SINGLESHOTRIGLE, 20);
        magazineSize.Add(GUNTYPE.AUTORIFLE, 30);
        magazineSize.Add(GUNTYPE.SNIPERRIFLE, 5);

        //���ñ�����
        reserveAmmo.Add(GUNTYPE.SINGLESHOTRIGLE, 80);
        reserveAmmo.Add(GUNTYPE.AUTORIFLE, 120);
        reserveAmmo.Add(GUNTYPE.SNIPERRIFLE, 30);

        //��Ϸ��ʼʱ��ϻ��������ӵ�
        ammoInMag.Add(GUNTYPE.SINGLESHOTRIGLE, magazineSize[GUNTYPE.SINGLESHOTRIGLE]);
        ammoInMag.Add(GUNTYPE.AUTORIFLE, magazineSize[GUNTYPE.AUTORIFLE]);
        ammoInMag.Add(GUNTYPE.SNIPERRIFLE, magazineSize[GUNTYPE.SNIPERRIFLE]);

        //�����������˺�
        weaponDamage.Add(GUNTYPE.SINGLESHOTRIGLE, 5);
        weaponDamage.Add(GUNTYPE.AUTORIFLE, 1);
        weaponDamage.Add(GUNTYPE.SNIPERRIFLE, 50);
    }

    private void InitUI()
    {
        gunUI = new Dictionary<GUNTYPE, GameObject>();

        gunUI.Add(GUNTYPE.SINGLESHOTRIGLE, SingleShotRifleUI);
        gunUI.Add(GUNTYPE.AUTORIFLE, AutoShotRifleUI);
        gunUI.Add(GUNTYPE.SNIPERRIFLE, SniperRifleUI);
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

        animator.SetFloat("MoveX", offsetH);
        animator.SetFloat("MoveY", offsetV);

        if(!(offsetH ==0 && offsetV == 0))
        {
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
        }
        else
        {
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
        }
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
        if (ammoInMag[gunType] > 0 && !isReloading)//��ϻ�����ӵ��������
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
        else//��ϻ��û���ӵ�
        {
            //to do
        }
        
    }

    /// <summary>
    /// ����װ��
    /// </summary>
    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("Reloading");
            isReloading = true;
            Invoke("RecoverAttackState", 2.8f);//2.8��װ�����ʱ��

            if (reserveAmmo[gunType] == 0)
            {
                Debug.Log("������Ϊ0��װ��ʧ��");
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
    /// ����ǹеģ��
    /// </summary>
    /// <param name="cur"></param>
    private void ChangeGunGameobject(int cur)
    {
        for(int i= 0; i < gun.Length; ++i)
        {
            gun[i].SetActive(false);
        }
        gun[cur].SetActive(true);
    }

    /// <summary>
    /// ��Ծ
    /// </summary>
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRigidbody.AddForce(jumpforce * Vector3.up);
            audioSource.PlayOneShot(jumpAudioClip);
        }
    }

    /// <summary>
    /// �л�ǹе����
    /// </summary>
    private void ChangeGunType()
    {
        //�����л�
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gunType = GUNTYPE.SINGLESHOTRIGLE;
            attackCD = 0.4f;
            ChangeGunGameobject(0);
            Debug.Log("�л�Ϊ����");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gunType = GUNTYPE.AUTORIFLE;
            attackCD = 0.1f;
            ChangeGunGameobject(1);
            Debug.Log("�л�Ϊ�Զ�");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gunType = GUNTYPE.SNIPERRIFLE;
            attackCD = 1f;
            ChangeGunGameobject(2);
            Debug.Log("�л�Ϊ�ѻ�");
        }
        ChangeGunUI();

        //�����л�
        //to do
    }

    /// <summary>
    /// �������
    /// </summary>
    private void SingleShotAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            PlaySound(singleShotRifleAudioClip);
            attackTimer = Time.time;
            ammoInMag[gunType]--;
            Debug.Log("ʣ���ӵ���Ϊ" + ammoInMag[gunType]);

            GameObject fire = Instantiate(fireEffect, muzzleTransform);
            fire.transform.localPosition = Vector3.zero;
            fire.transform.localEulerAngles = Vector3.zero;//ע�ⶼ��local

            animator.SetTrigger("SingleAttack");
            Invoke("GunAttack", 0.2f);//������ʱ��0.5�룬���������������0.2������
        }
    }
    /// <summary>
    /// �Զ����
    /// </summary>

    private void AutoShotAttack()
    {
        if(Input.GetMouseButton(0) && Time.time - attackTimer >= attackCD)
        {
            PlaySound(autoShotRifleAudioClip);
            attackTimer = Time.time;
            ammoInMag[gunType]--;
            Debug.Log("ʣ���ӵ���Ϊ" + ammoInMag[gunType]);

            GameObject fire = Instantiate(fireEffect2, muzzleTransform);
            fire.transform.localPosition = Vector3.zero;
            fire.transform.localEulerAngles = Vector3.zero;//ע�ⶼ��local

            animator.SetBool("AutoAttack", true);
            GunAttack();

            //�Զ������Ҫ���ӵ�Ϊ0ʱֱ��ֹͣ�����������Ȼ�ӵ�Ϊ0ʱ�߲�����������
            //ֻ���Զ�����Ķ�����Ҫ������������Ϊ���ö����л�idle״̬����������
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
    /// �ѻ����
    /// </summary>
    private void SniperAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            PlaySound(sniperRifleAudioClip);
            attackTimer = Time.time;
            ammoInMag[gunType]--;
            Debug.Log("ʣ���ӵ���Ϊ" + ammoInMag[gunType]);

            GameObject fire = Instantiate(fireEffect3, muzzleTransform);
            fire.transform.localPosition = Vector3.zero;
            fire.transform.localEulerAngles = Vector3.zero;//ע�ⶼ��local

            animator.SetTrigger("SniperAttack");
            Invoke("GunAttack", 0.2f);//������ʱ��0.5�룬���������������0.2������
        }
    }

    private void ScopeControl()
    {
        if (Input.GetMouseButtonDown(1) && gunType == GUNTYPE.SNIPERRIFLE) 
        {
            if (isScopeOpen)
            {
                scope.SetActive(false);
                isScopeOpen = false;
            }
            else
            {
                scope.SetActive(true);
                isScopeOpen = true;
            }
        }
    }

    /// <summary>
    /// ��������߼��
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
                case "Enemy": Instantiate(bloodEffect, hit.point, Quaternion.identity);
                    hit.collider.GetComponent<Enemy>().TakeDamage(weaponDamage[gunType]);
                    break;
                case "Wall": Instantiate(wallEffect, hit.point, Quaternion.identity);
                    PlaySound(hitGroundAudioClip);
                    break;
                case "Grass": Instantiate(grassEffect, hit.point, Quaternion.identity);
                    PlaySound(hitGroundAudioClip); 
                    break;
                case "Tree": Instantiate(treeEffect, hit.point, Quaternion.identity);
                    PlaySound(hitGroundAudioClip); 
                    break;
                case "River": Instantiate(riverEffect, hit.point + Vector3.up * 0.2f, Quaternion.identity);
                    PlaySound(hitGroundAudioClip); 
                    break;
            }
        }
    }

    /// <summary>
    /// �ܵ�����
    /// </summary>
    public void TakeDamage(int damage)
    {
        HP -= damage;
        Debug.Log("���ʣ��HPΪ" + HP);

        if (HP <= 0)
        {
            HP = 0;
        }

        playerHP.text = HP.ToString();
        BloodUI.SetActive(true);
        Invoke("DelayHideBloodUI", 0.4f);
    }

    private void DelayHideBloodUI()
    {
        BloodUI.SetActive(false);
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    private void UpdateUI()
    {
        //�ӵ���������
        if (!isReloading)
        {
            ammoInMagText.text = ammoInMag[gunType].ToString();
            reserveAmmoText.text = reserveAmmo[gunType].ToString();
        }
        //��Ҹ���
        //moneyText.text 
    }

    private void ChangeGunUI()
    {
        //��ǰʹ��ǹеUI����
        gunUI[GUNTYPE.SINGLESHOTRIGLE].transform.Find("mask").gameObject.SetActive(true);
        gunUI[GUNTYPE.SINGLESHOTRIGLE].transform.Find("GunName").gameObject.SetActive(false);
        gunUI[GUNTYPE.AUTORIFLE].transform.Find("mask").gameObject.SetActive(true);
        gunUI[GUNTYPE.AUTORIFLE].transform.Find("GunName").gameObject.SetActive(false);
        gunUI[GUNTYPE.SNIPERRIFLE].transform.Find("mask").gameObject.SetActive(true);
        gunUI[GUNTYPE.SNIPERRIFLE].transform.Find("GunName").gameObject.SetActive(false);

        gunUI[gunType].transform.Find("mask").gameObject.SetActive(false);
        gunUI[gunType].transform.Find("GunName").gameObject.SetActive(true);

        Debug.Log(gunUI[gunType].transform.Find("GunName").gameObject.name);
    }
}
