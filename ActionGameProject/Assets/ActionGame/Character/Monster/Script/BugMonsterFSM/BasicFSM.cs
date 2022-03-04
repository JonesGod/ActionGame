using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFSM : FSMBase
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

    public List<FSMBase> partnerMonster;
    private float partnerRange = 30.0f;
    private Vector3 startPosition;
    public bool hide;
    private MonsterDeadDissolve monsterHurt;
    public GameObject spawnEffect;
    void OnEnable()
    {
        Instantiate(spawnEffect, this.transform.position, Quaternion.identity);
    }
    void Start()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        strafeDirection = 0; 
        startPosition = this.transform.position;
        monsterHurt = this.GetComponent<MonsterDeadDissolve>();
        if(hide == true)
        {
            this.gameObject.SetActive(false);
        }
        if(GameManager.Instance.allMonster != null && GameManager.Instance.allMonster.Length > 0)
        {           
            foreach(GameObject m in GameManager.Instance.allMonster)
            {
                if(m.name != this.gameObject.name)
                {
                    Vector3 vec = m.transform.position - transform.position;
                    if(vec.magnitude <= partnerRange)
                    {
                        partnerMonster.Add(m.GetComponent<FSMBase>());
                    }    
                }             
            }
        }        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position + (transform.up * 1.5f) + (transform.right * widthDistance), transform.forward * range, Color.red);
        Debug.DrawRay(transform.position + (transform.up * 1.5f) - (transform.right * widthDistance), transform.forward * range, Color.red);
        Debug.DrawRay(transform.position + (transform.up * 1.5f) - (transform.forward * backDistance), -transform.right * range, Color.yellow);
        Debug.DrawRay(transform.position + (transform.up * 1.5f) - (transform.forward * backDistance), transform.right * range, Color.yellow);        
        // var v3 = transform.forward * 1.0f;
        // v3.y = myRigidbody.velocity.y;
        // myRigidbody.velocity = v3;
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
    public override void HelpPartner()
    {
        if(currentState == FSMState.Idle)
        {
            data.target = GameManager.Instance.GetPlayer(); 
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
                data.strafeTime = Random.Range(1.0f, 3.0f);;
                currentTime = 0.0f;
                currentState = FSMState.Strafe;
                strafeDirection = Random.Range(0, 3);
                doState = DoStrafeState;
                checkState = CheckStrafeState;
            }
        }
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
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
        }        
    }
    public override void DoIdleState()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out hit, 1.0f, Physics.AllLayers))
        {
            transform.up = hit.normal;
        }
        //Debug.Log("DoIdle");
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
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out hit, 1.0f, Physics.AllLayers))
        {
            Debug.Log("hit");
            transform.up = hit.normal;
        }
        Debug.Log("DoChaseState");
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);

        data.speed = 6.0f;
        animator.SetBool("IsMoveRight", false);
        animator.SetBool("IsMoveForward", true);
        
        //AvoidCollisionMove();
        transform.LookAt(data.target.transform.position, transform.up);
        myRigidbody.velocity = transform.forward * data.speed;
        
        
        //myRigidbody.velocity = transform.forward * data.speed;
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
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
        {
            myRigidbody.velocity = Vector3.zero;
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
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out hit, 1.0f, Physics.AllLayers))
        {
            Debug.Log("hit");
            transform.up = hit.normal;
        }
        data.targetPosition = new Vector3(data.target.transform.position.x, this.transform.position.y, data.target.transform.position.z);

		Vector3 v = data.targetPosition - this.transform.position;
		float fDist = v.magnitude;
        for(int i = 0; i < partnerMonster.Count; i++)
        {
            Vector3 vec = partnerMonster[i].transform.position - transform.position;
            float partnerDist = v.magnitude;
            if(partnerMonster[i].currentState == FSMState.Chase && partnerDist < data.attackRange + 1)
            {
                Vector3 targetDir = partnerMonster[i].transform.position - transform.position;            
                float dotPartner = Vector3.Dot(transform.right, targetDir.normalized);

                animator.SetBool("IsIdle", false);
                animator.SetBool("IsMoveForward", false); 
                animator.SetBool("IsMoveRight", true); 
                transform.LookAt(data.target.transform.position, transform.up);
                data.speed = 3.0f;
                if(dotPartner > 0)
                {
                    transform.Translate(Vector3.left * data.speed * Time.deltaTime);
                    strafeDirection = 1;
                }
                else
                {
                    transform.Translate(Vector3.right * data.speed * Time.deltaTime);
                    strafeDirection = 0;
                }
                currentTime += 0;
                return;
            }           
        }        
        if(fDist > data.strafeRange)
        {
            animator.SetBool("IsMoveRight", false); 
            animator.SetBool("IsMoveForward", true);    
            data.speed = 1.5f;    
            transform.LookAt(data.target.transform.position, transform.up);
            //transform.position = Vector3.MoveTowards(transform.position, data.target.transform.position, data.speed * Time.deltaTime);
            myRigidbody.velocity = transform.forward * data.speed;
            currentTime += Time.deltaTime;
            return;
        }        
        transform.LookAt(data.target.transform.position, transform.up);
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
        {
            myRigidbody.velocity = Vector3.zero;
            return;
        }        
        else if(strafeDirection == 0)
        {
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsMoveForward", false); 
            animator.SetBool("IsMoveRight", true); 
            data.speed = 1.5f;
            myRigidbody.velocity = transform.right * data.speed;
            //transform.Translate(Vector3.right * data.speed * Time.deltaTime);
        }
        else if(strafeDirection == 1)
        {
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsMoveForward", false); 
            animator.SetBool("IsMoveRight", true); 
            data.speed = 1.5f;
            myRigidbody.velocity = -transform.right * data.speed;
            //transform.Translate(Vector3.left * data.speed * Time.deltaTime);
        }        
        else
        {
            animator.SetBool("IsMoveRight", false);
            animator.SetBool("IsMoveForward", false);
            animator.SetBool("IsIdle", true);
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
            data.strafeTime = Random.Range(1.0f, 4.0f);
            currentTime = 0.0f;
            currentState = FSMState.Strafe;
            strafeDirection = Random.Range(0, 3);
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
        myRigidbody.velocity = Vector3.zero;
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
            strafeDirection = Random.Range(0, 3);
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
        data.speed = 0.0f;
        myRigidbody.velocity = Vector3.zero;   
        for(int i = 0; i < partnerMonster.Count; i++)
        {
            partnerMonster[i].HelpPartner();           
        }
        
        data.hp -= damageAmount;
        monsterHurt.HitFlash();
        myHealth.ModifyHealth(damageAmount);        
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
        animator.SetBool("IsMoveRight", false);
        animator.SetBool("IsMoveForward", false);
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
        animator.SetBool("IsMoveRight", false);
        animator.SetBool("IsMoveForward", false);
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