using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "NewLink")
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            player.SetCheckPoint(this);
        }

    }
}
