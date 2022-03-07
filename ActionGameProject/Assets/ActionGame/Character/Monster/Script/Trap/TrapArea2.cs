using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapArea2 : MonoBehaviour
{
    public BasicFSM[] trapMonsters;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            for(int i = 0; i < trapMonsters.Length; i++)
            {
                trapMonsters[i].transform.gameObject.SetActive(true);
                trapMonsters[i].data.sightRange = 57.6f;
            }
            Destroy(this.gameObject);
        }
    }
}
