using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Transform[] link;
    public Transform sword;

    private bool attackState;
    // Start is called before the first frame update
    void Start()
    {
        link = GetComponentsInChildren<Transform>(true);
        foreach(Transform child in link)
        {
            if (child.name == "mesh_masterSword")
            {
                sword = child;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SwordOn()
    {
        sword.gameObject.SetActive(true);
    }
    void SwordOff()
    {
        sword.gameObject.SetActive(false);
    }
}
