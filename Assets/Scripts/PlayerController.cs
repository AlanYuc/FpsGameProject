using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    //���Բ�д��transform��Ĭ��Ϊ�ýű����ص������transform�����
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
        float offsetX = Input.GetAxis("Mouse X");//��ȡ���ˮƽƫ����
        float offsetY = Input.GetAxis("Mouse Y");//��ȡ��괹ֱƫ����
        //��Ԫ����ת
        Quaternion rotationX = Quaternion.AngleAxis(offsetX, Vector3.up);//Vector3.up������Y���ˮƽ��ת
        transform.rotation *= rotationX;
    }
}
