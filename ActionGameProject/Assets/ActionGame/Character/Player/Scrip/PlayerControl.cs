using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController characterController;
    public FolowCamera followCamera;

    private Animator m_Am;
    private PlayerInput m_Input; //準備獲取玩家輸入

    private float rotateSpeed = 10.0f;
    private float speed = 10.0f;
    private float gravity = 20.0f;
    private float rollSpeed = 15.0f;
    private float statetime;
    private float fallSpeed;
    private float mouse;
    private float normalMove;
    private float bowRightMove;

    public int sensitivity=12;

    AnimatorStateInfo stateinfo;
    AnimatorStateInfo nextStateinfo;       

    readonly int hashAttack01 = Animator.StringToHash("attack01");
    readonly int hashAttack02 = Animator.StringToHash("attack02");
    readonly int hashAttack03 = Animator.StringToHash("attack03");
    readonly int hashAttack04 = Animator.StringToHash("attack04");
    readonly int hashSpecialAttackState=Animator.StringToHash("specialAttackState");
    readonly int hashRoll=Animator.StringToHash("Roll");
    readonly int hashIdle= Animator.StringToHash("Idle");
    readonly int m_StateTime = Animator.StringToHash("StateTime");
    readonly int hashBowIdle = Animator.StringToHash("BowIdle");

    private bool isGrounded = true;
    private bool attackState;
    private bool rollState;
    private bool bowState;
    private bool idleIsNext;
    private bool rollIsNext;
    private bool isTrasition;
    private bool bowIsNext;

    Vector3 move = Vector3.zero;
    Vector2 moveInput;
    Vector2 runInput;

    void Start()
    {
        characterController = GetComponent<CharacterController>();    
       
        m_Am = GetComponent<Animator>();
        m_Input = GetComponent<PlayerInput>();
       
    }
    void Update()
    {
        BowAngle();

        moveInput = PlayerInput.Instance.MoveInput;
        runInput = PlayerInput.Instance.MoveInput;
    }
    void FixedUpdate()
    {       
        stateinfo = m_Am.GetCurrentAnimatorStateInfo(0);
        nextStateinfo = m_Am.GetNextAnimatorStateInfo(0);
        isTrasition = m_Am.IsInTransition(0);

        m_Am.SetFloat(m_StateTime, Mathf.Repeat(m_Am.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));//讓statetime不斷從0數到1
        statetime = m_Am.GetFloat("StateTime");

        CalculateGravity();

        if (m_Input.bowState)   //弓狀態與一般狀態的基本參數改變
        {
            BowBasicValue();
        }
        else
        {
            NormalBasicValue();
        }

        CantBowToRoll();
        if (m_Input.bowState && !attackState)
        {
            m_Am.SetBool("BowBool", true);
        }
        else
        {
            m_Am.SetBool("BowBool", false);
        }

        if (m_Input.moveFlag)
            m_Am.SetBool("RunBool", true);
        else
        {
            moveInput = Vector2.zero;
            m_Am.SetBool("RunBool", false);
        }

        ResetTrigger();
        if (m_Input.avoid )      //迴避
        {            
            if (statetime >= 0.5f && !isTrasition && !rollState)
            {
                RollRotating(moveInput.x, moveInput.y);
            }
           
            m_Am.SetTrigger("AvoidTrigger");
            PlayerInput.Instance.bowState = false;
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
        
        GetAttackState();
        GetCurrentState();
        GetNextState();
        if (m_Input.moveFlag && !attackState && !rollState && !rollIsNext)
            Rotating(moveInput.x, moveInput.y);
        
    }
    
    void OnAnimatorMove()
    {
        stateinfo = m_Am.GetCurrentAnimatorStateInfo(0);
 
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);//在林克身上做一條與Y軸平行的雷射用以偵測四周
        if (Physics.Raycast(ray, out hit, 1.0f, Physics.AllLayers))
        {
            move = Vector3.ProjectOnPlane(m_Am.deltaPosition, hit.normal);
        }

        if (!attackState || rollIsNext)//讀取按鍵決定方向
            move = followCamera.horizontalVector * moveInput.y + followCamera.cameraRight * moveInput.x;
        else
            move = Vector3.zero;

        if (rollState || rollIsNext)
            move = transform.forward * rollSpeed * Time.deltaTime;
        else
            move = Vector3.Normalize(move) * speed * Time.deltaTime;

        if (idleIsNext || bowIsNext)
            move = transform.forward * 0.0f;

        move += fallSpeed * Vector3.up * Time.deltaTime;
        move += m_Am.deltaPosition;

        characterController.Move(move);
    }
    void CalculateGravity()
    {        
        if (characterController.isGrounded)
        {
            fallSpeed = -gravity * 0.3f;
        }
        else
        {
            fallSpeed -= gravity * Time.deltaTime;
        }
    }
    void Rotating(float moveH, float moveV)
    {
        // 建立角色目標方向的向量                  
        Vector3 newDirectionVector = followCamera.horizontalVector * moveV + followCamera.cameraRight * moveH;       
        if(newDirectionVector != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
            characterController.transform.rotation = Quaternion.Lerp(characterController.transform.rotation, newRotation, Time.deltaTime * rotateSpeed);
        }    
    }
    void RollRotating(float moveH, float moveV)
    {
        Vector3 newDirectionVector = (followCamera.horizontalVector * moveV + followCamera.cameraRight * moveH).normalized;
        if (newDirectionVector != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
            characterController.transform.rotation = newRotation;
        }
    }
    void GetAttackState()
    {                                 
        if(stateinfo.shortNameHash == hashAttack01 ||
           stateinfo.shortNameHash == hashAttack02 ||
           stateinfo.shortNameHash == hashAttack03 ||
           stateinfo.shortNameHash == hashAttack04 ||
           stateinfo.shortNameHash == hashSpecialAttackState||
           nextStateinfo.shortNameHash== hashAttack01)
        {
            attackState = true;
        }
        else        
            attackState = false;

        PlayerInput.Instance.attackState = attackState;
    }
    void GetCurrentState()
    {        
        if (stateinfo.shortNameHash == hashRoll)
            rollState = true;
        else
            rollState = false;

        if (stateinfo.shortNameHash == hashBowIdle)
            bowState = true;
        else
            bowState = false;

        PlayerInput.Instance.rollState = rollState;
    }    
    void GetNextState()
    {
        if (nextStateinfo.shortNameHash == hashRoll)
            rollIsNext = true;
        else
            rollIsNext = false;

        if (nextStateinfo.shortNameHash == hashIdle)
            idleIsNext = true;
        else
            idleIsNext = false;

        if (nextStateinfo.shortNameHash == hashBowIdle)
            bowIsNext = true;
        else
            bowIsNext = false;

        PlayerInput.Instance.rollIsNext = rollIsNext;
    }
    void ResetTrigger()
    {
        m_Am.ResetTrigger("AttackTrigger");
        m_Am.ResetTrigger("SpecialAttackTrigger");
        m_Am.ResetTrigger("AvoidTrigger");
    }
    void BowAngle()
    {
        mouse-=PlayerInput.Instance.MouseInput.y*sensitivity;
        if (mouse > 260f)
            mouse = 260f;
        else if (mouse < -240f)
            mouse = -240f;

        m_Am.SetFloat("BowAngle",mouse+500f);
    }
    void NormalBasicValue()
    {
        normalMove = (Mathf.Abs(runInput.x) + Mathf.Abs(runInput.y)) * 2;
        if (normalMove > 1f)
            normalMove = 1f;
        m_Am.SetFloat("RunBlend", Mathf.Abs(normalMove));

        speed = 10.0f;
    }
    void BowBasicValue()
    {
        normalMove = runInput.y;
        bowRightMove = runInput.x;

        m_Am.SetFloat("RunBlend", normalMove);
        m_Am.SetFloat("RightRunBlend", bowRightMove);
        
        speed = 4.0f;
    }
    void CantBowToRoll()
    {
        if(statetime<0.5f && rollState)
            PlayerInput.Instance.rollToBow = false;
        else
            PlayerInput.Instance.rollToBow = true;
    }
}
