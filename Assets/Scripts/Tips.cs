using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tips : MonoBehaviour
{
    public GameObject player;
    public GameObject tipPanel;
    public GameObject tip;
    public bool isTipsOpen;


    // Start is called before the first frame update
    void Start()
    {
        isTipsOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        ShowTipPanel();
    }

    private void ShowTipPanel()
    {
        //if(transform.tag == "HealBox")
        //{
        //    Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        //}
        if(Vector3.Distance(transform.position , player.transform.position) <= 2.0f)
        {
            tipPanel.SetActive(true);
            tip.SetActive(true);
            isTipsOpen = true;
        }
        else
        {
            if (isTipsOpen)
            {
                tipPanel.SetActive(false);
                tip.SetActive(false);
                isTipsOpen = false;
            }
        }
    }
}
