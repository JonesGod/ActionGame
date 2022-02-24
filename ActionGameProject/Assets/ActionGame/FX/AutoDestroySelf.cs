using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroySelf : MonoBehaviour
{
    public float destroyDelay = 3.0f;
    void Start()
    {
        Destroy(this.gameObject, destroyDelay);
    }
}
