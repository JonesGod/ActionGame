using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockBossHPBar : MonoBehaviour
{
    [SerializeField]
    private Image foregroundImage;

    [SerializeField]
    private Text bossName;

    [SerializeField]
    private Health health;

    [SerializeField]
    private Image hpBarBackground;

    private void Awake()
    {
        health.OnHealthPctChanged += HandleHealthChanged;
    }
    void Start()
    {
        SetHPBarToInActive();
    }
    public void SetUIHPBarToActive()
    {
        hpBarBackground.gameObject.SetActive(true);
    }

    public void SetHPBarToInActive()
    {
        hpBarBackground.gameObject.SetActive(false);
    }

    private void HandleHealthChanged(float pct)
    {
        foregroundImage.fillAmount = pct;
    }
    
}