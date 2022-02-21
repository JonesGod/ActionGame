using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossHeadHurt : MonoBehaviour
{
    public FSMBase myFSM;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.name == "mesh_masterSword" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(30, true, true); 
        }
            
        if(other.transform.name == "Arrow(Clone)" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(20, true, true);
        }
    }
}
