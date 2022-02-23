using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterShieldHurt : MonoBehaviour
{
    public RockMonsterFSM myFSM;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.name == "mesh_masterSword" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.ShieldHurt(30, false, false); 
        }
    }
}
