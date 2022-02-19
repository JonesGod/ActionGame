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
        itemList.Add(new Item(1, "A", "It is item", Resources.Load<Sprite>("1"),0,true,1));
        itemList.Add(new Item(2, "B", "It is item", Resources.Load<Sprite>("2"),0,false,0));
        itemList.Add(new Item(3, "C", "It is item", Resources.Load<Sprite>("3"),0,false,0));
        itemList.Add(new Item(4, "D", "It is item", Resources.Load<Sprite>("4"),0,false,0));
        itemList.Add(new Item(5, "E", "It is item", Resources.Load<Sprite>("5"),0,false,0));

    }
}
