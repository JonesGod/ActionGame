using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHurtBlink : MonoBehaviour
{
    public Material material;
    public float dissolveAmount;
    public float endDissolveAmount;
    public float startDissolveAmount;
    public float dissolveTime;
    private BoxCollider hitBox;
    void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;      
        material.SetFloat("_DissolveAmount", dissolveAmount);  
        hitBox = this.GetComponent<BoxCollider>();
    }
    public void StartDissolveShield()
    {
        StartCoroutine(ShieldDissolveProperty(dissolveAmount, endDissolveAmount, dissolveTime));
    }

    public void StartRecoverShield()
    {
        StartCoroutine(ShieldRecoveryProperty(dissolveAmount, startDissolveAmount, dissolveTime));
    }

    IEnumerator ShieldDissolveProperty(float v_start, float v_end, float duration)
    {
        Debug.Log("Dissolve");
        float time = 0.0f;
        while (time < duration )
        {
            dissolveAmount = Mathf.Lerp(v_start, v_end, time / duration );
            material.SetFloat("_DissolveAmount", dissolveAmount);
            time += Time.deltaTime;
            yield return null;
        }
        dissolveAmount = v_end;
        //this.gameObject.SetActive(false);
        hitBox.enabled = false;
    }
    IEnumerator ShieldRecoveryProperty(float v_start, float v_end, float duration)
    {
        Debug.Log("BackDissolve");
        float time = 0.0f;
        while (time < duration )
        {
            dissolveAmount = Mathf.Lerp(v_start, v_end, time / duration );
            material.SetFloat("_DissolveAmount", dissolveAmount);
            time += Time.deltaTime;
            yield return null;
        }
        dissolveAmount = v_end;
        //this.gameObject.SetActive(true);
        hitBox.enabled = true;
    }
    public void HitFlash()
    {
        StartCoroutine(BeHitFlash());
    }
    IEnumerator BeHitFlash()
    {
        material.SetColor("_ColorGlitter", new Color(1.0f, 1.0f, 1.0f, 0.0f));
        yield return new WaitForSeconds(0.1f);
        material.SetColor("_ColorGlitter", new Color(0.0f, 0.0f, 0.0f, 0.0f));
    }
}
