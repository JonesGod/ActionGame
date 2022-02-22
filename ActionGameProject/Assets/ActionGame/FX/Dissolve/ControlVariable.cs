using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlVariable : MonoBehaviour
{
    public float speed = 100.0f;
    private Material material;
    private float dissolveAmount = -1.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeSpeed(speed, 0.0f, 2f));
        
        material = this.GetComponent<SkinnedMeshRenderer>().material;        
        material.SetFloat("_DissolveAmount", dissolveAmount);
        StartCoroutine(ChangeMaterialProperty(dissolveAmount, 1.5f, 3f));        
    }
    void Update()
    {

    }
    
    IEnumerator ChangeSpeed(float v_start, float v_end, float duration)
    {
        float time = 0.0f;
        while (time < duration )
        {
            speed = Mathf.Lerp(v_start, v_end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        speed = v_end;
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
