using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterHurt : MonoBehaviour
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
            Debug.Log("asdasdas");
            myFSM.CallHurt(20, false, false);
        }
    }
}
