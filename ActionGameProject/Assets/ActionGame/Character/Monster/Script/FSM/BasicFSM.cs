using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicFSM : MonoBehaviour
{
    public enum FSMState//列出怪物所有的狀態
    {
        NONE = -1,
        Idle,
        Chase,
        Strafe,
        NormalAttack,
        Dead
    }
    private delegate void DoState();
    private DoState doState;
    private delegate void CheckAIState();
    private CheckAIState checkState;

    public BasicAIData data;
    private FSMState currentState;
    private GameObject currentEnemyTarget;
    private Animator animator;

    void Start()
    {
        currentEnemyTarget = null;
        currentState = FSMState.Idle;
        doState = DoIdleState;
        checkState = CheckIdleState;
        animator = GetComponent<Animator>();        
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
    private GameObject CheckEnemyInSight(ref bool attack)
	{
		GameObject go = GameManager.Instance.GetPlayer();
		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;//計算和玩家之間的距離
		if(fDist < data.attackRange)//如果再攻擊距離以內
		{
			attack = true;
			return go;
		}
		else if(fDist < data.sightRange)//如果在警戒範圍內
		{
			attack = false;
			return go;
		}
		return null;
	}
    void DoIdleState()
    {
        //Debug.Log("DoIdle");
    }
    void DoAttackState()
    {

    }
    void DoChaseState()
    {

    }
    void DoStrafeState()
    {

    }
    void DoDeadState()
    {
        //Debug.Log("DoDead");
        animator.SetTrigger("Die");
    }

    void CheckIdleState()
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
        currentEnemyTarget = CheckEnemyInSight(ref attack);
        if(currentEnemyTarget == null)
        {
            data.target = currentEnemyTarget;
            if (attack)
            {
                currentState = FSMState.NormalAttack;
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
    }
    void CheckAttackState()
    {

    }
    void CheckChaseState()
    {

    }
    void CheckStrafeState()
    {

    }
    void CheckDeadState()
    {

    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.name == "mesh_masterSword" && currentState != FSMState.Dead)
        {
            animator.SetTrigger("TakeDamage");
            Debug.Log("Damage");
            data.hp -= 30;
        }
    }
}