using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickingUp : MonoBehaviour
{
    public GameObject Item;
    public bool canPickUp;
    public int Items;
    public GameObject pickUpText;

    public static bool pick;
    public static GameObject y;

    // Start is called before the first frame update
    void Start()
    {
        Items = 0;

        pick = false;
    } 

    // Update is called once per frame
    void Update()
    {
        if (canPickUp == true)
        {
            //拾取道具後刪除已拾取的道具物件
            //並關閉拾取開關
            if (Input.GetKeyDown(KeyCode.E ))
            {
                Destroy(Item);
                Item = null;
                //Items += 1;

                pick = true;
                canPickUp = false;
            }
        }


        if (canPickUp == true)
        {
            pickUpText.SetActive(true);
        }
        else
        {
            pickUpText.SetActive(false);
        }

    }
    //顯示+執行拾取
     void OnTriggerEnter(Collider col)
    {
        if (col.tag == "item")
        {
            y = col.gameObject;
            Item = col.gameObject;
            canPickUp = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "item")
        {
            y = null;
            Item = null;
            canPickUp = false;
        }
    }


}
