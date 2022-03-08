using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossBodyHurt : MonoBehaviour
{
    public FSMBase myFSM;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.name == "mesh_masterSword" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(30, false, false); 
        }
            
        if(other.transform.name == "Arrow(Clone)" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(20, false, false);
        }
        if(other.transform.name == "ExplosiveArrow" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(60, false, false);
        }
        if (other.transform.name == "AOE (1)" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(50, false, true);
        }
    }
}
