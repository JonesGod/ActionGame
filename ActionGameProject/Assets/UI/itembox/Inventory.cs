using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventory;
    public GameObject slotHolder;

    private int slots;
    private Transform[] slot;

    private GameObject itemPickedUP;
    private bool itemAdded;

    public void Start()
    {
        slots = slotHolder.transform.childCount;

        slot = new Transform[slots];
        DetectInventorySlots();

    }

    public void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        //other.tag ==  "Item"
        if (other.gameObject.GetComponent<Item>())
        {
            //print("466");
            itemPickedUP = other.gameObject;
            AddItem(itemPickedUP); 
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "item")
        {
            itemAdded = false;
        }
    }

    public void AddItem(GameObject item)
    {
        for (int i = 0; i < slots; i++)
        {
            if (slot[i].GetComponent<Slot>().empty && itemAdded == false)
            {
                slot[i].GetComponent<Slot>().item = itemPickedUP;
                slot[i].GetComponent<Slot>().itemIcon = itemPickedUP.GetComponent<Item>().icon;
                itemAdded = true;
            }
        }
    }

    public void DetectInventorySlots()
    {
        for (int i = 0; i < slots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i);
            
        }
    }
}
