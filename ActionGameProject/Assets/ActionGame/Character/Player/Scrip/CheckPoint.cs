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
        PlayerControl player=other.GetComponent<PlayerControl>();

        if (player = null)
            return;

        player.SetCheckPoint(this);

    }
}
