using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMonsterHurt : MonoBehaviour
{
    public BasicFSM myFSM;
    public Health myHealth;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.name == "mesh_masterSword" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(30, false); 
            myHealth.ModifyHealth(30);  
        }
            
        if(other.transform.name=="Arrow(Clone)" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(20, false);
            myHealth.ModifyHealth(20);
        }
    }
}
