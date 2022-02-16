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


        //Boss的血條取於對應的資料庫
        private void Awake()
        {
            bossHPBar = FindObjectOfType<UIBossHPBar>();
            eamyStats = GetComponent<EamyStats>();
        }

        //Boss的血條跟名子
        private void Start()
        {
            bossHPBar.SetBossName(bossName);
            bossHPBar.SetBossCurrentHP(eamyStats.maxHp);
        }

        
    }
}