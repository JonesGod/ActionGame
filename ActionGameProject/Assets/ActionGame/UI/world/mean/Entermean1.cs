using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entermean1 : MonoBehaviour
{

    public GameObject hintbox;

    public GameObject fx;

    public GameObject hide;




    public void Start()
    {


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            hintbox.SetActive(true);

            fx.SetActive(false);

            hide.SetActive(false);
        }

    }



}
