using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WorldEvManager : MonoBehaviour
    {
        //Wall
        public DragonBossHealthBar dragonBossHPBar;
        EnemyBossManager boss;

        public bool bossFightIsActive;      //正在打boss
        public bool bossHasBeenAwakened;    //woke the boss/watched cutscene but died during fight
        public bool bossHasBeenDefeated;    //boss被幹掉

        public void ActivateBossFight()
        {
            bossFightIsActive = true;
            bossHasBeenAwakened = true;
            dragonBossHPBar.SetUIHPBarToActive();
            //in wall(?)
        }

        public void BossHasBeenDefeated()
        {
            bossHasBeenDefeated = true;
            bossFightIsActive = false;

            //out wall(?)
        }

    }
}