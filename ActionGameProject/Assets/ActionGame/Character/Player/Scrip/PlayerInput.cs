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

    public bool moveFlag = false;    //WASD移動旗標   
    public bool attack = false;
  
    void Start()
    {
        m_Am = GetComponent<Animator>();       

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool a = GetAttackState();

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            moveFlag = true;
        }
        else
        {
            moveFlag = false;
        }

        if ((h!=0 || v!=0)
            && !a)
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
    }
    void Rotating(float moveH, float moveV)
    {
        // 建立角色目標方向的向量
        Vector3 newDirectionVector = followCamera.horizontalVector * moveV + (followCamera.cameraRight * moveH);
        Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
        transform.rotation = newRotation;
    }
    /// <summary>
    /// 讓move給PlayerControl呼叫使用
    /// </summary>
    public Vector3 Move  
    {
        get { return move ; }
    }
    /// <summary>
    /// 判斷是否在攻擊狀態中
    /// </summary>
    /// <returns></returns>
    bool GetAttackState()
    {
        bool a01 = m_Am.GetCurrentAnimatorStateInfo(0).IsName("attack01");
        bool a02 = m_Am.GetCurrentAnimatorStateInfo(0).IsName("attack02");
        var a = a01 || a02;
        return a;
    }
}
