using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEvManager : MonoBehaviour
{
    public static WorldEvManager Instance
    {
        get { return s_Instance; }
    }
    protected static WorldEvManager s_Instance;

    public DragonBossHealthBar dragonBossHPBar;
    public RockBossHPBar rockBossHPBar;
    public RockBossShieldBar rockBossShieldBar;

    public List<FogWell> fogWells;
    //public UIBossHPBar bossHPBar;
    //public EnemyBossManager boss;
    public GameObject hintbox;
    public GameObject hide;
    public GameObject getskill;


    public bool bossFightIsActive;      //正在打boss
    public bool bossHasBeenAwakened;    //woke the boss/watched cutscene but died during fight
    public bool bossHasBeenDefeated;    //boss被幹掉

    ///Audio
    private AudioSource currentAudio;
    public GameObject BossBGM1Audio;
    private AudioSource BossBGM1Source;
    public GameObject BossBGM2Audio;
    private AudioSource BossBGM2Source;
    public GameObject normalBGMAudio;
    private AudioSource normalBGMSource;
    
    public void ActivateDragonBossFight()
    {
        //normalBGMSource.Stop();
        //BossBGM1Source.Play();
        StartCoroutine(BGMChange(currentAudio, BossBGM1Source));
        currentAudio = BossBGM1Source;

        bossFightIsActive = true;
        bossHasBeenAwakened = true;
        dragonBossHPBar.SetUIHPBarToActive();
        //in wall(?)

        foreach (var fogWall in fogWells)
        {
            fogWall.ActivateFogWell();
        }
    }
    public void ActivateRockBossFight()
    {
        //normalBGMSource.Stop();
        //BossBGM2Source.Play();
        StartCoroutine(BGMChange(currentAudio, BossBGM2Source));
        currentAudio = BossBGM2Source;

        bossFightIsActive = true;
        bossHasBeenAwakened = true;
        rockBossHPBar.SetUIHPBarToActive();
        rockBossShieldBar.SetUIShieldBarToActive();
        //in wall(?)

        foreach (var fogWall in fogWells)
        {
            fogWall.ActivateFogWell();
        }
    }
    public void Awake()
    {
        s_Instance = this;
    }
    public void Start()
    {
        hintbox.SetActive(false);
        getskill.SetActive(false);

        BossBGM1Source = BossBGM1Audio.GetComponent<AudioSource>();
        BossBGM2Source = BossBGM2Audio.GetComponent<AudioSource>();
        normalBGMSource = normalBGMAudio.GetComponent<AudioSource>();

        currentAudio = normalBGMSource;
    }

    public void BossHasBeenDefeated(bool isDrageon)
    {
        //BossBGM1Source.Stop();
        //BossBGM2Source.Stop();
        //normalBGMSource.Play();
        if(isDrageon == true)
        {
            StartCoroutine(BGMChange(currentAudio, normalBGMSource));
            currentAudio = normalBGMSource;  
        }
        else if(isDrageon == false)
        {
            currentAudio.Stop();  
        }            

        bossFightIsActive = false;
        dragonBossHPBar.SetHPBarToInActive();
        rockBossHPBar.SetHPBarToInActive();
        rockBossShieldBar.SetShieldBarToInActive();

        if (isDrageon == true)
        {
            hintbox.SetActive(true);
            hide.SetActive(false);
            ShowGetSkillUI();

            var player = GameManager.Instance.m_Player.GetComponent<PlayerControl>();
            player.UnlockSkill(3);  
        }
        //out wall(?)
        foreach (var fogWall in fogWells)
        {
            fogWall.DeactivteFoWell();
        }
    }
    protected IEnumerator BGMChange(AudioSource stopAudio,AudioSource startAudio)
    {       
        while(!Mathf.Approximately(stopAudio.volume,0f))
        {
            stopAudio.volume = Mathf.MoveTowards(stopAudio.volume, 0f, 0.2f*Time.deltaTime);
            yield return null;
        }
        stopAudio.volume = 0f;
        stopAudio.Stop();

        startAudio.volume = 0f;
        startAudio.Play();
        while (!Mathf.Approximately(startAudio.volume, 0.4f))
        {
            startAudio.volume = Mathf.MoveTowards(startAudio.volume, 0.4f, 0.2f * Time.deltaTime);
            yield return null;
        }
        startAudio.volume = 0.4f;
    }
    public void ShowGetSkillUI()
    {
        getskill.SetActive(true);
    }
}
