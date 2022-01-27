using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt : MonoBehaviour
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
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if(collision.transform.name== "mesh_masterSword")
    //         animator.SetTrigger("TakeDamage");
    //         Debug.Log("damage");
    // }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.name == "mesh_masterSword")
            animator.SetTrigger("TakeDamage");
            Debug.Log("damage");        
    }
}
