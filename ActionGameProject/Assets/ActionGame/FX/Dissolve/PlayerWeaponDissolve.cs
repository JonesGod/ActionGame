using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponDissolve : MonoBehaviour
{
    public Material material;
    public float dissolveAmount;
    public float endDissolveAmount;
    public float dissolveTime;
    public float startDissolveAmount;
    void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;        
        material.SetFloat("_DissolveAmount", dissolveAmount);  
        startDissolveAmount = dissolveAmount;  
    }
    private void OnEnable() 
    {
        StartDissolve();
    }
    private void StartDissolve()
    {
        StartCoroutine(ChangeMaterialProperty(dissolveAmount, endDissolveAmount, dissolveTime));
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
    }
}
