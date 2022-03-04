using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FSMState//列出怪物所有的狀態
{
    NONE = -1,
    Idle,
    Chase,
    Strafe,
    Attack,
    Hurt,
    Dead,
    Patrol,
    ReallyDead
}
public class FSMBase : MonoBehaviour
{
    public FSMState currentState;
    public BasicMonsterData data;
    public Health myHealth;
    public Rigidbody myRigidbody;
    /// <summary>
    /// obstacle
    /// </summary>
    public float rotationSpeed;
    public bool isThereObstacle = false;
    public LayerMask obstacles;
    public int range;
    public int widthDistance;
    public int backDistance;
    
    public virtual void CheckIdleState()
    {   
        
    }
    public virtual void DoIdleState()
    {

    }
    public virtual void CheckChaseState()
    {
        
    }    
    public virtual void DoChaseState()
    {
        
    }
    public virtual void CheckStrafeState()
    {
        
    }
    public virtual void DoStrafeState()
    {
        
    }
    public virtual void CheckAttackState()
    {
        
    }    
    public virtual void DoAttackState()
    {   
        
    }    
    public virtual void CheckHurtState()
    {

    }
    public virtual void DoHurtState()
    {
        
    }
    public virtual void CallHurt(float damageAmount, bool isHead, bool isHurtAnimation)
    {
        
    }
    public virtual void DoDeadState()
    {
        
    }
    public virtual void CheckDeadState()
    {
       
    }
    public virtual void HelpPartner()
    {
        
    }
    public virtual void PlayerIsDead()
    {
        Debug.Log("PlayerIsDead");
    }
    public virtual void PlayerIsReLife()
    {
        Debug.Log("PlayerIsRelife");
    }
    

    public void AvoidCollisionMove()
    {
        if (!isThereObstacle) 
        {
            Vector3 relativePos = data.target.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
            //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
        }
        myRigidbody.velocity = transform.forward * data.speed;

        Vector3 detectPosition = transform.position + (transform.up * 1.5f);
        Transform rightRay = transform;

        RaycastHit hit;
        if (Physics.Raycast(detectPosition + (transform.right * widthDistance)+ (transform.right * widthDistance), transform.forward, out hit, range, obstacles) || Physics.Raycast(detectPosition - (transform.right * widthDistance), transform.forward, out hit, range, obstacles))
        {
            Debug.Log("hit");
            isThereObstacle = true;
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);                 
        }        
        // Now Two More RayCast At The End of Object to detect that object has already pass the obsatacle.
        // Just making this boolean variable false it means there is nothing in front of object.
        if (Physics.Raycast(detectPosition - (transform.forward * backDistance), transform.right, out hit, range, obstacles) ||Physics.Raycast(detectPosition - (transform.forward * backDistance), -transform.right, out hit, range, obstacles)) 
        {
            isThereObstacle = false;                       
        }
        
    }
}
