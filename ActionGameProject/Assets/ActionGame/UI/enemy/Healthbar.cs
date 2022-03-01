using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Healthbar : MonoBehaviour
{

    [SerializeField]
    private Image foregroundImage;
    [SerializeField]
    private Health health;

    private void Awake()
    {
        health.OnHealthPctChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(float pct)
    {
        foregroundImage.enabled = true;
        foregroundImage.fillAmount = pct;        
    }
    
    private void Update()
    {  
        transform.LookAt(transform.position + FolowCamera.Instance.transform.rotation * Vector3.back, FolowCamera.Instance.transform.rotation * Vector3.up);
    }
}
