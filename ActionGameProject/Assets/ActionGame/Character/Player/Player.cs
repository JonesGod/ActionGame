using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController characterController;
    public float speed = 4;
    void Start()
    {
        characterController = GetComponent<CharacterController>();        
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = Vector3.zero;
        if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") !=0)
        {
            // move += transform.forward * Mathf.Abs(v);
            // move += transform.right * Mathf.Abs(h);
            move += transform.forward * v;
            move += transform.right * h;
            move = move * speed * Time.deltaTime;

        }
        characterController.Move(move);           
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2.0f);
    }
}
