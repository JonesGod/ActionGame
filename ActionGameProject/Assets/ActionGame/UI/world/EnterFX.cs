using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterFX : MonoBehaviour
{
    public GameObject enterDialog;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            enterDialog.SetActive(false);
        }

    }


}
