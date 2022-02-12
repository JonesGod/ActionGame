using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFSM : FSMBase
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

    public List<BasicFSM> partnerMonster;
    private float partnerRange = 30.0f;
    void Start()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        strafeDirection = 0;        

        if(GameManager.Instance.allMonster != null && GameManager.Instance.allMonster.Length > 0)
        {           
            foreach(GameObject m in GameManager.Instance.allMonster)
            {
                if(m.name != this.gameObject.name)
                {
                    Vector3 vec = m.transform.position - transform.position;
                    if(vec.magnitude <= partnerRange)
                    {
                        partnerMonster.Add(m.GetComponent<BasicFSM>());
                    }    
                }             
            }
        }
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
    public void HelpPartner()
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
                strafeDirection = Random.Range(0, 2);
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
    public override void DoIdleState()
    {

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

		Vector3 v = data.targetPosition - this.transform.position;
		float fDist = v.magnitude;
        for(int i = 0; i < partnerMonster.Count; i++)
        {
            if(partnerMonster[i].currentState == FSMState.Chase)
            {
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
                currentTime += 0;
                return;
            }           
        }        
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
    public void CallHurt(float damageAmount)
    {        
        Debug.Log("TakeDamage");
        for(int i = 0; i < partnerMonster.Count; i++)
        {
            partnerMonster[i].HelpPartner();           
        }
        data.hp -= damageAmount;
        if(data.hp > 0)
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