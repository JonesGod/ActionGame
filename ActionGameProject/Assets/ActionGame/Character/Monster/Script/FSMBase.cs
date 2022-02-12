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
    Angry
}
public class FSMBase : MonoBehaviour
{
    public FSMState currentState;
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
    public virtual void CheckDeadState()
    {
       
    }
    public virtual void DoDeadState()
    {
        
    }
}
