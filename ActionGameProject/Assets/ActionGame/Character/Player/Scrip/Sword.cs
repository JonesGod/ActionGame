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
    public Transform sword;
    public GameObject hitEffectControl;
    ///Audio
    public GameObject Slash;
    private AudioSource slashAudio;
    public GameObject SwordSkill01;
    private AudioSource swordSkillAudio01;
    public GameObject SwordSkill02;
    private AudioSource swordSkillAudio02;

    /// effect
    public GameObject m_AOE;

    public GameObject attackEffect01;
    public GameObject attackEffect02;
    public GameObject attackEffect03;
    public GameObject attackEffect04;
    public GameObject specialEffect1_1;
    public GameObject specialEffect1_2;
    public GameObject specialEffect2_3;

    private SwordHitEffect swordhit;
    private ParticleSystem attackParticle01;
    private ParticleSystem attackParticle02;
    private ParticleSystem attackParticle03;
    private ParticleSystem attackParticle04;
    private ParticleSystem specialParticle1_1;
    private ParticleSystem specialParticle1_2;
    private ParticleSystem specialParticle2_3;

    [HideInInspector] public bool battleRunIsNext;//��PlayerControl�P�w
    [HideInInspector] public bool runIsNext;//��PlayerControl�P�w
    [HideInInspector] public bool bowIsNext;//��PlayerControl�P�w
    [HideInInspector] public bool idleState;
    [HideInInspector] public bool idleIsNext;
    [HideInInspector] public bool isTransition;

    private BoxCollider swordBoxCollider;
    private BoxCollider aoeCollider;

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
        aoeCollider = m_AOE.GetComponent<BoxCollider>();
        swordBoxCollider = sword.GetComponent<BoxCollider>();
        swordhit = hitEffectControl.GetComponent<SwordHitEffect>();

        attackParticle01 = attackEffect01.GetComponent<ParticleSystem>();
        attackParticle02 = attackEffect02.GetComponent<ParticleSystem>();
        attackParticle03 = attackEffect03.GetComponent<ParticleSystem>();
        attackParticle04 = attackEffect04.GetComponent<ParticleSystem>();
        specialParticle1_1 = specialEffect1_1.GetComponent<ParticleSystem>();
        specialParticle1_2 = specialEffect1_2.GetComponent<ParticleSystem>();
        specialParticle2_3 = specialEffect2_3.GetComponent<ParticleSystem>();

        slashAudio = Slash.GetComponent<AudioSource>();
        swordSkillAudio01 = SwordSkill01.GetComponent<AudioSource>();
        swordSkillAudio02 = SwordSkill02.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (battleRunIsNext)
        {
            if (sword.gameObject.activeSelf == false)
                SwordOn();
            if (trail.activeSelf == true)
                SwordTrailOff();
        }
        if (attackState)
        {
            if (sword.gameObject.activeSelf == false)
                SwordOn();
        }

        if (runIsNext || bowIsNext || idleIsNext)
        {
            if (sword.gameObject.activeSelf == true)
                SwordOff();
        }
    }
    /// <summary>
    /// for attack01
    /// </summary>
    void SwordTrailOn()
    {
        //trail.SetActive(true);
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
    void SwordColliderOn(int id)
    {
        swordhit.SelectHitEffect(id);
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
        while (time < duration)
        {
            dissolveAmount = Mathf.Lerp(v_start, v_end, time / duration);
            swordMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            time += Time.deltaTime;
            yield return null;
        }
        dissolveAmount = v_end;
    }
    IEnumerator WeaponOff(float v_start, float v_end, float duration)
    {
        float time = 0.0f;
        while (time < duration)
        {
            dissolveAmount = Mathf.Lerp(v_start, v_end, time / duration);
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
    void SlashEffect(int number)
    {

        switch (number)
        {
            case 1:
                attackParticle01.Play();
                slashAudio.Play();
                break;
            case 2:
                attackParticle02.Play();
                slashAudio.Play();
                break;
            case 3:
                attackParticle03.Play();
                slashAudio.Play();
                break;
            case 4:
                attackParticle04.Play();
                slashAudio.Play();
                break;
            case 11:
                specialParticle1_1.Play();
                swordSkillAudio01.Play();
                break;
            case 12:
                specialParticle1_2.Play();
                swordSkillAudio01.Play();
                break;
            case 23:
                specialParticle2_3.Play();
                slashAudio.Play();
                break;
        }

    }
    void StartAOE()
    {
        StartCoroutine(RangAttack());
    }
    protected IEnumerator RangAttack()
    {
        m_AOE.SetActive(true);
        swordSkillAudio02.Play();
        aoeCollider.enabled = true;
        yield return new WaitForSeconds(0.25f);
        aoeCollider.enabled = false;
        yield return new WaitForSeconds(1f);
        m_AOE.SetActive(false);
    }
}
