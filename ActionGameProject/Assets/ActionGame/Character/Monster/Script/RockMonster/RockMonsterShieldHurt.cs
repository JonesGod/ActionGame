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
        if (other.transform.name == "AOE (1)" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(50, false, true);
        }
    }
}
