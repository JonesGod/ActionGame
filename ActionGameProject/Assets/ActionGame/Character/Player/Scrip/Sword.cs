using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public static Sword Instance
    {
        get { return s_Instance; }
    }

    protected static Sword s_Instance;

    public GameObject trail;

    public Transform[] link;
    public Transform sword;
    
    [HideInInspector] public bool battleRunIsNext;//��PlayerControl�P�w
    [HideInInspector] public bool runIsNext;//��PlayerControl�P�w
    [HideInInspector] public bool bowIsNext;//��PlayerControl�P�w

    private BoxCollider swordBoxCollider;

    [HideInInspector] public bool attackState;
    //武器消融相關
    public Material swordMaterial;
    public float dissolveAmount = -1.0f;
    public float endDissolveAmount = 1.5f;
    public float dissolveTime = 1.0f;
    public float startDissolveAmount;
    
    private void Awake()
    {
        s_Instance = this;
        swordMaterial = sword.GetComponent<MeshRenderer>().material;       
        swordMaterial.SetFloat("_DissolveAmount", dissolveAmount);  
    }
    void Start()
    {
        swordBoxCollider = sword.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (battleRunIsNext)
        {
            if(sword.gameObject.activeSelf==false)
                SwordOn();
            if (trail.activeSelf == true)
                SwordTrailOff();
        }
        if(attackState)
        {
            if (sword.gameObject.activeSelf == false)
                SwordOn();
        }

        if (runIsNext || bowIsNext)
        {
            if(sword.gameObject.activeSelf == true)
                SwordOff();
        }
    }   
    void SwordTrailOn()
    {
        trail.SetActive(true);
    }
    void SwordTrailOff()
    {
        trail.SetActive(false);
    }
    void SwordOn()
    {
        sword.gameObject.SetActive(true);
        WeaponOnDissolve();       
    }
    public void SwordOff()
    {
        //sword.gameObject.SetActive(false);
        //startDissolveAmount = dissolveAmount;
        WeaponOffDissolve();
    }
    void SwordColliderOn()
    {
        Debug.Log("ON123");
        swordBoxCollider.enabled = true;
    }
    public void SwordColliderOff()
    {
        swordBoxCollider.enabled = false;
    }

    IEnumerator WeaponOn(float v_start, float v_end, float duration)
    {
        Debug.Log("weaponOn");
        float time = 0.0f;
        while (time < duration )
        {
            dissolveAmount = Mathf.Lerp(v_start, v_end, time / duration );
            swordMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            time += Time.deltaTime;
            yield return null;
        }
        dissolveAmount = v_end;
    }
    IEnumerator WeaponOff(float v_start, float v_end, float duration)
    {
        float time = 0.0f;
        while (time < duration )
        {
            dissolveAmount = Mathf.Lerp(v_start, v_end, time / duration );
            swordMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            time += Time.deltaTime;
            yield return null;
        }
        dissolveAmount = v_end;
        sword.gameObject.SetActive(false);
    }
    private void WeaponOnDissolve()
    {
        StartCoroutine(WeaponOn(dissolveAmount, endDissolveAmount, dissolveTime));
    }
    private void WeaponOffDissolve()
    {
        StartCoroutine(WeaponOff(dissolveAmount, startDissolveAmount, 0.15f));
    }

}
