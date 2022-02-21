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
    private float maxShield = 150.0f;
    private float currentShield;
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
        currentShield = maxShield;
    }

    void Update()
    {
        if(data.hp <= maxHp / 2 && isAngry == false)
        {
            isAngry = true;
            return;
        }
        if(currentState != FSMState.Dead)
        {
            checkState();
            doState();    
        }      
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
    private bool CheckEnemyInAttackRange(GameObject target, ref bool normalAttack, ref bool chargeAttack, ref bool angryAttack)
	{
		GameObject go = target;
		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;
        if(fDist < data.attackRange && isAngry == true)
        {
            chargeAttack = false;
			normalAttack = false;
            angryAttack = true;
			return true;
        }
		else if (fDist < data.attackRange)
		{
            chargeAttack = false;
			normalAttack = true;
            angryAttack = false;
			return true;
		}
        else if(fDist > data.strafeRange && isAngry == false)
        {
            normalAttack = false;
            chargeAttack = true;
            angryAttack = false;
			return true;
        }
		return false;
	}    
}
