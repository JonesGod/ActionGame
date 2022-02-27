using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entermean2 : MonoBehaviour
{
    public GameObject hintbox;


    public void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (hintbox)
        {

            if (other.tag == "Player")
            {
                hintbox.SetActive(true);
            }

        }




    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(hintbox);
        }
    }
}
