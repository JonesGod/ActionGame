using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private List<FSMBase> monster;
    // Start is called before the first frame update
    void Start()
    {
        monster = new List<FSMBase>();
        GameObject[] allMonster = GameObject.FindGameObjectsWithTag("Test");//將場景裡的怪物存起來
        Debug.Log(allMonster.Length);
       if(allMonster!=null || allMonster.Length>0)
       {
           Debug.Log("ttttt");
            foreach(GameObject m in allMonster)
            {
                monster.Add(m.GetComponent<FSMBase>());
                Debug.Log(m.name);
            }
       }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
