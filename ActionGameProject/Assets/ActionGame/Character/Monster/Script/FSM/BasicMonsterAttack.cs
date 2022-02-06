using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMonsterAttack : MonoBehaviour
{
    public Transform basicAttackObject;
    private BoxCollider basicAttackCollider;
    void Awake()
    {
        basicAttackCollider = basicAttackObject.GetComponent<BoxCollider>();
    }
    void Start()
    {
        AttackColliderOff();
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
}
