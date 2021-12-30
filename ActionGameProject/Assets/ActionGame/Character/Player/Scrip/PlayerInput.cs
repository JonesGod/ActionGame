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

    public float speed = 4;
    private Vector3 velocity;
    //Vector3 move = Vector3.zero;
    public float gravity = -9.81f;

    private bool isGrounded = true;
    public float groundDistance = 0.2f;
    Vector3 move = Vector3.zero;

    private Animator m_Am;

    private bool attackFlag = false;  //�����X��
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
         
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
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
        get { return move; }
    }
    
}
