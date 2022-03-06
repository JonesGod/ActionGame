using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DS : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(HideSelf());
    }

    IEnumerator HideSelf()
    {
        yield return new WaitForSeconds(2.0f);
        this.gameObject.SetActive(false);
    }
}
