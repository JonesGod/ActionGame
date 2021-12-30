using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController characterController;
    public FolowCamera followCamera;

    private Animator m_Am;
    private PlayerInput m_Input; //準備獲取玩家輸入

    readonly int m_StateTime = Animator.StringToHash("statetime");
    void Start()
    {
        characterController = GetComponent<CharacterController>();    
       
        m_Am = GetComponent<Animator>();
        m_Input = GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        if (m_Input.Move != Vector3.zero)
        {
            characterController.Move(m_Input.Move);
            m_Am.SetBool("RunBool", true);
        }
        else
        {
            m_Am.SetBool("RunBool", false);
        }

        m_Am.SetFloat(m_StateTime, Mathf.Repeat(m_Am.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));//讓statetime不斷從0數到1
        m_Am.ResetTrigger("attack");

        if(m_Input.attack)   //左鍵攻擊
        {           
            m_Am.SetBool("RunBool", false);
            m_Am.SetTrigger("attack");            
            m_Input.attack = false;
        }
                
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2.0f);

        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(groundChecker.position, groundDistance);
    }
}
