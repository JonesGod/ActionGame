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


    public float groundDistance = 0.2f;
    Vector3 move = Vector3.zero;
    private Vector2 m_Movement;
    private Vector2 m_Mouse;

    private Animator m_Am;
    AnimatorStateInfo stateinfo;
    AnimatorStateInfo nextStateinfo;

    [HideInInspector] public PlayerMode playerState;
    [HideInInspector] public bool moveFlag = false;    //WASD²¾°ÊºX¼Ð   
    [HideInInspector] public bool attack = false;
    [HideInInspector] public bool specialAttack = false;
    [HideInInspector] public bool avoid = false;
    [HideInInspector] public bool isTrasition = false;
    [HideInInspector] public bool bowState = false;
    [HideInInspector] public bool cantBowState;
    [HideInInspector] public bool attackState;
    [HideInInspector] public bool rollState;
    [HideInInspector] public bool rollIsNext;

    public enum PlayerMode
    {
        normal = 1,
        Bow = 2
    }

    public Vector2 MoveInput
    {
        get
        {
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

        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_Mouse.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        
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

        CantBow();
        if (Input.GetButtonDown("Switch") && !bowState && !FolowCamera.Instance.isSwitch && cantBowState)
        {
            bowState = true;
        }
        else if(Input.GetButtonDown("Switch") && bowState && !FolowCamera.Instance.isSwitch)
        {
            bowState = false;
        }

    }   
    public Vector3 Move  
    {
        get { return move ; }
    }
    void CantBow()
    {
        cantBowState=!attackState && !rollState && !rollIsNext;
    }

}
