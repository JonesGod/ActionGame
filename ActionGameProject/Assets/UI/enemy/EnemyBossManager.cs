using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
    public class EnemyBossManager : MonoBehaviour
    {
        public string bossName;

        UIBossHPBar bossHPBar;
        //

        private void Awake()
        {
            bossHPBar = FindObjectOfType<UIBossHPBar>();
            //
        }

        private void Start()
        {
            bossHPBar.SetBossName(bossName);
            //bossHPBar.SetBossCurrentHP(*.maxHP);
        }

        
    }
}