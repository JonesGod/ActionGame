using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{

    private AudioSource testSource;
    // Start is called before the first frame update
    void Start()
    {
        
            testSource=GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("n"))
        {
            testSource.Play();
        }
    }
}
