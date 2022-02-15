using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossAttack : MonoBehaviour
{
    public Transform basicAttackObject;
    private BoxCollider basicAttackCollider;

    public Transform headAttackObject;
    private BoxCollider headAttackCollider;

    private Animator animator;
    void Awake()
    {
        basicAttackCollider = basicAttackObject.GetComponent<BoxCollider>();
        headAttackCollider = headAttackObject.GetComponent<BoxCollider>();
    }
    void Start()
    {
        AllAttackColliderOff();
        animator = GetComponent<Animator>();
    }
    void BasicAttackColliderOn()
    {
        basicAttackCollider.enabled = true;
    }
    void HeadAttackColliderOn()
    {
        headAttackCollider.enabled = true;
    }
    void AllAttackColliderOff()
    {
        basicAttackCollider.enabled = false;
        headAttackCollider.enabled = false;
    }
    void ChangeAnimatorSpeed(float newSpeed)
    {
        animator.speed = newSpeed;
    }
}
