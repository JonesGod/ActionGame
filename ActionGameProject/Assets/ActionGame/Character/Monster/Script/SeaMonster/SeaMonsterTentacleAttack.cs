using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMonsterTentacleAttack : MonoBehaviour
{
    public Transform basicAttackObject;
    private BoxCollider basicAttackCollider;
    private Animator animator;

    public GameObject attackAudio;
    private AudioSource attackSource;
    void Awake()
    {
        basicAttackCollider = basicAttackObject.GetComponent<BoxCollider>();
    }
    void Start()
    {
        AttackColliderOff();
        animator = GetComponent<Animator>();

        attackSource = attackAudio.GetComponent<AudioSource>();
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
    void PlayAttackAudio()
    {
        attackSource.Play();
    }
}
