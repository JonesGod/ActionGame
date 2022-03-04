using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMonsterAttack : MonoBehaviour
{
    public Transform basicAttackObject;
    private BoxCollider basicAttackCollider;
    private Animator animator;

    /// Audio
    public GameObject hitAudio;
    private AudioSource hitSource;
    void Awake()
    {
        basicAttackCollider = basicAttackObject.GetComponent<BoxCollider>();
    }
    void Start()
    {
        AttackColliderOff();
        animator = GetComponent<Animator>();

        hitSource = hitAudio.GetComponent<AudioSource>();
    }
    void AttackColliderOn()
    {
        Debug.Log("AttackColliderOn");
        hitSource.Play();
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
