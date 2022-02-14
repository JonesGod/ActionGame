using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SG
{
    public class UIBossHPBar : MonoBehaviour
    {
        public Text bossName;
        Slider slider;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        private void Start()
        {
            SetHPBerToInactive();
        }


        public void SetBossName(string name) 
        {
            bossName.text = name;
            bossName = GetComponentInChildren<Text>();
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
