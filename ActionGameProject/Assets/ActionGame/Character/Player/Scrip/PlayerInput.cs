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

    public float speed = 15;
    private Vector3 velocity;
    //Vector3 move = Vector3.zero;
    public float gravity = -9.81f;

    private bool isGrounded = true;
    public float groundDistance = 0.2f;
    Vector3 move = Vector3.zero;
    private Vector2 m_Movement;

    private Animator m_Am;
    float rollTime;

    public bool moveFlag = false;    //WASD���ʺX��   
    public bool attack = false;
    public bool specialAttack = false;
    public bool avoid = false;
    public bool cantMoveState= false;

    public Vector2 MoveInput
    {
        get
        {
            if (!moveFlag || cantMoveState)
                return Vector2.zero;
            return m_Movement;
        }
    }
    void Start()
    {
        m_Am = GetComponent<Animator>();     
    }

    // Update is called once per frame
    void Update()
    {
        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));        

        float stateTime = m_Am.GetFloat("StateTime");
        cantMoveState = GetCantMoveState();
        
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            moveFlag = true;
        }
        else
        {
            moveFlag = false;
        }

        //if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        //    && !cantMoveState)
        //{
        //    Rotating(h, v);
        //    move = transform.forward * Mathf.Abs(v);
        //    move += transform.forward * Mathf.Abs(h);
        //    move = Vector3.Normalize(move) * speed * Time.deltaTime;
        //}
        //else 
        //{           
        //    move = Vector3.zero;           
        //}

        if (Input.GetButtonDown("Fire1"))
        {            
            attack = true;            
        }
        if(Input.GetButtonDown("Fire2"))
        {
            specialAttack = true;
        }
        
        if (Input.GetButtonDown("Avoid") )
        {
            if (cantMoveState) rollTime = stateTime; else rollTime = 1.0f;

            if (rollTime>=0.5f)
            {
                avoid = true;
                //Rotating(h, v);
            }
        }
    }
    //void Rotating(float moveH, float moveV)
    //{
    //    �إߨ���ؼФ�V���V�q
    //   Vector3 newDirectionVector = followCamera.horizontalVector * moveV + (followCamera.cameraRight * moveH);
    //    Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
    //    transform.rotation = newRotation;
    //}
    /// <summary>
    /// ��move��PlayerControl�I�s�ϥ�
    /// </summary>
    public Vector3 Move  
    {
        get { return move ; }
    }
    /// <summary>
    /// �P�_�O�_�b�������A��
    /// </summary>
    /// <returns></returns>
    bool GetCantMoveState()
    {
        AnimatorStateInfo stateinfo = m_Am.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateinfo = m_Am.GetNextAnimatorStateInfo(0);

        bool nextAttackState= nextStateinfo.IsName("attack01");
        bool nextAvoidState = nextStateinfo.IsName("Roll");

        bool specialAttackState = stateinfo.IsName("specialAttack");
        bool avoid = stateinfo.IsName("Roll");
        bool attack01 = stateinfo.IsName("attack01");
        bool attack02 = stateinfo.IsName("attack02");
        bool attack03 = stateinfo.IsName("attack03");

        var attackstate = nextAvoidState || nextAttackState || avoid
                       || specialAttackState || attack01 || attack02 || attack03; 
        
        return attackstate;
    }
    
}
