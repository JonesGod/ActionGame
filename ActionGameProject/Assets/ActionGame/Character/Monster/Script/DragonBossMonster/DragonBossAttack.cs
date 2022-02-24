using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossAttack : MonoBehaviour
{
    public Transform basicAttackObject;
    private BoxCollider basicAttackCollider;

    public Transform headAttackObject;
    private BoxCollider headAttackCollider;
    public BoxCollider angryClawAttackCollider;

    private Animator animator;
    
    public GameObject angryClawEffect;
    public GameObject angryClawPosition;
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
    void AngryClawAttackColliderOn()
    {
        angryClawAttackCollider.enabled = true;
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
    void PlayAngryClawParticle()
    {
        Instantiate(angryClawEffect, angryClawPosition.transform.position, Quaternion.identity);
    }
}
