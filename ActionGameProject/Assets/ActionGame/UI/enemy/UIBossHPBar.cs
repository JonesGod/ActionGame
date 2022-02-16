using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SG
{
    public class UIBossHPBar : MonoBehaviour
    {
        public Text bossName;
        public Slider slider;


        //設定要顯示的Boss血條跟名子
        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            bossName = GetComponentInChildren<Text>();
        }

        private void Start()
        {
            SetHPBerToInactive();
        }

        //亮出名子
        public void SetBossName(string name) 
        {
            bossName.text = name;
        }
        //亮出名子打開
        public void SetUIHPBarToActive()
        {
            slider.gameObject.SetActive(true);
        }
        //亮出名子關閉
        public void SetHPBerToInactive()
        {
            slider.gameObject.SetActive(false);
        }

        public void SerBossMaxHP(int maxHP)
        {
            slider.maxValue = maxHP;
            slider.value = maxHP;
        }


        public void SetBossCurrentHP(int currentHP)
        {
            slider.value = currentHP;
        }
    }
}
