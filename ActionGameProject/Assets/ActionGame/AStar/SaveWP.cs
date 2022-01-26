using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveWP : MonoBehaviour
{
    void Start()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("WayPoint");
        StreamWriter sw = new StreamWriter("Assets/ActionGame/AStar/WPData.txt", false);
        Debug.Log("寫入WP資料");

        string s = "";
        for(int i = 0; i < gos.Length; i++)
        {
            s = "";
            s += gos[i].name;



            sw.WriteLine(s);
        }
        sw.Close();        
    }
}
