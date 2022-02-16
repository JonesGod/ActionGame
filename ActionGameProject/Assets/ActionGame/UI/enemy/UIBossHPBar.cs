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

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            bossName = GetComponentInChildren<Text>();
        }

        private void Start()
        {
            SetHPBerToInactive();
        }


        public void SetBossName(string name) 
        {
            bossName.text = name;
        }

        public void SetUIHPBarToActive()
        {
            slider.gameObject.SetActive(true);
        }

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
