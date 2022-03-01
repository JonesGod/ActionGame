using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockBossShieldBar : MonoBehaviour
{
    [SerializeField]
    private Image foregroundImage;

    [SerializeField]
    private Shield shield;

    [SerializeField]
    private Image hpBarBackground;

    private void Awake()
    {
        shield.OnHealthPctChanged += HandleHealthChanged;
    }
    void Start()
    {
        SetShieldBarToInActive();
    }
    public void SetUIShieldBarToActive()
    {
        hpBarBackground.gameObject.SetActive(true);
    }

    public void SetShieldBarToInActive()
    {
        hpBarBackground.gameObject.SetActive(false);
    }

    private void HandleHealthChanged(float pct)
    {
        foregroundImage.fillAmount = pct;
    }
    
}