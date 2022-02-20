using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WorldEvManager : MonoBehaviour
    {
        public static WorldEvManager Instance
        {
            get { return s_Instance; }
        }
        protected static WorldEvManager s_Instance;

        public DragonBossHealthBar dragonBossHPBar;

        public List<FogWell> fogWells;
        //public UIBossHPBar bossHPBar;
        //public EnemyBossManager boss;


        public bool bossFightIsActive;      //正在打boss
        public bool bossHasBeenAwakened;    //woke the boss/watched cutscene but died during fight
        public bool bossHasBeenDefeated;    //boss被幹掉

        public void ActivateBossFight()
        {
            bossFightIsActive = true;
            bossHasBeenAwakened = true;
            dragonBossHPBar.SetUIHPBarToActive();
            //in wall(?)

            foreach (var fogWall in fogWells)
            {
                fogWall.ActivateFogWell();
            }
        }

        public void BossHasBeenDefeated()
        {
            bossHasBeenDefeated = true;
            bossFightIsActive = false;
            dragonBossHPBar.SetHPBarToInActive();

            //out wall(?)
            foreach (var fogWall in fogWells)
            {
                fogWall.DeactivteFoWell();
            }
        }

    }
}