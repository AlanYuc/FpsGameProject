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
        ItemInteraction();
    }

    private void ShowTipPanel()
    {
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

    private void ItemInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && isTipsOpen)
        {
            Debug.Log(transform.tag);
            switch (transform.tag)
            {
                case "AmmoStack":
                    player.GetComponent<PlayerController>().FullAmmo();
                    break;
                case "HealBox":
                    player.GetComponent<PlayerController>().Recover();
                    break;
                case "SpawnEnemyBox":
                    player.GetComponent<PlayerController>().SpawnEnemy();
                    break;
                default:
                    break;
            }
        }
    }
}
