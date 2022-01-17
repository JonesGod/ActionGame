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

    private Animator m_Am;

    private float dodgeTime = 0.5f; //攻擊時，最小可迴避時間
    private float attackTime = 0.4f; //攻擊時，最小可再攻擊時間

    [HideInInspector] public bool moveFlag = false;    //WASD移動旗標   
    [HideInInspector] public bool attack = false;
    [HideInInspector] public bool specialAttack = false;
    [HideInInspector] public bool avoid = false;
    [HideInInspector] public bool nextIsRoll = false;
    [HideInInspector] public bool isTrasition = false;

    private float statetime;

    private bool attackState;

    public Vector2 MoveInput
    {
        get
        {
            if (!moveFlag || nextIsRoll)
                return Vector2.zero;
            return m_Movement;
        }
    }
    void Start()
    {
        m_Am = GetComponent<Animator>();     
    }
    void Update()
    {                      
        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

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
        
    }   
    public Vector3 Move  
    {
        get { return move ; }
    }    
}
