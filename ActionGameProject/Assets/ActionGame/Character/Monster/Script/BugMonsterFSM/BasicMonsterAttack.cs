using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMonsterAttack : MonoBehaviour
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
        Debug.Log("AttackColliderOn");
        basicAttackCollider.enabled = true;
    }
    void AttackColliderOff()
    {
        Debug.Log("AttackColliderOff");
        basicAttackCollider.enabled = false;
    }
    void ChangeAnimatorSpeed(float newSpeed)
    {
        animator.speed = newSpeed;
    }
}
