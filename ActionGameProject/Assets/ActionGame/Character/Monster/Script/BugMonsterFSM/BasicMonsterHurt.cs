using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMonsterHurt : MonoBehaviour
{
    public BasicFSM myFSM;
    public Health myHealth;

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log(other.transform.name);
        
        if(other.transform.name == "mesh_masterSword" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(30); 
            myHealth.ModifyHealth(30);  
        }
            
        if(other.transform.name == "Arrow(Clone)" && myFSM.currentState != FSMState.Dead)
        {
            myFSM.CallHurt(20);
            myHealth.ModifyHealth(20);
        }
    }
}
