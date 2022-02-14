using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicMonsterData
{
    public float hp;
    public float speed;
    public GameObject target;
    public Vector3 targetPosition;
    public float attackRange;
    public float sightRange;
    public float strafeRange;
    public float strafeTime;
    public float patrolTime;
    
}
