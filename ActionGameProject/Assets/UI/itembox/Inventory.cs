using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Item> yourInventory = new List<Item>();

    public int slotsNumbr;

    public GameObject x;
    public int n;

    public Image[] slot;
    public Sprite[] slotSprite;
   

    public Text[] stackText;

   
    void Start()
    {
     
    }

   
    void Update()
    {
        for (int i = 0; i < slotsNumbr; i++)
        {
            slot[i].sprite = slotSprite[i];
        }

        for (int i = 0; i < slotsNumbr; i++)
        {
            slotSprite[i] = yourInventory[i].itemSprite;
        }


        


        if (PickingUp.y != null)
        {
            x = PickingUp.y;
            n = x.GetComponent<ThisItem>().thisId;
        }
        else
        {
            x = null;
        }

        if (PickingUp.pick == true)
        {
            for (int i = 0; i < slotsNumbr; i++) 
            {
                if (yourInventory[i].id == n)
                {
                    yourInventory[i].stack += 1;
                    i = slotsNumbr;
                    PickingUp.pick = false;
                }
            }


            for (int i = 0; i < slotsNumbr; i++)
            {
                if (yourInventory[i].id == 0 && PickingUp.pick == true)
                {
                    yourInventory[i] = Database.itemList[n];

                    yourInventory[i].stack += 1;

                    PickingUp.pick = false;
                }
            }
            PickingUp.pick = false;
        }

        for (int i = 0; i < slotsNumbr; i++)
        {
            stackText[i].text = "" + yourInventory[i].stack;
        }
    }
}
