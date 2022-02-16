using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
    public class EnemyBossManager : MonoBehaviour
    {
        public string bossName;

        UIBossHPBar bossHPBar;
        EamyStats eamyStats;


        //Boss����������������Ʈw
        private void Awake()
        {
            bossHPBar = FindObjectOfType<UIBossHPBar>();
            eamyStats = GetComponent<EamyStats>();
        }

        //Boss�������W�l
        private void Start()
        {
            bossHPBar.SetBossName(bossName);
            bossHPBar.SetBossCurrentHP(eamyStats.maxHp);
        }

        
    }
}