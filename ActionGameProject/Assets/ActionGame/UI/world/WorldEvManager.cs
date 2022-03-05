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
    public GameObject BattleBGMAudio;
    private AudioSource BattleBGMSource;
    public void ActivateDragonBossFight()
    {
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
        BattleBGMSource = BattleBGMAudio.GetComponent<AudioSource>();
    }

    public void BossHasBeenDefeated()
    {
        BossBGM1Source.Stop();
        BossBGM2Source.Stop();

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
}
