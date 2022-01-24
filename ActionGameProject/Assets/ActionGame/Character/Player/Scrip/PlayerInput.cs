using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance
    {
        get { return s_Instance; }
    }

    protected static PlayerInput s_Instance;

    public FolowCamera followCamera;

    private Vector3 velocity;

    public float groundDistance = 0.2f;
    Vector3 move = Vector3.zero;
    private Vector2 m_Movement;
    private Vector2 m_Mouse;

    private Animator m_Am;
    AnimatorStateInfo stateinfo;
    AnimatorStateInfo nextStateinfo;

    private float dodgeTime = 0.5f; //攻擊時，最小可迴避時間
    private float attackTime = 0.4f; //攻擊時，最小可再攻擊時間
    private bool isBow=false;

    [HideInInspector] public PlayerMode playerState;
    [HideInInspector] public bool moveFlag = false;    //WASD移動旗標   
    [HideInInspector] public bool attack = false;
    [HideInInspector] public bool specialAttack = false;
    [HideInInspector] public bool avoid = false;
    [HideInInspector] public bool nextIsRoll = false;
    [HideInInspector] public bool isTrasition = false;
    [HideInInspector] public bool bowState = false;
    [HideInInspector] public bool attackState;

    readonly int hashAttack01 = Animator.StringToHash("attack01");
    readonly int hashAttack02 = Animator.StringToHash("attack02");
    readonly int hashAttack03 = Animator.StringToHash("attack03");
    readonly int hashAttack04 = Animator.StringToHash("attack04");
    readonly int hashSpecialAttackState = Animator.StringToHash("specialAttackState");

    public enum PlayerMode
    {
        normal = 1,
        Bow = 2
    }

    public Vector2 MoveInput
    {
        get
        {
            if (!moveFlag )
            {
                return Vector2.zero;
            }
            return m_Movement;
        }
    }
    public Vector2 MouseInput
    {
        get
        { 
            return m_Mouse;
        }
    }
    private void Awake()
    {
        s_Instance = this;
    }
    void Start()
    {
        m_Am = GetComponent<Animator>();
       
    }
    void Update()
    {
        stateinfo = m_Am.GetCurrentAnimatorStateInfo(0);
        nextStateinfo = m_Am.GetNextAnimatorStateInfo(0);
        GetAttackState();
        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_Mouse.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        float stateTime = m_Am.GetFloat("StateTime");
        
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))        
            moveFlag = true;        
        else        
            moveFlag = false;
        
        if (Input.GetButtonDown("Fire1"))                    
            attack = true;                    
        if (Input.GetButtonDown("Fire2"))        
            specialAttack = true;                
        if (Input.GetButtonDown("Avoid") )                  
            avoid = true;

        if (Input.GetButtonDown("Switch") && !isBow)
        {
            isBow = true;
            bowState = true;
        }
        else if(Input.GetButtonDown("Switch") && isBow)
        {
            isBow = false;
            bowState = false;
        }

    }   
    public Vector3 Move  
    {
        get { return move ; }
    }
    void GetAttackState()
    {
        if (stateinfo.shortNameHash == hashAttack01 ||
           stateinfo.shortNameHash == hashAttack02 ||
           stateinfo.shortNameHash == hashAttack03 ||
           stateinfo.shortNameHash == hashAttack04 ||
           stateinfo.shortNameHash == hashSpecialAttackState ||
           nextStateinfo.shortNameHash == hashAttack01)
        {
            attackState = true;
        }
        else
            attackState = false;
    }
}
