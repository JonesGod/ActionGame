using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterCoreHurt : MonoBehaviour
{
    public FSMBase myFSM;

    private void OnTriggerEnter(Collider other) 
    {            
        if(other.transform.name == "Arrow(Clone)" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(20, false, true);
        }
    }
}
