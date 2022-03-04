using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossAttack : MonoBehaviour
{
    Vector3 playerPosition;
    //BasicAttack
    public Transform basicAttackObject;
    private BoxCollider basicAttackCollider;
    //HeadAttack
    public Transform headAttackObject;
    private BoxCollider headAttackCollider;
    //AngryBasicAttack
    public BoxCollider angryClawAttackCollider;

    private Animator animator;

    public GameObject grassEffect;
    
    public GameObject angryClawEffect;
    public GameObject angryClawPosition;

    public GameObject jumpAttackEffect;
    public GameObject jumpAttackPosition;
    public GameObject[] jumpDustPosition;

    public GameObject screamAttackEffect;

    /// Audio
    public GameObject jumpAttackAudio;
    public GameObject clawAudio;
    public GameObject screamAudio;
    public GameObject biteAudio;
    public GameObject hornAudio;
    public GameObject magicBallAudio;

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

    //爪擊
    void PlayAngryClawParticle()
    {
        Instantiate(angryClawEffect, angryClawPosition.transform.position, Quaternion.identity);
        Instantiate(clawAudio, angryClawPosition.transform.position, Quaternion.identity);
    }
    void PlayBasicClawGrassParticle()
    {
        Instantiate(grassEffect, angryClawPosition.transform.position, Quaternion.identity);
        Instantiate(clawAudio, angryClawPosition.transform.position, Quaternion.identity);
    }

    //跳躍攻擊
    void PlayJumpAttackParticle()
    {
        Instantiate(jumpAttackEffect, jumpAttackPosition.transform.position, jumpAttackPosition.transform.rotation);
        Instantiate(jumpAttackAudio, jumpAttackPosition.transform.position, jumpAttackPosition.transform.rotation);
    }
    void PlayJumpClawGrassParticle()
    {
        for(int i = 0; i < jumpDustPosition.Length; i++)
        {
            Instantiate(grassEffect, jumpDustPosition[i].transform.position, jumpDustPosition[i].transform.rotation);
        }
        //Instantiate(grassEffect, jumpAttackPosition.transform.position, jumpAttackPosition.transform.rotation);
        Instantiate(jumpAttackAudio, jumpAttackPosition.transform.position, jumpAttackPosition.transform.rotation);
    }    

    //吼叫攻擊
    void PlayScreamAttackParticle()
    {
        StartCoroutine(PlayScreamAttack());
    }
    IEnumerator PlayScreamAttack()
    {
        playerPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        Instantiate(screamAttackEffect, playerPosition, Quaternion.identity);
        Instantiate(magicBallAudio, playerPosition, Quaternion.identity);
        yield return new WaitForSeconds(2.0f);
        playerPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        Instantiate(screamAttackEffect, playerPosition, Quaternion.identity);
        Instantiate(magicBallAudio, playerPosition, Quaternion.identity);
        yield return new WaitForSeconds(2.0f);
        playerPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        Instantiate(screamAttackEffect, playerPosition, Quaternion.identity);
        Instantiate(magicBallAudio, playerPosition, Quaternion.identity);
        yield return new WaitForSeconds(2.0f);
        playerPosition = GameManager.Instance.GetPlayer().transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        Instantiate(screamAttackEffect, playerPosition, Quaternion.identity);
        Instantiate(magicBallAudio, playerPosition, Quaternion.identity);
    }
    /// <summary>
    /// 吼叫聲
    /// </summary>
    void PlayScreamAudio()
    {
        Instantiate(screamAudio,transform.position+ new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
    }
    void PlayBiteAudio()
    {
        Instantiate(biteAudio, transform.position + new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
    }
    void PlayHornAudio()
    {
        Instantiate(hornAudio, transform.position + new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
    }
}
