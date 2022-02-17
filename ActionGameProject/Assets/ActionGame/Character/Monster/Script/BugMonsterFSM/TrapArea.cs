using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapArea : MonoBehaviour
{
    public TrapBasicMonsterFSM[] trapMonsters;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            for(int i = 0; i < trapMonsters.Length; i++)
            {
                trapMonsters[i].transform.Rotate(new Vector3(0, 0, 180), Space.Self);
                trapMonsters[i].myRigidbody.useGravity = true;
            }     
            Destroy(this);       
        }
    }
}
