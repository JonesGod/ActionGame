using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController characterController;
    public FolowCamera followCamera;

    private Animator m_Am;
    private PlayerInput m_Input; //準備獲取玩家輸入
    private float speed = 5.0f;
    private float gravity=20.0f;
    private float rollSpeed=3.0f;
    private float fallSpeed;
    private float totalSpeed;


    private bool isGrounded = true;
    private bool attackState;
    private bool rollState;
    private bool nextIsIdleState;
    private bool nextIsRoll;

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
        AnimatorStateInfo stateinfo = m_Am.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateinfo = m_Am.GetNextAnimatorStateInfo(0);

        CalculateGravity();

        attackState = GetAttackState(stateinfo, nextStateinfo);
        rollState= GetRollState(stateinfo);
        nextIsIdleState = GetIdleState(nextStateinfo);
        nextIsRoll=GetNextRollState(nextStateinfo);

        if (m_Input.moveFlag)
        {
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
            Rotating(m_Input.MoveInput.x, m_Input.MoveInput.y);

            m_Am.SetTrigger("AvoidTrigger");
            m_Input.avoid = false;
        }
        if (m_Input.attack)   //左鍵攻擊
        {          
            m_Am.SetTrigger("AttackTrigger");
            m_Input.attack = false;
        }

        if(m_Input.specialAttack)
        {
            m_Am.SetTrigger("SpecialAttackTrigger");
            m_Input.specialAttack = false;
        }
        
        if (m_Input.moveFlag && !attackState && !rollState)
            Rotating(m_Input.MoveInput.x, m_Input.MoveInput.y);
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
        Ray ray = new Ray(transform.position+Vector3.up,-Vector3.up);//在林克身上做一條與Y軸平行的雷射用以偵測四周
        if (Physics.Raycast(ray, out hit, 1.0f, Physics.AllLayers))
        {
            move= Vector3.ProjectOnPlane(m_Am.deltaPosition, hit.normal);
        }
       
        move = transform.forward * Mathf.Abs(m_Input.MoveInput.y);
        move += transform.forward * Mathf.Abs(m_Input.MoveInput.x);
        if (rollState || nextIsRoll)
            move = transform.forward * (speed + rollSpeed) * Time.deltaTime;
        else
            move = Vector3.Normalize(move) * speed * Time.deltaTime;

        if (nextIsIdleState)
            move = transform.forward*0.0f;

        move +=fallSpeed * Vector3.up * Time.deltaTime;

        if(!attackState || nextIsRoll)
            characterController.Move(move);
    }   
    void Rotating(float moveH, float moveV)
    {
        // 建立角色目標方向的向量
        Vector3 newDirectionVector = followCamera.horizontalVector * moveV + (followCamera.cameraRight * moveH);
        if (newDirectionVector == Vector3.zero)
            newDirectionVector = transform.forward;
        Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
        characterController.transform.rotation = newRotation;
    }
    public bool GetAttackState(AnimatorStateInfo stateinfo, AnimatorStateInfo nextStateinfo)
    {                
        bool nextAttackState = nextStateinfo.IsName("attack01");        

        bool specialAttackState = stateinfo.IsName("specialAttack");
        bool attack01 = stateinfo.IsName("attack01");
        bool attack02 = stateinfo.IsName("attack02");
        bool attack03 = stateinfo.IsName("attack03");

        var attackstate = nextAttackState
                       || specialAttackState || attack01 || attack02 || attack03;

        return attackstate;
    }
    bool GetRollState(AnimatorStateInfo stateinfo)
    {       
        bool roll = stateinfo.IsName("Roll");
        return roll;
    }
    bool GetNextRollState(AnimatorStateInfo nextStateinfo)
    {
        bool nextRoll = nextStateinfo.IsName("Roll");
        return nextRoll;
    }
    bool GetIdleState(AnimatorStateInfo nextStateinfo)
    {
        bool idle= nextStateinfo.IsName("Idle");
        return idle;
    }
    
}
