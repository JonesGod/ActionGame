using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBowPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            PlayerInput playerInput = other.transform.GetComponent<PlayerInput>();
            playerInput.BowUnlock();
        }
    }

}
