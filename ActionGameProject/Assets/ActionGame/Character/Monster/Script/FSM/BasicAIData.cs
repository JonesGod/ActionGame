using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicAIData
{
    public float hp;
    public float speed = 10.0f;
    public GameObject target;
    public Vector3 targetPosition;
    public float attackRange = 3.0f;
    public float sightRange = 12.0f;
    public float strafeRange = 8.0f;
    public float strafeTime;

    
}
