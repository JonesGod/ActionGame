using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossAttack : MonoBehaviour
{
    Vector3 playerPosition;
    public Transform basicAttackObject;
    private BoxCollider basicAttackCollider;

    public Transform headAttackObject;
    private BoxCollider headAttackCollider;
    public BoxCollider angryClawAttackCollider;

    private Animator animator;
    
    public GameObject angryClawEffect;
    public GameObject angryClawPosition;

    public GameObject jumpAttackEffect;
    public GameObject jumpAttackPosition;

    public GameObject screamAttackEffect;
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
    void PlayJumpAttackParticle()
    {
        Instantiate(jumpAttackEffect, jumpAttackPosition.transform.position, jumpAttackPosition.transform.rotation);
    }
    void PlayScreamAttackParticle()
    {
        StartCoroutine(PlayScreamAttack());
    }
    IEnumerator PlayScreamAttack()
    {
        playerPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        Instantiate(screamAttackEffect, playerPosition, Quaternion.identity);
        yield return new WaitForSeconds(2.0f);
        playerPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        Instantiate(screamAttackEffect, playerPosition, Quaternion.identity);
        yield return new WaitForSeconds(2.0f);
        playerPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        Instantiate(screamAttackEffect, playerPosition, Quaternion.identity);        
        yield return new WaitForSeconds(2.0f);
        playerPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        Instantiate(screamAttackEffect, playerPosition, Quaternion.identity);
    }
}
