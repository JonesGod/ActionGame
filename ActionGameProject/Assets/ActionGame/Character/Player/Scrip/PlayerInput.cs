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


    private Vector2 m_Movement;//存取WASD輸入
    private Vector2 m_Mouse;//存取滑鼠滑動

    [HideInInspector] public bool moveFlag = false;
    [HideInInspector] public bool attack = false;
    [HideInInspector] public bool specialAttack = false;
    [HideInInspector] public bool avoid = false;
    [HideInInspector] public bool isTrasition = false;
    [HideInInspector] public bool bowState = false;
    [HideInInspector] public bool cantBowState;
    [HideInInspector] public bool attackState;
    [HideInInspector] public bool rollState;
    [HideInInspector] public bool rollIsNext;
    [HideInInspector] public bool rollToBow=false;
    [HideInInspector] public bool bowShoot;

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
        
    }
    void Update()
    {
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
  
        if (Input.GetButtonDown("Avoid"))                  
            avoid = true;

        CantBow();
        if (Input.GetButtonDown("Switch") && !bowState && !FolowCamera.Instance.isSwitch && cantBowState
            && rollToBow)
        {
            bowState = true;
        }
        else if(Input.GetButtonDown("Switch") && bowState && !FolowCamera.Instance.isSwitch)
        {
            bowState = false;
        }

    }
    /// <summary>
    /// 不能切到弓的狀態
    /// 從PlayerControl的Scrip來獲得attackState和rollIsNext
    /// </summary>
    void CantBow()
    {
        cantBowState=!attackState &&!rollIsNext;
    }
}
