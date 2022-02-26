using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterDialog : MonoBehaviour
{

    public GameObject enterDialog;

    public GameObject hintbox;

    public GameObject fx;




    public void Start()
    {
        enterDialog.SetActive(false);

        hintbox.SetActive(false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            enterDialog.SetActive(true);

            hintbox.SetActive(true);

            fx.SetActive(false);
        }

    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        enterDialog.SetActive(false);
    //    }
    //}

}
