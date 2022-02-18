using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterTentacleFSM : FSMBase
{
    private delegate void DoState();
    private DoState doState;
    private delegate void CheckAIState();
    private CheckAIState checkState;

    private GameObject currentEnemyTarget;
    private Animator animator;
    private float currentTime;
    
    public Rigidbody myRigidbody;
    //public BoxCollider CharacterCollisionBlocker; 
    Vector3 myPosition;    
    private bool isRotateTowardPlayer = false;
    public bool canRelife = true;

    void OnEnable()
    {
        Debug.Log("Start");
        currentEnemyTarget = GameManager.Instance.GetPlayer();
        data.target = currentEnemyTarget;         
        myPosition = new Vector3(this.transform.position.x, this.transform.position.y + 9.0f, this.transform.position.z);
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        data.strafeTime = Random.Range(2.0f, 3.5f);
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        myPosition = new Vector3(this.transform.position.x, this.transform.position.y + 9.0f, this.transform.position.z);
        checkState();
        doState();
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
            checkState = CheckDeadState;
            doState = DoDeadState;
            currentTime = 0;
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
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.05f);
        currentTime += Time.deltaTime;
    }    
    
    public override void CheckAttackState()
    {
        //CheckDead
        if(data.hp <= 0)
        {          
            checkState = CheckDeadState;
            doState = DoDeadState;
            currentTime = 0;
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
            data.strafeTime = Random.Range(2.0f, 3.5f);
            currentState = FSMState.Idle;
            doState = DoIdleState;
            checkState = CheckIdleState;
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
        animator.SetTrigger("AttackTrigger");
    }    
    public override void CheckHurtState()
    {
        if(data.hp <= 0)
        {          
            checkState = CheckDeadState;
            doState = DoDeadState;
            currentTime = 0;
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
        if(currentTime >= 5.0f)
        {
            data.hp = 10.0f;
            doState = DoRelifeState;
            checkState = CheckRelifeState;
        }
    }
    public override void DoDeadState()
    {
        if(canRelife == true)
        {
            currentTime += Time.deltaTime;
        }
        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            //Debug.Log("IsAttack");
            return;
        }
        animator.SetTrigger("Die");
    }
    public void CheckRelifeState()
    {
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
    public void DoRelifeState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            //Debug.Log("IsAttack");
            return;
        }

        if(animator.IsInTransition(0))
        {
            //Debug.Log("IsInTransition");
            return;
        }
        animator.SetTrigger("ReLife");
    }
    public override void CallHurt(float damageAmount, bool isHead)
    {
        data.hp -= damageAmount;
        myHealth.ModifyHealth(damageAmount);  
        currentTime = 0.0f;
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
        Debug.Log("Rotate");
        while(isRotateTowardPlayer != false)
        {
            var targetRotation = Quaternion.LookRotation(data.target.transform.position - myPosition);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.8f);
            yield return null;
        }
        Debug.Log("RotateTowardPlayerOff");
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
