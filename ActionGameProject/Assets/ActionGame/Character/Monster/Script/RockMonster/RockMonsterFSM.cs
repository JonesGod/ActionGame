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
    private float waitTime;
    private float smashCD;

    //Rigidbody myRigidbody;
    public BoxCollider CharacterCollisionBlocker; 
    private float maxHp;
    public Shield myShield;
    private float maxShield;
    public bool isAngry = false;
    public bool isAwake = false;
    private bool isRotateTowardPlayer = false;

    public BoxCollider coreHitBox;
    public GameObject shieldHitBox;
    WorldEvManager worldEvManager;
    private bool canRestoreShield = true;
    public ShieldHurtBlink shieldHurt;

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
        worldEvManager = FindObjectOfType<WorldEvManager>();
    }

    void Update()
    {
        smashCD += Time.deltaTime;
        if(data.hp <= maxHp / 2 && isAngry == false)
        {
            isAngry = true;
            return;
        }
        if(currentState != FSMState.Dead && isAwake == true)
        {
            checkState();
            doState();    
        }      
    }
    IEnumerator PlayStartAnimation()
    {
        yield return new WaitForSeconds(1.0f);
        animator.SetTrigger("BattleStart");
        yield return new WaitForSeconds(5.0f);
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        currentEnemyTarget = GameManager.Instance.GetPlayer();
        isAwake = true;
        //shieldHitBox.SetActive(true);
        shieldHurt.StartRecoverShield();
    }
    public void StartBattle()
    {
        StartCoroutine(PlayStartAnimation());
        // currentEnemyTarget = null;
        // currentState = FSMState.Idle;
        // doState = DoIdleState;
        // checkState = CheckIdleState;
        // currentEnemyTarget = GameManager.Instance.GetPlayer();
        // isAwake = true;
        //animator.SetTrigger("BattleStart");
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
    private bool CheckEnemyInAttackRange(GameObject target, ref bool punchAttack, ref bool rangeAttack, ref bool circleAttack, ref bool smashAttack, ref bool pullAttack)
	{        
		GameObject go = target;

        Vector3 targetDir = go.transform.position - transform.position;            
        float dotDirection = Vector3.Dot(transform.forward, targetDir.normalized);

		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;
        if(fDist > data.attackRange && fDist < data.strafeRange && smashCD >= 15.0f)//吸子彈回來
        {
            punchAttack = false;
			circleAttack = false;
            smashAttack = false;
            rangeAttack = false;
            pullAttack = true;
			return true;
        }
        else if(fDist > data.attackRange && fDist < data.strafeRange)//敲地板
        {
            punchAttack = false;
			circleAttack = false;
            smashAttack = true;
            rangeAttack = false;
            pullAttack = false;
			return true;
        }
		else if (fDist < data.attackRange && (dotDirection > 0.3))//普通攻擊
		{
            punchAttack = true;
			circleAttack = false;
            smashAttack = false;
            rangeAttack = false;
            pullAttack = false;
			return true;
		}
        else if(fDist < data.attackRange && (dotDirection <= 0.3 ))//轉圈
        {
            punchAttack = false;
			circleAttack = true;
            smashAttack = false;
            rangeAttack = false;
            pullAttack = false;
			return true;
        }
        else if(fDist > data.strafeRange)//遠程攻擊
        {
            punchAttack = false;
			circleAttack = false;
            smashAttack = false;
            rangeAttack = true;
            pullAttack = false;
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
        bool pullAttack = false;
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        } 
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("BattleStart"))
        {
            //Debug.Log("IsHurt");
            return;
        }
        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        currentEnemyTarget = CheckEnemyInBossArea();
        if(currentEnemyTarget != null)
        {
            data.target = currentEnemyTarget;
            CheckEnemyInAttackRange(data.target, ref punchAttack, ref rangeAttack, ref circleAttack, ref smashAttack, ref pullAttack);
            if(punchAttack)
            {
                currentState = FSMState.Attack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(pullAttack)
            {
                currentState = FSMState.Attack;
                doState = DoPullAttackState;
                checkState = CheckPullAttackState;
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
    public void CheckWaitState()
    {   
        //CheckDead
        bool punchAttack = false;
        bool rangeAttack = false;
        bool circleAttack = false;
        bool smashAttack = false;
        bool pullAttack = false;
        if(data.hp <= 0)
        {
            currentState = FSMState.Dead;            
            checkState = CheckDeadState;
            doState = DoDeadState;
            return;
        }        
        currentEnemyTarget = CheckEnemyInBossArea();
        if(currentEnemyTarget != null && currentTime >= waitTime)
        {
            data.target = currentEnemyTarget;
            CheckEnemyInAttackRange(data.target, ref punchAttack, ref rangeAttack, ref circleAttack, ref smashAttack, ref pullAttack);
            if(punchAttack)
            {
                currentState = FSMState.Attack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(pullAttack)
            {
                currentState = FSMState.Attack;
                doState = DoPullAttackState;
                checkState = CheckPullAttackState;
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
            else 
            {
                data.strafeTime = Random.Range(1.0f, 2.0f);
                currentTime = 0.0f;
                currentState = FSMState.Strafe;
                doState = DoStrafeState;
                checkState = CheckStrafeState;
            }                   
            return;
        }
    }
    public  void DoWaitState()
    {
        animator.SetBool("IsIdle", true);
        animator.SetBool("IsWalk", false);  
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
            data.strafeTime = Random.Range(0.0f, 0.5f);
            currentTime = 0.0f;
            currentState = FSMState.Attack;
            doState = DoCircleAttackState;
            checkState = CheckCircleAttackState;
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
            data.strafeTime = Random.Range(0.5f, 1.0f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }        
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
            waitTime = Random.Range(1.5f, 2.5f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoWaitState;
            checkState = CheckWaitState;
        }        
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
    public void CheckPullAttackState()
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
            waitTime = Random.Range(1.5f, 2.5f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoWaitState;
            checkState = CheckWaitState;
        }        
    }    
    public void DoPullAttackState()
    {
        data.speed = 0;
        myRigidbody.velocity = transform.forward * data.speed;
        smashCD = 0;
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PullAttack"))
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
        animator.SetTrigger("PullAttack");
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
        bool pullAttack = false;
        if(currentTime >= data.strafeTime)
        {
            data.target = currentEnemyTarget;
            CheckEnemyInAttackRange(data.target, ref punchAttack, ref rangeAttack, ref circleAttack, ref smashAttack, ref pullAttack);
            if(punchAttack)
            {
                currentState = FSMState.Attack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(pullAttack)
            {
                currentState = FSMState.Attack;
                doState = DoPullAttackState;
                checkState = CheckPullAttackState;
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
            else 
            {
                doState = DoStrafeState;
                checkState = CheckStrafeState;
            }
            return;
        }    
        return;
    }
    public override void DoStrafeState()
    {
        //Debug.Log("DoStrafe");
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);
        data.speed = 5.0f;

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
    public override void CallHurt(float damageAmount, bool isHead, bool isHurtAnimation)
    {        
        data.hp -= damageAmount;
        myHealth.ModifyHealth(damageAmount);
        if(data.target == null)
        {
            data.target = GameManager.Instance.GetPlayer();
            currentEnemyTarget = GameManager.Instance.GetPlayer();
        }
        if(data.hp <= maxHp / 2 && canRestoreShield == true)
        {
            canRestoreShield = false;
            RestoreShield();
        }
        if(data.hp > 0 && isHurtAnimation == true)
        {
            currentState = FSMState.Hurt;  
            animator.SetTrigger("TakeDamage"); 
            data.speed = 0.0f;
            myRigidbody.velocity = transform.forward * data.speed;
            doState = DoHurtState;
            checkState = CheckHurtState;
            return;
        }   
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
        {
            return;
        }
        if(animator.IsInTransition(0))
        {
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            waitTime = Random.Range(1.0f, 1.5f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoWaitState;
            checkState = CheckWaitState;
        }
    }
    public override void DoHurtState()
    {
        myRigidbody.velocity = Vector3.zero;
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
        worldEvManager.BossHasBeenDefeated(false);
        GameManager.Instance.PlayEnding();
    }

    public void ShieldHurt(float damageAmount, bool isHead, bool isHurtAnimation)
    {        
        data.shield -= damageAmount;
        myShield.ModifyShield(damageAmount);
        shieldHurt.HitFlash();
        if(data.target == null)
        {
            data.target = GameManager.Instance.GetPlayer();
            currentEnemyTarget = GameManager.Instance.GetPlayer();
        }
        if(data.shield <= 0)
        {
            coreHitBox.enabled = true;
            shieldHurt.StartDissolveShield();
            //shieldHitBox.SetActive(false);
        }
    }
    public void RestoreShield()
    {                
        coreHitBox.enabled = false;
        //shieldHitBox.SetActive(true);
        shieldHurt.StartRecoverShield();
        myShield.ModifyShield(-maxShield + data.shield);
        data.shield = maxShield;
    }
    private void StartRotateTowardPlayer()
    {
        isRotateTowardPlayer = true;
        StartCoroutine(RotateTowardPlayer());
    }
    private void EndRotateTowardPlayer()
    {
        isRotateTowardPlayer = false;
    }
    protected IEnumerator RotateTowardPlayer()
    {        
        while(isRotateTowardPlayer != false)
        {
            var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.8f);
            yield return null;
        }
    }
    public override void PlayerIsDead()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        animator.SetBool("IsWalk", false);
        animator.SetBool("IsIdle", true);
        checkState = DoIdleState;
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
