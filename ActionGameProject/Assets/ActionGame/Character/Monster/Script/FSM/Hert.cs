using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hert : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        //if(collision.transform.name== "mesh_masterSword")
            animator.SetTrigger("Take Damage");
    }
}
