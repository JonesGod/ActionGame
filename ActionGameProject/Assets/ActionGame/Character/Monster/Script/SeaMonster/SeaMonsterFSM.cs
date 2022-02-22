using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterFSM : FSMBase
{
    private delegate void DoState();
    private DoState doState;
    private delegate void CheckAIState();
    private CheckAIState checkState;

    private GameObject currentEnemyTarget;
    private Animator animator;
    private float currentTime;
    
    //Rigidbody myRigidbody;
    //public BoxCollider CharacterCollisionBlocker; 
    
    public SeaMonsterTentacleFSM[] partnerMonster;
    Vector3 myPosition;

    void OnEnable()
    {
        currentEnemyTarget = GameManager.Instance.GetPlayer();
        data.target = currentEnemyTarget;
        myPosition = new Vector3(this.transform.position.x, this.transform.position.y + 44, this.transform.position.z);
        currentState = FSMState.Idle;        
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        data.strafeTime = Random.Range(3.0f, 4.0f);
        currentTime = 0;        
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
        if(currentEnemyTarget != null && currentTime >= data.strafeTime)
        {
            data.target = currentEnemyTarget;
            CheckEnemyInAttackRange(data.target, ref attack);
            if (attack)//在攻擊距離以內了:直接攻擊
            {
                currentState = FSMState.Attack;
                doState = DoAttackState;
                checkState = CheckAttackState;
            }
            else
            {
                currentState = FSMState.Idle;
                doState = DoIdleState;
                checkState = CheckIdleState;
            }
            return;
        }
    }
    public override void DoIdleState()
    {
        var targetRotation = Quaternion.LookRotation(data.target.transform.position - myPosition);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
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
            currentTime = 0.0f;
            data.strafeTime = Random.Range(3.0f, 4.0f);
            currentState = FSMState.Idle;
            doState = DoIdleState;
            checkState = CheckIdleState;
        }        
    }    
    public override void DoAttackState()
    {
        var targetRotation = Quaternion.LookRotation(data.target.transform.position - myPosition); 
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
        //Debug.Log("DoAttack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Roar"))
        {
            //Debug.Log("IsAttack");
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            //Debug.Log("IsAttack");
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            //Debug.Log("IsAttack");
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
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
            currentTime = 0.0f;
            data.strafeTime = Random.Range(2.5f, 3.0f);
            currentState = FSMState.Idle;
            doState = DoIdleState;
            checkState = CheckIdleState;
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
        if(partnerMonster != null && partnerMonster.Length > 0)
        {   
            for(int i = 0; i < partnerMonster.Length; i++)
            {
                if(partnerMonster[i].currentState != FSMState.Dead)
                {
                    partnerMonster[i].data.hp = 0;
                    partnerMonster[i].canRelife = false;
                }
            }                    
        }
        animator.SetTrigger("Die");
        myRigidbody.isKinematic = true;
        //CharacterCollisionBlocker.enabled = false;
    }
    public override void CallHurt(float damageAmount, bool isHead, bool isHurtAnimation)
    {
        data.hp -= damageAmount;
        myHealth.ModifyHealth(damageAmount);  
        if(data.hp > 0)
        {
            currentState = FSMState.Hurt;  
            //animator.SetTrigger("TakeDamage"); 
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
        animator.SetBool("IsIdle", true);
        checkState = DoIdleState;
        doState = DoIdleState;
        return;
    }
    public override void PlayerIsReLife()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        animator.SetBool("IsIdle", true);
        checkState = CheckIdleState;
        doState = DoIdleState;
        return;
    }
    public void ShootBullet()
    {
        
    }
    public void SummonTentacles()
    {
        for(int i = 0; i < partnerMonster.Length; i++)
        {
            partnerMonster[i].gameObject.SetActive(true);
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
