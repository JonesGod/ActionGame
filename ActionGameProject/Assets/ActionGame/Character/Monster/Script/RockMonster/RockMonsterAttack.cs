using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterAttack : MonoBehaviour
{
    public BoxCollider punchAttackCollider;

    public SphereCollider circleAttackCollider;

    //public BoxCollider smashAttackCollider;

    private Animator animator;
    void Start()
    {
        AllAttackColliderOff();
        animator = GetComponent<Animator>();
    }
    void PunchAttackColliderOn()
    {
        punchAttackCollider.enabled = true;
    }
    // void SmashAttackColliderOn()
    // {
    //     smashAttackCollider.enabled = true;
    // }
    void CircleAttackColliderOn()
    {
        circleAttackCollider.enabled = true;
    }
    void AllAttackColliderOff()
    {
        punchAttackCollider.enabled = false;
        //smashAttackCollider.enabled = false;
        circleAttackCollider.enabled = false;
    }
    void ChangeAnimatorSpeed(float newSpeed)
    {
        animator.speed = newSpeed;
    }
}
