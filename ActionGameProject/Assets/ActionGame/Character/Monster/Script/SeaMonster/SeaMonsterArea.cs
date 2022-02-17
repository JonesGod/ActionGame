using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterArea : MonoBehaviour
{
    public GameObject[] trapMonsters;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            for(int i = 0; i < trapMonsters.Length; i++)
            {
                trapMonsters[i].SetActive(true);;
            }       
            Destroy(this);     
        }
    }
}
