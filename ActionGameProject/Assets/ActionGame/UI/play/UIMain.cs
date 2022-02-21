using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    private static UIMain mInstance = null;
    public static UIMain Instance() { return mInstance; }

    public Image m_HpBar;
    public Image m_MpBar;

    public GameObject m_PlayerObject;
    public GameObject skillWindow;


    private void Awake()
    {
        mInstance = this;
    }
       

    //更新血條
    public void UpdateHpBar(float fValue)
    {
        
        m_HpBar.fillAmount = fValue;
    }
    //更新魔槽
    public void UpdateMpBar(float fValue)
    {
        
        m_MpBar.fillAmount = fValue;
    }
    public void ToggleGroupUpdate(ToggleGroup tg)
    {
        Toggle t = tg.GetFirstActiveToggle();
        if(t != null)
        {
            Debug.Log(t.name + ": On");
        }
    }
    /// <summary>
    /// 開關技能視窗
    /// </summary>
    public bool OpenSkillWindow()
    {
        if(skillWindow.activeSelf==false)
        {
            skillWindow.SetActive(true);
            Cursor.visible = true;
            return true;
        }
        else
        {
            skillWindow.SetActive(false);
            Cursor.visible = false;
            return false;
        }
    }
    public void SwordSkillUnLock()
    {
        GameObject swordSkillLock = skillWindow.transform.GetChild(4).gameObject;
        swordSkillLock.SetActive(false);
    }
    public void BowUnlock()
    {
        GameObject BowLock = skillWindow.transform.GetChild(5).gameObject;
        BowLock.SetActive(false);
    }
    public void ExplodeArrowUnlock()
    {
        GameObject ArrowLock = skillWindow.transform.GetChild(6).gameObject;
        ArrowLock.SetActive(false);
    }
   
}
