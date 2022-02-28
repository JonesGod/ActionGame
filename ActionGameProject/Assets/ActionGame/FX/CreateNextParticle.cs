using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNextParticle : MonoBehaviour
{
    public GameObject nextEffect;
    void Awake()
    {
        StartCoroutine(BeHitFlash());
    }
    IEnumerator BeHitFlash()
    {
        yield return new WaitForSeconds(0.8f);
        Instantiate(nextEffect, this.transform.position, Quaternion.identity);
    }
}
