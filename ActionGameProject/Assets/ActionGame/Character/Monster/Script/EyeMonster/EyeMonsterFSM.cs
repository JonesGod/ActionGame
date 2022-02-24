using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeMonsterFSM : FSMBase
{
    private delegate void DoState();
    private DoState doState;
    private delegate void CheckAIState();
    private CheckAIState checkState;

    private GameObject currentEnemyTarget;
    private Animator animator;
    private float currentTime;
    public int strafeDirection;
    
    //Rigidbody myRigidbody;
    public BoxCollider CharacterCollisionBlocker; 

    public GameObject[] wayPoints;
    public GameObject targetWayPoint;
    private Vector3 startPosition;
    private MonsterDeadDissolve monsterHurt;
    void Start()
    {
        startPosition = this.transform.position;  
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        strafeDirection = 0;
        data.patrolTime = Random.Range(3.0f, 5.0f);
        targetWayPoint = wayPoints[Random.Range(0, wayPoints.Length)];
        monsterHurt = this.GetComponent<MonsterDeadDissolve>();
    }

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

    public override void CheckIdleState()
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
                currentState = FSMState.Attack;
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
        if(currentTime >= data.patrolTime)
        {
            currentState = FSMState.Patrol;
            GameObject newWayPoint = wayPoints[Random.Range(0, wayPoints.Length)];
            if(targetWayPoint.name == newWayPoint.name)
            {
                return;
            }
            targetWayPoint = newWayPoint;
            doState = DoMoveToState;
            checkState = CheckMoveToState;
            return;
        }
    }
    public override void DoIdleState()
    {
        //Debug.Log("DoIdle");
        animator.SetBool("IsIdle", true);
        animator.SetBool("Chase", false);
        animator.SetBool("Fly", false);
        currentTime += Time.deltaTime;
    }
    public void CheckMoveToState()
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
                currentState = FSMState.Attack;
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
        Vector3 v = targetWayPoint.transform.position - this.transform.position;
		float fDist = v.magnitude;     
        if(fDist <= 5)
        {
            data.speed = 0.0f;
            myRigidbody.velocity = transform.forward * data.speed;
            currentState = FSMState.Idle;
            data.patrolTime = Random.Range(3.0f, 5.0f);
            currentTime = 0.0f;
            doState = DoIdleState;
            checkState = CheckIdleState;            
            return;
        }  
    }
    public void DoMoveToState()
    {        
            data.speed = 4.0f;
            animator.SetBool("IsIdle", false);
            animator.SetBool("Chase", false);
            animator.SetBool("Fly", true);
            var targetRotation = Quaternion.LookRotation(targetWayPoint.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
            var v3 = transform.forward * data.speed;
            v3.y = myRigidbody.velocity.y;
            //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
            myRigidbody.velocity = v3;
            return;        
    }
    public override void CheckChaseState()
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
            currentState = FSMState.Attack;
            doState = DoAttackState;
            checkState = CheckAttackState;
        }        
    }    
    public override void DoChaseState()
    {
        //Debug.Log("DoChaseState");
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);

        data.speed = 6.0f;
        animator.SetBool("Fly", false);
        animator.SetBool("Chase", true);
        var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
        //transform.LookAt(data.target.transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
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
                currentState = FSMState.Attack;
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
    public override void DoStrafeState()
    {
        //Debug.Log("DoStrafe");
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);
        data.speed = 2.0f;

        var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);

		Vector3 v = data.targetPosition - this.transform.position;
		float fDist = v.magnitude;        
        if(fDist > data.strafeRange)
        {
            animator.SetBool("Fly", false); 
            animator.SetBool("Chase", true);        
            //transform.LookAt(data.target.transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
            //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
            myRigidbody.velocity = transform.forward * data.speed;
            currentTime += Time.deltaTime;
            return;
        }
        animator.SetBool("Chase", false); 
        animator.SetBool("Fly", true); 
        //transform.LookAt(data.target.transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
        if(strafeDirection == 0)
        {
            transform.Translate(Vector3.right * data.speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * data.speed * Time.deltaTime);
        }      

        currentTime += Time.deltaTime;
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
            data.strafeTime = Random.Range(2.0f, 4.0f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            strafeDirection = Random.Range(0, 2);
            Debug.Log(strafeDirection);
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }        
    }    
    public override void DoAttackState()
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
        myRigidbody.velocity = Vector3.zero;
        animator.SetTrigger("AttackTrigger");
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
            strafeDirection = Random.Range(0, 2);
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
    public override void CallHurt(float damageAmount, bool isHead, bool isHurtAnimation)
    {        
        data.hp -= damageAmount;
        myHealth.ModifyHealth(damageAmount);  
        monsterHurt.HitFlash();
        if(data.hp > 0 && isHurtAnimation == true)
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

    public override void PlayerIsDead()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        animator.SetBool("Fly", false);
        animator.SetBool("Chase", false);
        animator.SetBool("IsIdle", true);
        checkState = DoIdleState;
        doState = DoIdleState;
        return;
    }
    public override void PlayerIsReLife()
    {
        this.transform.position = startPosition;  
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        animator.SetBool("Fly", false);
        animator.SetBool("Chase", false);
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
