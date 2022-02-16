using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossFSM : FSMBase
{
    private delegate void DoState();
    private DoState doState;
    private delegate void CheckAIState();
    private CheckAIState checkState;
    
    private GameObject currentEnemyTarget;
    private Animator animator;
    private float currentTime;
    public int strafeDirection;

    Rigidbody myRigidbody;
    public BoxCollider CharacterCollisionBlocker; 
    private float maxHp;
    public bool isAngry = false;

    void Start()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        maxHp = data.hp;
    }

    void Update()
    {
        if(currentState != FSMState.Dead)
        {
            checkState();
            doState();    
        }        
        //myRigidbody.velocity = (GameManager.Instance.GetPlayer().transform.position - this.transform.position) * 1.0f;
    }

    public void StartBattle()
    {
        currentEnemyTarget = GameManager.Instance.GetPlayer();
        animator.SetTrigger("Scream");
    }
    
    private GameObject CheckEnemyInBossArea()
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
    private bool CheckEnemyInAttackRange(GameObject target, ref bool normalAttack, ref bool chargeAttack, ref bool angryAttack)
	{
		GameObject go = target;
		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;
        if(fDist < data.attackRange && isAngry == true)
        {
            chargeAttack = false;
			normalAttack = false;
            angryAttack = true;
			return true;
        }
		else if (fDist < data.attackRange)
		{
            chargeAttack = false;
			normalAttack = true;
            angryAttack = false;
			return true;
		}
        else if(fDist > 2 * data.attackRange)
        {
            normalAttack = false;
            chargeAttack = true;
            angryAttack = false;
			return true;
        }
		return false;
	}    

    public override void CheckIdleState()
    {   
        //CheckDead
        bool normalAttack = false;
        bool chargeAttack = false;
        bool angryAttack = false;
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        }        
        currentEnemyTarget = CheckEnemyInBossArea();
        if(currentEnemyTarget != null)
        {
            data.target = currentEnemyTarget;
            CheckEnemyInAttackRange(data.target, ref normalAttack, ref chargeAttack, ref angryAttack);
            if(normalAttack)//在普通攻擊距離以內:直接攻擊
            {
                currentState = FSMState.Attack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(chargeAttack)//在普通攻擊距離以外:進入追逐
            {
                currentState = FSMState.Attack;
                doState = DoChargeAttackState;
                checkState = CheckChargeAttackState;
            }
            else if(angryAttack)
            {
                currentState = FSMState.Attack;
                doState = DoAngryAttackState;
                checkState = CheckAngryAttackState;
            }
            return;
        }
    }
    public override void DoIdleState()
    {
    }
    public override void CheckChaseState()
    {
        //CheckDead
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        }        
        bool normalAttack = false;
        bool chargeAttack = false;
        bool angryAttack = false;
        CheckEnemyInAttackRange(data.target, ref normalAttack, ref chargeAttack, ref angryAttack);
        if (normalAttack)
        {
            currentState = FSMState.Attack;
            doState = DoAttackState;
            checkState = CheckAttackState;
        }
        else if(chargeAttack)
        {
            currentState = FSMState.Attack;
            doState = DoChargeAttackState;
            checkState = CheckChargeAttackState;
        }
        else if(angryAttack)
        {
            currentState = FSMState.Attack;
            doState = DoAngryAttackState;
            checkState = CheckAngryAttackState;
        }        
    }    
    public override void DoChaseState()
    {
        //Debug.Log("DoChaseState");
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);

        data.speed = 15.0f;
        animator.SetBool("IsWalkForward", false);
        animator.SetBool("IsRunForward", true);
        transform.LookAt(data.target.transform.position);
        //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
        myRigidbody.velocity = transform.forward * data.speed;
        // if (SteeringBehavior.CollisionAvoid(data) == false)
        // {
        //     SteeringBehavior.Seek(data);
        // }

        // SteeringBehavior.Move(data);
    }
    public override void CheckStrafeState()
    {
        //CheckDead
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        }
        bool normalAttack = false;
        bool chargeAttack = false;
        bool angryAttack = false;
        if(currentTime >= data.strafeTime)
        {
            CheckEnemyInAttackRange(data.target, ref normalAttack, ref chargeAttack, ref angryAttack);
            if(normalAttack)//在攻擊距離以內了:直接攻擊
            {
                currentState = FSMState.Attack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(chargeAttack)
            {
                currentState = FSMState.Attack;
                doState = DoChargeAttackState;
                checkState = CheckChargeAttackState;
            }
            else if(angryAttack)
            {
                currentState = FSMState.Attack;
                doState = DoAngryAttackState;
                checkState = CheckAngryAttackState;
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
    public override void DoStrafeState()
    {
        //Debug.Log("DoStrafe");
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);
        data.speed = 3.0f;

		Vector3 v = data.targetPosition - this.transform.position;
		float fDist = v.magnitude;
        if(fDist > data.attackRange)
        {
            animator.SetBool("IsRunForward", false); 
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsWalkForward", true);        
            //var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
            transform.LookAt(data.target.transform.position);
            //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.1f);
            //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
            myRigidbody.velocity = transform.forward * data.speed;
            currentTime += Time.deltaTime;
            return;
        }
        else
        {             
            animator.SetBool("IsWalkForward", false); 
            animator.SetBool("IsRunForward", false);
            animator.SetBool("IsIdle", true);     
            data.speed = 0;
            myRigidbody.velocity = transform.forward * data.speed;
            transform.LookAt(data.target.transform.position);
            currentTime += Time.deltaTime;
            return;
        }
    }
    public override void CheckAttackState()
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
            data.strafeTime = Random.Range(1.5f, 3.0f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }        
    }    
    public override void DoAttackState()
    {   
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ClawAttack"))
        {
            //Debug.Log("IsAttack");
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        myRigidbody.velocity = Vector3.zero;
        animator.SetTrigger("NormalAttack");
    }    
    public override void CheckHurtState()
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
    public override void DoHurtState()
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
    public override void CallHurt(float damageAmount, bool isHead)
    {        
        Debug.Log("TakeDamage");
        data.hp -= damageAmount;
        if(data.hp < maxHp / 2)
        {
            isAngry = true;
        }
        if(data.hp > 0 && isHead == true)
        {
            currentState = FSMState.Hurt;  
            animator.SetTrigger("TakeDamage"); 
            if(data.target == null)
            {
                data.target = GameManager.Instance.GetPlayer();
            }
            doState = DoHurtState;
            checkState = CheckHurtState;
        }                
    }
    public override void CheckDeadState()
    {
        //Dead : Do nothing
    }
    public override void DoDeadState()
    {
        //Debug.Log("DoDead");
        animator.SetTrigger("Die");
        myRigidbody.isKinematic = true;
        CharacterCollisionBlocker.enabled = false;
    }
    private void CheckChargeAttackState()
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
            data.strafeTime = Random.Range(2.0f, 3.5f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }        
    }

    private void DoChargeAttackState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Charge Attack"))
        {
            //Debug.Log("IsAttack");
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        Vector3 v = data.target.transform.position - this.transform.position;
		float fDist = v.magnitude;
        if(fDist > data.attackRange)
        {
            data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);

            data.speed = 15.0f;
            animator.SetBool("IsWalkForward", false);
            animator.SetBool("IsRunForward", true);
            transform.LookAt(data.target.transform.position);
            //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
            myRigidbody.velocity = transform.forward * data.speed;
        }
        else
        {
            myRigidbody.velocity = Vector3.zero;
            animator.SetTrigger("ChargeAttack");
        }        
    }
    private void CheckAngryAttackState()
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
            data.strafeTime = Random.Range(2.0f, 3.5f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }        
    }

    private void DoAngryAttackState()
    {
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Angry Claw Attack"))
        {
            //Debug.Log("IsAttack");
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Horn Attack"))
        {
            //Debug.Log("IsAttack");
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Angry Charge Attack"))
        {
            //Debug.Log("IsAttack");
            return;
        }
        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        myRigidbody.velocity = Vector3.zero;
        animator.SetTrigger("AngryAttack");
    }
    public override void PlayerIsDead()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        animator.SetBool("IsWalkForward", false);
        animator.SetBool("IsRunForward", false);
        animator.SetBool("IsIdle", true);
        checkState = DoIdleState;
        doState = DoIdleState;
        return;
    }
    public override void PlayerIsReLife()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        animator.SetBool("IsWalkForward", false);
        animator.SetBool("IsRunForward", false);
        animator.SetBool("IsIdle", true);
        checkState = CheckIdleState;
        doState = DoIdleState;
        return;
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
