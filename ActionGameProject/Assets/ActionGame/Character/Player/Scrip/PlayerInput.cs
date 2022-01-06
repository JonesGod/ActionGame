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

    private Animator m_Am;
    

    public bool moveFlag = false;    //WASD���ʺX��   
    public bool attack = false;
    public bool specialAttack = false;
    public bool avoid = false;


    void Start()
    {
        m_Am = GetComponent<Animator>();       

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool noRunState = GetState();

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            moveFlag = true;
        }
        else
        {
            moveFlag = false;
        }

        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            && !noRunState)
        {            
            Rotating(h, v);
            move = transform.forward * Mathf.Abs(v);
            move += transform.forward * Mathf.Abs(h);
            move = Vector3.Normalize(move) * speed * Time.deltaTime;           
        }
        else 
        {           
            move = Vector3.zero;           
        }

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
                avoid = true;                
                Rotating(h, v);
        }
    }
    void Rotating(float moveH, float moveV)
    {
        // �إߨ���ؼФ�V���V�q
        Vector3 newDirectionVector = followCamera.horizontalVector * moveV + (followCamera.cameraRight * moveH);
        Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
        transform.rotation = newRotation;
    }
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
    bool GetState()
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
