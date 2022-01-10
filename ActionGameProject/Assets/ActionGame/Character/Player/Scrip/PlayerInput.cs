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
    public bool moveFlag = false;    //WASD移動旗標   
    public bool attack = false;
    public bool specialAttack = false;
    public bool avoid = false;
    private float statetime;

    private bool attackState;

    public Vector2 MoveInput
    {
        get
        {
            if (!moveFlag )
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
        AnimatorStateInfo stateinfo = m_Am.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateinfo = m_Am.GetNextAnimatorStateInfo(0);

        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));        

        float stateTime = m_Am.GetFloat("StateTime");
        //cantMoveState = GetCantMoveState();
        
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
        attackState = GetAttackState(stateinfo, nextStateinfo);
        statetime=GetStateTime();

        if (Input.GetButtonDown("Fire1") && statetime >= attackTime)
        {            
            attack = true;            
        }
        if (Input.GetButtonDown("Fire2"))
        {
            specialAttack = true;
        }        
        if (Input.GetButtonDown("Avoid") && statetime>= dodgeTime)
        {          
                avoid = true;            
        }
    }
    //void Rotating(float moveH, float moveV)
    //{
    //    建立角色目標方向的向量
    //   Vector3 newDirectionVector = followCamera.horizontalVector * moveV + (followCamera.cameraRight * moveH);
    //    Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
    //    transform.rotation = newRotation;
    //}
    /// <summary>
    /// 讓move給PlayerControl呼叫使用
    /// </summary>
    public Vector3 Move  
    {
        get { return move ; }
    }
    float GetStateTime()
    {
        if (attackState)
            statetime = m_Am.GetFloat("StateTime");
        else
            statetime = 1.0f;
        return statetime;
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
}
