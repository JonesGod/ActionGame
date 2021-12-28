using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController characterController;
    public FolowCamera followCamera;

    public float speed = 4;
    private Vector3 velocity;
    public float gravity = -9.81f;

    private bool isGrounded = true;
    public float groundDistance = 0.2f;
    private Transform groundChecker;
    public LayerMask checkGroundLayerMask;

    private Animator m_Am;
    void Start()
    {
        characterController = GetComponent<CharacterController>();    
        groundChecker = transform.GetChild(0);

        m_Am = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = Vector3.zero;
        Debug.Log(v);
        if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") !=0)
        {
            Rotating(h, v);
            move = transform.forward * Mathf.Abs(v);
            move += transform.forward * Mathf.Abs(h);
            move = Vector3.Normalize(move) * speed * Time.deltaTime;

            m_Am.SetBool("RunBool", true);
        }
        else
        {
            m_Am.SetBool("RunBool", false);
        }
        characterController.Move(move);   

        // isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, checkGroundLayerMask, QueryTriggerInteraction.Ignore);
        // if (isGrounded && velocity.y < 0)
        //     velocity.y = 0f;        

        //velocity.y += gravity * Time.deltaTime; 
        //characterController.Move(velocity * Time.deltaTime);          
    }
    void Rotating(float moveH, float moveV)
    {
        // 建立角色目標方向的向量
        Vector3 newDirectionVector = followCamera.horizontalVector * moveV + (followCamera.cameraRight * moveH);
        Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
        transform.rotation = newRotation;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2.0f);

        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(groundChecker.position, groundDistance);
    }
}
