using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterAttack : MonoBehaviour
{
    public BoxCollider punchAttackCollider;

    public BoxCollider circleAttackCollider;

    public BoxCollider smashAttackCollider;
    public GameObject bulletSpawnPosition;
    public RockMonsterBulletPool monsterBulletPool;
    private Animator animator;

    public GameObject smashEffect;
    public GameObject smashEffectPosition;
    void Start()
    {
        AllAttackColliderOff();
        animator = GetComponent<Animator>();
    }
    void PunchAttackColliderOn()
    {
        punchAttackCollider.enabled = true;
    }
    void SmashAttackColliderOn()
    {
        smashAttackCollider.enabled = true;
    }
    void CircleAttackColliderOn()
    {
        circleAttackCollider.enabled = true;
    }
    void AllAttackColliderOff()
    {
        punchAttackCollider.enabled = false;
        smashAttackCollider.enabled = false;
        circleAttackCollider.enabled = false;
    }
    void ChangeAnimatorSpeed(float newSpeed)
    {
        animator.speed = newSpeed;
    }
    public void ShootBullet()
    {
        monsterBulletPool.ReUse(bulletSpawnPosition.transform.position, bulletSpawnPosition.transform.rotation);       
    }
    void PlaySmashParticle()
    {
        Instantiate(smashEffect, smashEffectPosition.transform.position, Quaternion.identity);
    }
}
