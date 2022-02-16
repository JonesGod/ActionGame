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


        //�]�w�n��ܪ�Boss�����W�l
        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            bossName = GetComponentInChildren<Text>();
        }

        private void Start()
        {
            SetHPBerToInactive();
        }

        //�G�X�W�l
        public void SetBossName(string name) 
        {
            bossName.text = name;
        }
        //�G�X�W�l���}
        public void SetUIHPBarToActive()
        {
            slider.gameObject.SetActive(true);
        }
        //�G�X�W�l����
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
