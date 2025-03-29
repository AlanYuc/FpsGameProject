using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        transform.position = new Vector3(-0.2f, transform.position.y, transform.position.z);
        moveSpeed = 1.5f;
        mouseSensitivity = 800.0f;
        cameraMinVerticalAngle = -60f;
        cameraMaxVerticalAngle = 60f;
        xRotation = 0f;
        yRotation = 0f;

        //Cursor.lockState = CursorLockMode.Locked; // �������
        //Cursor.visible = false; // �������
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        PlayerView();
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
}
