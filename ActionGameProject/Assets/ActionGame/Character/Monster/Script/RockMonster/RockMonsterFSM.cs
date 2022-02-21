using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterFSM : FSMBase
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
    private float maxShield;
    public bool isAngry = false;
    private bool isRotateTowardPlayer = false;

    void Start()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        maxHp = data.hp;
        maxShield = data.shield;
    }

    void Update()
    {
        var targetRotation = Quaternion.LookRotation(GameManager.Instance.GetPlayer().transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);
        myRigidbody.velocity = transform.forward * 10.0f;
        Vector3 targetDir = GameManager.Instance.GetPlayer().transform.position - transform.position;            
        float dotDirection = Vector3.Dot(transform.forward, targetDir.normalized);
        Debug.Log(dotDirection);
        if(data.hp <= maxHp / 2 && isAngry == false)
        {
            isAngry = true;
            return;
        }
        // if(currentState != FSMState.Dead)
        // {
        //     checkState();
        //     doState();    
        // }      
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
    private bool CheckEnemyInAttackRange(GameObject target, ref bool punchAttack, ref bool rangeAttack, ref bool circleAttack, ref bool smashAttack)
	{        
		GameObject go = target;

        Vector3 targetDir = go.transform.position - transform.position;            
        float dotDirection = Vector3.Dot(transform.forward, targetDir.normalized);

		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;
        if(fDist < data.attackRange && isAngry == true)//敲地板
        {
            punchAttack = false;
			circleAttack = false;
            smashAttack = true;
            rangeAttack = false;
			return true;
        }
		else if (fDist < data.attackRange)//普通攻擊
		{
            punchAttack = true;
			circleAttack = false;
            smashAttack = false;
            rangeAttack = false;
			return true;
		}
        else if(fDist > data.strafeRange)//轉圈
        {
            punchAttack = false;
			circleAttack = true;
            smashAttack = false;
            rangeAttack = false;
			return true;
        }
        else if(fDist > data.strafeRange)//遠程攻擊
        {
            punchAttack = false;
			circleAttack = false;
            smashAttack = false;
            rangeAttack = true;
            return true;
        }
		return false;
	}    
    public override void CheckIdleState()
    {   
        //CheckDead
        bool punchAttack = false;
        bool rangeAttack = false;
        bool circleAttack = false;
        bool smashAttack = false;
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
            CheckEnemyInAttackRange(data.target, ref punchAttack, ref rangeAttack, ref circleAttack, ref smashAttack);
            if(punchAttack)
            {
                currentState = FSMState.Attack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(circleAttack)
            {
                currentState = FSMState.Attack;
                doState = DoCircleAttackState;
                checkState = CheckCircleAttackState;
            }
            else if(rangeAttack)
            {
                currentState = FSMState.Attack;
                doState = DoRangeAttackState;
                checkState = CheckRangeAttackState;
            }  
            else if(smashAttack)
            {
                currentState = FSMState.Attack;
                doState = DoSmashAttackState;
                checkState = CheckSmashAttackState;
            }                      
            return;
        }
    }
    public override void DoIdleState()
    {

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
            data.strafeTime = Random.Range(1.0f, 2.0f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }        
    }    
    public override void DoAttackState()
    {
        data.speed = 0;
        myRigidbody.velocity = transform.forward * data.speed;
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PunchAttack"))
        {            
            myRigidbody.velocity = Vector3.zero;
            //Debug.Log("IsAttack");
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        animator.SetTrigger("PunchAttack");
    }  

    public void CheckCircleAttackState()
    {
    }    
    public void DoCircleAttackState()
    {
        data.speed = 0;
        myRigidbody.velocity = transform.forward * data.speed;
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("CircleAttack"))
        {            
            myRigidbody.velocity = Vector3.zero;
            //Debug.Log("IsAttack");
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        animator.SetTrigger("CircleAttack");
    }

    public void CheckSmashAttackState()
    {
    }    
    public void DoSmashAttackState()
    {
        data.speed = 0;
        myRigidbody.velocity = transform.forward * data.speed;
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("SmashAttack"))
        {            
            myRigidbody.velocity = Vector3.zero;
            //Debug.Log("IsAttack");
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        animator.SetTrigger("SmashAttack");
    }

    public void CheckRangeAttackState()
    {
    }    
    public void DoRangeAttackState()
    {
        data.speed = 0;
        myRigidbody.velocity = transform.forward * data.speed;
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("RangeAttack"))
        {            
            myRigidbody.velocity = Vector3.zero;
            //Debug.Log("IsAttack");
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        animator.SetTrigger("RangeAttack");
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
        bool punchAttack = false;
        bool rangeAttack = false;
        bool circleAttack = false;
        bool smashAttack = false;
        if(currentTime >= data.strafeTime)
        {
            if(punchAttack)
            {
                currentState = FSMState.Attack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(circleAttack)
            {
                currentState = FSMState.Attack;
                doState = DoCircleAttackState;
                checkState = CheckCircleAttackState;
            }
            else if(rangeAttack)
            {
                currentState = FSMState.Attack;
                doState = DoRangeAttackState;
                checkState = CheckRangeAttackState;
            }  
            else if(smashAttack)
            {
                currentState = FSMState.Attack;
                doState = DoSmashAttackState;
                checkState = CheckSmashAttackState;
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
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsWalk", true);        
            var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);
            myRigidbody.velocity = transform.forward * data.speed;
            currentTime += Time.deltaTime;
            return;
        }
        else
        {
            animator.SetBool("IsWalk", false);
            animator.SetBool("IsIdle", true);     
            data.speed = 0;
            myRigidbody.velocity = transform.forward * data.speed;
            var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);
            currentTime += Time.deltaTime;
            return;
        }
    }
        
}
