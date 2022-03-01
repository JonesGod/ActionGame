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

    public GameObject pullAttackEffect;

    public GameObject chargeParticle;
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
    public void PlayPullParticle()
    {
        Vector3 direction = GameManager.Instance.GetPlayer().transform.position - this.transform.position;
        playPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0, 1.0f, 0) - GameManager.Instance.GetPlayer().transform.forward * 10.0f;
        Instantiate(pullAttackEffect, playPosition, Quaternion.identity);       

        // playPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(10, 1.0f, 0) - GameManager.Instance.GetPlayer().transform.forward * 5.0f;
        // Instantiate(pullAttackEffect, playPosition, Quaternion.identity);       

        // playPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(-10, 1.0f, 0) - GameManager.Instance.GetPlayer().transform.forward * 5.0f;
        // Instantiate(pullAttackEffect, playPosition, Quaternion.identity);       
    }
    void PlaySmashParticle()
    {
        Instantiate(smashEffect, smashEffectPosition.transform.position, smashEffectPosition.transform.rotation);
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
    void PlayChargeParticle()
    {
        Instantiate(chargeParticle, bulletSpawnPosition.transform.position, Quaternion.identity);
    }

}
