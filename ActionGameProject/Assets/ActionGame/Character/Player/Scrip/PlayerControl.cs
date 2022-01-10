using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController characterController;
    public FolowCamera followCamera;

    private Animator m_Am;
    private PlayerInput m_Input; //準備獲取玩家輸入
    private float speed = 4.0f;
    private float gravity=20.0f;
    private float fallSpeed;
    private bool isGrounded = true;

    Vector3 move = Vector3.zero;

    readonly int m_StateTime = Animator.StringToHash("StateTime");
    void Start()
    {
        characterController = GetComponent<CharacterController>();    
       
        m_Am = GetComponent<Animator>();
        m_Input = GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        CalculateGravity();

        if (m_Input.moveFlag)
        {
            //characterController.Move(move);
            m_Am.SetBool("RunBool", true);
        }
        else
        {
            m_Am.SetBool("RunBool", false);
                      
        }
        
        m_Am.SetFloat(m_StateTime, Mathf.Repeat(m_Am.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));//讓statetime不斷從0數到1
       
        m_Am.ResetTrigger("AttackTrigger");
        m_Am.ResetTrigger("SpecialAttackTrigger");
        m_Am.ResetTrigger("AvoidTrigger");

        if (m_Input.avoid)      //迴避
        {
            m_Am.SetTrigger("AvoidTrigger");
            m_Input.avoid = false;
        }
        if (m_Input.attack)   //左鍵攻擊
        {           
            m_Am.SetBool("RunBool", false);

            m_Am.SetTrigger("AttackTrigger");            
            m_Input.attack = false;
        }
        if(m_Input.specialAttack)
        {
            m_Am.SetTrigger("SpecialAttackTrigger");
            m_Input.specialAttack = false;
        }           
    }
    void CalculateGravity()
    {
        if(isGrounded)
        {
            fallSpeed = -gravity * 0.3f;
        }
        else
        {
            fallSpeed -= gravity * Time.deltaTime;
        }
    }
    void OnAnimatorMove()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position+Vector3.up,-Vector3.up);//在林克身上做一條雷射用以偵測四周
        if (Physics.Raycast(ray, out hit, 1.0f, Physics.AllLayers))
        {
            move= Vector3.ProjectOnPlane(m_Am.deltaPosition, hit.normal);
        }


        Rotating(m_Input.MoveInput.x, m_Input.MoveInput.y);

        move = transform.forward * Mathf.Abs(m_Input.MoveInput.y);
        move += transform.forward * Mathf.Abs(m_Input.MoveInput.x);
        move = Vector3.Normalize(move) * speed * Time.deltaTime;

        move +=fallSpeed * Vector3.up * Time.deltaTime;

        characterController.Move(move);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2.0f);

        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(groundChecker.position, groundDistance);
    }
    void Rotating(float moveH, float moveV)
    {
        // 建立角色目標方向的向量
        Vector3 newDirectionVector = followCamera.horizontalVector * moveV + (followCamera.cameraRight * moveH);
        Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
        transform.rotation = newRotation;
    }
}
