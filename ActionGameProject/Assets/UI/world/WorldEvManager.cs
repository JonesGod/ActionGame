using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WorldEvManager : MonoBehaviour
    {
        //Wall
        UIBossHPBar bossHPBar;
        EnemyBossManager boss;

        public bool bossFightIsAcyive;      //正在打boss
        public bool bossHasBeenAwakened;    //woke the boss/watched cutscene but died during fight
        public bool bossHasBeenDefeated;    //boss被幹掉

        private void Awake()
        {
            bossHPBar = FindObjectOfType<UIBossHPBar>();
        }

        public void ActivateBossFight()
        {
            bossFightIsAcyive = true;
            bossHasBeenAwakened = true;
            bossHPBar.SetUIHPBarToActive();
            //in wall(?)
        }

        public void BossHasBeenDefeated()
        {
            bossHasBeenDefeated = true;
            bossFightIsAcyive = false;

            //out wall(?)
        }

    }
}