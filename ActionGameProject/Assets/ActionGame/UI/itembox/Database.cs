using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static List<Item> itemList = new List<Item> ();

    //道具分配
    void Awake()
    {
        itemList.Add (new Item (0, "None", "None", Resources.Load <Sprite>("0"),0,false,0));
        itemList.Add(new Item(1, "HP pt", "It is item", Resources.Load<Sprite>("1"),0,true,100));
        itemList.Add(new Item(2, "MP pt", "It is item", Resources.Load<Sprite>("2"),0,true,100));


    }
}
