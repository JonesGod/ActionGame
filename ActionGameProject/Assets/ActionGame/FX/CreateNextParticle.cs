using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNextParticle : MonoBehaviour
{
    public GameObject nextEffect;
    public float waitTime = 0.8f;
    void Awake()
    {
        StartCoroutine(BeHitFlash());
    }
    IEnumerator BeHitFlash()
    {
        yield return new WaitForSeconds(waitTime);
        Instantiate(nextEffect, this.transform.position, this.transform.rotation);
    }
}
