using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDeadDissolve : MonoBehaviour
{
    public Material material;
    public float dissolveAmount = -1.0f;
    public float endDissolveAmount = 1.5f;
    public float dissolveTime = 1.0f;
    void Awake()
    {
        material = this.GetComponentInChildren<SkinnedMeshRenderer>().material;        
        material.SetFloat("_DissolveAmount", dissolveAmount);
    }

    private void StartDissolve()
    {
        StartCoroutine(ChangeMaterialProperty(dissolveAmount, endDissolveAmount, dissolveTime));
    }
    public void HitFlash()
    {
        StartCoroutine(BeHitFlash());
    }
    IEnumerator ChangeMaterialProperty(float v_start, float v_end, float duration)
    {
        float time = 0.0f;
        while (time < duration )
        {
            dissolveAmount = Mathf.Lerp(v_start, v_end, time / duration );
            material.SetFloat("_DissolveAmount", dissolveAmount);
            time += Time.deltaTime;
            yield return null;
        }
        dissolveAmount = v_end;
        this.gameObject.SetActive(false);
    }
    IEnumerator BeHitFlash()
    {
        material.SetColor("_ColorGlitter", new Color(0.4f, 0.4f, 0.4f));
        yield return new WaitForSeconds(0.1f);
        material.SetColor("_ColorGlitter", Color.black);
    }
}
