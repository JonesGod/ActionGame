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

    public bool bossFightIsActive;      //正在打boss
    public bool bossHasBeenAwakened;    //woke the boss/watched cutscene but died during fight
    public bool bossHasBeenDefeated;    //boss被幹掉

    ///Audio
    public GameObject BossBGM1Audio;
    private AudioSource BossBGM1Source;
    public GameObject BossBGM2Audio;
    private AudioSource BossBGM2Source;
    public GameObject normalBGMAudio;
    private AudioSource normalBGMSource;
    public void ActivateDragonBossFight()
    {
        normalBGMSource.Stop();
        BossBGM1Source.Play();

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
        normalBGMSource.Stop();
        BossBGM2Source.Play();

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

    public void Start()
    {
        hintbox.SetActive(false);

        BossBGM1Source = BossBGM1Audio.GetComponent<AudioSource>();
        BossBGM2Source = BossBGM2Audio.GetComponent<AudioSource>();
        normalBGMSource = normalBGMAudio.GetComponent<AudioSource>();
    }

    public void BossHasBeenDefeated()
    {
        BossBGM1Source.Stop();
        BossBGM2Source.Stop();
        normalBGMSource.Play();
        //BGMStop(BossBGM1Source);
        //BGMStop(BossBGM2Source);
        var player=GameManager.Instance.m_Player.GetComponent<PlayerControl>();
        player.UnlockSkill(2);

        bossHasBeenDefeated = true;
        bossFightIsActive = false;
        dragonBossHPBar.SetHPBarToInActive();
        rockBossHPBar.SetHPBarToInActive();
        rockBossShieldBar.SetShieldBarToInActive();

        if (bossHasBeenDefeated == true)
        {
            hintbox.SetActive(true);
            hide.SetActive(false);
        }

        //out wall(?)
        foreach (var fogWall in fogWells)
        {
            fogWall.DeactivteFoWell();
        }
    }
    void BGMStop(AudioSource audio)
    {
        audio.volume = Mathf.Lerp(audio.volume,0f,0.1f);
        if(Mathf.Approximately(audio.volume,0f))
        {
            audio.volume = 0f;
        }
    }
}
