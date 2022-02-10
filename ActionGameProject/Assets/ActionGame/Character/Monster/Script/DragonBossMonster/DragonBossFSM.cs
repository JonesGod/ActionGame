using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossFSM : FSMBase
{
    private delegate void DoState();
    private DoState doState;
    private delegate void CheckAIState();
    private CheckAIState checkState;

    public BasicMonsterData data;
    private GameObject currentEnemyTarget;
    private Animator animator;
    private float currentTime;
    public int strafeDirection;

    Rigidbody myRigidbody;
    public BoxCollider CharacterCollisionBlocker; 

    void Start()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        strafeDirection = 0;                
    }

    void Update()
    {
        if(currentState != FSMState.Dead)
        {
            checkState();
            doState();    
        }        
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
    private bool CheckEnemyInAttackRange(GameObject target, ref bool normalAttack, ref bool chargeAttack)
	{
		GameObject go = target;
		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;
		if (fDist < data.attackRange)
		{
            chargeAttack = false;
			normalAttack = true;
			return true;
		}
        else if(fDist > 2 * data.attackRange)
        {
            normalAttack = false;
            chargeAttack = true;
			return true;
        }
		return false;
	}    

    public override void CheckIdleState()
    {   
        //CheckDead
        bool normalAttack = false;
        bool chargeAttack = false;
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
            CheckEnemyInAttackRange(data.target, ref normalAttack, ref chargeAttack);
            if (normalAttack)//在攻擊距離以內了:直接攻擊
            {
                currentState = FSMState.NormalAttack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(chargeAttack)
            {
                currentState = FSMState.NormalAttack;
                doState = DoChargeAttackState;
                checkState = CheckChargeAttackState;
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
    public override void DoIdleState()
    {
        //Debug.Log("DoIdle");
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
        CheckEnemyInAttackRange(data.target, ref normalAttack, ref chargeAttack);
        if (normalAttack)//在攻擊距離以內了:直接攻擊
        {
            currentState = FSMState.NormalAttack;
            doState = DoAttackState;
            checkState = CheckAttackState;
        }
        else if(chargeAttack)
        {
            currentState = FSMState.NormalAttack;
            doState = DoChargeAttackState;
            checkState = CheckChargeAttackState;
        }        
    }    
    public override void DoChaseState()
    {
        //Debug.Log("DoChaseState");
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);

        data.speed = 6.0f;
        animator.SetBool("IsMoveRight", false);
        animator.SetBool("IsMoveForward", true);
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
        if(currentTime >= data.strafeTime)
        {
            CheckEnemyInAttackRange(data.target, ref normalAttack, ref chargeAttack);
            if (normalAttack)//在攻擊距離以內了:直接攻擊
            {
                currentState = FSMState.NormalAttack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(chargeAttack)
            {
                currentState = FSMState.NormalAttack;
                doState = DoChargeAttackState;
                checkState = CheckChargeAttackState;
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

		Vector3 v = data.targetPosition - this.transform.position;
		float fDist = v.magnitude;
        if(fDist > data.strafeRange)
        {
            animator.SetBool("IsMoveRight", false); 
            animator.SetBool("IsMoveForward", true);        
            transform.LookAt(data.target.transform.position);
            //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
            myRigidbody.velocity = transform.forward * data.speed;
            currentTime += Time.deltaTime;
            return;
        }
        animator.SetBool("IsMoveForward", false); 
        animator.SetBool("IsMoveRight", true); 
        transform.LookAt(data.target.transform.position);
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
            data.strafeTime = Random.Range(3.0f, 5.0f);
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
    private void CheckChargeAttackState()
    {

    }

    private void DoChargeAttackState()
    {

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
