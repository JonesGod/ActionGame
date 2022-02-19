using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Item> yourInventory = new List<Item>();

    public List<Item> draggedItem = new List<Item>();

    public int slotsNumbr;

    public GameObject x;
    public int n;

    public Image[] slot;
    public Sprite[] slotSprite;
   

    public Text[] stackText;

    public int a;
    public int b;

    public int[] slotStack;
    public int maxStacks;

    public bool canConsume;


    void Start()
    {
        yourInventory[0] = Database.itemList[1];
        yourInventory[0].stack += 0;

        yourInventory[1] = Database.itemList[2];
        yourInventory[1].stack += 0;
    }

   
    void Update()
    {

        for (int i = 0; i < slotsNumbr; i++)
        {
            if (yourInventory[i].id == 0)
            {
                stackText[i].text = "";
            }
            else
            {
                stackText[i].text = "" + yourInventory[i].stack;
            }
        }




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

        //for (int i = 0; i < slotsNumbr; i++)
        //{
        //    stackText[i].text = "" + yourInventory[i].stack;
        //}

        


        /////////////////////////////////////////////////////////


        if (yourInventory[b].consumable == true)
        {
            canConsume = true;
        }
        else
        {
            canConsume = false;
        }

        if (canConsume == true && Input.GetKeyDown("1"))
        {
            if (slotStack[b] == 1)
            {
                PlayerControl.PLHP += yourInventory[b].nutritionalValue;
                yourInventory[b] = Database.itemList[0];
                slotStack[b] = 0;
            }
            else
            {
                slotStack[b]--;
                PlayerControl.PLHP += yourInventory[b].nutritionalValue;
            }
        }


       

    }

    public void StartDrag(Image slotX)
    {
        print("ST drag~~~" + slotX.name);
        for (int i = 0; i < slotsNumbr; i++)
        {
            if (slot[i] == slotX)
            {
                a = i;
            }
        }
    }


    public void Drop(Image slotX)
    {
        print("SP drag~~~" + slotX.name);
        if (a!=b)
        {
            draggedItem[0] = yourInventory[a];
            yourInventory[a] = yourInventory[b];
            yourInventory[b] = draggedItem[0];
            a = 0;
            b = 0;
        }        
    }

    public void Enter(Image slotX)
    {
        print("enter");
        for (int i = 0; i < slotsNumbr; i++)
        {
            if (slot[i] == slotX)
            {
                b = i;
            }
        }
    }


}
