using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonsterAttack : MonoBehaviour
{
    Vector3 playPosition;
    public BoxCollider punchAttackCollider;

    public BoxCollider circleAttackCollider;

    public BoxCollider smashAttackCollider;
    public GameObject bulletSpawnPosition;
    public RockMonsterBulletPool monsterBulletPool;
    private Animator animator;

    public GameObject smashEffect;
    public GameObject smashEffectPosition;
    public GameObject smashEffectGround;
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
    void PlaySmashGroundParticle()
    {
        playPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0, 0.5f, 0);
        Instantiate(smashEffectGround, playPosition, smashEffectGround.transform.rotation);

        Vector3 newPlayPosition = playPosition + new Vector3(Random.Range(5, 10), 0, Random.Range(5, 10));
        Instantiate(smashEffectGround, newPlayPosition, smashEffectGround.transform.rotation);

        newPlayPosition = playPosition - new Vector3(Random.Range(5, 10), 0, Random.Range(5, 10));
        Instantiate(smashEffectGround, newPlayPosition, smashEffectGround.transform.rotation);

        newPlayPosition = playPosition + new Vector3(Random.Range(5, 10), 0, Random.Range(5, 10));
        Instantiate(smashEffectGround, newPlayPosition, smashEffectGround.transform.rotation);

        newPlayPosition = playPosition - new Vector3(Random.Range(5, 10), 0, Random.Range(5, 10));
        Instantiate(smashEffectGround, newPlayPosition, smashEffectGround.transform.rotation);

        newPlayPosition = playPosition + new Vector3(Random.Range(5, 10), 0, Random.Range(5, 10));
        Instantiate(smashEffectGround, newPlayPosition, smashEffectGround.transform.rotation);

        newPlayPosition = playPosition - new Vector3(Random.Range(5, 10), 0, Random.Range(5, 10));
        Instantiate(smashEffectGround, newPlayPosition, smashEffectGround.transform.rotation);
    }
}
