using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterTentacleAttack : MonoBehaviour
{
    public Transform basicAttackObject;
    private BoxCollider basicAttackCollider;
    private Animator animator;
    void Awake()
    {
        basicAttackCollider = basicAttackObject.GetComponent<BoxCollider>();
    }
    void Start()
    {
        AttackColliderOff();
        animator = GetComponent<Animator>();
    }
    void AttackColliderOn()
    {
        basicAttackCollider.enabled = true;
    }
    void AttackColliderOff()
    {
        basicAttackCollider.enabled = false;
    }
    void ChangeAnimatorSpeed(float newSpeed)
    {
        animator.speed = newSpeed;
    }
}
