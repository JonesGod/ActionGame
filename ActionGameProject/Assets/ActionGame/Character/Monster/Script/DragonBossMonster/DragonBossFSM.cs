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

    //Rigidbody myRigidbody;
    public BoxCollider CharacterCollisionBlocker; 
    private float maxHp;
    public bool isAngry = false;
    private bool isRotateTowardPlayer = false;
    private int beAttackCount = 0;
    private float screamCD;
    private MonsterDeadDissolve monsterHurt;

    public GameObject[] angryEffect;

    WorldEvManager worldEvManager;

    void Start()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        maxHp = data.hp;
        monsterHurt = this.GetComponent<MonsterDeadDissolve>();
        worldEvManager = FindObjectOfType<WorldEvManager>();
    }

    void Update()
    {
        screamCD += Time.deltaTime;
        // Quaternion targetRotation = Quaternion.LookRotation(GameManager.Instance.GetPlayer().transform.position - transform.position);
        // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1f);
        if(data.hp <= maxHp / 2 && isAngry == false)
        {
            screamCD = 16.0f;
            isAngry = true;     
            // for(int i = 0; i < angryEffect.Length; i++)
            // {
            //     angryEffect[i].SetActive(true);  
            // }       
            return;
        }
        if(currentState != FSMState.Dead)
        {
            checkState();
            doState();    
        }        
        //myRigidbody.velocity = (GameManager.Instance.GetPlayer().transform.position - this.transform.position) * 1.0f;
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
    private bool CheckEnemyInAttackRange(GameObject target, ref bool normalAttack, ref bool chargeAttack, ref bool angryAttack, ref bool screamAttack)
	{
		GameObject go = target;
		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;
        if(isAngry == true && screamCD > 15.0f)
        {
            chargeAttack = false;
			normalAttack = false;
            angryAttack = false;
            screamAttack = true;
			return true;
        }
        if(fDist < data.attackRange && isAngry == true)
        {
            chargeAttack = false;
			normalAttack = false;
            angryAttack = true;
            screamAttack = false;
			return true;
        }
		else if (fDist < data.attackRange)
		{
            chargeAttack = false;
			normalAttack = true;
            angryAttack = false;
            screamAttack = false;
			return true;
		}
        else if(fDist > data.strafeRange && isAngry == false)
        {
            normalAttack = false;
            chargeAttack = true;
            angryAttack = false;
            screamAttack = false;
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
        bool screamAttack = false;
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
            CheckEnemyInAttackRange(data.target, ref normalAttack, ref chargeAttack, ref angryAttack, ref screamAttack);
            if(screamAttack)
            {
                currentState = FSMState.Attack;
                doState = DoScreamState;
                checkState = CheckScreamState;
            }
            else if(normalAttack)//在普通攻擊距離以內:直接攻擊
            {
                currentState = FSMState.Attack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else if(angryAttack)
            {
                currentState = FSMState.Attack;
                doState = DoAngryAttackState;
                checkState = CheckAngryAttackState;
            }
            else if(chargeAttack)//在普通攻擊距離以外:進入追逐
            {
                currentState = FSMState.Attack;
                doState = DoChargeAttackState;
                checkState = CheckChargeAttackState;
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
        bool screamAttack = false;
        CheckEnemyInAttackRange(data.target, ref normalAttack, ref chargeAttack, ref angryAttack, ref screamAttack);
        if(screamAttack)
        {
            currentState = FSMState.Attack;
            doState = DoScreamState;
            checkState = CheckScreamState;
        }
        else if (normalAttack)
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
        var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);
        //myRigidbody.velocity = (data.target.transform.position - transform.position) * data.speed;
        //transform.LookAt(data.target.transform.position);
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
        bool screamAttack = false;
        if(currentTime >= data.strafeTime)
        {
            CheckEnemyInAttackRange(data.target, ref normalAttack, ref chargeAttack, ref angryAttack, ref screamAttack);
            if(screamAttack)
            {
                currentState = FSMState.Attack;
                doState = DoScreamState;
                checkState = CheckScreamState;
            }
            else if(normalAttack)//在攻擊距離以內了:直接攻擊
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
            var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);
            //myRigidbody.velocity = (data.target.transform.position - transform.position) * data.speed;            
            //transform.LookAt(data.target.transform.position);
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
            var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);
            //transform.LookAt(data.target.transform.position);
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
        data.speed = 0;
        myRigidbody.velocity = transform.forward * data.speed;
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ClawAttack"))
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
    public override void CallHurt(float damageAmount, bool isHead, bool isHurtAnimation)
    {        
        data.hp -= damageAmount;
        myHealth.ModifyHealth(damageAmount);
        monsterHurt.HitFlash();
        if(data.target == null)
        {
            data.target = GameManager.Instance.GetPlayer();
            currentEnemyTarget = GameManager.Instance.GetPlayer();
        }
        if(isHead == true)
        {
            beAttackCount += 1;
        }
        // if(data.hp <= maxHp / 2 && isAngry == false)
        // {
        //     isAngry = true;
        //     doState = DoScreamState;
        //     checkState = CheckScreamState;
        //     return;
        // }
        if(data.hp > 0 && isHead == true && isAngry == false && beAttackCount >= 5)
        {
            beAttackCount = 0;
            currentState = FSMState.Hurt;  
            animator.SetTrigger("TakeDamage"); 
            data.speed = 0.0f;
            myRigidbody.velocity = transform.forward * data.speed;
            doState = DoHurtState;
            checkState = CheckHurtState;
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
        // for(int i = 0; i < angryEffect.Length; i++)
        // {
        //     angryEffect[i].SetActive(false);  
        // }       
        worldEvManager.BossHasBeenDefeated();
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
            data.strafeTime = Random.Range(1.0f, 2.5f);
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
            var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.1f);
            //myRigidbody.velocity = (data.target.transform.position - transform.position) * data.speed;
            //transform.LookAt(data.target.transform.position);
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
        animator.SetBool("IsRunForward", false);
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
            Vector3 v = data.target.transform.position - this.transform.position;
            float fDist = v.magnitude;
            if(fDist < 2 * data.attackRange)
            {
                currentState = FSMState.Attack;
                doState = DoJumpAttackState;
                checkState = CheckJumpAttackState;
            }
            else
            {
                data.strafeTime = Random.Range(0.5f, 2.0f);
                currentTime = 0.0f;
                currentState = FSMState.Attack;
                doState = DoHornAttackState;
                checkState = CheckHornAttackState;
            }            
        }        
    }

    private void DoAngryAttackState()
    {
        myRigidbody.velocity = Vector3.zero;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Angry Claw Attack"))
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
        animator.SetTrigger("AngryAttack");
    }
    private void CheckScreamState()
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
            currentState = FSMState.Strafe;
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }        
    }
    public void DoScreamState()
    {
        myRigidbody.velocity = Vector3.zero;
        screamCD = 0;
        animator.SetBool("IsWalkForward", false);
        animator.SetBool("IsRunForward", false);
        animator.SetBool("IsIdle", false);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Scream"))
        {            
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }        
        animator.SetTrigger("Scream");
    }
    private void CheckHornAttackState()
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            data.strafeTime = Random.Range(0.5f, 2.0f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            doState = DoStrafeState;
            checkState = CheckStrafeState;
        }        
    }

    private void DoHornAttackState()
    {
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
        Vector3 v = data.target.transform.position - this.transform.position;
		float fDist = v.magnitude;
        if(fDist > data.attackRange)
        {
            data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);

            data.speed = 15.0f;
            animator.SetBool("IsWalkForward", false);
            animator.SetBool("IsRunForward", true);
            var targetRotation = Quaternion.LookRotation(data.target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.1f);
            //myRigidbody.velocity = (data.target.transform.position - transform.position) * data.speed;
            //transform.LookAt(data.target.transform.position);
            //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
            myRigidbody.velocity = transform.forward * data.speed;
        }
        else
        {
            myRigidbody.velocity = Vector3.zero;
            animator.SetTrigger("HornAttack");
        }        
    }
    private void CheckJumpAttackState()
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
            data.strafeTime = Random.Range(0.5f, 2.0f);
            currentTime = 0.0f;
            currentState = FSMState.Attack;
            doState = DoHornAttackState;
            checkState = CheckHornAttackState;
        }        
    }

    private void DoJumpAttackState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
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
        animator.SetTrigger("JumpAttack");       
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
