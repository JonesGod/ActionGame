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

    public int slotTmporary;

    public int rest;
    public bool shift;

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
        if (Input.GetKeyDown("left shift"))
        {
            shift = true;
        }
        if (Input.GetKeyUp("left shift"))
        {
            shift = false;
        }



        for (int i = 0; i < slotsNumbr; i++)
        {
            if (yourInventory[i].id == 0)
            {
                stackText[i].text = "";
            }
            else
            {
                stackText[i].text = "" + slotStack[i];
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

                    if (slotStack[i] == maxStacks)
                    {
                        continue;
                    }
                    else
                    {
                        slotStack[i] += 1;
                        i = slotsNumbr;
                        PickingUp.pick = false;
                    }
                }
            }


            for (int i = 0; i < slotsNumbr; i++)
            {
                if (yourInventory[i].id == 0 && PickingUp.pick == true)
                {
                    yourInventory[i] = Database.itemList[n];

                    slotStack[i] += 1;

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
            
            if (slotStack[b] < 2)
            {
                PlayerControl.HpIncrease(yourInventory[b].nutritionalValue);
                yourInventory[b] = Database.itemList[0];
                slotStack[b] = 0 ;
            }
            else
            {
                slotStack[b]--;
                PlayerControl.HpIncrease(yourInventory[b].nutritionalValue);
            }
        }

        if (yourInventory[a].consumable == true)
        {
            canConsume = true;
        }
        else
        {
            canConsume = false;
        }

        if (canConsume == true && Input.GetKeyDown("2"))
        {
            if (slotStack[a] < 2)
            {
                PlayerControl.MpIncrease(yourInventory[a].nutritionalValue);
                yourInventory[a] = Database.itemList[0];
                slotStack[a] = 0;
            }
            else
            {
                slotStack[a]--;
                PlayerControl.MpIncrease(yourInventory[a].nutritionalValue);
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

        if (shift == true)
        {
            if (yourInventory[b].id == 0)
            {
                yourInventory[b] = yourInventory[a];
                slotStack[b] = slotStack[a] / 2;
                rest = slotStack[a] % 2;
                slotStack[a] = slotStack[a] / 2 + rest;
            }
        }
        else
        {
            print("SP drag~~~" + slotX.name);
            if (a != b)
            {
                if (yourInventory[a].id != yourInventory[b].id)
                {
                    draggedItem[0] = yourInventory[a];
                    slotTmporary = slotStack[a];
                    yourInventory[a] = yourInventory[b];
                    slotStack[a] = slotStack[b];
                    yourInventory[b] = draggedItem[0];
                    slotStack[b] = slotTmporary;
                    a = 0;
                    b = 0;
                }
                else
                {
                    if (slotStack[a] + slotStack[b] <= maxStacks)
                    {
                        slotStack[b] = slotStack[a] + slotStack[b];
                        yourInventory[a] = Database.itemList[0];
                    }
                    else
                    {
                        slotStack[a] = slotStack[a] + slotStack[b] - maxStacks;
                        slotStack[b] = maxStacks;
                    }
                }

            }
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
