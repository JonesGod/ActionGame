using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFSM : MonoBehaviour
{
    public enum FSMState//列出怪物所有的狀態
    {
        NONE = -1,
        Idle,
        Chase,
        Strafe,
        NormalAttack,
        Hurt,
        Dead
    }
    // public enum FSMState//列出怪物所有的狀態
    // {
    //     NONE = -1,
    //     Idle,
    //     Chase,
    //     Strafe,
    //     NormalAttack,
    //     Hurt,
    //     Dead
    // }
    private delegate void DoState();
    private DoState doState;
    private delegate void CheckAIState();
    private CheckAIState checkState;

    public BasicMonsterData data;
    public FSMState currentState;
    private GameObject currentEnemyTarget;
    private Animator animator;
    private float currentTime;
    Rigidbody myRigidbody;
    private CharacterController characterController;
    Rigidbody rigidbody;

    void Start()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState != FSMState.Dead)
        {
            checkState();
            doState();    
        }
    }
    private GameObject CheckEnemyInSight()
	{
		GameObject go = GameManager.Instance.GetPlayer();
		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;//計算和玩家之間的距離
		if(fDist < data.sightRange)//如果在警戒範圍內
		{
			return go;
		}
		return null;
	}
    private bool CheckEnemyInAttackRange(GameObject target, ref bool bAttack)
	{
		GameObject go = target;
		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;
		if (fDist < data.attackRange)
		{
			bAttack = true;
			return true;
		}
		return false;
	}

    void CheckIdleState()
    {   
        //CheckDead
        bool attack = false;
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        }        
        currentEnemyTarget = CheckEnemyInSight();
        if(currentEnemyTarget != null)
        {
            data.target = currentEnemyTarget;
            CheckEnemyInAttackRange(data.target, ref attack);
            if (attack)//在攻擊距離以內了:直接攻擊
            {
                currentState = FSMState.NormalAttack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else//在攻擊距離以外:進入追逐
            {
                currentState = FSMState.Chase;
                doState = DoChaseState;
                checkState = CheckChaseState;
            }
            return;
        }
    }
    void DoIdleState()
    {
        //Debug.Log("DoIdle");
    }
    void CheckChaseState()
    {
        //CheckDead
        bool attack = false;
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        }        
        CheckEnemyInAttackRange(data.target, ref attack);
        if (attack)//在攻擊距離以內了:攻擊
        {
            currentState = FSMState.NormalAttack;
            doState = DoAttackState;
            checkState = CheckAttackState;
        }        
    }    
    void DoChaseState()
    {
        //Debug.Log("DoChaseState");
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);

        data.speed = 6.0f;
        animator.SetBool("IsMoveRight", false);
        animator.SetBool("IsMoveForward", true);
        transform.LookAt(data.target.transform.position);
        //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
        rigidbody.velocity = transform.forward * data.speed;
        // if (SteeringBehavior.CollisionAvoid(data) == false)
        // {
        //     SteeringBehavior.Seek(data);
        // }

        // SteeringBehavior.Move(data);
    }
    void CheckStrafeState()
    {
        //CheckDead
        bool attack = false;
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        }
        if(currentTime >= data.strafeTime)
        {
            CheckEnemyInAttackRange(data.target, ref attack);
            if(attack == true)
            {
                currentState = FSMState.NormalAttack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else
            {
                currentState = FSMState.Chase;
                doState = DoChaseState;
                checkState = CheckChaseState;
            }
            return;
        }    
        return;
    }
    void DoStrafeState()
    {
        //Debug.Log("DoStrafe");
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);
        data.speed = 2.0f;

		Vector3 v = data.targetPosition - this.transform.position;
		float fDist = v.magnitude;
        if(fDist > data.strafeRange)
        {
            animator.SetBool("IsMoveRight", false); 
            animator.SetBool("IsMoveForward", true);        
            transform.LookAt(data.target.transform.position);
            //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
            rigidbody.velocity = transform.forward * data.speed;
            currentTime += Time.deltaTime;
            return;
        }
        animator.SetBool("IsMoveForward", false); 
        animator.SetBool("IsMoveRight", true);        
        transform.LookAt(data.target.transform.position);
        transform.Translate(Vector3.right * data.speed * Time.deltaTime);

        currentTime += Time.deltaTime;
    }
    void CheckAttackState()
    {
        //CheckDead
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        }        
        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            data.strafeTime = Random.Range(3.0f, 5.0f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }        
    }    
    void DoAttackState()
    {   
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            //Debug.Log("IsAttack");
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        animator.SetTrigger("AttackTrigger");
    }    
    void CheckHurtState()
    {
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        }        
        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            data.strafeTime = Random.Range(1.0f, 2.5f);;
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }

    }
    void DoHurtState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
        {
            //Debug.Log("IsHurt");
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
    }
    void CheckDeadState()
    {
        //Dead : Do nothing
    }
    void DoDeadState()
    {
        //Debug.Log("DoDead");
        animator.SetTrigger("Die");
    }
    public void CallHurt()
    {        
        Debug.Log("TakeDamage");
        data.hp -= 30;
        if(data.hp > 0)
        {
            currentState = FSMState.Hurt;  
            animator.SetTrigger("TakeDamage"); 
            doState = DoHurtState;
            checkState = CheckHurtState;
        }                
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, data.sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, data.attackRange);    
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, data.strafeRange);    
    }
}