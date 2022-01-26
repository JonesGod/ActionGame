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
        Watch,
        NormalAttack,
        Dead
    }
    private delegate void DoState();
    private DoState doState;
    private delegate void CheckAIState();
    private CheckAIState checkState;

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
        checkState();
        doState();
        
    }
    void DoIdleState()
    {

    }
    void CheckIdleState()
    {

    }
}
