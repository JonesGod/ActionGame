using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    public BasicFSM myFSM;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log(other.transform.name);
        
        if(other.transform.name == "mesh_masterSword" && myFSM.currentState != BasicFSM.FSMState.Dead)
            myFSM.CallHurt();   
    }
}
